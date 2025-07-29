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
    Switch,
    FormControlLabel,
    Chip,
    IconButton,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Divider,
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
import { configurationService } from '../services';
import { useNotifications } from '../hooks/useNotifications';

export function ConfigurationsPage() {
    const { showSuccess, showError } = useNotifications();
    const [configurations, setConfigurations] = useState([]);
    const [loading, setLoading] = useState(true);
    const [openDialog, setOpenDialog] = useState(false);
    const [editingConfig, setEditingConfig] = useState(null);
    const [visibleSecrets, setVisibleSecrets] = useState({});
    const [formData, setFormData] = useState({
        key: '',
        value: '',
        description: '',
        category: 'Azure',
        isSecret: false,
        isActive: true
    });

    useEffect(() => {
        loadConfigurations();
    }, []);

    const loadConfigurations = async () => {
        try {
            setLoading(true);
            const response = await configurationService.getAll();
            if (response.Success) {
                setConfigurations(response.Data || []);
            } else {
                showError('Erro ao carregar configurações');
            }
        } catch {
            showError('Erro ao conectar com o servidor');
        } finally {
            setLoading(false);
        }
    };

    const handleOpenDialog = (config = null) => {
        if (config) {
            setEditingConfig(config);
            setFormData({
                key: config.Key,
                value: config.Value === '*****' ? '' : config.Value,
                description: config.Description,
                category: config.Category,
                isSecret: config.IsSecret,
                isActive: config.IsActive
            });
        } else {
            setEditingConfig(null);
            setFormData({
                key: '',
                value: '',
                description: '',
                category: 'Azure',
                isSecret: false,
                isActive: true
            });
        }
        setOpenDialog(true);
    };

    const handleCloseDialog = () => {
        setOpenDialog(false);
        setEditingConfig(null);
        setFormData({
            key: '',
            value: '',
            description: '',
            category: 'Azure',
            isSecret: false,
            isActive: true
        });
    };

    const handleSave = async () => {
        try {
            if (!formData.key || !formData.value) {
                showError('Chave e valor são obrigatórios');
                return;
            }

            if (editingConfig) {
                const response = await configurationService.update(editingConfig.Id, {
                    Value: formData.value,
                    Description: formData.description,
                    Category: formData.category,
                    IsSecret: formData.isSecret,
                    IsActive: formData.isActive
                });
                if (response.Success) {
                    showSuccess('Configuração atualizada com sucesso!');
                }
            } else {
                const response = await configurationService.create(formData);
                if (response.Success) {
                    showSuccess('Configuração criada com sucesso!');
                }
            }

            handleCloseDialog();
            loadConfigurations();
        } catch (error) {
            showError(error.response?.data?.message || 'Erro ao salvar configuração');
        }
    };

    const handleDelete = async (config) => {
        if (window.confirm(`Tem certeza que deseja excluir a configuração "${config.Key}"?`)) {
            try {
                const response = await configurationService.delete(config.Id);
                if (response.Success) {
                    showSuccess('Configuração excluída com sucesso!');
                    loadConfigurations();
                }
            } catch {
                showError('Erro ao excluir configuração');
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
        const category = config.Category || 'Outros';
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
                                    variant="contained"
                                    startIcon={<AddIcon />}
                                    onClick={() => handleOpenDialog()}
                                >
                                    Nova Configuração
                                </Button>
                            </Box>
                        </Box>

                        <Alert severity="info" sx={{ mt: 2 }}>
                            Configure aqui apenas as <strong>credenciais sensíveis</strong> para a integração com o Azure DevOps.
                            As configurações de URL, versão da API e endpoint estão no appsettings.json da API.
                            O nome do projeto será definido em cada história de usuário conforme a demanda.
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
                                    <Grid item xs={12} md={6} lg={4} key={config.Id}>
                                        <Card variant="outlined" sx={{ height: '100%' }}>
                                            <CardContent>
                                                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
                                                    <Typography variant="subtitle1" fontWeight="bold" sx={{
                                                        wordBreak: 'break-word',
                                                        flex: 1,
                                                        mr: 1
                                                    }}>
                                                        {config.Key}
                                                    </Typography>
                                                    <Box sx={{ display: 'flex', gap: 0.5 }}>
                                                        {config.IsSecret && (
                                                            <Chip label="Secreto" size="small" color="warning" />
                                                        )}
                                                        {!config.IsActive && (
                                                            <Chip label="Inativo" size="small" color="error" />
                                                        )}
                                                    </Box>
                                                </Box>

                                                <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                                                    {config.Description || 'Sem descrição'}
                                                </Typography>

                                                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                                    <TextField
                                                        size="small"
                                                        value={config.IsSecret && !visibleSecrets[config.Id] ? '*****' : config.Value}
                                                        type={config.IsSecret && !visibleSecrets[config.Id] ? 'password' : 'text'}
                                                        fullWidth
                                                        InputProps={{ readOnly: true }}
                                                        sx={{ '& .MuiInputBase-input': { fontSize: '0.875rem' } }}
                                                    />
                                                    {config.IsSecret && (
                                                        <IconButton
                                                            size="small"
                                                            onClick={() => toggleSecretVisibility(config.Id)}
                                                        >
                                                            {visibleSecrets[config.Id] ? <VisibilityOffIcon /> : <VisibilityIcon />}
                                                        </IconButton>
                                                    )}
                                                </Box>
                                            </CardContent>

                                            <CardActions sx={{ justifyContent: 'flex-end', pt: 0 }}>
                                                <IconButton
                                                    size="small"
                                                    onClick={() => handleOpenDialog(config)}
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
                                onClick={() => handleOpenDialog()}
                                size="large"
                            >
                                Nova Configuração
                            </Button>
                        </Paper>
                    )}
                </Box>
            </Container>

            {/* Dialog para criar/editar configuração */}
            <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
                <DialogTitle>
                    {editingConfig ? 'Editar Configuração' : 'Nova Configuração'}
                </DialogTitle>
                <DialogContent>
                    <Box sx={{ pt: 1 }}>
                        <Grid container spacing={2}>
                            <Grid item xs={12}>
                                <TextField
                                    fullWidth
                                    label="Chave"
                                    value={formData.key}
                                    onChange={(e) => setFormData({ ...formData, key: e.target.value })}
                                    disabled={editingConfig} // Não permite editar a chave
                                    required
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    fullWidth
                                    label="Valor"
                                    value={formData.value}
                                    onChange={(e) => setFormData({ ...formData, value: e.target.value })}
                                    type={formData.isSecret ? 'password' : 'text'}
                                    required
                                    placeholder={editingConfig?.IsSecret ? 'Digite para alterar...' : ''}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    fullWidth
                                    label="Descrição"
                                    value={formData.description}
                                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                                    multiline
                                    rows={2}
                                />
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <FormControl fullWidth>
                                    <InputLabel>Categoria</InputLabel>
                                    <Select
                                        value={formData.category}
                                        onChange={(e) => setFormData({ ...formData, category: e.target.value })}
                                        label="Categoria"
                                    >
                                        <MenuItem value="Azure">Azure</MenuItem>
                                        <MenuItem value="Database">Database</MenuItem>
                                        <MenuItem value="Email">Email</MenuItem>
                                        <MenuItem value="General">General</MenuItem>
                                    </Select>
                                </FormControl>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Box sx={{ pt: 1 }}>
                                    <FormControlLabel
                                        control={
                                            <Switch
                                                checked={formData.isSecret}
                                                onChange={(e) => setFormData({ ...formData, isSecret: e.target.checked })}
                                            />
                                        }
                                        label="Valor secreto"
                                    />
                                    <FormControlLabel
                                        control={
                                            <Switch
                                                checked={formData.isActive}
                                                onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                                            />
                                        }
                                        label="Ativo"
                                        sx={{ ml: 2 }}
                                    />
                                </Box>
                            </Grid>
                        </Grid>
                    </Box>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseDialog}>Cancelar</Button>
                    <Button onClick={handleSave} variant="contained">
                        {editingConfig ? 'Atualizar' : 'Criar'}
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
}
