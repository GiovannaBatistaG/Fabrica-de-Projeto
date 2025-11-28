# 📊 Análise Completa do Projeto

## 🎯 Visão Geral

Este é um projeto que tem como proposito criar, gerenciar e validar propostas!

Nossa ideia foi criar um gestor de proposta, onde as informações de um cliente interessado podem ser vizualidas, através dela, adicionamos as informações de retorno com a proposta, geramos o pdf, também conta com a parte de validar, caso não seja aprovada pode ser gerada novamente ou fnalizada, e há também uma listagem dos clientes com o histórico das propostas e ações dentro do sistema.

Háviamos estruturado o backend para receber futuramente a integração de uma Api que conseguisse gerar aproposta e modelos padrões que poderiam ser utilizados

- **Backend**: API REST em **C# (.NET 8.0)** com Entity Framework Core e SQLite
- **Frontend**: Aplicação React com **Vite**, Material-UI e React Router
- **Arquitetura**: Clean Architecture (Domain, Application, Infrastructure, API)

---

## Estrutura do Projeto

### Backend (`/backend`)
```
backend/
├── Api/Controllers/          # Controllers REST
├── Application/              # Camada de aplicação (DTOs, Services, Ports)
├── Domain/                   # Entidades e interfaces (Ports)
├── Infrastructure/           # Implementações (Repositories, DbContext)
├── Migrations/               # Migrações do Entity Framework
```

### Frontend (`/crm-projeto`)
```
crm-projeto/
├── src/
│   ├── pages/                # Páginas da aplicação
│   ├── services/             # Serviços de API
│   ├── elementes/            # Componentes reutilizáveis (Layout, Header, Navbar)
│   └── assets/               # Recursos estáticos
└── public/                   # Arquivos públicos
```

---

## 🗄️ Modelo de Dados

### Entidades Principais

#### 1. **Cliente**
- `Id`, `Nome`, `Email`, `Status`
- `QuantidadeTemplates`, `PdfGerado`
- `DataCadastro`
- Relacionamento: 1-N com `Propostas`

#### 2. **Proposta**
- `Id`, `NomeCliente`, `EmailCliente`
- `DataProposta`, `StatusValidacao`
- `Valor`, `Responsavel`, `MensagemEquipe`
- `Slides` (TEXT - conteúdo gerado por IA)
- `PdfUrl` (URL do PDF gerado)
- `ClienteId` (FK para Cliente)
- `DataCriacao`

#### 3. **Modelo**
- `Id`, `Titulo`, `Descricao`
- `Plano`, `Status`
- Relacionamentos: 1-N com `EnviosFormularios` e `ProcessosModelos`

#### 4. **EnvioFormulario**
- `Id`, `NomeLead`, `EmailContato`
- `StatusEnvio`, `DadosFormularioJson`
- `IdModelo` (FK para Modelo)

#### 5. **ProcessoModelo**
- `Id`, `DescricaoProcesso`
- `IdModelo` (FK para Modelo)

#### 6. **Usuario**
- `Id`, `NomeUsuario`, `Email`
- `HashSenha`, `PerfilAcesso`
- Índices únicos em `NomeUsuario` e `Email`

---

## 🔌 API Endpoints

### Controllers Disponíveis

1. **ClientesController** (`/api/Clientes`)
   - `GET /api/Clientes` - Lista todos
   - `GET /api/Clientes/{id}` - Busca por ID
   - `POST /api/Clientes` - Cria novo
   - `PUT /api/Clientes/{id}` - Atualiza
   - `DELETE /api/Clientes/{id}` - Remove

2. **PropostasController** (`/api/Propostas`)
   - `GET /api/Propostas` - Lista todas
   - `GET /api/Propostas/{id}` - Busca por ID
   - `POST /api/Propostas` - Cria nova
   - `PUT /api/Propostas/{id}` - Atualiza
   - `DELETE /api/Propostas/{id}` - Remove

3. **Outros Controllers**:
   - `ModelosController`
   - `EnviosFormulariosController`
   - `ProcessosModelosController`
   - `UsuariosController`
   - `LeadsController`
   - `GerarPropostaController`

---

## Frontend - Páginas e Funcionalidades

### Rotas Configuradas (`App.jsx`)

1. **`/` ou `/forms`** → `FormsPage`
   - Lista de formulários recebidos (Leads)
   - DataGrid do Material-UI
   - Busca por nome do cliente
   - Navegação para detalhes do lead

2. **`/lead/:id`** → `LeadDetails`
   - Detalhes de um lead específico

3. **`/Validar`** → `ValidarPage`
   - Validação de propostas

4. **`/proposta`** → `PropostaPage`
   - Gestão de propostas

5. **`/Dashbord`** → `DashboardPage`
   - Dashboard com Kanban board
   - Cards de status (Recebidas, Para Validar, Finalizadas)
   - Drag & Drop de tarefas

6. **`/clientes`** → `ClientesPage`
   - Gestão de clientes

### Backend (`appsettings.json`)
- **Banco de Dados**: SQLite (`ProjetoApi.db`)
- **JWT**: Configurado com secret key
- **CORS**: Permitindo todas as origens (desenvolvimento)
- **Swagger**: Habilitado em desenvolvimento

### Frontend (`vite.config.js`)
- Plugin React configurado
- Variável de ambiente: `VITE_API_BASE_URL` (padrão: `http://localhost:5237`)

## Como Executar

### Backend
```bash
cd backend
dotnet restore
dotnet run
# API disponível em: https://localhost:5001 ou http://localhost:5000
# Swagger: http://localhost:5000
```

### Frontend
```bash
cd crm-projeto
npm install
npm run dev
# Aplicação disponível em: http://localhost:5173
```
