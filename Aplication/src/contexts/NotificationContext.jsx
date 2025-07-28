import { createContext, useState, useContext } from 'react';

const NotificationContext = createContext({});

export function NotificationProvider({ children }) {
    const [notification, setNotification] = useState({
        open: false,
        type: 'success', // 'success', 'error', 'warning', 'info'
        title: '',
        message: '',
        errors: []
    });

    const showNotification = ({ type, title, message, errors = [] }) => {
        setNotification({
            open: true,
            type,
            title,
            message,
            errors
        });
    };

    const showSuccess = (title, message = '') => {
        showNotification({
            type: 'success',
            title,
            message
        });
    };

    const showError = (title, message = '', errors = []) => {
        showNotification({
            type: 'error',
            title,
            message,
            errors
        });
    };

    const showWarning = (title, message = '') => {
        showNotification({
            type: 'warning',
            title,
            message
        });
    };

    const showInfo = (title, message = '') => {
        showNotification({
            type: 'info',
            title,
            message
        });
    };

    const hideNotification = () => {
        setNotification(prev => ({
            ...prev,
            open: false
        }));
    };

    return (
        <NotificationContext.Provider value={{
            notification,
            showSuccess,
            showError,
            showWarning,
            showInfo,
            hideNotification
        }}>
            {children}
        </NotificationContext.Provider>
    );
}

export function useNotification() {
    const context = useContext(NotificationContext);
    if (!context) {
        throw new Error('useNotification deve ser usado dentro de NotificationProvider');
    }
    return context;
}
