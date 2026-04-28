# FCG Platform
# **Descrição do projeto**
- Plataforma backend desenvolvida em .NET para gerenciamento de usuários e jogos, com foco em boas práticas como Clean Architecture, validação de dados, logging e operações seguras via API REST.
## **Regras de Negócio**

De acordo com os requisitos do desafio, o sistema implementa as seguintes regras:

### **Usuário**
- O usuário deve possuir: Nome, E-mail e Senha.

### **Validação de E-mail**
- O e-mail deve seguir um formato válido (ex: usuario@email.com)

### **Validação de Senha**
- A senha deve conter:
  - No mínimo 8 caracteres
  - Pelo menos uma letra
  - Pelo menos um número
  - Pelo menos um caractere especial

### **Cadastro de Usuário**
- Não é permitido cadastrar usuários com dados inválidos

### **Biblioteca de Jogos**
- O usuário pode possuir uma lista de jogos associados
---

## **Observações**
- As validações são implementadas utilizando FluentValidation
- Os erros são tratados e retornados de forma padronizada via Result<T>
---

# **Solução**
- API REST desenvolvida em .NET 8.0, utilizando Entity Framework Core e os padrões Unit of Work e Repository, com suporte a validações, tratamento de erros, logging e documentação via Swagger.

---

## **Tecnologias**
- .NET 8.0
- Entity Framework Core
- SQL Server

## **Ferramentas**
- Visual Studio 2022
- SQL Server Management Studio (SSMS)
- Git / Git Bash
---
## **Recursos do Projeto**
- **Serilog**: Para geração e gerenciamento de logs.
- **FluentValidator**: Para validação de dados e regras de negócios.
- **Entity Framework (ORM)**: Para mapeamento e interação com o banco de dados.
- **Unit of Work**: Padrão de design para gerenciar transações e persistência de dados de forma coesa.
- **Migrations**: Gerenciamento de alterações no banco de dados.
- **Xunit**: Para criação de testes unitários. 3A's (Arrange, Act, Assert).
- **BDD com Reqnroll**: Testes de comportamento baseados em cenários (Gherkin).
- **FluentAssertions**: Melhor legibilidade nas validações.
- **ASP.NET Core Identity**: Implementação de autenticação e autorização baseada em identidade, com gerenciamento de usuários, roles e controle de acesso seguro à aplicação.
---
## **Como Executar o Projeto**
### **1. Configuração Inicial do Banco de Dados**
1. Faça o clone do projeto.
2. Verifique se a pasta `Migrations` no projeto está vazia. Caso contrário, delete todos os arquivos dessa pasta.   
3. Execute os seguintes comandos no **Package Manager Console**:
   - Certifique-se de selecionar o projeto relacionado ao banco de dados no menu "Default project".
   - Execute:
     ```bash
     add-migration PrimeiraMigracao
     update-database
     ```
   - Isso criará e configurará o banco de dados no Microsoft SQL Server.
---
### **2. Executando o Projeto**
1. Abra o projeto no Visual Studio 2022.
2. Configure o projeto principal para execução:
   - Clique com o botão direito no projeto **FCG.Platform** e selecione `Set as Startup Project`.
3. Clique no botão **HTTPS** no menu superior para iniciar a aplicação.

### **3. Banco de Dados**
- **Centralização de Exceções:**  
  Implementada a classe `ExceptionMiddleware` para unificar o tratamento de erros no sistema.
- **Alterações Realizadas:**  
  Ajustadas as classes `Program` e `RepositoryUoW` para integrar o middleware.
- **Mensagens de Erro:**  
  - Se o banco de dados não existir, os endpoints retornam:  
    ```text
    The database is currently unavailable. Please try again later.
    ```
  - Para erros inesperados na criação do banco, é exibido:  
    ```text
    An unexpected error occurred. Please contact support if the problem persists.
    ```
---
### **4. Configuração do Log**
- O sistema gera logs diários com informações sobre os processos executados no projeto.
- O log será salvo no diretório:  
  `C://Users//User//Downloads//FCG-Platform-logs`.  
  **Nota**: É necessário criar a pasta manualmente nesse caminho ou alterar o diretório no código, caso deseje personalizá-lo.

  **Formato do arquivo de log criado**:
- Arquivo diário com informações estruturadas.
---
### **5. Finalização**
- Após seguir as etapas anteriores, o sistema será iniciado, e uma página com a interface **Swagger** será aberta automaticamente no navegador configurado no Visual Studio. Essa página permitirá explorar e testar os endpoints da API.
---
