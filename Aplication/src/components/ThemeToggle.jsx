import React from 'react';
import { IconButton, Tooltip } from '@mui/material';
import { Brightness4, Brightness7 } from '@mui/icons-material';
import { useTheme } from '../contexts/ThemeContext';

export const ThemeToggle = ({ size = 'medium', sx = {} }) => {
    const { mode, toggleColorMode } = useTheme();

    return (
        <Tooltip title={mode === 'dark' ? 'Ativar tema claro' : 'Ativar tema escuro'}>
            <IconButton
                onClick={toggleColorMode}
                color="inherit"
                size={size}
                sx={{
                    transition: 'transform 0.2s ease-in-out',
                    '&:hover': {
                        transform: 'scale(1.1)',
                    },
                    ...sx
                }}
            >
                {mode === 'dark' ? <Brightness7 /> : <Brightness4 />}
            </IconButton>
        </Tooltip>
    );
};
