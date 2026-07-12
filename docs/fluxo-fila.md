# Fluxo: check-in e fila

Este passo inicia o MVP 2 com o check-in do paciente e a fila de atendimento.

Check-ins e mudancas de status da fila usam PostgreSQL com Entity Framework Core.

## Arquivos principais

- `src/UBSFlow.Dominio/Fila/CheckInAtendimento.cs`: representa a entrada do paciente na fila.
- `src/UBSFlow.Dominio/Fila/StatusFilaAtendimento.cs`: define os status da fila.
- `src/UBSFlow.Aplicacao/Fila/FilaAtendimentoService.cs`: contem as regras de check-in e listagem da fila.
- `src/UBSFlow.Infraestrutura/Fila/CheckInAtendimentoRepositorioEf.cs`: persiste check-ins com Entity Framework Core.
- `src/UBSFlow.Api/Controllers/FilaController.cs`: cria os endpoints HTTP.

## Endpoints criados

### Fazer check-in

```http
POST /fila/check-ins
Content-Type: application/json

{
  "agendamentoId": "00000000-0000-0000-0000-000000000000"
}
```

### Listar fila de hoje

```http
GET /fila/hoje
```

### Listar fila por data

```http
GET /fila?data=2026-07-12
```

## Regras iniciais

- agendamento e obrigatorio;
- agendamento informado precisa existir;
- agendamento cancelado nao pode receber check-in;
- nao permite check-in duplicado para o mesmo agendamento;
- check-in entra na fila com status `AguardandoTriagem`;
- pacientes ja triados e aguardando atendimento aparecem antes dos pacientes aguardando triagem;
- entre pacientes aguardando atendimento, a fila prioriza a maior classificacao de risco.
