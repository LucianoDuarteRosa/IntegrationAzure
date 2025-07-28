import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Navbar } from './Navbar';
import { failureService } from '../services/failureService';
import { useNotifications } from '../hooks/useNotifications';
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
    BugReport as BugReportIcon,
    Warning as WarningIcon,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import * as yup from 'yup';
import { yupResolver } from '@hookform/resolvers/yup';

const schema = yup.object().shape({
    demandNumber: yup.string().required('Número da demanda é obrigatório'),
    userStoryId: yup.string().required('História é obrigatória'),
    title: yup.string().required('Título é obrigatório'),
    severity: yup.string().required('Severidade é obrigatória'),
    environment: yup.string().required('Ambiente é obrigatório'),
    observations: yup.string(),
    givenWhenThen: yup.array().of(
        yup.object().shape({
            given: yup.string().trim().required('Campo "Dado que" é obrigatório'),
            when: yup.string().trim().required('Campo "Quando" é obrigatório'),
            then: yup.string().trim().required('Campo "Então" é obrigatório'),
        })
    ).min(1, 'Pelo menos um cenário completo é obrigatório')
        .test('at-least-one-complete', 'Pelo menos um cenário completo é obrigatório', function (value) {
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
        { id: 'US-001-1', title: 'Como usuário, quero fazer login no sistema' },
        { id: 'US-001-2', title: 'Como usuário, quero recuperar minha senha' },
        { id: 'US-001-3', title: 'Como admin, quero gerenciar permissões' },
    ],
    'DEM-002': [
        { id: 'US-002-1', title: 'Como usuário, quero criar um relatório' },
        { id: 'US-002-2', title: 'Como usuário, quero exportar dados' },
        { id: 'US-002-3', title: 'Como gestor, quero visualizar dashboard' },
    ],
    'DEM-003': [
        { id: 'US-003-1', title: 'Como usuário, quero integrar com API externa' },
        { id: 'US-003-2', title: 'Como admin, quero configurar webhooks' },
        { id: 'US-003-3', title: 'Como dev, quero monitorar performance' },
    ],
};

const severidades = [
    { value: 'Critical', label: 'Crítico', color: '#d32f2f' },
    { value: 'Normal', label: 'Normal', color: '#ff9800' },
    { value: 'NonCritical', label: 'Não Crítico', color: '#4caf50' },
    { value: 'Enhancement', label: 'Melhoria', color: '#2196f3' },
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
                            label="Dado que..."
                            multiline
                            rows={2}
                            value={scenario.given}
                            onChange={(e) => onScenarioChange(index, 'given', e.target.value)}
                            disabled={disabled}
                            placeholder="Descreva o contexto inicial da falha"
                            error={!!(errors?.givenWhenThen?.[index]?.given)}
                            helperText={errors?.givenWhenThen?.[index]?.given?.message}
                            required
                        />
                        <TextField
                            fullWidth
                            label="Quando..."
                            multiline
                            rows={2}
                            value={scenario.when}
                            onChange={(e) => onScenarioChange(index, 'when', e.target.value)}
                            disabled={disabled}
                            placeholder="Descreva a ação que causou a falha"
                            error={!!(errors?.givenWhenThen?.[index]?.when)}
                            helperText={errors?.givenWhenThen?.[index]?.when?.message}
                            required
                        />
                        <TextField
                            fullWidth
                            label="Então..."
                            multiline
                            rows={2}
                            value={scenario.then}
                            onChange={(e) => onScenarioChange(index, 'then', e.target.value)}
                            disabled={disabled}
                            placeholder="Descreva o comportamento incorreto observado"
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
                    Adicionar Cenário
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
    const { showSuccess, showError } = useNotifications();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [attachments, setAttachments] = useState([]);
    const [givenWhenThen, setGivenWhenThen] = useState([
        { given: '', when: '', then: '' }
    ]);
    const [selectedDemand, setSelectedDemand] = useState('');
    const [availableStories, setAvailableStories] = useState([]);

    const {
        control,
        handleSubmit,
        watch,
        setValue,
        setError,
        clearErrors,
        formState: { errors }
    } = useForm({
        resolver: yupResolver(schema),
        defaultValues: {
            demandNumber: '',
            userStoryId: '',
            title: '',
            severity: 'Normal',
            environment: 'Production',
            observations: '',
            givenWhenThen: [{ given: '', when: '', then: '' }],
        }
    });

    const watchedDemand = watch('demandNumber');

    // Atualiza histórias disponíveis quando a demanda muda
    useEffect(() => {
        if (watchedDemand) {
            setSelectedDemand(watchedDemand);
            setAvailableStories(historiasPorDemanda[watchedDemand] || []);
            setValue('userStoryId', ''); // Limpa a história selecionada
        } else {
            setAvailableStories([]);
        }
    }, [watchedDemand, setValue]);

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
            // Os cenários já foram validados pelo schema do Yup
            const validScenarios = data.givenWhenThen.filter(scenario =>
                scenario.given.trim() && scenario.when.trim() && scenario.then.trim()
            );

            const failureData = {
                ...data,
                failureNumber: `FAIL-${Date.now()}`, // Número gerado automaticamente
                occuredAt: new Date().toISOString(),
                stepsToReproduce: validScenarios.map((scenario, index) =>
                    `Cenário ${index + 1}:\n` +
                    `Dado que: ${scenario.given}\n` +
                    `Quando: ${scenario.when}\n` +
                    `Então: ${scenario.then}`
                ).join('\n\n'),
                reportedBy: 'usuario.atual@example.com', // Seria obtido do contexto de auth
                attachments: attachments,
            };

            // Simulação de envio (descomente quando a API estiver pronta)
            // const result = await failureService.create(failureData);

            // Simula delay da API
            await new Promise(resolve => setTimeout(resolve, 1500));

            showSuccess('Falha registrada com sucesso!');
            navigate('/dashboard');

        } catch (error) {
            console.error('Erro ao criar falha:', error);
            showError('Erro ao registrar falha. Tente novamente.');
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
                                    {/* Primeira linha: Demanda (1fr) e História (1fr) */}
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
                                                    disabled={!selectedDemand}
                                                    required
                                                >
                                                    <InputLabel>História do Usuário</InputLabel>
                                                    <Select
                                                        {...field}
                                                        label="História do Usuário"
                                                        disabled={isSubmitting || !selectedDemand}
                                                        required
                                                    >
                                                        {availableStories.map((story) => (
                                                            <MenuItem key={story.id} value={story.id}>
                                                                {story.id} - {story.title}
                                                            </MenuItem>
                                                        ))}
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
                                    <Box sx={{ display: 'grid', gridTemplateColumns: '2fr 1fr 1fr', gap: 3 }}>
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

                            {/* Cenários da Falha */}
                            <Section
                                title="Cenários da Falha"
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
