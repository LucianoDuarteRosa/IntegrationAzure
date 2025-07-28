import api from './api';

export const configurationService = {
    async getAll() {
        const response = await api.get('/configurations');
        return response.data;
    },

    async getById(id) {
        const response = await api.get(`/configurations/${id}`);
        return response.data;
    },

    async getByKey(key) {
        const response = await api.get(`/configurations/key/${key}`);
        return response.data;
    },

    async getByCategory(category) {
        const response = await api.get(`/configurations/category/${category}`);
        return response.data;
    },

    async create(configurationData) {
        const response = await api.post('/configurations', configurationData);
        return response.data;
    },

    async update(id, configurationData) {
        const response = await api.put(`/configurations/${id}`, configurationData);
        return response.data;
    },

    async delete(id) {
        const response = await api.delete(`/configurations/${id}`);
        return response.data;
    }
};
