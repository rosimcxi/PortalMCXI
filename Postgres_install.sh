### 1. Kam soubor umístit (Adresářová struktura)

Soubor `docker-compose.yml` patří do **kořenového (root) adresáře** tvého projektu. To je ta nejvyšší složka, kde máš i `.git` a složky `backend` a `frontend`.

**Příklad cesty:**

* **Asus:** `C:\Vyvoj\PortalMCXI\docker-compose.yml`
* **VPS:** `/var/www/portalmcxi/docker-compose.yml`

Tímto způsobem může Docker Compose spravovat celou infrastrukturu projektu (databázi, administraci, případně Redis nebo RabbitMQ) z jednoho místa.

---FILENAME: docker-compose.yml---
version: '3.8'

services:
db:
image: postgres:17-alpine
container_name: portal_mcxi_db
restart: always
environment:
POSTGRES_USER: ${DB_USER}
POSTGRES_PASSWORD: ${DB_PASSWORD}
POSTGRES_DB: ${DB_NAME}
ports:
- "5432:5432"
volumes:
- portal_mcxi_data:/var/lib/postgresql/data
- ./database/init.sql:/docker-entrypoint-initdb.d/init.sql
networks:
- portal_network

pgadmin:
image: dpage/pgadmin4
container_name: portal_mcxi_pgadmin
restart: always
environment:
PGADMIN_DEFAULT_EMAIL: ${PGADMIN_EMAIL}
PGADMIN_DEFAULT_PASSWORD: ${PGADMIN_PASSWORD}
PGADMIN_CONFIG_SERVER_MODE: 'False'
ports:
- "5050:80"
depends_on:
- db
networks:
- portal_network

networks:
portal_network:
driver: bridge

## volumes:
portal_mcxi_data:

### Jak to opravit na tvém Asusu (krok za krokem):

1. **Zapni Docker Desktop**: Klikni ve Windows na ikonu Docker Desktop a počkej, až "velryba" v liště přestane blikat a bude zelená. Bez toho příkaz `docker` neuvidí engine.
2. **Zkontroluj .env**: Ujisti se, že ve stejné složce máš soubor `.env` s definovanými proměnnými (DB_USER, DB_PASSWORD atd.), jinak si Docker bude stěžovat na prázdné hodnoty.
3. **Oprav soubor**: Smaž starý `docker-compose.yml` a vlož tam ten výše. Klíčové je, že `pgadmin:` nesmí být odsazený vpravo pod `db:`.
4. **Spusť to znovu**:
V Git Bashi napiš:
`docker-compose down` (pro jistotu vyčistit)
`docker-compose up -d`

> **Tip od seniora:** Pokud používáš VS Code, nainstaluj si rozšíření "YAML". To ti červeně podtrhne, když uděláš chybu v odsazení, ještě než to zkusíš spustit.

Až se ti podaří kontejnery nahodit (uvidíš zelené "Started"), zkus se přihlásit do pgAdminu na `http://localhost:5050`.

Povedlo se ti Docker nastartovat, nebo ta velryba pořád stávkuje?
