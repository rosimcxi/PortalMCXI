# Soubor: frontend/Dockerfile
# Tento soubor sestaví React aplikaci a spustí ji na lehkém Nginx serveru uvnitř kontejneru.

# Fáze 1: Build (Sestavení Node.js)
FROM node:24-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# Fáze 2: Produkční běh (Nginx)
FROM nginx:alpine
# Zkopírujeme sestavený React do Nginxu
COPY --from=build /app/dist /usr/share/nginx/html
# Přidáme konfiguraci, aby React Router správně fungoval (catch-all)
ENV API_UPSTREAM=http://api:5000
COPY nginx.conf.template /etc/nginx/templates/default.conf.template

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
