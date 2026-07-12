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

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "/api";

export async function login(usuario: string, senha: string): Promise<LoginResponse> {
  const response = await fetch(`${API_BASE_URL}/auth/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({ usuario, senha })
  });

  if (!response.ok) {
    throw new Error("Nao foi possivel autenticar com essas credenciais.");
  }

  return response.json() as Promise<LoginResponse>;
}

export const demoUsers = [
  { usuario: "admin", senha: "admin123", papel: "ADMIN" },
  { usuario: "recepcao", senha: "recepcao123", papel: "RECEPCIONISTA" },
  { usuario: "enfermagem", senha: "enfermagem123", papel: "ENFERMEIRO" },
  { usuario: "medico", senha: "medico123", papel: "MEDICO" },
  { usuario: "gestao", senha: "gestao123", papel: "GESTOR" }
];
