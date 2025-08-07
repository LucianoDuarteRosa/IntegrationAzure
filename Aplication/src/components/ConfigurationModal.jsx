import { useState, useEffect } from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    Button,
    Box,
    Grid,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Switch,
    FormControlLabel,
    Typography,
    Divider,
    Alert
} from '@mui/material';

export function ConfigurationModal({
    open,
    onClose,
    onSave,
    editingConfig = null,
    loading = false
}) {
    const [formData, setFormData] = useState({
        key: '',
        value: '',
        description: '',
        category: 'Azure',
        isSecret: false,
        isActive: true
    });

    const [azureConfigs, setAzureConfigs] = useState([]);
    const isAzureSetup = editingConfig?.azureSetup;

    useEffect(() => {
        if (isAzureSetup && editingConfig?.configs) {
            setAzureConfigs(editingConfig.configs.map(config => ({
                ...config,
                value: config.value || ''
            })));
        } else if (editingConfig && !isAzureSetup) {
            setFormData({
                key: editingConfig.Key || editingConfig.key || '',
                value: (editingConfig.Value || editingConfig.value) === '*****' ? '' : (editingConfig.Value || editingConfig.value || ''),
                description: editingConfig.Description || editingConfig.description || '',
                category: editingConfig.Category || editingConfig.category || 'Azure',
                isSecret: editingConfig.IsSecret ?? editingConfig.isSecret ?? false,
                isActive: editingConfig.IsActive ?? editingConfig.isActive ?? true
            });
        } else if (!isAzureSetup) {
            setFormData({
                key: '',
                value: '',
                description: '',
                category: 'Azure',
                isSecret: false,
                isActive: true
            });
        }
    }, [editingConfig, open, isAzureSetup]);

    const handleClose = () => {
        if (isAzureSetup) {
            setAzureConfigs([]);
        } else {
            setFormData({
                key: '',
                value: '',
                description: '',
                category: 'Azure',
                isSecret: false,
                isActive: true
            });
        }
        onClose();
    };

    const handleSave = () => {
        if (isAzureSetup) {
            onSave({ azureConfigs });
        } else {
            onSave(formData);
        }
    };

    const handleChange = (field, value) => {
        setFormData(prev => ({
            ...prev,
            [field]: value
        }));
    };

    const handleAzureConfigChange = (index, field, value) => {
        setAzureConfigs(prev => prev.map((config, i) =>
            i === index ? { ...config, [field]: value } : config
        ));
    };

    return (
        <Dialog open={open} onClose={handleClose} maxWidth="md">
            <DialogTitle>
                {isAzureSetup ? 'Configurar Azure DevOps' : (editingConfig ? 'Editar Configuração' : 'Nova Configuração')}
            </DialogTitle>
            <DialogContent>
                <Box sx={{ pt: 1 }}>
                    {isAzureSetup ? (
                        <>
                            <Alert severity="info" sx={{ mb: 3 }}>
                                Configure as credenciais do Azure DevOps para integração com projetos e work items.
                            </Alert>

                            {azureConfigs.map((config, index) => (
                                <Box key={config.key} sx={{ mb: 3 }}>
                                    <Typography variant="h6" gutterBottom>
                                        {config.key.replace('_', ' ')}
                                    </Typography>
                                    <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                                        {config.description}
                                    </Typography>

                                    <Grid container spacing={2}>
                                        <Grid item xs={12}>
                                            <TextField
                                                label={config.key === 'Azure_Token' ? 'Token de Acesso Pessoal' :
                                                    config.key === 'Organizacao' ? 'Nome da Organização' : 'Versão da API'}
                                                value={config.value}
                                                onChange={(e) => handleAzureConfigChange(index, 'value', e.target.value)}
                                                fullWidth
                                                required
                                                type={config.isSecret ? 'password' : 'text'}
                                                placeholder={
                                                    config.key === 'Azure_Token' ? 'Seu token do Azure DevOps' :
                                                        config.key === 'Organizacao' ? 'nome-da-organizacao' :
                                                            '7.0'
                                                }
                                                helperText={
                                                    config.key === 'Azure_Token' ? 'Token de acesso pessoal com permissões de leitura/escrita' :
                                                        config.key === 'Organizacao' ? 'Nome da organização como aparece na URL do Azure DevOps' :
                                                            'Versão da API REST do Azure DevOps (padrão: 7.0)'
                                                }
                                            />
                                        </Grid>
                                    </Grid>

                                    {index < azureConfigs.length - 1 && <Divider sx={{ mt: 2 }} />}
                                </Box>
                            ))}
                        </>
                    ) : (
                        <>
                            {/* Primeira linha: Chave, Valor e Categoria */}
                            <Grid container spacing={2} sx={{ mb: 2 }}>
                                <Grid item xs={12} sm={4}>
                                    <TextField
                                        fullWidth
                                        label="Chave"
                                        value={formData.key}
                                        onChange={(e) => handleChange('key', e.target.value)}
                                        disabled={editingConfig}
                                        required
                                    />
                                </Grid>
                                <Grid item xs={12} sm={5}>
                                    <TextField
                                        fullWidth
                                        label="Valor"
                                        value={formData.value}
                                        onChange={(e) => handleChange('value', e.target.value)}
                                        type={formData.isSecret ? 'password' : 'text'}
                                        required
                                        placeholder={editingConfig?.IsSecret ? 'Digite para alterar...' : ''}
                                    />
                                </Grid>
                                <Grid item xs={12} sm={3}>
                                    <FormControl fullWidth sx={{ minWidth: '150px' }}>
                                        <InputLabel>Categoria</InputLabel>
                                        <Select
                                            value={formData.category}
                                            onChange={(e) => handleChange('category', e.target.value)}
                                            label="Categoria"
                                        >
                                            <MenuItem value="Azure">Azure</MenuItem>
                                            <MenuItem value="General">Geral</MenuItem>
                                        </Select>
                                    </FormControl>
                                </Grid>
                            </Grid>

                            {/* Segunda linha: Descrição */}
                            <TextField
                                fullWidth
                                label="Descrição"
                                value={formData.description}
                                onChange={(e) => handleChange('description', e.target.value)}
                                multiline
                                rows={2}
                                sx={{ mb: 2 }}
                            />

                            {/* Terceira linha: Switches */}
                            <Box sx={{ pt: 1 }}>
                                <FormControlLabel
                                    control={
                                        <Switch
                                            checked={formData.isSecret}
                                            onChange={(e) => handleChange('isSecret', e.target.checked)}
                                        />
                                    }
                                    label="Valor secreto"
                                />
                                <FormControlLabel
                                    control={
                                        <Switch
                                            checked={formData.isActive}
                                            onChange={(e) => handleChange('isActive', e.target.checked)}
                                        />
                                    }
                                    label="Ativo"
                                    sx={{ ml: 2 }}
                                />
                            </Box>
                        </>
                    )}
                </Box>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose} disabled={loading}>
                    Cancelar
                </Button>
                <Button
                    onClick={handleSave}
                    variant="contained"
                    disabled={loading || (isAzureSetup ? azureConfigs.some(c => !c.value) : (!formData.key || !formData.value))}
                >
                    {isAzureSetup ? 'Configurar Azure' : (editingConfig ? 'Atualizar' : 'Criar')}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
