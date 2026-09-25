"use client";

import {
  CalendarClock,
  Loader2,
  Save,
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

import { useCreateProcessAppointment } from "@/features/process-appointments/use-process-appointments";

type AppointmentDialogProps = {
  open: boolean;

  onOpenChange: (
    open: boolean,
  ) => void;

  storeProcessId: string;

  responsibleUserId?: string | null;

  storeContactId?: string | null;

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

export function AppointmentDialog({
  open,
  onOpenChange,
  storeProcessId,
  responsibleUserId = null,
  storeContactId = null,
  storeName,
}: AppointmentDialogProps) {
  const mutation =
    useCreateProcessAppointment(
      storeProcessId,
    );

  const [
    scheduledAt,
    setScheduledAt,
  ] = useState("");

  const [notes, setNotes] =
    useState("");

  const [error, setError] =
    useState<string | null>(
      null,
    );

  useEffect(() => {
    if (!open) {
      return;
    }

    setScheduledAt(
      toLocalDateTimeInputValue(
        new Date(),
      ),
    );

    setNotes("");

    setError(null);
  }, [open]);

  async function handleSubmit() {
    if (!scheduledAt) {
      setError(
        "Informe a data e hora do agendamento.",
      );

      return;
    }

    setError(null);

    try {
      await mutation.mutateAsync({
        storeProcessId,

        scheduledAt:
          new Date(
            scheduledAt,
          ).toISOString(),

        responsibleUserId,

        storeContactId,

        notes:
          notes.trim() ||
          null,
      });

      onOpenChange(false);
    } catch {
      setError(
        "Não foi possível criar o agendamento.",
      );
    }
  }

  return (
    <Dialog
      open={open}
      onOpenChange={onOpenChange}
    >
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2 text-[#00213d]">
            <CalendarClock className="h-5 w-5 text-[#087fdb]" />

            Agendar implantação
          </DialogTitle>

          <DialogDescription>
            {storeName}
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          <div className="space-y-1.5">
            <label className="flex items-center gap-1.5 text-xs font-semibold uppercase tracking-wide text-slate-500">
              <CalendarClock className="h-3.5 w-3.5" />
              Data e hora
            </label>

            <Input
              type="datetime-local"
              value={scheduledAt}
              onChange={(event) =>
                setScheduledAt(
                  event.target.value,
                )
              }
            />
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-semibold uppercase tracking-wide text-slate-500">
              Observação
            </label>

            <Textarea
              value={notes}
              onChange={(event) =>
                setNotes(
                  event.target.value,
                )
              }
              rows={4}
              placeholder="Ex.: Acesso remoto confirmado com a loja."
            />
          </div>

          <div className="rounded-lg border border-slate-200 bg-slate-50 px-3 py-3 text-xs text-slate-500">
            O técnico responsável do
            processo será utilizado neste
            agendamento.
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
                  Agendando...
                </>
              ) : (
                <>
                  <Save className="h-4 w-4" />
                  Criar agendamento
                </>
              )}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}