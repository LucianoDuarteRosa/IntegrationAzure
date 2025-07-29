import { useState } from 'react';
import { AppBar, Box, Toolbar, Button } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { IntegrationLogo } from './IntegrationLogo';
import { UserMenu } from './UserMenu';
import { ProfileModal } from './ProfileModal';
import { ChangePasswordModal } from './ChangePasswordModal';
import { useAuth } from '../contexts/AuthContext';
import { userService } from '../services';
import { useNotifications } from '../hooks/useNotifications';

export function Navbar() {
    const { user } = useAuth();
    const { showSuccess, showError } = useNotifications();
    const navigate = useNavigate();
    const [profileModalOpen, setProfileModalOpen] = useState(false);
    const [passwordModalOpen, setPasswordModalOpen] = useState(false);

    const handleLogoClick = () => {
        navigate('/dashboard');
    };

    const handleEditProfile = () => {
        setProfileModalOpen(true);
    };

    const handleChangePassword = () => {
        setPasswordModalOpen(true);
    };

    const handleSavePassword = async (passwordData) => {
        try {
            const userId = user.id || user.Id;
            const response = await userService.changePassword(userId, passwordData);

            if (response && response.success === true) {
                showSuccess('Senha alterada!', 'Sua senha foi alterada com sucesso!');
                setPasswordModalOpen(false);
            } else {
                showError('Erro ao alterar senha', response?.message || 'Erro ao alterar senha');
            }
        } catch (error) {
            console.error('Erro ao alterar senha:', error);
            showError('Erro', 'Erro ao conectar com o servidor');
        }
    };

    const canChangePasswordWithoutCurrent = () => {
        // Para o próprio usuário sempre precisa da senha atual
        return false;
    };

    return (
        <>
            <AppBar position="fixed">
                <Toolbar>
                    <Box sx={{ flexGrow: 1, display: 'flex', alignItems: 'center' }}>
                        <Button
                            onClick={handleLogoClick}
                            sx={{
                                p: 0,
                                textTransform: 'none',
                                '&:hover': {
                                    backgroundColor: 'transparent',
                                    opacity: 0.8
                                },
                                '&:focus': {
                                    outline: 'none',
                                    boxShadow: 'none'
                                },
                                '&:active': {
                                    outline: 'none',
                                    boxShadow: 'none'
                                }
                            }}
                        >
                            <IntegrationLogo size="small" isNavbar={true} />
                        </Button>
                    </Box>
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                        <UserMenu
                            onEditProfile={handleEditProfile}
                            onChangePassword={handleChangePassword}
                        />
                    </Box>
                </Toolbar>
            </AppBar>

            {/* Modal de Edição de Perfil */}
            <ProfileModal
                open={profileModalOpen}
                onClose={() => setProfileModalOpen(false)}
            />

            {/* Modal de Alteração de Senha */}
            <ChangePasswordModal
                open={passwordModalOpen}
                onClose={() => setPasswordModalOpen(false)}
                onSave={handleSavePassword}
                userName={user?.name || user?.Name}
                isAdminChange={canChangePasswordWithoutCurrent()}
            />
        </>
    );
}
