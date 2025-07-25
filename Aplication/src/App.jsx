import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { Login } from './pages/Login';
import { Home } from './components/Home';
import { UserStoryForm } from './components/UserStoryForm';
import { CssBaseline } from '@mui/material';

function PrivateRoute({ children }) {
  const { user } = useAuth();

  if (!user) {
    return <Navigate to="/" />;
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
