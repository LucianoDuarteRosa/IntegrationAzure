# IntegrationAzure - Sistema de Integração com Azure DevOps

## 📋 Visão Geral

O **IntegrationAzure** é um sistema de padronização e automatização para criação de User Stories, Issues e Failures com integração direta ao Azure DevOps. O projeto consiste em uma API backend em .NET e uma aplicação frontend em React, proporcionando formulários estruturados e padronizados que garantem consistência na documentação e envio automático para o Azure DevOps como Work Items.

## 🏗️ Arquitetura do Projeto

### Backend API (.NET)
- **Framework**: .NET Core com Entity Framework
- **Banco de Dados**: PostgreSQL
- **Arquitetura**: Clean Architecture (Domain, Application, Infrastructure)
- **Documentação**: Swagger/OpenAPI integrado
- **Padrões**: Repository Pattern, Dependency Injection, FluentValidation

### Frontend (React)
- **Framework**: React 19 com Vite
- **UI Library**: Material-UI (MUI) v7
- **Formulários**: React Hook Form com Yup validation
- **Navegação**: React Router DOM
- **Cliente HTTP**: Axios

## 🚀 Principais Funcionalidades

### 1. Padronização de User Stories
- **Formulários Estruturados**: Templates padronizados seguindo metodologias ágeis
- **Critérios de Aceite**: Campos obrigatórios para definição clara de critérios
- **Estrutura Como/Quero/Para**: Formatação automática padrão de user stories
- **Seções Organizadas**: Impacto, objetivos, campos de formulário, regras de negócio
- **Cenários de Teste**: Estrutura Given/When/Then padronizada
- **Anexos Padronizados**: Upload organizado de screenshots e documentos
- **Envio Automático**: Criação direta no Azure DevOps como Work Items

### 2. Padronização de Issues
- **Classificação Padronizada**: Tipos (Bug, Feature Request, Task), prioridades e ambientes
- **Vinculação Obrigatória**: Relacionamento mandatório com User Stories
- **Atividades Estruturadas**: Integração com atividades do Azure DevOps (Development, Testing, etc.)
- **Cenários Reproduzíveis**: Estrutura Given/When/Then para reprodução padronizada
- **Campos Organizados**: Observações e contexto estruturados
- **Descrição Automática**: Geração de HTML estruturado para o Azure DevOps
- **Envio Direto**: Criação automática como Work Items no Azure DevOps

### 3. Padronização de Failures (Falhas)
- **Severidade Classificada**: Sistema padronizado (Critical, High, Medium, Low)
- **Status Controlado**: Estados padronizados (Reported, InProgress, Resolved, Closed)
- **Documentação Estruturada**: Campos obrigatórios para informações detalhadas
- **Cenários Padronizados**: Detalhamento estruturado de passos, resultados esperados e obtidos
- **Envio como Bugs**: Criação automática como Bugs no Azure DevOps
- **Ambiente Especificado**: Campo obrigatório para especificação do ambiente

### 4. Sistema de Usuários e Controle de Acesso
- **Autenticação Simples**: Sistema de usuários para controle de autoria
- **Perfis Diferenciados**: Desenvolvedor e Usuário com permissões específicas
- **Cadastro de Usuários**: CRUD básico de usuários para identificação
- **Upload de Perfil**: Sistema simples de upload para fotos de perfil
- **Controle de Autoria**: Rastreamento de quem criou cada item

### 5. Integração Direta com Azure DevOps
- **Envio Automático**: Criação imediata de Work Items após preenchimento
- **Relacionamentos Automáticos**: Hierarquia Parent-Child entre Work Items
- **Upload de Anexos**: Envio automático de arquivos e imagens
- **Mapeamento de Campos**: Conversão automática dos campos padronizados
- **Seleção de Projetos**: Escolha do projeto de destino no Azure DevOps
- **Activities Dinâmicas**: Carregamento das atividades permitidas do Azure DevOps

### 6. Sistema de Logs e Rastreabilidade
- **Registro de Ações**: Log de todas as criações e envios para o Azure DevOps
- **Níveis de Log**: Success, Info, Warning, Error para monitoramento
- **Rastreabilidade**: Informações sobre usuário, ação, entidade e timestamp
- **Auditoria de Envios**: Registro de todos os itens enviados ao Azure DevOps

### 7. Sistema de Configurações para Integração
- **Configurações de Conexão**: Gestão de URLs e tokens do Azure DevOps
- **Configurações por Categoria**: Organização das configurações de integração
- **Configurações Secretas**: Proteção de tokens e informações sensíveis
- **Projetos Padrão**: Configuração de projetos de destino no Azure DevOps

## 🛠️ Tecnologias Utilizadas

### Backend
- **.NET Core 6+**: Framework principal
- **Entity Framework Core**: ORM com PostgreSQL
- **Npgsql**: Driver PostgreSQL
- **FluentValidation**: Validação de dados estruturada
- **Swagger/OpenAPI**: Documentação automática da API
- **System.Text.Json**: Serialização JSON nativa

### Frontend
- **React 19**: Biblioteca de UI mais recente
- **Material-UI v7**: Componentes de interface modernos
- **React Hook Form**: Gerenciamento eficiente de formulários
- **Yup**: Validação de esquemas
- **Axios**: Cliente HTTP com interceptadores
- **React Router DOM**: Roteamento SPA
- **Vite**: Build tool rápido

### Banco de Dados
- **PostgreSQL**: Banco principal com suporte a JSON
- **Entity Framework Migrations**: Versionamento do schema
- **Data Seeding**: Dados iniciais automáticos

### Infraestrutura
- **CORS**: Configuração para frontend React
- **Static Files**: Servir arquivos de upload
- **Exception Handling**: Tratamento global de erros
- **File Upload**: Sistema de upload com validação

## 🔄 Fluxo de Trabalho

### 1. Criação Padronizada de User Story
1. Usuário preenche formulário estruturado com campos obrigatórios e opcionais
2. Sistema valida dados usando FluentValidation para garantir qualidade
3. Serviço HTML Generator cria descrição formatada e padronizada
4. Entidade é salva temporariamente no PostgreSQL para controle
5. Work Item é enviado automaticamente para o Azure DevOps
6. Anexos são transferidos diretamente para o Azure DevOps

### 2. Criação Padronizada de Issue
1. Usuário seleciona User Story relacionada (campo obrigatório)
2. Sistema carrega atividades disponíveis do Azure DevOps
3. Formulário padronizado é preenchido com cenários estruturados
4. Issue é processada e enviada como Work Item filho no Azure DevOps
5. Relacionamento Parent-Child é estabelecido automaticamente

### 3. Registro Padronizado de Falhas
1. Formulário estruturado de Failure é preenchido com campos obrigatórios
2. Severidade e ambiente são classificados conforme padrões
3. Bug é criado automaticamente no Azure DevOps
4. Relacionamento com User Story (quando aplicável) é estabelecido
5. Status de acompanhamento é sincronizado

## ⚙️ Configuração e Deploy

### Pré-requisitos
- **.NET SDK 6.0+**
- **Node.js 16.0+**
- **PostgreSQL 12+**
- **Azure DevOps Organization**

### Configuração do Backend
```bash
# Navegar para o diretório da API
cd Api/IntegrationAzure.Api

# Restaurar pacotes
dotnet restore

# Configurar string de conexão no appsettings.json
# Executar migrações
dotnet ef database update

# Executar a aplicação
dotnet run
```

### Configuração do Frontend
```bash
# Navegar para o diretório da aplicação
cd Aplication

# Instalar dependências
npm install

# Executar em modo desenvolvimento
npm run dev
```

### Configurações Necessárias
- **Azure DevOps**: URL da organização, Personal Access Token
- **PostgreSQL**: String de conexão
- **Upload**: Configuração de diretórios de upload
- **CORS**: URLs permitidas para o frontend

## 📁 Estrutura do Projeto

```
IntegrationAzure/
├── Api/
│   └── IntegrationAzure.Api/
│       ├── Controllers/         # Controladores da API
│       ├── Application/         # Camada de aplicação
│       │   ├── DTOs/           # Data Transfer Objects
│       │   ├── Services/       # Serviços de aplicação
│       │   └── Validators/     # Validadores FluentValidation
│       ├── Domain/             # Camada de domínio
│       │   ├── Entities/       # Entidades do domínio
│       │   └── Interfaces/     # Contratos e interfaces
│       ├── Infrastructure/     # Camada de infraestrutura
│       │   ├── Data/          # Contexto do Entity Framework
│       │   └── Repositories/  # Implementações dos repositórios
│       └── Migrations/        # Migrações do banco de dados
├── Aplication/                # Frontend React
│   ├── src/
│   │   ├── components/        # Componentes React
│   │   ├── pages/            # Páginas da aplicação
│   │   ├── services/         # Serviços HTTP
│   │   ├── contexts/         # Contextos React
│   │   └── hooks/            # Hooks customizados
│   └── public/               # Arquivos públicos
└── uploads/                  # Diretório de uploads
```

## 🤝 Contribuição

Para contribuir com o projeto:

1. Faça um fork do repositório
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto é proprietário e desenvolvido para uso interno.

## 👥 Equipe

- **Desenvolvedor Principal**: Luciano Duarte Rosa
- **Repositório**: [IntegrationAzure](https://github.com/LucianoDuarteRosa/IntegrationAzure)

## 📞 Suporte

Para suporte e dúvidas sobre o sistema, entre em contato com a equipe de desenvolvimento.
