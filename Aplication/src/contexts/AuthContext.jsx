import { createContext, useState, useContext } from 'react';

const AuthContext = createContext({});

export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);

    const login = (email, password) => {
        // Aqui você implementará a lógica de autenticação com sua API
        // Por enquanto, vamos apenas simular um login
        if (email === 'user@example.com' && password === '123456') {
            setUser({ email });
            return true;
        }
        return false;
    };

    const logout = () => {
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
}
