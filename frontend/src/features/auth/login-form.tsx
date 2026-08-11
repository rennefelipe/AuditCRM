"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import axios from "axios";
import {
  Eye,
  EyeOff,
  Loader2,
  LockKeyhole,
  Mail,
} from "lucide-react";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Field,
  FieldError,
  FieldGroup,
  FieldLabel,
} from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { saveAuth } from "@/features/auth/auth-storage";
import { login } from "@/features/auth/auth-service";

const loginSchema = z.object({
  email: z
    .string()
    .min(1, "Informe seu e-mail.")
    .email("Informe um e-mail válido."),
  password: z
    .string()
    .min(1, "Informe sua senha."),
});

type LoginFormData = z.infer<typeof loginSchema>;

export function LoginForm() {
  const router = useRouter();

  const [showPassword, setShowPassword] = useState(false);
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: {
      errors,
      isSubmitting,
    },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  async function onSubmit(data: LoginFormData) {
    setServerError(null);

    try {
      const result = await login(data);

      saveAuth(result);

      router.replace("/dashboard");
    } catch (error) {
      if (axios.isAxiosError(error)) {
        if (!error.response) {
          setServerError(
            "Não foi possível conectar ao servidor.",
          );
          return;
        }

        if (error.response.status === 401) {
          setServerError(
            "E-mail ou senha inválidos.",
          );
          return;
        }

        const message =
          error.response?.data?.message ??
          "Não foi possível realizar o login.";

        setServerError(message);
        return;
      }

      setServerError(
        "Não foi possível acessar o Audit CRM. Tente novamente.",
      );
    }
  }

  return (
    <Card className="w-full max-w-lg overflow-hidden border border-white/10 bg-white/[0.98] shadow-2xl shadow-black/30 backdrop-blur-xl">
      <div className="h-1.5 w-full bg-gradient-to-r from-[#007dd8] via-cyan-400 to-[#00d4e8]" />

      <CardHeader className="space-y-6 px-7 pb-7 pt-8 sm:px-10">
        <div className="flex items-center justify-between">
          <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-[#007dd8] text-white shadow-lg shadow-blue-500/20">
            <LockKeyhole className="h-7 w-7" />
          </div>

          <div className="text-xl font-bold tracking-tight text-[#00213d]">
            Audit
            <span className="text-[#007dd8]"> CRM</span>
          </div>
        </div>

        <div className="space-y-2">
          <CardTitle className="text-3xl font-semibold tracking-tight text-[#00213d]">
            Acessar o sistema
          </CardTitle>

          <CardDescription className="text-base leading-6 text-slate-500">
            Entre com seu usuário e senha.
          </CardDescription>
        </div>
      </CardHeader>

      <CardContent className="px-7 pb-8 sm:px-10">
        <form
          onSubmit={handleSubmit(onSubmit)}
          className="space-y-6"
        >
          <FieldGroup>
            <Field>
              <FieldLabel
                htmlFor="email"
                className="text-base font-semibold text-[#00213d]"
              >
                E-mail
              </FieldLabel>

              <div className="relative">
                <Mail className="pointer-events-none absolute left-3.5 top-1/2 h-5 w-5 -translate-y-1/2 text-slate-400" />

                <Input
                  id="email"
                  type="email"
                  autoComplete="email"
                  placeholder="seuemail@auditbs.com.br"
                  className="h-12 border-slate-200 bg-white pl-11 text-base text-[#00213d] shadow-sm outline-none transition placeholder:text-slate-400 focus-visible:border-[#007dd8] focus-visible:ring-[#007dd8]/20"
                  disabled={isSubmitting}
                  {...register("email")}
                />
              </div>

              {errors.email?.message && (
                <FieldError>
                  {errors.email.message}
                </FieldError>
              )}
            </Field>

            <Field>
              <FieldLabel
                htmlFor="password"
                className="text-base font-semibold text-[#00213d]"
              >
                Senha
              </FieldLabel>

              <div className="relative">
                <LockKeyhole className="pointer-events-none absolute left-3.5 top-1/2 h-5 w-5 -translate-y-1/2 text-slate-400" />

                <Input
                  id="password"
                  type={showPassword ? "text" : "password"}
                  autoComplete="current-password"
                  placeholder="Digite sua senha"
                  className="h-12 border-slate-200 bg-white px-11 text-base text-[#00213d] shadow-sm outline-none transition placeholder:text-slate-400 focus-visible:border-[#007dd8] focus-visible:ring-[#007dd8]/20"
                  disabled={isSubmitting}
                  {...register("password")}
                />

                <button
                  type="button"
                  onClick={() =>
                    setShowPassword((current) => !current)
                  }
                  className="absolute right-3.5 top-1/2 -translate-y-1/2 text-slate-400 transition hover:text-[#007dd8]"
                  aria-label={
                    showPassword
                      ? "Ocultar senha"
                      : "Mostrar senha"
                  }
                >
                  {showPassword ? (
                    <EyeOff className="h-5 w-5" />
                  ) : (
                    <Eye className="h-5 w-5" />
                  )}
                </button>
              </div>

              {errors.password?.message && (
                <FieldError>
                  {errors.password.message}
                </FieldError>
              )}
            </Field>
          </FieldGroup>

          {serverError && (
            <div className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm font-medium text-red-600">
              {serverError}
            </div>
          )}

          <Button
            type="submit"
            size="lg"
            disabled={isSubmitting}
            className="h-12 w-full bg-[#007dd8] text-base font-semibold text-white shadow-lg shadow-blue-500/20 transition hover:bg-[#006fc0] focus-visible:ring-[#00d4e8]/30"
          >
            {isSubmitting ? (
              <>
                <Loader2 className="h-5 w-5 animate-spin" />
                Entrando...
              </>
            ) : (
              "Entrar"
            )}
          </Button>

          <div className="pt-1 text-center text-sm text-slate-500">
            Acesso restrito
          </div>
        </form>
      </CardContent>
    </Card>
  );
}