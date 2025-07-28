import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    Typography,
    Box,
    List,
    ListItem,
    ListItemText,
    Alert
} from '@mui/material';
import {
    CheckCircle as SuccessIcon,
    Error as ErrorIcon,
    Warning as WarningIcon,
    Info as InfoIcon
} from '@mui/icons-material';
import { useNotification } from '../contexts/NotificationContext';

const iconMap = {
    success: SuccessIcon,
    error: ErrorIcon,
    warning: WarningIcon,
    info: InfoIcon
};

const colorMap = {
    success: 'success',
    error: 'error',
    warning: 'warning',
    info: 'info'
};

export function NotificationModal() {
    const { notification, hideNotification } = useNotification();
    const IconComponent = iconMap[notification.type];

    const handleClose = () => {
        hideNotification();
    };

    return (
        <Dialog
            open={notification.open}
            onClose={handleClose}
            maxWidth="sm"
            fullWidth
            PaperProps={{
                sx: {
                    borderRadius: 2,
                    maxHeight: '80vh'
                }
            }}
        >
            <DialogTitle
                sx={{
                    display: 'flex',
                    alignItems: 'center',
                    gap: 2,
                    pb: 1
                }}
            >
                <IconComponent
                    color={colorMap[notification.type]}
                    sx={{ fontSize: '2rem' }}
                />
                <Typography variant="h6" component="span">
                    {notification.title}
                </Typography>
            </DialogTitle>

            <DialogContent>
                {notification.message && (
                    <Alert
                        severity={notification.type}
                        sx={{ mb: notification.errors.length > 0 ? 2 : 0 }}
                    >
                        {notification.message}
                    </Alert>
                )}

                {notification.errors.length > 0 && (
                    <Box>
                        <Typography variant="subtitle2" gutterBottom color="error">
                            Detalhes dos erros:
                        </Typography>
                        <List dense>
                            {notification.errors.map((error, index) => (
                                <ListItem key={index} sx={{ py: 0.5 }}>
                                    <ListItemText
                                        primary={`• ${error}`}
                                        primaryTypographyProps={{
                                            variant: 'body2',
                                            color: 'error'
                                        }}
                                    />
                                </ListItem>
                            ))}
                        </List>
                    </Box>
                )}
            </DialogContent>

            <DialogActions sx={{ px: 3, pb: 2, justifyContent: 'center' }}>
                <Button
                    onClick={handleClose}
                    variant="contained"
                    color={colorMap[notification.type]}
                    sx={{ width: '50%' }}
                >
                    OK
                </Button>
            </DialogActions>
        </Dialog>
    );
}
