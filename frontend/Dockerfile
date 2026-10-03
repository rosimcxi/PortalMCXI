# Soubor: frontend/Dockerfile
# Tento soubor sestaví React aplikaci a spustí ji na lehkém Nginx serveru uvnitř kontejneru.

# Fáze 1: Build (Sestavení Node.js)
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# Fáze 2: Produkční běh (Nginx)
FROM nginx:alpine
# Zkopírujeme sestavený React do Nginxu
COPY --from=build /app/dist /usr/share/nginx/html
# Přidáme konfiguraci, aby React Router správně fungoval (catch-all)
RUN echo 'server { listen 80; location / { root /usr/share/nginx/html; index index.html; try_files $uri $uri/ /index.html; } }' > /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]