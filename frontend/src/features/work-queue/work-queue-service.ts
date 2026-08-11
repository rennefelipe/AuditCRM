import { api } from "@/lib/api";

import type { WorkQueueItem } from "./work-queue-types";

export type WorkQueueFilters = {
  shoppingGroupId?: string;
  shoppingId?: string;
  responsibleUserId?: string;
  overdueOnly?: boolean;
};

export async function getWorkQueue(
  filters: WorkQueueFilters = {},
): Promise<WorkQueueItem[]> {
  const response = await api.get<WorkQueueItem[]>(
    "/work-queue",
    {
      params: {
        shoppingGroupId:
          filters.shoppingGroupId || undefined,
        shoppingId:
          filters.shoppingId || undefined,
        responsibleUserId:
          filters.responsibleUserId || undefined,
        overdueOnly:
          filters.overdueOnly === true
            ? true
            : undefined,
      },
    },
  );

  return response.data;
}