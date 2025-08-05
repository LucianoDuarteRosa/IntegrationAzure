import api from './api';

/**
 * Serviço para operações com falhas
 * Integra com os endpoints da API .NET
 */
export const failureService = {
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
