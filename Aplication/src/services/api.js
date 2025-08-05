import axios from 'axios';

// Configuração base do axios
const api = axios.create({
    baseURL: 'http://localhost:5066/api', // URL da API .NET
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    }
});

// Interceptor para adicionar token de autenticação e usuário atual
api.interceptors.request.use(
    (config) => {
        // TODO: Adicionar token de autenticação quando implementado
        // const token = localStorage.getItem('authToken');
        // if (token) {
        //     config.headers.Authorization = `Bearer ${token}`;
        // }

        // Adicionar usuário atual do localStorage
        const userData = localStorage.getItem('userData');
        if (userData) {
            try {
                const user = JSON.parse(userData);
                if (user.email) {
                    config.headers['X-Current-User'] = user.email;
                }
            } catch (error) {
                console.warn('Erro ao parsear userData do localStorage:', error);
            }
        }

        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Interceptor para tratamento global de respostas
api.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        // Log do erro original para debug (apenas no console)
        console.error('API Error:', error);

        // Sanitizar dados sensíveis das respostas de erro
        if (error.response) {
            const { status, data } = error.response;

            // Criar uma resposta de erro sanitizada
            const sanitizedError = {
                ...error,
                response: {
                    ...error.response,
                    data: {
                        success: false,
                        message: sanitizeErrorMessage(data?.message, status),
                        errors: sanitizeErrorArray(data?.errors, status),
                        status: status
                    }
                }
            };

            if (status === 401) {
                // Token inválido ou expirado
                localStorage.removeItem('authToken');
                localStorage.removeItem('userData');
                window.location.href = '/';
            }

            return Promise.reject(sanitizedError);
        }

        return Promise.reject(error);
    }
);

// Função para sanitizar mensagens de erro
function sanitizeErrorMessage(message, status) {
    if (!message) {
        return getGenericErrorMessage(status);
    }

    // Lista de palavras/termos que indicam informações sensíveis
    const sensitiveTerms = [
        'exception',
        'stack trace',
        'database',
        'connection string',
        'server error',
        'internal error',
        'sql',
        'query',
        'path',
        'file not found',
        'access denied',
        'unauthorized access',
        'token',
        'secret',
        'key',
        'password'
    ];

    const lowerMessage = message.toLowerCase();
    const containsSensitiveInfo = sensitiveTerms.some(term =>
        lowerMessage.includes(term.toLowerCase())
    );

    if (containsSensitiveInfo) {
        return getGenericErrorMessage(status);
    }

    return message;
}

// Função para sanitizar array de erros
function sanitizeErrorArray(errors, status) {
    if (!errors || !Array.isArray(errors)) {
        return [getGenericErrorMessage(status)];
    }

    return errors.map(error => sanitizeErrorMessage(error, status));
}

// Função para obter mensagem genérica baseada no status
function getGenericErrorMessage(status) {
    switch (status) {
        case 400:
            return 'Dados inválidos. Verifique as informações fornecidas.';
        case 401:
            return 'Sessão expirada. Faça login novamente.';
        case 403:
            return 'Você não tem permissão para realizar esta ação.';
        case 404:
            return 'Recurso não encontrado.';
        case 422:
            return 'Dados inválidos. Verifique os campos obrigatórios.';
        case 500:
            return 'Erro interno do servidor. Tente novamente mais tarde.';
        case 502:
        case 503:
        case 504:
            return 'Serviço temporariamente indisponível. Tente novamente em alguns minutos.';
        default:
            return 'Ocorreu um erro inesperado. Tente novamente.';
    }
}

export default api;
