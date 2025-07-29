import { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { UserStoryForm } from '../components/UserStoryForm';
import {
    Container,
    Box,
    Button,
    Typography,
    AppBar,
    Toolbar,
    IconButton,
} from '@mui/material';
import {
    ExitToApp as LogoutIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';

export function Dashboard() {
    const [showStoryForm, setShowStoryForm] = useState(false);
    const { logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/');
    };

    return (
        <Box sx={{
            display: 'flex',
            flexDirection: 'column',
            width: '100vw',
            height: '100vh'
        }}>
            <AppBar position="static">
                <Toolbar sx={{ width: '100%', maxWidth: '100%' }}>
                    <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                        Azure DevOps - Histórias
                    </Typography>
                    <IconButton color="inherit" onClick={handleLogout}>
                        <LogoutIcon />
                    </IconButton>
                </Toolbar>
            </AppBar>

            <Container maxWidth={false} sx={{
                flex: 1,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                width: '100%'
            }}>
                {!showStoryForm ? (
                    <Box>
                        <Button
                            variant="contained"
                            size="large"
                            onClick={() => setShowStoryForm(true)}
                        >
                            Nova História
                        </Button>
                    </Box>
                ) : (
                    <UserStoryForm />
                )}
            </Container>
        </Box>
    );
}
