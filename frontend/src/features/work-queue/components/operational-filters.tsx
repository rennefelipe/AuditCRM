"use client";

import {
  Building2,
  CalendarClock,
  Filter,
  Gauge,
  Network,
  RotateCcw,
  Search,
  Store,
  UserRound,
  Wrench,
} from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useShoppingGroups } from "@/features/shopping-groups/use-shopping-groups";
import { useShoppings } from "@/features/shoppings/use-shoppings";

export type OperationalFilterValues = {
  shoppingGroupId?: string;
  shoppingId?: string;
  storeId?: string;
  search?: string;
  installationTypeId?: string;
  status?: number;
  responsibleUserId?: string;
  frequency?: number;
  priority?: number;
  overdueOnly?: boolean;
};

type FilterOption = {
  value: string;
  label: string;
};

type OperationalFiltersProps = {
  values: OperationalFilterValues;

  storeOptions?: FilterOption[];
  installationTypeOptions?: FilterOption[];
  responsibleUserOptions?: FilterOption[];

  onChange: (
    values: OperationalFilterValues,
  ) => void;

  onClear: () => void;
};

const statusOptions = [
  { value: "1", label: "Serviço" },
  { value: "2", label: "Instalado" },
  { value: "3", label: "Em tratativas" },
  {
    value: "4",
    label: "Notificado sem retorno",
  },
  {
    value: "5",
    label: "Não autorizado",
  },
  {
    value: "6",
    label: "Saiu do shopping",
  },
];

const frequencyOptions = [
  { value: "1", label: "Diário" },
  { value: "2", label: "Semanal" },
  { value: "3", label: "Quinzenal" },
  { value: "4", label: "Mensal" },
];

const priorityOptions = [
  { value: "1", label: "Baixa" },
  { value: "2", label: "Normal" },
  { value: "3", label: "Alta" },
  { value: "4", label: "Urgente" },
];

export function OperationalFilters({
  values,
  storeOptions = [],
  installationTypeOptions = [],
  responsibleUserOptions = [],
  onChange,
  onClear,
}: OperationalFiltersProps) {
  const {
    data: shoppingGroups = [],
    isLoading: isLoadingGroups,
  } = useShoppingGroups();

  const {
    data: shoppings = [],
    isLoading: isLoadingShoppings,
  } = useShoppings(values.shoppingGroupId);

  function update(
    changes: Partial<OperationalFilterValues>,
  ) {
    onChange({
      ...values,
      ...changes,
    });
  }

  function handleGroupChange(
    value: string | null,
  ) {
    if (!value || value === "all") {
      onChange({
        ...values,
        shoppingGroupId: undefined,
        shoppingId: undefined,
        storeId: undefined,
      });

      return;
    }

    onChange({
      ...values,
      shoppingGroupId: value,
      shoppingId: undefined,
      storeId: undefined,
    });
  }

  function handleShoppingChange(
    value: string | null,
  ) {
    if (!value || value === "all") {
      onChange({
        ...values,
        shoppingId: undefined,
        storeId: undefined,
      });

      return;
    }

    onChange({
      ...values,
      shoppingId: value,
      storeId: undefined,
    });
  }

  return (
    <section className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
      <div className="flex flex-col gap-3 border-b border-slate-200 bg-[#f8fafc] px-4 py-3 lg:flex-row lg:items-center lg:justify-between">
        <div className="flex items-center gap-3">
          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-[#087fdb]/10 text-[#087fdb]">
            <Filter className="h-4 w-4" />
          </div>

          <div>
            <h2 className="text-sm font-bold text-[#00213d]">
              Filtros
            </h2>

            <p className="text-xs text-slate-500">
              Encontre rapidamente a loja que precisa trabalhar.
            </p>
          </div>
        </div>

        <Button
          type="button"
          variant="outline"
          onClick={onClear}
          className="h-8 gap-2 px-3 text-xs"
        >
          <RotateCcw className="h-3.5 w-3.5" />
          Limpar
        </Button>
      </div>

      <div className="space-y-3 p-4">
        <div className="relative">
          <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" />

          <Input
            value={values.search ?? ""}
            onChange={(event) =>
              update({
                search:
                  event.target.value ||
                  undefined,
              })
            }
            placeholder="Buscar por loja, LUC, CNPJ, contato, e-mail ou telefone..."
            className="h-10 pl-9"
          />
        </div>

        <div className="grid gap-2 md:grid-cols-2 xl:grid-cols-6">
          <FilterSelect
            icon={Network}
            label="Grupo"
            value={values.shoppingGroupId}
            placeholder="Todos"
            disabled={isLoadingGroups}
            options={shoppingGroups
              .filter((group) => group.isActive)
              .map((group) => ({
                value: group.id,
                label: group.name,
              }))}
            onChange={handleGroupChange}
          />

          <FilterSelect
            icon={Building2}
            label="Shopping"
            value={values.shoppingId}
            placeholder={
              values.shoppingGroupId
                ? "Todos"
                : "Selecione grupo"
            }
            disabled={
              !values.shoppingGroupId ||
              isLoadingShoppings
            }
            options={shoppings
              .filter(
                (shopping) =>
                  shopping.isActive,
              )
              .map((shopping) => ({
                value: shopping.id,
                label: shopping.name,
              }))}
            onChange={handleShoppingChange}
          />

          <FilterSelect
            icon={Store}
            label="Loja"
            value={values.storeId}
            placeholder="Todas"
            options={storeOptions}
            onChange={(value) =>
              update({
                storeId:
                  !value || value === "all"
                    ? undefined
                    : value,
              })
            }
          />

          <FilterSelect
            icon={Gauge}
            label="Status"
            value={
              values.status?.toString()
            }
            placeholder="Todos"
            options={statusOptions}
            onChange={(value) =>
              update({
                status:
                  !value || value === "all"
                    ? undefined
                    : Number(value),
              })
            }
          />

          <FilterSelect
            icon={Wrench}
            label="Tipo"
            value={
              values.installationTypeId
            }
            placeholder="Todos"
            options={
              installationTypeOptions
            }
            onChange={(value) =>
              update({
                installationTypeId:
                  !value || value === "all"
                    ? undefined
                    : value,
              })
            }
          />

          <FilterSelect
            icon={UserRound}
            label="Técnico"
            value={
              values.responsibleUserId
            }
            placeholder="Todos"
            options={
              responsibleUserOptions
            }
            onChange={(value) =>
              update({
                responsibleUserId:
                  !value || value === "all"
                    ? undefined
                    : value,
              })
            }
          />

          <FilterSelect
            icon={CalendarClock}
            label="Periodicidade"
            value={
              values.frequency?.toString()
            }
            placeholder="Todas"
            options={frequencyOptions}
            onChange={(value) =>
              update({
                frequency:
                  !value || value === "all"
                    ? undefined
                    : Number(value),
              })
            }
          />

          <FilterSelect
            icon={Gauge}
            label="Prioridade"
            value={
              values.priority?.toString()
            }
            placeholder="Todas"
            options={priorityOptions}
            onChange={(value) =>
              update({
                priority:
                  !value || value === "all"
                    ? undefined
                    : Number(value),
              })
            }
          />

          <div className="xl:col-span-2">
            <button
              type="button"
              onClick={() =>
                update({
                  overdueOnly:
                    values.overdueOnly
                      ? undefined
                      : true,
                })
              }
              className={[
                "flex h-9 w-full items-center justify-between rounded-md border px-3 text-xs font-semibold transition",
                values.overdueOnly
                  ? "border-red-200 bg-red-50 text-red-700"
                  : "border-slate-200 bg-white text-slate-600 hover:bg-slate-50",
              ].join(" ")}
            >
              <span className="flex items-center gap-2">
                <CalendarClock className="h-3.5 w-3.5" />
                Somente atrasadas
              </span>

              <span
                className={[
                  "h-2 w-2 rounded-full",
                  values.overdueOnly
                    ? "bg-red-500"
                    : "bg-slate-300",
                ].join(" ")}
              />
            </button>
          </div>
        </div>
      </div>
    </section>
  );
}

type FilterSelectProps = {
  icon: React.ElementType;
  label: string;
  value?: string;
  placeholder: string;
  options: FilterOption[];
  disabled?: boolean;

  onChange: (
    value: string | null,
  ) => void;
};

function FilterSelect({
  icon: Icon,
  label,
  value,
  placeholder,
  options,
  disabled,
  onChange,
}: FilterSelectProps) {
  return (
    <div className="space-y-1">
      <label className="flex items-center gap-1.5 text-[10px] font-semibold uppercase tracking-wide text-slate-500">
        <Icon className="h-3.5 w-3.5 text-[#087fdb]" />
        {label}
      </label>

      <Select
        value={value ?? "all"}
        onValueChange={onChange}
        disabled={disabled}
      >
        <SelectTrigger className="h-9 w-full bg-white text-sm">
          <SelectValue
            placeholder={placeholder}
          />
        </SelectTrigger>

        <SelectContent>
          <SelectItem value="all">
            {placeholder}
          </SelectItem>

          {options.map((option) => (
            <SelectItem
              key={option.value}
              value={option.value}
            >
              {option.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}