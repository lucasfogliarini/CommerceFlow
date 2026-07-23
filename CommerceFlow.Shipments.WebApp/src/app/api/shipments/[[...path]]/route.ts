import { NextRequest, NextResponse } from "next/server";

const API_URL = process.env.SHIPMENTS_API_URL ?? "http://localhost:2012";

type RouteContext = { params: Promise<{ path?: string[] }> };

async function forward(request: NextRequest, context: RouteContext) {
  try {
    const { path = [] } = await context.params;
    const headers: Record<string, string> = {};
    const authorization = request.headers.get("Authorization");
    const contentType = request.headers.get("Content-Type");

    if (authorization) headers.Authorization = authorization;
    if (contentType) headers["Content-Type"] = contentType;

    const response = await fetch(`${API_URL}/shipments/${path.join("/")}`, {
      method: request.method,
      headers,
      body: request.method === "GET" ? undefined : await request.text(),
      cache: "no-store",
    });

    return new NextResponse(await response.text(), {
      status: response.status,
      headers: { "Content-Type": response.headers.get("Content-Type") ?? "application/json" },
    });
  } catch {
    return NextResponse.json({ error: "Backend service unavailable" }, { status: 503 });
  }
}

export async function GET(request: NextRequest, context: RouteContext) {
  return forward(request, context);
}

export async function POST(request: NextRequest, context: RouteContext) {
  return forward(request, context);
}

export async function PUT(request: NextRequest, context: RouteContext) {
  return forward(request, context);
}
