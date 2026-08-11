"use client";

import { useQuery } from "@tanstack/react-query";

import { getShoppingGroups } from "./shopping-group-service";

export function useShoppingGroups() {
  return useQuery({
    queryKey: ["shopping-groups"],
    queryFn: getShoppingGroups,
  });
}