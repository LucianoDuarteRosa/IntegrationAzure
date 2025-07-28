import { AppBar, Box, Toolbar, Typography, IconButton, Button } from '@mui/material';
import { Logout as LogoutIcon } from '@mui/icons-material';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';
import { ThemeToggle } from './ThemeToggle';
import { IntegrationLogo } from './IntegrationLogo';

export function Navbar() {
    const { logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/');
    };

    const handleLogoClick = () => {
        navigate('/dashboard');
    };

    return (
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
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <ThemeToggle />
                    <IconButton
                        color="inherit"
                        onClick={handleLogout}
                        title="Sair"
                    >
                        <LogoutIcon />
                    </IconButton>
                </Box>
            </Toolbar>
        </AppBar>
    );
}
