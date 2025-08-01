import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Navbar } from './Navbar';
import { useNotifications } from '../hooks/useNotifications';
import { issueService } from '../services/issueService';
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
    Alert,
    CircularProgress,
    Chip,
} from '@mui/material';
import {
    Delete as DeleteIcon,
    Add as AddIcon,
    AttachFile as AttachFileIcon,
    BugReport as BugReportIcon,
    Info as InfoIcon
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import * as yup from 'yup';
import { yupResolver } from '@hookform/resolvers/yup';

const schema = yup.object().shape({
    demandNumber: yup.string().required('Projeto é obrigatório'),
    userStoryId: yup.string(), // Removendo validação GUID complexa temporariamente
    title: yup.string().required('Título é obrigatório'),
    type: yup.number().required('Tipo de issue é obrigatório'),
    priority: yup.number().required('Prioridade é obrigatória'),
    occurrenceType: yup.number().required('Tipo de ocorrência é obrigatório'),
    environment: yup.string().required('Ambiente é obrigatório'),
    description: yup.string(), // Opcional
    scenarioDetails: yup.array().of(
        yup.object().shape({
            given: yup.string().trim(),
            when: yup.string().trim(),
            then: yup.string().trim(),
        })
    ), // Removendo validação complexa - API gerará descrição se necessário
});

const issueTypes = [
    { value: 1, label: 'Bug', color: '#f44336' },
    { value: 2, label: 'Feature', color: '#4caf50' },
    { value: 3, label: 'Melhoria', color: '#2196f3' },
    { value: 4, label: 'Tarefa', color: '#ff9800' },
];

const priorities = [
    { value: 1, label: 'Baixa', color: '#4caf50' },
    { value: 2, label: 'Média', color: '#ff9800' },
    { value: 3, label: 'Alta', color: '#f44336' },
    { value: 4, label: 'Crítica', color: '#d32f2f' },
];

const tiposOcorrencia = [
    { value: 1, label: 'Apoio Operacional' },
    { value: 2, label: 'Desempenho' },
    { value: 3, label: 'Dúvida ou Erro de Procedimento' },
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

const ScenarioDetailsFields = ({ scenarios, onAdd, onRemove, onScenarioChange, disabled, errors }) => {
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
                            Cenário {index + 1}
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
                            label="Contexto Atual"
                            multiline
                            rows={2}
                            value={scenario.given}
                            onChange={(e) => onScenarioChange(index, 'given', e.target.value)}
                            disabled={disabled}
                            placeholder="Descreva o contexto atual relacionado à issue"
                            error={!!(errors?.scenarioDetails?.[index]?.given)}
                            helperText={errors?.scenarioDetails?.[index]?.given?.message}
                        />
                        <TextField
                            fullWidth
                            label="Situação Problema"
                            multiline
                            rows={2}
                            value={scenario.when}
                            onChange={(e) => onScenarioChange(index, 'when', e.target.value)}
                            disabled={disabled}
                            placeholder="Descreva quando/como a issue ocorre"
                            error={!!(errors?.scenarioDetails?.[index]?.when)}
                            helperText={errors?.scenarioDetails?.[index]?.when?.message}
                        />
                        <TextField
                            fullWidth
                            label="Resultado Esperado"
                            multiline
                            rows={2}
                            value={scenario.then}
                            onChange={(e) => onScenarioChange(index, 'then', e.target.value)}
                            disabled={disabled}
                            placeholder="Descreva o resultado esperado ou a solução desejada"
                            error={!!(errors?.scenarioDetails?.[index]?.then)}
                            helperText={errors?.scenarioDetails?.[index]?.then?.message}
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
                    Adicionar Cenário
                </Button>
            </Box>
        </Stack>
    );
};

export function IssueForm() {
    const navigate = useNavigate();
    const { showSuccess, showError, showInfo } = useNotifications();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [attachments, setAttachments] = useState([]);
    const [scenarioDetails, setScenarioDetails] = useState([
        { given: '', when: '', then: '' }
    ]);
    const [selectedDemand, setSelectedDemand] = useState('');
    const [availableStories, setAvailableStories] = useState([]);
    const [azureProjects, setAzureProjects] = useState([]);
    const [loadingProjects, setLoadingProjects] = useState(false);

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
            userStoryId: '', // string vazia por padrão para o Material-UI
            title: '',
            type: 1, // Bug por padrão
            priority: 2, // Média por padrão
            occurrenceType: 5, // Erro de Sistema por padrão
            environment: 'Production',
            description: '',
            scenarioDetails: [{ given: '', when: '', then: '' }],
        }
    });

    // Função para resetar todo o formulário
    const resetFormToInitialState = () => {
        // Reset do react-hook-form
        reset({
            demandNumber: '',
            userStoryId: '', // string vazia por padrão para o Material-UI
            title: '',
            type: 1, // Bug por padrão
            priority: 2, // Média por padrão
            occurrenceType: 5, // Erro de Sistema por padrão
            environment: 'Production',
            description: '',
            scenarioDetails: [{ given: '', when: '', then: '' }],
        });

        // Reset dos estados locais
        setAttachments([]);
        setScenarioDetails([{ given: '', when: '', then: '' }]);
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
                setAzureProjects([]);
            } else if (error.code === 'NETWORK_ERROR' || error.message?.includes('network')) {
                showError('Erro de conexão', 'Não foi possível conectar ao Azure DevOps. Verifique sua conexão e as configurações.');
            } else {
                showError('Erro ao carregar projetos', 'Não foi possível carregar os projetos do Azure DevOps. Verifique as configurações.');
            }

            // Não usar projetos mock, deixar vazio para mostrar as mensagens informativas
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
                showInfo('Informação sobre histórias', 'Este projeto não possui histórias de usuário cadastradas. Como o campo "História do Usuário" é opcional para Issues, você pode continuar o registro sem selecionar uma história específica.');
                setAvailableStories([]);
            } else {
                // Erro real de conectividade ou configuração
                showError('Erro ao carregar histórias', 'Não foi possível carregar as histórias do usuário. Verifique a conexão com o Azure DevOps.');
                setAvailableStories([]);
            }
        }
    };

    const handleAddScenario = () => {
        const newScenarios = [...scenarioDetails, { given: '', when: '', then: '' }];
        setScenarioDetails(newScenarios);
        setValue('scenarioDetails', newScenarios);
    };

    const handleRemoveScenario = (index) => {
        if (scenarioDetails.length > 1) {
            const newScenarios = scenarioDetails.filter((_, i) => i !== index);
            setScenarioDetails(newScenarios);
            setValue('scenarioDetails', newScenarios);
        }
    };

    const handleScenarioChange = (index, field, value) => {
        const newScenarios = [...scenarioDetails];
        newScenarios[index][field] = value;
        setScenarioDetails(newScenarios);
        setValue('scenarioDetails', newScenarios);

        // Limpa erros quando o usuário começa a digitar
        if (value.trim()) {
            clearErrors(`scenarioDetails.${index}.${field}`);
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
            // Enviar descrição simples - API gerenciará o formato final
            const description = data.description?.trim() || `Issue registrada: ${data.title}`;

            // Montar cenários completos (Dado que/Quando/Então) para criar os Cenários
            const completedScenarios = scenarioDetails.filter(scenario =>
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

            // Preparar dados para envio seguindo o formato do CreateIssueDto
            const issueData = {
                IssueNumber: `ISS-${String(Date.now()).slice(-6)}`, // Formato: ISS-XXXXXX
                Title: data.title,
                Description: description,
                Type: parseInt(data.type), // IssueType enum (1-4)
                Priority: parseInt(data.priority), // Priority enum (1-4)
                OccurrenceType: parseInt(data.occurrenceType), // Tipo de ocorrência (1-9)
                Environment: data.environment,
                UserStoryId: data.userStoryId && data.userStoryId.trim() && data.userStoryId !== " " ? data.userStoryId : null, // null se string vazia, espaço ou GUID válido
                Scenarios: scenariosForApi, // Cenários estruturados para a API
                Observations: data.description?.trim() || null, // Observações (usando o campo description)
                Attachments: attachments.map(att => ({ // Evidências
                    Name: att.name,
                    Size: att.size,
                    Type: att.type
                }))
            };

            console.log('Enviando dados para API:', issueData); // Debug

            // Enviar para a API
            const response = await issueService.create(issueData);

            // A API retorna um ApiResponseDto<T> com propriedade success
            const isSuccess = response && response.success === true;

            if (isSuccess) {
                showSuccess('Issue registrada!', 'Issue registrada com sucesso!');

                // Resetar o formulário para registrar uma nova issue
                resetFormToInitialState();
            } else {
                // Se não tem success=true, tratar como erro
                const errorMessage = response?.Message || response?.message || 'Erro ao registrar issue';
                const errorList = response?.Errors || response?.errors || [];
                throw new Error(JSON.stringify({ message: errorMessage, errors: errorList }));
            }
        } catch (error) {

            // Tratamento específico para diferentes tipos de erro
            if (error.response?.status === 500) {
                // Erro interno do servidor (500)
                const errorData = error.response.data;

                showError('Erro interno do servidor',
                    errorData?.message || errorData?.Message ||
                    'Erro interno no servidor. Verifique os logs da API.');
            } else if (error.response?.status === 400) {
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
                // Se errors é um objeto com propriedades
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
                showError('Erro ao registrar', 'Erro ao registrar issue. Tente novamente.');
            }
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleCancel = () => {
        navigate('/dashboard');
    };

    const getTypeColor = (type) => {
        const typeObj = issueTypes.find(t => t.value === type);
        return typeObj?.color || '#757575';
    };

    const getPriorityColor = (priority) => {
        const priorityObj = priorities.find(p => p.value === priority);
        return priorityObj?.color || '#757575';
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
                            Registrar Nova Issue
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
                        <form onSubmit={(e) => {
                            e.preventDefault();
                            handleSubmit((data) => {
                                onSubmit(data);
                            })(e);
                        }}>
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
                                                >
                                                    <InputLabel>História do Usuário</InputLabel>
                                                    <Select
                                                        {...field}
                                                        label="História do Usuário"
                                                        disabled={isSubmitting || (!watchedDemand || availableStories.length === 0)}
                                                    >
                                                        <MenuItem value=" ">
                                                            <em>Nenhuma história específica</em>
                                                        </MenuItem>
                                                        {!watchedDemand ? (
                                                            <MenuItem disabled>
                                                                <Box sx={{ textAlign: 'center', py: 1 }}>
                                                                    <Typography variant="body2" color="text.secondary">
                                                                        Selecione um projeto primeiro
                                                                    </Typography>
                                                                </Box>
                                                            </MenuItem>
                                                        ) : availableStories.length === 0 ? (
                                                            <MenuItem disabled>
                                                                <Box sx={{ textAlign: 'center', py: 1 }}>
                                                                    <Typography variant="body2" color="text.secondary">
                                                                        Não existem histórias associadas ao projeto
                                                                    </Typography>
                                                                    <Typography variant="caption" color="text.secondary">
                                                                        Você pode continuar sem selecionar uma história específica
                                                                    </Typography>
                                                                </Box>
                                                            </MenuItem>
                                                        ) : (
                                                            availableStories.map((story) => (
                                                                <MenuItem key={story.id} value={story.id}>
                                                                    <Box>
                                                                        <Typography>#{story.id} - {story.title}</Typography>
                                                                        <Typography variant="caption" color="text.secondary">
                                                                            Status: {story.state} | Atribuído: {story.assignedTo}
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

                                    {/* Segunda linha: Título, Tipo, Prioridade, Tipo de Ocorrência e Ambiente */}
                                    <Box sx={{ display: 'grid', gridTemplateColumns: '3fr 1fr 1fr 2fr 1fr', gap: 2 }}>
                                        <Controller
                                            name="title"
                                            control={control}
                                            render={({ field }) => (
                                                <TextField
                                                    {...field}
                                                    fullWidth
                                                    label="Título da Issue"
                                                    error={!!errors.title}
                                                    helperText={errors.title?.message}
                                                    disabled={isSubmitting}
                                                    placeholder="Descreva brevemente a issue"
                                                    required
                                                />
                                            )}
                                        />

                                        <Controller
                                            name="type"
                                            control={control}
                                            render={({ field }) => (
                                                <FormControl fullWidth error={!!errors.type} required>
                                                    <InputLabel>Tipo</InputLabel>
                                                    <Select
                                                        {...field}
                                                        label="Tipo"
                                                        disabled={isSubmitting}
                                                        required
                                                        renderValue={(value) => (
                                                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                                                <Chip
                                                                    size="small"
                                                                    label={issueTypes.find(t => t.value === value)?.label}
                                                                    sx={{
                                                                        backgroundColor: getTypeColor(value),
                                                                        color: 'white',
                                                                        fontWeight: 'bold'
                                                                    }}
                                                                />
                                                            </Box>
                                                        )}
                                                    >
                                                        {issueTypes.map((type) => (
                                                            <MenuItem key={type.value} value={type.value}>
                                                                <Chip
                                                                    size="small"
                                                                    label={type.label}
                                                                    sx={{
                                                                        backgroundColor: type.color,
                                                                        color: 'white',
                                                                        fontWeight: 'bold'
                                                                    }}
                                                                />
                                                            </MenuItem>
                                                        ))}
                                                    </Select>
                                                    {errors.type && (
                                                        <Typography variant="caption" color="error" sx={{ mt: 1 }}>
                                                            {errors.type.message}
                                                        </Typography>
                                                    )}
                                                </FormControl>
                                            )}
                                        />

                                        <Controller
                                            name="priority"
                                            control={control}
                                            render={({ field }) => (
                                                <FormControl fullWidth error={!!errors.priority} required>
                                                    <InputLabel>Prioridade</InputLabel>
                                                    <Select
                                                        {...field}
                                                        label="Prioridade"
                                                        disabled={isSubmitting}
                                                        required
                                                        renderValue={(value) => (
                                                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                                                <Chip
                                                                    size="small"
                                                                    label={priorities.find(p => p.value === value)?.label}
                                                                    sx={{
                                                                        backgroundColor: getPriorityColor(value),
                                                                        color: 'white',
                                                                        fontWeight: 'bold'
                                                                    }}
                                                                />
                                                            </Box>
                                                        )}
                                                    >
                                                        {priorities.map((priority) => (
                                                            <MenuItem key={priority.value} value={priority.value}>
                                                                <Chip
                                                                    size="small"
                                                                    label={priority.label}
                                                                    sx={{
                                                                        backgroundColor: priority.color,
                                                                        color: 'white',
                                                                        fontWeight: 'bold'
                                                                    }}
                                                                />
                                                            </MenuItem>
                                                        ))}
                                                    </Select>
                                                    {errors.priority && (
                                                        <Typography variant="caption" color="error" sx={{ mt: 1 }}>
                                                            {errors.priority.message}
                                                        </Typography>
                                                    )}
                                                </FormControl>
                                            )}
                                        />

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

                            {/* Cenários da Issue */}
                            <Section
                                title="Cenários da Issue"
                            >
                                <ScenarioDetailsFields
                                    scenarios={scenarioDetails}
                                    onAdd={handleAddScenario}
                                    onRemove={handleRemoveScenario}
                                    onScenarioChange={handleScenarioChange}
                                    disabled={isSubmitting}
                                    errors={errors}
                                />
                            </Section>

                            {/* Descrição Detalhada */}
                            <Section title="Descrição Detalhada">
                                <Controller
                                    name="description"
                                    control={control}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            fullWidth
                                            label="Descrição Detalhada"
                                            multiline
                                            rows={4}
                                            disabled={isSubmitting}
                                            placeholder="Descreva detalhadamente a issue: o que acontece, quando acontece, qual o impacto, passos para reproduzir, etc."
                                            error={!!errors.description}
                                            helperText={errors.description?.message}
                                        />
                                    )}
                                />
                            </Section>

                            {/* Anexos */}
                            <Section title="Anexos">
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
                                            Anexar Arquivos
                                        </Button>
                                    </label>
                                    <Typography variant="caption" display="block" sx={{ mt: 1, color: 'text.secondary' }}>
                                        Anexe screenshots, logs, documentos relacionados à issue
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
                                    {isSubmitting ? 'Registrando...' : 'Registrar Issue'}
                                </Button>
                            </Box>
                        </form>
                    </Paper>
                </Box>
            </Container>
        </>
    );
}
