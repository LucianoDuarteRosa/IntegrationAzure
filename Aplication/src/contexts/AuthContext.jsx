import { createContext, useState, useContext, useEffect } from 'react';

// Constantes para gerenciamento de sessão
const SESSION_STORAGE_KEY = 'userSession';
const SESSION_EXPIRY_KEY = 'sessionExpiry';
const SESSION_DURATION = 8 * 60 * 60 * 1000; // 8 horas em millisegundos

const AuthContext = createContext({});

// Utilitários para gerenciar sessão
const sessionUtils = {
    // Salva sessão no localStorage
    saveSession(userData) {
        const expiryTime = Date.now() + SESSION_DURATION;
        localStorage.setItem(SESSION_STORAGE_KEY, JSON.stringify(userData));
        localStorage.setItem(SESSION_EXPIRY_KEY, expiryTime.toString());
    },

    // Carrega sessão do localStorage
    loadSession() {
        try {
            const userData = localStorage.getItem(SESSION_STORAGE_KEY);
            const expiryTime = localStorage.getItem(SESSION_EXPIRY_KEY);

            if (!userData || !expiryTime) {
                return null;
            }

            // Verifica se a sessão expirou
            if (Date.now() > parseInt(expiryTime)) {
                this.clearSession();
                return null;
            }

            return JSON.parse(userData);
        } catch (error) {
            console.error('Erro ao carregar sessão:', error);
            this.clearSession();
            return null;
        }
    },

    // Remove sessão do localStorage
    clearSession() {
        localStorage.removeItem(SESSION_STORAGE_KEY);
        localStorage.removeItem(SESSION_EXPIRY_KEY);
    },

    // Verifica se sessão ainda é válida
    isSessionValid() {
        const expiryTime = localStorage.getItem(SESSION_EXPIRY_KEY);
        if (!expiryTime) return false;
        return Date.now() < parseInt(expiryTime);
    },

    // Renova o tempo de expiração da sessão
    renewSession() {
        if (this.isSessionValid()) {
            const expiryTime = Date.now() + SESSION_DURATION;
            localStorage.setItem(SESSION_EXPIRY_KEY, expiryTime.toString());
        }
    }
};

export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    // Carrega sessão salva ao inicializar o app
    useEffect(() => {
        const savedUser = sessionUtils.loadSession();
        if (savedUser) {
            setUser(savedUser);
            // Renova a sessão se ainda válida
            sessionUtils.renewSession();
        }
        setIsLoading(false);
    }, []);

    // Auto-renovação de sessão (verifica a cada 5 minutos)
    useEffect(() => {
        if (!user) return;

        const interval = setInterval(() => {
            if (sessionUtils.isSessionValid()) {
                sessionUtils.renewSession();
            } else {
                // Sessão expirou, faz logout automático
                handleLogout();
            }
        }, 5 * 60 * 1000); // 5 minutos

        return () => clearInterval(interval);
    }, [user]);

    const login = (email, password) => {
        // Aqui você implementará a lógica de autenticação com sua API
        // Por enquanto, vamos apenas simular um login
        if (email === 'user@example.com' && password === '123456') {
            const userData = {
                email,
                loginTime: new Date().toISOString(),
                id: Date.now() // Simula um ID único
            };

            setUser(userData);
            sessionUtils.saveSession(userData);
            return true;
        }
        return false;
    };

    const handleLogout = () => {
        setUser(null);
        sessionUtils.clearSession();
    };

    // Função para verificar se usuário está autenticado
    const isAuthenticated = () => {
        return user !== null && sessionUtils.isSessionValid();
    };

    // Função para obter tempo restante da sessão
    const getSessionTimeRemaining = () => {
        const expiryTime = localStorage.getItem(SESSION_EXPIRY_KEY);
        if (!expiryTime) return 0;

        const remaining = parseInt(expiryTime) - Date.now();
        return Math.max(0, remaining);
    };

    return (
        <AuthContext.Provider value={{
            user,
            login,
            logout: handleLogout,
            isLoading,
            isAuthenticated,
            getSessionTimeRemaining
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
