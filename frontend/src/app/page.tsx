import { LoginForm } from "@/features/auth/login-form";

export default function HomePage() {
  return (
    <main className="relative flex min-h-screen items-center justify-center overflow-hidden bg-[#00182d] p-5 sm:p-8">
      <div className="pointer-events-none absolute inset-0">
        <div
          className="absolute inset-0 opacity-[0.06]"
          style={{
            backgroundImage: `
              linear-gradient(rgba(34,211,238,.7) 1px, transparent 1px),
              linear-gradient(90deg, rgba(34,211,238,.7) 1px, transparent 1px)
            `,
            backgroundSize: "42px 42px",
          }}
        />

        <div className="absolute -left-40 -top-40 h-[420px] w-[420px] rounded-full bg-cyan-400/10 blur-[120px]" />

        <div className="absolute -bottom-48 -right-40 h-[520px] w-[520px] rounded-full bg-blue-500/15 blur-[140px]" />
      </div>

      <div className="relative w-full max-w-lg">
        <div className="mb-8 text-center">
          <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-2xl border border-cyan-300/30 bg-cyan-400/10 shadow-lg shadow-cyan-950/20">
            <span className="text-3xl font-bold text-cyan-300">
              A
            </span>
          </div>

          <h1 className="mt-5 text-4xl font-bold tracking-tight text-white">
            Audit
            <span className="text-cyan-300"> CRM</span>
          </h1>
        </div>

        <LoginForm />

        <p className="mt-5 text-center text-sm text-slate-500">
          Sistema interno
        </p>
      </div>
    </main>
  );
}