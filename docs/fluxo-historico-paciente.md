# Fluxo: historico do paciente

Este passo cria uma visao consolidada do historico clinico-operacional do paciente.

## Endpoint criado

```http
GET /pacientes/{id}/historico
```

## Resposta

O historico retorna:

- agendamentos do paciente;
- triagens relacionadas ao paciente;
- atendimentos relacionados ao paciente.

## Regra inicial

- paciente precisa existir.
