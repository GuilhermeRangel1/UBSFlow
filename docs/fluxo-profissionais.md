# Fluxo: profissionais

Este passo cria o cadastro e a listagem de profissionais da UBS.

Profissionais e disponibilidades usam PostgreSQL com Entity Framework Core.

## Arquivos principais

- `src/UBSFlow.Dominio/Profissionais/Profissional.cs`: representa um profissional.
- `src/UBSFlow.Dominio/Profissionais/PapelProfissional.cs`: define os papeis possiveis.
- `src/UBSFlow.Aplicacao/Profissionais/ProfissionalService.cs`: contem as regras de cadastro e listagem.
- `src/UBSFlow.Infraestrutura/Profissionais/ProfissionalRepositorioEf.cs`: persiste profissionais com Entity Framework Core.
- `src/UBSFlow.Api/Controllers/ProfissionaisController.cs`: cria os endpoints HTTP.

## Endpoints criados

### Listar profissionais

```http
GET /profissionais
GET /profissionais?nome=ana
GET /profissionais?papel=Medico
GET /profissionais?pagina=1&tamanhoPagina=10
```

### Buscar profissional por id

```http
GET /profissionais/{id}
```

### Criar profissional

```http
POST /profissionais
Content-Type: application/json

{
  "nome": "Dra Ana",
  "papel": "Medico",
  "especialidade": "Clinica Geral",
  "registroProfissional": "CRM12345",
  "disponibilidades": [
    {
      "diaSemana": "Monday",
      "horaInicio": "08:00:00",
      "horaFim": "12:00:00"
    }
  ]
}
```

## Regras iniciais

- nome e obrigatorio;
- papel profissional precisa ser valido;
- medico e enfermeiro precisam ter registro profissional;
- registro profissional nao pode ser duplicado;
- horario final da disponibilidade precisa ser maior que o horario inicial.
