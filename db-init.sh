#!/bin/bash

if [ -f /tmp/.db-init-done ]; then
  echo "Database already initialized. Recreate this container to force init to run."
  exit 0;
fi

dotnet restore && \
cd PlatformTM.Data && \
rm Migrations/* && \
dotnet ef --startup-project ../PlatformTM.API/ migrations add Initial -c BioSPEAKdbContext && \
dotnet ef --startup-project ../PlatformTM.API/ database update -c BioSPEAKdbContext --verbose && \

mysql=( mysql -uroot -pimperial -hmariadb -DeTRIKSdata )

for f in /docker-entrypoint-initdb.d/*; do
	case "$f" in
		*.sh)     echo "$0: running $f"; . "$f" ;;
		*.sql)    echo "$0: running $f"; "${mysql[@]}" < "$f"; echo ;;
		*.sql.gz) echo "$0: running $f"; gunzip -c "$f" | "${mysql[@]}"; echo ;;
		*)        echo "$0: ignoring $f" ;;
	esac
	echo
done

touch /tmp/.db-init-done
