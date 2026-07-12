import {
  Activity,
  BarChart3,
  CalendarClock,
  ClipboardList,
  FileClock,
  HeartPulse,
  LayoutDashboard,
  LockKeyhole,
  LogIn,
  Search,
  ShieldCheck,
  Stethoscope,
  UserRound,
  UsersRound
} from "lucide-react";
import { type ReactNode, useEffect, useMemo, useState } from "react";
import {
  carregarRelatorios,
  criarAgendamento,
  criarAtendimento,
  criarCheckIn,
  criarPaciente,
  criarProfissional,
  criarTriagem,
  demoUsers,
  finalizarAtendimento,
  listarAgendamentos,
  listarAuditoria,
  listarFilaHoje,
  listarPacientes,
  listarProfissionais,
  login,
  type Agendamento,
  type Atendimento,
  type CheckIn,
  type CriarAgendamentoRequest,
  type CriarAtendimentoRequest,
  type CriarPacienteRequest,
  type CriarProfissionalRequest,
  type CriarTriagemRequest,
  type LogAuditoria,
  type LoginResponse,
  type Paciente,
  type Profissional,
  type RelatorioAtendimentos,
  type RelatorioCancelamentos,
  type RelatorioRisco
} from "./api/ubsflowApi";

type ModuleKey =
  | "visao-geral"
  | "pacientes"
  | "profissionais"
  | "agenda"
  | "fila"
  | "triagem"
  | "atendimentos"
  | "relatorios"
  | "auditoria";

type ModuleItem = {
  key: ModuleKey;
  title: string;
  icon: typeof Activity;
  roles: string[];
};

type ReportsState = {
  atendimentos: RelatorioAtendimentos | null;
  riscos: RelatorioRisco | null;
  cancelamentos: RelatorioCancelamentos | null;
};

const profileLabels: Record<string, string> = {
  admin: "Administração",
  recepcao: "Recepção",
  enfermagem: "Enfermagem",
  medico: "Médico",
  gestao: "Gestão"
};

const roleLabels: Record<string, string> = {
  ADMIN: "Administração",
  RECEPCIONISTA: "Recepção",
  ENFERMEIRO: "Enfermagem",
  MEDICO: "Médico",
  GESTOR: "Gestão",
  Todos: "Todas as equipes"
};

const modules: ModuleItem[] = [
  { key: "visao-geral", title: "Início", icon: LayoutDashboard, roles: ["Todos"] },
  { key: "pacientes", title: "Pacientes", icon: UserRound, roles: ["ADMIN", "RECEPCIONISTA", "ENFERMEIRO", "MEDICO", "GESTOR"] },
  { key: "profissionais", title: "Profissionais", icon: UsersRound, roles: ["ADMIN", "GESTOR"] },
  { key: "agenda", title: "Agenda", icon: CalendarClock, roles: ["ADMIN", "RECEPCIONISTA"] },
  { key: "fila", title: "Fila", icon: ClipboardList, roles: ["ADMIN", "RECEPCIONISTA", "ENFERMEIRO", "MEDICO", "GESTOR"] },
  { key: "triagem", title: "Triagem", icon: HeartPulse, roles: ["ADMIN", "ENFERMEIRO"] },
  { key: "atendimentos", title: "Atendimentos", icon: Stethoscope, roles: ["ADMIN", "MEDICO"] },
  { key: "relatorios", title: "Relatórios", icon: BarChart3, roles: ["ADMIN", "GESTOR"] },
  { key: "auditoria", title: "Auditoria", icon: FileClock, roles: ["ADMIN"] }
];

function canAccess(module: ModuleItem, role?: string) {
  return module.roles.includes("Todos") || (role ? module.roles.includes(role) : false);
}

function today() {
  return new Date().toISOString().slice(0, 10);
}

function toDateTimeOffset(value: string) {
  return value ? new Date(value).toISOString() : "";
}

export function App() {
  const [activeModule, setActiveModule] = useState<ModuleKey>("visao-geral");
  const [usuario, setUsuario] = useState("medico");
  const [senha, setSenha] = useState("medico123");
  const [session, setSession] = useState<LoginResponse | null>(null);
  const [loginStatus, setLoginStatus] = useState("Escolha sua área para continuar.");

  const [pacientes, setPacientes] = useState<Paciente[]>([]);
  const [pacientesStatus, setPacientesStatus] = useState("");
  const [filtroNome, setFiltroNome] = useState("");
  const [filtroCpf, setFiltroCpf] = useState("");
  const [novoPaciente, setNovoPaciente] = useState<CriarPacienteRequest>({
    nome: "",
    cpf: "",
    dataNascimento: "",
    telefone: "",
    cns: ""
  });

  const [profissionais, setProfissionais] = useState<Profissional[]>([]);
  const [profissionaisStatus, setProfissionaisStatus] = useState("");
  const [novoProfissional, setNovoProfissional] = useState<CriarProfissionalRequest>({
    nome: "",
    papel: "MEDICO",
    especialidade: "",
    registroProfissional: ""
  });

  const [agendamentos, setAgendamentos] = useState<Agendamento[]>([]);
  const [agendaStatus, setAgendaStatus] = useState("");
  const [novoAgendamento, setNovoAgendamento] = useState<CriarAgendamentoRequest>({
    pacienteId: "",
    profissionalId: "",
    inicio: "",
    fim: ""
  });

  const [fila, setFila] = useState<CheckIn[]>([]);
  const [filaStatus, setFilaStatus] = useState("");
  const [agendamentoCheckIn, setAgendamentoCheckIn] = useState("");

  const [triagemStatus, setTriagemStatus] = useState("");
  const [novaTriagem, setNovaTriagem] = useState<CriarTriagemRequest>({
    checkInId: "",
    temperatura: 36.5,
    pressaoSistolica: 120,
    pressaoDiastolica: 80,
    frequenciaCardiaca: 80,
    sintomas: "",
    classificacaoRisco: "VERDE",
    observacoes: ""
  });

  const [atendimentoStatus, setAtendimentoStatus] = useState("");
  const [atendimentoCriado, setAtendimentoCriado] = useState<Atendimento | null>(null);
  const [novoAtendimento, setNovoAtendimento] = useState<CriarAtendimentoRequest>({
    checkInId: "",
    queixa: "",
    hipoteseDiagnostica: "",
    conduta: "",
    prescricao: "",
    encaminhamento: ""
  });

  const [relatoriosStatus, setRelatoriosStatus] = useState("");
  const [periodoInicio, setPeriodoInicio] = useState(today());
  const [periodoFim, setPeriodoFim] = useState(today());
  const [relatorios, setRelatorios] = useState<ReportsState>({
    atendimentos: null,
    riscos: null,
    cancelamentos: null
  });

  const [auditoria, setAuditoria] = useState<LogAuditoria[]>([]);
  const [auditoriaStatus, setAuditoriaStatus] = useState("");

  const visibleModules = useMemo(
    () => modules.filter((module) => canAccess(module, session?.usuario.papel)),
    [session?.usuario.papel]
  );

  const selectedModule = useMemo(
    () => visibleModules.find((module) => module.key === activeModule) ?? visibleModules[0],
    [activeModule, visibleModules]
  );

  async function handleLogin() {
    setLoginStatus("Entrando...");

    try {
      const response = await login(usuario, senha);
      setSession(response);
      setLoginStatus(`Conectado como ${response.usuario.nome}`);
      setActiveModule("visao-geral");
    } catch (error) {
      setSession(null);
      setLoginStatus(error instanceof Error ? error.message : "Não foi possível entrar.");
    }
  }

  async function carregarPacientes() {
    if (!session) return;

    setPacientesStatus("Carregando...");
    try {
      const response = await listarPacientes(session.token, {
        nome: filtroNome.trim(),
        cpf: filtroCpf.trim()
      });
      setPacientes(response.itens);
      setPacientesStatus(`${response.totalItens} encontrado(s).`);
    } catch (error) {
      setPacientesStatus(error instanceof Error ? error.message : "Falha ao carregar.");
    }
  }

  async function handleCriarPaciente() {
    if (!session) return;

    setPacientesStatus("Salvando...");
    try {
      await criarPaciente(session.token, {
        ...novoPaciente,
        cpf: novoPaciente.cpf.replace(/\D/g, ""),
        cns: novoPaciente.cns?.trim() || null
      });
      setNovoPaciente({ nome: "", cpf: "", dataNascimento: "", telefone: "", cns: "" });
      await carregarPacientes();
      setPacientesStatus("Paciente salvo.");
    } catch (error) {
      setPacientesStatus(error instanceof Error ? error.message : "Falha ao salvar.");
    }
  }

  async function carregarProfissionais() {
    if (!session) return;

    setProfissionaisStatus("Carregando...");
    try {
      const response = await listarProfissionais(session.token);
      setProfissionais(response.itens);
      setProfissionaisStatus(`${response.totalItens} encontrado(s).`);
    } catch (error) {
      setProfissionaisStatus(error instanceof Error ? error.message : "Falha ao carregar.");
    }
  }

  async function handleCriarProfissional() {
    if (!session) return;

    setProfissionaisStatus("Salvando...");
    try {
      await criarProfissional(session.token, {
        ...novoProfissional,
        especialidade: novoProfissional.especialidade || null,
        registroProfissional: novoProfissional.registroProfissional || null
      });
      setNovoProfissional({ nome: "", papel: "MEDICO", especialidade: "", registroProfissional: "" });
      await carregarProfissionais();
      setProfissionaisStatus("Profissional salvo.");
    } catch (error) {
      setProfissionaisStatus(error instanceof Error ? error.message : "Falha ao salvar.");
    }
  }

  async function carregarAgenda() {
    if (!session) return;

    setAgendaStatus("Carregando...");
    try {
      const response = await listarAgendamentos(session.token);
      setAgendamentos(response.itens);
      setAgendaStatus(`${response.totalItens} encontrado(s).`);
    } catch (error) {
      setAgendaStatus(error instanceof Error ? error.message : "Falha ao carregar.");
    }
  }

  async function handleCriarAgendamento() {
    if (!session) return;

    setAgendaStatus("Salvando...");
    try {
      await criarAgendamento(session.token, {
        pacienteId: novoAgendamento.pacienteId,
        profissionalId: novoAgendamento.profissionalId,
        inicio: toDateTimeOffset(novoAgendamento.inicio),
        fim: toDateTimeOffset(novoAgendamento.fim)
      });
      setNovoAgendamento({ pacienteId: "", profissionalId: "", inicio: "", fim: "" });
      await carregarAgenda();
      setAgendaStatus("Agendamento salvo.");
    } catch (error) {
      setAgendaStatus(error instanceof Error ? error.message : "Falha ao salvar.");
    }
  }

  async function carregarFila() {
    if (!session) return;

    setFilaStatus("Carregando...");
    try {
      const response = await listarFilaHoje(session.token);
      setFila(response);
      setFilaStatus(`${response.length} na fila.`);
    } catch (error) {
      setFilaStatus(error instanceof Error ? error.message : "Falha ao carregar.");
    }
  }

  async function handleCriarCheckIn() {
    if (!session) return;

    setFilaStatus("Salvando...");
    try {
      await criarCheckIn(session.token, agendamentoCheckIn);
      setAgendamentoCheckIn("");
      if (session.usuario.papel !== "RECEPCIONISTA") {
        await carregarFila();
      }
      setFilaStatus("Chegada registrada.");
    } catch (error) {
      setFilaStatus(error instanceof Error ? error.message : "Falha ao salvar.");
    }
  }

  async function handleCriarTriagem() {
    if (!session) return;

    setTriagemStatus("Salvando...");
    try {
      await criarTriagem(session.token, {
        ...novaTriagem,
        observacoes: novaTriagem.observacoes || null
      });
      setNovaTriagem({
        checkInId: "",
        temperatura: 36.5,
        pressaoSistolica: 120,
        pressaoDiastolica: 80,
        frequenciaCardiaca: 80,
        sintomas: "",
        classificacaoRisco: "VERDE",
        observacoes: ""
      });
      setTriagemStatus("Triagem salva.");
      await carregarFila();
    } catch (error) {
      setTriagemStatus(error instanceof Error ? error.message : "Falha ao salvar.");
    }
  }

  async function handleCriarAtendimento() {
    if (!session) return;

    setAtendimentoStatus("Salvando...");
    try {
      const atendimento = await criarAtendimento(session.token, {
        ...novoAtendimento,
        prescricao: novoAtendimento.prescricao || null,
        encaminhamento: novoAtendimento.encaminhamento || null
      });
      setAtendimentoCriado(atendimento);
      setNovoAtendimento({
        checkInId: "",
        queixa: "",
        hipoteseDiagnostica: "",
        conduta: "",
        prescricao: "",
        encaminhamento: ""
      });
      setAtendimentoStatus("Atendimento salvo.");
    } catch (error) {
      setAtendimentoStatus(error instanceof Error ? error.message : "Falha ao salvar.");
    }
  }

  async function handleFinalizarAtendimento() {
    if (!session || !atendimentoCriado) return;

    setAtendimentoStatus("Finalizando...");
    try {
      const response = await finalizarAtendimento(session.token, atendimentoCriado.id);
      setAtendimentoCriado(response);
      setAtendimentoStatus("Atendimento finalizado.");
    } catch (error) {
      setAtendimentoStatus(error instanceof Error ? error.message : "Falha ao finalizar.");
    }
  }

  async function handleCarregarRelatorios() {
    if (!session) return;

    setRelatoriosStatus("Carregando...");
    try {
      setRelatorios(await carregarRelatorios(session.token, periodoInicio, periodoFim));
      setRelatoriosStatus("Atualizado.");
    } catch (error) {
      setRelatoriosStatus(error instanceof Error ? error.message : "Falha ao carregar.");
    }
  }

  async function handleCarregarAuditoria() {
    if (!session) return;

    setAuditoriaStatus("Carregando...");
    try {
      const response = await listarAuditoria(session.token);
      setAuditoria(response);
      setAuditoriaStatus(`${response.length} registro(s).`);
    } catch (error) {
      setAuditoriaStatus(error instanceof Error ? error.message : "Falha ao carregar.");
    }
  }

  useEffect(() => {
    if (!session) return;

    if (!visibleModules.some((module) => module.key === activeModule)) {
      setActiveModule("visao-geral");
    }
  }, [activeModule, session, visibleModules]);

  useEffect(() => {
    if (!session) return;

    if (activeModule === "pacientes") void carregarPacientes();
    if (activeModule === "profissionais") void carregarProfissionais();
    if (activeModule === "agenda") void carregarAgenda();
    if (activeModule === "fila" && session.usuario.papel !== "RECEPCIONISTA") void carregarFila();
    if (activeModule === "relatorios") void handleCarregarRelatorios();
    if (activeModule === "auditoria") void handleCarregarAuditoria();
  }, [activeModule, session]);

  if (!session) {
    return (
      <PortfolioLanding
        loginStatus={loginStatus}
        onLogin={handleLogin}
        senha={senha}
        setSenha={setSenha}
        setUsuario={setUsuario}
        usuario={usuario}
      />
    );
  }

  return (
    <main className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-mark">
            <Activity size={24} />
          </div>
          <div>
            <strong>UBSFlow</strong>
            <span>{roleLabels[session.usuario.papel] ?? session.usuario.papel}</span>
          </div>
        </div>

        <nav className="nav-list" aria-label="Áreas">
          {visibleModules.map((module) => {
            const Icon = module.icon;

            return (
              <button
                className={module.key === activeModule ? "nav-item active" : "nav-item"}
                key={module.key}
                onClick={() => setActiveModule(module.key)}
                type="button"
              >
                <Icon size={18} />
                <span>{module.title}</span>
              </button>
            );
          })}
        </nav>

        <section className="session-panel">
          <div className="session-heading">
            <LockKeyhole size={16} />
            <span>Acesso</span>
          </div>
          <label>
            Área
            <select
              value={usuario}
              onChange={(event) => {
                const selected = demoUsers.find((item) => item.usuario === event.target.value);
                setUsuario(event.target.value);
                setSenha(selected?.senha ?? "");
              }}
            >
              {demoUsers.map((user) => (
                <option key={user.usuario} value={user.usuario}>
                  {profileLabels[user.usuario] ?? user.papel}
                </option>
              ))}
            </select>
          </label>
          <label>
            Senha
            <input value={senha} onChange={(event) => setSenha(event.target.value)} type="password" />
          </label>
          <button className="primary-action" onClick={handleLogin} type="button">
            <LogIn size={16} />
            Entrar
          </button>
          <p>{loginStatus}</p>
        </section>
      </aside>

      <section className="content app-content">
        {selectedModule.key === "visao-geral" && (
          <HomeScreen modules={visibleModules} onNavigate={setActiveModule} userName={session.usuario.nome} />
        )}

        {selectedModule.key === "pacientes" && (
          <WorkPage title="Pacientes">
            <PacientesCrud
              filtroCpf={filtroCpf}
              filtroNome={filtroNome}
              novoPaciente={novoPaciente}
              onBuscar={carregarPacientes}
              onCriar={handleCriarPaciente}
              pacientes={pacientes}
              podeCriar={session.usuario.papel === "ADMIN" || session.usuario.papel === "RECEPCIONISTA"}
              setFiltroCpf={setFiltroCpf}
              setFiltroNome={setFiltroNome}
              setNovoPaciente={setNovoPaciente}
              status={pacientesStatus}
            />
          </WorkPage>
        )}

        {selectedModule.key === "profissionais" && (
          <WorkPage title="Profissionais">
            <ProfissionaisPage
              novoProfissional={novoProfissional}
              onCriar={handleCriarProfissional}
              onListar={carregarProfissionais}
              podeCriar={session.usuario.papel === "ADMIN"}
              profissionais={profissionais}
              setNovoProfissional={setNovoProfissional}
              status={profissionaisStatus}
            />
          </WorkPage>
        )}

        {selectedModule.key === "agenda" && (
          <WorkPage title="Agenda">
            <AgendaPage
              agendamentos={agendamentos}
              novoAgendamento={novoAgendamento}
              onCriar={handleCriarAgendamento}
              onListar={carregarAgenda}
              setNovoAgendamento={setNovoAgendamento}
              status={agendaStatus}
            />
          </WorkPage>
        )}

        {selectedModule.key === "fila" && (
          <WorkPage title="Fila">
            <FilaPage
              agendamentoCheckIn={agendamentoCheckIn}
              fila={fila}
              onCriarCheckIn={handleCriarCheckIn}
              onListar={carregarFila}
              podeCriar={session.usuario.papel === "ADMIN" || session.usuario.papel === "RECEPCIONISTA"}
              podeListar={session.usuario.papel !== "RECEPCIONISTA"}
              setAgendamentoCheckIn={setAgendamentoCheckIn}
              status={filaStatus}
            />
          </WorkPage>
        )}

        {selectedModule.key === "triagem" && (
          <WorkPage title="Triagem">
            <TriagemPage
              novaTriagem={novaTriagem}
              onCriar={handleCriarTriagem}
              setNovaTriagem={setNovaTriagem}
              status={triagemStatus}
            />
          </WorkPage>
        )}

        {selectedModule.key === "atendimentos" && (
          <WorkPage title="Atendimentos">
            <AtendimentosPage
              atendimentoCriado={atendimentoCriado}
              novoAtendimento={novoAtendimento}
              onCriar={handleCriarAtendimento}
              onFinalizar={handleFinalizarAtendimento}
              setNovoAtendimento={setNovoAtendimento}
              status={atendimentoStatus}
            />
          </WorkPage>
        )}

        {selectedModule.key === "relatorios" && (
          <WorkPage title="Relatórios">
            <RelatoriosPage
              fim={periodoFim}
              inicio={periodoInicio}
              onCarregar={handleCarregarRelatorios}
              relatorios={relatorios}
              setFim={setPeriodoFim}
              setInicio={setPeriodoInicio}
              status={relatoriosStatus}
            />
          </WorkPage>
        )}

        {selectedModule.key === "auditoria" && (
          <WorkPage title="Auditoria">
            <AuditoriaPage auditoria={auditoria} onListar={handleCarregarAuditoria} status={auditoriaStatus} />
          </WorkPage>
        )}
      </section>
    </main>
  );
}

function WorkPage({ children, title }: { children: ReactNode; title: string }) {
  return (
    <div className="app-page work-page">
      <header className="work-header">
        <span>UBSFlow</span>
        <h1>{title}</h1>
      </header>
      {children}
    </div>
  );
}

function HomeScreen({
  modules,
  onNavigate,
  userName
}: {
  modules: ModuleItem[];
  onNavigate: (module: ModuleKey) => void;
  userName: string;
}) {
  return (
    <div className="app-page home-page">
      <section className="home-compact">
        <span>Olá, {userName}</span>
        <h1>Escolha uma área.</h1>
      </section>

      <section className="drawer-grid" aria-label="Áreas disponíveis">
        {modules
          .filter((module) => module.key !== "visao-geral")
          .map((module) => {
            const Icon = module.icon;
            return (
              <button key={module.key} onClick={() => onNavigate(module.key)} type="button">
                <Icon size={24} />
                <strong>{module.title}</strong>
              </button>
            );
          })}
      </section>
    </div>
  );
}

function PortfolioLanding({
  loginStatus,
  onLogin,
  senha,
  setSenha,
  setUsuario,
  usuario
}: {
  loginStatus: string;
  onLogin: () => void;
  senha: string;
  setSenha: (value: string) => void;
  setUsuario: (value: string) => void;
  usuario: string;
}) {
  function selecionarUsuario(value: string) {
    const selected = demoUsers.find((item) => item.usuario === value);
    setUsuario(value);
    setSenha(selected?.senha ?? "");
  }

  return (
    <main className="portfolio-shell">
      <nav className="portfolio-nav" aria-label="Principal">
        <div className="portfolio-brand">
          <span>
            <Activity size={22} />
          </span>
          <strong>UBSFlow</strong>
        </div>
        <div className="portfolio-links">
          <a href="#produto">Produto</a>
          <a href="#fluxo">Fluxo</a>
          <a href="#acesso">Acesso</a>
        </div>
      </nav>

      <section className="portfolio-hero">
        <div className="hero-copy">
          <span className="portfolio-kicker">Atendimento organizado. Equipe conectada.</span>
          <h1>O fluxo da unidade inteiro em uma plataforma só.</h1>
          <p>
            UBSFlow aproxima recepção, enfermagem, médicos e gestão em uma jornada clara:
            da chegada do paciente ao fechamento do atendimento.
          </p>
          <div className="portfolio-actions">
            <a className="landing-primary" href="#acesso">Entrar</a>
            <a className="landing-secondary" href="#fluxo">Ver fluxo</a>
          </div>
        </div>

        <div className="showcase-panel" aria-label="Prévia do UBSFlow">
          <video
            autoPlay
            loop
            muted
            playsInline
            poster="https://images.unsplash.com/photo-1584515933487-779824d29309?auto=format&fit=crop&w=1200&q=85"
          >
            <source
              src="https://cdn.coverr.co/videos/coverr-medical-team-discussing-patient-results-5133/1080p.mp4"
              type="video/mp4"
            />
          </video>
          <div className="showcase-screen">
            <div>
              <small>Atendimento em movimento</small>
              <strong>Triagem prioritária</strong>
            </div>
            <div className="pulse-line">
              <span />
              <span />
              <span />
            </div>
            <div className="patient-stack">
              <article>
                <b>Maria Souza</b>
                <small>Risco amarelo - 09 min</small>
              </article>
              <article>
                <b>João Lima</b>
                <small>Consulta médica - sala 2</small>
              </article>
              <article>
                <b>Ana Costa</b>
                <small>Check-in concluído</small>
              </article>
            </div>
          </div>
        </div>
      </section>

      <section className="portfolio-section" id="produto">
        <div className="section-heading">
          <span>Por que existe</span>
          <h2>Menos espera, mais clareza, melhor continuidade.</h2>
        </div>
        <div className="feature-reel">
          <article>
            <UserRound size={24} />
            <strong>Paciente</strong>
            <p>Cadastro, busca e histórico entram como porta de entrada do atendimento.</p>
          </article>
          <article>
            <HeartPulse size={24} />
            <strong>Prioridade</strong>
            <p>Triagem, sinais vitais e risco mostram uma rotina real.</p>
          </article>
          <article>
            <ShieldCheck size={24} />
            <strong>Controle</strong>
            <p>Cada equipe atua no seu espaço.</p>
          </article>
        </div>
      </section>

      <section className="story-section" id="fluxo">
        <div className="section-heading">
          <span>Jornada</span>
          <h2>Da chegada ao fechamento.</h2>
        </div>
      </section>

      <section className="demo-section" id="acesso">
        <div className="demo-copy">
          <span>Acesso</span>
          <h2>Escolha sua área.</h2>
        </div>

        <div className="landing-login">
          <div className="session-heading">
            <LockKeyhole size={16} />
            <span>Entrar na UBSFlow</span>
          </div>
          <label>
            Área
            <select value={usuario} onChange={(event) => selecionarUsuario(event.target.value)}>
              {demoUsers.map((user) => (
                <option key={user.usuario} value={user.usuario}>
                  {profileLabels[user.usuario] ?? user.papel}
                </option>
              ))}
            </select>
          </label>
          <label>
            Senha
            <input value={senha} onChange={(event) => setSenha(event.target.value)} type="password" />
          </label>
          <button className="landing-primary full" onClick={onLogin} type="button">
            Entrar
            <LogIn size={17} />
          </button>
          <p>{loginStatus}</p>
        </div>
      </section>
    </main>
  );
}

function PacientesCrud({
  filtroCpf,
  filtroNome,
  novoPaciente,
  onBuscar,
  onCriar,
  pacientes,
  podeCriar,
  setFiltroCpf,
  setFiltroNome,
  setNovoPaciente,
  status
}: {
  filtroCpf: string;
  filtroNome: string;
  novoPaciente: CriarPacienteRequest;
  onBuscar: () => void;
  onCriar: () => void;
  pacientes: Paciente[];
  podeCriar: boolean;
  setFiltroCpf: (value: string) => void;
  setFiltroNome: (value: string) => void;
  setNovoPaciente: (value: CriarPacienteRequest) => void;
  status: string;
}) {
  return (
    <div className="work-grid">
      <section className="work-card">
        <CardTitle title="Buscar" status={status} />
        <div className="form-grid two-columns">
          <label>
            Nome
            <input value={filtroNome} onChange={(event) => setFiltroNome(event.target.value)} />
          </label>
          <label>
            CPF
            <input value={filtroCpf} onChange={(event) => setFiltroCpf(event.target.value)} maxLength={11} />
          </label>
        </div>
        <button className="primary-action" onClick={onBuscar} type="button">
          <Search size={16} />
          Buscar
        </button>
      </section>

      {podeCriar && (
        <section className="work-card">
          <CardTitle title="Cadastrar" />
          <div className="form-grid">
            <label>
              Nome
              <input value={novoPaciente.nome} onChange={(event) => setNovoPaciente({ ...novoPaciente, nome: event.target.value })} />
            </label>
            <label>
              CPF
              <input maxLength={11} value={novoPaciente.cpf} onChange={(event) => setNovoPaciente({ ...novoPaciente, cpf: event.target.value })} />
            </label>
            <label>
              Nascimento
              <input type="date" value={novoPaciente.dataNascimento} onChange={(event) => setNovoPaciente({ ...novoPaciente, dataNascimento: event.target.value })} />
            </label>
            <label>
              Telefone
              <input value={novoPaciente.telefone} onChange={(event) => setNovoPaciente({ ...novoPaciente, telefone: event.target.value })} />
            </label>
            <label>
              CNS
              <input value={novoPaciente.cns ?? ""} onChange={(event) => setNovoPaciente({ ...novoPaciente, cns: event.target.value })} />
            </label>
          </div>
          <button className="primary-action" onClick={onCriar} type="button">Salvar</button>
        </section>
      )}

      <section className="work-card wide">
        <CardTitle title="Cadastrados" />
        <SimpleTable
          columns={["Nome", "CPF", "Nascimento", "Telefone", "CNS"]}
          rows={pacientes.map((item) => [
            item.nome,
            item.cpf,
            item.dataNascimento,
            item.telefone,
            item.cns ?? "-"
          ])}
        />
      </section>
    </div>
  );
}

function ProfissionaisPage({
  novoProfissional,
  onCriar,
  onListar,
  podeCriar,
  profissionais,
  setNovoProfissional,
  status
}: {
  novoProfissional: CriarProfissionalRequest;
  onCriar: () => void;
  onListar: () => void;
  podeCriar: boolean;
  profissionais: Profissional[];
  setNovoProfissional: (value: CriarProfissionalRequest) => void;
  status: string;
}) {
  return (
    <div className="work-grid">
      {podeCriar && (
        <section className="work-card">
          <CardTitle title="Cadastrar" status={status} />
          <div className="form-grid">
            <label>
              Nome
              <input value={novoProfissional.nome} onChange={(event) => setNovoProfissional({ ...novoProfissional, nome: event.target.value })} />
            </label>
            <label>
              Papel
              <select value={novoProfissional.papel} onChange={(event) => setNovoProfissional({ ...novoProfissional, papel: event.target.value })}>
                <option value="MEDICO">Médico</option>
                <option value="ENFERMEIRO">Enfermeiro</option>
                <option value="RECEPCIONISTA">Recepcionista</option>
                <option value="GESTOR">Gestor</option>
                <option value="ADMIN">Administração</option>
              </select>
            </label>
            <label>
              Especialidade
              <input value={novoProfissional.especialidade ?? ""} onChange={(event) => setNovoProfissional({ ...novoProfissional, especialidade: event.target.value })} />
            </label>
            <label>
              Registro
              <input value={novoProfissional.registroProfissional ?? ""} onChange={(event) => setNovoProfissional({ ...novoProfissional, registroProfissional: event.target.value })} />
            </label>
          </div>
          <button className="primary-action" onClick={onCriar} type="button">Salvar</button>
        </section>
      )}

      <section className="work-card wide">
        <CardTitle title="Equipe" status={status} />
        <button className="secondary-action bordered" onClick={onListar} type="button">Atualizar</button>
        <SimpleTable
          columns={["Nome", "Papel", "Especialidade", "Registro"]}
          rows={profissionais.map((item) => [
            item.nome,
            roleLabels[item.papel] ?? item.papel,
            item.especialidade ?? "-",
            item.registroProfissional ?? "-"
          ])}
        />
      </section>
    </div>
  );
}

function AgendaPage({
  agendamentos,
  novoAgendamento,
  onCriar,
  onListar,
  setNovoAgendamento,
  status
}: {
  agendamentos: Agendamento[];
  novoAgendamento: CriarAgendamentoRequest;
  onCriar: () => void;
  onListar: () => void;
  setNovoAgendamento: (value: CriarAgendamentoRequest) => void;
  status: string;
}) {
  return (
    <div className="work-grid">
      <section className="work-card">
        <CardTitle title="Marcar" status={status} />
        <div className="form-grid">
          <label>
            Paciente ID
            <input value={novoAgendamento.pacienteId} onChange={(event) => setNovoAgendamento({ ...novoAgendamento, pacienteId: event.target.value })} />
          </label>
          <label>
            Profissional ID
            <input value={novoAgendamento.profissionalId} onChange={(event) => setNovoAgendamento({ ...novoAgendamento, profissionalId: event.target.value })} />
          </label>
          <label>
            Início
            <input type="datetime-local" value={novoAgendamento.inicio} onChange={(event) => setNovoAgendamento({ ...novoAgendamento, inicio: event.target.value })} />
          </label>
          <label>
            Fim
            <input type="datetime-local" value={novoAgendamento.fim} onChange={(event) => setNovoAgendamento({ ...novoAgendamento, fim: event.target.value })} />
          </label>
        </div>
        <button className="primary-action" onClick={onCriar} type="button">Salvar</button>
      </section>

      <section className="work-card wide">
        <CardTitle title="Agendados" />
        <button className="secondary-action bordered" onClick={onListar} type="button">Atualizar</button>
        <SimpleTable
          columns={["Paciente", "Profissional", "Início", "Fim", "Status"]}
          rows={agendamentos.map((item) => [
            item.pacienteId,
            item.profissionalId,
            new Date(item.inicio).toLocaleString("pt-BR"),
            new Date(item.fim).toLocaleString("pt-BR"),
            item.status
          ])}
        />
      </section>
    </div>
  );
}

function FilaPage({
  agendamentoCheckIn,
  fila,
  onCriarCheckIn,
  onListar,
  podeCriar,
  podeListar,
  setAgendamentoCheckIn,
  status
}: {
  agendamentoCheckIn: string;
  fila: CheckIn[];
  onCriarCheckIn: () => void;
  onListar: () => void;
  podeCriar: boolean;
  podeListar: boolean;
  setAgendamentoCheckIn: (value: string) => void;
  status: string;
}) {
  return (
    <div className="work-grid">
      {podeCriar && (
        <section className="work-card">
          <CardTitle title="Check-in" status={status} />
          <label>
            Agendamento ID
            <input value={agendamentoCheckIn} onChange={(event) => setAgendamentoCheckIn(event.target.value)} />
          </label>
          <button className="primary-action" onClick={onCriarCheckIn} type="button">Registrar</button>
        </section>
      )}

      {podeListar && (
        <section className="work-card wide">
          <CardTitle title="Hoje" status={status} />
          <button className="secondary-action bordered" onClick={onListar} type="button">Atualizar</button>
          <SimpleTable
            columns={["Check-in", "Paciente", "Profissional", "Status", "Risco"]}
            rows={fila.map((item) => [
              item.id,
              item.pacienteId,
              item.profissionalId,
              item.status,
              item.classificacaoRisco ?? "-"
            ])}
          />
        </section>
      )}
    </div>
  );
}

function TriagemPage({
  novaTriagem,
  onCriar,
  setNovaTriagem,
  status
}: {
  novaTriagem: CriarTriagemRequest;
  onCriar: () => void;
  setNovaTriagem: (value: CriarTriagemRequest) => void;
  status: string;
}) {
  return (
    <section className="work-card wide">
      <CardTitle title="Nova triagem" status={status} />
      <div className="form-grid">
        <label>
          Check-in ID
          <input value={novaTriagem.checkInId} onChange={(event) => setNovaTriagem({ ...novaTriagem, checkInId: event.target.value })} />
        </label>
        <label>
          Temperatura
          <input type="number" step="0.1" value={novaTriagem.temperatura} onChange={(event) => setNovaTriagem({ ...novaTriagem, temperatura: Number(event.target.value) })} />
        </label>
        <label>
          Sistólica
          <input type="number" value={novaTriagem.pressaoSistolica} onChange={(event) => setNovaTriagem({ ...novaTriagem, pressaoSistolica: Number(event.target.value) })} />
        </label>
        <label>
          Diastólica
          <input type="number" value={novaTriagem.pressaoDiastolica} onChange={(event) => setNovaTriagem({ ...novaTriagem, pressaoDiastolica: Number(event.target.value) })} />
        </label>
        <label>
          Batimentos
          <input type="number" value={novaTriagem.frequenciaCardiaca} onChange={(event) => setNovaTriagem({ ...novaTriagem, frequenciaCardiaca: Number(event.target.value) })} />
        </label>
        <label>
          Risco
          <select value={novaTriagem.classificacaoRisco} onChange={(event) => setNovaTriagem({ ...novaTriagem, classificacaoRisco: event.target.value })}>
            <option value="AZUL">Azul</option>
            <option value="VERDE">Verde</option>
            <option value="AMARELO">Amarelo</option>
            <option value="LARANJA">Laranja</option>
            <option value="VERMELHO">Vermelho</option>
          </select>
        </label>
        <label>
          Sintomas
          <input value={novaTriagem.sintomas} onChange={(event) => setNovaTriagem({ ...novaTriagem, sintomas: event.target.value })} />
        </label>
        <label>
          Observações
          <input value={novaTriagem.observacoes ?? ""} onChange={(event) => setNovaTriagem({ ...novaTriagem, observacoes: event.target.value })} />
        </label>
      </div>
      <button className="primary-action" onClick={onCriar} type="button">Salvar</button>
    </section>
  );
}

function AtendimentosPage({
  atendimentoCriado,
  novoAtendimento,
  onCriar,
  onFinalizar,
  setNovoAtendimento,
  status
}: {
  atendimentoCriado: Atendimento | null;
  novoAtendimento: CriarAtendimentoRequest;
  onCriar: () => void;
  onFinalizar: () => void;
  setNovoAtendimento: (value: CriarAtendimentoRequest) => void;
  status: string;
}) {
  return (
    <div className="work-grid">
      <section className="work-card wide">
        <CardTitle title="Registrar" status={status} />
        <div className="form-grid">
          <label>
            Check-in ID
            <input value={novoAtendimento.checkInId} onChange={(event) => setNovoAtendimento({ ...novoAtendimento, checkInId: event.target.value })} />
          </label>
          <label>
            Queixa
            <input value={novoAtendimento.queixa} onChange={(event) => setNovoAtendimento({ ...novoAtendimento, queixa: event.target.value })} />
          </label>
          <label>
            Hipótese
            <input value={novoAtendimento.hipoteseDiagnostica} onChange={(event) => setNovoAtendimento({ ...novoAtendimento, hipoteseDiagnostica: event.target.value })} />
          </label>
          <label>
            Conduta
            <input value={novoAtendimento.conduta} onChange={(event) => setNovoAtendimento({ ...novoAtendimento, conduta: event.target.value })} />
          </label>
          <label>
            Prescrição
            <input value={novoAtendimento.prescricao ?? ""} onChange={(event) => setNovoAtendimento({ ...novoAtendimento, prescricao: event.target.value })} />
          </label>
          <label>
            Encaminhamento
            <input value={novoAtendimento.encaminhamento ?? ""} onChange={(event) => setNovoAtendimento({ ...novoAtendimento, encaminhamento: event.target.value })} />
          </label>
        </div>
        <button className="primary-action" onClick={onCriar} type="button">Salvar</button>
      </section>

      {atendimentoCriado && (
        <section className="work-card">
          <CardTitle title="Aberto" />
          <p className="mono-id">{atendimentoCriado.id}</p>
          <button className="primary-action" disabled={Boolean(atendimentoCriado.finalizadoEm)} onClick={onFinalizar} type="button">
            Finalizar
          </button>
        </section>
      )}
    </div>
  );
}

function RelatoriosPage({
  fim,
  inicio,
  onCarregar,
  relatorios,
  setFim,
  setInicio,
  status
}: {
  fim: string;
  inicio: string;
  onCarregar: () => void;
  relatorios: ReportsState;
  setFim: (value: string) => void;
  setInicio: (value: string) => void;
  status: string;
}) {
  return (
    <div className="work-grid">
      <section className="work-card">
        <CardTitle title="Período" status={status} />
        <div className="form-grid two-columns">
          <label>
            Início
            <input type="date" value={inicio} onChange={(event) => setInicio(event.target.value)} />
          </label>
          <label>
            Fim
            <input type="date" value={fim} onChange={(event) => setFim(event.target.value)} />
          </label>
        </div>
        <button className="primary-action" onClick={onCarregar} type="button">Carregar</button>
      </section>

      <section className="work-card wide">
        <CardTitle title="Resultado" />
        <div className="stats-grid">
          <strong>{relatorios.atendimentos?.totalAtendimentos ?? 0}<span>Atendimentos</span></strong>
          <strong>{relatorios.atendimentos?.totalFinalizados ?? 0}<span>Finalizados</span></strong>
          <strong>{relatorios.cancelamentos?.totalCancelamentos ?? 0}<span>Cancelamentos</span></strong>
        </div>
        <SimpleTable
          columns={["Risco", "Total"]}
          rows={(relatorios.riscos?.itens ?? []).map((item) => [item.classificacaoRisco, String(item.total)])}
        />
      </section>
    </div>
  );
}

function AuditoriaPage({
  auditoria,
  onListar,
  status
}: {
  auditoria: LogAuditoria[];
  onListar: () => void;
  status: string;
}) {
  return (
    <section className="work-card wide">
      <CardTitle title="Registros" status={status} />
      <button className="secondary-action bordered" onClick={onListar} type="button">Atualizar</button>
      <SimpleTable
        columns={["Ação", "Entidade", "Usuário", "Data"]}
        rows={auditoria.map((item) => [
          item.acao,
          item.entidade,
          item.usuario,
          new Date(item.registradoEm).toLocaleString("pt-BR")
        ])}
      />
    </section>
  );
}

function CardTitle({ status, title }: { status?: string; title: string }) {
  return (
    <div className="card-title">
      <h2>{title}</h2>
      {status && <span>{status}</span>}
    </div>
  );
}

function SimpleTable({ columns, rows }: { columns: string[]; rows: string[][] }) {
  return (
    <div className="data-table" role="table">
      <div className="data-row header" role="row" style={{ gridTemplateColumns: `repeat(${columns.length}, minmax(140px, 1fr))` }}>
        {columns.map((column) => <span key={column}>{column}</span>)}
      </div>
      {rows.map((row, index) => (
        <div className="data-row" key={`${row.join("-")}-${index}`} role="row" style={{ gridTemplateColumns: `repeat(${columns.length}, minmax(140px, 1fr))` }}>
          {row.map((cell, cellIndex) => cellIndex === 0 ? <strong key={cellIndex}>{cell}</strong> : <span key={cellIndex}>{cell}</span>)}
        </div>
      ))}
      {rows.length === 0 && <div className="empty-row">Nada por aqui.</div>}
    </div>
  );
}
