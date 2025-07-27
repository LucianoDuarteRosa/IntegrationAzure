# Integration Azure API

API REST desenvolvida em .NET 8 para gerenciamento de Histórias de Usuário, Issues e Falhas, com integração ao Azure.

## 🏗️ Arquitetura

A API segue os princípios de **Domain-Driven Design (DDD)** e **Clean Architecture**:

```
├── Domain/                  # Entidades e interfaces do domínio
│   ├── Entities/           # Entidades de domínio
│   └── Interfaces/         # Contratos dos repositórios
├── Infrastructure/         # Implementação de infraestrutura
│   ├── Data/              # Contexto do banco de dados
│   └── Repositories/      # Implementação dos repositórios
├── Application/           # Camada de aplicação
│   ├── DTOs/             # Data Transfer Objects
│   ├── Services/         # Serviços de aplicação
│   └── Validators/       # Validadores FluentValidation
└── Controllers/          # Controllers da API
```

## 🚀 Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **FluentValidation** - Validação de dados
- **Swagger/OpenAPI** - Documentação da API
- **AutoMapper** - Mapeamento de objetos (implícito)

## 📋 Funcionalidades

### Histórias de Usuário
- ✅ Criar nova história
- ✅ Listar todas as histórias
- ✅ Obter história por ID
- ✅ Atualizar história
- ✅ Excluir história
- ✅ Gerenciar casos de teste

### Issues
- ✅ Criar nova issue
- ✅ Listar todas as issues
- ✅ Obter issue por ID
- ✅ Atualizar issue
- ✅ Excluir issue
- ✅ Associar com histórias

### Falhas
- ✅ Criar nova falha
- ✅ Listar todas as falhas
- ✅ Obter falha por ID
- ✅ Atualizar falha
- ✅ Excluir falha
- ✅ Análise de causa raiz

## 🔧 Configuração

### Pré-requisitos
- .NET 8 SDK
- PostgreSQL 12+

### Configuração do Banco de Dados

1. **Instalar PostgreSQL** (se não estiver instalado)
2. **Criar banco de dados**:
```sql
CREATE DATABASE integrationazure;
```

3. **Configurar connection string** no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=integrationazure;Username=seu_usuario;Password=sua_senha;Port=5432"
  }
}
```

### Executar a Aplicação

1. **Restaurar dependências**:
```bash
dotnet restore
```

2. **Executar a aplicação**:
```bash
dotnet run
```

3. **Acessar a documentação**:
   - Swagger: http://localhost:5000
   - API: http://localhost:5000/api

## 📡 Endpoints da API

### Histórias de Usuário
```
GET    /api/userstories           # Listar todas
GET    /api/userstories/{id}      # Obter por ID
POST   /api/userstories           # Criar nova
PUT    /api/userstories/{id}      # Atualizar
DELETE /api/userstories/{id}      # Excluir
```

### Issues
```
GET    /api/issues                # Listar todas
GET    /api/issues/{id}           # Obter por ID
POST   /api/issues                # Criar nova
PUT    /api/issues/{id}           # Atualizar
DELETE /api/issues/{id}           # Excluir
```

### Falhas
```
GET    /api/failures              # Listar todas
GET    /api/failures/{id}         # Obter por ID
POST   /api/failures              # Criar nova
PUT    /api/failures/{id}         # Atualizar
DELETE /api/failures/{id}         # Excluir
```

### Health Check
```
GET    /health                    # Status da API
```

## 📋 Modelos de Dados

### Criar História de Usuário
```json
{
  "demandNumber": "DEM-001",
  "title": "Título da história",
  "acceptanceCriteria": "Critérios de aceite detalhados",
  "description": "Descrição opcional",
  "priority": "Medium",
  "testCases": [
    {
      "description": "Caso de teste 1",
      "orderIndex": 1
    }
  ]
}
```

### Criar Issue
```json
{
  "issueNumber": "ISS-001",
  "title": "Título da issue",
  "description": "Descrição detalhada",
  "type": "Bug",
  "priority": "High",
  "assignedTo": "usuario@exemplo.com",
  "environment": "Produção",
  "stepsToReproduce": "Passos para reproduzir",
  "expectedResult": "Resultado esperado",
  "actualResult": "Resultado atual",
  "userStoryId": "uuid-opcional"
}
```

### Criar Falha
```json
{
  "failureNumber": "FLH-001",
  "title": "Título da falha",
  "description": "Descrição detalhada",
  "severity": "Critical",
  "occurredAt": "2025-01-26T10:00:00Z",
  "reportedBy": "usuario@exemplo.com",
  "environment": "Produção",
  "systemsAffected": "Sistema de pagamentos",
  "impactDescription": "Impacto nos usuários",
  "estimatedImpactCost": 5000.00
}
```

## 🔒 Validações

A API implementa validações robustas usando **FluentValidation**:

- **Campos obrigatórios** verificados
- **Formatos específicos** para números de demanda/issue/falha
- **Tamanhos mínimos e máximos** para textos
- **Valores de enum** validados
- **Relacionamentos** verificados

## 🌐 CORS

Configurado para aceitar requests do frontend React:
- `http://localhost:3000` (Create React App)
- `http://localhost:5173` (Vite)

## 🚦 Status Codes

A API segue os padrões HTTP:
- `200` - Sucesso
- `201` - Criado com sucesso
- `400` - Dados inválidos
- `404` - Não encontrado
- `500` - Erro interno

## 📖 Documentação

A documentação completa está disponível via **Swagger** quando a aplicação está rodando em modo desenvolvimento.

## 🧪 Próximos Passos

- [ ] Implementar autenticação JWT
- [ ] Adicionar testes unitários
- [ ] Implementar upload de anexos
- [ ] Adicionar logs estruturados
- [ ] Implementar cache Redis
- [ ] Adicionar métricas e monitoring
- [ ] Dockerizar a aplicação

## 📝 Observações

- A aplicação cria automaticamente as tabelas no banco em modo desenvolvimento
- Para produção, use migrações do Entity Framework
- Todos os endpoints retornam respostas padronizadas com `ApiResponseDto`
- Suporte a CORS configurado para desenvolvimento local
