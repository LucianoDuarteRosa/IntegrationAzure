import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useNotifications } from '../hooks/useNotifications';
import { IntegrationLogo } from '../components/IntegrationLogo';
import {
    Container,
    Box,
    TextField,
    Button,
    Typography,
    Paper,
    CircularProgress
} from '@mui/material';

export function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();
    const { login } = useAuth();
    const { error } = useNotifications();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setIsLoading(true);

        try {
            const result = await login(email, password);

            if (result.success) {
                navigate('/dashboard');
            } else {
                error.validation(result.errors || [result.message || 'Credenciais inválidas']);
            }
        } catch (error) {
            error.connection();
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <>
            <Container
                component="main"
                maxWidth={false}
                sx={{
                    display: 'flex',
                    width: '100vw',
                    height: '100vh',
                    alignItems: 'center',
                    justifyContent: 'center'
                }}
            >
                <Box
                    sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                        maxWidth: '400px',
                        width: '100%'
                    }}
                >
                    {/* Logo acima do formulário */}
                    <Box sx={{ mb: 4, display: 'flex', justifyContent: 'center' }}>
                        <IntegrationLogo size="large" variant="vertical" isLoginPage={true} />
                    </Box>

                    <Paper elevation={3} sx={{ p: 4, width: '100%' }}>
                        <Typography component="h1" variant="h5" align="center" sx={{ mb: 3 }}>
                            Login
                        </Typography>
                        <Box component="form" onSubmit={handleSubmit} sx={{ mt: 1 }}>
                            <TextField
                                margin="normal"
                                required
                                fullWidth
                                id="email"
                                label="Email"
                                name="email"
                                autoComplete="email"
                                autoFocus
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                            />
                            <TextField
                                margin="normal"
                                required
                                fullWidth
                                name="password"
                                label="Senha"
                                type="password"
                                id="password"
                                autoComplete="current-password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                            />
                            <Button
                                type="submit"
                                fullWidth
                                variant="contained"
                                disabled={isLoading}
                                sx={{ mt: 3, mb: 2 }}
                            >
                                {isLoading ? (
                                    <>
                                        <CircularProgress size={20} sx={{ mr: 1 }} />
                                        Entrando...
                                    </>
                                ) : (
                                    'Entrar'
                                )}
                            </Button>
                        </Box>
                    </Paper>
                </Box>
            </Container>
        </>
    );
}
