"use client";

import {
  Building2,
  LayoutDashboard,
  LogOut,
  Menu,
  Network,
  Store,
  Users,
  Workflow,
} from "lucide-react";
import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";

import { Button } from "@/components/ui/button";
import {
  Sheet,
  SheetContent,
  SheetTrigger,
} from "@/components/ui/sheet";
import {
  clearAuth,
  getAuth,
} from "@/features/auth/auth-storage";

type AppShellProps = {
  children: React.ReactNode;
};

const navigation = [
  {
    label: "Dashboard",
    href: "/dashboard",
    icon: LayoutDashboard,
  },
  {
    label: "Fila de trabalho",
    href: "/work-queue",
    icon: Workflow,
  },
  {
    label: "Redes",
    href: "/shopping-groups",
    icon: Network,
  },
  {
    label: "Shoppings",
    href: "/shoppings",
    icon: Building2,
  },
  {
    label: "Lojas",
    href: "/stores",
    icon: Store,
  },
  {
    label: "Usuários",
    href: "/users",
    icon: Users,
  },
];

function Navigation() {
  const pathname = usePathname();

  return (
    <nav className="space-y-1.5">
      {navigation.map((item) => {
        const Icon = item.icon;

        const active =
          pathname === item.href ||
          pathname.startsWith(`${item.href}/`);

        return (
          <Link
            key={item.href}
            href={item.href}
            className={[
              "flex h-11 items-center gap-3 rounded-xl px-3.5 text-sm font-medium transition",
              active
                ? "bg-[#007dd8] text-white shadow-md shadow-blue-950/20"
                : "text-slate-300 hover:bg-white/7 hover:text-white",
            ].join(" ")}
          >
            <Icon className="h-5 w-5" />
            {item.label}
          </Link>
        );
      })}
    </nav>
  );
}

function SidebarContent() {
  const router = useRouter();
  const auth = getAuth();

  function logout() {
    clearAuth();
    router.replace("/");
  }

  return (
    <div className="flex h-full flex-col">
      <div className="border-b border-white/10 px-5 py-6">
        <div className="flex items-center gap-3">
          <div className="flex h-11 w-11 items-center justify-center rounded-xl border border-cyan-300/30 bg-cyan-400/10">
            <span className="text-xl font-bold text-cyan-300">
              A
            </span>
          </div>

          <div>
            <div className="text-xl font-bold text-white">
              Audit
              <span className="text-cyan-300"> CRM</span>
            </div>

            <div className="text-xs text-slate-500">
              Sistema interno
            </div>
          </div>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-3 py-5">
        <Navigation />
      </div>

      <div className="border-t border-white/10 p-4">
        <div className="mb-4 rounded-xl bg-white/5 px-3.5 py-3">
          <div className="truncate text-sm font-semibold text-white">
            {auth?.name ?? "Usuário"}
          </div>

          <div className="mt-0.5 truncate text-xs text-slate-400">
            {auth?.email ?? ""}
          </div>
        </div>

        <Button
          variant="ghost"
          onClick={logout}
          className="w-full justify-start gap-3 text-slate-300 hover:bg-red-500/10 hover:text-red-300"
        >
          <LogOut className="h-4 w-4" />
          Sair
        </Button>
      </div>
    </div>
  );
}

export function AppShell({
  children,
}: AppShellProps) {
  return (
    <div className="min-h-screen bg-[#f4f7fb]">
      <aside className="fixed inset-y-0 left-0 z-40 hidden w-64 bg-[#00182d] lg:block">
        <SidebarContent />
      </aside>

      <div className="lg:pl-64">
        <header className="sticky top-0 z-30 flex h-16 items-center border-b border-slate-200 bg-white/95 px-4 backdrop-blur sm:px-6 lg:px-8">
          <div className="lg:hidden">
            <Sheet>
              <SheetTrigger
                render={
                  <Button
                    variant="ghost"
                    size="icon"
                    aria-label="Abrir menu"
                  />
                }
              >
                <Menu className="h-5 w-5" />
              </SheetTrigger>

              <SheetContent
                side="left"
                className="w-72 border-none bg-[#00182d] p-0 text-white"
              >
                <SidebarContent />
              </SheetContent>
            </Sheet>
          </div>

          <div className="ml-3 text-sm font-semibold text-[#00213d] lg:ml-0">
            Audit CRM
          </div>
        </header>

        <main className="p-4 sm:p-6 lg:p-8">
          {children}
        </main>
      </div>
    </div>
  );
}