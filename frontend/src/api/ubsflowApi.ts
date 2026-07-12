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

export type Profissional = {
  id: string;
  nome: string;
  papel: string;
  especialidade: string | null;
  registroProfissional: string | null;
};

export type CriarProfissionalRequest = {
  nome: string;
  papel: string;
  especialidade?: string | null;
  registroProfissional?: string | null;
};

export type Agendamento = {
  id: string;
  pacienteId: string;
  profissionalId: string;
  inicio: string;
  fim: string;
  status: string;
  motivoCancelamento: string | null;
};

export type CriarAgendamentoRequest = {
  pacienteId: string;
  profissionalId: string;
  inicio: string;
  fim: string;
};

export type CheckIn = {
  id: string;
  agendamentoId: string;
  pacienteId: string;
  profissionalId: string;
  realizadoEm: string;
  status: string;
  classificacaoRisco: string | null;
};

export type Triagem = {
  id: string;
  checkInId: string;
  pacienteId: string;
  temperatura: number;
  pressaoSistolica: number;
  pressaoDiastolica: number;
  frequenciaCardiaca: number;
  sintomas: string;
  classificacaoRisco: string;
  observacoes: string | null;
  realizadaEm: string;
};

export type CriarTriagemRequest = {
  checkInId: string;
  temperatura: number;
  pressaoSistolica: number;
  pressaoDiastolica: number;
  frequenciaCardiaca: number;
  sintomas: string;
  classificacaoRisco: string;
  observacoes?: string | null;
};

export type Atendimento = {
  id: string;
  checkInId: string;
  pacienteId: string;
  profissionalId: string;
  queixa: string;
  hipoteseDiagnostica: string;
  conduta: string;
  prescricao: string | null;
  encaminhamento: string | null;
  iniciadoEm: string;
  finalizadoEm: string | null;
};

export type CriarAtendimentoRequest = {
  checkInId: string;
  queixa: string;
  hipoteseDiagnostica: string;
  conduta: string;
  prescricao?: string | null;
  encaminhamento?: string | null;
};

export type RelatorioAtendimentos = {
  inicio: string;
  fim: string;
  totalAtendimentos: number;
  totalFinalizados: number;
};

export type RelatorioCancelamentos = {
  inicio: string;
  fim: string;
  totalCancelamentos: number;
};

export type RelatorioRisco = {
  inicio: string;
  fim: string;
  itens: Array<{ classificacaoRisco: string; total: number }>;
};

export type LogAuditoria = {
  id: string;
  acao: string;
  entidade: string;
  entidadeId: string;
  usuario: string;
  descricao: string;
  registradoEm: string;
};

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "/api";

async function parseError(response: Response) {
  try {
    const body = await response.json();
    return body.mensagem ?? "Não foi possível concluir.";
  } catch {
    return "Não foi possível concluir.";
  }
}

async function request<T>(path: string, token?: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(init?.body ? { "Content-Type": "application/json" } : {}),
      ...init?.headers
    }
  });

  if (!response.ok) {
    throw new Error(await parseError(response));
  }

  return response.json() as Promise<T>;
}

export async function login(usuario: string, senha: string): Promise<LoginResponse> {
  return request<LoginResponse>("/auth/login", undefined, {
    method: "POST",
    body: JSON.stringify({ usuario, senha })
  });
}

export async function listarPacientes(
  token: string,
  filtros: { nome?: string; cpf?: string }
): Promise<ResultadoPaginado<Paciente>> {
  const params = new URLSearchParams({ pagina: "1", tamanhoPagina: "20" });

  if (filtros.nome) {
    params.set("nome", filtros.nome);
  }

  if (filtros.cpf) {
    params.set("cpf", filtros.cpf);
  }

  return request<ResultadoPaginado<Paciente>>(`/pacientes?${params.toString()}`, token);
}

export async function criarPaciente(
  token: string,
  paciente: CriarPacienteRequest
): Promise<Paciente> {
  return request<Paciente>("/pacientes", token, {
    method: "POST",
    body: JSON.stringify(paciente)
  });
}

export async function listarProfissionais(token: string): Promise<ResultadoPaginado<Profissional>> {
  return request<ResultadoPaginado<Profissional>>("/profissionais?pagina=1&tamanhoPagina=30", token);
}

export async function criarProfissional(
  token: string,
  profissional: CriarProfissionalRequest
): Promise<Profissional> {
  return request<Profissional>("/profissionais", token, {
    method: "POST",
    body: JSON.stringify({ ...profissional, disponibilidades: [] })
  });
}

export async function listarAgendamentos(token: string): Promise<ResultadoPaginado<Agendamento>> {
  return request<ResultadoPaginado<Agendamento>>("/agendamentos?pagina=1&tamanhoPagina=30", token);
}

export async function criarAgendamento(
  token: string,
  agendamento: CriarAgendamentoRequest
): Promise<Agendamento> {
  return request<Agendamento>("/agendamentos", token, {
    method: "POST",
    body: JSON.stringify(agendamento)
  });
}

export async function listarFilaHoje(token: string): Promise<CheckIn[]> {
  return request<CheckIn[]>("/fila/hoje", token);
}

export async function criarCheckIn(token: string, agendamentoId: string): Promise<CheckIn> {
  return request<CheckIn>("/fila/check-ins", token, {
    method: "POST",
    body: JSON.stringify({ agendamentoId })
  });
}

export async function criarTriagem(token: string, triagem: CriarTriagemRequest): Promise<Triagem> {
  return request<Triagem>("/triagens", token, {
    method: "POST",
    body: JSON.stringify(triagem)
  });
}

export async function criarAtendimento(
  token: string,
  atendimento: CriarAtendimentoRequest
): Promise<Atendimento> {
  return request<Atendimento>("/atendimentos", token, {
    method: "POST",
    body: JSON.stringify(atendimento)
  });
}

export async function finalizarAtendimento(token: string, id: string): Promise<Atendimento> {
  return request<Atendimento>(`/atendimentos/${id}/finalizar`, token, {
    method: "PATCH",
    body: JSON.stringify({})
  });
}

export async function carregarRelatorios(token: string, inicio: string, fim: string) {
  const params = `inicio=${inicio}&fim=${fim}`;

  const [atendimentos, riscos, cancelamentos] = await Promise.all([
    request<RelatorioAtendimentos>(`/relatorios/atendimentos?${params}`, token),
    request<RelatorioRisco>(`/relatorios/classificacoes-risco?${params}`, token),
    request<RelatorioCancelamentos>(`/relatorios/cancelamentos?${params}`, token)
  ]);

  return { atendimentos, riscos, cancelamentos };
}

export async function listarAuditoria(token: string): Promise<LogAuditoria[]> {
  return request<LogAuditoria[]>("/auditoria", token);
}

export const demoUsers = [
  { usuario: "admin", senha: "admin123", papel: "ADMIN" },
  { usuario: "recepcao", senha: "recepcao123", papel: "RECEPCIONISTA" },
  { usuario: "enfermagem", senha: "enfermagem123", papel: "ENFERMEIRO" },
  { usuario: "medico", senha: "medico123", papel: "MEDICO" },
  { usuario: "gestao", senha: "gestao123", papel: "GESTOR" }
];
