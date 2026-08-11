export type Shopping = {
  id: string;
  shoppingGroupId: string;
  shoppingGroupName: string;
  name: string;
  corporateName: string | null;
  document: string | null;
  city: string | null;
  state: string | null;
  isActive: boolean;
};