import { useState, useEffect } from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    Button,
    Box,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Grid,
    Typography,
    Alert,
    InputAdornment,
    IconButton,
    Divider,
    Avatar
} from '@mui/material';
import {
    Visibility as VisibilityIcon,
    VisibilityOff as VisibilityOffIcon,
    Person as PersonIcon,
    Edit as EditIcon,
    Add as AddIcon,
    PhotoCamera as PhotoCameraIcon
} from '@mui/icons-material';
import { profileService, fileUploadService } from '../services';

export function UserModal({
    open,
    onClose,
    onSave,
    user = null,
    currentUserProfile = 'Usuário'
}) {
    const isEditing = !!user;
    const [loading, setLoading] = useState(false);
    const [profiles, setProfiles] = useState([]);
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const [errors, setErrors] = useState({});
    const [selectedImage, setSelectedImage] = useState(null);
    const [imagePreview, setImagePreview] = useState('');
    const [uploadingImage, setUploadingImage] = useState(false);

    const [formData, setFormData] = useState({
        name: '',
        nickname: '',
        email: '',
        password: '',
        confirmPassword: '',
        profileImagePath: '',
        profileId: ''
    });

    // Carregar perfis e inicializar formulário
    useEffect(() => {
        if (open) {
            loadProfiles();
            if (isEditing && user) {
                const imagePath = user.profileImagePath || user.ProfileImagePath;
                setFormData({
                    name: user.name || user.Name || '',
                    nickname: user.nickname || user.Nickname || '',
                    email: user.email || user.Email || '',
                    password: '',
                    confirmPassword: '',
                    profileImagePath: imagePath || '',
                    profileId: (user.profile || user.Profile)?.id || (user.profile || user.Profile)?.Id || ''
                });

                // Para usuários existentes, não mostrar preview local
                setSelectedImage(null);
                setImagePreview('');
            } else {
                setFormData({
                    name: '',
                    nickname: '',
                    email: '',
                    password: '',
                    confirmPassword: '',
                    profileImagePath: '',
                    profileId: ''
                });

                // Para novos usuários, limpar tudo
                setSelectedImage(null);
                setImagePreview('');
            }
            setErrors({});
            setUploadingImage(false);
        }
    }, [open, isEditing, user]);

    const loadProfiles = async () => {
        try {
            const response = await profileService.getActiveProfiles();
            // Usando camelCase (configurado no backend)
            const isSuccess = response && response.success === true;

            if (isSuccess) {
                let availableProfiles = response.data || [];

                // Filtrar perfis baseado no perfil do usuário atual
                if (currentUserProfile === 'Administrador') {
                    // Administrador pode atribuir qualquer perfil exceto Desenvolvedor
                    availableProfiles = availableProfiles.filter(p => p.name !== 'Desenvolvedor');
                } else if (currentUserProfile === 'Desenvolvedor') {
                    // Desenvolvedor pode atribuir qualquer perfil
                    // Não filtrar nada
                } else {
                    // Usuário comum só pode ver seu próprio perfil
                    availableProfiles = availableProfiles.filter(p => p.name === 'Usuário');
                }

                setProfiles(availableProfiles);
            }
        } catch (error) {
            console.error('Erro ao carregar perfis:', error);
        }
    };

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

                    // Atualizar o formData com o caminho da imagem
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

        if (!isEditing) {
            if (!formData.password) {
                newErrors.password = 'Senha é obrigatória';
            } else if (formData.password.length < 6) {
                newErrors.password = 'Senha deve ter pelo menos 6 caracteres';
            } else if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$/.test(formData.password)) {
                newErrors.password = 'Senha deve conter pelo menos uma letra minúscula, uma maiúscula e um número';
            }

            if (formData.password !== formData.confirmPassword) {
                newErrors.confirmPassword = 'Confirmação de senha não confere';
            }
        }

        if (!formData.profileId) {
            newErrors.profileId = 'Perfil é obrigatório';
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
                profileId: formData.profileId
            };

            if (!isEditing) {
                userData.password = formData.password;
            }

            await onSave(userData);

            // Limpar estados de imagem após salvar
            setSelectedImage(null);
            setImagePreview('');

            onClose();
        } catch (error) {
            console.error('Erro ao salvar usuário:', error);
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

    const getProfileName = (profileId) => {
        const profile = profiles.find(p => p.id === profileId);
        return profile ? profile.name : '';
    };

    return (
        <Dialog
            open={open}
            onClose={handleClose}
            maxWidth="md"
            fullWidth
            PaperProps={{
                sx: { borderRadius: 2 }
            }}
        >
            <DialogTitle sx={{ pb: 1 }}>
                <Box display="flex" alignItems="center" gap={1}>
                    {isEditing ? <EditIcon /> : <AddIcon />}
                    <Typography variant="h6">
                        {isEditing ? 'Editar Usuário' : 'Novo Usuário'}
                    </Typography>
                </Box>
            </DialogTitle>

            <DialogContent>
                <Box sx={{ pt: 1 }}>
                    <Grid container spacing={3}>
                        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', height: '100%', justifyContent: 'center' }}>
                            <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', height: '100%', justifyContent: 'center', marginBottom: 1 }}>
                                <Avatar
                                    sx={{
                                        width: 80,
                                        height: 80,
                                        bgcolor: 'primary.main'
                                    }}
                                    src={
                                        imagePreview ||
                                        (formData.profileImagePath ? fileUploadService.getImageUrl(formData.profileImagePath) : null)
                                    }
                                >
                                    <PersonIcon sx={{ fontSize: 40 }} />
                                </Avatar>
                            </Box>
                            <Box>
                                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, height: '100%' }}>
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
                        </Box>

                        <Box sx={{ marginLeft: 5 }}>
                            <Box sx={{ display: 'flex', flexDirection: 'row', gap: 2, marginBottom: 2 }}>
                                <Grid sx={{ width: '100%' }}>
                                    <TextField
                                        fullWidth
                                        label="Nome Completo"
                                        value={formData.name}
                                        onChange={handleInputChange('name')}
                                        error={!!errors.name}
                                        helperText={errors.name}
                                        required
                                    />
                                </Grid>

                                <Grid sx={{ width: '100%' }}>
                                    <TextField
                                        fullWidth
                                        label="Nickname"
                                        value={formData.nickname}
                                        onChange={handleInputChange('nickname')}
                                        error={!!errors.nickname}
                                        helperText={errors.nickname}
                                        required
                                    />
                                </Grid>
                            </Box>
                            <Grid>
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
                            </Grid>
                        </Box>

                        <Box sx={{ width: '100%' }}>
                            <FormControl fullWidth error={!!errors.profileId} required>
                                <InputLabel>Perfil</InputLabel>
                                <Select
                                    value={formData.profileId}
                                    onChange={handleInputChange('profileId')}
                                    label="Perfil"
                                >
                                    {profiles.map((profile) => (
                                        <MenuItem key={profile.id} value={profile.id}>
                                            {profile.name}
                                            {profile.description && (
                                                <Typography variant="caption" sx={{ ml: 1, color: 'text.secondary' }}>
                                                    - {profile.description}
                                                </Typography>
                                            )}
                                        </MenuItem>
                                    ))}
                                </Select>
                                {errors.profileId && (
                                    <Typography variant="caption" color="error" sx={{ mt: 0.5, ml: 2 }}>
                                        {errors.profileId}
                                    </Typography>
                                )}
                            </FormControl>
                        </Box>


                        {!isEditing && (
                            <Box sx={{ width: '100%', display: 'flex', flexDirection: 'row', gap: 2 }}>
                                <TextField
                                    fullWidth
                                    label="Senha"
                                    type={showPassword ? 'text' : 'password'}
                                    value={formData.password}
                                    onChange={handleInputChange('password')}
                                    error={!!errors.password}
                                    helperText={errors.password}
                                    required
                                    InputProps={{
                                        endAdornment: (
                                            <InputAdornment position="end">
                                                <IconButton
                                                    onClick={() => setShowPassword(!showPassword)}
                                                    edge="end"
                                                >
                                                    {showPassword ? <VisibilityOffIcon /> : <VisibilityIcon />}
                                                </IconButton>
                                            </InputAdornment>
                                        )
                                    }}
                                />

                                <TextField
                                    fullWidth
                                    label="Confirmar Senha"
                                    type={showConfirmPassword ? 'text' : 'password'}
                                    value={formData.confirmPassword}
                                    onChange={handleInputChange('confirmPassword')}
                                    error={!!errors.confirmPassword}
                                    helperText={errors.confirmPassword}
                                    required
                                    InputProps={{
                                        endAdornment: (
                                            <InputAdornment position="end">
                                                <IconButton
                                                    onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                                                    edge="end"
                                                >
                                                    {showConfirmPassword ? <VisibilityOffIcon /> : <VisibilityIcon />}
                                                </IconButton>
                                            </InputAdornment>
                                        )
                                    }}
                                />
                            </Box>
                        )}

                        {/* Informações para edição */}
                        {isEditing && (
                            <Grid item xs={12}>
                                <Alert severity="info">
                                    Para alterar a senha, use a funcionalidade específica de alteração de senha.
                                </Alert>
                            </Grid>
                        )}
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
                    {loading ? 'Salvando...' : (isEditing ? 'Atualizar' : 'Criar')}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
