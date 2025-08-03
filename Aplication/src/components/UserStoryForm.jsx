import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Navbar } from './Navbar';
import { userStoryService } from '../services/userStoryService';
import { azureDevOpsService } from '../services/azureDevOpsService';
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
    Checkbox,
    FormControlLabel,
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
    Visibility as VisibilityIcon,
    Assignment as AssignmentIcon,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import * as yup from 'yup';
import { yupResolver } from '@hookform/resolvers/yup';

const schema = yup.object().shape({
    demandNumber: yup.string().required('Projeto é obrigatório'),
    title: yup.string().required('Título é obrigatório'),
    priority: yup.string().required('Prioridade é obrigatória'),
});

const priorities = [
    { value: 'Low', label: 'Baixa', color: '#4caf50' },
    { value: 'Medium', label: 'Média', color: '#ff9800' },
    { value: 'High', label: 'Alta', color: '#f44336' },
    { value: 'Critical', label: 'Crítica', color: '#d32f2f' },
];

const Section = ({ title, children, notApplicable, onNotApplicableChange, isFirst }) => {
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
                justifyContent: 'space-between'
            }}>
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
                {notApplicable !== undefined && (
                    <FormControlLabel
                        control={
                            <Checkbox
                                checked={notApplicable}
                                onChange={(e) => onNotApplicableChange(e.target.checked)}
                            />
                        }
                        label="Não se aplica"
                        sx={{ ml: 2 }}
                    />
                )}
            </Box>
            {!notApplicable && children}
        </Box>
    );
};

export function UserStoryForm() {
    const navigate = useNavigate();
    const { showSuccess, showError } = useNotifications();
    const [files, setFiles] = useState({
        notApplicable: false,
        items: []
    });
    const [screenshots, setScreenshots] = useState({
        notApplicable: false,
        items: []
    });
    const [userStory, setUserStory] = useState({
        como: '',
        quero: '',
        para: ''
    });
    const [impact, setImpact] = useState({
        notApplicable: false,
        items: [
            {
                id: 1,
                current: '',
                expected: '',
            },
        ],
    });
    const [objective, setObjective] = useState({
        notApplicable: false,
        fields: [{ id: 1, content: '' }],
    });
    const [fields, setFields] = useState({
        notApplicable: false,
        items: [
            {
                id: 1,
                name: '',
                type: 'text',
                size: '',
                required: false,
            },
        ],
    });
    const [messages, setMessages] = useState({
        notApplicable: false,
        items: [{ id: 1, content: '' }],
    });
    const [rules, setRules] = useState({
        notApplicable: false,
        items: [{ id: 1, content: '' }],
    });
    const [scenarios, setScenarios] = useState({
        notApplicable: false,
        items: [
            {
                id: 1,
                given: '',
                when: '',
                then: '',
            },
        ],
    });
    const [acceptanceCriteria, setAcceptanceCriteria] = useState('');
    const [azureProjects, setAzureProjects] = useState([]);
    const [loadingProjects, setLoadingProjects] = useState(false);

    const [loading, setLoading] = useState(false);

    const { control, handleSubmit, formState: { errors }, reset } = useForm({
        resolver: yupResolver(schema)
    });

    // Carregar projetos do Azure DevOps ao inicializar o componente
    useEffect(() => {
        loadAzureProjects();
    }, []);

    const loadAzureProjects = async () => {
        setLoadingProjects(true);
        try {
            const projects = await azureDevOpsService.getProjects();
            setAzureProjects(projects);
        } catch (error) {
            console.error('Erro ao carregar projetos do Azure:', error);
            showError('Erro ao carregar projetos', 'Não foi possível carregar os projetos do Azure DevOps. Verifique as configurações.');
            // Em caso de erro, usar projetos mock para não quebrar a funcionalidade;
        } finally {
            setLoadingProjects(false);
        }
    };

    // Função para resetar todo o formulário
    const resetFormToInitialState = () => {
        // Reset do react-hook-form
        reset({
            demandNumber: '',
            title: '',
            priority: 'Medium'
        });

        // Reset dos estados locais
        setFiles({
            notApplicable: false,
            items: []
        });
        setScreenshots({
            notApplicable: false,
            items: []
        });
        setUserStory({
            como: '',
            quero: '',
            para: ''
        });
        setImpact({
            notApplicable: false,
            items: [
                {
                    id: 1,
                    current: '',
                    expected: '',
                },
            ],
        });
        setObjective({
            notApplicable: false,
            fields: [{ id: 1, content: '' }],
        });
        setFields({
            notApplicable: false,
            items: [
                {
                    id: 1,
                    name: '',
                    type: 'text',
                    size: '',
                    required: false,
                },
            ],
        });
        setMessages({
            notApplicable: false,
            items: [{ id: 1, content: '' }],
        });
        setRules({
            notApplicable: false,
            items: [{ id: 1, content: '' }],
        });
        setScenarios({
            notApplicable: false,
            items: [
                {
                    id: 1,
                    given: '',
                    when: '',
                    then: '',
                },
            ],
        });
        setAcceptanceCriteria('');
    };

    const handleFileChange = (e) => {
        if (!files.notApplicable) {
            const newFiles = Array.from(e.target.files);
            setFiles({
                ...files,
                items: [...files.items, ...newFiles]
            });
        }
    };

    const onSubmit = async (data) => {
        setLoading(true);

        try {
            // Validação personalizada para critérios de aceite
            if (!acceptanceCriteria.trim()) {
                showError('Dados obrigatórios', 'Critérios de aceite são obrigatórios');
                setLoading(false);
                return;
            }

            // Mapeamento dos valores de prioridade para os números do enum
            const priorityMap = {
                'Low': 1,
                'Medium': 2,
                'High': 3,
                'Critical': 4
            };

            // Preparar os dados completos para enviar para a API
            const userStoryData = {
                DemandNumber: data.demandNumber,
                Title: data.title,
                Priority: priorityMap[data.priority] || 2, // Default para Medium
                AcceptanceCriteria: acceptanceCriteria,

                // História do usuário (como/quero/para)
                UserStory: {
                    Como: userStory.como,
                    Quero: userStory.quero,
                    Para: userStory.para
                },

                // Seções opcionais
                Impact: impact.notApplicable ? null : {
                    Items: impact.items.filter(item => item.current.trim() || item.expected.trim()).map(item => ({
                        Current: item.current,
                        Expected: item.expected
                    }))
                },

                Objective: objective.notApplicable ? null : {
                    Fields: objective.fields.filter(field => field.content.trim()).map(field => ({
                        Content: field.content
                    }))
                },

                Screenshots: screenshots.notApplicable ? null : {
                    Items: screenshots.items.map(file => ({
                        Name: file.name,
                        Size: file.size,
                        Type: file.type
                    }))
                },

                FormFields: fields.notApplicable ? null : {
                    Items: fields.items.filter(field => field.name.trim()).map(field => ({
                        Name: field.name,
                        Type: field.type,
                        Size: field.size,
                        Required: field.required
                    }))
                },

                Messages: messages.notApplicable ? null : {
                    Items: messages.items.filter(item => item.content.trim()).map(item => ({
                        Content: item.content
                    }))
                },

                BusinessRules: rules.notApplicable ? null : {
                    Items: rules.items.filter(item => item.content.trim()).map(item => ({
                        Content: item.content
                    }))
                },

                Scenarios: scenarios.notApplicable ? null : {
                    Items: scenarios.items.filter(scenario =>
                        scenario.given.trim() || scenario.when.trim() || scenario.then.trim()
                    ).map(scenario => ({
                        Given: scenario.given,
                        When: scenario.when,
                        Then: scenario.then
                    }))
                },

                Attachments: files.notApplicable ? null : {
                    Items: files.items.map(file => ({
                        Name: file.name,
                        Size: file.size,
                        Type: file.type
                    }))
                }
            };

            await userStoryService.create(userStoryData);

            // Mostrar notificação de sucesso
            showSuccess('História criada!', 'História de usuário criada com sucesso!');

            // Resetar o formulário para criar uma nova história
            resetFormToInitialState();

        } catch (err) {
            // Tratamento específico para diferentes tipos de erro
            if (err.errors && Array.isArray(err.errors) && err.errors.length > 0) {
                showError('Erro de validação', err.errors.join(', '));
            } else if (err.message) {
                showError('Erro na operação', err.message);
            } else if (err.response) {
                // Erro de resposta da API
                const apiError = err.response.data;
                if (apiError.errors && apiError.errors.length > 0) {
                    showError('Erro de validação', apiError.errors.join(', '));
                } else {
                    showError('Erro na API', apiError.message || 'Erro na resposta da API');
                }
            } else {
                showError('Erro ao criar', 'Erro ao criar história de usuário');
            }
        } finally {
            setLoading(false);
        }
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
                            <AssignmentIcon sx={{ fontSize: '2.5rem' }} />
                            Nova História
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
                        <Box component="form" onSubmit={handleSubmit(onSubmit)}>
                            {/* Informações Básicas */}
                            <Box sx={{ mb: 4 }}>
                                <Typography variant="h5" component="h2" sx={{ mb: 3, fontWeight: 'bold' }}>
                                    Informações Básicas
                                </Typography>
                                <Box sx={{
                                    display: 'grid',
                                    gridTemplateColumns: '2fr 5fr 1fr',
                                    gap: 3,
                                    '@media (max-width: 900px)': {
                                        gridTemplateColumns: '1fr',
                                        gap: 2
                                    }
                                }}>
                                    <Box sx={{ display: 'flex', gap: 1, alignItems: 'flex-start' }}>
                                        <Controller
                                            name="demandNumber"
                                            control={control}
                                            defaultValue=""
                                            render={({ field }) => (
                                                <FormControl fullWidth error={!!errors.demandNumber}>
                                                    <InputLabel>Projeto do Azure DevOps</InputLabel>
                                                    <Select
                                                        {...field}
                                                        label="Projeto do Azure DevOps"
                                                        disabled={loading || loadingProjects}
                                                    >
                                                        {loadingProjects ? (
                                                            <MenuItem disabled>
                                                                <CircularProgress size={20} sx={{ mr: 1 }} />
                                                                Carregando projetos...
                                                            </MenuItem>
                                                        ) : azureProjects.length === 0 ? (
                                                            <MenuItem disabled>
                                                                Nenhum projeto disponível
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
                                                </FormControl>
                                            )}
                                        />
                                    </Box>
                                    <Controller
                                        name="title"
                                        control={control}
                                        defaultValue=""
                                        render={({ field }) => (
                                            <TextField
                                                {...field}
                                                fullWidth
                                                label="Título"
                                                error={!!errors.title}
                                                helperText={errors.title?.message}
                                            />
                                        )}
                                    />
                                    <Controller
                                        name="priority"
                                        control={control}
                                        defaultValue="Medium"
                                        render={({ field }) => (
                                            <FormControl fullWidth error={!!errors.priority}>
                                                <InputLabel>Prioridade</InputLabel>
                                                <Select
                                                    {...field}
                                                    label="Prioridade"
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
                                                    <Typography variant="caption" color="error" sx={{ mt: 1, ml: 2 }}>
                                                        {errors.priority?.message}
                                                    </Typography>
                                                )}
                                            </FormControl>
                                        )}
                                    />
                                </Box>
                            </Box>

                            <Divider sx={{ my: 4 }} />

                            <Section title="História do Usuário" isFirst>
                                <Stack spacing={3}>
                                    <TextField
                                        fullWidth
                                        label="Como"
                                        value={userStory.como}
                                        onChange={(e) => setUserStory({ ...userStory, como: e.target.value })}
                                        multiline
                                        rows={3}
                                        placeholder="Ex: Como um usuário do sistema..."
                                        required
                                    />
                                    <TextField
                                        fullWidth
                                        label="Quero"
                                        value={userStory.quero}
                                        onChange={(e) => setUserStory({ ...userStory, quero: e.target.value })}
                                        multiline
                                        rows={3}
                                        placeholder="Ex: Quero poder realizar..."
                                        required
                                    />
                                    <TextField
                                        fullWidth
                                        label="Para"
                                        value={userStory.para}
                                        onChange={(e) => setUserStory({ ...userStory, para: e.target.value })}
                                        multiline
                                        rows={3}
                                        placeholder="Ex: Para que eu possa..."
                                        required
                                    />
                                </Stack>
                            </Section>

                            <Section
                                title="Critérios de Aceite"
                            >
                                <TextField
                                    fullWidth
                                    multiline
                                    rows={4}
                                    value={acceptanceCriteria}
                                    onChange={(e) => setAcceptanceCriteria(e.target.value)}
                                    required
                                />
                            </Section>

                            <Section
                                title="Impacto"
                                notApplicable={impact.notApplicable}
                                onNotApplicableChange={(checked) => setImpact({ ...impact, notApplicable: checked })}
                            >
                                <Stack spacing={3}>
                                    {impact.items.map((item, index) => (
                                        <Paper
                                            key={item.id}
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
                                                {impact.items.length > 1 && (
                                                    <IconButton
                                                        onClick={() => setImpact({
                                                            ...impact,
                                                            items: impact.items.filter(i => i.id !== item.id)
                                                        })}
                                                        disabled={impact.notApplicable}
                                                        size="small"
                                                        sx={{ color: '#d32f2f' }}
                                                    >
                                                        <DeleteIcon />
                                                    </IconButton>
                                                )}
                                            </Box>
                                            <Stack spacing={2}>
                                                <TextField
                                                    fullWidth
                                                    label="Processo Atual"
                                                    value={item.current}
                                                    onChange={(e) => setImpact({
                                                        ...impact,
                                                        items: impact.items.map((i) =>
                                                            i.id === item.id
                                                                ? { ...i, current: e.target.value }
                                                                : i
                                                        )
                                                    })}
                                                    disabled={impact.notApplicable}
                                                    multiline
                                                    rows={3}
                                                    placeholder="Ex: Atualmente o processo é realizado de forma manual..."
                                                />
                                                <TextField
                                                    fullWidth
                                                    label="Melhoria Esperada"
                                                    value={item.expected}
                                                    onChange={(e) => setImpact({
                                                        ...impact,
                                                        items: impact.items.map((i) =>
                                                            i.id === item.id
                                                                ? { ...i, expected: e.target.value }
                                                                : i
                                                        )
                                                    })}
                                                    disabled={impact.notApplicable}
                                                    multiline
                                                    rows={3}
                                                    placeholder="Ex: Com a automação, esperamos que..."
                                                />
                                            </Stack>
                                        </Paper>
                                    ))}
                                    <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                                        <Button
                                            startIcon={<AddIcon />}
                                            onClick={() => setImpact({
                                                ...impact,
                                                items: [
                                                    ...impact.items,
                                                    {
                                                        id: Date.now(),
                                                        current: '',
                                                        expected: '',
                                                    }
                                                ]
                                            })}
                                            variant="outlined"
                                            disabled={impact.notApplicable}
                                            sx={{ width: '50%' }}
                                        >
                                            Adicionar Impacto
                                        </Button>
                                    </Box>
                                </Stack>
                            </Section>

                            <Section
                                title="Objetivo"
                                notApplicable={objective.notApplicable}
                                onNotApplicableChange={(checked) => setObjective({ ...objective, notApplicable: checked })}
                            >
                                <Stack spacing={3}>
                                    {objective.fields.map((field, index) => (
                                        <Paper
                                            key={field.id}
                                            sx={{
                                                p: 3,
                                                border: '1px solid',
                                                borderColor: 'divider',
                                                backgroundColor: 'background.default'
                                            }}
                                        >
                                            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                                                <Typography variant="subtitle1" fontWeight="medium">
                                                    Objetivo {index + 1}
                                                </Typography>
                                                {objective.fields.length > 1 && (
                                                    <IconButton
                                                        onClick={() => setObjective({
                                                            ...objective,
                                                            fields: objective.fields.filter((_, i) => i !== index)
                                                        })}
                                                        disabled={objective.notApplicable}
                                                        size="small"
                                                        sx={{ color: '#d32f2f' }}
                                                    >
                                                        <DeleteIcon />
                                                    </IconButton>
                                                )}
                                            </Box>
                                            <TextField
                                                fullWidth
                                                multiline
                                                rows={3}
                                                value={field.content}
                                                onChange={(e) => setObjective({
                                                    ...objective,
                                                    fields: objective.fields.map((item, i) =>
                                                        i === index ? { ...item, content: e.target.value } : item
                                                    )
                                                })}
                                                disabled={objective.notApplicable}
                                                placeholder="Digite o objetivo aqui..."
                                            />
                                        </Paper>
                                    ))}
                                    <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                                        <Button
                                            startIcon={<AddIcon />}
                                            onClick={() => setObjective({
                                                ...objective,
                                                fields: [...objective.fields, { id: objective.fields.length + 1, content: '' }]
                                            })}
                                            variant="outlined"
                                            disabled={objective.notApplicable}
                                            sx={{ width: '50%' }}
                                        >
                                            Adicionar Objetivo
                                        </Button>
                                    </Box>
                                </Stack>
                            </Section>

                            <Section
                                title="Campos de Preenchimento"
                                notApplicable={fields.notApplicable}
                                onNotApplicableChange={(checked) => setFields({ ...fields, notApplicable: checked })}
                            >
                                <Stack spacing={3}>
                                    {fields.items.map((field, index) => (
                                        <Paper
                                            key={field.id}
                                            sx={{
                                                p: 3,
                                                border: '1px solid',
                                                borderColor: 'divider',
                                                backgroundColor: 'background.default'
                                            }}
                                        >
                                            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                                                <Typography variant="subtitle1" fontWeight="medium">
                                                    Campo {index + 1}
                                                </Typography>
                                                {fields.items.length > 1 && (
                                                    <IconButton
                                                        onClick={() => setFields({
                                                            ...fields,
                                                            items: fields.items.filter((f) => f.id !== field.id)
                                                        })}
                                                        disabled={fields.notApplicable}
                                                        size="small"
                                                        sx={{ color: '#d32f2f' }}
                                                    >
                                                        <DeleteIcon />
                                                    </IconButton>
                                                )}
                                            </Box>
                                            <Grid container spacing={2}>
                                                <Grid item xs={12} sm={3}>
                                                    <TextField
                                                        fullWidth
                                                        label="Nome do Campo"
                                                        value={field.name}
                                                        onChange={(e) => setFields({
                                                            ...fields,
                                                            items: fields.items.map((f) =>
                                                                f.id === field.id ? { ...f, name: e.target.value } : f
                                                            )
                                                        })}
                                                        disabled={fields.notApplicable}
                                                    />
                                                </Grid>
                                                <Grid item xs={12} sm={2}>
                                                    <FormControl fullWidth disabled={fields.notApplicable}>
                                                        <InputLabel>Tipo</InputLabel>
                                                        <Select
                                                            value={field.type}
                                                            label="Tipo"
                                                            onChange={(e) => {
                                                                const newType = e.target.value;
                                                                const newSize = ['boolean', 'date', 'datetime'].includes(newType) ? '' : field.size;
                                                                setFields({
                                                                    ...fields,
                                                                    items: fields.items.map((f) =>
                                                                        f.id === field.id ? { ...f, type: newType, size: newSize } : f
                                                                    )
                                                                });
                                                            }}
                                                        >
                                                            <MenuItem value="text">Campo de Texto</MenuItem>
                                                            <MenuItem value="number">Número (Inteiro/Decimal)</MenuItem>
                                                            <MenuItem value="date">Data</MenuItem>
                                                            <MenuItem value="datetime">Data e Hora</MenuItem>
                                                            <MenuItem value="boolean">Sim/Não</MenuItem>
                                                            <MenuItem value="select">Lista de Opções</MenuItem>
                                                        </Select>
                                                    </FormControl>
                                                </Grid>
                                                <Grid item xs={12} sm={2}>
                                                    {!['boolean', 'date', 'datetime', 'select'].includes(field.type) && (
                                                        <TextField
                                                            fullWidth
                                                            label={field.type === 'number' ? 'Dígitos (total.decimais)' : 'Tamanho máximo'}
                                                            placeholder={field.type === 'number' ? 'Ex: 10.2' : 'Ex: 100'}
                                                            value={field.size}
                                                            onChange={(e) => setFields({
                                                                ...fields,
                                                                items: fields.items.map((f) =>
                                                                    f.id === field.id ? { ...f, size: e.target.value } : f
                                                                )
                                                            })}
                                                            disabled={fields.notApplicable}
                                                        />
                                                    )}
                                                </Grid>
                                                <Grid item xs={12} sm={3}>
                                                    <FormControlLabel
                                                        control={
                                                            <Checkbox
                                                                checked={field.required}
                                                                onChange={(e) => setFields({
                                                                    ...fields,
                                                                    items: fields.items.map((f) =>
                                                                        f.id === field.id ? { ...f, required: e.target.checked } : f
                                                                    )
                                                                })}
                                                                disabled={fields.notApplicable}
                                                            />
                                                        }
                                                        label="Obrigatório"
                                                    />
                                                </Grid>
                                            </Grid>
                                        </Paper>
                                    ))}
                                    <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                                        <Button
                                            startIcon={<AddIcon />}
                                            onClick={() => setFields({
                                                ...fields,
                                                items: [
                                                    ...fields.items,
                                                    {
                                                        id: Date.now(),
                                                        name: '',
                                                        type: 'text',
                                                        size: '',
                                                        required: false,
                                                    }
                                                ]
                                            })}
                                            variant="outlined"
                                            disabled={fields.notApplicable}
                                            sx={{ width: '50%' }}
                                        >
                                            Adicionar Campo
                                        </Button>
                                    </Box>
                                </Stack>
                            </Section>

                            <Section
                                title="Mensagens Informativas"
                                notApplicable={messages.notApplicable}
                                onNotApplicableChange={(checked) => setMessages({ ...messages, notApplicable: checked })}
                            >
                                <Stack spacing={3}>
                                    {messages.items.map((message, index) => (
                                        <Paper
                                            key={message.id}
                                            sx={{
                                                p: 3,
                                                border: '1px solid',
                                                borderColor: 'divider',
                                                backgroundColor: 'background.default'
                                            }}
                                        >
                                            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                                                <Typography variant="subtitle1" fontWeight="medium">
                                                    Mensagem Informativa {index + 1}
                                                </Typography>
                                                {messages.items.length > 1 && (
                                                    <IconButton
                                                        onClick={() => setMessages({
                                                            ...messages,
                                                            items: messages.items.filter((_, i) => i !== index)
                                                        })}
                                                        disabled={messages.notApplicable}
                                                        size="small"
                                                        sx={{ color: '#d32f2f' }}
                                                    >
                                                        <DeleteIcon />
                                                    </IconButton>
                                                )}
                                            </Box>
                                            <TextField
                                                fullWidth
                                                multiline
                                                rows={3}
                                                value={message.content}
                                                onChange={(e) => setMessages({
                                                    ...messages,
                                                    items: messages.items.map((item, i) =>
                                                        i === index ? { ...item, content: e.target.value } : item
                                                    )
                                                })}
                                                disabled={messages.notApplicable}
                                                placeholder="Digite a mensagem informativa aqui..."
                                            />
                                        </Paper>
                                    ))}
                                    <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                                        <Button
                                            startIcon={<AddIcon />}
                                            onClick={() => setMessages({
                                                ...messages,
                                                items: [...messages.items, { id: messages.items.length + 1, content: '' }]
                                            })}
                                            variant="outlined"
                                            disabled={messages.notApplicable}
                                            sx={{ width: '50%' }}
                                        >
                                            Adicionar Mensagem
                                        </Button>
                                    </Box>
                                </Stack>
                            </Section>

                            <Section
                                title="Regras de Negócio"
                                notApplicable={rules.notApplicable}
                                onNotApplicableChange={(checked) => setRules({ ...rules, notApplicable: checked })}
                            >
                                <Stack spacing={3}>
                                    {rules.items.map((rule, index) => (
                                        <Paper
                                            key={rule.id}
                                            sx={{
                                                p: 3,
                                                border: '1px solid',
                                                borderColor: 'divider',
                                                backgroundColor: 'background.default'
                                            }}
                                        >
                                            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                                                <Typography variant="subtitle1" fontWeight="medium">
                                                    Regra de Negócio {index + 1}
                                                </Typography>
                                                {rules.items.length > 1 && (
                                                    <IconButton
                                                        onClick={() => setRules({
                                                            ...rules,
                                                            items: rules.items.filter((_, i) => i !== index)
                                                        })}
                                                        disabled={rules.notApplicable}
                                                        size="small"
                                                        sx={{ color: '#d32f2f' }}
                                                    >
                                                        <DeleteIcon />
                                                    </IconButton>
                                                )}
                                            </Box>
                                            <TextField
                                                fullWidth
                                                multiline
                                                rows={3}
                                                value={rule.content}
                                                onChange={(e) => setRules({
                                                    ...rules,
                                                    items: rules.items.map((item, i) =>
                                                        i === index ? { ...item, content: e.target.value } : item
                                                    )
                                                })}
                                                disabled={rules.notApplicable}
                                                placeholder="Digite a regra de negócio aqui..."
                                            />
                                        </Paper>
                                    ))}
                                    <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                                        <Button
                                            startIcon={<AddIcon />}
                                            onClick={() => setRules({
                                                ...rules,
                                                items: [...rules.items, { id: rules.items.length + 1, content: '' }]
                                            })}
                                            variant="outlined"
                                            disabled={rules.notApplicable}
                                            sx={{ width: '50%' }}
                                        >
                                            Adicionar Regra
                                        </Button>
                                    </Box>
                                </Stack>
                            </Section>

                            <Section
                                title="Cenários"
                                notApplicable={scenarios.notApplicable}
                                onNotApplicableChange={(checked) => setScenarios({ ...scenarios, notApplicable: checked })}
                            >
                                <Stack spacing={3}>
                                    {scenarios.items.map((scenario, index) => (
                                        <Paper
                                            key={scenario.id}
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
                                                {scenarios.items.length > 1 && (
                                                    <IconButton
                                                        onClick={() => setScenarios({
                                                            ...scenarios,
                                                            items: scenarios.items.filter((s) => s.id !== scenario.id)
                                                        })}
                                                        disabled={scenarios.notApplicable}
                                                        size="small"
                                                        sx={{ color: '#d32f2f' }}
                                                    >
                                                        <DeleteIcon />
                                                    </IconButton>
                                                )}
                                            </Box>
                                            <Stack spacing={2}>
                                                <TextField
                                                    fullWidth
                                                    label="Dado que"
                                                    value={scenario.given}
                                                    onChange={(e) => setScenarios({
                                                        ...scenarios,
                                                        items: scenarios.items.map((s) =>
                                                            s.id === scenario.id
                                                                ? { ...s, given: e.target.value }
                                                                : s
                                                        )
                                                    })}
                                                    disabled={scenarios.notApplicable}
                                                    multiline
                                                    rows={2}
                                                    placeholder="Ex: Dado que estou na tela de cadastro..."
                                                />
                                                <TextField
                                                    fullWidth
                                                    label="Quando"
                                                    value={scenario.when}
                                                    onChange={(e) => setScenarios({
                                                        ...scenarios,
                                                        items: scenarios.items.map((s) =>
                                                            s.id === scenario.id
                                                                ? { ...s, when: e.target.value }
                                                                : s
                                                        )
                                                    })}
                                                    disabled={scenarios.notApplicable}
                                                    multiline
                                                    rows={2}
                                                    placeholder="Ex: Quando preencho todos os campos..."
                                                />
                                                <TextField
                                                    fullWidth
                                                    label="Então"
                                                    value={scenario.then}
                                                    onChange={(e) => setScenarios({
                                                        ...scenarios,
                                                        items: scenarios.items.map((s) =>
                                                            s.id === scenario.id
                                                                ? { ...s, then: e.target.value }
                                                                : s
                                                        )
                                                    })}
                                                    disabled={scenarios.notApplicable}
                                                    multiline
                                                    rows={2}
                                                    placeholder="Ex: Então o sistema deve..."
                                                />
                                            </Stack>
                                        </Paper>
                                    ))}
                                    <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                                        <Button
                                            startIcon={<AddIcon />}
                                            onClick={() => setScenarios({
                                                ...scenarios,
                                                items: [
                                                    ...scenarios.items,
                                                    {
                                                        id: Date.now(),
                                                        given: '',
                                                        when: '',
                                                        then: '',
                                                    }
                                                ]
                                            })}
                                            variant="outlined"
                                            disabled={scenarios.notApplicable}
                                            sx={{ width: '50%' }}
                                        >
                                            Adicionar Cenário
                                        </Button>
                                    </Box>
                                </Stack>
                            </Section>

                            <Section
                                title="Telas Ilustrativas"
                                notApplicable={screenshots.notApplicable}
                                onNotApplicableChange={(checked) => setScreenshots({ ...screenshots, notApplicable: checked })}
                            >
                                <Stack spacing={2}>
                                    <input
                                        type="file"
                                        multiple
                                        accept="image/*"
                                        onChange={(e) => {
                                            const newFiles = Array.from(e.target.files);
                                            setScreenshots({
                                                ...screenshots,
                                                items: [...screenshots.items, ...newFiles]
                                            });
                                        }}
                                        disabled={screenshots.notApplicable}
                                        style={{ display: 'none' }}
                                        id="screenshots-input"
                                    />
                                    <Box sx={{ display: 'flex', justifyContent: 'flex-start', width: '100%' }}>
                                        <label htmlFor="screenshots-input">
                                            <Button
                                                variant="outlined"
                                                component="span"
                                                startIcon={<AttachFileIcon />}
                                                disabled={screenshots.notApplicable}
                                                sx={{ minWidth: '200px' }}
                                            >
                                                Anexar Imagens
                                            </Button>
                                        </label>
                                    </Box>
                                    <Typography variant="body2" color="text.secondary" sx={{ fontStyle: 'italic' }}>
                                        Anexe imagens das telas relacionadas à história (mockups, wireframes, etc.)
                                    </Typography>

                                    {screenshots.items.length > 0 && !screenshots.notApplicable && (
                                        <Box sx={{ mt: 2 }}>
                                            {screenshots.items.map((file, index) => (
                                                <Box
                                                    key={index}
                                                    sx={{
                                                        display: 'flex',
                                                        alignItems: 'center',
                                                        justifyContent: 'space-between',
                                                        p: 1,
                                                        border: '1px solid',
                                                        borderColor: 'divider',
                                                        borderRadius: 1,
                                                        mb: 1,
                                                    }}
                                                >
                                                    <Typography>{file.name}</Typography>
                                                    <Box>
                                                        <IconButton
                                                            size="small"
                                                            onClick={() => window.URL.createObjectURL(file)}
                                                            sx={{ mr: 1 }}
                                                        >
                                                            <VisibilityIcon />
                                                        </IconButton>
                                                        <IconButton
                                                            size="small"
                                                            onClick={() => {
                                                                const newItems = screenshots.items.filter((_, i) => i !== index);
                                                                setScreenshots({
                                                                    ...screenshots,
                                                                    items: newItems
                                                                });
                                                            }}
                                                            sx={{ color: '#d32f2f' }}
                                                        >
                                                            <DeleteIcon />
                                                        </IconButton>
                                                    </Box>
                                                </Box>
                                            ))}
                                        </Box>
                                    )}
                                </Stack>
                            </Section>

                            <Section
                                title="Anexos"
                                notApplicable={files.notApplicable}
                                onNotApplicableChange={(checked) => setFiles({ ...files, notApplicable: checked })}
                            >
                                <input
                                    type="file"
                                    multiple
                                    onChange={handleFileChange}
                                    style={{ display: 'none' }}
                                    id="file-input"
                                    disabled={files.notApplicable}
                                />
                                <Box sx={{ display: 'flex', justifyContent: 'flex-start', width: '100%' }}>
                                    <label htmlFor="file-input">
                                        <Button
                                            variant="outlined"
                                            component="span"
                                            startIcon={<AttachFileIcon />}
                                            disabled={files.notApplicable}
                                            sx={{ minWidth: '200px' }}
                                        >
                                            Anexar Arquivos
                                        </Button>
                                    </label>
                                </Box>
                                <Typography variant="body2" color="text.secondary" sx={{ fontStyle: 'italic', marginTop: 2 }}>
                                    Anexe documentos complementares (PDFs, planilhas, documentos de requisitos, etc.)
                                </Typography>

                                {files.items.length > 0 && !files.notApplicable && (
                                    <Box sx={{ mt: 2 }}>
                                        {files.items.map((file, index) => (
                                            <Box
                                                key={index}
                                                sx={{
                                                    display: 'flex',
                                                    alignItems: 'center',
                                                    justifyContent: 'space-between',
                                                    p: 1,
                                                    border: '1px solid',
                                                    borderColor: 'divider',
                                                    borderRadius: 1,
                                                    mb: 1,
                                                }}
                                            >
                                                <Typography>{file.name}</Typography>
                                                <IconButton
                                                    size="small"
                                                    onClick={() => {
                                                        const newItems = files.items.filter((_, i) => i !== index);
                                                        setFiles({
                                                            ...files,
                                                            items: newItems
                                                        });
                                                    }}
                                                    sx={{ color: '#d32f2f' }}
                                                >
                                                    <DeleteIcon />
                                                </IconButton>
                                            </Box>
                                        ))}
                                    </Box>
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
                                    onClick={() => navigate('/dashboard')}
                                    size="large"
                                    color="warning"
                                    sx={{ minWidth: 120 }}
                                >
                                    Cancelar
                                </Button>
                                <Button
                                    type="submit"
                                    variant="contained"
                                    color="primary"
                                    size="large"
                                    disabled={loading}
                                    sx={{ minWidth: 120 }}
                                    startIcon={loading ? <CircularProgress size={20} color="inherit" /> : <AssignmentIcon />}
                                >
                                    {loading ? 'Criando...' : 'Criar História'}
                                </Button>
                            </Box>
                        </Box>
                    </Paper>
                </Box>
            </Container>
        </>
    );
}
