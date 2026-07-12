# Fluxo: triagem em memoria

Este passo registra a triagem de um paciente que ja fez check-in.

## Endpoints criados

### Criar triagem

```http
POST /triagens
Content-Type: application/json

{
  "checkInId": "00000000-0000-0000-0000-000000000000",
  "temperatura": 37.8,
  "pressaoSistolica": 130,
  "pressaoDiastolica": 85,
  "frequenciaCardiaca": 92,
  "sintomas": "Febre e dor no corpo",
  "classificacaoRisco": "Amarelo",
  "observacoes": "Paciente relata sintomas ha dois dias"
}
```

### Buscar triagem por id

```http
GET /triagens/{id}
```

## Regras iniciais

- check-in e obrigatorio;
- check-in precisa existir;
- check-in precisa estar aguardando triagem;
- nao permite triagem duplicada para o mesmo check-in;
- sinais vitais precisam ter valores validos;
- sintomas sao obrigatorios;
- ao concluir a triagem, a fila muda para `AguardandoAtendimento`.
