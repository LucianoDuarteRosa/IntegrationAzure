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
    Typography,
    Avatar,
    Alert
} from '@mui/material';
import {
    Person as PersonIcon,
    Edit as EditIcon,
    PhotoCamera as PhotoCameraIcon
} from '@mui/icons-material';
import { useAuth } from '../contexts/AuthContext';
import { userService, fileUploadService } from '../services';
import { useNotifications } from '../hooks/useNotifications';

export function ProfileModal({ open, onClose }) {
    const { user, updateUser } = useAuth();
    const { showSuccess, showError } = useNotifications();
    const [loading, setLoading] = useState(false);
    const [errors, setErrors] = useState({});
    const [selectedImage, setSelectedImage] = useState(null);
    const [imagePreview, setImagePreview] = useState('');
    const [uploadingImage, setUploadingImage] = useState(false);

    const [formData, setFormData] = useState({
        name: '',
        nickname: '',
        email: '',
        profileImagePath: ''
    });

    // Inicializar formulário com dados do usuário
    useEffect(() => {
        if (open && user) {
            const imagePath = user.profileImagePath || user.ProfileImagePath;
            setFormData({
                name: user.name || user.Name || '',
                nickname: user.nickname || user.Nickname || '',
                email: user.email || user.Email || '',
                profileImagePath: imagePath || ''
            });

            // Limpar preview (imagem será carregada do src do Avatar)
            setSelectedImage(null);
            setImagePreview('');
            setErrors({});
            setUploadingImage(false);
        }
    }, [open, user]);

    const handleInputChange = (field) => (event) => {
        const value = event.target.value;
        setFormData(prev => ({
            ...prev,
            [field]: value
        }));

        // Limpar erro do campo quando usuário começar a digitar
        if (errors[field]) {
            setErrors(prev => ({
                ...prev,
                [field]: null
            }));
        }
    };

    const handleImageChange = async (event) => {
        const file = event.target.files[0];
        if (file) {
            // Validar arquivo
            const validation = fileUploadService.validateImageFile(file);
            if (!validation.isValid) {
                setErrors(prev => ({
                    ...prev,
                    profileImagePath: validation.errors.join(', ')
                }));
                return;
            }

            setUploadingImage(true);
            try {
                // Fazer upload da imagem
                const response = await fileUploadService.uploadUserProfileImage(file);

                if (response.success) {
                    const imagePath = response.data;

                    // Atualizar o formData com o nome do arquivo
                    setFormData(prev => ({
                        ...prev,
                        profileImagePath: imagePath
                    }));

                    // Criar preview da imagem usando a URL da API
                    const imageUrl = fileUploadService.getImageUrl(imagePath);
                    setImagePreview(imageUrl);
                    setSelectedImage(file);

                    // Limpar erro se existir
                    if (errors.profileImagePath) {
                        setErrors(prev => ({
                            ...prev,
                            profileImagePath: null
                        }));
                    }
                } else {
                    setErrors(prev => ({
                        ...prev,
                        profileImagePath: response.message || 'Erro ao fazer upload da imagem'
                    }));
                }
            } catch (error) {
                console.error('Erro ao fazer upload da imagem:', error);
                setErrors(prev => ({
                    ...prev,
                    profileImagePath: 'Erro ao fazer upload da imagem. Tente novamente.'
                }));
            } finally {
                setUploadingImage(false);
            }
        }
    };

    const validateForm = () => {
        const newErrors = {};

        if (!formData.name.trim()) {
            newErrors.name = 'Nome é obrigatório';
        }

        if (!formData.nickname.trim()) {
            newErrors.nickname = 'Nickname é obrigatório';
        } else if (!/^[a-zA-Z0-9_.-]+$/.test(formData.nickname)) {
            newErrors.nickname = 'Nickname deve conter apenas letras, números, pontos, hífens e sublinhados';
        }

        if (!formData.email.trim()) {
            newErrors.email = 'Email é obrigatório';
        } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
            newErrors.email = 'Email inválido';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSave = async () => {
        if (!validateForm()) {
            return;
        }

        setLoading(true);
        try {
            const userData = {
                name: formData.name,
                nickname: formData.nickname,
                email: formData.email,
                profileImagePath: formData.profileImagePath || null,
                profileId: user.profile?.id || user.Profile?.Id
            };

            const userId = user.id || user.Id;
            const response = await userService.updateUser(userId, userData);

            if (response && response.success === true) {
                showSuccess('Perfil atualizado!', 'Seu perfil foi atualizado com sucesso!');

                // Atualizar contexto do usuário
                const updatedUser = {
                    ...user,
                    ...response.data
                };
                updateUser(updatedUser);

                // Limpar estados e fechar modal
                setSelectedImage(null);
                setImagePreview('');
                onClose();
            } else {
                showError('Erro ao atualizar', response?.message || 'Erro ao atualizar perfil');
            }
        } catch (error) {
            console.error('Erro ao atualizar perfil:', error);
            showError('Erro', 'Erro ao conectar com o servidor');
        } finally {
            setLoading(false);
        }
    };

    const handleClose = () => {
        // Limpar estados de imagem ao fechar
        setSelectedImage(null);
        setImagePreview('');
        setUploadingImage(false);
        onClose();
    };

    const getImageUrl = () => {
        if (imagePreview) return imagePreview;
        if (formData.profileImagePath) {
            return fileUploadService.getImageUrl(formData.profileImagePath);
        }
        return null;
    };

    return (
        <Dialog
            open={open}
            onClose={handleClose}
            maxWidth="sm"
            fullWidth
            PaperProps={{
                sx: { borderRadius: 2 }
            }}
        >
            <DialogTitle sx={{ pb: 1 }}>
                <Box display="flex" alignItems="center" gap={1}>
                    <EditIcon />
                    <Typography variant="h6">
                        Editar Perfil
                    </Typography>
                </Box>
            </DialogTitle>

            <DialogContent>
                <Box sx={{ pt: 1 }}>
                    <Grid container spacing={3}>
                        {/* Avatar/Imagem */}
                        <Box item xs={12} sx={{ width: '100%', textAlign: 'center', mb: 2, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                            <Avatar
                                sx={{
                                    width: 80,
                                    height: 80,
                                    mx: 'auto',
                                    mb: 2,
                                    bgcolor: 'primary.main'
                                }}
                                src={getImageUrl()}
                            >
                                <PersonIcon sx={{ fontSize: 40 }} />
                            </Avatar>

                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                <TextField
                                    fullWidth
                                    label="Imagem de Perfil"
                                    value={formData.profileImagePath ? 'Imagem selecionada' : 'Nenhuma imagem selecionada'}
                                    placeholder="Nenhuma imagem selecionada"
                                    size="small"
                                    InputProps={{ readOnly: true }}
                                    error={!!errors.profileImagePath}
                                    helperText={errors.profileImagePath}
                                />
                                <input
                                    accept="image/*"
                                    style={{ display: 'none' }}
                                    id="image-upload"
                                    type="file"
                                    onChange={handleImageChange}
                                    disabled={uploadingImage}
                                />
                                <label htmlFor="image-upload">
                                    <Button
                                        variant="outlined"
                                        component="span"
                                        startIcon={<PhotoCameraIcon />}
                                        size="small"
                                        disabled={uploadingImage}
                                    >
                                        {uploadingImage ? 'Enviando...' : 'Buscar'}
                                    </Button>
                                </label>
                            </Box>
                        </Box>

                        {/* Dados Pessoais */}
                        <TextField
                            fullWidth
                            label="Nome Completo"
                            value={formData.name}
                            onChange={handleInputChange('name')}
                            error={!!errors.name}
                            helperText={errors.name}
                            required
                        />
                        <TextField
                            fullWidth
                            label="Nickname"
                            value={formData.nickname}
                            onChange={handleInputChange('nickname')}
                            error={!!errors.nickname}
                            helperText={errors.nickname}
                            required
                        />
                        <TextField
                            fullWidth
                            label="Email"
                            type="email"
                            value={formData.email}
                            onChange={handleInputChange('email')}
                            error={!!errors.email}
                            helperText={errors.email}
                            required
                        />

                        <Grid item xs={12}>
                            <Alert severity="info">
                                Para alterar sua senha, use a opção "Alterar Senha" no menu do usuário.
                            </Alert>
                        </Grid>
                    </Grid>
                </Box>
            </DialogContent>

            <DialogActions sx={{ p: 3, pt: 1 }}>
                <Button
                    onClick={handleClose}
                    disabled={loading}
                >
                    Cancelar
                </Button>
                <Button
                    onClick={handleSave}
                    variant="contained"
                    disabled={loading}
                >
                    {loading ? 'Salvando...' : 'Salvar'}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
