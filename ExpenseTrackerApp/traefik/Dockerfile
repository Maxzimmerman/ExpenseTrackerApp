FROM docker.io/traefik:3.3.3
RUN mkdir -p /etc/traefik/acme \
  && touch /etc/traefik/acme/acme.json \
  && chmod 600 /etc/traefik/acme/acme.json
COPY ./ExpenseTrackerApp/traefik/traefik.yml /etc/traefik/
