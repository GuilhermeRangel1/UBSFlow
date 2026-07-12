# Primeiro fluxo: pacientes

Este passo cria a primeira funcionalidade real da API: cadastrar, listar e buscar pacientes.

Na versao inicial, pacientes ficavam em memoria para facilitar o aprendizado. Agora o modulo usa PostgreSQL com Entity Framework Core.

## Arquivos principais

- `src/UBSFlow.Dominio/Pacientes/Paciente.cs`: representa um paciente dentro do sistema.
- `src/UBSFlow.Aplicacao/Pacientes/PacienteService.cs`: contem a regra de aplicacao para criar e consultar pacientes.
- `src/UBSFlow.Aplicacao/Pacientes/IPacienteRepositorio.cs`: define quais operacoes um repositorio de pacientes precisa ter.
- `src/UBSFlow.Infraestrutura/Pacientes/PacienteRepositorioEf.cs`: implementa o repositorio usando Entity Framework Core.
- `src/UBSFlow.Api/Controllers/PacientesController.cs`: cria os endpoints HTTP.

## Fluxo de cadastro

```text
POST /pacientes
        |
        v
PacientesController
        |
        v
PacienteService
        |
        v
PacienteRepositorioEf
        |
        v
PostgreSQL
```

## Endpoints criados

Ao rodar localmente com `dotnet run --project src/UBSFlow.Api`, a API fica disponivel em:

```text
http://localhost:5000
```

O Swagger fica em:

```text
http://localhost:5000/swagger
```

### Listar pacientes

```http
GET /pacientes
```

Tambem e possivel filtrar por nome ou CPF:

```http
GET /pacientes?nome=maria
GET /pacientes?cpf=12345678901
```

A listagem tambem aceita paginacao:

```http
GET /pacientes?pagina=1&tamanhoPagina=10
```

A resposta de listagem vem com metadados:

```json
{
  "itens": [],
  "pagina": 1,
  "tamanhoPagina": 10,
  "totalItens": 0,
  "totalPaginas": 0
}
```

### Buscar paciente por id

```http
GET /pacientes/{id}
```

### Criar paciente

```http
POST /pacientes
Content-Type: application/json

{
  "nome": "Maria Silva",
  "cpf": "12345678901",
  "dataNascimento": "1990-05-12",
  "telefone": "11999990000",
  "cns": null
}
```

## Primeira regra de negocio

Nao pode cadastrar dois pacientes com o mesmo CPF.

Se tentar cadastrar CPF duplicado, a API responde conflito:

```http
409 Conflict
```

## Validacoes iniciais

O cadastro tambem valida:

- nome obrigatorio;
- CPF obrigatorio com exatamente 11 digitos;
- data de nascimento nao pode estar no futuro;
- telefone obrigatorio.

Quando algum dado invalido e enviado, a API responde:

```http
400 Bad Request
```
