import api from './api';

/**
 * Serviço para operações com histórias de usuário
 * Integra com os endpoints da API .NET
 */
export const userStoryService = {
    /**
     * Cria uma nova história de usuário
     */
    async create(userStoryData) {
        try {
            const response = await api.post('/userstories', userStoryData);
            return response.data;
        } catch (error) {
            throw this.handleError(error);
        }
    },

    /**
     * Cria uma nova história de usuário com anexos
     */
    async createWithAttachments(userStoryData, attachmentFiles, screenshotFiles) {
        try {
            const formData = new FormData();

            // Adicionar os dados JSON
            formData.append('jsonData', JSON.stringify(userStoryData));

            // Adicionar arquivos de anexos
            if (attachmentFiles && attachmentFiles.length > 0) {
                attachmentFiles.forEach((file, index) => {
                    formData.append(`files`, file);
                });
            }

            // Adicionar arquivos de screenshots
            if (screenshotFiles && screenshotFiles.length > 0) {
                screenshotFiles.forEach((file, index) => {
                    formData.append(`files`, file);
                });
            }

            const response = await api.post('/userstories/with-attachments', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });

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
                message: data.message || 'Erro na operação solicitada',
                errors: data.errors || ['Erro interno do sistema'],
                status
            };
        } else if (error.request) {
            // Erro de rede
            return {
                message: 'Erro de conexão com o servidor',
                errors: ['Verifique sua conexão com a internet e tente novamente'],
                status: 0
            };
        } else {
            // Outro tipo de erro
            return {
                message: 'Erro inesperado',
                errors: ['Ocorreu um erro inesperado. Tente novamente.'],
                status: 0
            };
        }
    }
};
