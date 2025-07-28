import axios from 'axios';

// Configuração base do axios
const api = axios.create({
    baseURL: 'http://localhost:5066/api', // URL da API .NET
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    }
});

// Interceptor para adicionar token de autenticação (quando implementado)
api.interceptors.request.use(
    (config) => {
        // TODO: Adicionar token de autenticação quando implementado
        // const token = localStorage.getItem('authToken');
        // if (token) {
        //     config.headers.Authorization = `Bearer ${token}`;
        // }
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
        if (error.response?.status === 401) {
            // TODO: Redirecionar para login quando implementado
            console.error('Não autorizado');
        }
        return Promise.reject(error);
    }
);

export default api;
