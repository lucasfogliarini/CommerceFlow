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

COPY --from=node_builder /app/package.json ./package.json
COPY --from=node_builder /app/node_modules ./node_modules
COPY --from=node_builder /app/.next ./.next
COPY --from=node_builder /app/public ./public
COPY --from=node_builder /app/next.config.ts ./next.config.ts

EXPOSE 3000

CMD ["npm", "run", "start"]
