PGPASSWORD="password" psql -U postgres postgres -f '/initialization/basic.sql'
PGPASSWORD="password" pg_restore -d keycloak -U keycloak -j 8 -Fc /initialization/keycloak.dump