import React, { createContext, useContext, useState, useEffect } from 'react';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { CssBaseline } from '@mui/material';

const ThemeContext = createContext();

export const useTheme = () => {
    const context = useContext(ThemeContext);
    if (!context) {
        throw new Error('useTheme deve ser usado dentro de um CustomThemeProvider');
    }
    return context;
};

export const CustomThemeProvider = ({ children }) => {
    const [mode, setMode] = useState(() => {
        // Verifica se há preferência salva no localStorage
        const savedMode = localStorage.getItem('themeMode');
        if (savedMode) {
            return savedMode;
        }
        // Se não há preferência salva, usa a preferência do sistema
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    });

    // Salva a preferência no localStorage quando muda
    useEffect(() => {
        localStorage.setItem('themeMode', mode);
    }, [mode]);

    const toggleColorMode = () => {
        setMode((prevMode) => (prevMode === 'light' ? 'dark' : 'light'));
    };

    const theme = React.useMemo(
        () =>
            createTheme({
                palette: {
                    mode,
                    ...(mode === 'light'
                        ? {
                            // Tema claro
                            primary: {
                                main: '#1976d2',
                            },
                            secondary: {
                                main: '#dc004e',
                            },
                            background: {
                                default: '#fafafa',
                                paper: '#ffffff',
                            },
                            text: {
                                primary: '#1a202c',
                                secondary: '#718096',
                            },
                        }
                        : {
                            // Tema escuro
                            primary: {
                                main: '#90caf9',
                            },
                            secondary: {
                                main: '#f48fb1',
                            },
                            background: {
                                default: '#121212',
                                paper: '#1e1e1e',
                            },
                            text: {
                                primary: '#ffffff',
                                secondary: '#b0b0b0',
                            },
                        }),
                },
                typography: {
                    fontFamily: '"Mulish", system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif',
                    h4: {
                        fontWeight: 700,
                    },
                    h5: {
                        fontWeight: 600,
                    },
                    h6: {
                        fontWeight: 600,
                    },
                },
                components: {
                    // Customizações globais dos componentes
                    MuiButton: {
                        styleOverrides: {
                            root: {
                                textTransform: 'none', // Remove uppercase automático
                                borderRadius: 8,
                                fontWeight: 500,
                            },
                        },
                    },
                    MuiAppBar: {
                        styleOverrides: {
                            root: {
                                borderRadius: 0, // Remove bordas arredondadas da navbar
                            },
                        },
                    },
                    MuiPaper: {
                        styleOverrides: {
                            root: {
                                borderRadius: 12,
                            },
                        },
                    },
                    MuiTextField: {
                        styleOverrides: {
                            root: {
                                '& .MuiOutlinedInput-root': {
                                    borderRadius: 8,
                                },
                            },
                        },
                    },
                },
            }),
        [mode],
    );

    return (
        <ThemeContext.Provider value={{ mode, toggleColorMode }}>
            <ThemeProvider theme={theme}>
                <CssBaseline />
                {children}
            </ThemeProvider>
        </ThemeContext.Provider>
    );
};
