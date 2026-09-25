import { api } from "@/lib/api";

import type { TimelineItem } from "./process-timeline-types";

export async function getProcessTimeline(
  storeProcessId: string,
): Promise<TimelineItem[]> {
  const response = await api.get<TimelineItem[]>(
    `/store-processes/${storeProcessId}/timeline`,
  );

  return response.data;
}