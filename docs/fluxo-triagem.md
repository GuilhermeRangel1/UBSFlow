# Fluxo: triagem

Este passo registra a triagem de um paciente que ja fez check-in.

Triagens usam PostgreSQL com Entity Framework Core. Ao concluir, a classificacao de risco e a mudanca de status da fila sao persistidas.

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
- classificacao de risco e calculada automaticamente com base em sinais vitais e sintomas;
- se a classificacao informada for mais grave que a calculada, a API preserva a mais grave;
- ao concluir a triagem, a fila muda para `AguardandoAtendimento`.

## Classificacao automatica inicial

- `Vermelho`: sintomas criticos, febre a partir de 40, pressao muito alta ou frequencia cardiaca a partir de 130.
- `Laranja`: febre a partir de 39, pressao alta ou frequencia cardiaca a partir de 120.
- `Amarelo`: febre a partir de 37.8, pressao levemente alta ou frequencia cardiaca a partir de 100.
- `Verde`: sem sinais de alerta inicial.
