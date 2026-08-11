import { api } from "@/lib/api";

import type { DashboardSummary } from "./dashboard-types";

export type DashboardFilters = {
  shoppingGroupId?: string;
  shoppingId?: string;
  responsibleUserId?: string;
};

export async function getDashboardSummary(
  filters: DashboardFilters = {},
): Promise<DashboardSummary> {
  const response = await api.get<DashboardSummary>(
    "/dashboard/summary",
    {
      params: {
        shoppingGroupId:
          filters.shoppingGroupId || undefined,
        shoppingId:
          filters.shoppingId || undefined,
        responsibleUserId:
          filters.responsibleUserId || undefined,
      },
    },
  );

  return response.data;
}