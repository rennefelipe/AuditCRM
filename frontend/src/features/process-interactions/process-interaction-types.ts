export type InteractionChannel =
  | 1
  | 2
  | 3
  | 4
  | 5;

export type ProcessInteraction = {
  id: string;

  storeProcessId: string;

  responsibleUserId: string | null;
  responsibleUserName: string | null;

  storeContactId: string | null;
  storeContactName: string | null;

  channel: InteractionChannel;

  occurredAt: string;

  description: string;

  result: string | null;

  nextAction: string | null;
  nextActionAt: string | null;

  createdAt: string;
};

export type CreateProcessInteractionRequest = {
  storeProcessId: string;

  responsibleUserId: string | null;
  storeContactId: string | null;

  channel: InteractionChannel;

  occurredAt: string;

  description: string;

  result: string | null;

  nextAction: string | null;
  nextActionAt: string | null;
};

export type UpdateProcessInteractionRequest = {
  responsibleUserId: string | null;
  storeContactId: string | null;

  channel: InteractionChannel;

  occurredAt: string;

  description: string;

  result: string | null;

  nextAction: string | null;
  nextActionAt: string | null;
};

export const interactionChannelOptions: {
  value: InteractionChannel;
  label: string;
}[] = [
  {
    value: 1,
    label: "E-mail",
  },
  {
    value: 2,
    label: "WhatsApp",
  },
  {
    value: 3,
    label: "Telefone",
  },
  {
    value: 4,
    label: "Presencial",
  },
  {
    value: 5,
    label: "Outro",
  },
];

export function getInteractionChannelLabel(
  channel: InteractionChannel,
) {
  return (
    interactionChannelOptions.find(
      (option) =>
        option.value === channel,
    )?.label ?? "Outro"
  );
}