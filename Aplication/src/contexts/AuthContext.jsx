import { createContext, useState, useContext, useEffect } from 'react';
import { authService } from '../services';

const AuthContext = createContext({});

export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    // Carrega sessão salva ao inicializar o app
    useEffect(() => {
        const checkExistingSession = async () => {
            try {
                const isValid = await authService.checkSession();
                if (isValid) {
                    const userData = authService.getCurrentUser();
                    setUser(userData);
                }
            } catch {
                // Erro silencioso na verificação de sessão
            }
            setIsLoading(false);
        };

        checkExistingSession();
    }, []);

    // Função de login integrada com o serviço
    const login = async (email, password) => {
        try {
            const result = await authService.login(email, password);

            if (result.success) {
                setUser(result.data);
                return { success: true, message: result.message };
            } else {
                return { success: false, message: result.message, errors: result.errors };
            }
        } catch (error) {
            return {
                success: false,
                message: 'Erro interno. Tente novamente.',
                errors: [error.message]
            };
        }
    };

    // Função de logout
    const handleLogout = () => {
        const result = authService.logout();
        setUser(null);
        return result;
    };

    // Função para verificar se usuário está autenticado
    const isAuthenticated = () => {
        return authService.isAuthenticated() && user !== null;
    };

    // Função para obter dados do usuário atual
    const getCurrentUser = () => {
        return authService.getCurrentUser();
    };

    // Função para obter tempo restante da sessão
    const getSessionTimeRemaining = () => {
        return authService.getSessionTimeRemaining();
    };

    // Função para atualizar dados do usuário
    const updateUser = (userData) => {
        setUser(userData);
        // Também atualiza no localStorage/sessionStorage
        authService.updateCurrentUser(userData);
    };

    return (
        <AuthContext.Provider value={{
            user,
            login,
            logout: handleLogout,
            isLoading,
            isAuthenticated,
            getCurrentUser,
            getSessionTimeRemaining,
            updateUser
        }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
}
