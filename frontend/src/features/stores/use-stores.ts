"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";

import {
  activateStore,
  deactivateStore,
  deleteStore,
  getStoreById,
  getStores,
  updateStore,
  type UpdateStoreRequest,
} from "./store-service";

export function useStores(
  shoppingId?: string,
  shoppingGroupId?: string,
) {
  return useQuery({
    queryKey: [
      "stores",
      shoppingId ?? null,
      shoppingGroupId ?? null,
    ],

    queryFn: () =>
      getStores(
        shoppingId,
        shoppingGroupId,
      ),

    enabled:
      Boolean(shoppingId) ||
      Boolean(shoppingGroupId),
  });
}

export function useStore(
  storeId?: string,
) {
  return useQuery({
    queryKey: [
      "store",
      storeId ?? null,
    ],

    queryFn: () =>
      getStoreById(
        storeId as string,
      ),

    enabled: Boolean(storeId),
  });
}

export function useUpdateStore(
  storeId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request: UpdateStoreRequest,
    ) =>
      updateStore(
        storeId,
        request,
      ),

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: [
            "store",
            storeId,
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "stores",
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "work-queue",
          ],
        }),
      ]);
    },
  });
}

export function useActivateStore(
  storeId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: () =>
      activateStore(storeId),

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: [
            "store",
            storeId,
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "stores",
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "work-queue",
          ],
        }),
      ]);
    },
  });
}

export function useDeactivateStore(
  storeId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: () =>
      deactivateStore(storeId),

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: [
            "store",
            storeId,
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "stores",
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "work-queue",
          ],
        }),
      ]);
    },
  });
}

export function useDeleteStore(
  storeId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: () =>
      deleteStore(storeId),

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: [
            "stores",
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "work-queue",
          ],
        }),
      ]);
    },
  });
}