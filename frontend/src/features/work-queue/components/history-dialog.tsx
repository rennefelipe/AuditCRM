"use client";

import { Clock3 } from "lucide-react";

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

import { ProcessTimelinePanel } from "./process-timeline-panel";

type HistoryDialogProps = {
  open: boolean;

  onOpenChange: (
    open: boolean,
  ) => void;

  storeProcessId: string;

  storeName: string;
};

export function HistoryDialog({
  open,
  onOpenChange,
  storeProcessId,
  storeName,
}: HistoryDialogProps) {
  return (
    <Dialog
      open={open}
      onOpenChange={onOpenChange}
    >
      <DialogContent className="w-[calc(100vw-2rem)] max-w-3xl p-0 sm:w-full">
        <div className="border-b border-slate-200 px-5 py-4">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2 text-[#00213d]">
              <Clock3 className="h-5 w-5 text-[#087fdb]" />

              Histórico
            </DialogTitle>

            <DialogDescription>
              {storeName}
            </DialogDescription>
          </DialogHeader>
        </div>

        <div className="max-h-[70vh] overflow-y-auto px-5 py-4">
          <ProcessTimelinePanel
            storeProcessId={
              storeProcessId
            }
          />
        </div>
      </DialogContent>
    </Dialog>
  );
}