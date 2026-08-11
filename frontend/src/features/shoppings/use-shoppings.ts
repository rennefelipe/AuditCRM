"use client";

import { useQuery } from "@tanstack/react-query";

import { getShoppings } from "./shopping-service";

export function useShoppings(
  shoppingGroupId?: string,
) {
  return useQuery({
    queryKey: [
      "shoppings",
      shoppingGroupId ?? null,
    ],
    queryFn: () =>
      getShoppings(shoppingGroupId),
    enabled: Boolean(shoppingGroupId),
  });
}