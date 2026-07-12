# Fluxo de banco de dados

O projeto comecou usando repositorios em memoria para facilitar o aprendizado do fluxo.

Agora a infraestrutura ja possui:

- provider PostgreSQL para Entity Framework Core;
- `UbsFlowDbContext`;
- mapeamento inicial das entidades principais;
- `docker-compose.yml` com PostgreSQL local.

## Subir o PostgreSQL

```bash
docker compose up -d
```

Se esse comando falhar dizendo que nao conseguiu conectar ao Docker, abra o Docker Desktop e tente novamente.

## Aplicar migrations

```bash
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/UBSFlow.Infraestrutura --startup-project src/UBSFlow.Api --context UbsFlowDbContext
```

Se o comando listar a migration, mas falhar ao conectar em `localhost:5432`, o PostgreSQL ainda nao esta rodando.

Dados locais:

```text
Host: localhost
Porta: 5432
Database: ubsflow
Usuario: ubsflow
Senha: ubsflow
```

## Modulos usando EF Core

- Pacientes.
- Profissionais.
- Agendamentos.
- Check-ins/fila.
- Triagens.
- Atendimentos.
- Auditoria.
