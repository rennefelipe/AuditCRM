export type AppointmentStatus =
  | 1
  | 2
  | 3
  | 4
  | 5;

export type ProcessAppointment = {
  id: string;

  storeProcessId: string;

  responsibleUserId: string | null;
  responsibleUserName: string | null;

  storeContactId: string | null;
  storeContactName: string | null;

  scheduledAt: string;

  status: AppointmentStatus;

  notes: string | null;

  completedAt: string | null;
  cancelledAt: string | null;

  createdAt: string;
};

export type CreateProcessAppointmentRequest = {
  storeProcessId: string;

  scheduledAt: string;

  responsibleUserId: string | null;

  storeContactId: string | null;

  notes: string | null;
};

export type UpdateProcessAppointmentRequest = {
  scheduledAt: string;

  responsibleUserId: string | null;

  storeContactId: string | null;

  notes: string | null;
};

export const appointmentStatusOptions: {
  value: AppointmentStatus;
  label: string;
}[] = [
  {
    value: 1,
    label: "Agendado",
  },
  {
    value: 2,
    label: "Confirmado",
  },
  {
    value: 3,
    label: "Concluído",
  },
  {
    value: 4,
    label: "Cancelado",
  },
  {
    value: 5,
    label: "Reagendado",
  },
];

export function getAppointmentStatusLabel(
  status: AppointmentStatus,
) {
  return (
    appointmentStatusOptions.find(
      (option) =>
        option.value === status,
    )?.label ?? "Desconhecido"
  );
}