"use client";

import { Building2, Network } from "lucide-react";

import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useShoppingGroups } from "@/features/shopping-groups/use-shopping-groups";
import { useShoppings } from "@/features/shoppings/use-shoppings";

type DashboardFiltersProps = {
  shoppingGroupId?: string;
  shoppingId?: string;
  onShoppingGroupChange: (value?: string) => void;
  onShoppingChange: (value?: string) => void;
};

export function DashboardFilters({
  shoppingGroupId,
  shoppingId,
  onShoppingGroupChange,
  onShoppingChange,
}: DashboardFiltersProps) {
  const {
    data: shoppingGroups = [],
    isLoading: isLoadingGroups,
  } = useShoppingGroups();

  const {
    data: shoppings = [],
    isLoading: isLoadingShoppings,
  } = useShoppings(shoppingGroupId);

  function handleGroupChange(value: string | null) {
    if (!value || value === "all") {
      onShoppingGroupChange(undefined);
      onShoppingChange(undefined);
      return;
    }

    onShoppingGroupChange(value);
    onShoppingChange(undefined);
  }

  function handleShoppingChange(value: string | null) {
    if (!value || value === "all") {
      onShoppingChange(undefined);
      return;
    }

    onShoppingChange(value);
  }

  return (
    <div className="grid gap-4 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:grid-cols-2">
      <div className="space-y-2">
        <div className="flex items-center gap-2 text-sm font-semibold text-[#00213d]">
          <Network className="h-4 w-4 text-[#007dd8]" />
          Rede
        </div>

        <Select
          value={shoppingGroupId ?? "all"}
          onValueChange={handleGroupChange}
          disabled={isLoadingGroups}
        >
          <SelectTrigger className="h-11 w-full">
            <SelectValue placeholder="Todas as redes" />
          </SelectTrigger>

          <SelectContent>
            <SelectItem value="all">
              Todas as redes
            </SelectItem>

            {shoppingGroups
              .filter((group) => group.isActive)
              .map((group) => (
                <SelectItem
                  key={group.id}
                  value={group.id}
                >
                  {group.name}
                </SelectItem>
              ))}
          </SelectContent>
        </Select>
      </div>

      <div className="space-y-2">
        <div className="flex items-center gap-2 text-sm font-semibold text-[#00213d]">
          <Building2 className="h-4 w-4 text-[#007dd8]" />
          Shopping
        </div>

        <Select
          value={shoppingId ?? "all"}
          onValueChange={handleShoppingChange}
          disabled={
            !shoppingGroupId ||
            isLoadingShoppings
          }
        >
          <SelectTrigger className="h-11 w-full">
            <SelectValue
              placeholder={
                shoppingGroupId
                  ? "Todos os shoppings"
                  : "Selecione uma rede primeiro"
              }
            />
          </SelectTrigger>

          <SelectContent>
            <SelectItem value="all">
              Todos os shoppings
            </SelectItem>

            {shoppings
              .filter((shopping) => shopping.isActive)
              .map((shopping) => (
                <SelectItem
                  key={shopping.id}
                  value={shopping.id}
                >
                  {shopping.name}
                </SelectItem>
              ))}
          </SelectContent>
        </Select>
      </div>
    </div>
  );
}