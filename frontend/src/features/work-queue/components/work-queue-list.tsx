import {
  CalendarClock,
  ChevronRight,
  CircleAlert,
  Store,
  UserRound,
} from "lucide-react";
import Link from "next/link";

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

        <h3 className="mt-4 font-semibold text-[#00213d]">
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

          return (
            <Link
              key={item.storeProcessId}
              href={`/stores/${item.storeId}/processes/${item.storeProcessId}`}
              className="group block p-4 transition hover:bg-slate-50 sm:p-5"
            >
              <div className="flex gap-4">
                <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-[#007dd8]/10 text-[#007dd8]">
                  <Store className="h-5 w-5" />
                </div>

                <div className="min-w-0 flex-1">
                  <div className="flex flex-col gap-3 lg:flex-row lg:items-start lg:justify-between">
                    <div className="min-w-0">
                      <div className="flex flex-wrap items-center gap-2">
                        <h3 className="truncate text-base font-semibold text-[#00213d]">
                          {item.storeName}
                        </h3>

                        {overdue && (
                          <span className="inline-flex items-center gap-1 rounded-full bg-red-50 px-2.5 py-1 text-xs font-semibold text-red-600">
                            <CircleAlert className="h-3.5 w-3.5" />
                            Atrasada
                          </span>
                        )}
                      </div>

                      <p className="mt-1 text-sm text-slate-500">
                        {item.shoppingGroupName ??
                          "Sem rede"}
                        {" · "}
                        {item.shoppingName}
                      </p>
                    </div>

                    <ChevronRight className="hidden h-5 w-5 shrink-0 text-slate-300 transition group-hover:translate-x-1 group-hover:text-[#007dd8] lg:block" />
                  </div>

                  <div className="mt-4 grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
                    <div>
                      <div className="text-xs font-medium uppercase tracking-wide text-slate-400">
                        Próxima ação
                      </div>

                      <div className="mt-1 text-sm font-medium text-slate-700">
                        {item.nextAction ??
                          "Sem próxima ação"}
                      </div>
                    </div>

                    <div>
                      <div className="flex items-center gap-1.5 text-xs font-medium uppercase tracking-wide text-slate-400">
                        <CalendarClock className="h-3.5 w-3.5" />
                        Prazo
                      </div>

                      <div
                        className={[
                          "mt-1 text-sm font-medium",
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

                    <div>
                      <div className="flex items-center gap-1.5 text-xs font-medium uppercase tracking-wide text-slate-400">
                        <UserRound className="h-3.5 w-3.5" />
                        Responsável
                      </div>

                      <div className="mt-1 text-sm font-medium text-slate-700">
                        {item.responsibleUserName ??
                          "Não definido"}
                      </div>
                    </div>

                    <div>
                      <div className="text-xs font-medium uppercase tracking-wide text-slate-400">
                        Status / Prioridade
                      </div>

                      <div className="mt-1 text-sm font-medium text-slate-700">
                        {item.status} / {item.priority}
                      </div>
                    </div>
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