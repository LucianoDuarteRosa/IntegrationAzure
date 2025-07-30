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
    FormControlLabel
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

    useEffect(() => {
        if (editingConfig) {
            setFormData({
                key: editingConfig.Key || editingConfig.key || '',
                value: (editingConfig.Value || editingConfig.value) === '*****' ? '' : (editingConfig.Value || editingConfig.value || ''),
                description: editingConfig.Description || editingConfig.description || '',
                category: editingConfig.Category || editingConfig.category || 'Azure',
                isSecret: editingConfig.IsSecret ?? editingConfig.isSecret ?? false,
                isActive: editingConfig.IsActive ?? editingConfig.isActive ?? true
            });
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
    }, [editingConfig, open]);

    const handleClose = () => {
        setFormData({
            key: '',
            value: '',
            description: '',
            category: 'Azure',
            isSecret: false,
            isActive: true
        });
        onClose();
    };

    const handleSave = () => {
        onSave(formData);
    };

    const handleChange = (field, value) => {
        setFormData(prev => ({
            ...prev,
            [field]: value
        }));
    };

    return (
        <Dialog open={open} onClose={handleClose} maxWidth="md">
            <DialogTitle>
                {editingConfig ? 'Editar Configuração' : 'Nova Configuração'}
            </DialogTitle>
            <DialogContent>
                <Box sx={{ pt: 1 }}>
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
                                    <MenuItem value="Database">Database</MenuItem>
                                    <MenuItem value="Email">Email</MenuItem>
                                    <MenuItem value="General">General</MenuItem>
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
                </Box>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose} disabled={loading}>
                    Cancelar
                </Button>
                <Button
                    onClick={handleSave}
                    variant="contained"
                    disabled={loading || !formData.key || !formData.value}
                >
                    {editingConfig ? 'Atualizar' : 'Criar'}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
