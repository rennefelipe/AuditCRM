"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";

import {
  cancelProcessAppointment,
  completeProcessAppointment,
  confirmProcessAppointment,
  createProcessAppointment,
  deleteProcessAppointment,
  getProcessAppointments,
  rescheduleProcessAppointment,
  updateProcessAppointment,
} from "./process-appointment-service";

import type {
  CreateProcessAppointmentRequest,
  UpdateProcessAppointmentRequest,
} from "./process-appointment-types";

export function useProcessAppointments(
  storeProcessId?: string,
  responsibleUserId?: string,
) {
  return useQuery({
    queryKey: [
      "process-appointments",
      storeProcessId ?? null,
      responsibleUserId ?? null,
    ],

    queryFn: () =>
      getProcessAppointments(
        storeProcessId,
        responsibleUserId,
      ),

    enabled:
      Boolean(storeProcessId) ||
      Boolean(responsibleUserId),
  });
}

export function useCreateProcessAppointment(
  storeProcessId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request: CreateProcessAppointmentRequest,
    ) =>
      createProcessAppointment(
        request,
      ),

    onSuccess: async () => {
      await refreshAppointmentQueries(
        queryClient,
        storeProcessId,
      );
    },
  });
}

export function useUpdateProcessAppointment(
  storeProcessId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: ({
      appointmentId,
      request,
    }: {
      appointmentId: string;
      request: UpdateProcessAppointmentRequest;
    }) =>
      updateProcessAppointment(
        appointmentId,
        request,
      ),

    onSuccess: async () => {
      await refreshAppointmentQueries(
        queryClient,
        storeProcessId,
      );
    },
  });
}

export function useConfirmProcessAppointment(
  storeProcessId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: (
      appointmentId: string,
    ) =>
      confirmProcessAppointment(
        appointmentId,
      ),

    onSuccess: async () => {
      await refreshAppointmentQueries(
        queryClient,
        storeProcessId,
      );
    },
  });
}

export function useCompleteProcessAppointment(
  storeProcessId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: (
      appointmentId: string,
    ) =>
      completeProcessAppointment(
        appointmentId,
      ),

    onSuccess: async () => {
      await refreshAppointmentQueries(
        queryClient,
        storeProcessId,
      );
    },
  });
}

export function useCancelProcessAppointment(
  storeProcessId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: (
      appointmentId: string,
    ) =>
      cancelProcessAppointment(
        appointmentId,
      ),

    onSuccess: async () => {
      await refreshAppointmentQueries(
        queryClient,
        storeProcessId,
      );
    },
  });
}

export function useRescheduleProcessAppointment(
  storeProcessId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: ({
      appointmentId,
      scheduledAt,
    }: {
      appointmentId: string;
      scheduledAt: string;
    }) =>
      rescheduleProcessAppointment(
        appointmentId,
        scheduledAt,
      ),

    onSuccess: async () => {
      await refreshAppointmentQueries(
        queryClient,
        storeProcessId,
      );
    },
  });
}

export function useDeleteProcessAppointment(
  storeProcessId: string,
) {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn: (
      appointmentId: string,
    ) =>
      deleteProcessAppointment(
        appointmentId,
      ),

    onSuccess: async () => {
      await refreshAppointmentQueries(
        queryClient,
        storeProcessId,
      );
    },
  });
}

async function refreshAppointmentQueries(
  queryClient: ReturnType<
    typeof useQueryClient
  >,
  storeProcessId: string,
) {
  await Promise.all([
    queryClient.invalidateQueries({
      queryKey: [
        "process-appointments",
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