export type LoginResponse = {
  token: string;
  tipo: string;
  expiraEm: string;
  usuario: {
    id: string;
    nome: string;
    usuario: string;
    papel: string;
  };
};

export type ResultadoPaginado<T> = {
  itens: T[];
  pagina: number;
  tamanhoPagina: number;
  totalItens: number;
  totalPaginas: number;
};

export type Paciente = {
  id: string;
  nome: string;
  cpf: string;
  cns: string | null;
  dataNascimento: string;
  telefone: string;
};

export type CriarPacienteRequest = {
  nome: string;
  cpf: string;
  dataNascimento: string;
  telefone: string;
  cns?: string | null;
};

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "/api";

async function parseError(response: Response) {
  try {
    const body = await response.json();
    return body.mensagem ?? "Nao foi possivel concluir a operacao.";
  } catch {
    return "Nao foi possivel concluir a operacao.";
  }
}

export async function login(usuario: string, senha: string): Promise<LoginResponse> {
  const response = await fetch(`${API_BASE_URL}/auth/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({ usuario, senha })
  });

  if (!response.ok) {
    throw new Error(await parseError(response));
  }

  return response.json() as Promise<LoginResponse>;
}

export async function listarPacientes(
  token: string,
  filtros: { nome?: string; cpf?: string }
): Promise<ResultadoPaginado<Paciente>> {
  const params = new URLSearchParams({
    pagina: "1",
    tamanhoPagina: "20"
  });

  if (filtros.nome) {
    params.set("nome", filtros.nome);
  }

  if (filtros.cpf) {
    params.set("cpf", filtros.cpf);
  }

  const response = await fetch(`${API_BASE_URL}/pacientes?${params.toString()}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  if (!response.ok) {
    throw new Error(await parseError(response));
  }

  return response.json() as Promise<ResultadoPaginado<Paciente>>;
}

export async function criarPaciente(
  token: string,
  paciente: CriarPacienteRequest
): Promise<Paciente> {
  const response = await fetch(`${API_BASE_URL}/pacientes`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json"
    },
    body: JSON.stringify(paciente)
  });

  if (!response.ok) {
    throw new Error(await parseError(response));
  }

  return response.json() as Promise<Paciente>;
}

export const demoUsers = [
  { usuario: "admin", senha: "admin123", papel: "ADMIN" },
  { usuario: "recepcao", senha: "recepcao123", papel: "RECEPCIONISTA" },
  { usuario: "enfermagem", senha: "enfermagem123", papel: "ENFERMEIRO" },
  { usuario: "medico", senha: "medico123", papel: "MEDICO" },
  { usuario: "gestao", senha: "gestao123", papel: "GESTOR" }
];
