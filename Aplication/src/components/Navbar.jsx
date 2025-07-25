import { AppBar, Box, Toolbar, Typography, IconButton } from '@mui/material';
import { Logout as LogoutIcon } from '@mui/icons-material';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';

export function Navbar() {
    const { logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/');
    };

    return (
        <AppBar position="fixed">
            <Toolbar>
                <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                    Azure DevOps Stories
                </Typography>
                <IconButton
                    color="inherit"
                    onClick={handleLogout}
                    title="Sair"
                >
                    <LogoutIcon />
                </IconButton>
            </Toolbar>
        </AppBar>
    );
}
