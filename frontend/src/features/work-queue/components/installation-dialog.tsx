"use client";

import {
  CheckCircle2,
  Loader2,
  Save,
  Wrench,
  XCircle,
} from "lucide-react";
import {
  useEffect,
  useState,
} from "react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";

import {
  xmlLocationTypeOptions,
  type XmlLocationType,
} from "@/features/process-installations/process-installation-types";
import { useCreateProcessInstallation } from "@/features/process-installations/use-process-installations";

type InstallationDialogProps = {
  open: boolean;

  onOpenChange: (
    open: boolean,
  ) => void;

  storeProcessId: string;

  responsibleUserId?: string | null;

  erpId?: string | null;

  numberOfRegisters?: number | null;

  storeName: string;
};

function toLocalDateTimeInputValue(
  value: Date,
) {
  const offset =
    value.getTimezoneOffset();

  const localDate =
    new Date(
      value.getTime() -
        offset * 60_000,
    );

  return localDate
    .toISOString()
    .slice(0, 16);
}

export function InstallationDialog({
  open,
  onOpenChange,
  storeProcessId,
  responsibleUserId = null,
  erpId = null,
  numberOfRegisters = null,
  storeName,
}: InstallationDialogProps) {
  const mutation =
    useCreateProcessInstallation(
      storeProcessId,
    );

  const [
    installedAt,
    setInstalledAt,
  ] = useState("");

  const [
    successful,
    setSuccessful,
  ] = useState(true);

  const [
    xmlLocationType,
    setXmlLocationType,
  ] =
    useState<XmlLocationType>(0);

  const [
    registers,
    setRegisters,
  ] = useState("");

  const [
    result,
    setResult,
  ] = useState("");

  const [
    notes,
    setNotes,
  ] = useState("");

  const [
    error,
    setError,
  ] = useState<string | null>(
    null,
  );

  useEffect(() => {
    if (!open) {
      return;
    }

    setInstalledAt(
      toLocalDateTimeInputValue(
        new Date(),
      ),
    );

    setSuccessful(true);

    setXmlLocationType(0);

    setRegisters(
      numberOfRegisters != null
        ? String(numberOfRegisters)
        : "",
    );

    setResult("");
    setNotes("");
    setError(null);
  }, [
    open,
    numberOfRegisters,
  ]);

  async function handleSubmit() {
    if (!installedAt) {
      setError(
        "Informe a data e hora da instalação.",
      );

      return;
    }

    setError(null);

    try {
      await mutation.mutateAsync({
        storeProcessId,

        installedAt:
          new Date(
            installedAt,
          ).toISOString(),

        successful,

        responsibleUserId,

        erpId,

        xmlLocationType,

        numberOfRegisters:
          registers.trim()
            ? Number(registers)
            : null,

        result:
          result.trim() || null,

        notes:
          notes.trim() || null,
      });

      onOpenChange(false);
    } catch {
      setError(
        "Não foi possível registrar a instalação.",
      );
    }
  }

  return (
    <Dialog
      open={open}
      onOpenChange={onOpenChange}
    >
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2 text-[#00213d]">
            <Wrench className="h-5 w-5 text-[#087fdb]" />
            Registrar instalação
          </DialogTitle>

          <DialogDescription>
            {storeName}
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          <div className="grid gap-4 sm:grid-cols-2">
            <div className="space-y-1.5">
              <label className="text-xs font-semibold uppercase tracking-wide text-slate-500">
                Data e hora
              </label>

              <Input
                type="datetime-local"
                value={installedAt}
                onChange={(event) =>
                  setInstalledAt(
                    event.target.value,
                  )
                }
              />
            </div>

            <div className="space-y-1.5">
              <label className="text-xs font-semibold uppercase tracking-wide text-slate-500">
                Resultado da instalação
              </label>

              <div className="grid grid-cols-2 gap-2">
                <button
                  type="button"
                  onClick={() =>
                    setSuccessful(true)
                  }
                  className={[
                    "flex h-10 items-center justify-center gap-2 rounded-md border text-sm font-semibold transition",
                    successful
                      ? "border-emerald-300 bg-emerald-50 text-emerald-700"
                      : "border-slate-200 bg-white text-slate-500 hover:bg-slate-50",
                  ].join(" ")}
                >
                  <CheckCircle2 className="h-4 w-4" />
                  Sucesso
                </button>

                <button
                  type="button"
                  onClick={() =>
                    setSuccessful(false)
                  }
                  className={[
                    "flex h-10 items-center justify-center gap-2 rounded-md border text-sm font-semibold transition",
                    !successful
                      ? "border-red-300 bg-red-50 text-red-700"
                      : "border-slate-200 bg-white text-slate-500 hover:bg-slate-50",
                  ].join(" ")}
                >
                  <XCircle className="h-4 w-4" />
                  Falha
                </button>
              </div>
            </div>
          </div>

          <div className="grid gap-4 sm:grid-cols-2">
            <div className="space-y-1.5">
              <label className="text-xs font-semibold uppercase tracking-wide text-slate-500">
                Localização do XML
              </label>

              <select
                value={xmlLocationType}
                onChange={(event) =>
                  setXmlLocationType(
                    Number(
                      event.target.value,
                    ) as XmlLocationType,
                  )
                }
                className="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-sm text-[#00213d] outline-none focus:border-[#087fdb]"
              >
                {xmlLocationTypeOptions.map(
                  (option) => (
                    <option
                      key={option.value}
                      value={option.value}
                    >
                      {option.label}
                    </option>
                  ),
                )}
              </select>
            </div>

            <div className="space-y-1.5">
              <label className="text-xs font-semibold uppercase tracking-wide text-slate-500">
                Nº de caixas
              </label>

              <Input
                type="number"
                min="0"
                value={registers}
                onChange={(event) =>
                  setRegisters(
                    event.target.value,
                  )
                }
              />
            </div>
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-semibold uppercase tracking-wide text-slate-500">
              Resultado
            </label>

            <Input
              value={result}
              onChange={(event) =>
                setResult(
                  event.target.value,
                )
              }
              placeholder="Ex.: ControlSync instalado e comunicação validada"
            />
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-semibold uppercase tracking-wide text-slate-500">
              Observações
            </label>

            <Textarea
              value={notes}
              onChange={(event) =>
                setNotes(
                  event.target.value,
                )
              }
              rows={4}
              placeholder="Ex.: Instalação realizada via acesso remoto. XML localizado em nuvem."
            />
          </div>

          <div className="rounded-lg border border-slate-200 bg-slate-50 px-3 py-3 text-xs text-slate-500">
            O técnico, ERP e quantidade de caixas do cadastro atual da loja serão utilizados como referência.
          </div>

          {error && (
            <div className="rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm font-medium text-red-700">
              {error}
            </div>
          )}

          <div className="flex justify-end gap-2 border-t border-slate-200 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={() =>
                onOpenChange(false)
              }
              disabled={
                mutation.isPending
              }
            >
              Cancelar
            </Button>

            <Button
              type="button"
              onClick={handleSubmit}
              disabled={
                mutation.isPending
              }
              className="gap-2 bg-[#087fdb] text-white hover:bg-[#006ec2]"
            >
              {mutation.isPending ? (
                <>
                  <Loader2 className="h-4 w-4 animate-spin" />
                  Registrando...
                </>
              ) : (
                <>
                  <Save className="h-4 w-4" />
                  Registrar instalação
                </>
              )}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}