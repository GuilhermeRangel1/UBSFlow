# UBSFlow Frontend

Interface React criada como uma camada separada e reversivel da API.

## Como rodar

```bash
npm install
npm run dev
```

Por padrao, o Vite abre em `http://localhost:5173`.

## Como rodar com Docker

Na raiz do projeto:

```bash
docker compose up -d --build
```

O frontend fica em `http://localhost:5173`.

## Integracao com a API

Durante o desenvolvimento, chamadas para `/api` sao encaminhadas para `http://localhost:5000`.

Dentro do Docker Compose, o proxy usa `http://api:8080`, configurado pela variavel `VITE_PROXY_TARGET`.

Para apontar para outra URL, crie um arquivo `.env.local` nesta pasta:

```env
VITE_API_BASE_URL=http://localhost:5000
```

## Por que fica em uma pasta separada

O frontend foi criado dentro de `frontend/` para manter o backend intacto. Se a interface precisar ser refeita, basta substituir essa pasta sem alterar os projetos `.NET` em `src/` e `tests/`.
