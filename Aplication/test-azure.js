// Teste direto da API do Azure DevOps
const testAzureConnection = async () => {
    const organization = 'IntegrationAzureTeste';
    const apiVersion = '7.1-preview.3';

    // Primeiro vamos testar se conseguimos acessar a API local
    try {
        console.log('Testando API local...');
        const localResponse = await fetch('http://localhost:5066/api/configurations/key/Azure_Token');
        const localData = await localResponse.json();
        console.log('Resposta da API local:', localData);

        if (!localData.success) {
            console.error('Erro ao buscar token local:', localData.message);
            return;
        }

        const token = localData.data.value;
        console.log('Token obtido (mascarado):', token);

        if (token === '*****') {
            console.error('Token está mascarado. Vamos buscar o token real do backend.');
            return;
        }

        // Testar conexão com Azure DevOps
        console.log('\nTestando conexão com Azure DevOps...');
        const azureUrl = `https://dev.azure.com/${organization}/_apis/projects?api-version=${apiVersion}`;
        console.log('URL do Azure:', azureUrl);

        const azureResponse = await fetch(azureUrl, {
            headers: {
                'Authorization': `Basic ${btoa(':' + token)}`,
                'Content-Type': 'application/json'
            }
        });

        console.log('Status da resposta do Azure:', azureResponse.status);

        if (azureResponse.ok) {
            const azureData = await azureResponse.json();
            console.log('Projetos encontrados:', azureData.count);
            console.log('Projetos:', azureData.value?.map(p => ({ id: p.id, name: p.name })));
        } else {
            const errorText = await azureResponse.text();
            console.error('Erro do Azure DevOps:', azureResponse.status, errorText);
        }

    } catch (error) {
        console.error('Erro no teste:', error);
    }
};

// Executar teste
testAzureConnection();
