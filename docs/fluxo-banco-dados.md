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

## Aplicar migrations

```bash
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/UBSFlow.Infraestrutura --startup-project src/UBSFlow.Api --context UbsFlowDbContext
```

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
