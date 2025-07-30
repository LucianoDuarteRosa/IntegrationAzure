import api from './api';

/**
 * Serviço para operações com falhas
 * Integra com os endpoints da API .NET
 */
export const failureService = {
    /**
     * Obtém todos os falhas
     */
    async getAll() {
        try {
            const response = await api.get('/failures');
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    },

    /**
     * Obtém todos os tipos de ocorrência disponíveis
     */
    async getOccurrenceTypes() {
        try {
            const response = await api.get('/failures/occurrence-types');
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    },

    /**
     * Obtém uma falha específica por ID
     */
    async getById(id) {
        try {
            const response = await api.get(`/failures/${id}`);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    },

    /**
     * Cria uma nova falha
     */
    async create(failureData) {
        try {
            const response = await api.post('/failures', failureData);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    },

    /**
     * Atualiza uma falha existente
     */
    async update(id, failureData) {
        try {
            const response = await api.put(`/failures/${id}`, failureData);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    },

    /**
     * Exclui uma falha
     */
    async delete(id) {
        try {
            const response = await api.delete(`/failures/${id}`);
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
            const { data, status } = error.response;
            return {
                message: data.message || 'Erro na requisição',
                errors: data.errors || [],
                status
            };
        } else if (error.request) {
            return {
                message: 'Erro de conexão com o servidor',
                errors: ['Verifique sua conexão com a internet'],
                status: 0
            };
        } else {
            return {
                message: 'Erro inesperado',
                errors: [error.message],
                status: 0
            };
        }
    }
};
