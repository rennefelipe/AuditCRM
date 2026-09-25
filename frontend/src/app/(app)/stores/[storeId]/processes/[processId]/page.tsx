"use client";

import {
  ArrowLeft,
  Building2,
  CalendarClock,
  CheckCircle2,
  Clipboard,
  ClipboardCheck,
  ClipboardList,
  ExternalLink,
  Globe2,
  Loader2,
  Mail,
  MapPin,
  MonitorCheck,
  Save,
  Server,
  ShoppingBag,
  Store,
  UserRound,
  Wrench,
} from "lucide-react";
import Link from "next/link";
import { useParams } from "next/navigation";
import {
  useEffect,
  useMemo,
  useState,
} from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

import { useShopping } from "@/features/shoppings/use-shoppings";
import type {
  ProcessFrequency,
  ProcessPriority,
  StoreProcessStatus,
} from "@/features/store-processes/store-process-service";
import {
  useStoreProcess,
  useUpdateStoreProcess,
} from "@/features/store-processes/use-store-process";
import { useStore } from "@/features/stores/use-stores";

const statusOptions = [
  { value: 1, label: "Serviço" },
  { value: 2, label: "Instalado" },
  { value: 3, label: "Em tratativas" },
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
  { value: 1, label: "Baixa" },
  { value: 2, label: "Normal" },
  { value: 3, label: "Alta" },
  { value: 4, label: "Urgente" },
];

const frequencyOptions = [
  { value: 1, label: "Diário" },
  { value: 2, label: "Semanal" },
  { value: 3, label: "Quinzenal" },
  { value: 4, label: "Mensal" },
];

const monitoringLabels: Record<
  number,
  string
> = {
  0: "Não cadastrado",
  1: "Verde",
  2: "Amarelo",
  3: "Vermelho",
  4: "Não monitorado",
  5: "Não se aplica",
};

function formatDateTime(
  value: string | null,
) {
  if (!value) {
    return "Não informado";
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

  const digits = value.replace(
    /\D/g,
    "",
  );

  if (digits.length !== 14) {
    return value;
  }

  return digits.replace(
    /^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/,
    "$1.$2.$3/$4-$5",
  );
}

function normalizeUrl(
  value: string | null,
) {
  if (!value) {
    return null;
  }

  if (
    value.startsWith("http://") ||
    value.startsWith("https://")
  ) {
    return value;
  }

  return `https://${value}`;
}

export default function StoreProcessPage() {
  const params = useParams<{
    storeId: string;
    processId: string;
  }>();

  const storeId = params.storeId;
  const processId = params.processId;

  const {
    data: process,
    isLoading: isLoadingProcess,
    isError: isProcessError,
  } = useStoreProcess(processId);

  const {
    data: store,
    isLoading: isLoadingStore,
    isError: isStoreError,
  } = useStore(storeId);

  const {
    data: shopping,
    isLoading: isLoadingShopping,
  } = useShopping(
    store?.shoppingId,
  );

  const updateMutation =
    useUpdateStoreProcess(processId);

  const [status, setStatus] =
    useState<StoreProcessStatus>(1);

  const [priority, setPriority] =
    useState<ProcessPriority>(2);

  const [frequency, setFrequency] =
    useState<ProcessFrequency>(4);

  const [
    nextAction,
    setNextAction,
  ] = useState("");

  const [
    nextActionAt,
    setNextActionAt,
  ] = useState("");

  const [notes, setNotes] =
    useState("");

  const [copiedKey, setCopiedKey] =
    useState<string | null>(null);

  useEffect(() => {
    if (!process) {
      return;
    }

    setStatus(process.status);
    setPriority(process.priority);
    setFrequency(process.frequency);

    setNextAction(
      process.nextAction ?? "",
    );

    setNextActionAt(
      process.nextActionAt
        ? process.nextActionAt.slice(
            0,
            16,
          )
        : "",
    );

    setNotes(process.notes ?? "");
  }, [process]);

  const apontamento =
    useMemo(() => {
      if (!store) {
        return "";
      }

      return [
        `Shopping: ${
          shopping?.name ??
          store.shoppingName
        }`,
        `API: ${
          shopping?.apiName ??
          "Não informado"
        }`,
        `Loja: ${store.tradeName}`,
        `CNPJ: ${formatDocument(
          store.document,
        )}`,
        `Razão Social: ${
          store.corporateName ??
          "Não informado"
        }`,
      ].join("\n");
    }, [shopping, store]);

  async function copyText(
    key: string,
    value: string | null,
  ) {
    if (!value) {
      return;
    }

    await navigator.clipboard.writeText(
      value,
    );

    setCopiedKey(key);

    window.setTimeout(() => {
      setCopiedKey(null);
    }, 1800);
  }

  async function handleSave() {
    if (!process) {
      return;
    }

    await updateMutation.mutateAsync({
      installationTypeId:
        process.installationTypeId,

      responsibleUserId:
        process.responsibleUserId,

      status,
      priority,
      frequency,

      nextAction:
        nextAction.trim() || null,

      nextActionAt:
        nextActionAt
          ? new Date(
              nextActionAt,
            ).toISOString()
          : null,

      notes:
        notes.trim() || null,
    });
  }

  const isLoading =
    isLoadingProcess ||
    isLoadingStore ||
    (store &&
      isLoadingShopping);

  if (isLoading) {
    return (
      <div className="flex min-h-[420px] items-center justify-center">
        <div className="text-center">
          <Loader2 className="mx-auto h-7 w-7 animate-spin text-[#087fdb]" />

          <p className="mt-3 text-sm text-slate-500">
            Carregando operação...
          </p>
        </div>
      </div>
    );
  }

  if (
    isProcessError ||
    isStoreError ||
    !process ||
    !store
  ) {
    return (
      <div className="rounded-2xl border border-red-200 bg-red-50 p-6">
        <h2 className="font-semibold text-red-800">
          Não foi possível carregar a
          operação.
        </h2>

        <p className="mt-1 text-sm text-red-600">
          Verifique se a API está em
          execução e tente novamente.
        </p>

        <Link
          href="/work-queue"
          className="mt-4 inline-flex items-center gap-2 text-sm font-semibold text-red-700 hover:underline"
        >
          <ArrowLeft className="h-4 w-4" />
          Voltar para fila
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <Link
        href="/work-queue"
        className="inline-flex h-9 items-center gap-2 rounded-md px-3 text-sm font-medium text-slate-600 transition hover:bg-slate-100 hover:text-[#00213d]"
      >
        <ArrowLeft className="h-4 w-4" />
        Voltar para fila
      </Link>

      {/* CABEÇALHO */}
      <section className="overflow-hidden rounded-2xl bg-gradient-to-r from-[#041a2d] via-[#062b49] to-[#07466a] shadow-lg">
        <div className="relative px-5 py-6 sm:px-7 sm:py-7">
          <div className="pointer-events-none absolute right-0 top-0 h-48 w-48 rounded-full bg-cyan-300/10 blur-3xl" />

          <div className="relative flex flex-col gap-5 xl:flex-row xl:items-center xl:justify-between">
            <div className="flex items-start gap-4">
              <div className="flex h-14 w-14 shrink-0 items-center justify-center rounded-2xl bg-cyan-300/10 text-cyan-300 ring-1 ring-cyan-300/20">
                <Store className="h-6 w-6" />
              </div>

              <div>
                <p className="text-xs font-bold uppercase tracking-[0.18em] text-cyan-300">
                  Central operacional
                </p>

                <h1 className="mt-1 text-2xl font-bold text-white sm:text-3xl">
                  {store.tradeName}
                </h1>

                <div className="mt-2 flex flex-wrap gap-x-5 gap-y-1 text-sm text-slate-300">
                  <span>
                    Shopping:{" "}
                    <strong className="text-white">
                      {shopping?.name ??
                        store.shoppingName}
                    </strong>
                  </span>

                  <span>
                    LUC:{" "}
                    <strong className="text-white">
                      {store.luc ??
                        "Não informado"}
                    </strong>
                  </span>

                  <span>
                    CNPJ:{" "}
                    <strong className="text-white">
                      {formatDocument(
                        store.document,
                      )}
                    </strong>
                  </span>
                </div>
              </div>
            </div>

            <Button
              type="button"
              onClick={handleSave}
              disabled={
                updateMutation.isPending
              }
              className="h-11 gap-2 bg-[#0b8ee8] px-5 text-white hover:bg-[#0877c5]"
            >
              {updateMutation.isPending ? (
                <>
                  <Loader2 className="h-4 w-4 animate-spin" />
                  Salvando...
                </>
              ) : (
                <>
                  <Save className="h-4 w-4" />
                  Salvar alterações
                </>
              )}
            </Button>
          </div>
        </div>
      </section>

      {updateMutation.isSuccess && (
        <div className="flex items-center gap-3 rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm font-medium text-emerald-700">
          <CheckCircle2 className="h-5 w-5" />
          Alterações salvas com sucesso.
        </div>
      )}

      {updateMutation.isError && (
        <div className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm font-medium text-red-700">
          Não foi possível salvar as
          alterações do processo.
        </div>
      )}

      {/* LOJA + SHOPPING */}
      <section className="grid gap-5 xl:grid-cols-2">
        <InfoCard
          icon={ShoppingBag}
          title="Dados da loja"
          subtitle="Informações cadastrais e técnicas."
        >
          <InfoGrid>
            <Info
              label="Loja"
              value={store.tradeName}
            />

            <Info
              label="LUC"
              value={
                store.luc ??
                "Não informado"
              }
            />

            <Info
              label="CNPJ"
              value={formatDocument(
                store.document,
              )}
            />

            <Info
              label="Razão Social"
              value={
                store.corporateName ??
                "Não informado"
              }
            />

            <Info
              label="ERP / PDV"
              value={
                store.erpName ??
                "Não informado"
              }
            />

            <Info
              label="Nº de caixas"
              value={
                store.numberOfRegisters !=
                null
                  ? String(
                      store.numberOfRegisters,
                    )
                  : "Não informado"
              }
            />

            <Info
              label="Tipo de negócio"
              value={
                store.businessType ??
                "Não informado"
              }
            />

            <Info
              label="Monitoramento"
              value={
                monitoringLabels[
                  store.monitoringStatus
                ] ?? "Não informado"
              }
            />
          </InfoGrid>
        </InfoCard>

        <InfoCard
          icon={Building2}
          title="Dados do shopping"
          subtitle="Acessos e informações operacionais."
        >
          <InfoGrid>
            <Info
              label="Rede"
              value={
                shopping?.shoppingGroupName ??
                "Não informado"
              }
            />

            <Info
              label="Shopping"
              value={
                shopping?.name ??
                store.shoppingName
              }
            />

            <Info
              label="API"
              value={
                shopping?.apiName ??
                "Não informado"
              }
            />

            <Info
              label="E-mail de cadastro"
              value={
                shopping?.registrationEmail ??
                "Não informado"
              }
            />
          </InfoGrid>

          <div className="mt-5 grid gap-3 sm:grid-cols-3">
            <ExternalAction
              icon={Globe2}
              label="Site"
              url={
                shopping?.websiteUrl ??
                null
              }
            />

            <ExternalAction
              icon={Server}
              label="ControlShop"
              url={
                shopping?.controlShopUrl ??
                null
              }
            />

            <ExternalAction
              icon={ExternalLink}
              label="Portal"
              url={
                shopping?.portalUrl ??
                null
              }
            />
          </div>
        </InfoCard>
      </section>

      {/* EMAILS OPERACIONAIS */}
      <section className="grid gap-5 lg:grid-cols-2">
        <CopyCard
          icon={Mail}
          title="E-mail de leitura XML"
          description="Endereço utilizado para recebimento/leitura de XML."
          value={
            shopping?.xmlReadingEmail ??
            null
          }
          copied={
            copiedKey === "xml-email"
          }
          onCopy={() =>
            copyText(
              "xml-email",
              shopping?.xmlReadingEmail ??
                null,
            )
          }
        />

        <CopyCard
          icon={Mail}
          title="E-mail de cadastro"
          description="E-mail operacional utilizado no cadastro/configuração."
          value={
            shopping?.registrationEmail ??
            null
          }
          copied={
            copiedKey ===
            "registration-email"
          }
          onCopy={() =>
            copyText(
              "registration-email",
              shopping?.registrationEmail ??
                null,
            )
          }
        />
      </section>

      {/* APONTAMENTO */}
      <section className="overflow-hidden rounded-2xl border border-[#087fdb]/20 bg-white shadow-sm">
        <div className="border-b border-slate-100 bg-[#f5f9fd] px-5 py-4">
          <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <div className="flex items-center gap-3">
              <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-[#087fdb]/10 text-[#087fdb]">
                <ClipboardList className="h-5 w-5" />
              </div>

              <div>
                <h2 className="font-bold text-[#00213d]">
                  Apontamento
                </h2>

                <p className="text-sm text-slate-500">
                  Dados prontos para repassar
                  à configuração externa.
                </p>
              </div>
            </div>

            <Button
              type="button"
              variant="outline"
              onClick={() =>
                copyText(
                  "apontamento",
                  apontamento,
                )
              }
              className="gap-2"
            >
              {copiedKey ===
              "apontamento" ? (
                <>
                  <ClipboardCheck className="h-4 w-4 text-emerald-600" />
                  Copiado
                </>
              ) : (
                <>
                  <Clipboard className="h-4 w-4" />
                  Copiar apontamento
                </>
              )}
            </Button>
          </div>
        </div>

        <div className="p-5">
          <pre className="whitespace-pre-wrap rounded-xl bg-[#061d31] p-4 font-sans text-sm leading-7 text-slate-200">
            {apontamento}
          </pre>
        </div>
      </section>

      {/* PROCESSO */}
      <section className="grid gap-5 xl:grid-cols-[2fr_1fr]">
        <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
          <div className="mb-5 flex items-center gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-[#087fdb]/10 text-[#087fdb]">
              <ClipboardList className="h-5 w-5" />
            </div>

            <div>
              <h2 className="font-bold text-[#00213d]">
                Dados operacionais
              </h2>

              <p className="text-sm text-slate-500">
                Atualize rapidamente o
                processo da loja.
              </p>
            </div>
          </div>

          <div className="grid gap-4 md:grid-cols-2">
            <Field label="Status">
              <select
                value={status}
                onChange={(event) =>
                  setStatus(
                    Number(
                      event.target.value,
                    ) as StoreProcessStatus,
                  )
                }
                className="h-10 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-[#00213d] outline-none focus:border-[#087fdb] focus:ring-2 focus:ring-[#087fdb]/10"
              >
                {statusOptions.map(
                  (option) => (
                    <option
                      key={option.value}
                      value={option.value}
                    >
                      {option.label}
                    </option>
                  ),
                )}
              </select>
            </Field>

            <Field label="Prioridade">
              <select
                value={priority}
                onChange={(event) =>
                  setPriority(
                    Number(
                      event.target.value,
                    ) as ProcessPriority,
                  )
                }
                className="h-10 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-[#00213d] outline-none focus:border-[#087fdb] focus:ring-2 focus:ring-[#087fdb]/10"
              >
                {priorityOptions.map(
                  (option) => (
                    <option
                      key={option.value}
                      value={option.value}
                    >
                      {option.label}
                    </option>
                  ),
                )}
              </select>
            </Field>

            <Field label="Periodicidade">
              <select
                value={frequency}
                onChange={(event) =>
                  setFrequency(
                    Number(
                      event.target.value,
                    ) as ProcessFrequency,
                  )
                }
                className="h-10 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-[#00213d] outline-none focus:border-[#087fdb] focus:ring-2 focus:ring-[#087fdb]/10"
              >
                {frequencyOptions.map(
                  (option) => (
                    <option
                      key={option.value}
                      value={option.value}
                    >
                      {option.label}
                    </option>
                  ),
                )}
              </select>
            </Field>

            <Field label="Prazo da próxima ação">
              <Input
                type="datetime-local"
                value={nextActionAt}
                onChange={(event) =>
                  setNextActionAt(
                    event.target.value,
                  )
                }
              />
            </Field>

            <div className="md:col-span-2">
              <Field label="Próxima ação">
                <Input
                  value={nextAction}
                  onChange={(event) =>
                    setNextAction(
                      event.target.value,
                    )
                  }
                  placeholder="Descreva a próxima ação..."
                />
              </Field>
            </div>

            <div className="md:col-span-2">
              <label className="mb-2 block text-sm font-semibold text-slate-700">
                Observações
              </label>

              <textarea
                value={notes}
                onChange={(event) =>
                  setNotes(
                    event.target.value,
                  )
                }
                rows={5}
                className="w-full rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-[#00213d] outline-none transition focus:border-[#087fdb] focus:ring-2 focus:ring-[#087fdb]/10"
                placeholder="Observações do processo..."
              />
            </div>
          </div>
        </div>

        <div className="space-y-5">
          <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
            <h2 className="font-bold text-[#00213d]">
              Resumo do processo
            </h2>

            <div className="mt-5 space-y-4">
              <SummaryItem
                icon={Wrench}
                label="Tipo de instalação"
                value={
                  process.installationTypeName ??
                  "Não definido"
                }
              />

              <SummaryItem
                icon={UserRound}
                label="Técnico responsável"
                value={
                  process.responsibleUserName ??
                  "Não definido"
                }
              />

              <SummaryItem
                icon={CalendarClock}
                label="Início"
                value={formatDateTime(
                  process.startedAt,
                )}
              />

              <SummaryItem
                icon={CheckCircle2}
                label="Conclusão"
                value={formatDateTime(
                  process.completedAt,
                )}
              />

              <SummaryItem
                icon={MonitorCheck}
                label="Situação"
                value={
                  process.isClosed
                    ? "Fechado"
                    : "Aberto"
                }
              />
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}

type FieldProps = {
  label: string;
  children: React.ReactNode;
};

function Field({
  label,
  children,
}: FieldProps) {
  return (
    <div>
      <label className="mb-2 block text-sm font-semibold text-slate-700">
        {label}
      </label>

      {children}
    </div>
  );
}

type InfoCardProps = {
  icon: React.ElementType;
  title: string;
  subtitle: string;
  children: React.ReactNode;
};

function InfoCard({
  icon: Icon,
  title,
  subtitle,
  children,
}: InfoCardProps) {
  return (
    <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <div className="mb-5 flex items-center gap-3">
        <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-[#087fdb]/10 text-[#087fdb]">
          <Icon className="h-5 w-5" />
        </div>

        <div>
          <h2 className="font-bold text-[#00213d]">
            {title}
          </h2>

          <p className="text-sm text-slate-500">
            {subtitle}
          </p>
        </div>
      </div>

      {children}
    </div>
  );
}

function InfoGrid({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="grid gap-3 sm:grid-cols-2">
      {children}
    </div>
  );
}

function Info({
  label,
  value,
}: {
  label: string;
  value: string;
}) {
  return (
    <div className="rounded-xl bg-slate-50 p-3">
      <p className="text-xs font-bold uppercase tracking-wide text-slate-400">
        {label}
      </p>

      <p className="mt-1 break-words text-sm font-semibold text-[#00213d]">
        {value}
      </p>
    </div>
  );
}

type ExternalActionProps = {
  icon: React.ElementType;
  label: string;
  url: string | null;
};

function ExternalAction({
  icon: Icon,
  label,
  url,
}: ExternalActionProps) {
  const normalizedUrl =
    normalizeUrl(url);

  if (!normalizedUrl) {
    return (
      <div className="flex h-11 items-center justify-center gap-2 rounded-xl border border-slate-200 bg-slate-50 text-sm font-semibold text-slate-400">
        <Icon className="h-4 w-4" />
        {label}
      </div>
    );
  }

  return (
    <a
      href={normalizedUrl}
      target="_blank"
      rel="noreferrer"
      className="flex h-11 items-center justify-center gap-2 rounded-xl border border-[#087fdb]/20 bg-[#087fdb]/5 text-sm font-semibold text-[#087fdb] transition hover:bg-[#087fdb]/10"
    >
      <Icon className="h-4 w-4" />
      {label}
      <ExternalLink className="h-3.5 w-3.5" />
    </a>
  );
}

type CopyCardProps = {
  icon: React.ElementType;
  title: string;
  description: string;
  value: string | null;
  copied: boolean;
  onCopy: () => void;
};

function CopyCard({
  icon: Icon,
  title,
  description,
  value,
  copied,
  onCopy,
}: CopyCardProps) {
  return (
    <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <div className="flex items-start gap-3">
        <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-[#087fdb]/10 text-[#087fdb]">
          <Icon className="h-5 w-5" />
        </div>

        <div className="min-w-0 flex-1">
          <h2 className="font-bold text-[#00213d]">
            {title}
          </h2>

          <p className="mt-1 text-sm text-slate-500">
            {description}
          </p>

          <div className="mt-4 flex flex-col gap-3 sm:flex-row sm:items-center">
            <div className="min-w-0 flex-1 rounded-lg bg-slate-50 px-3 py-2 text-sm font-semibold text-slate-700">
              {value ?? "Não informado"}
            </div>

            <Button
              type="button"
              variant="outline"
              onClick={onCopy}
              disabled={!value}
              className="gap-2"
            >
              {copied ? (
                <>
                  <ClipboardCheck className="h-4 w-4 text-emerald-600" />
                  Copiado
                </>
              ) : (
                <>
                  <Clipboard className="h-4 w-4" />
                  Copiar
                </>
              )}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}

type SummaryItemProps = {
  icon: React.ElementType;
  label: string;
  value: string;
};

function SummaryItem({
  icon: Icon,
  label,
  value,
}: SummaryItemProps) {
  return (
    <div className="flex gap-3 rounded-xl bg-slate-50 p-3">
      <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-[#087fdb]/10 text-[#087fdb]">
        <Icon className="h-4 w-4" />
      </div>

      <div className="min-w-0">
        <p className="text-xs font-bold uppercase tracking-wide text-slate-400">
          {label}
        </p>

        <p className="mt-1 break-words text-sm font-semibold text-[#00213d]">
          {value}
        </p>
      </div>
    </div>
  );
}