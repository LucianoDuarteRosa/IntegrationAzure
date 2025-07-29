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
import { profileService } from '../services';

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
                setFormData({
                    name: user.Name || user.name || '',
                    nickname: user.Nickname || user.nickname || '',
                    email: user.Email || user.email || '',
                    password: '',
                    confirmPassword: '',
                    profileImagePath: user.ProfileImagePath || user.profileImagePath || '',
                    profileId: (user.Profile || user.profile)?.Id || (user.Profile || user.profile)?.id || ''
                });

                // Para usuários existentes, limpar preview (imagem será carregada do src do Avatar)
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
        }
    }, [open, isEditing, user]);

    const loadProfiles = async () => {
        try {
            const response = await profileService.getActiveProfiles();
            // Verifica tanto 'success' quanto 'Success' (case-insensitive)
            const isSuccess = response && (response.success === true || response.Success === true);

            if (isSuccess) {
                let availableProfiles = response.Data || response.data || [];

                // Filtrar perfis baseado no perfil do usuário atual
                if (currentUserProfile === 'Administrador') {
                    // Administrador pode atribuir qualquer perfil exceto Desenvolvedor
                    availableProfiles = availableProfiles.filter(p => (p.Name || p.name) !== 'Desenvolvedor');
                } else if (currentUserProfile === 'Desenvolvedor') {
                    // Desenvolvedor pode atribuir qualquer perfil
                    // Não filtrar nada
                } else {
                    // Usuário comum só pode ver seu próprio perfil
                    availableProfiles = availableProfiles.filter(p => (p.Name || p.name) === 'Usuário');
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

    const handleImageChange = (event) => {
        const file = event.target.files[0];
        if (file) {
            setSelectedImage(file);

            // Criar preview da imagem
            const reader = new FileReader();
            reader.onload = (e) => {
                setImagePreview(e.target.result);
            };
            reader.readAsDataURL(file);

            // Atualizar o formData com o nome do arquivo
            setFormData(prev => ({
                ...prev,
                profileImagePath: file.name
            }));

            // Limpar erro do campo se existir
            if (errors.profileImagePath) {
                setErrors(prev => ({
                    ...prev,
                    profileImagePath: null
                }));
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
        onClose();
    };

    const getProfileName = (profileId) => {
        const profile = profiles.find(p => (p.Id || p.id) === profileId);
        return profile ? (profile.Name || profile.name) : '';
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
                        {/* Avatar/Imagem */}
                        <Grid item xs={12} sx={{ textAlign: 'center', mb: 2 }}>
                            <Avatar
                                sx={{
                                    width: 80,
                                    height: 80,
                                    mx: 'auto',
                                    mb: 2,
                                    bgcolor: 'primary.main'
                                }}
                                src={imagePreview || formData.profileImagePath}
                            >
                                <PersonIcon sx={{ fontSize: 40 }} />
                            </Avatar>

                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                <TextField
                                    fullWidth
                                    label="Imagem de Perfil"
                                    value={formData.profileImagePath}
                                    placeholder="Nenhum arquivo selecionado"
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
                                />
                                <label htmlFor="image-upload">
                                    <Button
                                        variant="outlined"
                                        component="span"
                                        startIcon={<PhotoCameraIcon />}
                                        size="small"
                                    >
                                        Buscar
                                    </Button>
                                </label>
                            </Box>
                        </Grid>

                        <Grid item xs={12}>
                            <Divider />
                        </Grid>

                        {/* Dados Pessoais */}
                        <Grid item xs={12} md={6}>
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

                        <Grid item xs={12} md={6}>
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

                        <Grid item xs={12}>
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

                        {/* Perfil */}
                        <Grid item xs={12}>
                            <FormControl fullWidth error={!!errors.profileId} required>
                                <InputLabel>Perfil</InputLabel>
                                <Select
                                    value={formData.profileId}
                                    onChange={handleInputChange('profileId')}
                                    label="Perfil"
                                >
                                    {profiles.map((profile) => (
                                        <MenuItem key={profile.Id || profile.id} value={profile.Id || profile.id}>
                                            {profile.Name || profile.name}
                                            {(profile.Description || profile.description) && (
                                                <Typography variant="caption" sx={{ ml: 1, color: 'text.secondary' }}>
                                                    - {profile.Description || profile.description}
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
                        </Grid>

                        {/* Senha - apenas para novos usuários */}
                        {!isEditing && (
                            <>
                                <Grid item xs={12}>
                                    <Divider />
                                    <Typography variant="subtitle2" sx={{ mt: 1, mb: 1 }}>
                                        Senha de Acesso
                                    </Typography>
                                </Grid>

                                <Grid item xs={12} md={6}>
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
                                </Grid>

                                <Grid item xs={12} md={6}>
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
                                </Grid>
                            </>
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
