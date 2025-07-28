import { useNotification } from '../contexts/NotificationContext';

/**
 * Hook personalizado para notificações padronizadas
 * Facilita o uso das notificações em toda a aplicação
 */
export function useNotifications() {
    const { showSuccess, showError, showWarning, showInfo } = useNotification();

    // Notificações específicas para operações comuns
    const notifySuccess = {
        create: (entity = 'item') => showSuccess(
            'Criado com Sucesso!',
            `O ${entity} foi criado e salvo no sistema.`
        ),
        update: (entity = 'item') => showSuccess(
            'Atualizado com Sucesso!',
            `O ${entity} foi atualizado no sistema.`
        ),
        delete: (entity = 'item') => showSuccess(
            'Excluído com Sucesso!',
            `O ${entity} foi removido do sistema.`
        ),
        save: () => showSuccess(
            'Salvo com Sucesso!',
            'As alterações foram salvas no sistema.'
        )
    };

    const notifyError = {
        create: (entity = 'item', errors = []) => showError(
            'Erro ao Criar',
            `Não foi possível criar o ${entity}.`,
            errors
        ),
        update: (entity = 'item', errors = []) => showError(
            'Erro ao Atualizar',
            `Não foi possível atualizar o ${entity}.`,
            errors
        ),
        delete: (entity = 'item', errors = []) => showError(
            'Erro ao Excluir',
            `Não foi possível excluir o ${entity}.`,
            errors
        ),
        load: (entity = 'dados', errors = []) => showError(
            'Erro ao Carregar',
            `Não foi possível carregar os ${entity}.`,
            errors
        ),
        connection: () => showError(
            'Erro de Conexão',
            'Não foi possível conectar ao servidor. Verifique sua conexão.',
            ['Tente novamente em alguns instantes']
        ),
        validation: (errors = []) => showError(
            'Erro de Validação',
            'Alguns campos não foram preenchidos corretamente.',
            errors
        ),
        permission: () => showError(
            'Acesso Negado',
            'Você não tem permissão para realizar esta ação.'
        ),
        notFound: (entity = 'item') => showError(
            'Não Encontrado',
            `O ${entity} solicitado não foi encontrado.`
        )
    };

    const notifyWarning = {
        unsavedChanges: () => showWarning(
            'Alterações Não Salvas',
            'Você possui alterações não salvas. Deseja continuar?'
        ),
        outdatedData: () => showWarning(
            'Dados Desatualizados',
            'Os dados podem estar desatualizados. Considere atualizar a página.'
        )
    };

    const notifyInfo = {
        loading: (action = 'carregando') => showInfo(
            'Processando...',
            `Aguarde enquanto estamos ${action} os dados.`
        ),
        empty: (entity = 'dados') => showInfo(
            'Nenhum Resultado',
            `Não foram encontrados ${entity} com os filtros aplicados.`
        )
    };

    return {
        // Métodos básicos
        showSuccess,
        showError,
        showWarning,
        showInfo,

        // Métodos padronizados
        success: notifySuccess,
        error: notifyError,
        warning: notifyWarning,
        info: notifyInfo
    };
}
