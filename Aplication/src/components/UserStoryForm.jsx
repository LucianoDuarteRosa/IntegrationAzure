import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Navbar } from './Navbar';
import {
    Box,
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
    FormGroup,
    Grid,
} from '@mui/material';
import {
    Delete as DeleteIcon,
    Add as AddIcon,
    AttachFile as AttachFileIcon,
    Visibility as VisibilityIcon,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import * as yup from 'yup';
import { yupResolver } from '@hookform/resolvers/yup';

const schema = yup.object().shape({
    demandNumber: yup.string().required('Número da demanda é obrigatório'),
    title: yup.string().required('Título é obrigatório'),
});

// Simulação de demandas disponíveis
const demandas = [
    { id: 'DEM-001', title: 'Demanda 1' },
    { id: 'DEM-002', title: 'Demanda 2' },
    { id: 'DEM-003', title: 'Demanda 3' },
];

const Section = ({ title, children, notApplicable, onNotApplicableChange, isFirst }) => {
    return (
        <Box sx={{ mb: 4, minWidth: '900px' }}>
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

const DynamicFields = ({ fields, onAdd, onRemove, onFieldChange, disabled }) => {
    return (
        <Stack spacing={2}>
            {fields.map((field, index) => (
                <Box key={index} sx={{ display: 'flex', gap: 1 }}>
                    <TextField
                        fullWidth
                        multiline
                        rows={2}
                        value={field.content}
                        onChange={(e) => onFieldChange(index, e.target.value)}
                        disabled={disabled}
                    />
                    <IconButton
                        onClick={() => onRemove(index)}
                        disabled={fields.length === 1 || disabled}
                    >
                        <DeleteIcon />
                    </IconButton>
                </Box>
            ))}
            <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                <Button
                    startIcon={<AddIcon />}
                    onClick={onAdd}
                    variant="outlined"
                    disabled={disabled}
                    sx={{ width: '50%' }}
                >
                    Adicionar
                </Button>
            </Box>
        </Stack>
    );
};

const FieldDefinition = ({ field, onChange, onRemove }) => (
    <Grid container spacing={2} sx={{ mb: 2 }}>
        <Grid item xs={12} sm={3}>
            <TextField
                fullWidth
                label="Nome do Campo"
                value={field.name}
                onChange={(e) => onChange(field.id, { ...field, name: e.target.value })}
            />
        </Grid>
        <Grid item xs={12} sm={2}>
            <FormControl fullWidth>
                <InputLabel>Tipo</InputLabel>
                <Select
                    value={field.type}
                    label="Tipo"
                    onChange={(e) => {
                        const newType = e.target.value;
                        // Limpa o tamanho se for booleano ou data
                        const newSize = ['boolean', 'date', 'datetime'].includes(newType) ? '' : field.size;
                        onChange(field.id, { ...field, type: newType, size: newSize });
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
                    onChange={(e) => onChange(field.id, { ...field, size: e.target.value })}
                />
            )}
        </Grid>
        <Grid item xs={12} sm={3}>
            <FormControlLabel
                control={
                    <Checkbox
                        checked={field.required}
                        onChange={(e) => onChange(field.id, { ...field, required: e.target.checked })}
                    />
                }
                label="Obrigatório"
            />
        </Grid>
        <Grid item xs={12} sm={2}>
            <IconButton onClick={() => onRemove(field.id)}>
                <DeleteIcon />
            </IconButton>
        </Grid>
    </Grid>
);

export function UserStoryForm() {
    const navigate = useNavigate();
    const containerStyles = {
        minWidth: '800px',
        p: 3
    };
    const [files, setFiles] = useState({
        notApplicable: false,
        items: []
    });
    const [screenshots, setScreenshots] = useState({
        notApplicable: false,
        items: []
    });
    const [userStory, setUserStory] = useState([{
        id: 1,
        como: '',
        quero: '',
        para: ''
    }]);
    const [impact, setImpact] = useState({
        notApplicable: false,
        current: [{ id: 1, content: '' }],
        expected: [{ id: 1, content: '' }],
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
    const [acceptanceCriteria, setAcceptanceCriteria] = useState({
        notApplicable: false,
        content: '',
    });

    const { control, handleSubmit, formState: { errors } } = useForm({
        resolver: yupResolver(schema)
    });

    const handleFileChange = (e) => {
        if (!files.notApplicable) {
            const newFiles = Array.from(e.target.files);
            setFiles({
                ...files,
                items: [...files.items, ...newFiles]
            });
        }
    };

    const onSubmit = (data) => {
        const formData = {
            ...data,
            userStory,
            impact,
            objective,
            fields,
            messages,
            rules,
            scenarios,
            acceptanceCriteria,
            screenshots,
            files,
        };
        console.log('Dados do formulário:', formData);
    };

    return (
        <>
            <Navbar />
            <Box
                sx={{
                    minHeight: '100vh',
                    display: 'flex',
                    alignItems: 'flex-start',
                    justifyContent: 'center',
                    p: 2,
                    pt: 10
                }}
            >
                <Paper
                    elevation={3}
                    sx={{
                        p: 4,
                        maxWidth: '1400px',
                        margin: '0 auto'
                    }}
                >
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 4 }}>
                        <Button
                            variant="outlined"
                            onClick={() => navigate('/dashboard')}
                            sx={{ mr: 2 }}
                        >
                            Voltar
                        </Button>
                        <Typography variant="h5" component="h1">
                            Nova História
                        </Typography>
                    </Box>

                    <Box component="form" onSubmit={handleSubmit(onSubmit)}>
                        <Stack spacing={3} sx={{ mb: 4 }}>
                            <Controller
                                name="demandNumber"
                                control={control}
                                defaultValue=""
                                render={({ field }) => (
                                    <FormControl error={!!errors.demandNumber}>
                                        <InputLabel>Número da Demanda</InputLabel>
                                        <Select
                                            {...field}
                                            label="Número da Demanda"
                                            sx={{ minWidth: 300 }}
                                        >
                                            {demandas.map((demanda) => (
                                                <MenuItem key={demanda.id} value={demanda.id}>
                                                    {demanda.id} - {demanda.title}
                                                </MenuItem>
                                            ))}
                                        </Select>
                                    </FormControl>
                                )}
                            />
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
                        </Stack>

                        <Section title="História do Usuário" isFirst>
                            <Stack spacing={4}>
                                {userStory.map((story, index) => (
                                    <Box key={story.id} sx={{
                                        display: 'flex',
                                        gap: 2,
                                        alignItems: 'center'
                                    }}>
                                        <Stack spacing={3} sx={{ flex: 1 }}>
                                            <Typography variant="subtitle2" gutterBottom sx={{ color: 'text.secondary' }}>
                                                História do Usuário {index + 1}
                                            </Typography>
                                            <TextField
                                                fullWidth
                                                label="Como"
                                                value={story.como}
                                                onChange={(e) => {
                                                    const newStories = [...userStory];
                                                    newStories[index] = { ...story, como: e.target.value };
                                                    setUserStory(newStories);
                                                }}
                                                multiline
                                                rows={3}
                                                placeholder="Ex: Como um usuário do sistema..."
                                            />
                                            <TextField
                                                fullWidth
                                                label="Quero"
                                                value={story.quero}
                                                onChange={(e) => {
                                                    const newStories = [...userStory];
                                                    newStories[index] = { ...story, quero: e.target.value };
                                                    setUserStory(newStories);
                                                }}
                                                multiline
                                                rows={3}
                                                placeholder="Ex: Quero poder realizar..."
                                            />
                                            <TextField
                                                fullWidth
                                                label="Para"
                                                value={story.para}
                                                onChange={(e) => {
                                                    const newStories = [...userStory];
                                                    newStories[index] = { ...story, para: e.target.value };
                                                    setUserStory(newStories);
                                                }}
                                                multiline
                                                rows={3}
                                                placeholder="Ex: Para que eu possa..."
                                            />
                                        </Stack>
                                        {userStory.length > 1 && (
                                            <IconButton
                                                sx={{
                                                    alignSelf: 'center',
                                                    mr: 1
                                                }}
                                                onClick={() => {
                                                    const newStories = userStory.filter((_, i) => i !== index);
                                                    setUserStory(newStories);
                                                }}
                                                color="error"
                                            >
                                                <DeleteIcon />
                                            </IconButton>
                                        )}
                                    </Box>
                                ))}
                                <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                                    <Button
                                        startIcon={<AddIcon />}
                                        onClick={() => {
                                            setUserStory([
                                                ...userStory,
                                                {
                                                    id: userStory.length + 1,
                                                    como: '',
                                                    quero: '',
                                                    para: ''
                                                }
                                            ]);
                                        }}
                                        variant="outlined"
                                        sx={{ width: '50%' }}
                                    >
                                        Adicionar História do Usuário
                                    </Button>
                                </Box>
                            </Stack>
                        </Section>

                        <Section
                            title="Impacto"
                            notApplicable={impact.notApplicable}
                            onNotApplicableChange={(checked) => setImpact({ ...impact, notApplicable: checked })}
                        >
                            <Stack spacing={3}>
                                <Box>
                                    <Typography variant="subtitle1" gutterBottom>Processo Atual:</Typography>
                                    <DynamicFields
                                        fields={impact.current}
                                        onAdd={() => setImpact({
                                            ...impact,
                                            current: [...impact.current, { id: impact.current.length + 1, content: '' }]
                                        })}
                                        onRemove={(index) => setImpact({
                                            ...impact,
                                            current: impact.current.filter((_, i) => i !== index)
                                        })}
                                        onFieldChange={(index, value) => setImpact({
                                            ...impact,
                                            current: impact.current.map((item, i) =>
                                                i === index ? { ...item, content: value } : item
                                            )
                                        })}
                                        disabled={impact.notApplicable}
                                    />
                                </Box>
                                <Box>
                                    <Typography variant="subtitle1" gutterBottom>Melhoria Esperada:</Typography>
                                    <DynamicFields
                                        fields={impact.expected}
                                        onAdd={() => setImpact({
                                            ...impact,
                                            expected: [...impact.expected, { id: impact.expected.length + 1, content: '' }]
                                        })}
                                        onRemove={(index) => setImpact({
                                            ...impact,
                                            expected: impact.expected.filter((_, i) => i !== index)
                                        })}
                                        onFieldChange={(index, value) => setImpact({
                                            ...impact,
                                            expected: impact.expected.map((item, i) =>
                                                i === index ? { ...item, content: value } : item
                                            )
                                        })}
                                        disabled={impact.notApplicable}
                                    />
                                </Box>
                            </Stack>
                        </Section>

                        <Section
                            title="Objetivo"
                            notApplicable={objective.notApplicable}
                            onNotApplicableChange={(checked) => setObjective({ ...objective, notApplicable: checked })}
                        >
                            <DynamicFields
                                fields={objective.fields}
                                onAdd={() => setObjective({
                                    ...objective,
                                    fields: [...objective.fields, { id: objective.fields.length + 1, content: '' }]
                                })}
                                onRemove={(index) => setObjective({
                                    ...objective,
                                    fields: objective.fields.filter((_, i) => i !== index)
                                })}
                                onFieldChange={(index, value) => setObjective({
                                    ...objective,
                                    fields: objective.fields.map((item, i) =>
                                        i === index ? { ...item, content: value } : item
                                    )
                                })}
                                disabled={objective.notApplicable}
                            />
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
                                <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                                    <Box sx={{ width: '50%' }}>
                                        <label htmlFor="screenshots-input" style={{ width: '100%', display: 'block' }}>
                                            <Button
                                                variant="outlined"
                                                component="span"
                                                startIcon={<AttachFileIcon />}
                                                disabled={screenshots.notApplicable}
                                                sx={{
                                                    width: '100%',
                                                    whiteSpace: 'nowrap'
                                                }}
                                            >
                                                Adicionar Telas
                                            </Button>
                                        </label>
                                    </Box>
                                </Box>

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
                            title="Campos de Preenchimento"
                            notApplicable={fields.notApplicable}
                            onNotApplicableChange={(checked) => setFields({ ...fields, notApplicable: checked })}
                        >
                            {fields.items.map((field) => (
                                <FieldDefinition
                                    key={field.id}
                                    field={field}
                                    onChange={(id, updatedField) => setFields({
                                        ...fields,
                                        items: fields.items.map((f) =>
                                            f.id === id ? updatedField : f
                                        )
                                    })}
                                    onRemove={(id) => setFields({
                                        ...fields,
                                        items: fields.items.filter((f) => f.id !== id)
                                    })}
                                />
                            ))}
                            <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                                <Button
                                    startIcon={<AddIcon />}
                                    onClick={() => setFields({
                                        ...fields,
                                        items: [
                                            ...fields.items,
                                            {
                                                id: fields.items.length + 1,
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
                        </Section>

                        <Section
                            title="Mensagens Informativas"
                            notApplicable={messages.notApplicable}
                            onNotApplicableChange={(checked) => setMessages({ ...messages, notApplicable: checked })}
                        >
                            <DynamicFields
                                fields={messages.items}
                                onAdd={() => setMessages({
                                    ...messages,
                                    items: [...messages.items, { id: messages.items.length + 1, content: '' }]
                                })}
                                onRemove={(index) => setMessages({
                                    ...messages,
                                    items: messages.items.filter((_, i) => i !== index)
                                })}
                                onFieldChange={(index, value) => setMessages({
                                    ...messages,
                                    items: messages.items.map((item, i) =>
                                        i === index ? { ...item, content: value } : item
                                    )
                                })}
                                disabled={messages.notApplicable}
                            />
                        </Section>

                        <Section
                            title="Regras de Negócio"
                            notApplicable={rules.notApplicable}
                            onNotApplicableChange={(checked) => setRules({ ...rules, notApplicable: checked })}
                        >
                            <DynamicFields
                                fields={rules.items}
                                onAdd={() => setRules({
                                    ...rules,
                                    items: [...rules.items, { id: rules.items.length + 1, content: '' }]
                                })}
                                onRemove={(index) => setRules({
                                    ...rules,
                                    items: rules.items.filter((_, i) => i !== index)
                                })}
                                onFieldChange={(index, value) => setRules({
                                    ...rules,
                                    items: rules.items.map((item, i) =>
                                        i === index ? { ...item, content: value } : item
                                    )
                                })}
                                disabled={rules.notApplicable}
                            />
                        </Section>

                        <Section
                            title="Cenários"
                            notApplicable={scenarios.notApplicable}
                            onNotApplicableChange={(checked) => setScenarios({ ...scenarios, notApplicable: checked })}
                        >
                            {scenarios.items.map((scenario, index) => (
                                <Box key={scenario.id} sx={{
                                    display: 'flex',
                                    gap: 2,
                                    alignItems: 'center',
                                    mb: 4
                                }}>
                                    <Stack spacing={3} sx={{ flex: 1 }}>
                                        <Typography variant="subtitle2" gutterBottom sx={{ color: 'text.secondary' }}>
                                            Cenário {index + 1}
                                        </Typography>
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
                                            rows={3}
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
                                            rows={3}
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
                                            rows={3}
                                            placeholder="Ex: Então o sistema deve..."
                                        />
                                    </Stack>
                                    {scenarios.items.length > 1 && (
                                        <IconButton
                                            onClick={() => setScenarios({
                                                ...scenarios,
                                                items: scenarios.items.filter((s) => s.id !== scenario.id)
                                            })}
                                            disabled={scenarios.notApplicable}
                                            color="error"
                                            sx={{ alignSelf: 'center', mr: 1 }}
                                        >
                                            <DeleteIcon />
                                        </IconButton>
                                    )}
                                </Box>
                            ))}
                            <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                                <Button
                                    startIcon={<AddIcon />}
                                    onClick={() => setScenarios({
                                        ...scenarios,
                                        items: [
                                            ...scenarios.items,
                                            {
                                                id: scenarios.items.length + 1,
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
                        </Section>

                        <Section
                            title="Critérios de Aceite"
                            notApplicable={acceptanceCriteria.notApplicable}
                            onNotApplicableChange={(checked) => setAcceptanceCriteria({
                                ...acceptanceCriteria,
                                notApplicable: checked
                            })}
                        >
                            <TextField
                                fullWidth
                                multiline
                                rows={4}
                                value={acceptanceCriteria.content}
                                onChange={(e) => setAcceptanceCriteria({
                                    ...acceptanceCriteria,
                                    content: e.target.value
                                })}
                                disabled={acceptanceCriteria.notApplicable}
                            />
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
                            <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                                <Box sx={{ width: '50%' }}>
                                    <label htmlFor="file-input" style={{ width: '100%', display: 'block' }}>
                                        <Button
                                            variant="outlined"
                                            component="span"
                                            startIcon={<AttachFileIcon />}
                                            disabled={files.notApplicable}
                                            sx={{
                                                width: '100%',
                                                whiteSpace: 'nowrap'
                                            }}
                                        >
                                            Adicionar Anexos
                                        </Button>
                                    </label>
                                </Box>
                            </Box>

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
                                            >
                                                <DeleteIcon />
                                            </IconButton>
                                        </Box>
                                    ))}
                                </Box>
                            )}
                        </Section>

                        <Box sx={{ mt: 4, display: 'flex', justifyContent: 'flex-end' }}>
                            <Button
                                type="submit"
                                variant="contained"
                                color="primary"
                                size="large"
                            >
                                Salvar História
                            </Button>
                        </Box>
                    </Box>
                </Paper>
            </Box>
        </>
    );
}
