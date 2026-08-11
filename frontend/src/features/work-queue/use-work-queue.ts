"use client";

import { useQuery } from "@tanstack/react-query";

import {
  getWorkQueue,
  type WorkQueueFilters,
} from "./work-queue-service";

export function useWorkQueue(
  filters: WorkQueueFilters,
) {
  return useQuery({
    queryKey: [
      "work-queue",
      filters.shoppingGroupId ?? null,
      filters.shoppingId ?? null,
      filters.responsibleUserId ?? null,
      filters.overdueOnly ?? false,
    ],
    queryFn: () => getWorkQueue(filters),
  });
}