import {
  Activity,
  AlertTriangle,
  ArrowRight,
  BarChart3,
  CalendarClock,
  ClipboardList,
  FileClock,
  HeartPulse,
  LayoutDashboard,
  ListChecks,
  LockKeyhole,
  LogIn,
  type LucideIcon,
  ShieldCheck,
  Stethoscope,
  UserRound,
  UsersRound
} from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import {
  criarPaciente,
  demoUsers,
  listarPacientes,
  login,
  type CriarPacienteRequest,
  type LoginResponse,
  type Paciente
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
  subtitle: string;
  icon: LucideIcon;
  roles: string[];
  status: string;
  endpoints: string[];
};

const modules: ModuleItem[] = [
  {
    key: "visao-geral",
    title: "Visao Geral",
    subtitle: "Painel de operacao da unidade",
    icon: LayoutDashboard,
    roles: ["Todos"],
    status: "Operacional",
    endpoints: ["/health", "/auth/login"]
  },
  {
    key: "pacientes",
    title: "Pacientes",
    subtitle: "Cadastro, busca e historico",
    icon: UserRound,
    roles: ["ADMIN", "RECEPCIONISTA", "ENFERMEIRO", "MEDICO", "GESTOR"],
    status: "Persistido",
    endpoints: ["GET /pacientes", "POST /pacientes", "GET /pacientes/{id}/historico"]
  },
  {
    key: "profissionais",
    title: "Profissionais",
    subtitle: "Equipe, papeis e disponibilidade",
    icon: UsersRound,
    roles: ["ADMIN", "GESTOR"],
    status: "Persistido",
    endpoints: ["GET /profissionais", "POST /profissionais"]
  },
  {
    key: "agenda",
    title: "Agenda",
    subtitle: "Marcacao, remarcacao e cancelamento",
    icon: CalendarClock,
    roles: ["ADMIN", "RECEPCIONISTA"],
    status: "Persistido",
    endpoints: ["GET /agendamentos", "POST /agendamentos", "PATCH /agendamentos/{id}/cancelar"]
  },
  {
    key: "fila",
    title: "Fila",
    subtitle: "Check-in e fluxo de atendimento",
    icon: ListChecks,
    roles: ["ADMIN", "RECEPCIONISTA", "ENFERMEIRO", "MEDICO", "GESTOR"],
    status: "Persistido",
    endpoints: ["POST /fila/check-ins", "GET /fila/hoje"]
  },
  {
    key: "triagem",
    title: "Triagem",
    subtitle: "Sinais vitais e risco automatico",
    icon: HeartPulse,
    roles: ["ADMIN", "ENFERMEIRO", "MEDICO"],
    status: "Persistido",
    endpoints: ["POST /triagens", "GET /triagens/{id}"]
  },
  {
    key: "atendimentos",
    title: "Atendimentos",
    subtitle: "Conduta, prescricao e fechamento",
    icon: Stethoscope,
    roles: ["ADMIN", "MEDICO"],
    status: "Persistido",
    endpoints: ["POST /atendimentos", "PATCH /atendimentos/{id}/finalizar"]
  },
  {
    key: "relatorios",
    title: "Relatorios",
    subtitle: "Indicadores operacionais",
    icon: BarChart3,
    roles: ["ADMIN", "GESTOR"],
    status: "Disponivel",
    endpoints: ["GET /relatorios/atendimentos", "GET /relatorios/classificacoes-risco"]
  },
  {
    key: "auditoria",
    title: "Auditoria",
    subtitle: "Logs de acoes criticas",
    icon: FileClock,
    roles: ["ADMIN"],
    status: "Persistido",
    endpoints: ["GET /auditoria"]
  }
];

const flowSteps = [
  { label: "Agenda", icon: CalendarClock },
  { label: "Check-in", icon: ClipboardList },
  { label: "Triagem", icon: HeartPulse },
  { label: "Atendimento", icon: Stethoscope },
  { label: "Historico", icon: FileClock }
];

const journeyStages: Array<{
  key: ModuleKey;
  label: string;
  description: string;
  metric: string;
  tone: string;
  icon: LucideIcon;
}> = [
  {
    key: "agenda",
    label: "Agendar",
    description: "Horarios, remarcacoes e faltas",
    metric: "12 marcados",
    tone: "Recepcao",
    icon: CalendarClock
  },
  {
    key: "fila",
    label: "Chegada",
    description: "Check-in e ordem de entrada",
    metric: "8 na fila",
    tone: "Porta aberta",
    icon: ClipboardList
  },
  {
    key: "triagem",
    label: "Triagem",
    description: "Sinais vitais e risco",
    metric: "3 prioridade",
    tone: "Enfermagem",
    icon: HeartPulse
  },
  {
    key: "atendimentos",
    label: "Consulta",
    description: "Conduta e fechamento",
    metric: "5 em curso",
    tone: "Medico",
    icon: Stethoscope
  },
  {
    key: "relatorios",
    label: "Gestao",
    description: "Indicadores da unidade",
    metric: "18 min espera",
    tone: "Gestor",
    icon: BarChart3
  }
];

export function App() {
  const [activeModule, setActiveModule] = useState<ModuleKey>("visao-geral");
  const [usuario, setUsuario] = useState("medico");
  const [senha, setSenha] = useState("medico123");
  const [session, setSession] = useState<LoginResponse | null>(null);
  const [loginStatus, setLoginStatus] = useState("Pronto para autenticar");
  const [pacientes, setPacientes] = useState<Paciente[]>([]);
  const [pacientesStatus, setPacientesStatus] = useState("Entre para carregar pacientes.");
  const [filtroNome, setFiltroNome] = useState("");
  const [filtroCpf, setFiltroCpf] = useState("");
  const [novoPaciente, setNovoPaciente] = useState<CriarPacienteRequest>({
    nome: "",
    cpf: "",
    dataNascimento: "",
    telefone: "",
    cns: ""
  });

  const selectedModule = useMemo(
    () => modules.find((module) => module.key === activeModule) ?? modules[0],
    [activeModule]
  );

  async function handleLogin() {
    setLoginStatus("Autenticando...");

    try {
      const response = await login(usuario, senha);
      setSession(response);
      setLoginStatus(`Sessao ativa para ${response.usuario.nome}`);
      setActiveModule("visao-geral");
    } catch (error) {
      setSession(null);
      setLoginStatus(error instanceof Error ? error.message : "Falha no login.");
    }
  }

  async function carregarPacientes() {
    if (!session) {
      setPacientesStatus("Faca login para carregar pacientes.");
      return;
    }

    setPacientesStatus("Carregando pacientes...");

    try {
      const response = await listarPacientes(session.token, {
        nome: filtroNome.trim(),
        cpf: filtroCpf.trim()
      });
      setPacientes(response.itens);
      setPacientesStatus(
        response.totalItens === 0
          ? "Nenhum paciente encontrado."
          : `${response.totalItens} paciente(s) encontrado(s).`
      );
    } catch (error) {
      setPacientesStatus(error instanceof Error ? error.message : "Falha ao carregar pacientes.");
    }
  }

  async function handleCriarPaciente() {
    if (!session) {
      setPacientesStatus("Faca login como ADMIN ou RECEPCIONISTA para cadastrar.");
      return;
    }

    setPacientesStatus("Salvando paciente...");

    try {
      await criarPaciente(session.token, {
        ...novoPaciente,
        cpf: novoPaciente.cpf.replace(/\D/g, ""),
        cns: novoPaciente.cns?.trim() || null
      });
      setNovoPaciente({ nome: "", cpf: "", dataNascimento: "", telefone: "", cns: "" });
      await carregarPacientes();
      setPacientesStatus("Paciente cadastrado com sucesso.");
    } catch (error) {
      setPacientesStatus(error instanceof Error ? error.message : "Falha ao cadastrar paciente.");
    }
  }

  useEffect(() => {
    if (session && activeModule === "pacientes") {
      void carregarPacientes();
    }
  }, [session, activeModule]);

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

  const VisibleIcon = selectedModule.icon;

  return (
    <main className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-mark">
            <Activity size={24} />
          </div>
          <div>
            <strong>UBSFlow</strong>
            <span>Gestao de fluxo clinico</span>
          </div>
        </div>

        <nav className="nav-list" aria-label="Modulos">
          {modules.map((module) => {
            const Icon = module.icon;
            const isActive = module.key === activeModule;

            return (
              <button
                className={isActive ? "nav-item active" : "nav-item"}
                key={module.key}
                onClick={() => setActiveModule(module.key)}
                title={module.title}
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
            <span>Acesso demo</span>
          </div>
          <label>
            Usuario
            <select value={usuario} onChange={(event) => {
              const selected = demoUsers.find((item) => item.usuario === event.target.value);
              setUsuario(event.target.value);
              setSenha(selected?.senha ?? "");
            }}>
              {demoUsers.map((user) => (
                <option key={user.usuario} value={user.usuario}>
                  {user.usuario} - {user.papel}
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

      <section className="content">
        <header className="command-header">
          <div>
            <div className="eyebrow">
              <ShieldCheck size={16} />
              Fluxo operacional da unidade
            </div>
            <h1>Central de atendimento</h1>
            <p>
              Cada area acompanha uma etapa da jornada do paciente, da agenda ao
              encerramento do atendimento.
            </p>
          </div>
          <div className="shift-card">
            <span>Plantao atual</span>
            <strong>{session ? session.usuario.papel : "Sem sessao"}</strong>
            <small>{session ? session.usuario.nome : "Entre para operar os modulos"}</small>
          </div>
        </header>

        <section className="journey-map" aria-label="Fluxo principal">
          {journeyStages.map((stage, index) => {
            const Icon = stage.icon;
            const isActive = stage.key === activeModule;

            return (
              <button
                className={isActive ? "journey-stage active" : "journey-stage"}
                key={stage.key}
                onClick={() => setActiveModule(stage.key)}
                type="button"
              >
                <span className="stage-index">{String(index + 1).padStart(2, "0")}</span>
                <span className="stage-icon">
                  <Icon size={20} />
                </span>
                <strong>{stage.label}</strong>
                <small>{stage.description}</small>
                <em>{stage.metric}</em>
                <span className="stage-tone">{stage.tone}</span>
              </button>
            );
          })}
        </section>

        <section className="metrics-grid" aria-label="Resumo operacional">
          <Metric label="Atendimentos hoje" value={String(Math.max(42, pacientes.length))} trend="operacao ativa" />
          <Metric label="Tempo medio espera" value="18 min" trend="fila monitorada" />
          <Metric label="Triagens pendentes" value="7" trend="risco em ordem" />
          <Metric label="Pacientes no cadastro" value={String(pacientes.length)} trend="dados persistidos" />
        </section>

        <section className="workspace">
          <div className="module-board">
            <div className="module-header">
              <div className="module-icon">
                <VisibleIcon size={24} />
              </div>
              <div>
                <span>{selectedModule.status}</span>
                <h2>{selectedModule.title}</h2>
                <p>{selectedModule.subtitle}</p>
              </div>
            </div>

            {activeModule === "pacientes" ? (
              <PacientesCrud
                filtroCpf={filtroCpf}
                filtroNome={filtroNome}
                novoPaciente={novoPaciente}
                onBuscar={carregarPacientes}
                onCriar={handleCriarPaciente}
                pacientes={pacientes}
                podeCriar={session?.usuario.papel === "ADMIN" || session?.usuario.papel === "RECEPCIONISTA"}
                setFiltroCpf={setFiltroCpf}
                setFiltroNome={setFiltroNome}
                setNovoPaciente={setNovoPaciente}
                status={pacientesStatus}
              />
            ) : activeModule === "visao-geral" ? (
              <OverviewPanel onNavigate={setActiveModule} pacientes={pacientes.length} />
            ) : (
              <ModuleDetails selectedModule={selectedModule} />
            )}
          </div>

          <div className="side-stack">
            <section className="status-card">
              <div>
                <AlertTriangle size={18} />
                <strong>Prioridade automatica</strong>
              </div>
              <p>
                A triagem combina sintomas, febre, pressao e classificacao informada
                para ordenar a fila por risco.
              </p>
            </section>

            <section className="status-card">
              <div>
                <ShieldCheck size={18} />
                <strong>RBAC ativo</strong>
              </div>
              <p>
                Cada modulo respeita papeis como recepcionista, enfermeiro, medico,
                gestor e admin.
              </p>
            </section>
          </div>
        </section>
      </section>
    </main>
  );
}

function Metric({ label, value, trend }: { label: string; value: string; trend: string }) {
  return (
    <article className="metric-card">
      <span>{label}</span>
      <strong>{value}</strong>
      <small>{trend}</small>
    </article>
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
  const selectedUser = demoUsers.find((item) => item.usuario === usuario) ?? demoUsers[0];

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
          <a href="#demo">Demo</a>
        </div>
      </nav>

      <section className="portfolio-hero">
        <div className="hero-copy">
          <span className="portfolio-kicker">Backend real. Interface de produto. Fluxo de UBS.</span>
          <h1>Uma API de clinica que parece um produto pronto para apresentar.</h1>
          <p>
            UBSFlow organiza pacientes, agenda, fila, triagem e atendimento em uma
            experiencia pensada para vender dominio de backend sem abrir mao de visual.
          </p>
          <div className="portfolio-actions">
            <a className="landing-primary" href="#demo">
              Entrar na demo
              <ArrowRight size={18} />
            </a>
            <a className="landing-secondary" href="#fluxo">
              Ver fluxo do produto
            </a>
          </div>
        </div>

        <div className="showcase-panel" aria-label="Preview do UBSFlow">
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
              <small>Operacao em movimento</small>
              <strong>Triagem prioritaria</strong>
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
                <b>Joao Lima</b>
                <small>Consulta medica - sala 2</small>
              </article>
              <article>
                <b>Ana Costa</b>
                <small>Check-in concluido</small>
              </article>
            </div>
          </div>
        </div>
      </section>

      <section className="media-ribbon" aria-label="Visao visual do produto">
        <img
          alt="Profissional de saude em atendimento"
          src="https://images.unsplash.com/photo-1551076805-e1869033e561?auto=format&fit=crop&w=900&q=80"
        />
        <img
          alt="Corredor de unidade de saude"
          src="https://images.unsplash.com/photo-1519494026892-80bbd2d6fd0d?auto=format&fit=crop&w=900&q=80"
        />
        <img
          alt="Equipe analisando dados clinicos"
          src="https://images.unsplash.com/photo-1576091160399-112ba8d25d1d?auto=format&fit=crop&w=900&q=80"
        />
        <div>
          <span>Visual + backend</span>
          <strong>Uma demo para prender o olhar antes do Swagger.</strong>
        </div>
      </section>

      <section className="portfolio-section" id="produto">
        <div className="section-heading">
          <span>Por que existe</span>
          <h2>O fluxo inteiro, embalado como produto.</h2>
        </div>
        <div className="feature-reel">
          <article>
            <UserRound size={24} />
            <strong>Paciente</strong>
            <p>Cadastro, busca e historico entram como porta de entrada do atendimento.</p>
          </article>
          <article>
            <HeartPulse size={24} />
            <strong>Prioridade</strong>
            <p>Triagem, sinais vitais e risco mostram regra de negocio, nao tela decorativa.</p>
          </article>
          <article>
            <ShieldCheck size={24} />
            <strong>Controle</strong>
            <p>RBAC, auditoria e persistencia fecham a historia tecnica do projeto.</p>
          </article>
        </div>
      </section>

      <section className="story-section" id="fluxo">
        <div className="section-heading">
          <span>Jornada</span>
          <h2>Da chegada ao fechamento, cada etapa conversa com a proxima.</h2>
        </div>
        <div className="story-track">
          {journeyStages.map((stage, index) => {
            const Icon = stage.icon;
            return (
              <article key={stage.key}>
                <small>{String(index + 1).padStart(2, "0")}</small>
                <Icon size={24} />
                <strong>{stage.label}</strong>
                <p>{stage.description}</p>
              </article>
            );
          })}
        </div>
      </section>

      <section className="demo-section" id="demo">
        <div className="demo-copy">
          <span>Demo navegavel</span>
          <h2>Escolha um papel e entre no sistema operacional.</h2>
          <p>
            Use a conta de recepcao para testar cadastro de pacientes ou entre como
            medico, gestor, enfermagem e admin para visualizar a experiencia por perfil.
          </p>
        </div>

        <div className="landing-login">
          <div className="session-heading">
            <LockKeyhole size={16} />
            <span>Acesso demo</span>
          </div>
          <label>
            Usuario
            <select value={usuario} onChange={(event) => selecionarUsuario(event.target.value)}>
              {demoUsers.map((user) => (
                <option key={user.usuario} value={user.usuario}>
                  {user.usuario} - {user.papel}
                </option>
              ))}
            </select>
          </label>
          <label>
            Senha
            <input value={senha} onChange={(event) => setSenha(event.target.value)} type="password" />
          </label>
          <button className="landing-primary full" onClick={onLogin} type="button">
            Entrar como {selectedUser.papel}
            <LogIn size={17} />
          </button>
          <p>{loginStatus}</p>
        </div>
      </section>
    </main>
  );
}

function ModuleDetails({ selectedModule }: { selectedModule: ModuleItem }) {
  return (
    <>
      <div className="flow-strip">
        {flowSteps.map((step, index) => {
          const Icon = step.icon;
          return (
            <div className="flow-step" key={step.label}>
              <Icon size={18} />
              <span>{step.label}</span>
              {index < flowSteps.length - 1 && <ArrowRight size={16} />}
            </div>
          );
        })}
      </div>

      <div className="details-grid">
        <section>
          <h3>Permissoes</h3>
          <div className="role-list">
            {selectedModule.roles.map((role) => (
              <span key={role}>{role}</span>
            ))}
          </div>
        </section>
        <section>
          <h3>Endpoints</h3>
          <ul className="endpoint-list">
            {selectedModule.endpoints.map((endpoint) => (
              <li key={endpoint}>{endpoint}</li>
            ))}
          </ul>
        </section>
      </div>
    </>
  );
}

function OverviewPanel({
  onNavigate,
  pacientes
}: {
  onNavigate: (module: ModuleKey) => void;
  pacientes: number;
}) {
  return (
    <div className="overview-grid">
      <section className="flow-board">
        <div className="crud-heading">
          <div>
            <h3>Fluxo do dia</h3>
            <p>Acompanhamento das etapas que mantem a UBS em movimento.</p>
          </div>
          <button className="primary-action" onClick={() => onNavigate("fila")} type="button">
            Abrir fila
            <ArrowRight size={16} />
          </button>
        </div>

        <div className="queue-lanes">
          <FlowLane label="Agendados" value="12" accent="blue" />
          <FlowLane label="Aguardando triagem" value="7" accent="amber" />
          <FlowLane label="Em atendimento" value="5" accent="green" />
          <FlowLane label="Finalizados" value="18" accent="slate" />
        </div>
      </section>

      <section className="quick-actions">
        <button onClick={() => onNavigate("pacientes")} type="button">
          <UserRound size={20} />
          <span>
            <strong>Cadastrar paciente</strong>
            <small>{pacientes} paciente(s) visiveis na sessao</small>
          </span>
        </button>
        <button onClick={() => onNavigate("agenda")} type="button">
          <CalendarClock size={20} />
          <span>
            <strong>Organizar agenda</strong>
            <small>Horarios e cancelamentos</small>
          </span>
        </button>
        <button onClick={() => onNavigate("triagem")} type="button">
          <HeartPulse size={20} />
          <span>
            <strong>Priorizar atendimento</strong>
            <small>Classificacao de risco</small>
          </span>
        </button>
      </section>
    </div>
  );
}

function FlowLane({
  accent,
  label,
  value
}: {
  accent: "blue" | "amber" | "green" | "slate";
  label: string;
  value: string;
}) {
  return (
    <article className={`flow-lane ${accent}`}>
      <span>{label}</span>
      <strong>{value}</strong>
    </article>
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
    <div className="crud-stack">
      <section className="crud-panel">
        <div className="crud-heading">
          <div>
            <h3>Buscar pacientes</h3>
            <p>Localize cadastros antes de abrir a fila ou agendar retorno.</p>
          </div>
          <button className="secondary-action bordered" onClick={onBuscar} type="button">
            Atualizar
          </button>
        </div>

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
      </section>

      <section className="crud-panel">
        <div className="crud-heading">
          <div>
            <h3>Cadastrar paciente</h3>
            <p>Entrada inicial para recepcao e administracao da unidade.</p>
          </div>
          <button className="primary-action" disabled={!podeCriar} onClick={onCriar} type="button">
            Salvar
          </button>
        </div>

        <div className="form-grid">
          <label>
            Nome
            <input
              value={novoPaciente.nome}
              onChange={(event) => setNovoPaciente({ ...novoPaciente, nome: event.target.value })}
            />
          </label>
          <label>
            CPF
            <input
              maxLength={11}
              value={novoPaciente.cpf}
              onChange={(event) => setNovoPaciente({ ...novoPaciente, cpf: event.target.value })}
            />
          </label>
          <label>
            Nascimento
            <input
              type="date"
              value={novoPaciente.dataNascimento}
              onChange={(event) => setNovoPaciente({ ...novoPaciente, dataNascimento: event.target.value })}
            />
          </label>
          <label>
            Telefone
            <input
              value={novoPaciente.telefone}
              onChange={(event) => setNovoPaciente({ ...novoPaciente, telefone: event.target.value })}
            />
          </label>
          <label>
            CNS
            <input
              value={novoPaciente.cns ?? ""}
              onChange={(event) => setNovoPaciente({ ...novoPaciente, cns: event.target.value })}
            />
          </label>
        </div>
      </section>

      <section className="crud-panel">
        <div className="crud-heading">
          <div>
            <h3>Pacientes cadastrados</h3>
            <p>{status}</p>
          </div>
        </div>

        <div className="data-table" role="table" aria-label="Pacientes cadastrados">
          <div className="data-row header" role="row">
            <span>Nome</span>
            <span>CPF</span>
            <span>Nascimento</span>
            <span>Telefone</span>
            <span>CNS</span>
          </div>
          {pacientes.map((paciente) => (
            <div className="data-row" key={paciente.id} role="row">
              <strong>{paciente.nome}</strong>
              <span>{paciente.cpf}</span>
              <span>{paciente.dataNascimento}</span>
              <span>{paciente.telefone}</span>
              <span>{paciente.cns ?? "-"}</span>
            </div>
          ))}
        </div>
      </section>
    </div>
  );
}
