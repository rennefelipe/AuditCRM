import type { LoginResponse } from "./types";

const AUTH_KEY = "audit-crm-auth";

export function saveAuth(data: LoginResponse) {
  localStorage.setItem(AUTH_KEY, JSON.stringify(data));
}

export function getAuth(): LoginResponse | null {
  if (typeof window === "undefined") {
    return null;
  }

  const value = localStorage.getItem(AUTH_KEY);

  if (!value) {
    return null;
  }

  try {
    return JSON.parse(value) as LoginResponse;
  } catch {
    return null;
  }
}

export function clearAuth() {
  localStorage.removeItem(AUTH_KEY);
}