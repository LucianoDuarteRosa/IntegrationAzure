import { useState, useEffect } from 'react';
import {
    Container,
    Paper,
    Typography,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Box,
    Chip,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    TextField,
    Button,
    Stack,
    CircularProgress,
    Grid,
    Avatar,
    IconButton,
    Tooltip,
    Alert,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions
} from '@mui/material';
import { useTheme } from '@mui/material/styles';
import {
    Refresh as RefreshIcon,
    FilterList as FilterIcon,
    PersonAdd as PersonAddIcon,
    Edit as EditIcon,
    VpnKey as VpnKeyIcon,
    Delete as DeleteIcon,
    Person as PersonIcon,
    AdminPanelSettings as AdminIcon,
    Build as DevIcon,
    Group as GroupIcon
} from '@mui/icons-material';
import { Navbar } from '../components/Navbar';
import { UserModal } from '../components/UserModal';
import { ChangePasswordModal } from '../components/ChangePasswordModal';
import { userService, profileService } from '../services';
import { useNotifications } from '../hooks/useNotifications';
import { useAuth } from '../contexts/AuthContext';

export function UsersPage() {
    const theme = useTheme();
    const { error, success } = useNotifications();
    const { user: currentUser } = useAuth();
    const [users, setUsers] = useState([]);
    const [profiles, setProfiles] = useState([]);
    const [loading, setLoading] = useState(true);
    const [userModalOpen, setUserModalOpen] = useState(false);
    const [passwordModalOpen, setPasswordModalOpen] = useState(false);
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [selectedUser, setSelectedUser] = useState(null);
    const [filters, setFilters] = useState({
        profileId: '',
        search: ''
    });

    // Carregar dados ao montar o componente
    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {
        await Promise.all([loadUsers(), loadProfiles()]);
    };

    const loadUsers = async () => {
        try {
            setLoading(true);
            const response = await userService.getUsers();

            // Verifica tanto 'success' quanto 'Success' (case-insensitive)
            const isSuccess = response && (response.success === true || response.Success === true);

            if (isSuccess) {
                let userData = response.Data || response.data || [];

                // Se for usuário comum, só mostra o próprio perfil
                if (currentUser?.profile?.name === 'Usuário') {
                    const currentUserEmail = currentUser.email;
                    userData = userData.filter(u => (u.Email || u.email) === currentUserEmail);
                }

                setUsers(userData);
            } else {
                error.load('usuários', [response?.message || response?.Message || 'Resposta inválida do servidor']);
            }
        } catch (err) {
            error.connection();
        } finally {
            setLoading(false);
        }
    };

    const loadProfiles = async () => {
        try {
            const response = await profileService.getActiveProfiles();

            // Verifica tanto 'success' quanto 'Success' (case-insensitive)
            const isSuccess = response && (response.success === true || response.Success === true);

            if (isSuccess) {
                const profileData = response.Data || response.data || [];
                setProfiles(profileData);
            }
        } catch (error) {
            console.error('Erro ao carregar perfis:', error);
        }
    };

    const handleFilterChange = (field, value) => {
        setFilters(prev => ({
            ...prev,
            [field]: value
        }));
    };

    const clearFilters = () => {
        setFilters({
            profileId: '',
            search: ''
        });
    };

    const getFilteredUsers = () => {
        return users.filter(user => {
            // Usar tanto PascalCase quanto camelCase para compatibilidade
            const profile = user.Profile || user.profile;
            const name = user.Name || user.name;
            const nickname = user.Nickname || user.nickname;
            const email = user.Email || user.email;

            const matchesProfile = !filters.profileId || profile?.Id === filters.profileId || profile?.id === filters.profileId;
            const matchesSearch = !filters.search ||
                name?.toLowerCase().includes(filters.search.toLowerCase()) ||
                nickname?.toLowerCase().includes(filters.search.toLowerCase()) ||
                email?.toLowerCase().includes(filters.search.toLowerCase());

            return matchesProfile && matchesSearch;
        });
    };

    const canEditUser = (user) => {
        if (!currentUser) return false;

        const currentProfile = currentUser.profile?.name;
        const userEmail = user.Email || user.email;

        // Usuário comum só pode editar o próprio perfil
        if (currentProfile === 'Usuário') {
            return userEmail === currentUser.email;
        }

        // Desenvolvedor e Administrador podem editar outros usuários
        return currentProfile === 'Desenvolvedor' || currentProfile === 'Administrador';
    };

    const canCreateUser = () => {
        if (!currentUser) return false;
        const currentProfile = currentUser.profile?.name;
        return currentProfile === 'Desenvolvedor' || currentProfile === 'Administrador';
    };

    const canDeleteUser = (user) => {
        if (!currentUser) return false;

        const currentProfile = currentUser.profile?.name;
        const userEmail = user.Email || user.email;

        // Usuário comum não pode deletar ninguém
        if (currentProfile === 'Usuário') return false;

        // Não pode deletar o próprio usuário
        if (userEmail === currentUser.email) return false;

        return currentProfile === 'Desenvolvedor' || currentProfile === 'Administrador';
    };

    const handleCreateUser = () => {
        setSelectedUser(null);
        setUserModalOpen(true);
    };

    const handleEditUser = (user) => {
        setSelectedUser(user);
        setUserModalOpen(true);
    };

    const handleChangePassword = (user) => {
        setSelectedUser(user);
        setPasswordModalOpen(true);
    };

    const handleDeleteUser = (user) => {
        setSelectedUser(user);
        setDeleteDialogOpen(true);
    };

    const handleSaveUser = async (userData) => {
        try {
            let response;

            if (selectedUser) {
                // Editar usuário existente
                const userId = selectedUser.Id || selectedUser.id;
                response = await userService.updateUser(userId, userData);
            } else {
                // Criar novo usuário
                response = await userService.createUser(userData);
            }

            const isSuccess = response && (response.success === true || response.Success === true);
            if (isSuccess) {
                success.show(response.message || response.Message || `Usuário ${selectedUser ? 'atualizado' : 'criado'} com sucesso!`);
                await loadUsers();
            } else {
                error.show(response?.message || response?.Message || 'Erro ao salvar usuário');
            }
        } catch (err) {
            error.connection();
        }
    };

    const handleSavePassword = async (passwordData) => {
        try {
            const userId = selectedUser.Id || selectedUser.id;
            const response = await userService.changePassword(userId, passwordData);

            const isSuccess = response && (response.success === true || response.Success === true);
            if (isSuccess) {
                success.show('Senha alterada com sucesso!');
            } else {
                error.show(response?.message || response?.Message || 'Erro ao alterar senha');
            }
        } catch (err) {
            error.connection();
        }
    };

    const confirmDeleteUser = async () => {
        try {
            const userId = selectedUser.Id || selectedUser.id;
            const response = await userService.deleteUser(userId);

            const isSuccess = response && (response.success === true || response.Success === true);
            if (isSuccess) {
                success.show('Usuário removido com sucesso!');
                await loadUsers();
            } else {
                error.show(response?.message || response?.Message || 'Erro ao remover usuário');
            }
        } catch (err) {
            error.connection();
        } finally {
            setDeleteDialogOpen(false);
            setSelectedUser(null);
        }
    };

    const getProfileIcon = (profileName) => {
        switch (profileName) {
            case 'Administrador':
                return <AdminIcon fontSize="small" />;
            case 'Desenvolvedor':
                return <DevIcon fontSize="small" />;
            case 'Usuário':
                return <GroupIcon fontSize="small" />;
            default:
                return <PersonIcon fontSize="small" />;
        }
    };

    const getProfileColor = (profileName) => {
        switch (profileName) {
            case 'Administrador':
                return 'error';
            case 'Desenvolvedor':
                return 'warning';
            case 'Usuário':
                return 'primary';
            default:
                return 'default';
        }
    };

    const formatDate = (dateString) => {
        return new Date(dateString).toLocaleString('pt-BR');
    };

    const filteredUsers = getFilteredUsers();

    return (
        <>
            <Navbar />
            <Container
                component="main"
                maxWidth={false}
                sx={{
                    display: 'flex',
                    width: '99vw',
                    minHeight: 'calc(98vh - 64px)',
                    alignItems: 'flex-start',
                    justifyContent: 'center',
                    mt: 10,
                    pt: 2,
                    pb: 4,
                    px: 2
                }}
            >
                <Box
                    sx={{
                        width: '100%',
                        maxWidth: '1400px',
                        display: 'flex',
                        flexDirection: 'column',
                    }}
                >
                    {/* Título */}
                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', mb: 4 }}>
                        <Typography
                            variant="h4"
                            component="h1"
                            sx={{
                                fontWeight: 'bold',
                                textAlign: 'center',
                                display: 'flex',
                                alignItems: 'center',
                                gap: 1
                            }}
                        >
                            <PersonIcon sx={{ fontSize: '2.5rem' }} />
                            Gerenciamento de Usuários
                        </Typography>
                    </Box>

                    {/* Filtros */}
                    <Paper elevation={1} sx={{ p: 3, borderRadius: 2, marginBottom: 4 }}>
                        <Box sx={{ display: 'flex', flexDirection: 'row', justifyContent: 'space-between' }}>
                            <Grid container spacing={3} alignItems="center">
                                <Grid item xs={12} md={4}>
                                    <FormControl fullWidth size="small" sx={{ minWidth: '200px' }}>
                                        <InputLabel>Filtrar por Perfil</InputLabel>
                                        <Select
                                            value={filters.profileId}
                                            onChange={(e) => handleFilterChange('profileId', e.target.value)}
                                            label="Filtrar por Perfil"
                                        >
                                            <MenuItem value="">Todos os Perfis</MenuItem>
                                            {profiles.map((profile) => (
                                                <MenuItem key={profile.Id || profile.id} value={profile.Id || profile.id}>
                                                    {profile.Name || profile.name}
                                                </MenuItem>
                                            ))}
                                        </Select>
                                    </FormControl>
                                </Grid>

                                <Grid item xs={12} md={4}>
                                    <TextField
                                        fullWidth
                                        size="small"
                                        label="Buscar usuários"
                                        placeholder="Nome, nickname ou email..."
                                        value={filters.search}
                                        onChange={(e) => handleFilterChange('search', e.target.value)}
                                    />
                                </Grid>

                                <Grid item xs={12} md={4}>
                                    <Stack direction="row" spacing={1} justifyContent="flex-end">
                                        <Button
                                            variant="outlined"
                                            startIcon={<FilterIcon />}
                                            onClick={clearFilters}
                                            size="small"
                                        >
                                            Limpar
                                        </Button>
                                        <Button
                                            variant="outlined"
                                            startIcon={<RefreshIcon />}
                                            onClick={loadUsers}
                                            size="small"
                                        >
                                            Atualizar
                                        </Button>
                                    </Stack>
                                </Grid>

                            </Grid>
                            <Box fullWidth display="flex" justifyContent="flex-end">
                                {canCreateUser() && (
                                    <Button
                                        variant="contained"
                                        startIcon={<PersonAddIcon />}
                                        onClick={handleCreateUser}
                                    >
                                        Novo Usuário
                                    </Button>
                                )}
                            </Box>
                        </Box>

                    </Paper>

                    {/* Lista de Usuários */}
                    <Paper elevation={1} sx={{ borderRadius: 2, overflow: 'hidden' }}>
                        {loading ? (
                            <Box display="flex" justifyContent="center" alignItems="center" py={8}>
                                <CircularProgress />
                            </Box>
                        ) : filteredUsers.length === 0 ? (
                            <Box textAlign="center" py={8}>
                                <PersonIcon sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
                                <Typography variant="h6" color="text.secondary" gutterBottom>
                                    Nenhum usuário encontrado
                                </Typography>
                                <Typography variant="body2" color="text.secondary">
                                    {users.length === 0 ? 'Ainda não há usuários cadastrados.' : 'Tente ajustar os filtros de busca.'}
                                </Typography>
                            </Box>
                        ) : (
                            <TableContainer>
                                <Table>
                                    <TableHead>
                                        <TableRow
                                            sx={{
                                                backgroundColor: theme.palette.mode === 'dark'
                                                    ? '#424242'
                                                    : '#ffffff',
                                                position: 'sticky',
                                                top: 0,
                                                zIndex: 100
                                            }}
                                        >
                                            <TableCell
                                                sx={{
                                                    backgroundColor: theme.palette.mode === 'dark'
                                                        ? '#424242'
                                                        : '#ffffff',
                                                    fontWeight: 'bold',
                                                    position: 'sticky',
                                                    top: 0,
                                                    zIndex: 99,
                                                    borderBottom: '2px solid',
                                                    borderBottomColor: theme.palette.mode === 'dark' ? '#666' : '#e0e0e0'
                                                }}
                                            >
                                                Usuário
                                            </TableCell>
                                            <TableCell
                                                sx={{
                                                    backgroundColor: theme.palette.mode === 'dark'
                                                        ? '#424242'
                                                        : '#ffffff',
                                                    fontWeight: 'bold',
                                                    position: 'sticky',
                                                    top: 0,
                                                    zIndex: 99,
                                                    borderBottom: '2px solid',
                                                    borderBottomColor: theme.palette.mode === 'dark' ? '#666' : '#e0e0e0'
                                                }}
                                            >
                                                Email
                                            </TableCell>
                                            <TableCell
                                                sx={{
                                                    backgroundColor: theme.palette.mode === 'dark'
                                                        ? '#424242'
                                                        : '#ffffff',
                                                    fontWeight: 'bold',
                                                    position: 'sticky',
                                                    top: 0,
                                                    zIndex: 99,
                                                    borderBottom: '2px solid',
                                                    borderBottomColor: theme.palette.mode === 'dark' ? '#666' : '#e0e0e0'
                                                }}
                                            >
                                                Perfil
                                            </TableCell>
                                            <TableCell
                                                sx={{
                                                    backgroundColor: theme.palette.mode === 'dark'
                                                        ? '#424242'
                                                        : '#ffffff',
                                                    fontWeight: 'bold',
                                                    position: 'sticky',
                                                    top: 0,
                                                    zIndex: 99,
                                                    borderBottom: '2px solid',
                                                    borderBottomColor: theme.palette.mode === 'dark' ? '#666' : '#e0e0e0'
                                                }}
                                            >
                                                Criado em
                                            </TableCell>
                                            <TableCell
                                                align="center"
                                                sx={{
                                                    backgroundColor: theme.palette.mode === 'dark'
                                                        ? '#424242'
                                                        : '#ffffff',
                                                    fontWeight: 'bold',
                                                    position: 'sticky',
                                                    top: 0,
                                                    zIndex: 99,
                                                    borderBottom: '2px solid',
                                                    borderBottomColor: theme.palette.mode === 'dark' ? '#666' : '#e0e0e0'
                                                }}
                                            >
                                                Ações
                                            </TableCell>
                                        </TableRow>
                                    </TableHead>
                                    <TableBody>
                                        {filteredUsers.map((user) => (
                                            <TableRow key={user.Id || user.id} hover>
                                                <TableCell>
                                                    <Box display="flex" alignItems="center" gap={2}>
                                                        <Avatar
                                                            src={user.ProfileImagePath || user.profileImagePath}
                                                            sx={{ width: 40, height: 40 }}
                                                        >
                                                            <PersonIcon />
                                                        </Avatar>
                                                        <Box>
                                                            <Typography variant="subtitle2" fontWeight="medium">
                                                                {user.Name || user.name}
                                                            </Typography>
                                                            <Typography variant="body2" color="text.secondary">
                                                                @{user.Nickname || user.nickname}
                                                            </Typography>
                                                        </Box>
                                                    </Box>
                                                </TableCell>
                                                <TableCell>
                                                    <Typography variant="body2">
                                                        {user.Email || user.email}
                                                    </Typography>
                                                </TableCell>
                                                <TableCell>
                                                    <Chip
                                                        icon={getProfileIcon((user.Profile || user.profile)?.Name || (user.Profile || user.profile)?.name)}
                                                        label={(user.Profile || user.profile)?.Name || (user.Profile || user.profile)?.name}
                                                        color={getProfileColor((user.Profile || user.profile)?.Name || (user.Profile || user.profile)?.name)}
                                                        size="small"
                                                    />
                                                </TableCell>
                                                <TableCell>
                                                    <Typography variant="body2" color="text.secondary">
                                                        {formatDate(user.CreatedAt || user.createdAt)}
                                                    </Typography>
                                                </TableCell>
                                                <TableCell align="center">
                                                    <Stack direction="row" spacing={1} justifyContent="center">
                                                        {canEditUser(user) && (
                                                            <Tooltip title="Editar usuário">
                                                                <IconButton
                                                                    size="small"
                                                                    onClick={() => handleEditUser(user)}
                                                                    color="primary"
                                                                >
                                                                    <EditIcon fontSize="small" />
                                                                </IconButton>
                                                            </Tooltip>
                                                        )}
                                                        {canEditUser(user) && (
                                                            <Tooltip title="Alterar senha">
                                                                <IconButton
                                                                    size="small"
                                                                    onClick={() => handleChangePassword(user)}
                                                                    color="warning"
                                                                >
                                                                    <VpnKeyIcon fontSize="small" />
                                                                </IconButton>
                                                            </Tooltip>
                                                        )}
                                                        {canDeleteUser(user) && (
                                                            <Tooltip title="Remover usuário">
                                                                <IconButton
                                                                    size="small"
                                                                    onClick={() => handleDeleteUser(user)}
                                                                    color="error"
                                                                >
                                                                    <DeleteIcon fontSize="small" />
                                                                </IconButton>
                                                            </Tooltip>
                                                        )}
                                                    </Stack>
                                                </TableCell>
                                            </TableRow>
                                        ))}
                                    </TableBody>
                                </Table>
                            </TableContainer>
                        )}
                    </Paper>
                </Box>
            </Container>

            {/* Modal de Usuário */}
            <UserModal
                open={userModalOpen}
                onClose={() => setUserModalOpen(false)}
                onSave={handleSaveUser}
                user={selectedUser}
                currentUserProfile={currentUser?.profile?.name}
            />

            {/* Modal de Alteração de Senha */}
            <ChangePasswordModal
                open={passwordModalOpen}
                onClose={() => setPasswordModalOpen(false)}
                onSave={handleSavePassword}
                userName={selectedUser?.Name || selectedUser?.name}
            />

            {/* Dialog de Confirmação de Exclusão */}
            <Dialog
                open={deleteDialogOpen}
                onClose={() => setDeleteDialogOpen(false)}
                maxWidth="sm"
                fullWidth
            >
                <DialogTitle>
                    Confirmar Remoção
                </DialogTitle>
                <DialogContent>
                    <Alert severity="warning" sx={{ mb: 2 }}>
                        Esta ação não pode ser desfeita!
                    </Alert>
                    <Typography>
                        Tem certeza que deseja remover o usuário <strong>{selectedUser?.Name || selectedUser?.name}</strong>?
                    </Typography>
                    <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                        O usuário será desativado no sistema.
                    </Typography>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setDeleteDialogOpen(false)}>
                        Cancelar
                    </Button>
                    <Button onClick={confirmDeleteUser} color="error" variant="contained">
                        Remover
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
}
