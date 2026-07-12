# Fluxo: agenda

Este passo cria a primeira versao da agenda.

A agenda usa PostgreSQL com Entity Framework Core para persistir criacao, remarcacao e cancelamento.

## Arquivos principais

- `src/UBSFlow.Dominio/Agenda/Agendamento.cs`: representa um horario marcado.
- `src/UBSFlow.Dominio/Agenda/StatusAgendamento.cs`: define os status possiveis.
- `src/UBSFlow.Aplicacao/Agenda/AgendamentoService.cs`: contem as regras da agenda.
- `src/UBSFlow.Infraestrutura/Agenda/AgendamentoRepositorioEf.cs`: persiste agendamentos com Entity Framework Core.
- `src/UBSFlow.Api/Controllers/AgendamentosController.cs`: cria os endpoints HTTP.

## Endpoints criados

### Listar agendamentos

```http
GET /agendamentos
GET /agendamentos?profissionalId={id}
GET /agendamentos?pacienteId={id}
GET /agendamentos?data=2026-07-12
GET /agendamentos?pagina=1&tamanhoPagina=10
```

### Buscar agendamento por id

```http
GET /agendamentos/{id}
```

### Criar agendamento

```http
POST /agendamentos
Content-Type: application/json

{
  "pacienteId": "00000000-0000-0000-0000-000000000000",
  "profissionalId": "00000000-0000-0000-0000-000000000000",
  "inicio": "2026-07-12T09:00:00-03:00",
  "fim": "2026-07-12T09:30:00-03:00"
}
```

### Remarcar agendamento

```http
PATCH /agendamentos/{id}/remarcar
Content-Type: application/json

{
  "inicio": "2026-07-12T10:00:00-03:00",
  "fim": "2026-07-12T10:30:00-03:00"
}
```

### Cancelar agendamento

```http
PATCH /agendamentos/{id}/cancelar
Content-Type: application/json

{
  "motivo": "Paciente solicitou cancelamento."
}
```

## Regras iniciais

- paciente e obrigatorio;
- profissional e obrigatorio;
- paciente informado precisa existir;
- profissional informado precisa existir;
- horario final precisa ser maior que horario inicial;
- nao permite conflito de horario para o mesmo profissional;
- agendamento cancelado nao pode ser remarcado;
- cancelamento exige motivo obrigatorio.
