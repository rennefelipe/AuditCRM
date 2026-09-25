export type TimelineItem = {
  id: string;

  type: string;

  title: string;

  description: string | null;

  occurredAt: string;

  userId: string | null;

  userName: string | null;
};