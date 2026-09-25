"use client";

import {
  CalendarClock,
  ChevronDown,
  ChevronUp,
  Loader2,
  UserRound,
} from "lucide-react";
import {
  useMemo,
  useState,
} from "react";

import { useProcessTimeline } from "@/features/process-timeline/use-process-timeline";

type ProcessTimelinePanelProps = {
  storeProcessId: string;
};

function formatTimelineDate(
  value: string,
) {
  return new Intl.DateTimeFormat(
    "pt-BR",
    {
      dateStyle: "short",
      timeStyle: "short",
    },
  ).format(new Date(value));
}

function getTypeLabel(
  type: string,
) {
  const normalized =
    type.toLowerCase();

  if (
    normalized.includes(
      "interaction",
    ) ||
    normalized.includes(
      "tratativa",
    )
  ) {
    return "Tratativa";
  }

  if (
    normalized.includes(
      "appointment",
    ) ||
    normalized.includes(
      "agendamento",
    )
  ) {
    return "Agendamento";
  }

  if (
    normalized.includes(
      "installation",
    ) ||
    normalized.includes(
      "instala",
    )
  ) {
    return "Instalação";
  }

  if (
    normalized.includes(
      "notification",
    ) ||
    normalized.includes(
      "notifica",
    )
  ) {
    return "Notificação";
  }

  if (
    normalized.includes(
      "status",
    )
  ) {
    return "Status";
  }

  return type || "Evento";
}

export function ProcessTimelinePanel({
  storeProcessId,
}: ProcessTimelinePanelProps) {
  const [
    showAll,
    setShowAll,
  ] = useState(false);

  const {
    data = [],
    isLoading,
    isError,
  } = useProcessTimeline(
    storeProcessId,
  );

  const orderedItems =
    useMemo(
      () =>
        [...data].sort(
          (a, b) =>
            new Date(
              b.occurredAt,
            ).getTime() -
            new Date(
              a.occurredAt,
            ).getTime(),
        ),
      [data],
    );

  const visibleItems =
    showAll
      ? orderedItems
      : orderedItems.slice(
          0,
          5,
        );

  if (isLoading) {
    return (
      <div className="flex min-h-28 items-center justify-center rounded-xl border border-slate-200 bg-white">
        <Loader2 className="h-5 w-5 animate-spin text-[#087fdb]" />
      </div>
    );
  }

  if (isError) {
    return (
      <div className="rounded-xl border border-red-200 bg-red-50 px-4 py-6 text-center text-sm text-red-600">
        Não foi possível carregar o histórico.
      </div>
    );
  }

  if (
    orderedItems.length === 0
  ) {
    return (
      <div className="rounded-xl border border-slate-200 bg-white px-5 py-8 text-center">
        <p className="text-sm text-slate-500">
          Nenhuma movimentação registrada.
        </p>
      </div>
    );
  }

  return (
    <div className="space-y-3">
      <div className="overflow-hidden rounded-xl border border-slate-200 bg-white">
        <div className="divide-y divide-slate-100">
          {visibleItems.map(
            (item) => (
              <article
                key={item.id}
                className="px-4 py-4"
              >
                <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                  <div className="min-w-0 flex-1">
                    <div className="flex flex-wrap items-center gap-2">
                      <span className="inline-flex rounded-md bg-slate-100 px-2 py-1 text-[10px] font-bold uppercase tracking-wide text-slate-600">
                        {getTypeLabel(
                          item.type,
                        )}
                      </span>

                      <div className="flex items-center gap-1.5 text-xs text-slate-400">
                        <CalendarClock className="h-3.5 w-3.5" />

                        {formatTimelineDate(
                          item.occurredAt,
                        )}
                      </div>
                    </div>

                    <h5 className="mt-2 break-words text-sm font-semibold text-[#00213d]">
                      {item.title}
                    </h5>

                    {item.description && (
                      <p className="mt-1 whitespace-pre-wrap break-words text-sm leading-6 text-slate-500">
                        {
                          item.description
                        }
                      </p>
                    )}
                  </div>

                  <div className="flex shrink-0 items-center gap-1.5 text-xs text-slate-500">
                    <UserRound className="h-3.5 w-3.5 text-slate-400" />

                    <span>
                      {item.userName ??
                        "Sistema"}
                    </span>
                  </div>
                </div>
              </article>
            ),
          )}
        </div>
      </div>

      {orderedItems.length >
        5 && (
        <div className="flex justify-center">
          <button
            type="button"
            onClick={() =>
              setShowAll(
                (current) =>
                  !current,
              )
            }
            className="inline-flex h-9 items-center gap-1.5 rounded-md px-3 text-xs font-semibold text-[#087fdb] transition hover:bg-[#087fdb]/5"
          >
            {showAll
              ? "Mostrar menos"
              : `Ver todos (${orderedItems.length})`}

            {showAll ? (
              <ChevronUp className="h-3.5 w-3.5" />
            ) : (
              <ChevronDown className="h-3.5 w-3.5" />
            )}
          </button>
        </div>
      )}
    </div>
  );
}