# Fluxo: atendimento medico em memoria

Este passo inicia o MVP 3 com o registro do atendimento medico.

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

## Regras iniciais

- check-in e obrigatorio;
- check-in precisa existir;
- check-in precisa estar `AguardandoAtendimento`;
- nao permite iniciar atendimento duas vezes para o mesmo check-in;
- queixa, hipotese diagnostica e conduta sao obrigatorias;
- ao iniciar atendimento, a fila muda para `EmAtendimento`.
