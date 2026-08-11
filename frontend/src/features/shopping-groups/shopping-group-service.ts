import { api } from "@/lib/api";

import type { ShoppingGroup } from "./shopping-group-types";

export async function getShoppingGroups(): Promise<ShoppingGroup[]> {
  const response = await api.get<ShoppingGroup[]>(
    "/shopping-groups",
  );

  return response.data;
}