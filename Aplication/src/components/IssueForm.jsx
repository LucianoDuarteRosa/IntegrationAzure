import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Navbar } from './Navbar';
import { useNotifications } from '../hooks/useNotifications';
import { issueService } from '../services/issueService';
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
    demandNumber: yup.string().required('Demanda é obrigatória'),
    userStoryId: yup.string(), // Opcional - pode não ter história associada
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
    ).test('at-least-one-complete', 'Pelo menos um cenário completo é obrigatório', function (value) {
        if (!value || value.length === 0) return false;
        return value.some(scenario =>
            scenario.given?.trim() &&
            scenario.when?.trim() &&
            scenario.then?.trim()
        );
    }),
});

// Dados simulados para demandas e histórias
const demandas = [
    { id: 'DEM-001', title: 'Demanda 1' },
    { id: 'DEM-002', title: 'Demanda 2' },
    { id: 'DEM-003', title: 'Demanda 3' },
];

const historiasPorDemanda = {
    'DEM-001': [
        { id: '550e8400-e29b-41d4-a716-446655440001', title: 'Como usuário, quero fazer login no sistema' },
        { id: '550e8400-e29b-41d4-a716-446655440002', title: 'Como usuário, quero recuperar minha senha' },
        { id: '550e8400-e29b-41d4-a716-446655440003', title: 'Como admin, quero gerenciar permissões' },
    ],
    'DEM-002': [
        { id: '550e8400-e29b-41d4-a716-446655440004', title: 'Como usuário, quero criar um relatório' },
        { id: '550e8400-e29b-41d4-a716-446655440005', title: 'Como usuário, quero exportar dados' },
        { id: '550e8400-e29b-41d4-a716-446655440006', title: 'Como gestor, quero visualizar dashboard' },
    ],
    'DEM-003': [
        { id: '550e8400-e29b-41d4-a716-446655440007', title: 'Como usuário, quero integrar com API externa' },
        { id: '550e8400-e29b-41d4-a716-446655440008', title: 'Como admin, quero configurar webhooks' },
        { id: '550e8400-e29b-41d4-a716-446655440009', title: 'Como dev, quero monitorar performance' },
    ],
};

const issueTypes = [
    { value: 1, label: 'Bug', color: '#f44336' },
    { value: 2, label: 'Feature', color: '#4caf50' },
    { value: 3, label: 'Improvement', color: '#2196f3' },
    { value: 4, label: 'Task', color: '#ff9800' },
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
                            required
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
                            required
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
                    Adicionar Cenário
                </Button>
            </Box>

            {/* Exibe erro geral dos cenários */}
            {errors?.scenarioDetails?.message && (
                <Alert severity="error" sx={{ mt: 2 }}>
                    {errors.scenarioDetails.message}
                </Alert>
            )}
        </Stack>
    );
};

export function IssueForm() {
    const navigate = useNavigate();
    const { showSuccess, showError } = useNotifications();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [attachments, setAttachments] = useState([]);
    const [scenarioDetails, setScenarioDetails] = useState([
        { given: '', when: '', then: '' }
    ]);
    const [selectedDemand, setSelectedDemand] = useState('');
    const [availableStories, setAvailableStories] = useState([]);

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

    // Atualiza histórias disponíveis quando a demanda muda (usando dados fictícios)
    useEffect(() => {
        if (watchedDemand) {
            setSelectedDemand(watchedDemand);
            const stories = historiasPorDemanda[watchedDemand] || [];
            setAvailableStories(stories);
            // Não limpa a história selecionada para permitir "Nenhuma história específica"
        } else {
            setAvailableStories([]);
        }
    }, [watchedDemand, setValue]);

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
            // Preparar cenários completos
            const completedScenarios = scenarioDetails.filter(scenario =>
                scenario.given?.trim() &&
                scenario.when?.trim() &&
                scenario.then?.trim()
            );

            // Gerar descrição baseada nos cenários
            let description = data.description?.trim() || '';
            if (completedScenarios.length > 0) {
                description += '\n\n## Cenários:\n\n';
                completedScenarios.forEach((scenario, index) => {
                    description += `**Cenário ${index + 1}:**\n`;
                    description += `- **Contexto:** ${scenario.given}\n`;
                    description += `- **Situação:** ${scenario.when}\n`;
                    description += `- **Resultado Esperado:** ${scenario.then}\n\n`;
                });
            }

            // Preparar dados para envio seguindo o formato do CreateIssueDto
            const issueData = {
                IssueNumber: `ISS-${String(Date.now()).slice(-6)}`, // Formato: ISS-XXXXXX
                Title: data.title,
                Description: description || 'Issue registrada via sistema',
                Type: parseInt(data.type), // IssueType enum (1-4)
                Priority: parseInt(data.priority), // Priority enum (1-4)
                OccurrenceType: parseInt(data.occurrenceType), // Tipo de ocorrência (1-9)
                Environment: data.environment,
                UserStoryId: data.userStoryId || null, // null se string vazia ou GUID válido
            };

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
                                                    <InputLabel>Demanda</InputLabel>
                                                    <Select
                                                        {...field}
                                                        label="Demanda"
                                                        disabled={isSubmitting}
                                                        required
                                                    >
                                                        {demandas.map((demanda) => (
                                                            <MenuItem key={demanda.id} value={demanda.id}>
                                                                {demanda.id} - {demanda.title}
                                                            </MenuItem>
                                                        ))}
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
                                                        disabled={isSubmitting || availableStories.length === 0}
                                                    >
                                                        <MenuItem value=" ">
                                                            <em>Nenhuma história específica</em>
                                                        </MenuItem>
                                                        {availableStories.length === 0 && selectedDemand ? (
                                                            <MenuItem disabled>
                                                                Nenhuma história disponível para esta demanda
                                                            </MenuItem>
                                                        ) : (
                                                            availableStories.map((story) => (
                                                                <MenuItem key={story.id} value={story.id}>
                                                                    {story.id} - {story.title}
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

                            {/* Informações Adicionais */}
                            <Section title="Informações Adicionais">
                                <Controller
                                    name="description"
                                    control={control}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            fullWidth
                                            label="Descrição Adicional"
                                            multiline
                                            rows={4}
                                            disabled={isSubmitting}
                                            placeholder="Descrição adicional, contexto, workarounds temporários, etc."
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
