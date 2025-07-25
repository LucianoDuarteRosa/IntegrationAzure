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
    Drawer,
    List,
    ListItem,
    ListItemIcon,
    ListItemText,
    ListItemButton,
    useTheme,
    useMediaQuery,
} from '@mui/material';
import {
    ExitToApp as LogoutIcon,
    Menu as MenuIcon,
    AddCircleOutline as AddIcon,
    FormatListBulleted as ListIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';

export function Dashboard() {
    const [showStoryForm, setShowStoryForm] = useState(false);
    const [mobileOpen, setMobileOpen] = useState(false);
    const { logout } = useAuth();
    const navigate = useNavigate();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    const handleDrawerToggle = () => {
        setMobileOpen(!mobileOpen);
    };

    const handleLogout = () => {
        logout();
        navigate('/');
    };

    const menuItems = [
        {
            text: 'Nova História',
            icon: <AddIcon />,
            onClick: () => {
                setShowStoryForm(true);
                if (isMobile) setMobileOpen(false);
            }
        },
        {
            text: 'Lista de Histórias',
            icon: <ListIcon />,
            onClick: () => {
                setShowStoryForm(false);
                if (isMobile) setMobileOpen(false);
            }
        }
    ];

    const drawer = (
        <Box sx={{ width: 250 }}>
            <List>
                {menuItems.map((item) => (
                    <ListItem key={item.text} disablePadding>
                        <ListItemButton onClick={item.onClick}>
                            <ListItemIcon>
                                {item.icon}
                            </ListItemIcon>
                            <ListItemText primary={item.text} />
                        </ListItemButton>
                    </ListItem>
                ))}
            </List>
        </Box>
    );

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
