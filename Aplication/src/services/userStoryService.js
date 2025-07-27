import api from './api';

/**
 * Serviço para operações com histórias de usuário
 * Integra com os endpoints da API .NET
 */
export const userStoryService = {
    /**
     * Obtém todas as histórias de usuário
     */
    async getAll() {
        try {
            const response = await api.get('/userstories');
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar histórias:', error);
            throw this.handleError(error);
        }
    },

    /**
     * Obtém uma história específica por ID
     */
    async getById(id) {
        try {
            const response = await api.get(`/userstories/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar história:', error);
            throw this.handleError(error);
        }
    },

    /**
     * Cria uma nova história de usuário
     */
    async create(userStoryData) {
        try {
            const response = await api.post('/userstories', userStoryData);
            return response.data;
        } catch (error) {
            console.error('Erro ao criar história:', error);
            throw this.handleError(error);
        }
    },

    /**
     * Atualiza uma história existente
     */
    async update(id, userStoryData) {
        try {
            const response = await api.put(`/userstories/${id}`, userStoryData);
            return response.data;
        } catch (error) {
            console.error('Erro ao atualizar história:', error);
            throw this.handleError(error);
        }
    },

    /**
     * Exclui uma história
     */
    async delete(id) {
        try {
            const response = await api.delete(`/userstories/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao excluir história:', error);
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
