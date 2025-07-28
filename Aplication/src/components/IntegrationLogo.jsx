import React from 'react';
import { Box, Typography } from '@mui/material';
import { useTheme as useMuiTheme } from '@mui/material/styles';

export const IntegrationLogo = ({ size = 'medium', variant = 'horizontal', isNavbar = false, isLoginPage = false }) => {
    const theme = useMuiTheme();
    const isDark = theme.palette.mode === 'dark';

    const sizes = {
        small: { fontSize: '1.5rem', spacing: '0.1em' },
        medium: { fontSize: '2rem', spacing: '0.15em' },
        large: { fontSize: '3rem', spacing: '0.2em' },
        xlarge: { fontSize: '4rem', spacing: '0.25em' }
    };

    const currentSize = sizes[size];

    // Cores que funcionam bem em qualquer fundo
    const getLogoColors = () => {
        // Se for página de login no tema claro, usar preto para "Integração"
        if (isLoginPage && !isDark) {
            return {
                integracao: {
                    main: `linear-gradient(135deg, #2c2c2c 0%, #1a1a1a 50%, #000000 100%)`,
                    shadow: `linear-gradient(135deg, #2c2c2c60 0%, #1a1a1a40 50%, #00000020 100%)`
                },
                azure: {
                    main: `linear-gradient(135deg, #ff9800 0%, #ffb74d 50%, #ffcc02 100%)`,
                    shadow: `linear-gradient(135deg, #ff980080 0%, #ffb74d60 50%, #ffcc0240 100%)`
                }
            };
        }

        return {
            integracao: {
                main: isDark
                    ? `linear-gradient(135deg, #ffffff 0%, #f5f5f5 50%, #e0e0e0 100%)`
                    : `linear-gradient(135deg, #ffffff 0%, #f8f9fa 50%, #e9ecef 100%)`,
                shadow: isDark
                    ? `linear-gradient(135deg, #ffffff60 0%, #f5f5f540 50%, #e0e0e020 100%)`
                    : `linear-gradient(135deg, #ffffff80 0%, #f8f9fa60 50%, #e9ecef40 100%)`
            },
            azure: {
                main: isDark
                    ? `linear-gradient(135deg, #ffd700 0%, #ffeb3b 50%, #fff176 100%)`
                    : `linear-gradient(135deg, #ff9800 0%, #ffb74d 50%, #ffcc02 100%)`,
                shadow: isDark
                    ? `linear-gradient(135deg, #ffd70060 0%, #ffeb3b40 50%, #fff17620 100%)`
                    : `linear-gradient(135deg, #ff980080 0%, #ffb74d60 50%, #ffcc0240 100%)`
            }
        };
    };

    const colors = getLogoColors(); return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: variant === 'vertical' ? 'column' : 'row',
                alignItems: 'center',
                gap: variant === 'vertical' ? 0.5 : 1,
                position: 'relative',
                fontFamily: '"Mulish", system-ui, sans-serif',
            }}
        >
            {/* Palavra "Integração" */}
            <Typography
                component="span"
                sx={{
                    fontSize: currentSize.fontSize,
                    fontWeight: 800,
                    letterSpacing: currentSize.spacing,
                    textTransform: 'uppercase',
                    background: colors.integracao.main,
                    backgroundClip: 'text',
                    WebkitBackgroundClip: 'text',
                    color: 'transparent',
                    WebkitTextFillColor: 'transparent',
                    position: 'relative',
                    // Adicionar sombra de texto para navbar
                    ...(isNavbar && {
                        filter: `drop-shadow(1px 1px 2px ${isDark ? '#00000080' : '#00000040'})`,
                    }),
                    '&::before': {
                        content: '"Integração"',
                        position: 'absolute',
                        top: 0,
                        left: 0,
                        background: colors.integracao.shadow,
                        backgroundClip: 'text',
                        WebkitBackgroundClip: 'text',
                        color: 'transparent',
                        WebkitTextFillColor: 'transparent',
                        filter: 'blur(1px)',
                        transform: 'translate(1px, 1px)',
                        zIndex: -1,
                    },
                    '&::after': {
                        content: '""',
                        position: 'absolute',
                        top: '10%',
                        left: '5%',
                        right: '5%',
                        bottom: '10%',
                        background: `
                            radial-gradient(circle at 20% 30%, transparent 8%, ${theme.palette.primary.main}20 9%, transparent 10%),
                            radial-gradient(circle at 80% 70%, transparent 8%, ${theme.palette.primary.main}15 9%, transparent 10%),
                            radial-gradient(circle at 40% 80%, transparent 6%, ${theme.palette.primary.main}10 7%, transparent 8%),
                            radial-gradient(circle at 70% 20%, transparent 7%, ${theme.palette.primary.main}12 8%, transparent 9%)
                        `,
                        mixBlendMode: isDark ? 'screen' : 'multiply',
                        opacity: 0.3,
                        pointerEvents: 'none',
                    }
                }}
            >
                Integração
            </Typography>

            {/* Palavra "Azure" */}
            <Typography
                component="span"
                sx={{
                    fontSize: currentSize.fontSize,
                    fontWeight: 800,
                    letterSpacing: currentSize.spacing,
                    textTransform: 'uppercase',
                    background: colors.azure.main,
                    backgroundClip: 'text',
                    WebkitBackgroundClip: 'text',
                    color: 'transparent',
                    WebkitTextFillColor: 'transparent',
                    position: 'relative',
                    // Adicionar sombra de texto para navbar
                    ...(isNavbar && {
                        filter: `drop-shadow(1px 1px 2px ${isDark ? '#00000080' : '#00000040'})`,
                    }),
                    '&::before': {
                        content: '"Azure"',
                        position: 'absolute',
                        top: 0,
                        left: 0,
                        background: colors.azure.shadow,
                        backgroundClip: 'text',
                        WebkitBackgroundClip: 'text',
                        color: 'transparent',
                        WebkitTextFillColor: 'transparent',
                        filter: 'blur(1px)',
                        transform: 'translate(-1px, 1px)',
                        zIndex: -1,
                    },
                    '&::after': {
                        content: '""',
                        position: 'absolute',
                        top: '15%',
                        left: '8%',
                        right: '8%',
                        bottom: '15%',
                        background: `
                            radial-gradient(circle at 25% 40%, transparent 6%, #0078d420 7%, transparent 8%),
                            radial-gradient(circle at 75% 60%, transparent 7%, #00d4ff15 8%, transparent 9%),
                            radial-gradient(circle at 50% 25%, transparent 5%, #0078d412 6%, transparent 7%),
                            radial-gradient(circle at 60% 85%, transparent 8%, #00457810 9%, transparent 10%)
                        `,
                        mixBlendMode: isDark ? 'screen' : 'multiply',
                        opacity: 0.4,
                        pointerEvents: 'none',
                    }
                }}
            >
                Azure
            </Typography>

            {/* Efeito de ruído/textura geral */}
            <Box
                sx={{
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    right: 0,
                    bottom: 0,
                    background: `
                        radial-gradient(circle at 10% 20%, transparent 88%, ${theme.palette.primary.main}05 90%, transparent 92%),
                        radial-gradient(circle at 90% 80%, transparent 89%, ${theme.palette.primary.main}04 91%, transparent 93%),
                        radial-gradient(circle at 30% 90%, transparent 87%, ${theme.palette.primary.main}03 89%, transparent 91%),
                        radial-gradient(circle at 70% 10%, transparent 88%, ${theme.palette.primary.main}06 90%, transparent 92%)
                    `,
                    mixBlendMode: isDark ? 'screen' : 'multiply',
                    opacity: 0.2,
                    pointerEvents: 'none',
                }}
            />
        </Box>
    );
};
