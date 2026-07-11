import { NextRequest, NextResponse } from "next/server";

const COMMERCEFLOW_API_URL = process.env.COMMERCEFLOW_API_URL;

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();

      const authHeader = request.headers.get("Authorization");
      const headers: Record<string, string> = { "Content-Type": "application/json" };
      if (authHeader) {
          headers["Authorization"] = authHeader;
      }

      const res = await fetch(`${COMMERCEFLOW_API_URL}/orders`, {
      method: "POST",
      headers,
      body: JSON.stringify(body),
    });

    if (!res.ok) {
      const errorText = await res.text();
      return NextResponse.json(
        { error: errorText || "Failed to create order" },
        { status: res.status }
      );
    }

    let data = null;
    const text = await res.text();
    if (text) {
      try {
        data = JSON.parse(text);
      } catch {
        data = { message: text };
      }
    }

    return NextResponse.json(data ?? { success: true });
  } catch {
    return NextResponse.json(
      { error: "Backend service unavailable" },
      { status: 503 }
    );
  }
}
