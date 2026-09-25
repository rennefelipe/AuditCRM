export type XmlLocationType =
  | 0
  | 1
  | 2
  | 3;

export type ProcessInstallation = {
  id: string;

  storeProcessId: string;

  responsibleUserId: string | null;
  responsibleUserName: string | null;

  erpId: string | null;
  erpName: string | null;

  installedAt: string;

  successful: boolean;

  xmlLocationType: XmlLocationType;

  numberOfRegisters: number | null;

  result: string | null;

  notes: string | null;

  createdAt: string;
};

export type CreateProcessInstallationRequest = {
  storeProcessId: string;

  installedAt: string;

  successful: boolean;

  responsibleUserId: string | null;

  erpId: string | null;

  xmlLocationType: XmlLocationType;

  numberOfRegisters: number | null;

  result: string | null;

  notes: string | null;
};

export type UpdateProcessInstallationRequest = {
  installedAt: string;

  successful: boolean;

  responsibleUserId: string | null;

  erpId: string | null;

  xmlLocationType: XmlLocationType;

  numberOfRegisters: number | null;

  result: string | null;

  notes: string | null;
};

export const xmlLocationTypeOptions: {
  value: XmlLocationType;
  label: string;
}[] = [
  {
    value: 0,
    label: "Desconhecido",
  },
  {
    value: 1,
    label: "Local",
  },
  {
    value: 2,
    label: "Nuvem",
  },
  {
    value: 3,
    label: "Misto",
  },
];

export function getXmlLocationTypeLabel(
  value: XmlLocationType,
) {
  return (
    xmlLocationTypeOptions.find(
      (option) =>
        option.value === value,
    )?.label ?? "Desconhecido"
  );
}