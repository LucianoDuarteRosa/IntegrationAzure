import { Box, Paper, Typography } from '@mui/material';

export function MenuCard({ icon: Icon, title, onClick }) {
    return (
        <Paper
            elevation={4}
            sx={{
                width: 250,
                height: 250,
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                justifyContent: 'center',
                cursor: 'pointer',
                borderRadius: 4,
                transition: 'all 0.3s ease',
                '&:hover': {
                    transform: 'translateY(-5px)',
                    boxShadow: 6
                }
            }}
            onClick={onClick}
        >
            <Box sx={{ mb: 2 }}>
                <Icon sx={{ fontSize: 64, color: 'primary.main' }} />
            </Box>
            <Typography variant="h6" align="center">
                {title}
            </Typography>
        </Paper>
    );
}
