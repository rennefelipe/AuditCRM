"use client";

import { useQuery } from "@tanstack/react-query";

import {
  getDashboardSummary,
  type DashboardFilters,
} from "./dashboard-service";

export function useDashboardSummary(
  filters: DashboardFilters,
) {
  return useQuery({
    queryKey: [
      "dashboard-summary",
      filters.shoppingGroupId ?? null,
      filters.shoppingId ?? null,
      filters.responsibleUserId ?? null,
    ],
    queryFn: () => getDashboardSummary(filters),
  });
}