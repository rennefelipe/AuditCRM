"use client";

import { Loader2 } from "lucide-react";
import { useState } from "react";

import { DashboardFilters } from "@/features/dashboard/components/dashboard-filters";
import { WorkQueueList } from "@/features/work-queue/components/work-queue-list";
import { useWorkQueue } from "@/features/work-queue/use-work-queue";

export default function WorkQueuePage() {
  const [
    shoppingGroupId,
    setShoppingGroupId,
  ] = useState<string | undefined>();

  const [
    shoppingId,
    setShoppingId,
  ] = useState<string | undefined>();

  const {
    data = [],
    isLoading,
    isError,
  } = useWorkQueue({
    shoppingGroupId,
    shoppingId,
  });

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight text-[#00213d] sm:text-3xl">
          Fila de trabalho
        </h1>

        <p className="mt-1 text-sm text-slate-500 sm:text-base">
          Lojas e processos que precisam de acompanhamento.
        </p>
      </div>

      <DashboardFilters
        shoppingGroupId={shoppingGroupId}
        shoppingId={shoppingId}
        onShoppingGroupChange={
          setShoppingGroupId
        }
        onShoppingChange={setShoppingId}
      />

      {isLoading && (
        <div className="flex min-h-52 items-center justify-center rounded-2xl border border-slate-200 bg-white">
          <Loader2 className="h-6 w-6 animate-spin text-[#007dd8]" />
        </div>
      )}

      {isError && (
        <div className="rounded-2xl border border-red-200 bg-red-50 p-5 text-sm font-medium text-red-600">
          Não foi possível carregar a fila de trabalho.
        </div>
      )}

      {!isLoading && !isError && (
        <WorkQueueList items={data} />
      )}
    </div>
  );
}