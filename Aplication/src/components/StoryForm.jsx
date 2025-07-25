import { useState } from 'react';
import {
    Box,
    TextField,
    Button,
    Paper,
    Typography,
    IconButton,
    List,
    ListItem,
    ListItemText,
    ListItemSecondaryAction,
    MenuItem,
    Select,
    FormControl,
    InputLabel,
    Container,
} from '@mui/material';
import { Delete as DeleteIcon, Add as AddIcon } from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';

const schema = yup.object({
    demandNumber: yup.string().required('Número da demanda é obrigatório'),
    title: yup.string().required('Título é obrigatório'),
    acceptanceCriteria: yup.string().required('Critérios de aceite são obrigatórios'),
}).required();

// Simulação de demandas disponíveis
const demandas = [
    { id: 'DEM-001', title: 'Demanda 1' },
    { id: 'DEM-002', title: 'Demanda 2' },
    { id: 'DEM-003', title: 'Demanda 3' },
    { id: 'DEM-004', title: 'Demanda 4' },
];

export function StoryForm() {
    const [casos, setCasos] = useState([{ id: 1, description: '' }]);
    const [files, setFiles] = useState([]);

    const { control, handleSubmit, formState: { errors } } = useForm({
        resolver: yupResolver(schema)
    });

    const handleAddCaso = () => {
        const newId = casos.length + 1;
        setCasos([...casos, { id: newId, description: '' }]);
    };

    const handleRemoveCaso = (id) => {
        setCasos(casos.filter(caso => caso.id !== id));
    };

    const handleCasoChange = (id, value) => {
        setCasos(casos.map(caso =>
            caso.id === id ? { ...caso, description: value } : caso
        ));
    };

    const handleFileChange = (e) => {
        const newFiles = Array.from(e.target.files);
        setFiles([...files, ...newFiles]);
    };

    const handleRemoveFile = (index) => {
        setFiles(files.filter((_, i) => i !== index));
    };

    const onSubmit = (data) => {
        // Aqui você implementará a lógica para enviar os dados para a API
        const formData = {
            ...data,
            casos,
            files
        };
        console.log('Dados do formulário:', formData);
    };

    return (
        <Container maxWidth="lg" sx={{
            minHeight: 'calc(100vh - 64px)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            py: 4
        }}>
            <Paper elevation={3} sx={{ p: 4, width: '100%', maxWidth: '800px' }}>
                <Typography variant="h5" component="h2" gutterBottom align="center">
                    Nova História
                </Typography>

                <Box component="form" onSubmit={handleSubmit(onSubmit)} sx={{ mt: 3 }}>
                    <Controller
                        name="demandNumber"
                        control={control}
                        defaultValue=""
                        render={({ field }) => (
                            <FormControl fullWidth margin="normal" error={!!errors.demandNumber}>
                                <InputLabel id="demanda-label">Número da Demanda</InputLabel>
                                <Select
                                    {...field}
                                    labelId="demanda-label"
                                    label="Número da Demanda"
                                >
                                    {demandas.map((demanda) => (
                                        <MenuItem key={demanda.id} value={demanda.id}>
                                            {demanda.id} - {demanda.title}
                                        </MenuItem>
                                    ))}
                                </Select>
                                {errors.demandNumber && (
                                    <Typography variant="caption" color="error">
                                        {errors.demandNumber.message}
                                    </Typography>
                                )}
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
                                margin="normal"
                            />
                        )}
                    />

                    <Typography variant="h6" sx={{ mt: 3, mb: 2 }}>
                        Casos
                    </Typography>

                    {casos.map((caso) => (
                        <Box key={caso.id} sx={{ display: 'flex', gap: 1, mb: 2 }}>
                            <TextField
                                fullWidth
                                label={`Caso ${caso.id}`}
                                multiline
                                rows={3}
                                value={caso.description}
                                onChange={(e) => handleCasoChange(caso.id, e.target.value)}
                            />
                            <IconButton
                                onClick={() => handleRemoveCaso(caso.id)}
                                disabled={casos.length === 1}
                            >
                                <DeleteIcon />
                            </IconButton>
                        </Box>
                    ))}

                    <Button
                        startIcon={<AddIcon />}
                        onClick={handleAddCaso}
                        variant="outlined"
                        sx={{ mb: 3 }}
                    >
                        Adicionar Caso
                    </Button>

                    <Controller
                        name="acceptanceCriteria"
                        control={control}
                        defaultValue=""
                        render={({ field }) => (
                            <TextField
                                {...field}
                                fullWidth
                                label="Critérios de Aceite"
                                multiline
                                rows={4}
                                error={!!errors.acceptanceCriteria}
                                helperText={errors.acceptanceCriteria?.message}
                                margin="normal"
                            />
                        )}
                    />

                    <Box sx={{ mt: 3 }}>
                        <input
                            type="file"
                            multiple
                            onChange={handleFileChange}
                            style={{ display: 'none' }}
                            id="file-input"
                        />
                        <label htmlFor="file-input">
                            <Button variant="outlined" component="span">
                                Adicionar Anexos
                            </Button>
                        </label>

                        <List>
                            {files.map((file, index) => (
                                <ListItem key={index}>
                                    <ListItemText primary={file.name} />
                                    <ListItemSecondaryAction>
                                        <IconButton edge="end" onClick={() => handleRemoveFile(index)}>
                                            <DeleteIcon />
                                        </IconButton>
                                    </ListItemSecondaryAction>
                                </ListItem>
                            ))}
                        </List>
                    </Box>

                    <Button
                        type="submit"
                        variant="contained"
                        color="primary"
                        size="large"
                        sx={{ mt: 3 }}
                        fullWidth
                    >
                        Salvar História
                    </Button>
                </Box>
            </Paper>
        </Container>
    );
}
