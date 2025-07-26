import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { Box, Typography, Chip } from '@mui/material';

export function SessionInfo() {
    const { user, getSessionTimeRemaining } = useAuth();
    const [timeRemaining, setTimeRemaining] = useState(0);

    useEffect(() => {
        if (!user) return;

        const updateTimer = () => {
            setTimeRemaining(getSessionTimeRemaining());
        };

        // Atualiza a cada minuto
        updateTimer();
        const interval = setInterval(updateTimer, 60000);

        return () => clearInterval(interval);
    }, [user, getSessionTimeRemaining]);

    if (!user) return null;

    const formatTime = (milliseconds) => {
        const hours = Math.floor(milliseconds / (1000 * 60 * 60));
        const minutes = Math.floor((milliseconds % (1000 * 60 * 60)) / (1000 * 60));
        return `${hours}h ${minutes}m`;
    };

    return (
        <Box sx={{ position: 'fixed', top: 10, right: 10, zIndex: 1000 }}>
            <Chip
                label={`Sessão: ${formatTime(timeRemaining)}`}
                color="primary"
                variant="outlined"
                size="small"
                sx={{ backgroundColor: 'rgba(255,255,255,0.9)' }}
            />
        </Box>
    );
}
