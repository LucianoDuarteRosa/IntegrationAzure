import api from './api';

/**
 * Serviço para operações de autenticação
 * Por enquanto simula autenticação, mas pode ser integrado com Azure AD
 */
export const authService = {
    /**
     * Realiza login (simulado)
     */
    async login(email, password) {
        try {
            // Por enquanto, simula uma autenticação bem-sucedida
            // TODO: Integrar com API de autenticação real

            if (!email || !password) {
                throw new Error('Email e senha são obrigatórios');
            }

            // Simula validação
            if (email === 'admin@admin' && password === '123') {
                const userData = {
                    id: '1',
                    email: email,
                    name: 'Administrador',
                    role: 'admin',
                    token: 'mock-jwt-token-' + Date.now()
                };

                // Salva no localStorage
                localStorage.setItem('authToken', userData.token);
                localStorage.setItem('userData', JSON.stringify(userData));

                return {
                    success: true,
                    message: 'Login realizado com sucesso',
                    data: userData
                };
            } else {
                throw new Error('Email ou senha inválidos');
            }
        } catch (error) {
            console.error('Erro ao fazer login:', error);
            return {
                success: false,
                message: error.message || 'Erro ao fazer login',
                errors: [error.message || 'Credenciais inválidas']
            };
        }
    },

    /**
     * Realiza logout
     */
    logout() {
        try {
            localStorage.removeItem('authToken');
            localStorage.removeItem('userData');
            return {
                success: true,
                message: 'Logout realizado com sucesso'
            };
        } catch (error) {
            console.error('Erro ao fazer logout:', error);
            return {
                success: false,
                message: 'Erro ao fazer logout'
            };
        }
    },

    /**
     * Verifica se o usuário está autenticado
     */
    isAuthenticated() {
        const token = localStorage.getItem('authToken');
        const userData = localStorage.getItem('userData');
        return !!(token && userData);
    },

    /**
     * Obtém dados do usuário atual
     */
    getCurrentUser() {
        try {
            const userData = localStorage.getItem('userData');
            return userData ? JSON.parse(userData) : null;
        } catch (error) {
            console.error('Erro ao obter dados do usuário:', error);
            return null;
        }
    },

    /**
     * Obtém token de autenticação
     */
    getToken() {
        return localStorage.getItem('authToken');
    },

    /**
     * Verifica se a sessão é válida
     */
    async checkSession() {
        try {
            // TODO: Fazer verificação real com a API
            // const response = await api.get('/auth/validate');
            // return response.data;

            // Por enquanto, só verifica se existe token
            return this.isAuthenticated();
        } catch (error) {
            console.error('Erro ao verificar sessão:', error);
            this.logout(); // Remove tokens inválidos
            return false;
        }
    }
};
