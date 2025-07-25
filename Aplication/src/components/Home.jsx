import { Box, Container, Grid } from '@mui/material';
import { Navbar } from './Navbar';
import { useNavigate } from 'react-router-dom';
import { MenuCard } from './MenuCard';
import { useLastSection } from '../hooks/useLastSection';
import { useEffect } from 'react';
import {
    LibraryBooks as StoryIcon,
    BugReport as IssueIcon,
    Error as FalhaIcon,
    People as UsersIcon,
    Settings as ConfigIcon,
    Assessment as LogsIcon
} from '@mui/icons-material';
export function Home() {
    const navigate = useNavigate();
    const { setLastSection, getLastSection } = useLastSection();

    // Removido o redirecionamento automático para manter o usuário na home

    const handleCardClick = (path) => {
        navigate(path);
    };

    const menuItems = [
        {
            icon: StoryIcon,
            title: 'Nova História',
            path: '/nova-historia'
        },
        {
            icon: IssueIcon,
            title: 'Nova Issue',
            path: '/nova-issue'
        },
        {
            icon: FalhaIcon,
            title: 'Nova Falha',
            path: '/nova-falha'
        },
        {
            icon: UsersIcon,
            title: 'Usuários',
            path: '/usuarios'
        },
        {
            icon: ConfigIcon,
            title: 'Configurações',
            path: '/configuracoes'
        },
        {
            icon: LogsIcon,
            title: 'Logs',
            path: '/logs'
        }
    ]; return (
        <>
            <Navbar />
            <Box
                sx={{
                    minHeight: '100vh',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    pt: 8 // para compensar a navbar
                }}
            >
                <Container maxWidth="md">
                    <Grid
                        container
                        spacing={4}
                        justifyContent="center"
                        alignItems="center"
                    >
                        {menuItems.map((item) => (
                            <Grid item key={item.path} xs={12} sm={6} md={4}>
                                <Box sx={{ display: 'flex', justifyContent: 'center' }}>
                                    <MenuCard
                                        icon={item.icon}
                                        title={item.title}
                                        onClick={() => handleCardClick(item.path)}
                                    />
                                </Box>
                            </Grid>
                        ))}
                    </Grid>
                </Container>
            </Box>
        </>
    );
}
