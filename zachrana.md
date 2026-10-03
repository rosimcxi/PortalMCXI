🚑 Záchrana a odemčení PortaineruPokud se nemůžeš přihlásit do Portaineru na svém Contabo VPS, použij tento postup k jeho odemčení a oživení.1. Používáš správnou adresu? (HTTPS)Portainer verze 2.9+ naslouchá výhradně na šifrovaném portu. Otevři prohlížeč a ujisti se, že píšeš https://:👉 https://www.google.com/search?q=https://62.84.181.247:9443(Prohlížeč ti ukáže varování, že certifikát není bezpečný - to je normální, protože si ho server vygeneroval sám. Klikni na "Rozšířené" a "Pokračovat na web".)2. Odstranění 5minutového zámku (Nejčastější problém)Pokud jsi Portainer nainstaloval a neotevřel ho hned do 5 minut, sám se zablokuje, aby ti ho někdo cizí neukradl.Otevři konzoli na svém VPS (přes SSH) a restartuj ho tímto příkazem:docker restart portainer
Hned poté si v prohlížeči otevři adresu https://62.84.181.247:9443 a vytvoř si heslo.3. Co když jsem zapomněl heslo? (Hard Reset)Pokud heslo máš, ale zapomněl jsi ho (nebo se pořád nedaří), můžeš použít oficiální nástroj pro reset hesla.DŮLEŽITÉ: Portainer MUSÍ být před spuštěním resetu zastaven, jinak dojde k chybě (timeout), protože databáze je uzamčena běžícím procesem.Vlož do konzole na VPS tyto tři příkazy v tomto pořadí:# 1. Zastaví Portainer (Nutné pro odemčení databáze!)
docker stop portainer

# 2. Spustí pomocný nástroj, který vygeneruje nové heslo
docker run --rm -v portainer_data:/data portainer/helper-reset-password

# 3. Znovu nastartuje Portainer
docker start portainer
Konzole ti po spuštění druhého příkazu (resetu) vypíše tvé nové přihlašovací jméno (admin) a nové dočasné heslo.⚠️ Upozornění pro nastavení nového hesla:
Až se přihlásíš do webového rozhraní a budeš vyzván ke změně hesla, nezapomeň, že heslo musí být delší (minimálně 12 znaků) a pro úspěšné uložení do něj přidej znak #.Až Portainer znovu nastartuješ, přihlas se s těmito údaji na adrese https://62.84.181.247:9443.