# Fluxo de auditoria

A auditoria registra acoes importantes do sistema para responder perguntas como:

- quem alterou algo importante;
- quando a acao aconteceu;
- qual entidade foi afetada;
- qual foi o motivo ou resumo da alteracao.

No MVP atual ainda nao existe autenticacao real. Por isso, os logs sao registrados com o usuario `sistema`.

## Acoes registradas agora

- cadastro de paciente;
- cancelamento de agendamento;
- realizacao de triagem;
- finalizacao de atendimento.

## Endpoint

```http
GET /auditoria
```

Retorna os logs mais recentes primeiro.

Exemplo de resposta:

```json
[
  {
    "id": "b8c7a9d1-1b42-4c4f-8af8-9fbf05a34e47",
    "acao": "AgendamentoCancelado",
    "entidade": "Agendamento",
    "entidadeId": "8c0db508-6252-42e9-aa84-9cfc64ed5473",
    "usuario": "sistema",
    "descricao": "Agendamento cancelado. Motivo: Paciente solicitou cancelamento.",
    "registradoEm": "2026-07-12T12:30:00Z"
  }
]
```
