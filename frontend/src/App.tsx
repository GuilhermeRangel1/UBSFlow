import {
  Activity,
  ArrowRight,
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
  icon: typeof Activity;
  roles: string[];
};

type ModuleExperience = {
  eyebrow: string;
  title: string;
  description: string;
  image: string;
  primary: string;
  secondary: string;
  focus: string;
  caption: string;
  steps: string[];
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
  {
    key: "visao-geral",
    title: "Início",
    subtitle: "Entrada da plataforma",
    icon: LayoutDashboard,
    roles: ["Todos"]
  },
  {
    key: "pacientes",
    title: "Pacientes",
    subtitle: "Cadastro, busca e histórico",
    icon: UserRound,
    roles: ["ADMIN", "RECEPCIONISTA", "ENFERMEIRO", "MEDICO", "GESTOR"]
  },
  {
    key: "profissionais",
    title: "Profissionais",
    subtitle: "Equipe e disponibilidade",
    icon: UsersRound,
    roles: ["ADMIN", "GESTOR"]
  },
  {
    key: "agenda",
    title: "Agenda",
    subtitle: "Marcação e remarcação",
    icon: CalendarClock,
    roles: ["ADMIN", "RECEPCIONISTA"]
  },
  {
    key: "fila",
    title: "Fila",
    subtitle: "Chegada e ordem de atendimento",
    icon: ClipboardList,
    roles: ["ADMIN", "RECEPCIONISTA", "ENFERMEIRO", "MEDICO", "GESTOR"]
  },
  {
    key: "triagem",
    title: "Triagem",
    subtitle: "Sinais vitais e risco",
    icon: HeartPulse,
    roles: ["ADMIN", "ENFERMEIRO", "MEDICO"]
  },
  {
    key: "atendimentos",
    title: "Atendimentos",
    subtitle: "Consulta e fechamento",
    icon: Stethoscope,
    roles: ["ADMIN", "MEDICO"]
  },
  {
    key: "relatorios",
    title: "Relatórios",
    subtitle: "Indicadores da unidade",
    icon: BarChart3,
    roles: ["ADMIN", "GESTOR"]
  },
  {
    key: "auditoria",
    title: "Auditoria",
    subtitle: "Histórico de alterações",
    icon: FileClock,
    roles: ["ADMIN"]
  }
];

const moduleExperiences: Record<ModuleKey, ModuleExperience> = {
  "visao-geral": {
    eyebrow: "UBSFlow",
    title: "Uma central simples para conduzir o atendimento do começo ao fim.",
    description:
      "Escolha a área, acompanhe a jornada do paciente e mantenha a unidade funcionando com menos ruído.",
    image: "https://images.unsplash.com/photo-1586773860418-d37222d8fce3?auto=format&fit=crop&w=1600&q=85",
    primary: "Abrir pacientes",
    secondary: "Ver agenda",
    focus: "Fluxo conectado",
    caption: "Agenda, chegada, triagem, consulta e gestão em uma experiência contínua.",
    steps: ["Agenda", "Chegada", "Triagem", "Consulta", "Gestão"]
  },
  pacientes: {
    eyebrow: "Pacientes",
    title: "Cadastro rápido, busca clara e prontuário sempre à mão.",
    description:
      "A porta de entrada da unidade fica organizada para recepção, enfermagem, médicos e gestão.",
    image: "https://images.unsplash.com/photo-1576765607924-7f3bb5b3359f?auto=format&fit=crop&w=1600&q=85",
    primary: "Cadastrar paciente",
    secondary: "Buscar cadastro",
    focus: "Identificação segura",
    caption: "CPF, CNS, contato e nascimento ajudam a evitar duplicidade no atendimento.",
    steps: ["Buscar", "Cadastrar", "Atualizar", "Acompanhar"]
  },
  profissionais: {
    eyebrow: "Profissionais",
    title: "Equipe organizada por função, especialidade e disponibilidade.",
    description:
      "A gestão visualiza quem atende, quando atende e qual papel cada pessoa ocupa na rotina.",
    image: "https://images.unsplash.com/photo-1559757175-0eb30cd8c063?auto=format&fit=crop&w=1600&q=85",
    primary: "Adicionar profissional",
    secondary: "Ver disponibilidade",
    focus: "Equipe alinhada",
    caption: "Escala, registro profissional e especialidade ficam em um único lugar.",
    steps: ["Perfil", "Registro", "Escala", "Disponibilidade"]
  },
  agenda: {
    eyebrow: "Agenda",
    title: "Horários claros para marcar, remarcar e cancelar com motivo.",
    description:
      "A recepção encontra a melhor janela sem criar conflitos para o mesmo profissional.",
    image: "https://images.unsplash.com/photo-1579684385127-1ef15d508118?auto=format&fit=crop&w=1600&q=85",
    primary: "Marcar consulta",
    secondary: "Remarcar horário",
    focus: "Agenda sem conflito",
    caption: "A unidade evita dois atendimentos no mesmo horário para a mesma pessoa da equipe.",
    steps: ["Paciente", "Profissional", "Horário", "Confirmação"]
  },
  fila: {
    eyebrow: "Fila",
    title: "A chegada vira uma fila simples, visível e ordenada.",
    description:
      "Depois do check-in, cada paciente segue para triagem e consulta com prioridade bem definida.",
    image: "https://images.unsplash.com/photo-1512678080530-7760d81faba6?auto=format&fit=crop&w=1600&q=85",
    primary: "Registrar chegada",
    secondary: "Abrir fila do dia",
    focus: "Tempo sob controle",
    caption: "O fluxo guarda os horários de chegada, triagem e atendimento para medir espera.",
    steps: ["Chegada", "Fila", "Triagem", "Consulta"]
  },
  triagem: {
    eyebrow: "Triagem",
    title: "Sinais vitais e sintomas ajudam a ordenar quem precisa passar antes.",
    description:
      "A classificação de risco combina dados informados pela enfermagem e critérios da unidade.",
    image: "https://images.unsplash.com/photo-1581056771107-24ca5f033842?auto=format&fit=crop&w=1600&q=85",
    primary: "Iniciar triagem",
    secondary: "Ver prioridades",
    focus: "Prioridade automática",
    caption: "Idade, febre, pressão, sintomas e risco informado ajustam a posição na fila.",
    steps: ["Sinais vitais", "Sintomas", "Risco", "Prioridade"]
  },
  atendimentos: {
    eyebrow: "Atendimentos",
    title: "A consulta registra queixa, conduta, prescrição e retorno.",
    description:
      "O médico fecha o atendimento com histórico consistente para a próxima passagem do paciente.",
    image: "https://images.unsplash.com/photo-1537368910025-700350fe46c7?auto=format&fit=crop&w=1600&q=85",
    primary: "Registrar consulta",
    secondary: "Finalizar atendimento",
    focus: "Histórico preservado",
    caption: "Atendimentos finalizados ficam protegidos para manter a linha clínica do paciente.",
    steps: ["Queixa", "Conduta", "Prescrição", "Fechamento"]
  },
  relatorios: {
    eyebrow: "Relatórios",
    title: "Indicadores para entender espera, volume, risco e produtividade.",
    description:
      "A gestão enxerga o comportamento da unidade por período, profissional e classificação.",
    image: "https://images.unsplash.com/photo-1576091160550-2173dba999ef?auto=format&fit=crop&w=1600&q=85",
    primary: "Ver indicadores",
    secondary: "Filtrar período",
    focus: "Decisão mais clara",
    caption: "Tempo médio de espera, faltas, cancelamentos e risco aparecem sem garimpar planilha.",
    steps: ["Período", "Unidade", "Profissional", "Resultado"]
  },
  auditoria: {
    eyebrow: "Auditoria",
    title: "Ações importantes ficam registradas com autoria e contexto.",
    description:
      "Alterações em cadastros, cancelamentos e fechamentos podem ser acompanhados pela administração.",
    image: "https://images.unsplash.com/photo-1450101499163-c8848c66ca85?auto=format&fit=crop&w=1600&q=85",
    primary: "Ver histórico",
    secondary: "Filtrar ações",
    focus: "Rastreio confiável",
    caption: "A unidade sabe quem alterou, quando alterou e por que a mudança aconteceu.",
    steps: ["Ação", "Responsável", "Data", "Motivo"]
  }
};

const journeyStages: Array<{
  key: ModuleKey;
  label: string;
  description: string;
  icon: typeof Activity;
}> = [
  { key: "agenda", label: "Agendar", description: "Horários e retornos", icon: CalendarClock },
  { key: "fila", label: "Chegada", description: "Entrada do paciente", icon: ClipboardList },
  { key: "triagem", label: "Triagem", description: "Sinais vitais e risco", icon: HeartPulse },
  { key: "atendimentos", label: "Consulta", description: "Conduta e fechamento", icon: Stethoscope },
  { key: "relatorios", label: "Gestão", description: "Indicadores da unidade", icon: BarChart3 }
];

export function App() {
  const [activeModule, setActiveModule] = useState<ModuleKey>("visao-geral");
  const [usuario, setUsuario] = useState("medico");
  const [senha, setSenha] = useState("medico123");
  const [session, setSession] = useState<LoginResponse | null>(null);
  const [loginStatus, setLoginStatus] = useState("Escolha sua área para continuar.");
  const [pacientes, setPacientes] = useState<Paciente[]>([]);
  const [pacientesStatus, setPacientesStatus] = useState("Entre para visualizar pacientes.");
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
    if (!session) {
      setPacientesStatus("Faça login para carregar pacientes.");
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
      setPacientesStatus("Entre pela recepção ou administração para cadastrar.");
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

  return (
    <main className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-mark">
            <Activity size={24} />
          </div>
          <div>
            <strong>UBSFlow</strong>
            <span>Gestão do cuidado</span>
          </div>
        </div>

        <nav className="nav-list" aria-label="Áreas da plataforma">
          {modules.map((module) => {
            const Icon = module.icon;
            const isActive = module.key === activeModule;

            return (
              <button
                className={isActive ? "nav-item active" : "nav-item"}
                key={module.key}
                onClick={() => setActiveModule(module.key)}
                title={module.subtitle}
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
        {activeModule === "visao-geral" ? (
          <HomeScreen
            currentRole={roleLabels[session.usuario.papel] ?? session.usuario.papel}
            onNavigate={setActiveModule}
            userName={session.usuario.nome}
          />
        ) : activeModule === "pacientes" ? (
          <PatientsPage
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
        ) : (
          <ModuleExperienceScreen
            module={selectedModule}
            onNavigate={setActiveModule}
            roleLabel={roleLabels[session.usuario.papel] ?? session.usuario.papel}
          />
        )}
      </section>
    </main>
  );
}

function HomeScreen({
  currentRole,
  onNavigate,
  userName
}: {
  currentRole: string;
  onNavigate: (module: ModuleKey) => void;
  userName: string;
}) {
  return (
    <div className="app-page home-page">
      <section className="page-hero home-hero">
        <div className="page-hero-copy">
          <span className="page-kicker">Bem-vindo, {userName}</span>
          <h1>O atendimento da unidade em uma jornada visual e direta.</h1>
          <p>
            Você está na área de {currentRole}. Entre em uma etapa para cuidar do fluxo sem
            carregar a tela com informação que não precisa estar aqui.
          </p>
          <div className="hero-action-row">
            <button className="landing-primary" onClick={() => onNavigate("pacientes")} type="button">
              Pacientes
              <ArrowRight size={18} />
            </button>
            <button className="landing-secondary" onClick={() => onNavigate("agenda")} type="button">
              Agenda
            </button>
          </div>
        </div>

        <div className="home-visual">
          <img
            alt="Equipe de saúde acompanhando atendimento"
            src="https://images.unsplash.com/photo-1550831107-1553da8c8464?auto=format&fit=crop&w=1200&q=85"
          />
          <div className="home-visual-card">
            <span>Agora</span>
            <strong>Fluxo conectado</strong>
            <small>Da chegada ao fechamento</small>
          </div>
        </div>
      </section>

      <section className="route-showcase" aria-label="Principais áreas">
        <button onClick={() => onNavigate("fila")} type="button">
          <span>01</span>
          <strong>Receber pacientes</strong>
          <small>Chegada, fila e encaminhamento</small>
        </button>
        <button onClick={() => onNavigate("triagem")} type="button">
          <span>02</span>
          <strong>Priorizar atendimento</strong>
          <small>Sinais vitais, sintomas e risco</small>
        </button>
        <button onClick={() => onNavigate("atendimentos")} type="button">
          <span>03</span>
          <strong>Fechar consulta</strong>
          <small>Conduta, prescrição e retorno</small>
        </button>
      </section>
    </div>
  );
}

function PatientsPage(props: {
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
  const experience = moduleExperiences.pacientes;

  return (
    <div className="app-page patients-page">
      <section className="page-hero compact-hero">
        <div className="page-hero-copy">
          <span className="page-kicker">{experience.eyebrow}</span>
          <h1>{experience.title}</h1>
          <p>{experience.description}</p>
        </div>
        <div className="rounded-photo">
          <img alt="Atendimento ao paciente" src={experience.image} />
        </div>
      </section>

      <PacientesCrud {...props} />
    </div>
  );
}

function ModuleExperienceScreen({
  module,
  onNavigate,
  roleLabel
}: {
  module: ModuleItem;
  onNavigate: (module: ModuleKey) => void;
  roleLabel: string;
}) {
  const experience = moduleExperiences[module.key];
  const Icon = module.icon;

  return (
    <div className="app-page">
      <section className="page-hero feature-hero">
        <div className="page-hero-copy">
          <span className="page-kicker">{experience.eyebrow}</span>
          <h1>{experience.title}</h1>
          <p>{experience.description}</p>
          <div className="hero-action-row">
            <button className="landing-primary" type="button">
              {experience.primary}
              <ArrowRight size={18} />
            </button>
            <button className="landing-secondary" type="button">
              {experience.secondary}
            </button>
          </div>
        </div>

        <div className="feature-media">
          <img alt={module.title} src={experience.image} />
          <div className="feature-caption">
            <Icon size={22} />
            <strong>{experience.focus}</strong>
            <small>{experience.caption}</small>
          </div>
        </div>
      </section>

      <section className="module-stage">
        <div className="step-ribbon">
          {experience.steps.map((step, index) => (
            <article key={step}>
              <span>{String(index + 1).padStart(2, "0")}</span>
              <strong>{step}</strong>
            </article>
          ))}
        </div>

        <ExperienceConsole moduleKey={module.key} roleLabel={roleLabel} />

        <div className="module-access">
          <ShieldCheck size={18} />
          <span>
            Área disponível para{" "}
            {module.roles.map((role) => roleLabels[role] ?? role).join(", ")}.
          </span>
          <button onClick={() => onNavigate("visao-geral")} type="button">
            Voltar ao início
          </button>
        </div>
      </section>
    </div>
  );
}

function ExperienceConsole({ moduleKey, roleLabel }: { moduleKey: ModuleKey; roleLabel: string }) {
  if (moduleKey === "profissionais") {
    return (
      <section className="experience-console split-console">
        <div>
          <span className="console-kicker">Equipe</span>
          <h2>Novo profissional</h2>
        </div>
        <div className="form-grid">
          <label>
            Nome
            <input placeholder="Nome completo" />
          </label>
          <label>
            Especialidade
            <input placeholder="Clínica médica" />
          </label>
          <label>
            Registro
            <input placeholder="CRM ou COREN" />
          </label>
        </div>
        <button className="primary-action" type="button">Salvar profissional</button>
      </section>
    );
  }

  if (moduleKey === "agenda") {
    return (
      <section className="experience-console split-console">
        <div>
          <span className="console-kicker">Agenda</span>
          <h2>Marcar consulta</h2>
        </div>
        <div className="form-grid">
          <label>
            Paciente
            <input placeholder="Buscar paciente" />
          </label>
          <label>
            Data
            <input type="date" />
          </label>
          <label>
            Horário
            <input type="time" />
          </label>
        </div>
        <button className="primary-action" type="button">Confirmar horário</button>
      </section>
    );
  }

  if (moduleKey === "fila") {
    return (
      <section className="experience-console queue-console">
        <div>
          <span className="console-kicker">Fila de hoje</span>
          <h2>Chegada registrada, próximo passo visível.</h2>
        </div>
        <div className="queue-preview">
          <article>
            <strong>Maria Souza</strong>
            <span>Triagem prioritária</span>
          </article>
          <article>
            <strong>João Lima</strong>
            <span>Aguardando consulta</span>
          </article>
          <article>
            <strong>Ana Costa</strong>
            <span>Check-in concluído</span>
          </article>
        </div>
        <button className="primary-action" type="button">Registrar chegada</button>
      </section>
    );
  }

  if (moduleKey === "triagem") {
    return (
      <section className="experience-console split-console">
        <div>
          <span className="console-kicker">Enfermagem</span>
          <h2>Classificar risco</h2>
        </div>
        <div className="form-grid">
          <label>
            Temperatura
            <input placeholder="38,2" />
          </label>
          <label>
            Pressão
            <input placeholder="140/90" />
          </label>
          <label>
            Sintomas
            <input placeholder="Dor, febre, tontura" />
          </label>
        </div>
        <button className="primary-action" type="button">Gerar prioridade</button>
      </section>
    );
  }

  if (moduleKey === "atendimentos") {
    return (
      <section className="experience-console split-console">
        <div>
          <span className="console-kicker">{roleLabel}</span>
          <h2>Registro da consulta</h2>
        </div>
        <div className="form-grid">
          <label>
            Queixa
            <input placeholder="Motivo da consulta" />
          </label>
          <label>
            Conduta
            <input placeholder="Orientação e prescrição" />
          </label>
          <label>
            Retorno
            <input placeholder="Quando necessário" />
          </label>
        </div>
        <button className="primary-action" type="button">Finalizar atendimento</button>
      </section>
    );
  }

  if (moduleKey === "relatorios") {
    return (
      <section className="experience-console report-console">
        <div>
          <span className="console-kicker">Gestão</span>
          <h2>Resumo da unidade</h2>
        </div>
        <div className="bars-preview" aria-label="Resumo visual">
          <span style={{ height: "72%" }} />
          <span style={{ height: "44%" }} />
          <span style={{ height: "88%" }} />
          <span style={{ height: "58%" }} />
          <span style={{ height: "66%" }} />
        </div>
        <button className="primary-action" type="button">Aplicar filtro</button>
      </section>
    );
  }

  return (
    <section className="experience-console audit-console">
      <div>
        <span className="console-kicker">Auditoria</span>
        <h2>Últimas alterações</h2>
      </div>
      <div className="audit-lines">
        <span>Cadastro atualizado pela recepção</span>
        <span>Consulta finalizada pelo médico</span>
        <span>Cancelamento registrado com motivo</span>
      </div>
      <button className="primary-action" type="button">Filtrar histórico</button>
    </section>
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
            <a className="landing-primary" href="#acesso">
              Entrar
              <ArrowRight size={18} />
            </a>
            <a className="landing-secondary" href="#fluxo">
              Ver fluxo
            </a>
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

      <section className="media-ribbon" aria-label="Visão visual do produto">
        <img
          alt="Profissional de saúde em atendimento"
          src="https://images.unsplash.com/photo-1551076805-e1869033e561?auto=format&fit=crop&w=900&q=80"
        />
        <img
          alt="Corredor de unidade de saúde"
          src="https://images.unsplash.com/photo-1519494026892-80bbd2d6fd0d?auto=format&fit=crop&w=900&q=80"
        />
        <img
          alt="Equipe analisando dados clínicos"
          src="https://images.unsplash.com/photo-1576091160399-112ba8d25d1d?auto=format&fit=crop&w=900&q=80"
        />
        <div>
          <span>Cuidado conectado</span>
          <strong>Uma experiência clara para operar a rotina da unidade.</strong>
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
            <p>Triagem, sinais vitais e risco mostram uma rotina real, não tela decorativa.</p>
          </article>
          <article>
            <ShieldCheck size={24} />
            <strong>Controle</strong>
            <p>Cada equipe atua no seu espaço, com segurança e rastreio das mudanças.</p>
          </article>
        </div>
      </section>

      <section className="story-section" id="fluxo">
        <div className="section-heading">
          <span>Jornada</span>
          <h2>Da chegada ao fechamento, cada etapa conversa com a próxima.</h2>
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

      <section className="demo-section" id="acesso">
        <div className="demo-copy">
          <span>Acesso</span>
          <h2>Escolha sua área e entre na plataforma.</h2>
          <p>
            Cada perfil abre uma rotina diferente para acompanhar pacientes, organizar
            agenda, priorizar triagem e visualizar a unidade.
          </p>
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
    <div className="crud-stack">
      <section className="crud-panel search-panel">
        <div className="crud-heading">
          <div>
            <h3>Buscar pacientes</h3>
            <p>Localize o cadastro antes de abrir fila, retorno ou atendimento.</p>
          </div>
          <button className="secondary-action bordered" onClick={onBuscar} type="button">
            <Search size={16} />
            Buscar
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
            <p>Entrada inicial para recepção e administração da unidade.</p>
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
