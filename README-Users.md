# Sistema de Gerenciamento de Usuários - IntegrationAzure

## 📋 Resumo da Implementação

Foi implementado um sistema completo de gerenciamento de usuários no projeto IntegrationAzure, incluindo tanto o back-end (.NET) quanto o front-end (React).

## 🚀 Funcionalidades Implementadas

### Back-end (.NET Core)

#### 📦 Entidades Criadas
- **Profile**: Representa os perfis de usuário (Administrador, Desenvolvedor, Usuário)
- **User**: Representa os usuários do sistema com informações pessoais e relacionamento com perfil

#### 🗃️ Estrutura do Banco
- Tabela `profiles` com os perfis padrão
- Tabela `users` com relacionamento com profiles
- Índices únicos para email, nickname e nome do perfil
- Soft delete implementado (campo IsActive)

#### 🛠️ Repositórios
- `IProfileRepository` / `ProfileRepository`: Operações específicas para perfis
- `IUserRepository` / `UserRepository`: Operações específicas para usuários
- Métodos incluem busca por email, nickname, perfil, etc.

#### 🎯 Serviços
- `ProfileService`: Regras de negócio para perfis
- `UserService`: Regras de negócio para usuários com hash de senha (SHA256)

#### 🌐 Controllers
- `ProfilesController`: CRUD completo para perfis
- `UsersController`: CRUD completo para usuários + alteração de senha

#### ✅ Validações
- FluentValidation para todos os DTOs
- Validação de email único, nickname único, nome de perfil único
- Validação de senha forte (maiúscula, minúscula, número, mín. 6 caracteres)

#### 🔄 Migration e Seed
- Migration `AddUserAndProfileEntities` criada
- DataSeeder para popular os 3 perfis padrão automaticamente

### Front-end (React + Material-UI)

#### 📄 Páginas
- **UsersPage**: Página principal de gerenciamento de usuários

#### 🧩 Componentes
- **UserModal**: Modal para criar/editar usuários
- **ChangePasswordModal**: Modal específico para alteração de senhas

#### 🔐 Controle de Permissões

##### Administrador
- ✅ Pode criar usuários
- ✅ Pode editar qualquer usuário
- ✅ Pode alterar senha de qualquer usuário
- ✅ Pode deletar usuários (exceto ele mesmo)
- ✅ Pode atribuir perfis: Administrador e Usuário (não pode criar Desenvolvedor)

##### Desenvolvedor
- ✅ Pode criar usuários
- ✅ Pode editar qualquer usuário
- ✅ Pode alterar senha de qualquer usuário
- ✅ Pode deletar usuários (exceto ele mesmo)
- ✅ Pode atribuir qualquer perfil (Administrador, Desenvolvedor, Usuário)

##### Usuário
- ❌ Não pode criar usuários
- ✅ Pode editar apenas seu próprio perfil
- ✅ Pode alterar apenas sua própria senha
- ❌ Não pode deletar usuários
- ✅ Só vê seu próprio perfil na lista
- ✅ Só pode manter o perfil "Usuário"

#### 🎨 Interface
- Design seguindo o padrão das outras páginas
- Filtros por perfil e busca por nome/nickname/email
- Tabela responsiva com avatares e chips coloridos para perfis
- Estatísticas de usuários por perfil
- Modais bem estruturados com validação visual

#### 🔧 Serviços
- `userService`: Comunicação com API de usuários
- `profileService`: Comunicação com API de perfis

## 🧪 Como Testar

### 1. Executar o Back-end
```bash
cd Api/IntegrationAzure.Api
dotnet run
```
A API estará disponível em: http://localhost:5066

### 2. Executar o Front-end
```bash
cd Aplication
npm run dev
```
O front-end estará disponível em: http://localhost:5173

### 3. Credenciais de Teste

#### Administrador
- **Email**: admin@admin
- **Senha**: 123

#### Desenvolvedor
- **Email**: dev@dev
- **Senha**: 123

#### Usuário
- **Email**: user@user
- **Senha**: 123

### 4. Testando Funcionalidades

1. **Login** com uma das credenciais acima
2. **Navegar** para "Usuários" no menu
3. **Observar** as diferenças de interface baseadas no perfil logado
4. **Testar** criação de usuários (se tiver permissão)
5. **Testar** edição de usuários (respeitando as permissões)
6. **Testar** alteração de senhas
7. **Testar** filtros e busca

## 📊 Endpoints da API

### Perfis
- `GET /api/profiles` - Listar todos os perfis
- `GET /api/profiles/active` - Listar perfis ativos
- `GET /api/profiles/{id}` - Obter perfil por ID
- `POST /api/profiles` - Criar novo perfil
- `PUT /api/profiles/{id}` - Atualizar perfil
- `DELETE /api/profiles/{id}` - Remover perfil

### Usuários
- `GET /api/users` - Listar todos os usuários
- `GET /api/users/{id}` - Obter usuário por ID
- `GET /api/users/profile/{profileId}` - Usuários por perfil
- `POST /api/users` - Criar novo usuário
- `PUT /api/users/{id}` - Atualizar usuário
- `DELETE /api/users/{id}` - Remover usuário
- `PATCH /api/users/{id}/change-password` - Alterar senha

## 🔒 Segurança

- Senhas são hasheadas com SHA256
- Validação de senha forte obrigatória
- Soft delete para manter histórico
- Controle de permissões no front-end
- Validações tanto no front quanto no back-end

## 📁 Arquivos Principais Criados/Modificados

### Back-end
- `Domain/Entities/Profile.cs`
- `Domain/Entities/User.cs`
- `Domain/Interfaces/IProfileRepository.cs`
- `Domain/Interfaces/IUserRepository.cs`
- `Infrastructure/Repositories/ProfileRepository.cs`
- `Infrastructure/Repositories/UserRepository.cs`
- `Application/Services/ProfileService.cs`
- `Application/Services/UserService.cs`
- `Application/DTOs/ProfileDtos.cs`
- `Application/DTOs/UserDtos.cs`
- `Application/Validators/ProfileValidators.cs`
- `Application/Validators/UserValidators.cs`
- `Controllers/ProfilesController.cs`
- `Controllers/UsersController.cs`
- `Infrastructure/Data/DataSeeder.cs`
- `Migrations/20250729160031_AddUserAndProfileEntities.cs`

### Front-end
- `services/userService.js`
- `components/UserModal.jsx`
- `components/ChangePasswordModal.jsx`
- `pages/UsersPage.jsx`
- Modificações em `App.jsx`, `services/index.js`, `services/authService.js`

## 🎯 Próximos Passos Sugeridos

1. Implementar autenticação real com JWT
2. Adicionar upload de imagens de perfil
3. Implementar logs de auditoria para ações de usuários
4. Adicionar paginação na lista de usuários
5. Implementar busca avançada com mais filtros
6. Adicionar configurações de perfil mais granulares
7. Implementar 2FA (autenticação de dois fatores)
