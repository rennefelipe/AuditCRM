"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";

import {
  createProcessInstallation,
  deleteProcessInstallation,
  getProcessInstallations,
  updateProcessInstallation,
} from "./process-installation-service";

import type {
  CreateProcessInstallationRequest,
  UpdateProcessInstallationRequest,
} from "./process-installation-types";

export function useProcessInstallations(
  storeProcessId?: string,
) {
  return useQuery({
    queryKey: [
      "process-installations",
      storeProcessId ?? null,
    ],

    queryFn: () =>
      getProcessInstallations(
        storeProcessId,
      ),

    enabled: Boolean(
      storeProcessId,
    ),
  });
}

export function useCreateProcessInstallation(
  storeProcessId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request: CreateProcessInstallationRequest,
    ) =>
      createProcessInstallation(
        request,
      ),

    onSuccess: async () => {
      await refreshInstallationQueries(
        queryClient,
        storeProcessId,
      );
    },
  });
}

export function useUpdateProcessInstallation(
  storeProcessId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: ({
      installationId,
      request,
    }: {
      installationId: string;
      request: UpdateProcessInstallationRequest;
    }) =>
      updateProcessInstallation(
        installationId,
        request,
      ),

    onSuccess: async () => {
      await refreshInstallationQueries(
        queryClient,
        storeProcessId,
      );
    },
  });
}

export function useDeleteProcessInstallation(
  storeProcessId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: (
      installationId: string,
    ) =>
      deleteProcessInstallation(
        installationId,
      ),

    onSuccess: async () => {
      await refreshInstallationQueries(
        queryClient,
        storeProcessId,
      );
    },
  });
}

async function refreshInstallationQueries(
  queryClient: ReturnType<
    typeof useQueryClient
  >,
  storeProcessId: string,
) {
  await Promise.all([
    queryClient.invalidateQueries({
      queryKey: [
        "process-installations",
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

    queryClient.invalidateQueries({
      queryKey: [
        "dashboard",
      ],
    }),
  ]);
}