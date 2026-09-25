"use client";

import { useQuery } from "@tanstack/react-query";

import {
  getShoppingById,
  getShoppings,
} from "./shopping-service";

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

export function useShopping(
  shoppingId?: string,
) {
  return useQuery({
    queryKey: [
      "shopping",
      shoppingId ?? null,
    ],

    queryFn: () =>
      getShoppingById(
        shoppingId as string,
      ),

    enabled: Boolean(shoppingId),
  });
}