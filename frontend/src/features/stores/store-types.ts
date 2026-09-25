export type MonitoringStatus =
  | 0
  | 1
  | 2
  | 3
  | 4
  | 5;

export type Store = {
  id: string;

  shoppingId: string;
  shoppingName: string;

  erpId: string | null;
  erpName: string | null;

  luc: string | null;

  tradeName: string;

  corporateName: string | null;

  document: string | null;

  stateRegistration: string | null;

  numberOfRegisters: number | null;

  monitoringStatus: MonitoringStatus;

  businessType: string | null;

  notes: string | null;

  isActive: boolean;

  createdAt: string;
};