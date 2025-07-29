import api from './api';

// Serviço para upload de arquivos
const fileUploadService = {
    /**
     * Upload de imagem de perfil de usuário
     * @param {File} file - Arquivo de imagem
     * @returns {Promise} Resposta com o caminho da imagem
     */
    async uploadUserProfileImage(file) {
        const formData = new FormData();
        formData.append('file', file);

        const response = await api.post('/fileupload/user-profile-image', formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
        return response.data;
    },

    /**
     * Remove uma imagem de perfil
     * @param {string} fileName - Nome do arquivo a ser removido
     * @returns {Promise} Resposta da operação
     */
    async deleteUserProfileImage(fileName) {
        const response = await api.delete('/fileupload/user-profile-image', {
            params: { fileName }
        });
        return response.data;
    },

    /**
     * Obtém informações sobre tipos de arquivo permitidos
     * @returns {Promise} Informações sobre tipos e limites
     */
    async getAllowedTypes() {
        const response = await api.get('/fileupload/allowed-types');
        return response.data;
    },

    /**
     * Gera URL completa para uma imagem
     * @param {string} imagePath - Caminho relativo da imagem ou nome do arquivo
     * @returns {string} URL completa da imagem
     */
    getImageUrl(imagePath) {
        if (!imagePath) return null;

        console.log('fileUploadService.getImageUrl - input:', imagePath); // Debug

        // Se já é uma URL completa, retorna como está
        if (imagePath.startsWith('http')) {
            return imagePath;
        }

        // Constrói a URL baseada na configuração da API
        const baseURL = api.defaults.baseURL?.replace('/api', '') || 'http://localhost:5066';
        console.log('fileUploadService.getImageUrl - baseURL:', baseURL); // Debug

        // Se é apenas o nome do arquivo, adiciona o caminho completo
        let fullPath = imagePath;
        if (!imagePath.startsWith('/uploads/users/')) {
            fullPath = `/uploads/users/${imagePath}`;
        }

        const finalURL = `${baseURL}${fullPath}`;
        console.log('fileUploadService.getImageUrl - finalURL:', finalURL); // Debug
        return finalURL;
    },

    /**
     * Valida se o arquivo é uma imagem válida
     * @param {File} file - Arquivo a ser validado
     * @returns {Object} Resultado da validação
     */
    validateImageFile(file) {
        const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/bmp', 'image/webp'];
        const maxSizeBytes = 5 * 1024 * 1024; // 5MB

        const result = {
            isValid: true,
            errors: []
        };

        if (!file) {
            result.isValid = false;
            result.errors.push('Nenhum arquivo selecionado');
            return result;
        }

        if (!allowedTypes.includes(file.type)) {
            result.isValid = false;
            result.errors.push('Tipo de arquivo não permitido. Use: JPG, PNG, GIF, BMP ou WEBP');
        }

        if (file.size > maxSizeBytes) {
            result.isValid = false;
            result.errors.push(`Arquivo muito grande. Tamanho máximo: ${maxSizeBytes / (1024 * 1024)}MB`);
        }

        return result;
    }
};

export { fileUploadService };
