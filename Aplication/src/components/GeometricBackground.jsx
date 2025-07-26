import React from 'react';
import { Box } from '@mui/material';
import { useTheme as useMuiTheme } from '@mui/material/styles';

export const GeometricBackground = ({ variant = 'default', children }) => {
    const theme = useMuiTheme();
    const isDark = theme.palette.mode === 'dark';

    const backgroundVariants = {
        default: {
            light: {
                background: `
                    linear-gradient(135deg, rgba(25, 118, 210, 0.05) 0%, rgba(33, 150, 243, 0.1) 50%, rgba(3, 169, 244, 0.05) 100%),
                    radial-gradient(circle at 20% 80%, rgba(25, 118, 210, 0.1) 0%, transparent 50%),
                    radial-gradient(circle at 80% 20%, rgba(33, 150, 243, 0.08) 0%, transparent 50%)
                `,
            },
            dark: {
                background: `
                    linear-gradient(135deg, rgba(25, 118, 210, 0.15) 0%, rgba(33, 150, 243, 0.2) 50%, rgba(3, 169, 244, 0.1) 100%),
                    radial-gradient(circle at 20% 80%, rgba(25, 118, 210, 0.2) 0%, transparent 50%),
                    radial-gradient(circle at 80% 20%, rgba(33, 150, 243, 0.15) 0%, transparent 50%)
                `,
            }
        },
        geometric: {
            light: {
                background: `
                    linear-gradient(135deg, #667eea 0%, #764ba2 100%),
                    linear-gradient(45deg, rgba(255,255,255,0.9) 0%, rgba(255,255,255,0.7) 100%)
                `,
            },
            dark: {
                background: `
                    linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%),
                    radial-gradient(circle at 30% 70%, rgba(25, 118, 210, 0.2) 0%, transparent 60%),
                    radial-gradient(circle at 70% 30%, rgba(144, 202, 249, 0.1) 0%, transparent 60%)
                `,
            }
        },
        abstract: {
            light: {
                background: `
                    linear-gradient(45deg, rgba(33, 150, 243, 0.03) 25%, transparent 25%),
                    linear-gradient(-45deg, rgba(33, 150, 243, 0.03) 25%, transparent 25%),
                    linear-gradient(45deg, transparent 75%, rgba(25, 118, 210, 0.05) 75%),
                    linear-gradient(-45deg, transparent 75%, rgba(25, 118, 210, 0.05) 75%),
                    linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)
                `,
                backgroundSize: '20px 20px, 20px 20px, 20px 20px, 20px 20px, 100% 100%',
                backgroundPosition: '0 0, 0 10px, 10px -10px, -10px 0px, 0 0',
            },
            dark: {
                background: `
                    linear-gradient(45deg, rgba(144, 202, 249, 0.05) 25%, transparent 25%),
                    linear-gradient(-45deg, rgba(144, 202, 249, 0.05) 25%, transparent 25%),
                    linear-gradient(45deg, transparent 75%, rgba(25, 118, 210, 0.08) 75%),
                    linear-gradient(-45deg, transparent 75%, rgba(25, 118, 210, 0.08) 75%),
                    linear-gradient(135deg, #1e1e1e 0%, #2d3748 100%)
                `,
                backgroundSize: '20px 20px, 20px 20px, 20px 20px, 20px 20px, 100% 100%',
                backgroundPosition: '0 0, 0 10px, 10px -10px, -10px 0px, 0 0',
            }
        }
    };

    const currentVariant = backgroundVariants[variant];
    const currentTheme = isDark ? currentVariant.dark : currentVariant.light;

    return (
        <Box
            sx={{
                position: 'relative',
                minHeight: '100vh',
                width: '100%',
                ...currentTheme,
                '&::before': {
                    content: '""',
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    right: 0,
                    bottom: 0,
                    background: isDark
                        ? `
                            radial-gradient(circle at 10% 20%, rgba(25, 118, 210, 0.1) 0%, transparent 40%),
                            radial-gradient(circle at 90% 80%, rgba(144, 202, 249, 0.08) 0%, transparent 40%),
                            radial-gradient(circle at 50% 50%, rgba(33, 150, 243, 0.05) 0%, transparent 60%)
                        `
                        : `
                            radial-gradient(circle at 10% 20%, rgba(25, 118, 210, 0.06) 0%, transparent 40%),
                            radial-gradient(circle at 90% 80%, rgba(33, 150, 243, 0.04) 0%, transparent 40%),
                            radial-gradient(circle at 50% 50%, rgba(3, 169, 244, 0.03) 0%, transparent 60%)
                        `,
                    pointerEvents: 'none',
                },
                '&::after': {
                    content: '""',
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    right: 0,
                    bottom: 0,
                    background: isDark
                        ? `
                            polygon(0 0, 100% 0, 85% 100%, 0% 100%) rgba(25, 118, 210, 0.02),
                            polygon(15% 0, 100% 0, 100% 100%, 30% 100%) rgba(144, 202, 249, 0.02)
                        `
                        : `
                            polygon(0 0, 100% 0, 85% 100%, 0% 100%) rgba(25, 118, 210, 0.01),
                            polygon(15% 0, 100% 0, 100% 100%, 30% 100%) rgba(33, 150, 243, 0.01)
                        `,
                    clipPath: isDark
                        ? `
                            polygon(0 0, 20% 0%, 40% 20%, 60% 0%, 80% 10%, 100% 0, 100% 100%, 0 100%),
                            polygon(0 0, 100% 0, 100% 80%, 90% 100%, 10% 90%, 0 100%)
                        `
                        : 'none',
                    pointerEvents: 'none',
                    opacity: 0.3,
                }
            }}
        >
            {children}
        </Box>
    );
};
