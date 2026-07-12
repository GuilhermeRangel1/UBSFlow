# Fluxo: profissionais em memoria

Este passo cria o cadastro e a listagem de profissionais da UBS.

Assim como pacientes, ainda usamos uma lista em memoria. O objetivo e entender o fluxo antes de colocar banco de dados.

## Arquivos principais

- `src/UBSFlow.Dominio/Profissionais/Profissional.cs`: representa um profissional.
- `src/UBSFlow.Dominio/Profissionais/PapelProfissional.cs`: define os papeis possiveis.
- `src/UBSFlow.Aplicacao/Profissionais/ProfissionalService.cs`: contem as regras de cadastro e listagem.
- `src/UBSFlow.Infraestrutura/Profissionais/ProfissionalRepositorioEmMemoria.cs`: guarda profissionais em uma lista.
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
  "registroProfissional": "CRM12345"
}
```

## Regras iniciais

- nome e obrigatorio;
- papel profissional precisa ser valido;
- medico e enfermeiro precisam ter registro profissional;
- registro profissional nao pode ser duplicado.
