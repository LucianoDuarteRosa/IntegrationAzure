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
    Grid
} from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { Refresh as RefreshIcon, FilterList as FilterIcon, Assessment as AssessmentIcon } from '@mui/icons-material';
import { Navbar } from '../components/Navbar';
import { logService } from '../services';
import { useNotifications } from '../hooks/useNotifications';

const LogLevel = {
    Info: 1,
    Warning: 2,
    Error: 3,
    Success: 4
};

const LogLevelNames = {
    1: 'Info',
    2: 'Warning',
    3: 'Error',
    4: 'Success'
};

const LogLevelColors = {
    1: 'info',
    2: 'warning',
    3: 'error',
    4: 'success'
};

export function LogsPage() {
    const theme = useTheme();
    const { error } = useNotifications();
    const [logs, setLogs] = useState([]);
    const [loading, setLoading] = useState(true);
    const [filters, setFilters] = useState({
        entity: '',
        level: '',
        userId: '',
        pageSize: 50
    });

    // Carregar logs ao montar o componente
    useEffect(() => {
        loadLogs();
    }, []);

    const loadLogs = async () => {
        await loadLogsWithFilters();
    };

    const handleFilterChange = (field, value) => {
        setFilters(prev => ({
            ...prev,
            [field]: value
        }));
    };

    const applyFilters = () => {
        loadLogsWithFilters();
    };

    const clearFilters = () => {
        const clearedFilters = {
            entity: '',
            level: '',
            userId: '',
            pageSize: 50
        };
        setFilters(clearedFilters);

        // Carregar logs sem filtros
        loadLogsWithFilters(clearedFilters);
    };

    const loadLogsWithFilters = async (customFilters = null) => {
        try {
            setLoading(true);

            const filtersToUse = customFilters || filters;
            // Remove filtros vazios
            const cleanFilters = Object.fromEntries(
                Object.entries(filtersToUse).filter(([, value]) => value !== '' && value !== null && value !== undefined)
            );

            const response = await logService.getLogs(cleanFilters);

            if (response && response.success) {
                setLogs(response.data || []);
            } else {
                error.load('logs', [response?.message || 'Resposta inválida do servidor']);
            }
        } catch {
            error.connection();
        } finally {
            setLoading(false);
        }
    };

    const formatDate = (dateString) => {
        if (!dateString) return 'Data inválida';
        try {
            return new Date(dateString).toLocaleString('pt-BR');
        } catch (error) {
            return 'Data inválida';
        }
    };

    const getActionColor = (action) => {
        if (!action || typeof action !== 'string') return 'primary';
        if (action.includes('SUCCESS')) return 'success';
        if (action.includes('FAILED') || action.includes('ERROR')) return 'error';
        if (action.includes('WARNING')) return 'warning';
        return 'primary';
    };

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
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                        width: '100%',
                        maxWidth: '1400px'
                    }}
                >
                    <Box sx={{ mb: 3, textAlign: 'center' }}>
                        <Typography variant="h4" component="h1" gutterBottom sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 1 }}>
                            <AssessmentIcon sx={{ fontSize: '2.5rem' }} />
                            Logs do Sistema
                        </Typography>
                    </Box>

                    {/* Filtros */}
                    <Paper
                        elevation={3}
                        sx={{
                            p: 3,
                            mb: 3,
                            width: '100%',
                            maxWidth: '1200px'
                        }}
                    >
                        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                            <FilterIcon sx={{ mr: 1 }} />
                            <Typography variant="h6">Filtros</Typography>
                        </Box>

                        <Grid container spacing={2} alignItems="center">
                            <Grid item xs={12} sm={6} md={3}>
                                <TextField
                                    fullWidth
                                    label="Usuário"
                                    value={filters.userId}
                                    onChange={(e) => handleFilterChange('userId', e.target.value)}
                                    size="small"
                                />
                            </Grid>
                            <Grid item xs={12} sm={6} md={3}>
                                <TextField
                                    fullWidth
                                    label="Entidade"
                                    value={filters.entity}
                                    onChange={(e) => handleFilterChange('entity', e.target.value)}
                                    size="small"
                                />
                            </Grid>
                            <Grid item xs={12} sm={6} md={2}>
                                <FormControl size="small" sx={{ width: '120px' }}>
                                    <InputLabel>Nível</InputLabel>
                                    <Select
                                        value={filters.level}
                                        label="Nível"
                                        onChange={(e) => handleFilterChange('level', e.target.value)}
                                    >
                                        <MenuItem value="">Todos</MenuItem>
                                        <MenuItem value={LogLevel.Info}>Info</MenuItem>
                                        <MenuItem value={LogLevel.Warning}>Warning</MenuItem>
                                        <MenuItem value={LogLevel.Error}>Error</MenuItem>
                                        <MenuItem value={LogLevel.Success}>Success</MenuItem>
                                    </Select>
                                </FormControl>
                            </Grid>
                            <Grid item xs={12} sm={6} md={2}>
                                <FormControl fullWidth size="small" sx={{ width: '100px' }}>
                                    <InputLabel>Por página</InputLabel>
                                    <Select
                                        value={filters.pageSize}
                                        label="Por página"
                                        onChange={(e) => handleFilterChange('pageSize', e.target.value)}
                                    >
                                        <MenuItem value={25}>25</MenuItem>
                                        <MenuItem value={50}>50</MenuItem>
                                        <MenuItem value={100}>100</MenuItem>
                                    </Select>
                                </FormControl>
                            </Grid>
                            <Grid item xs={12} md={2}>
                                <Stack direction="row" spacing={1}>
                                    <Button
                                        variant="contained"
                                        onClick={applyFilters}
                                        size="small"
                                        sx={{ minWidth: '80px' }}
                                    >
                                        Filtrar
                                    </Button>
                                    <Button
                                        variant="outlined"
                                        onClick={clearFilters}
                                        size="small"
                                    >
                                        Limpar
                                    </Button>
                                </Stack>
                            </Grid>
                        </Grid>
                    </Paper>

                    {/* Conteúdo principal */}
                    <Paper
                        elevation={3}
                        sx={{
                            width: '100%',
                            maxWidth: '1200px'
                        }}
                    >
                        <Box sx={{ p: 2, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                            <Typography variant="h6">
                                Registros de Log ({logs.length})
                            </Typography>
                            <Button
                                variant="outlined"
                                startIcon={<RefreshIcon />}
                                onClick={loadLogs}
                                disabled={loading}
                            >
                                Atualizar
                            </Button>
                        </Box>

                        {loading ? (
                            <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
                                <CircularProgress />
                            </Box>
                        ) : (
                            <TableContainer
                                sx={{
                                    maxHeight: 'calc(96vh - 400px)',
                                    overflowY: 'auto',
                                    overflowX: 'hidden'
                                }}
                            >
                                <Table stickyHeader>
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
                                                Data/Hora
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
                                                Ação
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
                                                Entidade
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
                                                Nível
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
                                                ID da Entidade
                                            </TableCell>
                                        </TableRow>
                                    </TableHead>
                                    <TableBody>
                                        {logs.length === 0 ? (
                                            <TableRow>
                                                <TableCell colSpan={6} align="center" sx={{ py: 4 }}>
                                                    <Typography color="text.secondary">
                                                        Nenhum log encontrado
                                                    </Typography>
                                                </TableCell>
                                            </TableRow>
                                        ) : (
                                            logs.map((log) => (
                                                <TableRow key={log.Id || log.id} hover>
                                                    <TableCell>
                                                        <Typography variant="body2">
                                                            {formatDate(log.CreatedAt || log.createdAt)}
                                                        </Typography>
                                                    </TableCell>
                                                    <TableCell>
                                                        <Chip
                                                            label={log.Action || log.action || 'N/A'}
                                                            color={getActionColor(log.Action || log.action)}
                                                            size="small"
                                                            variant="outlined"
                                                        />
                                                    </TableCell>
                                                    <TableCell>
                                                        <Typography variant="body2" fontWeight="medium">
                                                            {log.Entity || log.entity || 'N/A'}
                                                        </Typography>
                                                    </TableCell>
                                                    <TableCell>
                                                        <Typography variant="body2">
                                                            {log.UserId || log.userId || 'N/A'}
                                                        </Typography>
                                                    </TableCell>
                                                    <TableCell>
                                                        <Chip
                                                            label={LogLevelNames[log.Level || log.level] || 'UNKNOWN'}
                                                            color={LogLevelColors[log.Level || log.level] || 'default'}
                                                            size="small"
                                                        />
                                                    </TableCell>
                                                    <TableCell>
                                                        <Typography variant="body2" color="text.secondary">
                                                            {log.EntityId || log.entityId || '-'}
                                                        </Typography>
                                                    </TableCell>
                                                </TableRow>
                                            ))
                                        )}
                                    </TableBody>
                                </Table>
                            </TableContainer>
                        )}
                    </Paper>
                </Box>
            </Container>
        </>
    );
}
