"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";

import {
  createProcessInteraction,
  deleteProcessInteraction,
  getProcessInteractions,
  updateProcessInteraction,
} from "./process-interaction-service";

import type {
  CreateProcessInteractionRequest,
  UpdateProcessInteractionRequest,
} from "./process-interaction-types";

export function useProcessInteractions(
  storeProcessId?: string,
) {
  return useQuery({
    queryKey: [
      "process-interactions",
      storeProcessId ?? null,
    ],

    queryFn: () =>
      getProcessInteractions(
        storeProcessId,
      ),

    enabled: Boolean(storeProcessId),
  });
}

export function useCreateProcessInteraction(
  storeProcessId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request: CreateProcessInteractionRequest,
    ) =>
      createProcessInteraction(
        request,
      ),

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: [
            "process-interactions",
            storeProcessId,
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "store-process",
            storeProcessId,
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "store-process-timeline",
            storeProcessId,
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

export function useUpdateProcessInteraction(
  storeProcessId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: ({
      interactionId,
      request,
    }: {
      interactionId: string;
      request: UpdateProcessInteractionRequest;
    }) =>
      updateProcessInteraction(
        interactionId,
        request,
      ),

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: [
            "process-interactions",
            storeProcessId,
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "store-process-timeline",
            storeProcessId,
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

export function useDeleteProcessInteraction(
  storeProcessId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: (
      interactionId: string,
    ) =>
      deleteProcessInteraction(
        interactionId,
      ),

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: [
            "process-interactions",
            storeProcessId,
          ],
        }),

        queryClient.invalidateQueries({
          queryKey: [
            "store-process-timeline",
            storeProcessId,
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