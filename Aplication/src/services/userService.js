import api from './api';

// Serviço para gerenciar perfis
const profileService = {
    async getActiveProfiles() {
        const response = await api.get('/profiles/active');
        return response.data;
    }
};

// Serviço para gerenciar usuários
const userService = {
    async getUsers() {
        const response = await api.get('/users');
        return response.data;
    },

    async getUser(id) {
        const response = await api.get(`/users/${id}`);
        return response.data;
    },

    async getUsersByProfile(profileId) {
        const response = await api.get(`/users/profile/${profileId}`);
        return response.data;
    },

    async createUser(userData) {
        const response = await api.post('/users', userData);
        return response.data;
    },

    async updateUser(id, userData) {
        const response = await api.put(`/users/${id}`, userData);
        return response.data;
    },

    async deleteUser(id) {
        const response = await api.delete(`/users/${id}`);
        return response.data;
    },

    async changePassword(id, passwordData) {
        const response = await api.patch(`/users/${id}/change-password`, passwordData);
        return response.data;
    },

    async adminChangePassword(id, passwordData) {
        const response = await api.patch(`/users/${id}/admin-change-password`, passwordData);
        return response.data;
    }
};

export { profileService, userService };
