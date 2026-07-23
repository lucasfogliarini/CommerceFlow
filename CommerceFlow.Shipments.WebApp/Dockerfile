# =========================
# Builder
# =========================
FROM node:22-alpine AS node_builder

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build

# =========================
# Runner
# =========================
FROM node:22-alpine AS node_runner

WORKDIR /app

ENV NODE_ENV=production
ENV PORT=3000

RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs

COPY --from=node_builder /app/public ./public

COPY --from=node_builder --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=node_builder --chown=nextjs:nodejs /app/.next/static ./.next/static

USER nextjs

EXPOSE 3000

CMD ["node", "server.js"]
