Financeiro Pessoal

Sistema de gerenciamento financeiro pessoal desenvolvido em Blazor WebAssembly com ASP.NET Core API, permitindo cadastro de usuários, categorias, receitas e despesas, com autenticação JWT e interface responsiva.

Funcionalidades

Cadastro e login de usuários com autenticação JWT

Persistência de sessão com LocalStorage

CRUD de Categorias, Receitas e Despesas

Filtragem e pesquisa por nome, tipo e data

Modais de criação/edição com validação de campos

Mensagens de sucesso e erro via Snackbar

Controle de permissões com [Authorize]

Interface responsiva usando MudBlazor

Suporte a temas claro e escuro (configurável via layout)

Tecnologias utilizadas

Front-end: Blazor WebAssembly, MudBlazor

Back-end: ASP.NET Core Web API

Autenticação: JWT (JSON Web Token)

Armazenamento local: Blazored.LocalStorage

Banco de dados: SQL Server (ou outro configurado)

Gerenciamento de pacotes: .NET 8

Estrutura do projeto
FinanceiroPessoal/
├─ API/                       # Backend - ASP.NET Core Web API
├─ Apresentacao/              # Frontend - Blazor WebAssembly
│   ├─ Pages/                 # Páginas do sistema
│   ├─ ViewModels/            # Lógica de estado (MVVM)
│   └─ Servicos/              # Serviços e integração com API
├─ Dominio/                   # Entidades e enums
├─ Aplicacao/                 # DTOs, interfaces e serviços de aplicação
└─ README.md

Configuração do projeto

Clonar o repositório:

git clone https://github.com/seu-usuario/financeiro-pessoal.git


Configurar string de conexão no appsettings.json da API:

"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=FinanceiroPessoal;Trusted_Connection=True;"
}


Configurar JWT no appsettings.json da API:

"Jwt": {
  "Key": "sua_chave_secreta_aqui",
  "Issuer": "FinanceiroPessoal",
  "Audience": "FinanceiroPessoal"
}


Executar a API:

cd API
dotnet restore
dotnet run


Executar o front-end:

cd Apresentacao
dotnet restore
dotnet run

Endpoints principais da API
Endpoint	Método	Descrição
/api/auth/login	POST	Autentica usuário e retorna JWT
/api/auth/registrar	POST	Cria um novo usuário
/api/categorias	GET	Lista todas categorias
/api/categorias	POST	Cria uma nova categoria
/api/receitas	GET	Lista todas receitas
/api/despesas	GET	Lista todas despesas

(Adicionar demais endpoints conforme implementados)

Login e cadastro de usuário

Cadastrar: Acesse /cadastro e preencha Nome, Email e Senha

Login: Acesse /login e utilize Email e Senha cadastrados

Esqueci minha senha: (Implementação futura)

Após login, o token JWT é armazenado localmente, permitindo acesso às páginas protegidas.

Exibição do usuário logado

No layout principal, o sistema exibe:

Olá, NomeUsuario!


e fornece botão para Sair.

Próximos passos / melhorias

Recuperação de senha via e-mail

Gráficos e dashboards de receitas vs despesas

Exportação de relatórios em PDF ou Excel

Aprimorar temas escuro/claro com MudBlazor
