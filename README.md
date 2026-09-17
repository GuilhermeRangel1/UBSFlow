# UBSFlow

UBSFlow e uma plataforma para organizar o fluxo de atendimento de uma UBS ou clinica, conectando recepcao, enfermagem, medicos e gestao em uma jornada unica: cadastro, agenda, chegada, triagem, atendimento, historico, relatorios e auditoria.

O objetivo do projeto nao e apenas cadastrar pacientes. A proposta e mostrar um backend com regra de negocio real, controle de acesso por perfil, rastreabilidade e uma interface funcional para operar o atendimento ponta a ponta.

## Visao Geral

- API REST em C#/.NET 8 com ASP.NET Core.
- Frontend em React + Vite integrado aos endpoints da API.
- PostgreSQL com Entity Framework Core e migrations.
- Autenticacao JWT e autorizacao por perfil.
- Swagger/OpenAPI para documentacao dos endpoints.
- Docker Compose para subir frontend, API e banco juntos.
- Testes automatizados com xUnit.
- Logs estruturados com Serilog.

## Fluxo do Sistema

1. A recepcao cadastra pacientes e marca consultas.
2. O paciente chega na unidade e faz check-in.
3. A enfermagem realiza a triagem com sinais vitais, sintomas e classificacao de risco.
4. A fila e ordenada considerando prioridade e status do atendimento.
5. O medico registra consulta, conduta, prescricao e finalizacao.
6. A gestao acompanha indicadores e a auditoria registra acoes criticas.

## Perfis de Acesso

| Perfil | Acesso principal |
| --- | --- |
| `ADMIN` | Gerencia cadastros, acessa auditoria e opera todos os modulos |
| `RECEPCIONISTA` | Cadastra pacientes, agenda consultas e registra check-ins |
| `ENFERMEIRO` | Visualiza fila e realiza triagens |
| `MEDICO` | Visualiza fila e registra atendimentos |
| `GESTOR` | Acompanha relatorios, indicadores e dados operacionais |

## Funcionalidades

### Pacientes

- Cadastro com CPF, CNS, telefone e data de nascimento.
- Busca por nome e CPF.
- Historico clinico-operacional do paciente.
- Restricao de dados conforme perfil.

### Profissionais

- Cadastro por papel profissional.
- Especialidade, registro profissional e disponibilidade semanal.
- Listagem para apoiar agendamentos e gestao da equipe.

### Agenda

- Marcacao de consulta por paciente, profissional e horario.
- Remarcacao de consultas.
- Cancelamento com motivo obrigatorio.
- Regra contra conflito de horario para o mesmo profissional.

### Fila

- Check-in de pacientes agendados.
- Fila do dia por status.
- Transicao entre chegada, triagem, atendimento e finalizacao.

### Triagem

- Registro de temperatura, pressao arterial, frequencia cardiaca, sintomas e observacoes.
- Classificacao automatica de prioridade considerando sinais e risco informado.
- Encaminhamento do paciente para atendimento medico.

### Atendimento

- Registro de queixa, hipotese diagnostica, conduta, prescricao e encaminhamento.
- Finalizacao de atendimento.
- Regra que impede consulta medica sem triagem previa.

### Relatorios e Auditoria

- Atendimentos por periodo.
- Casos por classificacao de risco.
- Cancelamentos por periodo.
- Logs de acoes importantes, como cancelamento de consulta e finalizacao de atendimento.

## Stack

- C# / .NET 8
- ASP.NET Core Web API
- PostgreSQL
- Entity Framework Core
- JWT + RBAC
- React + Vite
- Docker Compose
- xUnit
- Serilog
- Swagger/OpenAPI
- GitHub Actions

## Como Rodar Com Docker

Com o Docker Desktop aberto, rode na raiz do projeto:

```bash
docker compose up -d --build
```

Depois acesse:

```text
Frontend: http://localhost:3000
API:      http://localhost:5000
Swagger:  http://localhost:5000/swagger
```

## Como Rodar Localmente

Restaure os pacotes:

```bash
dotnet restore
```

Suba o PostgreSQL:

```bash
docker compose up -d postgres
```

Aplique as migrations:

```bash
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/UBSFlow.Infraestrutura --startup-project src/UBSFlow.Api --context UbsFlowDbContext
```

Rode a API:

```bash
dotnet run --project src/UBSFlow.Api
```

Em outro terminal, rode o frontend:

```bash
cd frontend
npm install
npm run dev
```

## Usuarios Para Teste

| Usuario | Senha | Perfil |
| --- | --- | --- |
| `admin` | `admin123` | ADMIN |
| `recepcao` | `recepcao123` | RECEPCIONISTA |
| `enfermagem` | `enfermagem123` | ENFERMEIRO |
| `medico` | `medico123` | MEDICO |
| `gestao` | `gestao123` | GESTOR |

## Endpoints Principais

```text
POST   /auth/login

GET    /pacientes
POST   /pacientes
GET    /pacientes/{id}/historico

GET    /profissionais
POST   /profissionais

GET    /agendamentos
POST   /agendamentos
PATCH  /agendamentos/{id}/remarcar
PATCH  /agendamentos/{id}/cancelar

POST   /fila/check-ins
GET    /fila/hoje

POST   /triagens
GET    /triagens/{id}

POST   /atendimentos
PATCH  /atendimentos/{id}/finalizar

GET    /relatorios/atendimentos
GET    /relatorios/classificacoes-risco
GET    /relatorios/cancelamentos
GET    /auditoria
```

## Testes

```bash
dotnet test
```

## Resumo Para Curriculo

Desenvolvi o UBSFlow, uma plataforma full stack para gestao do fluxo de atendimento de UBS/clinicas, com API REST em C#/.NET, React, PostgreSQL, Entity Framework Core, JWT/RBAC, agenda, fila, triagem com prioridade automatica, atendimento medico, auditoria, relatorios, Docker, testes automatizados e documentacao OpenAPI.
