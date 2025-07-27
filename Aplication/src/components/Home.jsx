import { Box, Container, Grid } from '@mui/material';
import { Navbar } from './Navbar';
import { useNavigate } from 'react-router-dom';
import { MenuCard } from './MenuCard';
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
    ];

    return (
        <>
            <Navbar />
            <Container
                component="main"
                maxWidth={false}
                sx={{
                    display: 'flex',
                    width: '100vw',
                    height: 'calc(100vh - 64px)', // altura total menos navbar
                    alignItems: 'center',
                    justifyContent: 'center',
                    mt: 8 // margin-top para compensar a navbar
                }}
            >
                <Box
                    sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                        width: '100%',
                        maxWidth: '1000px'
                    }}
                >
                    <Grid
                        container
                        justifyContent="center"
                        alignItems="center"
                        sx={{
                            display: 'grid',
                            gridTemplateColumns: 'repeat(3, 1fr)',
                            gap: 3,
                            width: '100%',
                            '@media (max-width: 900px)': {
                                gridTemplateColumns: 'repeat(2, 1fr)'
                            },
                            '@media (max-width: 600px)': {
                                gridTemplateColumns: '1fr'
                            }
                        }}
                    >
                        {menuItems.map((item) => (
                            <Box key={item.path}>
                                <MenuCard
                                    icon={item.icon}
                                    title={item.title}
                                    onClick={() => handleCardClick(item.path)}
                                />
                            </Box>
                        ))}
                    </Grid>
                </Box>
            </Container>
        </>
    );
}
