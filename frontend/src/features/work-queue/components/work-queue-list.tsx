"use client";

import {
  CalendarClock,
  Check,
  ChevronDown,
  ChevronUp,
  CircleAlert,
  Clock3,
  Loader2,
  MessageSquareText,
  Pencil,
  Power,
  Save,
  Store,
  Trash2,
  UserRound,
  Wrench,
  X,
} from "lucide-react";
import { useEffect, useState } from "react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

import type {
  ProcessFrequency,
  ProcessPriority,
  StoreProcessStatus,
} from "@/features/store-processes/store-process-service";
import {
  useStoreProcess,
  useUpdateStoreProcess,
} from "@/features/store-processes/use-store-process";

import type { MonitoringStatus } from "@/features/stores/store-types";
import {
  useActivateStore,
  useDeactivateStore,
  useDeleteStore,
  useStore,
  useUpdateStore,
} from "@/features/stores/use-stores";

import {
  getPriorityInfo,
  getStatusInfo,
} from "../process-labels";
import type { WorkQueueItem } from "../work-queue-service";

import { AppointmentDialog } from "./appointment-dialog";
import { HistoryDialog } from "./history-dialog";
import { InstallationDialog } from "./installation-dialog";
import { InteractionDialog } from "./interaction-dialog";

type WorkQueueListProps = {
  items: WorkQueueItem[];
};

const statusOptions = [
  {
    value: 1,
    label: "Serviço",
  },
  {
    value: 2,
    label: "Instalado",
  },
  {
    value: 3,
    label: "Em tratativas",
  },
  {
    value: 4,
    label: "Notificado sem retorno",
  },
  {
    value: 5,
    label: "Não autorizado",
  },
  {
    value: 6,
    label: "Saiu do shopping",
  },
];

const priorityOptions = [
  {
    value: 1,
    label: "Baixa",
  },
  {
    value: 2,
    label: "Normal",
  },
  {
    value: 3,
    label: "Alta",
  },
  {
    value: 4,
    label: "Urgente",
  },
];

const frequencyOptions = [
  {
    value: 1,
    label: "Diário",
  },
  {
    value: 2,
    label: "Semanal",
  },
  {
    value: 3,
    label: "Quinzenal",
  },
  {
    value: 4,
    label: "Mensal",
  },
];

const monitoringOptions = [
  {
    value: 0,
    label: "Não cadastrado",
  },
  {
    value: 1,
    label: "Verde",
  },
  {
    value: 2,
    label: "Amarelo",
  },
  {
    value: 3,
    label: "Vermelho",
  },
  {
    value: 4,
    label: "Não monitorado",
  },
  {
    value: 5,
    label: "Não se aplica",
  },
];

const frequencyLabels: Record<number, string> = {
  1: "Diário",
  2: "Semanal",
  3: "Quinzenal",
  4: "Mensal",
};

function getFrequencyLabel(
  value: number,
) {
  return (
    frequencyLabels[value] ??
    `Periodicidade ${value}`
  );
}

function formatDate(
  value: string | null,
) {
  if (!value) {
    return "Sem prazo";
  }

  return new Intl.DateTimeFormat(
    "pt-BR",
    {
      dateStyle: "short",
      timeStyle: "short",
    },
  ).format(new Date(value));
}

function formatDocument(
  value: string | null,
) {
  if (!value) {
    return "Não informado";
  }

  const digits =
    value.replace(/\D/g, "");

  if (digits.length !== 14) {
    return value;
  }

  return digits.replace(
    /^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/,
    "$1.$2.$3/$4-$5",
  );
}

function isOverdue(
  value: string | null,
) {
  if (!value) {
    return false;
  }

  return (
    new Date(value).getTime() <
    Date.now()
  );
}

export function WorkQueueList({
  items,
}: WorkQueueListProps) {
  const [
    expandedProcessId,
    setExpandedProcessId,
  ] = useState<string | null>(
    null,
  );

  if (items.length === 0) {
    return (
      <div className="rounded-xl border border-slate-200 bg-white p-8 text-center shadow-sm">
        <div className="mx-auto flex h-11 w-11 items-center justify-center rounded-xl bg-slate-100 text-slate-400">
          <Store className="h-5 w-5" />
        </div>

        <h3 className="mt-3 font-semibold text-[#00213d]">
          Nenhuma loja encontrada
        </h3>

        <p className="mt-1 text-sm text-slate-500">
          Ajuste os filtros para localizar os processos.
        </p>
      </div>
    );
  }

  return (
    <div className="space-y-3">
      {items.map((item) => (
        <WorkQueueCard
          key={item.storeProcessId}
          item={item}
          expanded={
            expandedProcessId ===
            item.storeProcessId
          }
          onToggle={() =>
            setExpandedProcessId(
              expandedProcessId ===
                item.storeProcessId
                ? null
                : item.storeProcessId,
            )
          }
        />
      ))}
    </div>
  );
}

type WorkQueueCardProps = {
  item: WorkQueueItem;
  expanded: boolean;
  onToggle: () => void;
};

function WorkQueueCard({
  item,
  expanded,
  onToggle,
}: WorkQueueCardProps) {
  const overdue =
    isOverdue(
      item.nextActionAt,
    );

  const statusInfo =
    getStatusInfo(
      item.status,
    );

  const priorityInfo =
    getPriorityInfo(
      item.priority,
    );

  const {
    data: store,
    isLoading: loadingStore,
  } = useStore(
    expanded
      ? item.storeId
      : undefined,
  );

  const {
    data: process,
    isLoading: loadingProcess,
  } = useStoreProcess(
    expanded
      ? item.storeProcessId
      : "",
  );

  const updateStoreMutation =
    useUpdateStore(
      item.storeId,
    );

  const updateProcessMutation =
    useUpdateStoreProcess(
      item.storeProcessId,
    );

  const activateMutation =
    useActivateStore(
      item.storeId,
    );

  const deactivateMutation =
    useDeactivateStore(
      item.storeId,
    );

  const deleteMutation =
    useDeleteStore(
      item.storeId,
    );

  const [
    interactionDialogOpen,
    setInteractionDialogOpen,
  ] = useState(false);

  const [
    appointmentDialogOpen,
    setAppointmentDialogOpen,
  ] = useState(false);

  const [
    installationDialogOpen,
    setInstallationDialogOpen,
  ] = useState(false);

  const [
    historyDialogOpen,
    setHistoryDialogOpen,
  ] = useState(false);

  const [
    tradeName,
    setTradeName,
  ] = useState("");

  const [
    luc,
    setLuc,
  ] = useState("");

  const [
    corporateName,
    setCorporateName,
  ] = useState("");

  const [
    document,
    setDocument,
  ] = useState("");

  const [
    stateRegistration,
    setStateRegistration,
  ] = useState("");

  const [
    numberOfRegisters,
    setNumberOfRegisters,
  ] = useState("");

  const [
    monitoringStatus,
    setMonitoringStatus,
  ] =
    useState<MonitoringStatus>(
      0,
    );

  const [
    businessType,
    setBusinessType,
  ] = useState("");

  const [
    storeNotes,
    setStoreNotes,
  ] = useState("");

  const [
    status,
    setStatus,
  ] =
    useState<StoreProcessStatus>(
      1,
    );

  const [
    priority,
    setPriority,
  ] =
    useState<ProcessPriority>(
      2,
    );

  const [
    frequency,
    setFrequency,
  ] =
    useState<ProcessFrequency>(
      4,
    );

  const [
    nextAction,
    setNextAction,
  ] = useState("");

  const [
    nextActionAt,
    setNextActionAt,
  ] = useState("");

  const [
    processNotes,
    setProcessNotes,
  ] = useState("");

  const [
    feedback,
    setFeedback,
  ] = useState<{
    type:
      | "success"
      | "error";
    text: string;
  } | null>(null);

  useEffect(() => {
    if (!store) {
      return;
    }

    setTradeName(
      store.tradeName,
    );

    setLuc(
      store.luc ?? "",
    );

    setCorporateName(
      store.corporateName ??
        "",
    );

    setDocument(
      store.document ?? "",
    );

    setStateRegistration(
      store.stateRegistration ??
        "",
    );

    setNumberOfRegisters(
      store.numberOfRegisters !=
      null
        ? String(
            store.numberOfRegisters,
          )
        : "",
    );

    setMonitoringStatus(
      store.monitoringStatus,
    );

    setBusinessType(
      store.businessType ??
        "",
    );

    setStoreNotes(
      store.notes ?? "",
    );
  }, [store]);

  useEffect(() => {
    if (!process) {
      return;
    }

    setStatus(
      process.status,
    );

    setPriority(
      process.priority,
    );

    setFrequency(
      process.frequency,
    );

    setNextAction(
      process.nextAction ??
        "",
    );

    setNextActionAt(
      process.nextActionAt
        ? process.nextActionAt.slice(
            0,
            16,
          )
        : "",
    );

    setProcessNotes(
      process.notes ?? "",
    );
  }, [process]);

  async function handleSave() {
    if (
      !store ||
      !process
    ) {
      return;
    }

    setFeedback(null);

    try {
      await updateStoreMutation.mutateAsync(
        {
          shoppingId:
            store.shoppingId,

          erpId:
            store.erpId,

          tradeName:
            tradeName.trim(),

          luc:
            luc.trim() ||
            null,

          corporateName:
            corporateName.trim() ||
            null,

          document:
            document.trim() ||
            null,

          stateRegistration:
            stateRegistration.trim() ||
            null,

          numberOfRegisters:
            numberOfRegisters.trim()
              ? Number(
                  numberOfRegisters,
                )
              : null,

          monitoringStatus,

          businessType:
            businessType.trim() ||
            null,

          notes:
            storeNotes.trim() ||
            null,
        },
      );

      await updateProcessMutation.mutateAsync(
        {
          installationTypeId:
            process.installationTypeId,

          responsibleUserId:
            process.responsibleUserId,

          status,
          priority,
          frequency,

          nextAction:
            nextAction.trim() ||
            null,

          nextActionAt:
            nextActionAt
              ? new Date(
                  nextActionAt,
                ).toISOString()
              : null,

          notes:
            processNotes.trim() ||
            null,
        },
      );

      setFeedback({
        type: "success",
        text: "Alterações salvas.",
      });
    } catch {
      setFeedback({
        type: "error",
        text: "Erro ao salvar alterações.",
      });
    }
  }

  async function handleToggleActive() {
    if (!store) {
      return;
    }

    const confirmed =
      window.confirm(
        store.isActive
          ? `Inativar a loja "${store.tradeName}"?`
          : `Reativar a loja "${store.tradeName}"?`,
      );

    if (!confirmed) {
      return;
    }

    try {
      if (store.isActive) {
        await deactivateMutation.mutateAsync();
      } else {
        await activateMutation.mutateAsync();
      }

      setFeedback({
        type: "success",
        text: store.isActive
          ? "Loja inativada."
          : "Loja reativada.",
      });
    } catch {
      setFeedback({
        type: "error",
        text: "Não foi possível alterar a situação da loja.",
      });
    }
  }

  async function handleDelete() {
    if (!store) {
      return;
    }

    const confirmed =
      window.confirm(
        `Excluir a loja "${store.tradeName}"?\n\nA exclusão só será permitida se não houver registros vinculados.`,
      );

    if (!confirmed) {
      return;
    }

    try {
      await deleteMutation.mutateAsync();
    } catch {
      setFeedback({
        type: "error",
        text: "Não foi possível excluir a loja.",
      });
    }
  }

  const isSaving =
    updateStoreMutation.isPending ||
    updateProcessMutation.isPending;

  return (
    <>
      <article className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
        {/* CARD RESUMIDO */}
        <button
          type="button"
          onClick={onToggle}
          className="w-full text-left transition hover:bg-slate-50/70"
        >
          <div className="flex flex-col gap-3 p-4 lg:flex-row lg:items-center">
            <div className="flex min-w-0 flex-1 items-start gap-3">
              <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-[#00213d] text-cyan-300">
                <Store className="h-4 w-4" />
              </div>

              <div className="min-w-0">
                <div className="flex flex-wrap items-center gap-2">
                  <h3 className="font-bold text-[#00213d]">
                    {item.storeName}
                  </h3>

                  <Badge
                    variant="outline"
                    className={
                      statusInfo.className
                    }
                  >
                    {
                      statusInfo.label
                    }
                  </Badge>

                  <Badge
                    variant="outline"
                    className={
                      priorityInfo.className
                    }
                  >
                    {
                      priorityInfo.label
                    }
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

                <div className="mt-1 flex flex-wrap gap-x-3 gap-y-1 text-sm text-slate-500">
                  <span>
                    {item.shoppingGroupName ??
                      "Sem grupo"}
                  </span>

                  <span>•</span>

                  <span>
                    {
                      item.shoppingName
                    }
                  </span>

                  <span>•</span>

                  <span>
                    LUC{" "}
                    {item.storeLuc ??
                      "-"}
                  </span>

                  <span>•</span>

                  <span>
                    {item.installationTypeName ??
                      "Sem tipo"}
                  </span>

                  <span>•</span>

                  <span>
                    {getFrequencyLabel(
                      item.frequency,
                    )}
                  </span>
                </div>
              </div>
            </div>

            <div className="grid flex-1 gap-3 sm:grid-cols-3 lg:max-w-[650px]">
              <CompactInfo
                icon={
                  UserRound
                }
                label="Técnico"
                value={
                  item.responsibleUserName ??
                  "Não definido"
                }
              />

              <CompactInfo
                icon={
                  CalendarClock
                }
                label="Prazo"
                value={formatDate(
                  item.nextActionAt,
                )}
                danger={
                  overdue
                }
              />

              <CompactInfo
                icon={Wrench}
                label="Próxima ação"
                value={
                  item.nextAction ??
                  "Não definida"
                }
              />
            </div>

            <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-500">
              {expanded ? (
                <ChevronUp className="h-4 w-4" />
              ) : (
                <ChevronDown className="h-4 w-4" />
              )}
            </div>
          </div>
        </button>

        {/* CARD EXPANDIDO */}
        {expanded && (
          <div className="border-t border-slate-200 bg-[#f8fafc] p-4">
            {loadingStore ||
            loadingProcess ||
            !store ||
            !process ? (
              <div className="flex h-24 items-center justify-center">
                <Loader2 className="h-5 w-5 animate-spin text-[#087fdb]" />
              </div>
            ) : (
              <div className="space-y-3">
                {/* BARRA OPERACIONAL */}
                <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
                  <div>
                    <div className="flex items-center gap-2 text-sm font-bold text-[#00213d]">
                      <Pencil className="h-4 w-4 text-[#087fdb]" />

                      Edição rápida
                    </div>

                    <p className="text-xs text-slate-500">
                      Loja + processo atual
                    </p>
                  </div>

                  <div className="flex flex-wrap gap-2">
                    <Button
                      type="button"
                      variant="outline"
                      onClick={() =>
                        setInteractionDialogOpen(
                          true,
                        )
                      }
                      className="h-9 gap-2 border-[#087fdb]/30 text-[#087fdb] hover:bg-[#087fdb]/5"
                    >
                      <MessageSquareText className="h-4 w-4" />

                      Registrar tratativa
                    </Button>

                    <Button
                      type="button"
                      variant="outline"
                      onClick={() =>
                        setAppointmentDialogOpen(
                          true,
                        )
                      }
                      className="h-9 gap-2 border-[#087fdb]/30 text-[#087fdb] hover:bg-[#087fdb]/5"
                    >
                      <CalendarClock className="h-4 w-4" />

                      Agendar
                    </Button>

                    <Button
                      type="button"
                      variant="outline"
                      onClick={() =>
                        setInstallationDialogOpen(
                          true,
                        )
                      }
                      className="h-9 gap-2 border-emerald-300 text-emerald-700 hover:bg-emerald-50"
                    >
                      <Wrench className="h-4 w-4" />

                      Registrar instalação
                    </Button>

                    <Button
                      type="button"
                      variant="outline"
                      onClick={() =>
                        setHistoryDialogOpen(
                          true,
                        )
                      }
                      className="h-9 gap-2"
                    >
                      <Clock3 className="h-4 w-4" />

                      Histórico
                    </Button>

                    <Button
                      type="button"
                      onClick={
                        handleSave
                      }
                      disabled={
                        isSaving
                      }
                      className="h-9 gap-2 bg-[#087fdb] px-4 text-white hover:bg-[#006ec2]"
                    >
                      {isSaving ? (
                        <>
                          <Loader2 className="h-4 w-4 animate-spin" />

                          Salvando
                        </>
                      ) : (
                        <>
                          <Save className="h-4 w-4" />

                          Salvar
                        </>
                      )}
                    </Button>
                  </div>
                </div>

                {feedback && (
                  <div
                    className={[
                      "flex items-center gap-2 rounded-md border px-3 py-2 text-xs font-medium",
                      feedback.type ===
                      "success"
                        ? "border-emerald-200 bg-emerald-50 text-emerald-700"
                        : "border-red-200 bg-red-50 text-red-700",
                    ].join(
                      " ",
                    )}
                  >
                    {feedback.type ===
                    "success" ? (
                      <Check className="h-4 w-4" />
                    ) : (
                      <X className="h-4 w-4" />
                    )}

                    {
                      feedback.text
                    }
                  </div>
                )}

                {/* LOJA + PROCESSO */}
                <div className="grid gap-3 xl:grid-cols-[0.9fr_1.1fr]">
                  <section className="rounded-lg border border-slate-200 bg-white p-3">
                    <div className="mb-3 flex items-center justify-between">
                      <div>
                        <h4 className="text-sm font-bold text-[#00213d]">
                          Loja
                        </h4>

                        <p className="text-[11px] text-slate-400">
                          Cadastro principal
                        </p>
                      </div>

                      <span className="rounded-md bg-slate-100 px-2 py-1 text-[11px] font-medium text-slate-500">
                        {store.isActive
                          ? "Ativa"
                          : "Inativa"}
                      </span>
                    </div>

                    <div className="grid gap-2 sm:grid-cols-2">
                      <CompactField label="Nome">
                        <Input
                          value={
                            tradeName
                          }
                          onChange={(
                            event,
                          ) =>
                            setTradeName(
                              event
                                .target
                                .value,
                            )
                          }
                          className="h-9"
                        />
                      </CompactField>

                      <CompactField label="LUC">
                        <Input
                          value={luc}
                          onChange={(
                            event,
                          ) =>
                            setLuc(
                              event
                                .target
                                .value,
                            )
                          }
                          className="h-9"
                        />
                      </CompactField>

                      <CompactField label="CNPJ">
                        <Input
                          value={
                            document
                          }
                          onChange={(
                            event,
                          ) =>
                            setDocument(
                              event
                                .target
                                .value,
                            )
                          }
                          className="h-9"
                        />
                      </CompactField>

                      <CompactField label="Razão Social">
                        <Input
                          value={
                            corporateName
                          }
                          onChange={(
                            event,
                          ) =>
                            setCorporateName(
                              event
                                .target
                                .value,
                            )
                          }
                          className="h-9"
                        />
                      </CompactField>

                      <CompactField label="ERP / PDV">
                        <ReadOnlyBox
                          value={
                            store.erpName ??
                            "Não informado"
                          }
                        />
                      </CompactField>

                      <CompactField label="Monitoramento">
                        <select
                          value={
                            monitoringStatus
                          }
                          onChange={(
                            event,
                          ) =>
                            setMonitoringStatus(
                              Number(
                                event
                                  .target
                                  .value,
                              ) as MonitoringStatus,
                            )
                          }
                          className="h-9 w-full rounded-md border border-slate-200 bg-white px-2.5 text-sm"
                        >
                          {monitoringOptions.map(
                            (
                              option,
                            ) => (
                              <option
                                key={
                                  option.value
                                }
                                value={
                                  option.value
                                }
                              >
                                {
                                  option.label
                                }
                              </option>
                            ),
                          )}
                        </select>
                      </CompactField>
                    </div>

                    <details className="mt-3">
                      <summary className="cursor-pointer text-xs font-semibold text-[#087fdb]">
                        Mais dados da loja
                      </summary>

                      <div className="mt-3 grid gap-2 sm:grid-cols-2">
                        <CompactField label="Inscrição Estadual">
                          <Input
                            value={
                              stateRegistration
                            }
                            onChange={(
                              event,
                            ) =>
                              setStateRegistration(
                                event
                                  .target
                                  .value,
                              )
                            }
                            className="h-9"
                          />
                        </CompactField>

                        <CompactField label="Nº de caixas">
                          <Input
                            type="number"
                            min="0"
                            value={
                              numberOfRegisters
                            }
                            onChange={(
                              event,
                            ) =>
                              setNumberOfRegisters(
                                event
                                  .target
                                  .value,
                              )
                            }
                            className="h-9"
                          />
                        </CompactField>

                        <CompactField label="Tipo de negócio">
                          <Input
                            value={
                              businessType
                            }
                            onChange={(
                              event,
                            ) =>
                              setBusinessType(
                                event
                                  .target
                                  .value,
                              )
                            }
                            className="h-9"
                          />
                        </CompactField>

                        <div className="sm:col-span-2">
                          <CompactField label="Observação da loja">
                            <textarea
                              value={
                                storeNotes
                              }
                              onChange={(
                                event,
                              ) =>
                                setStoreNotes(
                                  event
                                    .target
                                    .value,
                                )
                              }
                              rows={2}
                              className="w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm outline-none focus:border-[#087fdb]"
                            />
                          </CompactField>
                        </div>
                      </div>

                      <p className="mt-2 text-[11px] text-slate-400">
                        CNPJ formatado:{" "}
                        {formatDocument(
                          store.document,
                        )}
                      </p>
                    </details>
                  </section>

                  <section className="rounded-lg border border-slate-200 bg-white p-3">
                    <div className="mb-3">
                      <h4 className="text-sm font-bold text-[#00213d]">
                        Processo atual
                      </h4>

                      <p className="text-[11px] text-slate-400">
                        Acompanhamento operacional
                      </p>
                    </div>

                    <div className="grid gap-2 sm:grid-cols-2 lg:grid-cols-3">
                      <CompactField label="Status">
                        <select
                          value={
                            status
                          }
                          onChange={(
                            event,
                          ) =>
                            setStatus(
                              Number(
                                event
                                  .target
                                  .value,
                              ) as StoreProcessStatus,
                            )
                          }
                          className="h-9 w-full rounded-md border border-slate-200 bg-white px-2.5 text-sm"
                        >
                          {statusOptions.map(
                            (
                              option,
                            ) => (
                              <option
                                key={
                                  option.value
                                }
                                value={
                                  option.value
                                }
                              >
                                {
                                  option.label
                                }
                              </option>
                            ),
                          )}
                        </select>
                      </CompactField>

                      <CompactField label="Prioridade">
                        <select
                          value={
                            priority
                          }
                          onChange={(
                            event,
                          ) =>
                            setPriority(
                              Number(
                                event
                                  .target
                                  .value,
                              ) as ProcessPriority,
                            )
                          }
                          className="h-9 w-full rounded-md border border-slate-200 bg-white px-2.5 text-sm"
                        >
                          {priorityOptions.map(
                            (
                              option,
                            ) => (
                              <option
                                key={
                                  option.value
                                }
                                value={
                                  option.value
                                }
                              >
                                {
                                  option.label
                                }
                              </option>
                            ),
                          )}
                        </select>
                      </CompactField>

                      <CompactField label="Periodicidade">
                        <select
                          value={
                            frequency
                          }
                          onChange={(
                            event,
                          ) =>
                            setFrequency(
                              Number(
                                event
                                  .target
                                  .value,
                              ) as ProcessFrequency,
                            )
                          }
                          className="h-9 w-full rounded-md border border-slate-200 bg-white px-2.5 text-sm"
                        >
                          {frequencyOptions.map(
                            (
                              option,
                            ) => (
                              <option
                                key={
                                  option.value
                                }
                                value={
                                  option.value
                                }
                              >
                                {
                                  option.label
                                }
                              </option>
                            ),
                          )}
                        </select>
                      </CompactField>

                      <CompactField label="Tipo">
                        <ReadOnlyBox
                          value={
                            process.installationTypeName ??
                            "Não definido"
                          }
                        />
                      </CompactField>

                      <CompactField label="Técnico">
                        <ReadOnlyBox
                          value={
                            process.responsibleUserName ??
                            "Não definido"
                          }
                        />
                      </CompactField>

                      <CompactField label="Prazo">
                        <Input
                          type="datetime-local"
                          value={
                            nextActionAt
                          }
                          onChange={(
                            event,
                          ) =>
                            setNextActionAt(
                              event
                                .target
                                .value,
                            )
                          }
                          className="h-9"
                        />
                      </CompactField>
                    </div>

                    <div className="mt-2 grid gap-2 lg:grid-cols-[1.4fr_1fr]">
                      <CompactField label="Próxima ação">
                        <Input
                          value={
                            nextAction
                          }
                          onChange={(
                            event,
                          ) =>
                            setNextAction(
                              event
                                .target
                                .value,
                            )
                          }
                          className="h-9"
                        />
                      </CompactField>

                      <CompactField label="Observação do processo">
                        <Input
                          value={
                            processNotes
                          }
                          onChange={(
                            event,
                          ) =>
                            setProcessNotes(
                              event
                                .target
                                .value,
                            )
                          }
                          className="h-9"
                        />
                      </CompactField>
                    </div>
                  </section>
                </div>

                {/* RODAPÉ */}
                <div className="flex flex-col gap-2 border-t border-slate-200 pt-3 sm:flex-row sm:items-center sm:justify-between">
                  <p className="text-[11px] text-slate-400">
                    Grupo e Shopping são vínculos. Seus cadastros serão mantidos nas áreas próprias.
                  </p>

                  <div className="flex gap-2">
                    <Button
                      type="button"
                      variant="outline"
                      onClick={
                        handleToggleActive
                      }
                      disabled={
                        activateMutation.isPending ||
                        deactivateMutation.isPending
                      }
                      className="h-8 gap-1.5 px-3 text-xs"
                    >
                      <Power className="h-3.5 w-3.5" />

                      {store.isActive
                        ? "Inativar"
                        : "Reativar"}
                    </Button>

                    <Button
                      type="button"
                      variant="outline"
                      onClick={
                        handleDelete
                      }
                      disabled={
                        deleteMutation.isPending
                      }
                      className="h-8 gap-1.5 border-red-200 px-3 text-xs text-red-600 hover:bg-red-50 hover:text-red-700"
                    >
                      <Trash2 className="h-3.5 w-3.5" />

                      Excluir
                    </Button>
                  </div>
                </div>
              </div>
            )}
          </div>
        )}
      </article>

      <InteractionDialog
        open={
          interactionDialogOpen
        }
        onOpenChange={
          setInteractionDialogOpen
        }
        storeProcessId={
          item.storeProcessId
        }
        responsibleUserId={
          process?.responsibleUserId ??
          item.responsibleUserId ??
          null
        }
        storeContactId={
          null
        }
        storeName={
          store?.tradeName ??
          item.storeName
        }
      />

      <AppointmentDialog
        open={
          appointmentDialogOpen
        }
        onOpenChange={
          setAppointmentDialogOpen
        }
        storeProcessId={
          item.storeProcessId
        }
        responsibleUserId={
          process?.responsibleUserId ??
          item.responsibleUserId ??
          null
        }
        storeContactId={
          null
        }
        storeName={
          store?.tradeName ??
          item.storeName
        }
      />

      <InstallationDialog
        open={
          installationDialogOpen
        }
        onOpenChange={
          setInstallationDialogOpen
        }
        storeProcessId={
          item.storeProcessId
        }
        responsibleUserId={
          process?.responsibleUserId ??
          item.responsibleUserId ??
          null
        }
        erpId={
          store?.erpId ??
          null
        }
        numberOfRegisters={
          store?.numberOfRegisters ??
          null
        }
        storeName={
          store?.tradeName ??
          item.storeName
        }
      />

      <HistoryDialog
        open={
          historyDialogOpen
        }
        onOpenChange={
          setHistoryDialogOpen
        }
        storeProcessId={
          item.storeProcessId
        }
        storeName={
          store?.tradeName ??
          item.storeName
        }
      />
    </>
  );
}

type CompactFieldProps = {
  label: string;
  children: React.ReactNode;
};

function CompactField({
  label,
  children,
}: CompactFieldProps) {
  return (
    <div className="space-y-1">
      <label className="block text-[11px] font-semibold uppercase tracking-wide text-slate-500">
        {label}
      </label>

      {children}
    </div>
  );
}

type CompactInfoProps = {
  icon: React.ElementType;
  label: string;
  value: string;
  danger?: boolean;
};

function CompactInfo({
  icon: Icon,
  label,
  value,
  danger = false,
}: CompactInfoProps) {
  return (
    <div className="min-w-0">
      <div
        className={[
          "flex items-center gap-1 text-[10px] font-semibold uppercase tracking-wide",
          danger
            ? "text-red-500"
            : "text-slate-400",
        ].join(" ")}
      >
        <Icon className="h-3.5 w-3.5" />

        {label}
      </div>

      <p
        className={[
          "mt-0.5 truncate text-sm font-medium",
          danger
            ? "text-red-700"
            : "text-slate-700",
        ].join(" ")}
      >
        {value}
      </p>
    </div>
  );
}

function ReadOnlyBox({
  value,
}: {
  value: string;
}) {
  return (
    <div className="flex h-9 items-center rounded-md border border-slate-200 bg-slate-50 px-2.5 text-sm text-slate-600">
      {value}
    </div>
  );
}