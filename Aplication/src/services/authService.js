/**
 * Serviço para operações de autenticação
 * Integrado com a API real do backend
 */

const API_BASE_URL = 'http://localhost:5066/api';

export const authService = {
    /**
     * Realiza login real com a API
     */
    async login(email, password) {
        try {
            if (!email || !password) {
                throw new Error('Email e senha são obrigatórios');
            }

            const response = await fetch(`${API_BASE_URL}/auth/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    email: email,
                    password: password
                })
            });

            const data = await response.json();

            // Verifica se a resposta foi bem-sucedida
            if (response.ok && data.success === true) {
                const userData = data.data;
                console.log('authService.login - raw userData:', userData); // Debug

                // Normalizar os dados do usuário para o formato esperado pelo frontend
                const normalizedUser = {
                    id: userData.id,
                    email: userData.email,
                    name: userData.name,
                    nickname: userData.nickname,
                    profileImagePath: userData.profileImagePath,
                    profile: {
                        id: userData.profile?.id,
                        name: userData.profile?.name
                    },
                    token: 'api-token-' + Date.now() // Por enquanto, gera um token mock
                };

                console.log('authService.login - normalizedUser:', normalizedUser); // Debug

                // Salvar no localStorage
                localStorage.setItem('authToken', normalizedUser.token);
                localStorage.setItem('userData', JSON.stringify(normalizedUser));

                return {
                    success: true,
                    message: data.message || 'Login realizado com sucesso',
                    data: normalizedUser
                };
            } else {
                // Erro de autenticação
                const errorMessage = data.message || 'Email ou senha incorretos';
                throw new Error(errorMessage);
            }
        } catch (error) {
            console.error('Login error:', error); // Log apenas no console para debug

            return {
                success: false,
                message: 'Erro ao realizar login. Verifique suas credenciais.',
                errors: ['Email ou senha incorretos. Tente novamente.']
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
        } catch {
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
        } catch {
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
        } catch {
            this.logout(); // Remove tokens inválidos
            return false;
        }
    },

    /**
     * Obtém tempo restante da sessão (em milissegundos)
     * Por enquanto retorna um valor fixo simulado
     */
    getSessionTimeRemaining() {
        try {
            const userData = this.getCurrentUser();
            if (!userData) return 0;

            // Simula uma sessão de 8 horas
            const sessionDuration = 8 * 60 * 60 * 1000; // 8 horas em milissegundos
            const loginTime = parseInt(userData.token.split('-').pop());
            const currentTime = Date.now();
            const elapsed = currentTime - loginTime;
            const remaining = sessionDuration - elapsed;

            return remaining > 0 ? remaining : 0;
        } catch {
            return 0;
        }
    },

    /**
     * Atualiza os dados do usuário na sessão atual
     */
    updateCurrentUser(userData) {
        try {
            localStorage.setItem('currentUser', JSON.stringify(userData));
            return true;
        } catch {
            return false;
        }
    }
};
