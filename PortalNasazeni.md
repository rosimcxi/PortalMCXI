🐳 Nasazení PortalMCXI přes Portainer (Docker)Přechod na Docker Stacks (Portainer) znamená, že se už nemusíme trápit s chybami typu 203/EXEC ve službách systemd.1. Příprava ve Visual Studiu / GituDo svého repozitáře na Azure DevOps přidej 3 nové soubory:frontend/Dockerfilebackend/Dockerfiledocker-compose.yml (do hlavního kořene)Poznámka: Tvůj starý azure-pipelines.yml můžeš smazat nebo vypnout, protože Portainer si teď bude projekt sestavovat sám!2. Nastavení v Portaineru (na tvém VPS)Přihlas se do svého Portaineru (např. přes https://tvá-ip:9443).V levém menu klikni na Stacks a vpravo nahoře dej + Add stack.Pojmenuj ho třeba portal-mcxi.Místo psaní kódu ručně vyber možnost Repository.Do políčka Repository URL vlož odkaz na svůj Git z Azure DevOps (např. přes HTTPS, nebo s využitím tvého Git tokenu).Ujistěte se, že Compose path je nastaveno na docker-compose.yml.Úplně dole klikni na Deploy the stack.Portainer si teď sám stáhne kódy, zkompiluje .NET, zkompiluje React, zabalí je do kontejnerů a nastartuje.3. Nastavení Nginxu (na Ubuntu)Aby subdomény (rosimcxi.eu, dashboard.rosimcxi.eu) mířily do tvého nového Dockeru, stačí upravit tvůj stávající /etc/nginx/sites-available/default na tvém Contabo serveru:server {
    listen 80;
    server_name rosimcxi.eu *.rosimcxi.eu;

    # Nginx vezme požadavek a pošle ho do React kontejneru na port 8080
    location / {
        proxy_pass http://localhost:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }

    # API požadavky pošle do .NET kontejneru na port 5000
    location /api/ {
        proxy_pass http://localhost:5000/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
    }
}
Nezapomeň po této změně zavolat sudo systemctl restart nginx.