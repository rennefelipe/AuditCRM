"use client";

import { useQuery } from "@tanstack/react-query";

import { getProcessTimeline } from "./process-timeline-service";

export function useProcessTimeline(
  storeProcessId?: string,
) {
  return useQuery({
    queryKey: [
      "store-process-timeline",
      storeProcessId ?? null,
    ],

    queryFn: () =>
      getProcessTimeline(
        storeProcessId as string,
      ),

    enabled: Boolean(storeProcessId),
  });
}