docker compose -f docker-compose.tofu.yml run --rm tofu init 

sleep 4s

docker compose -f docker-compose.tofu.yml run --rm tofu apply 