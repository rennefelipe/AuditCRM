export type Shopping = {
  id: string;

  shoppingGroupId: string;
  shoppingGroupName: string;

  name: string;

  corporateName: string | null;
  document: string | null;

  address: string | null;
  number: string | null;
  district: string | null;
  city: string | null;
  state: string | null;
  zipCode: string | null;

  contactName: string | null;
  contactEmail: string | null;
  contactPhone: string | null;

  websiteUrl: string | null;
  controlShopUrl: string | null;
  portalUrl: string | null;
  apiName: string | null;
  xmlReadingEmail: string | null;
  registrationEmail: string | null;

  paysInstallation: boolean;

  notes: string | null;

  isActive: boolean;

  createdAt: string;
};