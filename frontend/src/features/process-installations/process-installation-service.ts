import { api } from "@/lib/api";

import type {
  CreateProcessInstallationRequest,
  ProcessInstallation,
  UpdateProcessInstallationRequest,
} from "./process-installation-types";

export async function getProcessInstallations(
  storeProcessId?: string,
): Promise<ProcessInstallation[]> {
  const response =
    await api.get<ProcessInstallation[]>(
      "/process-installations",
      {
        params: {
          storeProcessId:
            storeProcessId || undefined,
        },
      },
    );

  return response.data;
}

export async function getProcessInstallationById(
  installationId: string,
): Promise<ProcessInstallation> {
  const response =
    await api.get<ProcessInstallation>(
      `/process-installations/${installationId}`,
    );

  return response.data;
}

export async function createProcessInstallation(
  request: CreateProcessInstallationRequest,
): Promise<ProcessInstallation> {
  const response =
    await api.post<ProcessInstallation>(
      "/process-installations",
      request,
    );

  return response.data;
}

export async function updateProcessInstallation(
  installationId: string,
  request: UpdateProcessInstallationRequest,
): Promise<ProcessInstallation> {
  const response =
    await api.put<ProcessInstallation>(
      `/process-installations/${installationId}`,
      request,
    );

  return response.data;
}

export async function deleteProcessInstallation(
  installationId: string,
): Promise<void> {
  await api.delete(
    `/process-installations/${installationId}`,
  );
}