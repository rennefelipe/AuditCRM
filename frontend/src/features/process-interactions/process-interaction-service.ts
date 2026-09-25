import { api } from "@/lib/api";

import type {
  CreateProcessInteractionRequest,
  ProcessInteraction,
  UpdateProcessInteractionRequest,
} from "./process-interaction-types";

export async function getProcessInteractions(
  storeProcessId?: string,
): Promise<ProcessInteraction[]> {
  const response =
    await api.get<ProcessInteraction[]>(
      "/process-interactions",
      {
        params: {
          storeProcessId:
            storeProcessId || undefined,
        },
      },
    );

  return response.data;
}

export async function getProcessInteractionById(
  interactionId: string,
): Promise<ProcessInteraction> {
  const response =
    await api.get<ProcessInteraction>(
      `/process-interactions/${interactionId}`,
    );

  return response.data;
}

export async function createProcessInteraction(
  request: CreateProcessInteractionRequest,
): Promise<ProcessInteraction> {
  const response =
    await api.post<ProcessInteraction>(
      "/process-interactions",
      request,
    );

  return response.data;
}

export async function updateProcessInteraction(
  interactionId: string,
  request: UpdateProcessInteractionRequest,
): Promise<ProcessInteraction> {
  const response =
    await api.put<ProcessInteraction>(
      `/process-interactions/${interactionId}`,
      request,
    );

  return response.data;
}

export async function deleteProcessInteraction(
  interactionId: string,
): Promise<void> {
  await api.delete(
    `/process-interactions/${interactionId}`,
  );
}