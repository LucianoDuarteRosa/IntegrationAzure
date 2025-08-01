import api from './api';

/**
 * Serviço para integração com Azure DevOps
 * Agora utiliza endpoints da API backend para maior segurança
 */
export const azureDevOpsService = {
    /**
     * Busca todos os projetos do Azure DevOps
     */
    async getProjects() {
        try {
            const response = await api.get('/azuredevops/projects');

            if (response.data.success) {
                return response.data.data || [];
            } else {
                throw new Error(response.data.message || 'Erro ao buscar projetos');
            }
        } catch (error) {
            console.error('Erro ao buscar projetos do Azure DevOps:', error);

            // Não usar fallback para dados mock, deixar o componente lidar com estado vazio
            throw new Error(error.response?.data?.message || error.message || 'Erro ao conectar com Azure DevOps');
        }
    },

    /**
     * Busca work items de um projeto específico
     */
    async getWorkItems(projectId, workItemType = 'User Story') {
        try {
            const response = await api.get(`/azuredevops/projects/${projectId}/workitems?workItemType=${encodeURIComponent(workItemType)}`);

            if (response.data.success) {
                return response.data.data || [];
            } else {
                throw new Error(response.data.message || 'Erro ao buscar work items');
            }
        } catch (error) {
            console.error('Erro ao buscar work items do Azure DevOps:', error);

            // Não usar fallback para dados mock, deixar o componente lidar com estado vazio
            throw new Error(error.response?.data?.message || error.message || 'Erro ao conectar com Azure DevOps');
        }
    },

    /**
     * Busca User Stories de um projeto específico (alias para getWorkItems)
     */
    async getUserStories(projectId) {
        return this.getWorkItems(projectId, 'User Story');
    },

    /**
     * Testa a conexão com Azure DevOps
     */
    async testConnection() {
        try {
            const response = await api.get('/azuredevops/test-connection');

            if (response.data.success) {
                return {
                    isConnected: true,
                    message: response.data.message,
                    data: response.data.data
                };
            } else {
                return {
                    isConnected: false,
                    message: response.data.message || 'Erro na conexão'
                };
            }
        } catch (error) {
            console.error('Erro ao testar conexão Azure DevOps:', error);
            return {
                isConnected: false,
                message: error.response?.data?.message || error.message || 'Erro ao conectar com Azure DevOps'
            };
        }
    },

    /**
     * Método legado para compatibilidade - agora usa a API
     */
    async createUserStory(projectId, userStoryData) {
        try {
            // Por enquanto, apenas simula criação
            // Em uma implementação futura, pode-se adicionar endpoint para criar work items
            console.warn('createUserStory não implementado via API ainda');
            return {
                success: false,
                message: 'Criação de User Stories via API ainda não implementada'
            };
        } catch (error) {
            console.error('Erro ao criar user story:', error);
            throw error;
        }
    }
};
