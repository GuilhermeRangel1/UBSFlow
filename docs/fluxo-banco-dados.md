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

Dados locais:

```text
Host: localhost
Porta: 5432
Database: ubsflow
Usuario: ubsflow
Senha: ubsflow
```

## Importante

Nesta etapa, o `DbContext` ja existe, mas os endpoints ainda usam os repositorios em memoria.

O proximo passo e trocar os repositorios por implementacoes com EF Core, um modulo por vez.

## Modulos usando EF Core

- Pacientes.
- Profissionais.
