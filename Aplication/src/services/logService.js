import api from './api';

/**
 * Serviço para operações com logs
 * Integra com os endpoints da API .NET
 */
export const logService = {
    /**
     * Obtém logs com filtros
     */
    async getLogs(filters = {}) {
        try {
            const params = new URLSearchParams();

            if (filters.userId) params.append('userId', filters.userId);
            if (filters.entity) params.append('entity', filters.entity);
            if (filters.level) params.append('level', filters.level);
            if (filters.startDate) params.append('startDate', filters.startDate);
            if (filters.endDate) params.append('endDate', filters.endDate);
            if (filters.pageSize) params.append('pageSize', filters.pageSize);
            if (filters.pageNumber) params.append('pageNumber', filters.pageNumber);

            const response = await api.get(`/logs?${params.toString()}`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    },

    /**
     * Obtém logs recentes
     */
    async getRecentLogs(count = 100) {
        try {
            const response = await api.get(`/logs/recent?count=${count}`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    },

    /**
     * Cria um novo log manual
     */
    async createLog(logData) {
        try {
            const response = await api.post('/logs', logData);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    },

    /**
     * Trata erros das requisições
     */
    handleError(error) {
        if (error.response) {
            // Erro de resposta da API
            const { data, status } = error.response;
            return {
                message: data.message || 'Erro na requisição',
                errors: data.errors || [],
                status
            };
        } else if (error.request) {
            // Erro de rede
            return {
                message: 'Erro de conexão com o servidor',
                errors: ['Verifique sua conexão com a internet'],
                status: 0
            };
        } else {
            // Outro tipo de erro
            return {
                message: 'Erro inesperado',
                errors: [error.message],
                status: 0
            };
        }
    }
};
