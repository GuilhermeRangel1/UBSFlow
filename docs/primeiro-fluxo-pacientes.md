# Primeiro fluxo: pacientes em memoria

Este passo cria a primeira funcionalidade real da API: cadastrar, listar e buscar pacientes.

Por enquanto ainda nao existe banco de dados. Os pacientes ficam guardados em uma lista em memoria, parecida com uma lista de registros em C. Quando a API for reiniciada, os dados somem. Isso e intencional para facilitar o aprendizado antes de entrar em PostgreSQL e Entity Framework.

## Arquivos principais

- `src/UBSFlow.Dominio/Pacientes/Paciente.cs`: representa um paciente dentro do sistema.
- `src/UBSFlow.Aplicacao/Pacientes/PacienteService.cs`: contem a regra de aplicacao para criar e consultar pacientes.
- `src/UBSFlow.Aplicacao/Pacientes/IPacienteRepositorio.cs`: define quais operacoes um repositorio de pacientes precisa ter.
- `src/UBSFlow.Infraestrutura/Pacientes/PacienteRepositorioEmMemoria.cs`: implementa o repositorio usando uma lista em memoria.
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
PacienteRepositorioEmMemoria
        |
        v
List<Paciente>
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
