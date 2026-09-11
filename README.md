## 👥 Integrantes do Grupo (Turma 2TDSPF)
* **Enzo Vaz** - RM: 561702
* **Lucas Ryuji Fukuda** - RM: 562152
* **Pietro Donella Salomão** - RM: 561722

# 🐾 Clyvo Vet API - Challenge 2026

## 📌 Descrição do Projeto

A **Clyvo Vet API** é o backend responsável por sustentar a infraestrutura de medicina veterinária digital da Clyvo. Desenvolvida em **.NET 10** utilizando **Clean Architecture**, a solução unifica e gerencia a jornada de saúde do pet, integrando tutores, clínicas, veterinários e históricos clínicos de forma contínua e preventiva.

---

## 🚀 Novidades da Sprint 3: Monitoramento, Observabilidade e Testes

Nesta sprint, a arquitetura da aplicação foi evoluída para incluir resiliência e qualidade de código, garantindo as melhores práticas de mercado:

* **Monitoramento (Health Checks):** Implementação de checagem de saúde da conexão com o banco de dados Oracle e validação de disponibilidade de serviços externos.
* **Observabilidade (Serilog & OpenTelemetry):** Configuração de logging estruturado. A saída dos logs ocorre tanto no console quanto em arquivos físicos diários (pasta `/logs`). Adicionalmente, foi integrado o OpenTelemetry para tracing distribuído e extração de métricas de desempenho.
* **Testes Automatizados:** Criação de projetos de testes separados por camadas (`UnitTests` e `IntegrationTests`). A validação das regras de negócio e endpoints foi construída seguindo rigorosamente o padrão AAA (Arrange, Act, Assert), utilizando xUnit, WebApplicationFactory e Moq para simulação de dependências.

---

# 🛠️ Instruções de Instalação e Execução

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) instalado
- Acesso a um banco de dados Oracle
- IDE recomendada: JetBrains Rider 

---

## 🚀 Passo a Passo

### 1. Clone o repositório

```bash
git clone https://github.com/EnzoVazz/ClyvoVet
cd ClyvoVet
```

### 2. Configure a String de Conexão

Navegue até o projeto da API, abra o arquivo `appsettings.json` e substitua os valores genéricos da chave `OracleConnection` com os dados do seu servidor Oracle:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "Data Source=SEU_SERVIDOR_ORACLE;User Id=SEU_USUARIO;Password=SUA_SENHA;"
  }
}
```

### 3. Restaure os pacotes e execute a aplicação

No terminal, na raiz da solução, execute:

```bash
dotnet restore
dotnet run --project ClyvoVet.Api
```

### 4. Acesse o Swagger

Abra o navegador e acesse a documentação interativa da API:

👉 `http://localhost:5134/swagger`

### 5. Como Executar os Testes Automatizados 🧪

A aplicação conta com cobertura de testes unitários (camadas internas) e testes de integração (camada HTTP). Para rodar toda a bateria de testes, abra o terminal na raiz da solução do projeto e execute:

```bash
dotnet test
```

---

# 🛣️ Documentação das Rotas (Endpoints)

## 🩺 Monitoramento / Health Check (`/health`)

**Métodos:** `GET`

**Descrição:**  
Retorna o status de saúde da API, verificando a conectividade com o banco de dados e serviços externos. O retorno é um JSON estruturado indicando o status geral (`Healthy`, `Degraded` ou `Unhealthy`). Os logs de execução podem ser acompanhados na pasta `/logs`.

---

## 🐾 Pets (`/api/animais`)

**Métodos:** `GET`, `POST`, `PUT`, `DELETE`

**Descrição:**  
Gerenciamento de animais (busca por espécie, ID do tutor, etc).

---

## 👤 Tutores (`/api/tutores`)

**Métodos:** `GET`, `POST`, `PUT`, `DELETE`

---

## 🏥 Clínicas (`/api/clinicas`)

**Métodos:** `GET`, `POST`, `PUT`, `DELETE`

**Descrição:**  
Gerenciamento de clínicas parceiras (inclui validação de CNPJ único).

---

## 🩺 Veterinários (`/api/veterinarios`)

**Métodos:** `GET`, `POST`, `PUT`, `DELETE`

**Descrição:**  
Cadastro de profissionais médicos (busca e validação por CRMV).

---

## 📋 Prontuários (`/api/prontuarios`)

**Métodos:** `GET`, `POST`, `PUT`, `DELETE`

**Descrição:**  
Histórico clínico (busca avançada por palavras-chave no diagnóstico).

---

## 📅 Consultas (`/api/consultas`)

**Métodos:** `GET`, `POST`, `PUT`, `DELETE`

**Descrição:**  
Agendamentos (filtro por status da consulta e histórico do pet).

---

# 📌 Nota sobre Status Codes

A API segue o padrão RESTful, retornando os seguintes códigos:

| Status Code | Descrição |
|---|---|
| `200 OK` | Sucesso |
| `201 Created` | Cadastro realizado com sucesso |
| `204 No Content` | Atualização ou exclusão realizada |
| `400 Bad Request` | Erros de validação |
| `404 Not Found` | Recurso não encontrado |