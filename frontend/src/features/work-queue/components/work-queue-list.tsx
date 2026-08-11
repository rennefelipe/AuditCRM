import {
  CalendarClock,
  ChevronRight,
  CircleAlert,
  Store,
  UserRound,
} from "lucide-react";
import Link from "next/link";

import { Badge } from "@/components/ui/badge";

import {
  getPriorityInfo,
  getStatusInfo,
} from "../process-labels";
import type { WorkQueueItem } from "../work-queue-types";

type WorkQueueListProps = {
  items: WorkQueueItem[];
};

function formatDate(value: string | null) {
  if (!value) {
    return "Sem prazo";
  }

  return new Intl.DateTimeFormat("pt-BR", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(new Date(value));
}

function isOverdue(value: string | null) {
  if (!value) {
    return false;
  }

  return new Date(value).getTime() < Date.now();
}

export function WorkQueueList({
  items,
}: WorkQueueListProps) {
  if (items.length === 0) {
    return (
      <div className="rounded-2xl border border-slate-200 bg-white p-10 text-center shadow-sm">
        <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-slate-100">
          <Store className="h-5 w-5 text-slate-400" />
        </div>

        <h3 className="mt-4 text-base font-semibold text-[#00213d]">
          Nenhum processo encontrado
        </h3>

        <p className="mt-1 text-sm text-slate-500">
          Não existem lojas na fila para os filtros selecionados.
        </p>
      </div>
    );
  }

  return (
    <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
      <div className="divide-y divide-slate-100">
        {items.map((item) => {
          const overdue = isOverdue(
            item.nextActionAt,
          );

          const status = getStatusInfo(
            item.status,
          );

          const priority = getPriorityInfo(
            item.priority,
          );

          return (
            <Link
              key={item.storeProcessId}
              href={`/stores/${item.storeId}/processes/${item.storeProcessId}`}
              className="group block p-4 transition hover:bg-slate-50 sm:p-5"
            >
              <div className="flex gap-4">
                <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-[#007dd8]/10 text-[#007dd8]">
                  <Store className="h-5 w-5" />
                </div>

                <div className="min-w-0 flex-1">
                  <div className="flex flex-col gap-3 xl:flex-row xl:items-start xl:justify-between">
                    <div className="min-w-0">
                      <div className="flex flex-wrap items-center gap-2">
                        <h3 className="text-base font-semibold text-[#00213d] sm:text-lg">
                          {item.storeName}
                        </h3>

                        <Badge
                          variant="outline"
                          className={status.className}
                        >
                          {status.label}
                        </Badge>

                        <Badge
                          variant="outline"
                          className={priority.className}
                        >
                          {priority.label}
                        </Badge>

                        {overdue && (
                          <Badge
                            variant="outline"
                            className="gap-1 border-red-200 bg-red-50 text-red-700"
                          >
                            <CircleAlert className="h-3.5 w-3.5" />
                            Atrasada
                          </Badge>
                        )}
                      </div>

                      <p className="mt-1.5 text-sm text-slate-500">
                        {item.shoppingGroupName ??
                          "Sem rede"}
                        {" • "}
                        {item.shoppingName}
                      </p>
                    </div>

                    <div className="hidden items-center gap-2 text-sm font-semibold text-[#007dd8] xl:flex">
                      Abrir operação
                      <ChevronRight className="h-5 w-5 transition group-hover:translate-x-1" />
                    </div>
                  </div>

                  <div className="mt-5 grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
                    <div className="rounded-xl bg-slate-50 p-3">
                      <div className="text-xs font-semibold uppercase tracking-wide text-slate-400">
                        Próxima ação
                      </div>

                      <div className="mt-1.5 text-sm font-medium leading-5 text-slate-700">
                        {item.nextAction ??
                          "Sem próxima ação"}
                      </div>
                    </div>

                    <div className="rounded-xl bg-slate-50 p-3">
                      <div className="flex items-center gap-1.5 text-xs font-semibold uppercase tracking-wide text-slate-400">
                        <CalendarClock className="h-3.5 w-3.5" />
                        Prazo
                      </div>

                      <div
                        className={[
                          "mt-1.5 text-sm font-semibold",
                          overdue
                            ? "text-red-600"
                            : "text-slate-700",
                        ].join(" ")}
                      >
                        {formatDate(
                          item.nextActionAt,
                        )}
                      </div>
                    </div>

                    <div className="rounded-xl bg-slate-50 p-3">
                      <div className="flex items-center gap-1.5 text-xs font-semibold uppercase tracking-wide text-slate-400">
                        <UserRound className="h-3.5 w-3.5" />
                        Técnico responsável
                      </div>

                      <div className="mt-1.5 text-sm font-semibold text-slate-700">
                        {item.responsibleUserName ??
                          "Não definido"}
                      </div>
                    </div>
                  </div>

                  <div className="mt-4 flex items-center justify-end text-sm font-semibold text-[#007dd8] xl:hidden">
                    Abrir operação
                    <ChevronRight className="ml-1 h-4 w-4" />
                  </div>
                </div>
              </div>
            </Link>
          );
        })}
      </div>
    </div>
  );
}