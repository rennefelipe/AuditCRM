export type WorkQueueItem = {
  storeProcessId: string;
  storeId: string;
  storeName: string;

  shoppingId: string;
  shoppingName: string;

  shoppingGroupId: string | null;
  shoppingGroupName: string | null;

  status: number;
  priority: number;

  responsibleUserId: string | null;
  responsibleUserName: string | null;

  nextAction: string | null;
  nextActionAt: string | null;

  startedAt: string;
  isClosed: boolean;
};