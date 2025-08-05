import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Navbar } from './Navbar';
import { useNotifications } from '../hooks/useNotifications';
import { failureService } from '../services/failureService';
import { azureDevOpsService } from '../services/azureDevOpsService';
import {
    Box,
    Container,
    Paper,
    Typography,
    TextField,
    Button,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    IconButton,
    Stack,
    Divider,
    Grid,
    Alert,
    CircularProgress,
    Chip,
} from '@mui/material';
import {
    Delete as DeleteIcon,
    Add as AddIcon,
    AttachFile as AttachFileIcon,
    BugReport as BugReportIcon
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import * as yup from 'yup';
import { yupResolver } from '@hookform/resolvers/yup';

const schema = yup.object().shape({
    demandNumber: yup.string().required('Projeto é obrigatório'),
    userStoryId: yup.string().required('História do usuário é obrigatória'),
    title: yup.string().required('Título é obrigatório'),
    severity: yup.string().required('Severidade é obrigatória'),
    occurrenceType: yup.number().required('Tipo de ocorrência é obrigatório'),
    environment: yup.string().required('Ambiente é obrigatório'),
    observations: yup.string(), // Opcional
    givenWhenThen: yup.array().of(
        yup.object().shape({
            given: yup.string().trim(),
            when: yup.string().trim(),
            then: yup.string().trim(),
        })
    ).test('at-least-one-complete', 'Pelo menos um impacto completo é obrigatório', function (value) {
        if (!value || value.length === 0) return false;
        return value.some(scenario =>
            scenario.given?.trim() &&
            scenario.when?.trim() &&
            scenario.then?.trim()
        );
    }),
});

const severidades = [
    { value: 'Low', label: 'Baixa', color: '#4caf50' },
    { value: 'Medium', label: 'Média', color: '#ff9800' },
    { value: 'High', label: 'Alta', color: '#f44336' },
    { value: 'Critical', label: 'Crítica', color: '#d32f2f' },
];

const tiposOcorrencia = [
    { value: 4, label: 'Erro de Migração de Dados' },
    { value: 5, label: 'Erro de Sistema' },
    { value: 6, label: 'Erro em Ambiente' },
    { value: 7, label: 'Problema de Banco de Dados' },
    { value: 8, label: 'Problema de Infraestrutura' },
    { value: 9, label: 'Problema de Parametrizações' },
];

const ambientes = [
    { value: 'Development', label: 'Desenvolvimento' },
    { value: 'Staging', label: 'Homologação' },
    { value: 'Production', label: 'Produção' },
];

const Section = ({ title, children, icon, isFirst }) => {
    return (
        <Box sx={{ mb: 4 }}>
            {!isFirst && (
                <Box sx={{ mb: 4, mx: 'auto', width: '90%' }}>
                    <Divider sx={{
                        opacity: 0.4,
                        borderColor: 'text.secondary'
                    }} />
                </Box>
            )}
            <Box sx={{
                display: 'flex',
                alignItems: 'center',
                mb: 2,
            }}>
                {icon && (
                    <Box sx={{ mr: 1, color: 'primary.main' }}>
                        {icon}
                    </Box>
                )}
                <Typography
                    variant="h6"
                    component="h2"
                    sx={{
                        fontSize: '1.5rem',
                        fontWeight: 500
                    }}
                >
                    {title}
                </Typography>
            </Box>
            {children}
        </Box>
    );
};

const GivenWhenThenFields = ({ scenarios, onAdd, onRemove, onScenarioChange, disabled, errors }) => {
    return (
        <Stack spacing={3}>
            {scenarios.map((scenario, index) => (
                <Paper
                    key={index}
                    sx={{
                        p: 3,
                        border: '1px solid',
                        borderColor: 'divider',
                        backgroundColor: 'background.default'
                    }}
                >
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                        <Typography variant="subtitle1" fontWeight="medium">
                            Impacto {index + 1}
                        </Typography>
                        <IconButton
                            onClick={() => onRemove(index)}
                            disabled={scenarios.length === 1 || disabled}
                            size="small"
                            sx={{ color: '#d32f2f' }}
                        >
                            <DeleteIcon />
                        </IconButton>
                    </Box>

                    <Stack spacing={2}>
                        <TextField
                            fullWidth
                            label="Processo Atual"
                            multiline
                            rows={2}
                            value={scenario.given}
                            onChange={(e) => onScenarioChange(index, 'given', e.target.value)}
                            disabled={disabled}
                            placeholder="Descreva o processo atual que apresenta a falha"
                            error={!!(errors?.givenWhenThen?.[index]?.given)}
                            helperText={errors?.givenWhenThen?.[index]?.given?.message}
                            required
                        />
                        <TextField
                            fullWidth
                            label="Ação Executada"
                            multiline
                            rows={2}
                            value={scenario.when}
                            onChange={(e) => onScenarioChange(index, 'when', e.target.value)}
                            disabled={disabled}
                            placeholder="Descreva a ação que foi executada quando a falha ocorreu"
                            error={!!(errors?.givenWhenThen?.[index]?.when)}
                            helperText={errors?.givenWhenThen?.[index]?.when?.message}
                            required
                        />
                        <TextField
                            fullWidth
                            label="Melhoria Esperada"
                            multiline
                            rows={2}
                            value={scenario.then}
                            onChange={(e) => onScenarioChange(index, 'then', e.target.value)}
                            disabled={disabled}
                            placeholder="Descreva como o processo deveria funcionar corretamente"
                            error={!!(errors?.givenWhenThen?.[index]?.then)}
                            helperText={errors?.givenWhenThen?.[index]?.then?.message}
                            required
                        />
                    </Stack>
                </Paper>
            ))}

            <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                <Button
                    startIcon={<AddIcon />}
                    onClick={onAdd}
                    variant="outlined"
                    disabled={disabled}
                    sx={{ width: '50%' }}
                >
                    Adicionar Impacto
                </Button>
            </Box>

            {/* Exibe erro geral dos cenários */}
            {errors?.givenWhenThen?.message && (
                <Alert severity="error" sx={{ mt: 2 }}>
                    {errors.givenWhenThen.message}
                </Alert>
            )}
        </Stack>
    );
};

export function FailureForm() {
    const navigate = useNavigate();
    const { showSuccess, showError, showInfo } = useNotifications();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [attachments, setAttachments] = useState([]);
    const [givenWhenThen, setGivenWhenThen] = useState([
        { given: '', when: '', then: '' }
    ]);
    const [selectedDemand, setSelectedDemand] = useState('');
    const [availableStories, setAvailableStories] = useState([]);
    const [azureProjects, setAzureProjects] = useState([]);
    const [loadingProjects, setLoadingProjects] = useState(false);
    // Usar apenas os tipos hardcoded específicos para falhas (não buscar da API)

    const {
        control,
        handleSubmit,
        watch,
        setValue,
        clearErrors,
        reset,
        formState: { errors }
    } = useForm({
        resolver: yupResolver(schema),
        defaultValues: {
            demandNumber: '',
            userStoryId: '',
            title: '',
            severity: 'Medium',
            occurrenceType: 5, // Valor padrão: "Erro de Sistema"
            environment: 'Production',
            observations: '',
            givenWhenThen: [{ given: '', when: '', then: '' }],
        }
    });

    // Função para resetar todo o formulário
    const resetFormToInitialState = () => {
        // Reset do react-hook-form
        reset({
            demandNumber: '',
            userStoryId: '',
            title: '',
            severity: 'Medium',
            occurrenceType: 5, // Valor padrão: "Erro de Sistema"
            environment: 'Production',
            observations: '',
            givenWhenThen: [{ given: '', when: '', then: '' }],
        });

        // Reset dos estados locais
        setAttachments([]);
        setGivenWhenThen([{ given: '', when: '', then: '' }]);
        setSelectedDemand('');
        setAvailableStories([]);
    };

    const watchedDemand = watch('demandNumber');

    // Carregar projetos do Azure DevOps ao inicializar o componente
    useEffect(() => {
        loadAzureProjects();
    }, []);

    const loadAzureProjects = async () => {
        setLoadingProjects(true);
        try {
            const projects = await azureDevOpsService.getProjects();
            setAzureProjects(projects || []);
        } catch (error) {
            console.error('Erro ao carregar projetos do Azure:', error);
            // Verifica se é erro de configuração/conectividade ou apenas não há projetos
            if (error.response?.status === 401 || error.response?.status === 403) {
                showError('Erro de autenticação', 'Verifique as credenciais do Azure DevOps nas configurações.');
            } else if (error.response?.status === 404 || error.message?.includes('not found')) {
                console.info('Nenhum projeto encontrado na organização do Azure DevOps.');
                showInfo('Informação sobre projetos', 'Nenhum projeto foi encontrado na organização do Azure DevOps. Verifique as configurações de integração.');
                setAzureProjects([]);
            } else if (error.code === 'NETWORK_ERROR' || error.message?.includes('network')) {
                showError('Erro de conexão', 'Não foi possível conectar ao Azure DevOps. Verifique sua conexão e as configurações.');
            } else {
                showError('Erro ao carregar projetos', 'Não foi possível carregar os projetos do Azure DevOps. Verifique as configurações.');
            }

            // Não usar projetos mock, deixar vazio
            setAzureProjects([]);
        } finally {
            setLoadingProjects(false);
        }
    };

    // Carregar User Stories do projeto selecionado
    useEffect(() => {
        if (watchedDemand) {
            loadWorkItems(watchedDemand);
        } else {
            setAvailableStories([]);
        }
    }, [watchedDemand, setValue]);

    const loadWorkItems = async (projectId) => {
        try {
            const workItems = await azureDevOpsService.getWorkItems(projectId, 'User Story');
            setAvailableStories(workItems || []);
            setValue('userStoryId', ''); // Limpa a história selecionada
        } catch (error) {
            console.error('Erro ao carregar work items:', error);
            // Verifica se é realmente um erro ou apenas não há dados
            if (error.response?.status === 404 || error.message?.includes('not found') || error.message?.includes('No work items')) {
                // Não é um erro real, apenas não há histórias - mostrar como informação
                console.info('Projeto não possui histórias de usuário cadastradas.');
                showInfo('Informação sobre histórias', 'Este projeto não possui histórias de usuário cadastradas. Selecione um projeto que contenha histórias para continuar.');
                setAvailableStories([]);
            } else {
                // Erro real de conectividade ou configuração
                showError('Erro ao carregar histórias', 'Não foi possível carregar as histórias do usuário. Verifique a conexão com o Azure DevOps.');
                setAvailableStories([]);
            }
        }
    };

    const handleAddScenario = () => {
        const newScenarios = [...givenWhenThen, { given: '', when: '', then: '' }];
        setGivenWhenThen(newScenarios);
        setValue('givenWhenThen', newScenarios);
    };

    const handleRemoveScenario = (index) => {
        if (givenWhenThen.length > 1) {
            const newScenarios = givenWhenThen.filter((_, i) => i !== index);
            setGivenWhenThen(newScenarios);
            setValue('givenWhenThen', newScenarios);
        }
    };

    const handleScenarioChange = (index, field, value) => {
        const newScenarios = [...givenWhenThen];
        newScenarios[index][field] = value;
        setGivenWhenThen(newScenarios);
        setValue('givenWhenThen', newScenarios);

        // Limpa erros quando o usuário começa a digitar
        if (value.trim()) {
            clearErrors(`givenWhenThen.${index}.${field}`);
        }
    };

    const handleFileUpload = (event) => {
        const files = Array.from(event.target.files);
        const newAttachments = files.map(file => ({
            id: Date.now() + Math.random(),
            name: file.name,
            size: file.size,
            type: file.type,
            file: file
        }));
        setAttachments([...attachments, ...newAttachments]);
    };

    const handleRemoveAttachment = (id) => {
        setAttachments(attachments.filter(att => att.id !== id));
    };

    const onSubmit = async (data) => {
        setIsSubmitting(true);

        try {
            // Mapear severidade para os valores da API
            const severityMap = {
                'Low': 1,
                'Medium': 2,
                'High': 3,
                'Critical': 4
            };

            // Montar cenários completos (Dado que/Quando/Então) para criar os Impactos
            const completedScenarios = givenWhenThen.filter(scenario =>
                scenario.given?.trim() &&
                scenario.when?.trim() &&
                scenario.then?.trim()
            );

            // Mapear os cenários para o formato esperado pela API
            const scenariosForApi = completedScenarios.map(scenario => ({
                Given: scenario.given.trim(),
                When: scenario.when.trim(),
                Then: scenario.then.trim()
            }));

            // Preparar dados para envio seguindo exatamente o formato do CreateFailureDto
            const failureData = {
                FailureNumber: `FLH-${String(Date.now()).slice(-6)}`, // Formato correto: FLH-XXXXXX
                Title: data.title,
                Description: '', // A API vai gerar a descrição em Markdown
                Severity: severityMap[data.severity], // Valor numérico (1-4)
                OccurrenceType: parseInt(data.occurrenceType), // Valor numérico do tipo de ocorrência
                OccurredAt: new Date().toISOString(),
                Environment: data.environment,
                UserStoryId: data.userStoryId || null, // Usar o GUID selecionado ou null
                Scenarios: scenariosForApi, // Cenários estruturados para a API
                Observations: data.observations?.trim() || null, // Observações
                Attachments: attachments.map(att => ({ // Evidências
                    Name: att.name,
                    Size: att.size,
                    Type: att.type
                }))
            };

            // Enviar para a API
            const response = await failureService.create(failureData);

            // A API retorna um ApiResponseDto<T> com propriedade success (camelCase)
            const isSuccess = response && response.success === true;

            if (isSuccess) {
                showSuccess('Falha registrada!', 'Falha registrada com sucesso!');

                // Resetar o formulário para registrar uma nova falha
                resetFormToInitialState();
            } else {
                // Se não tem success=true, tratar como erro
                const errorMessage = response?.Message || response?.message || 'Erro ao registrar falha';
                const errorList = response?.Errors || response?.errors || [];
                throw new Error(JSON.stringify({ message: errorMessage, errors: errorList }));
            }
        } catch (error) {
            // Tratamento específico para diferentes tipos de erro
            if (error.response?.status === 400) {
                // Erro de validação (400 Bad Request)
                const errorData = error.response.data;

                if (errorData.errors && Array.isArray(errorData.errors)) {
                    showError('Erro de validação', errorData.errors.join(', '));
                } else if (errorData.Errors && Array.isArray(errorData.Errors)) {
                    showError('Erro de validação', errorData.Errors.join(', '));
                } else if (errorData.message || errorData.Message) {
                    showError('Erro de validação', errorData.message || errorData.Message);
                } else {
                    showError('Erro de validação', 'Verifique os dados informados.');
                }
            } else if (error.errors && typeof error.errors === 'object') {
                // Se errors é um objeto com propriedades (como $.UserStoryId)
                const errorMessages = [];
                Object.keys(error.errors).forEach(key => {
                    const errorArray = error.errors[key];
                    if (Array.isArray(errorArray)) {
                        errorMessages.push(...errorArray);
                    } else {
                        errorMessages.push(errorArray);
                    }
                });
                showError('Erro de validação', errorMessages.join(', '));
            } else if (error.errors && Array.isArray(error.errors)) {
                showError('Erro de validação', error.errors.join(', '));
            } else if (error.message) {
                showError('Erro na operação', error.message);
            } else {
                showError('Erro ao registrar', 'Erro ao registrar falha. Tente novamente.');
            }
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleCancel = () => {
        navigate('/dashboard');
    };

    const getSeverityColor = (severity) => {
        const severityObj = severidades.find(s => s.value === severity);
        return severityObj?.color || '#757575';
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
                    minHeight: 'calc(100vh - 64px)',
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
                        width: '100%'
                    }}
                >
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
                            <BugReportIcon sx={{ fontSize: '2.5rem' }} />
                            Registrar Nova Falha
                        </Typography>
                    </Box>
                    <Paper
                        elevation={3}
                        sx={{
                            p: 4,
                            width: '100%',
                            maxWidth: '1150px'
                        }}
                    >
                        <form onSubmit={handleSubmit(onSubmit)}>
                            {/* Informações Básicas */}
                            <Section title="Informações Básicas" isFirst>
                                <Box sx={{ display: 'grid', gap: 3 }}>
                                    <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 3 }}>
                                        <Controller
                                            name="demandNumber"
                                            control={control}
                                            render={({ field }) => (
                                                <FormControl fullWidth error={!!errors.demandNumber} required>
                                                    <InputLabel>Projeto do Azure DevOps</InputLabel>
                                                    <Select
                                                        {...field}
                                                        label="Projeto do Azure DevOps"
                                                        disabled={isSubmitting || loadingProjects}
                                                        required
                                                    >
                                                        {loadingProjects ? (
                                                            <MenuItem disabled>
                                                                <CircularProgress size={20} sx={{ mr: 1 }} />
                                                                Carregando projetos...
                                                            </MenuItem>
                                                        ) : azureProjects.length === 0 ? (
                                                            <MenuItem disabled>
                                                                <Box sx={{ textAlign: 'center', py: 1 }}>
                                                                    <Typography variant="body2" color="text.secondary">
                                                                        Nenhum projeto disponível
                                                                    </Typography>
                                                                    <Typography variant="caption" color="text.secondary">
                                                                        Verifique as configurações do Azure DevOps
                                                                    </Typography>
                                                                </Box>
                                                            </MenuItem>
                                                        ) : (
                                                            azureProjects.map((project) => (
                                                                <MenuItem key={project.id} value={project.id}>
                                                                    <Box>
                                                                        <Typography>{project.name}</Typography>
                                                                        {project.description && (
                                                                            <Typography variant="caption" color="text.secondary">
                                                                                {project.description}
                                                                            </Typography>
                                                                        )}
                                                                    </Box>
                                                                </MenuItem>
                                                            ))
                                                        )}
                                                    </Select>
                                                    {errors.demandNumber && (
                                                        <Typography variant="caption" color="error" sx={{ mt: 1 }}>
                                                            {errors.demandNumber.message}
                                                        </Typography>
                                                    )}
                                                </FormControl>
                                            )}
                                        />

                                        <Controller
                                            name="userStoryId"
                                            control={control}
                                            render={({ field }) => (
                                                <FormControl
                                                    fullWidth
                                                    error={!!errors.userStoryId}
                                                    disabled={availableStories.length === 0}
                                                    required
                                                >
                                                    <InputLabel>História do Usuário</InputLabel>
                                                    <Select
                                                        {...field}
                                                        label="História do Usuário"
                                                        disabled={isSubmitting || availableStories.length === 0}
                                                        required
                                                    >
                                                        {availableStories.length === 0 ? (
                                                            <MenuItem disabled>
                                                                <Box sx={{ textAlign: 'center', py: 1 }}>
                                                                    <Typography variant="body2" color="text.secondary">
                                                                        {!watchedDemand
                                                                            ? 'Selecione um projeto primeiro'
                                                                            : 'Não existem histórias associadas ao projeto'
                                                                        }
                                                                    </Typography>
                                                                    {watchedDemand && (
                                                                        <Typography variant="caption" color="text.secondary">
                                                                            Selecione um projeto que contenha histórias para continuar
                                                                        </Typography>
                                                                    )}
                                                                </Box>
                                                            </MenuItem>
                                                        ) : (
                                                            availableStories.map((story) => (
                                                                <MenuItem key={story.id} value={story.id}>
                                                                    <Box>
                                                                        <Typography>
                                                                            #{story.id} - {story.title}
                                                                            <Typography component="span" variant="caption" color="text.secondary" sx={{ ml: 1 }}>
                                                                                | Status: {story.state} | Atribuído: {story.assignedTo}
                                                                            </Typography>
                                                                        </Typography>
                                                                    </Box>
                                                                </MenuItem>
                                                            ))
                                                        )}
                                                    </Select>
                                                    {errors.userStoryId && (
                                                        <Typography variant="caption" color="error" sx={{ mt: 1 }}>
                                                            {errors.userStoryId.message}
                                                        </Typography>
                                                    )}
                                                </FormControl>
                                            )}
                                        />
                                    </Box>

                                    {/* Segunda linha: Título (2fr), Severidade (1fr) e Ambiente (1fr) */}
                                    <Box sx={{ display: 'grid', gridTemplateColumns: '4fr 1fr 3fr 2fr', gap: 2 }}>
                                        <Controller
                                            name="title"
                                            control={control}
                                            render={({ field }) => (
                                                <TextField
                                                    {...field}
                                                    fullWidth
                                                    label="Título da Falha"
                                                    error={!!errors.title}
                                                    helperText={errors.title?.message}
                                                    disabled={isSubmitting}
                                                    placeholder="Descreva brevemente a falha encontrada"
                                                    required
                                                />
                                            )}
                                        />

                                        <Controller
                                            name="severity"
                                            control={control}
                                            render={({ field }) => (
                                                <FormControl fullWidth error={!!errors.severity} required>
                                                    <InputLabel>Severidade</InputLabel>
                                                    <Select
                                                        {...field}
                                                        label="Severidade"
                                                        disabled={isSubmitting}
                                                        required
                                                        renderValue={(value) => (
                                                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                                                <Chip
                                                                    size="small"
                                                                    label={severidades.find(s => s.value === value)?.label}
                                                                    sx={{
                                                                        backgroundColor: getSeverityColor(value),
                                                                        color: 'white',
                                                                        fontWeight: 'bold'
                                                                    }}
                                                                />
                                                            </Box>
                                                        )}
                                                    >
                                                        {severidades.map((severity) => (
                                                            <MenuItem key={severity.value} value={severity.value}>
                                                                <Chip
                                                                    size="small"
                                                                    label={severity.label}
                                                                    sx={{
                                                                        backgroundColor: severity.color,
                                                                        color: 'white',
                                                                        fontWeight: 'bold'
                                                                    }}
                                                                />
                                                            </MenuItem>
                                                        ))}
                                                    </Select>
                                                    {errors.severity && (
                                                        <Typography variant="caption" color="error" sx={{ mt: 1 }}>
                                                            {errors.severity.message}
                                                        </Typography>
                                                    )}
                                                </FormControl>
                                            )}
                                        />
                                        <Box sx={{ display: 'grid', gridTemplateColumns: '1fr', gap: 3 }}>
                                            <Controller
                                                name="occurrenceType"
                                                control={control}
                                                render={({ field }) => (
                                                    <FormControl fullWidth error={!!errors.occurrenceType} required>
                                                        <InputLabel>Tipo de Ocorrência</InputLabel>
                                                        <Select
                                                            {...field}
                                                            label="Tipo de Ocorrência"
                                                            disabled={isSubmitting}
                                                            required
                                                        >
                                                            {tiposOcorrencia.map((tipo) => (
                                                                <MenuItem key={tipo.value} value={tipo.value}>
                                                                    {tipo.label}
                                                                </MenuItem>
                                                            ))}
                                                        </Select>
                                                        {errors.occurrenceType && (
                                                            <Typography variant="caption" color="error" sx={{ mt: 1 }}>
                                                                {errors.occurrenceType.message}
                                                            </Typography>
                                                        )}
                                                    </FormControl>
                                                )}
                                            />
                                        </Box>
                                        <Controller
                                            name="environment"
                                            control={control}
                                            render={({ field }) => (
                                                <FormControl fullWidth error={!!errors.environment} required>
                                                    <InputLabel>Ambiente</InputLabel>
                                                    <Select
                                                        {...field}
                                                        label="Ambiente"
                                                        disabled={isSubmitting}
                                                        required
                                                    >
                                                        {ambientes.map((ambiente) => (
                                                            <MenuItem key={ambiente.value} value={ambiente.value}>
                                                                {ambiente.label}
                                                            </MenuItem>
                                                        ))}
                                                    </Select>
                                                    {errors.environment && (
                                                        <Typography variant="caption" color="error" sx={{ mt: 1 }}>
                                                            {errors.environment.message}
                                                        </Typography>
                                                    )}
                                                </FormControl>
                                            )}
                                        />
                                    </Box>
                                </Box>
                            </Section>

                            {/* Cenários de Falha */}
                            <Section
                                title="Cenários de Falha"
                            >
                                <GivenWhenThenFields
                                    scenarios={givenWhenThen}
                                    onAdd={handleAddScenario}
                                    onRemove={handleRemoveScenario}
                                    onScenarioChange={handleScenarioChange}
                                    disabled={isSubmitting}
                                    errors={errors}
                                />
                            </Section>

                            {/* Informações Adicionais */}
                            <Section title="Informações Adicionais">
                                <Controller
                                    name="observations"
                                    control={control}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            fullWidth
                                            label="Observações"
                                            multiline
                                            rows={4}
                                            disabled={isSubmitting}
                                            placeholder="Observações adicionais, workarounds temporários, contexto adicional da falha, etc."
                                        />
                                    )}
                                />
                            </Section>

                            {/* Evidências */}
                            <Section title="Evidências">
                                <Box sx={{ mb: 2 }}>
                                    <input
                                        type="file"
                                        multiple
                                        accept="image/*,.pdf,.doc,.docx,.txt"
                                        onChange={handleFileUpload}
                                        style={{ display: 'none' }}
                                        id="file-upload"
                                        disabled={isSubmitting}
                                    />
                                    <label htmlFor="file-upload">
                                        <Button
                                            component="span"
                                            variant="outlined"
                                            startIcon={<AttachFileIcon />}
                                            disabled={isSubmitting}
                                        >
                                            Anexar Evidências
                                        </Button>
                                    </label>
                                    <Typography variant="caption" display="block" sx={{ mt: 1, color: 'text.secondary' }}>
                                        Anexe screenshots, logs, documentos que evidenciem a falha
                                    </Typography>
                                </Box>

                                {attachments.length > 0 && (
                                    <Stack spacing={1}>
                                        {attachments.map((attachment) => (
                                            <Paper
                                                key={attachment.id}
                                                sx={{
                                                    p: 2,
                                                    display: 'flex',
                                                    justifyContent: 'space-between',
                                                    alignItems: 'center'
                                                }}
                                            >
                                                <Box>
                                                    <Typography variant="body2" fontWeight="medium">
                                                        {attachment.name}
                                                    </Typography>
                                                    <Typography variant="caption" color="text.secondary">
                                                        {(attachment.size / 1024).toFixed(1)} KB
                                                    </Typography>
                                                </Box>
                                                <IconButton
                                                    onClick={() => handleRemoveAttachment(attachment.id)}
                                                    disabled={isSubmitting}
                                                    size="small"
                                                    sx={{ color: '#d32f2f' }}
                                                >
                                                    <DeleteIcon />
                                                </IconButton>
                                            </Paper>
                                        ))}
                                    </Stack>
                                )}
                            </Section>

                            {/* Botões de Ação */}
                            <Box sx={{
                                display: 'flex',
                                justifyContent: 'center',
                                gap: 2,
                                mt: 4,
                                pt: 3,
                                borderTop: '1px solid',
                                borderColor: 'divider'
                            }}>
                                <Button
                                    variant="contained"
                                    color="warning"
                                    onClick={handleCancel}
                                    disabled={isSubmitting}
                                    sx={{ minWidth: 120 }}
                                >
                                    Cancelar
                                </Button>
                                <Button
                                    type="submit"
                                    variant="contained"
                                    disabled={isSubmitting}
                                    sx={{ minWidth: 120 }}
                                    startIcon={isSubmitting ? <CircularProgress size={20} /> : <BugReportIcon />}
                                >
                                    {isSubmitting ? 'Registrando...' : 'Registrar Falha'}
                                </Button>
                            </Box>
                        </form>
                    </Paper>
                </Box>
            </Container>
        </>
    );
}
