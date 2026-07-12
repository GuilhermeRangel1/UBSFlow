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
import { useMemo, useState } from "react";
import { demoUsers, login, type LoginResponse } from "./api/ubsflowApi";

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

export function App() {
  const [activeModule, setActiveModule] = useState<ModuleKey>("visao-geral");
  const [usuario, setUsuario] = useState("medico");
  const [senha, setSenha] = useState("medico123");
  const [session, setSession] = useState<LoginResponse | null>(null);
  const [loginStatus, setLoginStatus] = useState("Pronto para autenticar");

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
    } catch (error) {
      setSession(null);
      setLoginStatus(error instanceof Error ? error.message : "Falha no login.");
    }
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
        <header className="hero">
          <div className="hero-background" />
          <div className="hero-content">
            <div className="eyebrow">
              <ShieldCheck size={16} />
              API REST .NET + PostgreSQL + RBAC
            </div>
            <h1>UBSFlow</h1>
            <p>
              Uma interface operacional para acompanhar o fluxo real de atendimento:
              agenda, chegada, triagem, consulta, historico e auditoria em uma UBS.
            </p>
            <div className="hero-actions">
              <button className="primary-action" onClick={() => setActiveModule("fila")} type="button">
                Ver fila
                <ArrowRight size={16} />
              </button>
              <button className="secondary-action" onClick={() => setActiveModule("relatorios")} type="button">
                Indicadores
              </button>
            </div>
          </div>
        </header>

        <section className="metrics-grid" aria-label="Resumo operacional">
          <Metric label="Atendimentos hoje" value="42" trend="+12%" />
          <Metric label="Tempo medio espera" value="18 min" trend="-6 min" />
          <Metric label="Triagens pendentes" value="7" trend="risco monitorado" />
          <Metric label="Cancelamentos" value="3" trend="motivo obrigatorio" />
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
