# UBSFlow

API backend para gestao do fluxo de atendimento em uma UBS ou clinica, cobrindo cadastro de pacientes, profissionais, agenda, triagem, fila de atendimento, permissoes por perfil e historico clinico-operacional.

O diferencial do projeto nao e apenas cadastrar pacientes. A proposta e demonstrar entendimento do fluxo real de atendimento: chegada, check-in, triagem, prioridade, consulta, fechamento, historico, relatorios e auditoria.

## Objetivos

- Modelar o fluxo completo de atendimento de uma UBS/clinica.
- Criar uma API REST com regras de negocio relevantes para backend.
- Aplicar autenticacao, autorizacao por perfil, auditoria e historico.
- Usar uma stack profissional com .NET, PostgreSQL, Entity Framework Core, Docker, testes e documentacao OpenAPI.
- Evoluir o projeto em commits pequenos, lineares e bem organizados.

## Papeis

- `ADMIN`: gerencia unidade, profissionais e permissoes.
- `RECEPCIONISTA`: cadastra pacientes e agenda consultas.
- `ENFERMEIRO`: realiza triagem.
- `MEDICO`: registra atendimento e conduta.
- `GESTOR`: visualiza relatorios e indicadores.

## Modulos

### Pacientes

Cadastro de pacientes com CPF/CNS, contato, endereco, data de nascimento, condicoes pre-existentes e historico de atendimentos.

### Profissionais

Cadastro de medicos, enfermeiros e recepcionistas, incluindo especialidade, CRM/COREN quando aplicavel e disponibilidade semanal.

### Agenda

Criacao de horarios, marcacao, remarcacao, cancelamento e acompanhamento do status da consulta.

### Fila de Atendimento

Fluxo de chegada do paciente, check-in, entrada na fila, triagem e encaminhamento para atendimento medico.

### Triagem

Registro de sinais vitais, sintomas, classificacao de risco e observacoes. A triagem tambem deve calcular prioridade automaticamente com base em dados como idade, febre, pressao, sintomas e classificacao informada.

### Atendimento

Registro de queixa, hipotese ou diagnostico, conduta, prescricao simples, encaminhamento e retorno.

### Relatorios

Indicadores como atendimentos por periodo, tempo medio de espera, casos por classificacao de risco, produtividade por profissional, faltas e cancelamentos.

### Auditoria

Historico de alteracoes importantes, como alteracao de paciente, cancelamento de consulta e fechamento de atendimento.

## Stack

- C# / .NET 8
- ASP.NET Core Web API
- PostgreSQL
- Entity Framework Core
- JWT + RBAC
- Swagger/OpenAPI
- Docker Compose
- xUnit
- Serilog
- GitHub Actions
- React + Vite no frontend

## Frontend

O projeto possui uma interface React em `frontend/`, mantida separada do backend para ser facil de evoluir ou substituir.

Para rodar:

```bash
cd frontend
npm install
npm run dev
```

O frontend abre em `http://localhost:5173` e encaminha chamadas de `/api` para a API em `http://localhost:5000`.

Com Docker Compose, o frontend tambem sobe junto com a API e o PostgreSQL:

```bash
docker compose up -d --build
```

Depois acesse:

```text
http://localhost:5173
```

## Proximos Incrementos Tecnicos

- FluentValidation para centralizar validacoes.
- Hash de senha mais forte com salt por usuario.
- Testes de integracao com PostgreSQL.
- Aplicacao automatica de migrations em ambiente de desenvolvimento.

## CI

O projeto possui workflow de GitHub Actions em `.github/workflows/dotnet-ci.yml` para executar:

- `dotnet restore`
- `dotnet build`
- `dotnet test`

## Logs

A API usa Serilog para logs estruturados no console e logs automaticos de requisicoes HTTP.

## Como Rodar Localmente

Restaure os pacotes:

```bash
dotnet restore
```

Suba o PostgreSQL:

```bash
docker compose up -d
```

Para subir frontend, API e PostgreSQL juntos via Docker:

```bash
docker compose up -d --build
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

Swagger:

```text
http://localhost:5000/swagger
```

## Regras de Negocio

- Nao permitir dois agendamentos no mesmo horario para o mesmo profissional.
- Nao permitir atendimento medico sem triagem, exceto em caso de emergencia.
- Classificar prioridade automaticamente na triagem.
- Impedir que recepcionistas visualizem dados clinicos sensiveis.
- Registrar auditoria em alteracoes criticas.
- Calcular tempo de espera entre check-in, triagem e atendimento.
- Exigir motivo para cancelamento de consulta.
- Manter historico imutavel de atendimentos finalizados.
- Aplicar paginacao e filtros nas listagens.
- Permitir relatorios por periodo, unidade e profissional.

## Endpoints

### Autenticacao

- `POST /auth/login`

### Pacientes

- `GET /pacientes`
- `POST /pacientes`
- `GET /pacientes/{id}`
- `GET /pacientes/{id}/historico`

### Profissionais

- `GET /profissionais`
- `POST /profissionais`
- `GET /profissionais/{id}`

### Agenda

- `GET /agendamentos`
- `POST /agendamentos`
- `GET /agendamentos/{id}`
- `PATCH /agendamentos/{id}/remarcar`
- `PATCH /agendamentos/{id}/cancelar`

### Fila

- `POST /fila/check-ins`
- `GET /fila/hoje`
- `GET /fila`

### Triagem

- `POST /triagens`
- `GET /triagens/{id}`

### Atendimento

- `POST /atendimentos`
- `GET /atendimentos/{id}`
- `PATCH /atendimentos/{id}/finalizar`

### Relatorios

- `GET /relatorios/atendimentos`
- `GET /relatorios/classificacoes-risco`
- `GET /relatorios/cancelamentos`

### Auditoria

- `GET /auditoria`

## Usuarios de Demonstracao

| Usuario | Senha | Papel |
| --- | --- | --- |
| admin | admin123 | ADMIN |
| recepcao | recepcao123 | RECEPCIONISTA |
| enfermagem | enfermagem123 | ENFERMEIRO |
| medico | medico123 | MEDICO |
| gestao | gestao123 | GESTOR |

## Roadmap

### MVP 1

- Pacientes
- Profissionais
- Autenticacao
- Papeis e autorizacao
- Agenda
- Regra contra conflito de horario

### MVP 2

- Check-in
- Fila de atendimento
- Triagem
- Classificacao automatica de risco

### MVP 3

- Atendimento medico
- Historico clinico-operacional
- Auditoria
- Relatorios

## Descricao para Curriculo

Desenvolvi uma API REST em C#/.NET para gestao do fluxo de atendimento de clinicas/UBS, com agenda, fila, triagem, historico de pacientes, RBAC, auditoria, PostgreSQL, Entity Framework Core, Docker, testes automatizados e documentacao OpenAPI.
