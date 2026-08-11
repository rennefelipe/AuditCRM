export const STORE_PROCESS_STATUS = {
  1: {
    label: "Serviço",
    className:
      "border-slate-200 bg-slate-100 text-slate-700",
  },
  2: {
    label: "Instalado",
    className:
      "border-emerald-200 bg-emerald-50 text-emerald-700",
  },
  3: {
    label: "Em tratativas",
    className:
      "border-blue-200 bg-blue-50 text-blue-700",
  },
  4: {
    label: "Notificado sem retorno",
    className:
      "border-amber-200 bg-amber-50 text-amber-700",
  },
  5: {
    label: "Não autorizado",
    className:
      "border-red-200 bg-red-50 text-red-700",
  },
  6: {
    label: "Saiu do shopping",
    className:
      "border-slate-300 bg-slate-100 text-slate-600",
  },
} as const;

export const PROCESS_PRIORITY = {
  1: {
    label: "Baixa",
    className:
      "border-slate-200 bg-slate-50 text-slate-600",
  },
  2: {
    label: "Normal",
    className:
      "border-blue-200 bg-blue-50 text-blue-700",
  },
  3: {
    label: "Alta",
    className:
      "border-orange-200 bg-orange-50 text-orange-700",
  },
  4: {
    label: "Urgente",
    className:
      "border-red-200 bg-red-50 text-red-700",
  },
} as const;

export function getStatusInfo(status: number) {
  return (
    STORE_PROCESS_STATUS[
      status as keyof typeof STORE_PROCESS_STATUS
    ] ?? {
      label: `Status ${status}`,
      className:
        "border-slate-200 bg-slate-50 text-slate-600",
    }
  );
}

export function getPriorityInfo(priority: number) {
  return (
    PROCESS_PRIORITY[
      priority as keyof typeof PROCESS_PRIORITY
    ] ?? {
      label: `Prioridade ${priority}`,
      className:
        "border-slate-200 bg-slate-50 text-slate-600",
    }
  );
}