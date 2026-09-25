import { api } from "@/lib/api";

export type StoreProcessStatus =
  | 1
  | 2
  | 3
  | 4
  | 5
  | 6;

export type ProcessPriority =
  | 1
  | 2
  | 3
  | 4;

export type ProcessFrequency =
  | 1
  | 2
  | 3
  | 4;

export type StoreProcess = {
  id: string;

  storeId: string;
  storeName: string;

  installationTypeId: string | null;
  installationTypeName: string | null;

  responsibleUserId: string | null;
  responsibleUserName: string | null;

  status: StoreProcessStatus;
  priority: ProcessPriority;
  frequency: ProcessFrequency;

  nextAction: string | null;
  nextActionAt: string | null;

  startedAt: string;
  completedAt: string | null;

  notes: string | null;

  isClosed: boolean;

  createdAt: string;
};

export type UpdateStoreProcessRequest = {
  installationTypeId: string | null;
  responsibleUserId: string | null;

  status: StoreProcessStatus;
  priority: ProcessPriority;
  frequency: ProcessFrequency;

  nextAction: string | null;
  nextActionAt: string | null;

  notes: string | null;
};

export type ProcessTimelineItem = {
  id?: string;

  type?: string;
  title?: string;
  description?: string;

  occurredAt?: string;
  createdAt?: string;

  userId?: string | null;
  userName?: string | null;

  [key: string]: unknown;
};

export async function getStoreProcess(
  processId: string,
): Promise<StoreProcess> {
  const response =
    await api.get<StoreProcess>(
      `/store-processes/${processId}`,
    );

  return response.data;
}

export async function updateStoreProcess(
  processId: string,
  request: UpdateStoreProcessRequest,
): Promise<StoreProcess> {
  const response =
    await api.put<StoreProcess>(
      `/store-processes/${processId}`,
      request,
    );

  return response.data;
}

export async function getStoreProcessTimeline(
  processId: string,
): Promise<ProcessTimelineItem[]> {
  const response =
    await api.get<ProcessTimelineItem[]>(
      `/store-processes/${processId}/timeline`,
    );

  return response.data;
}

export async function closeStoreProcess(
  processId: string,
): Promise<void> {
  await api.patch(
    `/store-processes/${processId}/close`,
  );
}

export async function reopenStoreProcess(
  processId: string,
): Promise<void> {
  await api.patch(
    `/store-processes/${processId}/reopen`,
  );
}