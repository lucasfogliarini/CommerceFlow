import { NextRequest, NextResponse } from "next/server";

const API_URL = process.env.COMMERCEFLOW_API_URL;

export async function PUT(request: NextRequest, { params }: { params: Promise<{ id: string }> }) {
  return forward(request, (await params).id, await request.text());
}

export async function DELETE(request: NextRequest, { params }: { params: Promise<{ id: string }> }) {
  return forward(request, (await params).id);
}

async function forward(request: NextRequest, id: string, body?: string) {
  const headers: Record<string, string> = { "Content-Type": "application/json" };
  const authorization = request.headers.get("Authorization");
  if (authorization) headers.Authorization = authorization;

  try {
    const response = await fetch(`${API_URL}/customers/me/addresses/${id}`, { method: request.method, headers, body });
    if (response.status === 204) {
      return new NextResponse(null, { status: response.status });
    }

    return new NextResponse(await response.text(), { status: response.status });
  } catch {
    return NextResponse.json({ error: "Backend service unavailable" }, { status: 503 });
  }
}
