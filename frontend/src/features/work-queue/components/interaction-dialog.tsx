"use client";

import {
  CalendarClock,
  Loader2,
  MessageSquareText,
  Save,
} from "lucide-react";
import { useEffect, useState } from "react";

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
  interactionChannelOptions,
  type InteractionChannel,
} from "@/features/process-interactions/process-interaction-types";
import { useCreateProcessInteraction } from "@/features/process-interactions/use-process-interactions";

type InteractionDialogProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;

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

export function InteractionDialog({
  open,
  onOpenChange,
  storeProcessId,
  responsibleUserId = null,
  storeContactId = null,
  storeName,
}: InteractionDialogProps) {
  const mutation =
    useCreateProcessInteraction(
      storeProcessId,
    );

  const [channel, setChannel] =
    useState<InteractionChannel>(2);

  const [occurredAt, setOccurredAt] =
    useState("");

  const [description, setDescription] =
    useState("");

  const [result, setResult] =
    useState("");

  const [nextAction, setNextAction] =
    useState("");

  const [nextActionAt, setNextActionAt] =
    useState("");

  const [error, setError] =
    useState<string | null>(null);

  useEffect(() => {
    if (!open) {
      return;
    }

    setChannel(2);

    setOccurredAt(
      toLocalDateTimeInputValue(
        new Date(),
      ),
    );

    setDescription("");
    setResult("");
    setNextAction("");
    setNextActionAt("");
    setError(null);
  }, [open]);

  async function handleSubmit() {
    if (!description.trim()) {
      setError(
        "Informe a descrição da tratativa.",
      );

      return;
    }

    if (!occurredAt) {
      setError(
        "Informe a data e hora da tratativa.",
      );

      return;
    }

    setError(null);

    try {
      await mutation.mutateAsync({
        storeProcessId,

        responsibleUserId,
        storeContactId,

        channel,

        occurredAt:
          new Date(
            occurredAt,
          ).toISOString(),

        description:
          description.trim(),

        result:
          result.trim() || null,

        nextAction:
          nextAction.trim() || null,

        nextActionAt:
          nextActionAt
            ? new Date(
                nextActionAt,
              ).toISOString()
            : null,
      });

      onOpenChange(false);
    } catch {
      setError(
        "Não foi possível registrar a tratativa.",
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
            <MessageSquareText className="h-5 w-5 text-[#087fdb]" />
            Registrar tratativa
          </DialogTitle>

          <DialogDescription>
            {storeName}
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          <div className="grid gap-4 sm:grid-cols-2">
            <div className="space-y-1.5">
              <label className="text-xs font-semibold uppercase tracking-wide text-slate-500">
                Canal
              </label>

              <select
                value={channel}
                onChange={(event) =>
                  setChannel(
                    Number(
                      event.target.value,
                    ) as InteractionChannel,
                  )
                }
                className="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-sm text-[#00213d] outline-none focus:border-[#087fdb]"
              >
                {interactionChannelOptions.map(
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
              <label className="flex items-center gap-1.5 text-xs font-semibold uppercase tracking-wide text-slate-500">
                <CalendarClock className="h-3.5 w-3.5" />
                Data e hora
              </label>

              <Input
                type="datetime-local"
                value={occurredAt}
                onChange={(event) =>
                  setOccurredAt(
                    event.target.value,
                  )
                }
              />
            </div>
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-semibold uppercase tracking-wide text-slate-500">
              Descrição
            </label>

            <Textarea
              value={description}
              onChange={(event) =>
                setDescription(
                  event.target.value,
                )
              }
              rows={4}
              placeholder="Ex.: Contato realizado por WhatsApp. Loja informou que verificará disponibilidade..."
            />
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
              placeholder="Ex.: Aguardando retorno do gerente"
            />
          </div>

          <div className="grid gap-4 sm:grid-cols-[1fr_220px]">
            <div className="space-y-1.5">
              <label className="text-xs font-semibold uppercase tracking-wide text-slate-500">
                Próxima ação
              </label>

              <Input
                value={nextAction}
                onChange={(event) =>
                  setNextAction(
                    event.target.value,
                  )
                }
                placeholder="Ex.: Retornar contato"
              />
            </div>

            <div className="space-y-1.5">
              <label className="text-xs font-semibold uppercase tracking-wide text-slate-500">
                Prazo
              </label>

              <Input
                type="datetime-local"
                value={nextActionAt}
                onChange={(event) =>
                  setNextActionAt(
                    event.target.value,
                  )
                }
              />
            </div>
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
              disabled={mutation.isPending}
            >
              Cancelar
            </Button>

            <Button
              type="button"
              onClick={handleSubmit}
              disabled={mutation.isPending}
              className="gap-2 bg-[#087fdb] text-white hover:bg-[#006ec2]"
            >
              {mutation.isPending ? (
                <>
                  <Loader2 className="h-4 w-4 animate-spin" />
                  Salvando...
                </>
              ) : (
                <>
                  <Save className="h-4 w-4" />
                  Registrar tratativa
                </>
              )}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}