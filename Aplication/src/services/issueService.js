import api from './api';

/**
 * Serviço para operações com issues
 * Integra com os endpoints da API .NET
 */
export const issueService = {
    /**
     * Obtém todas as issues
     */
    async getAll() {
        try {
            const response = await api.get('/issues');
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar issues:', error);
            throw this.handleError(error);
        }
    },

    /**
     * Obtém uma issue específica por ID
     */
    async getById(id) {
        try {
            const response = await api.get(`/issues/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar issue:', error);
            throw this.handleError(error);
        }
    },

    /**
     * Cria uma nova issue
     */
    async create(issueData) {
        try {
            const response = await api.post('/issues', issueData);
            return response.data;
        } catch (error) {
            console.error('Erro ao criar issue:', error);
            throw this.handleError(error);
        }
    },

    /**
     * Atualiza uma issue existente
     */
    async update(id, issueData) {
        try {
            const response = await api.put(`/issues/${id}`, issueData);
            return response.data;
        } catch (error) {
            console.error('Erro ao atualizar issue:', error);
            throw this.handleError(error);
        }
    },

    /**
     * Exclui uma issue
     */
    async delete(id) {
        try {
            const response = await api.delete(`/issues/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao excluir issue:', error);
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
