import { NextRequest, NextResponse } from "next/server";

const API_URL = process.env.ORDERS_API_URL;

export async function GET(request: NextRequest) {
  return forward(request);
}

export async function POST(request: NextRequest) {
  return forward(request, await request.text());
}

async function forward(request: NextRequest, body?: string) {
  const headers: Record<string, string> = { "Content-Type": "application/json" };
  const authorization = request.headers.get("Authorization");
  if (authorization) headers.Authorization = authorization;

  try {
    const response = await fetch(`${API_URL}/customers/me/addresses`, { method: request.method, headers, body });
    return new NextResponse(await response.text(), { status: response.status, headers: { "Content-Type": "application/json" } });
  } catch {
    return NextResponse.json({ error: "Backend service unavailable" }, { status: 503 });
  }
}
