import { api } from "@/lib/api";

import type {
  MonitoringStatus,
  Store,
} from "./store-types";

export type UpdateStoreRequest = {
  shoppingId: string;
  erpId: string | null;

  tradeName: string;
  luc: string | null;

  corporateName: string | null;
  document: string | null;

  stateRegistration: string | null;

  numberOfRegisters: number | null;

  monitoringStatus: MonitoringStatus;

  businessType: string | null;

  notes: string | null;
};

export async function getStores(
  shoppingId?: string,
  shoppingGroupId?: string,
): Promise<Store[]> {
  const response = await api.get<Store[]>(
    "/stores",
    {
      params: {
        shoppingId:
          shoppingId || undefined,

        shoppingGroupId:
          shoppingGroupId || undefined,
      },
    },
  );

  return response.data;
}

export async function getStoreById(
  storeId: string,
): Promise<Store> {
  const response = await api.get<Store>(
    `/stores/${storeId}`,
  );

  return response.data;
}

export async function updateStore(
  storeId: string,
  request: UpdateStoreRequest,
): Promise<Store> {
  const response = await api.put<Store>(
    `/stores/${storeId}`,
    request,
  );

  return response.data;
}

export async function activateStore(
  storeId: string,
): Promise<void> {
  await api.patch(
    `/stores/${storeId}/activate`,
  );
}

export async function deactivateStore(
  storeId: string,
): Promise<void> {
  await api.patch(
    `/stores/${storeId}/deactivate`,
  );
}

export async function deleteStore(
  storeId: string,
): Promise<void> {
  await api.delete(
    `/stores/${storeId}`,
  );
}