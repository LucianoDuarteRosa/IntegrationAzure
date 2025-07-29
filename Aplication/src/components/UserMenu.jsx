import { useState } from 'react';
import {
    Avatar,
    Menu,
    MenuItem,
    Divider,
    IconButton,
    ListItemIcon,
    ListItemText,
    Typography,
    Box
} from '@mui/material';
import {
    Person as PersonIcon,
    Edit as EditIcon,
    VpnKey as VpnKeyIcon,
    Palette as PaletteIcon,
    Logout as LogoutIcon
} from '@mui/icons-material';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';
import { fileUploadService } from '../services';
import { useTheme } from '../contexts/ThemeContext';

export function UserMenu({ onEditProfile, onChangePassword }) {
    const { user, logout } = useAuth();
    const { toggleColorMode, mode } = useTheme();
    const navigate = useNavigate();
    const [anchorEl, setAnchorEl] = useState(null);
    const [imageError, setImageError] = useState(false);
    const open = Boolean(anchorEl);

    const handleClick = (event) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    const handleEditProfile = () => {
        handleClose();
        onEditProfile();
    };

    const handleChangePassword = () => {
        handleClose();
        onChangePassword();
    };

    const handleToggleTheme = () => {
        // Não fechar o menu ao trocar tema
        toggleColorMode();
    };

    const handleLogout = () => {
        handleClose();
        logout();
        navigate('/');
    };

    const getImageUrl = () => {
        if (imageError) return null; // Se houve erro, não tenta carregar

        const imagePath = user?.profileImagePath || user?.ProfileImagePath;
        if (!imagePath) return null;

        const url = fileUploadService.getImageUrl(imagePath);
        return url;
    };

    const handleImageError = () => {
        setImageError(true);
    };

    const handleImageLoad = () => {
        setImageError(false);
    };

    const getUserName = () => {
        return user?.name || user?.Name || 'Usuário';
    };

    const getUserNickname = () => {
        return user?.nickname || user?.Nickname || '';
    };

    return (
        <>
            <IconButton
                onClick={handleClick}
                size="small"
                sx={{ ml: 2 }}
                aria-controls={open ? 'user-menu' : undefined}
                aria-haspopup="true"
                aria-expanded={open ? 'true' : undefined}
            >
                <Avatar
                    src={getImageUrl()}
                    sx={{ width: 32, height: 32 }}
                    onError={handleImageError}
                    onLoad={handleImageLoad}
                >
                    <PersonIcon />
                </Avatar>
            </IconButton>
            <Menu
                anchorEl={anchorEl}
                id="user-menu"
                open={open}
                onClose={handleClose}
                PaperProps={{
                    elevation: 0,
                    sx: {
                        overflow: 'visible',
                        filter: 'drop-shadow(0px 2px 8px rgba(0,0,0,0.32))',
                        mt: 1.5,
                        minWidth: 220,
                        '& .MuiAvatar-root': {
                            width: 32,
                            height: 32,
                            ml: -0.5,
                            mr: 1,
                        },
                        '&:before': {
                            content: '""',
                            display: 'block',
                            position: 'absolute',
                            top: 0,
                            right: 14,
                            width: 10,
                            height: 10,
                            bgcolor: 'background.paper',
                            transform: 'translateY(-50%) rotate(45deg)',
                            zIndex: 0,
                        },
                    },
                }}
                transformOrigin={{ horizontal: 'right', vertical: 'top' }}
                anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
            >
                {/* Header do usuário */}
                <Box sx={{ px: 2, py: 1, borderBottom: '1px solid', borderColor: 'divider' }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <Avatar
                            src={getImageUrl()}
                            sx={{ width: 40, height: 40 }}
                            onError={handleImageError}
                            onLoad={handleImageLoad}
                        >
                            <PersonIcon />
                        </Avatar>
                        <Box>
                            <Typography variant="subtitle2" fontWeight="medium">
                                {getUserName()}
                            </Typography>
                            {getUserNickname() && (
                                <Typography variant="body2" color="text.secondary">
                                    @{getUserNickname()}
                                </Typography>
                            )}
                        </Box>
                    </Box>
                </Box>

                {/* Opções de perfil */}
                <MenuItem onClick={handleEditProfile}>
                    <ListItemIcon>
                        <EditIcon fontSize="small" />
                    </ListItemIcon>
                    <ListItemText>Editar Perfil</ListItemText>
                </MenuItem>

                <MenuItem onClick={handleChangePassword}>
                    <ListItemIcon>
                        <VpnKeyIcon fontSize="small" />
                    </ListItemIcon>
                    <ListItemText>Alterar Senha</ListItemText>
                </MenuItem>

                <Divider />

                {/* Opções do sistema */}
                <MenuItem onClick={handleToggleTheme}>
                    <ListItemIcon>
                        <PaletteIcon fontSize="small" />
                    </ListItemIcon>
                    <ListItemText>
                        Tema {mode === 'dark' ? 'Claro' : 'Escuro'}
                    </ListItemText>
                </MenuItem>

                <MenuItem onClick={handleLogout}>
                    <ListItemIcon>
                        <LogoutIcon fontSize="small" />
                    </ListItemIcon>
                    <ListItemText>Sair</ListItemText>
                </MenuItem>
            </Menu>
        </>
    );
}
