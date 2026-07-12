# Fluxo de autenticacao

O backend agora possui login com JWT e regras por papel.

## Endpoint

```http
POST /auth/login
```

Exemplo:

```json
{
  "usuario": "medico",
  "senha": "medico123"
}
```

A resposta retorna um token do tipo `Bearer`. Para chamar endpoints protegidos, envie:

```http
Authorization: Bearer seu-token-aqui
```

## Usuarios de demonstracao

| Usuario | Senha | Papel |
| --- | --- | --- |
| admin | admin123 | ADMIN |
| recepcao | recepcao123 | RECEPCIONISTA |
| enfermagem | enfermagem123 | ENFERMEIRO |
| medico | medico123 | MEDICO |
| gestao | gestao123 | GESTOR |

## Regras iniciais por papel

- `ADMIN`: acessa tudo.
- `RECEPCIONISTA`: pacientes, agenda e check-in.
- `ENFERMEIRO`: fila, triagem e historico clinico.
- `MEDICO`: fila, triagem para consulta e atendimentos.
- `GESTOR`: relatorios, profissionais e dados de acompanhamento.
