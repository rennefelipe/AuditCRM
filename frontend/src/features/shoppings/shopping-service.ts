import { api } from "@/lib/api";

import type { Shopping } from "./shopping-types";

export async function getShoppings(
  shoppingGroupId?: string,
): Promise<Shopping[]> {
  const response = await api.get<Shopping[]>(
    "/shoppings",
    {
      params: {
        shoppingGroupId:
          shoppingGroupId || undefined,
      },
    },
  );

  return response.data;
}

export async function getShoppingById(
  shoppingId: string,
): Promise<Shopping> {
  const response = await api.get<Shopping>(
    `/shoppings/${shoppingId}`,
  );

  return response.data;
}