"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";

import {
  closeStoreProcess,
  getStoreProcess,
  getStoreProcessTimeline,
  reopenStoreProcess,
  updateStoreProcess,
  type UpdateStoreProcessRequest,
} from "./store-process-service";

export function useStoreProcess(
  processId: string,
) {
  return useQuery({
    queryKey: [
      "store-process",
      processId,
    ],

    queryFn: () =>
      getStoreProcess(processId),

    enabled: Boolean(processId),
  });
}

export function useStoreProcessTimeline(
  processId: string,
) {
  return useQuery({
    queryKey: [
      "store-process-timeline",
      processId,
    ],

    queryFn: () =>
      getStoreProcessTimeline(processId),

    enabled: Boolean(processId),
  });
}

export function useUpdateStoreProcess(
  processId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request: UpdateStoreProcessRequest,
    ) =>
      updateStoreProcess(
        processId,
        request,
      ),

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: [
            "store-process",
            processId,
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "store-process-timeline",
            processId,
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "work-queue",
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "dashboard",
          ],
        }),
      ]);
    },
  });
}

export function useCloseStoreProcess(
  processId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: () =>
      closeStoreProcess(processId),

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: [
            "store-process",
            processId,
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "store-process-timeline",
            processId,
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "work-queue",
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "dashboard",
          ],
        }),
      ]);
    },
  });
}

export function useReopenStoreProcess(
  processId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: () =>
      reopenStoreProcess(processId),

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: [
            "store-process",
            processId,
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "store-process-timeline",
            processId,
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "work-queue",
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "dashboard",
          ],
        }),
      ]);
    },
  });
}