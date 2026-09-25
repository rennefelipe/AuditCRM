"use client";

import { useQuery } from "@tanstack/react-query";

import {
  getWorkQueue,
  type WorkQueueFilters,
} from "./work-queue-service";

export function useWorkQueue(filters: WorkQueueFilters) {
  return useQuery({
    queryKey: [
      "work-queue",
      {
        shoppingGroupId: filters.shoppingGroupId ?? null,
        shoppingId: filters.shoppingId ?? null,
        storeId: filters.storeId ?? null,
        search: filters.search ?? null,
        installationTypeId: filters.installationTypeId ?? null,
        status: filters.status ?? null,
        responsibleUserId: filters.responsibleUserId ?? null,
        frequency: filters.frequency ?? null,
        priority: filters.priority ?? null,
        overdueOnly: filters.overdueOnly ?? null,
      },
    ],

    queryFn: () => getWorkQueue(filters),
  });
}