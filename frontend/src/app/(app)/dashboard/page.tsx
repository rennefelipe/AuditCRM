"use client";

import { Loader2 } from "lucide-react";
import { useState } from "react";

import { DashboardFilters } from "@/features/dashboard/components/dashboard-filters";
import { SummaryCards } from "@/features/dashboard/components/summary-cards";
import { useDashboardSummary } from "@/features/dashboard/use-dashboard-summary";

export default function DashboardPage() {
  const [
    shoppingGroupId,
    setShoppingGroupId,
  ] = useState<string | undefined>();

  const [
    shoppingId,
    setShoppingId,
  ] = useState<string | undefined>();

  const {
    data,
    isLoading,
    isError,
  } = useDashboardSummary({
    shoppingGroupId,
    shoppingId,
  });

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight text-[#00213d] sm:text-3xl">
          Dashboard
        </h1>

        <p className="mt-1 text-sm text-slate-500 sm:text-base">
          Visão geral da operação.
        </p>
      </div>

      <DashboardFilters
        shoppingGroupId={shoppingGroupId}
        shoppingId={shoppingId}
        onShoppingGroupChange={setShoppingGroupId}
        onShoppingChange={setShoppingId}
      />

      {isLoading && (
        <div className="flex min-h-48 items-center justify-center rounded-2xl border border-slate-200 bg-white">
          <Loader2 className="h-6 w-6 animate-spin text-[#007dd8]" />
        </div>
      )}

      {isError && (
        <div className="rounded-2xl border border-red-200 bg-red-50 p-5 text-sm font-medium text-red-600">
          Não foi possível carregar o dashboard.
        </div>
      )}

      {data && (
        <>
          <SummaryCards data={data} />

          <div className="grid gap-4 lg:grid-cols-3">
            <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
              <div className="text-sm font-medium text-slate-500">
                Em negociação
              </div>

              <div className="mt-2 text-3xl font-bold text-[#00213d]">
                {data.inNegotiation}
              </div>
            </div>

            <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
              <div className="text-sm font-medium text-slate-500">
                Sem retorno
              </div>

              <div className="mt-2 text-3xl font-bold text-[#00213d]">
                {data.notifiedNoResponse}
              </div>
            </div>

            <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
              <div className="text-sm font-medium text-slate-500">
                Sem próxima ação
              </div>

              <div className="mt-2 text-3xl font-bold text-[#00213d]">
                {data.withoutNextAction}
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
}