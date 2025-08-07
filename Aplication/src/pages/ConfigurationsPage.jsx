import { useState, useEffect } from 'react';
import {
    Container,
    Paper,
    Typography,
    TextField,
    Button,
    Box,
    Grid,
    Card,
    CardContent,
    CardActions,
    Chip,
    IconButton,
    Alert
} from '@mui/material';
import {
    Settings as SettingsIcon,
    Add as AddIcon,
    Edit as EditIcon,
    Delete as DeleteIcon,
    Visibility as VisibilityIcon,
    VisibilityOff as VisibilityOffIcon,
    Cloud as CloudIcon
} from '@mui/icons-material';
import { Navbar } from '../components/Navbar';
import { ConfigurationModal } from '../components/ConfigurationModal';
import { configurationService } from '../services';
import { useNotifications } from '../hooks/useNotifications';

export function ConfigurationsPage() {
    const { showSuccess, showError } = useNotifications();
    const [configurations, setConfigurations] = useState([]);
    const [loading, setLoading] = useState(true);
    const [openModal, setOpenModal] = useState(false);
    const [editingConfig, setEditingConfig] = useState(null);
    const [visibleSecrets, setVisibleSecrets] = useState({});

    useEffect(() => {
        loadConfigurations();
    }, []);

    const loadConfigurations = async () => {
        try {
            setLoading(true);
            const response = await configurationService.getAll();
            if (response.success) {
                setConfigurations(response.data || []);
            } else {
                showError('Erro ao carregar', 'Erro ao carregar configurações');
            }
        } catch {
            showError('Erro de conexão', 'Erro ao conectar com o servidor');
        } finally {
            setLoading(false);
        }
    };

    const handleOpenModal = (config = null) => {
        setEditingConfig(config);
        setOpenModal(true);
    };

    const handleCloseModal = () => {
        setOpenModal(false);
        setEditingConfig(null);
    };

    const handleSaveConfiguration = async (formData) => {
        try {
            // Se for setup do Azure, processar múltiplas configurações
            if (formData.azureConfigs) {
                let allSuccess = true;
                const failedConfigs = [];

                for (const config of formData.azureConfigs) {
                    if (!config.value) {
                        showError('Dados obrigatórios', `${config.key} é obrigatório`);
                        return;
                    }

                    try {
                        const response = await configurationService.create({
                            key: config.key,
                            value: config.value,
                            description: config.description,
                            category: config.category,
                            isSecret: config.isSecret,
                            isActive: config.isActive
                        });

                        if (!response.success) {
                            allSuccess = false;
                            failedConfigs.push(config.key);
                        }
                    } catch (error) {
                        allSuccess = false;
                        failedConfigs.push(config.key);
                        console.error(`Erro ao criar configuração ${config.key}:`, error);
                    }
                }

                if (allSuccess) {
                    showSuccess('Azure DevOps configurado!', 'Todas as configurações do Azure DevOps foram criadas com sucesso!');
                } else if (failedConfigs.length > 0) {
                    showError('Erro parcial', `Erro ao criar: ${failedConfigs.join(', ')}`);
                } else {
                    showError('Erro', 'Erro ao configurar Azure DevOps');
                }

                handleCloseModal();
                loadConfigurations();
                return;
            }

            // Configuração individual
            if (!formData.key || !formData.value) {
                showError('Dados obrigatórios', 'Chave e valor são obrigatórios');
                return;
            }

            if (editingConfig && !editingConfig.azureSetup) {
                const response = await configurationService.update(editingConfig.Id || editingConfig.id, {
                    Value: formData.value,
                    Description: formData.description,
                    Category: formData.category,
                    IsSecret: formData.isSecret,
                    IsActive: formData.isActive
                });
                if (response.success) {
                    showSuccess('Configuração atualizada!', 'Configuração atualizada com sucesso!');
                }
            } else {
                const response = await configurationService.create(formData);
                if (response.success) {
                    showSuccess('Configuração criada!', 'Configuração criada com sucesso!');
                }
            }

            handleCloseModal();
            loadConfigurations();
        } catch (error) {
            showError('Erro ao salvar', error.response?.data?.message || 'Erro ao salvar configuração');
        }
    };

    const handleQuickAzureSetup = () => {
        setEditingConfig({
            azureSetup: true,
            configs: [
                {
                    key: 'Azure_Token',
                    value: '',
                    description: 'Token de acesso pessoal do Azure DevOps',
                    category: 'Azure',
                    isSecret: true,
                    isActive: true
                },
                {
                    key: 'Organizacao',
                    value: '',
                    description: 'Nome da organização no Azure DevOps',
                    category: 'Azure',
                    isSecret: false,
                    isActive: true
                },
                {
                    key: 'Versao_API',
                    value: '7.0',
                    description: 'Versão da API do Azure DevOps',
                    category: 'Azure',
                    isSecret: false,
                    isActive: true
                }
            ]
        });
        setOpenModal(true);
    };

    const handleDelete = async (config) => {
        if (window.confirm(`Tem certeza que deseja excluir a configuração "${config.Key || config.key || 'Esta configuração'}"?`)) {
            try {
                const response = await configurationService.delete(config.Id || config.id);
                if (response.success) {
                    showSuccess('Configuração excluída!', 'Configuração excluída com sucesso!');
                    loadConfigurations();
                }
            } catch {
                showError('Erro ao excluir', 'Erro ao excluir configuração');
            }
        }
    };

    const toggleSecretVisibility = (configId) => {
        setVisibleSecrets(prev => ({
            ...prev,
            [configId]: !prev[configId]
        }));
    };

    const groupedConfigs = configurations.reduce((groups, config) => {
        const category = config.Category || config.category || 'Outros';
        if (!groups[category]) {
            groups[category] = [];
        }
        groups[category].push(config);
        return groups;
    }, {});

    return (
        <>
            <Navbar />
            <Container
                component="main"
                maxWidth={false}
                sx={{
                    display: 'flex',
                    width: '99vw',
                    minHeight: 'calc(98vh - 64px)',
                    alignItems: 'flex-start',
                    justifyContent: 'center',
                    mt: 10,
                    pt: 2,
                    pb: 4,
                    px: 2
                }}
            >
                <Box
                    sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                        width: '100%',
                        maxWidth: '1400px'
                    }}
                >
                    <Box sx={{ mb: 3, textAlign: 'center' }}>
                        <Typography variant="h4" component="h1" gutterBottom
                            sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 1 }}>
                            <SettingsIcon sx={{ fontSize: '2.5rem' }} />
                            Configurações do Sistema
                        </Typography>
                    </Box>

                    {/* Ações principais */}
                    <Paper elevation={3} sx={{ p: 3, mb: 3, width: '100%', maxWidth: '1200px' }}>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                            <Typography variant="h6">Ações Rápidas</Typography>
                            <Box sx={{ display: 'flex', gap: 2 }}>
                                <Button
                                    variant="outlined"
                                    startIcon={<CloudIcon />}
                                    onClick={() => handleQuickAzureSetup()}
                                    color="primary"
                                >
                                    Configurar Azure DevOps
                                </Button>
                                <Button
                                    variant="contained"
                                    startIcon={<AddIcon />}
                                    onClick={() => handleOpenModal()}
                                >
                                    Nova Configuração
                                </Button>
                            </Box>
                        </Box>

                        <Alert severity="info" sx={{ mt: 2 }}>
                            <strong>Configurações necessárias para funcionamento do Azure DevOps:</strong><br />
                            • <strong>Azure_Token:</strong> Token de acesso pessoal do Azure DevOps (necessário para autenticação)<br />
                            • <strong>Organizacao:</strong> Nome da organização no Azure DevOps (define qual organização acessar)<br />
                            • <strong>Versao_api:</strong> Versão da API REST do Azure DevOps (padrão: 7.0, define compatibilidade)
                        </Alert>
                    </Paper>

                    {/* Lista de configurações por categoria */}
                    {Object.entries(groupedConfigs).map(([category, configs]) => (
                        <Paper key={category} elevation={3} sx={{ p: 3, mb: 3, width: '100%', maxWidth: '1200px' }}>
                            <Typography variant="h6" sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
                                <CloudIcon color="primary" />
                                {category}
                                <Chip label={configs.length} size="small" />
                            </Typography>

                            <Grid container spacing={2}>
                                {configs.map((config) => (
                                    <Grid item xs={12} md={6} lg={4} key={config.Id || config.id}>
                                        <Card variant="outlined" sx={{ height: '100%' }}>
                                            <CardContent>
                                                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
                                                    <Typography variant="subtitle1" fontWeight="bold" sx={{
                                                        wordBreak: 'break-word',
                                                        flex: 1,
                                                        mr: 1
                                                    }}>
                                                        {config.Key || config.key || 'Sem chave'}
                                                    </Typography>
                                                    <Box sx={{ display: 'flex', gap: 0.5 }}>
                                                        {(config.IsSecret || config.isSecret) && (
                                                            <Chip label="Secreto" size="small" color="warning" />
                                                        )}
                                                        {!(config.IsActive ?? config.isActive ?? true) && (
                                                            <Chip label="Inativo" size="small" color="error" />
                                                        )}
                                                    </Box>
                                                </Box>

                                                <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                                                    {config.Description || config.description || 'Sem descrição'}
                                                </Typography>

                                                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                                    <TextField
                                                        size="small"
                                                        value={(config.IsSecret || config.isSecret) && !visibleSecrets[config.Id || config.id] ? '*****' : (config.Value || config.value || '')}
                                                        type={(config.IsSecret || config.isSecret) && !visibleSecrets[config.Id || config.id] ? 'password' : 'text'}
                                                        fullWidth
                                                        InputProps={{ readOnly: true }}
                                                        sx={{ '& .MuiInputBase-input': { fontSize: '0.875rem' } }}
                                                    />
                                                    {(config.IsSecret || config.isSecret) && (
                                                        <IconButton
                                                            size="small"
                                                            onClick={() => toggleSecretVisibility(config.Id || config.id)}
                                                        >
                                                            {visibleSecrets[config.Id || config.id] ? <VisibilityOffIcon /> : <VisibilityIcon />}
                                                        </IconButton>
                                                    )}
                                                </Box>
                                            </CardContent>

                                            <CardActions sx={{ justifyContent: 'flex-end', pt: 0 }}>
                                                <IconButton
                                                    size="small"
                                                    onClick={() => handleOpenModal(config)}
                                                    color="primary"
                                                >
                                                    <EditIcon fontSize="small" />
                                                </IconButton>
                                                <IconButton
                                                    size="small"
                                                    onClick={() => handleDelete(config)}
                                                    color="error"
                                                >
                                                    <DeleteIcon fontSize="small" />
                                                </IconButton>
                                            </CardActions>
                                        </Card>
                                    </Grid>
                                ))}
                            </Grid>
                        </Paper>
                    ))}

                    {configurations.length === 0 && !loading && (
                        <Paper elevation={3} sx={{ p: 6, width: '100%', maxWidth: '1200px', textAlign: 'center' }}>
                            <SettingsIcon sx={{ fontSize: '4rem', color: 'text.secondary', mb: 2 }} />
                            <Typography variant="h6" color="text.secondary" gutterBottom>
                                Nenhuma configuração encontrada
                            </Typography>
                            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
                                Configure as credenciais do Azure DevOps para começar a usar a integração.
                                URLs e configurações de ambiente estão no appsettings.json da API.
                            </Typography>
                            <Button
                                variant="contained"
                                startIcon={<AddIcon />}
                                onClick={() => handleOpenModal()}
                                size="large"
                            >
                                Nova Configuração
                            </Button>
                        </Paper>
                    )}
                </Box>
            </Container>

            <ConfigurationModal
                open={openModal}
                onClose={handleCloseModal}
                onSave={handleSaveConfiguration}
                editingConfig={editingConfig}
                loading={loading}
            />
        </>
    );
}
