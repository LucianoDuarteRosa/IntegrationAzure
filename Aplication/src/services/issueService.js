import api from './api';

/**
 * Serviço para operações com issues
 * Integra com os endpoints da API .NET
 */
export const issueService = {
    /**
     * Cria uma nova issue
     */
    async create(issueData) {
        try {
            const response = await api.post('/issues', issueData);
            return response.data;
        } catch (error) {
            console.error('Erro na API:', error);
            console.error('Erro response:', error.response);
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
                message: data.message || 'Erro na operação solicitada',
                errors: data.errors || ['Erro interno do sistema'],
                status
            };
        } else if (error.request) {
            return {
                message: 'Erro de conexão com o servidor',
                errors: ['Verifique sua conexão com a internet e tente novamente'],
                status: 0
            };
        } else {
            return {
                message: 'Erro inesperado',
                errors: ['Ocorreu um erro inesperado. Tente novamente.'],
                status: 0
            };
        }
    }
};
