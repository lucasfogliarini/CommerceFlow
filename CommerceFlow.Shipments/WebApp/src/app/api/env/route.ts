import { NextResponse } from "next/server";

export const dynamic = "force-dynamic";

export async function GET() {
  const env = Object.fromEntries(
    Object.entries(process.env).sort(([keyA], [keyB]) => keyA.localeCompare(keyB))
  );

  return NextResponse.json(env);
}
