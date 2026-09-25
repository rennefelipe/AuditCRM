"use client";

import {
  Loader2,
  SearchX,
  SlidersHorizontal,
} from "lucide-react";
import {
  useDeferredValue,
  useMemo,
  useState,
} from "react";

import {
  OperationalFilters,
  type OperationalFilterValues,
} from "@/features/work-queue/components/operational-filters";
import { WorkQueueList } from "@/features/work-queue/components/work-queue-list";
import { useWorkQueue } from "@/features/work-queue/use-work-queue";

const emptyFilters: OperationalFilterValues = {};

export default function WorkQueuePage() {
  const [filters, setFilters] =
    useState<OperationalFilterValues>(emptyFilters);

  const deferredSearch = useDeferredValue(
    filters.search,
  );

  const {
    data = [],
    isLoading,
    isError,
    isFetching,
  } = useWorkQueue({
    ...filters,
    search: deferredSearch,
  });

  /*
   * Segunda consulta utilizada somente para montar
   * as opções de Loja, Tipo de Instalação e Técnico.
   *
   * Ela respeita Rede e Shopping, mas não é limitada
   * pelos demais filtros operacionais.
   */
  const {
    data: optionSource = [],
  } = useWorkQueue({
    shoppingGroupId:
      filters.shoppingGroupId,
    shoppingId:
      filters.shoppingId,
  });

  const storeOptions = useMemo(() => {
    const map = new Map<string, string>();

    optionSource.forEach((item) => {
      const luc = item.storeLuc
        ? ` • LUC ${item.storeLuc}`
        : "";

      map.set(
        item.storeId,
        `${item.storeName}${luc}`,
      );
    });

    return Array.from(map.entries())
      .map(([value, label]) => ({
        value,
        label,
      }))
      .sort((a, b) =>
        a.label.localeCompare(
          b.label,
          "pt-BR",
        ),
      );
  }, [optionSource]);

  const installationTypeOptions =
    useMemo(() => {
      const map = new Map<string, string>();

      optionSource.forEach((item) => {
        if (
          item.installationTypeId &&
          item.installationTypeName
        ) {
          map.set(
            item.installationTypeId,
            item.installationTypeName,
          );
        }
      });

      return Array.from(map.entries())
        .map(([value, label]) => ({
          value,
          label,
        }))
        .sort((a, b) =>
          a.label.localeCompare(
            b.label,
            "pt-BR",
          ),
        );
    }, [optionSource]);

  const responsibleUserOptions =
    useMemo(() => {
      const map = new Map<string, string>();

      optionSource.forEach((item) => {
        if (
          item.responsibleUserId &&
          item.responsibleUserName
        ) {
          map.set(
            item.responsibleUserId,
            item.responsibleUserName,
          );
        }
      });

      return Array.from(map.entries())
        .map(([value, label]) => ({
          value,
          label,
        }))
        .sort((a, b) =>
          a.label.localeCompare(
            b.label,
            "pt-BR",
          ),
        );
    }, [optionSource]);

  function handleFiltersChange(
    values: OperationalFilterValues,
  ) {
    setFilters(values);
  }

  function handleClearFilters() {
    setFilters(emptyFilters);
  }

  return (
    <div className="space-y-6">
      <div className="overflow-hidden rounded-2xl bg-gradient-to-r from-[#041a2d] via-[#062b49] to-[#07466a] shadow-lg">
        <div className="relative px-5 py-6 sm:px-7 sm:py-7">
          <div className="pointer-events-none absolute right-0 top-0 h-40 w-40 rounded-full bg-cyan-300/10 blur-3xl" />

          <div className="relative flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
            <div>
              <div className="mb-2 flex items-center gap-2 text-xs font-bold uppercase tracking-[0.18em] text-cyan-300">
                <SlidersHorizontal className="h-4 w-4" />
                Operação
              </div>

              <h1 className="text-2xl font-bold tracking-tight text-white sm:text-3xl">
                Fila de trabalho
              </h1>

              <p className="mt-2 max-w-2xl text-sm leading-6 text-slate-300 sm:text-base">
                Consulte, filtre e encontre rapidamente
                as lojas que precisam de acompanhamento.
              </p>
            </div>

            <div className="rounded-xl border border-white/10 bg-white/10 px-5 py-3 backdrop-blur-sm">
              <p className="text-xs font-semibold uppercase tracking-wide text-slate-300">
                Processos encontrados
              </p>

              <div className="mt-1 flex items-center gap-2">
                <span className="text-3xl font-bold text-white">
                  {data.length}
                </span>

                {isFetching && !isLoading && (
                  <Loader2 className="h-4 w-4 animate-spin text-cyan-300" />
                )}
              </div>
            </div>
          </div>
        </div>
      </div>

      <OperationalFilters
        values={filters}
        storeOptions={storeOptions}
        installationTypeOptions={
          installationTypeOptions
        }
        responsibleUserOptions={
          responsibleUserOptions
        }
        onChange={handleFiltersChange}
        onClear={handleClearFilters}
      />

      {isLoading && (
        <div className="flex min-h-64 items-center justify-center rounded-2xl border border-slate-200 bg-white shadow-sm">
          <div className="text-center">
            <Loader2 className="mx-auto h-7 w-7 animate-spin text-[#087fdb]" />

            <p className="mt-3 text-sm font-medium text-slate-500">
              Carregando fila de trabalho...
            </p>
          </div>
        </div>
      )}

      {isError && (
        <div className="rounded-2xl border border-red-200 bg-red-50 p-5">
          <div className="flex gap-3">
            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-red-100 text-red-600">
              <SearchX className="h-5 w-5" />
            </div>

            <div>
              <h3 className="font-semibold text-red-800">
                Não foi possível carregar a fila
              </h3>

              <p className="mt-1 text-sm text-red-600">
                Verifique se a API está em execução e
                tente novamente.
              </p>
            </div>
          </div>
        </div>
      )}

      {!isLoading && !isError && (
        <WorkQueueList items={data} />
      )}
    </div>
  );
}