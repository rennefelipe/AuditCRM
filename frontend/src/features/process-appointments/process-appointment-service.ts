import { api } from "@/lib/api";

import type {
  CreateProcessAppointmentRequest,
  ProcessAppointment,
  UpdateProcessAppointmentRequest,
} from "./process-appointment-types";

export async function getProcessAppointments(
  storeProcessId?: string,
  responsibleUserId?: string,
): Promise<ProcessAppointment[]> {
  const response =
    await api.get<ProcessAppointment[]>(
      "/process-appointments",
      {
        params: {
          storeProcessId:
            storeProcessId || undefined,

          responsibleUserId:
            responsibleUserId || undefined,
        },
      },
    );

  return response.data;
}

export async function getProcessAppointmentById(
  appointmentId: string,
): Promise<ProcessAppointment> {
  const response =
    await api.get<ProcessAppointment>(
      `/process-appointments/${appointmentId}`,
    );

  return response.data;
}

export async function createProcessAppointment(
  request: CreateProcessAppointmentRequest,
): Promise<ProcessAppointment> {
  const response =
    await api.post<ProcessAppointment>(
      "/process-appointments",
      request,
    );

  return response.data;
}

export async function updateProcessAppointment(
  appointmentId: string,
  request: UpdateProcessAppointmentRequest,
): Promise<ProcessAppointment> {
  const response =
    await api.put<ProcessAppointment>(
      `/process-appointments/${appointmentId}`,
      request,
    );

  return response.data;
}

export async function confirmProcessAppointment(
  appointmentId: string,
): Promise<void> {
  await api.patch(
    `/process-appointments/${appointmentId}/confirm`,
  );
}

export async function completeProcessAppointment(
  appointmentId: string,
): Promise<void> {
  await api.patch(
    `/process-appointments/${appointmentId}/complete`,
  );
}

export async function cancelProcessAppointment(
  appointmentId: string,
): Promise<void> {
  await api.patch(
    `/process-appointments/${appointmentId}/cancel`,
  );
}

export async function rescheduleProcessAppointment(
  appointmentId: string,
  scheduledAt: string,
): Promise<void> {
  await api.patch(
    `/process-appointments/${appointmentId}/reschedule`,
    {
      scheduledAt,
    },
  );
}

export async function deleteProcessAppointment(
  appointmentId: string,
): Promise<void> {
  await api.delete(
    `/process-appointments/${appointmentId}`,
  );
}