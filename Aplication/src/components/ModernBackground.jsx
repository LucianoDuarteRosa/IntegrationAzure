import React from 'react';
import { Box } from '@mui/material';
import { useTheme as useMuiTheme } from '@mui/material/styles';

export const ModernBackground = ({ children, intensity = 'subtle' }) => {
    const theme = useMuiTheme();
    const isDark = theme.palette.mode === 'dark';

    const intensityLevels = {
        subtle: { opacity: isDark ? 0.15 : 0.25, blur: '120px' },
        medium: { opacity: isDark ? 0.25 : 0.4, blur: '100px' },
        strong: { opacity: isDark ? 0.35 : 0.55, blur: '80px' }
    };

    const currentIntensity = intensityLevels[intensity];

    return (
        <Box
            sx={{
                position: 'relative',
                minHeight: '100vh',
                width: '100%',
                overflow: 'hidden',
                background: isDark
                    ? theme.palette.background.default
                    : `linear-gradient(135deg, 
                        ${theme.palette.background.default} 0%, 
                        ${theme.palette.primary.main}08 50%, 
                        ${theme.palette.background.default} 100%)`,
                backgroundColor: theme.palette.background.default,
            }}
        >
            {/* Formas geométricas flutuantes */}
            <Box
                sx={{
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    right: 0,
                    bottom: 0,
                    '&::before': {
                        content: '""',
                        position: 'absolute',
                        top: '-10%',
                        left: '-10%',
                        width: '40%',
                        height: '40%',
                        background: isDark
                            ? `linear-gradient(135deg, ${theme.palette.primary.main}40, ${theme.palette.secondary.main}20)`
                            : `linear-gradient(135deg, ${theme.palette.primary.main}50, ${theme.palette.secondary.main}35)`,
                        borderRadius: '30% 70% 70% 30% / 30% 30% 70% 70%',
                        filter: `blur(${currentIntensity.blur})`,
                        opacity: currentIntensity.opacity,
                        transform: 'rotate(-15deg)',
                        animation: 'float 20s ease-in-out infinite',
                    },
                    '&::after': {
                        content: '""',
                        position: 'absolute',
                        bottom: '-15%',
                        right: '-10%',
                        width: '50%',
                        height: '50%',
                        background: isDark
                            ? `linear-gradient(45deg, ${theme.palette.secondary.main}35, ${theme.palette.primary.main}20)`
                            : `linear-gradient(45deg, ${theme.palette.secondary.main}45, ${theme.palette.primary.main}30)`,
                        borderRadius: '70% 30% 30% 70% / 70% 70% 30% 30%',
                        filter: `blur(${currentIntensity.blur})`,
                        opacity: currentIntensity.opacity,
                        transform: 'rotate(25deg)',
                        animation: 'float 25s ease-in-out infinite reverse',
                    },
                    '@keyframes float': {
                        '0%, 100%': {
                            transform: 'translate(0, 0) rotate(-15deg)',
                        },
                        '25%': {
                            transform: 'translate(20px, -30px) rotate(-10deg)',
                        },
                        '50%': {
                            transform: 'translate(-15px, -20px) rotate(-20deg)',
                        },
                        '75%': {
                            transform: 'translate(10px, -10px) rotate(-5deg)',
                        },
                    },
                }}
            />

            {/* Formas menores adicionais */}
            <Box
                sx={{
                    position: 'absolute',
                    top: '20%',
                    right: '15%',
                    width: '200px',
                    height: '200px',
                    background: isDark
                        ? `radial-gradient(circle, ${theme.palette.primary.light}25, transparent 70%)`
                        : `radial-gradient(circle, ${theme.palette.primary.light}40, transparent 70%)`,
                    borderRadius: '50%',
                    filter: `blur(60px)`,
                    opacity: currentIntensity.opacity * 0.7,
                    animation: 'pulse 15s ease-in-out infinite',
                    '@keyframes pulse': {
                        '0%, 100%': {
                            transform: 'scale(1)',
                            opacity: currentIntensity.opacity * 0.7,
                        },
                        '50%': {
                            transform: 'scale(1.2)',
                            opacity: currentIntensity.opacity * 0.4,
                        },
                    },
                }}
            />

            <Box
                sx={{
                    position: 'absolute',
                    bottom: '30%',
                    left: '20%',
                    width: '150px',
                    height: '150px',
                    background: isDark
                        ? `linear-gradient(45deg, ${theme.palette.secondary.light}30, transparent)`
                        : `linear-gradient(45deg, ${theme.palette.secondary.light}35, transparent)`,
                    borderRadius: '40% 60% 60% 40% / 60% 30% 70% 40%',
                    filter: `blur(50px)`,
                    opacity: currentIntensity.opacity * 0.8,
                    animation: 'rotate 30s linear infinite',
                    '@keyframes rotate': {
                        '0%': {
                            transform: 'rotate(0deg)',
                        },
                        '100%': {
                            transform: 'rotate(360deg)',
                        },
                    },
                }}
            />

            {/* Degradê sutil adicional */}
            <Box
                sx={{
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    right: 0,
                    bottom: 0,
                    background: isDark
                        ? `
                            radial-gradient(ellipse at top left, ${theme.palette.primary.main}08 0%, transparent 60%),
                            radial-gradient(ellipse at bottom right, ${theme.palette.secondary.main}06 0%, transparent 60%)
                        `
                        : `
                            radial-gradient(ellipse at top left, ${theme.palette.primary.main}15 0%, transparent 60%),
                            radial-gradient(ellipse at bottom right, ${theme.palette.secondary.main}12 0%, transparent 60%)
                        `,
                    pointerEvents: 'none',
                }}
            />

            {/* Conteúdo da página */}
            <Box sx={{ position: 'relative', zIndex: 1 }}>
                {children}
            </Box>
        </Box>
    );
};
