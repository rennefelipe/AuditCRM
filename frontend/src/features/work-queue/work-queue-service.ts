import { api } from "@/lib/api";

export type WorkQueueFilters = {
  shoppingGroupId?: string;
  shoppingId?: string;
  storeId?: string;
  search?: string;
  installationTypeId?: string;
  status?: number;
  responsibleUserId?: string;
  frequency?: number;
  priority?: number;
  overdueOnly?: boolean;
};

export type WorkQueueItem = {
  storeProcessId: string;

  storeId: string;
  storeName: string;
  storeLuc: string | null;
  storeDocument: string | null;

  shoppingId: string;
  shoppingName: string;

  shoppingGroupId: string | null;
  shoppingGroupName: string | null;

  installationTypeId: string | null;
  installationTypeName: string | null;

  status: number;
  priority: number;
  frequency: number;

  responsibleUserId: string | null;
  responsibleUserName: string | null;

  nextAction: string | null;
  nextActionAt: string | null;

  startedAt: string;
  completedAt: string | null;

  isClosed: boolean;
};

export async function getWorkQueue(
  filters: WorkQueueFilters,
): Promise<WorkQueueItem[]> {
  const response = await api.get<WorkQueueItem[]>(
    "/work-queue",
    {
      params: {
        shoppingGroupId:
          filters.shoppingGroupId || undefined,

        shoppingId:
          filters.shoppingId || undefined,

        storeId:
          filters.storeId || undefined,

        search:
          filters.search?.trim() || undefined,

        installationTypeId:
          filters.installationTypeId || undefined,

        status:
          filters.status ?? undefined,

        responsibleUserId:
          filters.responsibleUserId || undefined,

        frequency:
          filters.frequency ?? undefined,

        priority:
          filters.priority ?? undefined,

        overdueOnly:
          filters.overdueOnly ?? undefined,
      },
    },
  );

  return response.data;
}