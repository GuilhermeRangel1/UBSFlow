# Fluxo: atendimento medico

Este passo inicia o MVP 3 com o registro do atendimento medico.

Atendimentos usam PostgreSQL com Entity Framework Core. Ao finalizar, o atendimento e o status da fila sao persistidos.

## Endpoints criados

### Criar atendimento

```http
POST /atendimentos
Content-Type: application/json

{
  "checkInId": "00000000-0000-0000-0000-000000000000",
  "queixa": "Dor no corpo e febre",
  "hipoteseDiagnostica": "Sindome viral",
  "conduta": "Orientacao, hidratacao e observacao",
  "prescricao": "Dipirona se febre",
  "encaminhamento": null
}
```

### Buscar atendimento por id

```http
GET /atendimentos/{id}
```

### Finalizar atendimento

```http
PATCH /atendimentos/{id}/finalizar
Content-Type: application/json

{
  "finalizadoEm": "2026-07-12T10:30:00-03:00"
}
```

## Regras iniciais

- check-in e obrigatorio;
- check-in precisa existir;
- check-in precisa estar `AguardandoAtendimento`;
- nao permite iniciar atendimento duas vezes para o mesmo check-in;
- queixa, hipotese diagnostica e conduta sao obrigatorias;
- ao iniciar atendimento, a fila muda para `EmAtendimento`;
- nao permite finalizar atendimento inexistente;
- nao permite finalizar duas vezes;
- horario de finalizacao nao pode ser anterior ao inicio;
- ao finalizar atendimento, a fila muda para `Finalizado`.
