import React from 'react';
import { Box, Typography } from '@mui/material';
import { useTheme as useMuiTheme } from '@mui/material/styles';

export const LogoHero = () => {
    const theme = useMuiTheme();
    const isDark = theme.palette.mode === 'dark';

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                gap: 2,
                position: 'relative',
                py: 4,
                px: 2,
            }}
        >
            {/* Logo principal */}
            <Box
                sx={{
                    display: 'flex',
                    flexDirection: { xs: 'column', sm: 'row' },
                    alignItems: 'center',
                    gap: { xs: 1, sm: 2 },
                    position: 'relative',
                }}
            >
                {/* Integração */}
                <Typography
                    component="h1"
                    sx={{
                        fontSize: { xs: '2.5rem', sm: '3.5rem', md: '5rem' },
                        fontWeight: 900,
                        letterSpacing: '0.1em',
                        textTransform: 'uppercase',
                        fontFamily: '"Mulish", system-ui, sans-serif',
                        background: isDark
                            ? `linear-gradient(135deg, 
                                ${theme.palette.primary.light} 0%, 
                                ${theme.palette.primary.main} 30%,
                                ${theme.palette.primary.dark} 70%,
                                ${theme.palette.primary.main} 100%)`
                            : `linear-gradient(135deg, 
                                ${theme.palette.primary.dark} 0%, 
                                ${theme.palette.primary.main} 30%,
                                ${theme.palette.primary.light} 70%,
                                ${theme.palette.primary.main} 100%)`,
                        backgroundClip: 'text',
                        WebkitBackgroundClip: 'text',
                        color: 'transparent',
                        WebkitTextFillColor: 'transparent',
                        position: 'relative',
                        textShadow: isDark
                            ? `0 0 20px ${theme.palette.primary.main}40`
                            : `0 2px 4px ${theme.palette.primary.main}30`,
                        '&::before': {
                            content: '"Integração"',
                            position: 'absolute',
                            top: 0,
                            left: 0,
                            background: isDark
                                ? `linear-gradient(135deg, 
                                    ${theme.palette.primary.light}60 0%, 
                                    ${theme.palette.primary.main}40 50%, 
                                    ${theme.palette.primary.dark}20 100%)`
                                : `linear-gradient(135deg, 
                                    ${theme.palette.primary.dark}60 0%, 
                                    ${theme.palette.primary.main}40 50%, 
                                    ${theme.palette.primary.light}20 100%)`,
                            backgroundClip: 'text',
                            WebkitBackgroundClip: 'text',
                            color: 'transparent',
                            WebkitTextFillColor: 'transparent',
                            filter: 'blur(2px)',
                            transform: 'translate(2px, 2px)',
                            zIndex: -1,
                        },
                        '&::after': {
                            content: '""',
                            position: 'absolute',
                            top: '5%',
                            left: '2%',
                            right: '2%',
                            bottom: '5%',
                            background: `
                                radial-gradient(circle at 15% 25%, transparent 6%, ${theme.palette.primary.main}25 8%, transparent 10%),
                                radial-gradient(circle at 85% 75%, transparent 7%, ${theme.palette.primary.main}20 9%, transparent 11%),
                                radial-gradient(circle at 35% 85%, transparent 5%, ${theme.palette.primary.main}15 7%, transparent 9%),
                                radial-gradient(circle at 75% 15%, transparent 8%, ${theme.palette.primary.main}18 10%, transparent 12%),
                                radial-gradient(circle at 50% 50%, transparent 10%, ${theme.palette.primary.main}10 12%, transparent 14%)
                            `,
                            mixBlendMode: isDark ? 'screen' : 'multiply',
                            opacity: 0.4,
                            pointerEvents: 'none',
                        }
                    }}
                >
                    Integração
                </Typography>

                {/* Azure */}
                <Typography
                    component="span"
                    sx={{
                        fontSize: { xs: '2.5rem', sm: '3.5rem', md: '5rem' },
                        fontWeight: 900,
                        letterSpacing: '0.1em',
                        textTransform: 'uppercase',
                        fontFamily: '"Mulish", system-ui, sans-serif',
                        background: isDark
                            ? `linear-gradient(135deg, 
                                #00d4ff 0%, 
                                #0078d4 30%, 
                                #004578 70%, 
                                #0078d4 100%)`
                            : `linear-gradient(135deg, 
                                #004578 0%, 
                                #0078d4 30%, 
                                #00d4ff 70%, 
                                #0078d4 100%)`,
                        backgroundClip: 'text',
                        WebkitBackgroundClip: 'text',
                        color: 'transparent',
                        WebkitTextFillColor: 'transparent',
                        position: 'relative',
                        textShadow: isDark
                            ? `0 0 20px #0078d440`
                            : `0 2px 4px #0078d430`,
                        '&::before': {
                            content: '"Azure"',
                            position: 'absolute',
                            top: 0,
                            left: 0,
                            background: isDark
                                ? `linear-gradient(135deg, #00d4ff60 0%, #0078d440 50%, #00457820 100%)`
                                : `linear-gradient(135deg, #00457860 0%, #0078d440 50%, #00d4ff20 100%)`,
                            backgroundClip: 'text',
                            WebkitBackgroundClip: 'text',
                            color: 'transparent',
                            WebkitTextFillColor: 'transparent',
                            filter: 'blur(2px)',
                            transform: 'translate(-2px, 2px)',
                            zIndex: -1,
                        },
                        '&::after': {
                            content: '""',
                            position: 'absolute',
                            top: '8%',
                            left: '5%',
                            right: '5%',
                            bottom: '8%',
                            background: `
                                radial-gradient(circle at 20% 35%, transparent 5%, #0078d425 7%, transparent 9%),
                                radial-gradient(circle at 80% 65%, transparent 6%, #00d4ff20 8%, transparent 10%),
                                radial-gradient(circle at 45% 20%, transparent 4%, #0078d415 6%, transparent 8%),
                                radial-gradient(circle at 65% 90%, transparent 7%, #00457812 9%, transparent 11%),
                                radial-gradient(circle at 10% 80%, transparent 5%, #0078d418 7%, transparent 9%)
                            `,
                            mixBlendMode: isDark ? 'screen' : 'multiply',
                            opacity: 0.5,
                            pointerEvents: 'none',
                        }
                    }}
                >
                    Azure
                </Typography>
            </Box>

            {/* Tagline */}
            <Typography
                variant="h6"
                sx={{
                    color: theme.palette.text.secondary,
                    fontWeight: 300,
                    letterSpacing: '0.05em',
                    textAlign: 'center',
                    opacity: 0.8,
                    fontSize: { xs: '0.9rem', sm: '1.1rem' },
                    fontStyle: 'italic',
                }}
            >
                Conectando Soluções • Criando Histórias
            </Typography>

            {/* Efeito de brilho geral */}
            <Box
                sx={{
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    right: 0,
                    bottom: 0,
                    background: `
                        radial-gradient(ellipse at 20% 30%, transparent 85%, ${theme.palette.primary.main}08 90%, transparent 95%),
                        radial-gradient(ellipse at 80% 70%, transparent 87%, #0078d406 92%, transparent 97%),
                        radial-gradient(ellipse at 50% 10%, transparent 88%, ${theme.palette.primary.main}04 93%, transparent 98%)
                    `,
                    mixBlendMode: isDark ? 'screen' : 'multiply',
                    opacity: 0.3,
                    pointerEvents: 'none',
                    animation: 'logoGlow 6s ease-in-out infinite alternate',
                    '@keyframes logoGlow': {
                        '0%': {
                            opacity: 0.3,
                            transform: 'scale(1)',
                        },
                        '100%': {
                            opacity: 0.5,
                            transform: 'scale(1.02)',
                        },
                    },
                }}
            />
        </Box>
    );
};
