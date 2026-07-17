import { NextResponse } from "next/server";

const COMMERCEFLOW_API_URL = process.env.COMMERCEFLOW_API_URL;

export async function GET(request: Request) {
  try {
      const query = new URL(request.url).search;
      const res = await fetch(`${COMMERCEFLOW_API_URL}/odata/products${query}`, {
      cache: "no-store",
    });

    if (!res.ok) {
      return NextResponse.json(
        { error: "Failed to fetch products" },
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
