# Fluxo: relatorios simples

Este passo adiciona relatorios operacionais usando os repositorios da aplicacao. No backend atual, esses repositorios leem dados persistidos com Entity Framework Core.

## Endpoints criados

```http
GET /relatorios/atendimentos?inicio=2026-07-01&fim=2026-07-31
GET /relatorios/classificacoes-risco?inicio=2026-07-01&fim=2026-07-31
GET /relatorios/cancelamentos?inicio=2026-07-01&fim=2026-07-31
```

## Regras iniciais

- data final nao pode ser anterior a data inicial;
- relatorio de atendimentos retorna total iniciado e total finalizado;
- relatorio de classificacoes agrupa triagens por risco;
- relatorio de cancelamentos conta agendamentos cancelados no periodo.
