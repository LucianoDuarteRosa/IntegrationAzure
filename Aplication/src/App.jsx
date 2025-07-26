import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { Login } from './pages/Login';
import { Home } from './components/Home';
import { UserStoryForm } from './components/UserStoryForm';
import { CssBaseline, CircularProgress, Box } from '@mui/material';

function PrivateRoute({ children }) {
  const { isAuthenticated, isLoading } = useAuth();

  // Mostra loading enquanto verifica a sessão
  if (isLoading) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          height: '100vh'
        }}
      >
        <CircularProgress />
      </Box>
    );
  }

  // Redireciona para login se não autenticado
  if (!isAuthenticated()) {
    return <Navigate to="/" replace />;
  }

  return children;
}

function App() {
  return (
    <BrowserRouter>
      <CssBaseline />
      <AuthProvider>
        <Routes>
          <Route path="/" element={<Login />} />
          <Route
            path="/dashboard"
            element={
              <PrivateRoute>
                <Home />
              </PrivateRoute>
            }
          />
          <Route
            path="/nova-historia"
            element={
              <PrivateRoute>
                <UserStoryForm />
              </PrivateRoute>
            }
          />
          <Route
            path="/nova-issue"
            element={
              <PrivateRoute>
                <div>Em desenvolvimento</div>
              </PrivateRoute>
            }
          />
          <Route
            path="/nova-falha"
            element={
              <PrivateRoute>
                <div>Em desenvolvimento</div>
              </PrivateRoute>
            }
          />
          <Route
            path="/usuarios"
            element={
              <PrivateRoute>
                <div>Em desenvolvimento</div>
              </PrivateRoute>
            }
          />
          <Route
            path="/configuracoes"
            element={
              <PrivateRoute>
                <div>Em desenvolvimento</div>
              </PrivateRoute>
            }
          />
          <Route
            path="/logs"
            element={
              <PrivateRoute>
                <div>Em desenvolvimento</div>
              </PrivateRoute>
            }
          />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
