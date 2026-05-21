# 🐾 Clyvo Vet API - Challenge 2026

## 📌 Descrição do Projeto

A **Clyvo Vet API** é o backend responsável por sustentar a infraestrutura de medicina veterinária digital da Clyvo. Desenvolvida em **.NET 10** utilizando **Clean Architecture**, a solução unifica e gerencia a jornada de saúde do pet, integrando tutores, clínicas, veterinários e históricos clínicos de forma contínua e preventiva.

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

---

# 🛣️ Documentação das Rotas (Endpoints)

## 🐾 Pets (`/api/animais`)

**Métodos:** `GET`, `POST`, `PUT`, `DELETE`

**Descrição:**  
Gerenciamento de animais (busca por espécie, ID do tutor, etc).

---

## 👤 Tutores (`/api/tutores`)

**Métodos:** `GET`, `POST`, `PUT`, `DELETE`

**Descrição:**  
Cadastro de donos de pets (inclui validação de CPF único).

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