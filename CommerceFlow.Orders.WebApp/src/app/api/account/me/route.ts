import { NextRequest, NextResponse } from "next/server";

const ORDERS_API_URL = process.env.ORDERS_API_URL;

export async function GET(request: NextRequest) {
  try {
    const authHeader = request.headers.get("Authorization");
    const headers: Record<string, string> = { "Content-Type": "application/json" };
    if (authHeader) {
        headers["Authorization"] = authHeader;
    }

    const res = await fetch(`${ORDERS_API_URL}/customers/me`, {
      method: "GET",
      headers,
    });

    if (!res.ok) {
      const errorText = await res.text();
      return NextResponse.json(
        { error: errorText || "Failed to fetch account" },
        { status: res.status }
      );
    }

    const data = await res.json();
    return NextResponse.json(data);
  } catch {
    return NextResponse.json(
      { error: "Backend service unavailable" },
      { status: 503 }
    );
  }
}
