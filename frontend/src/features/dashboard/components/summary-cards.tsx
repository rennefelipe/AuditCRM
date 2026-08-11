import {
  CalendarCheck2,
  CheckCircle2,
  CircleAlert,
  Clock3,
  Store,
  Workflow,
} from "lucide-react";

import type { DashboardSummary } from "../dashboard-types";

type SummaryCardsProps = {
  data: DashboardSummary;
};

const cards = [
  {
    key: "totalProcesses",
    label: "Total de processos",
    icon: Store,
  },
  {
    key: "openProcesses",
    label: "Em aberto",
    icon: Workflow,
  },
  {
    key: "installed",
    label: "Instaladas",
    icon: CheckCircle2,
  },
  {
    key: "scheduledAppointments",
    label: "Agendadas",
    icon: CalendarCheck2,
  },
  {
    key: "overdueActions",
    label: "Ações atrasadas",
    icon: CircleAlert,
  },
  {
    key: "dueToday",
    label: "Ações para hoje",
    icon: Clock3,
  },
] as const;

export function SummaryCards({
  data,
}: SummaryCardsProps) {
  return (
    <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-6">
      {cards.map((card) => {
        const Icon = card.icon;

        return (
          <div
            key={card.key}
            className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
          >
            <div className="flex items-center justify-between">
              <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-[#007dd8]/10 text-[#007dd8]">
                <Icon className="h-5 w-5" />
              </div>

              <span className="text-3xl font-bold tracking-tight text-[#00213d]">
                {data[card.key]}
              </span>
            </div>

            <div className="mt-4 text-sm font-medium text-slate-500">
              {card.label}
            </div>
          </div>
        );
      })}
    </div>
  );
}