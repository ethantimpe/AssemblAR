#!/usr/bin/env bash

echo "Applying migrations..."
python manage.py migrate -v 3 --noinput 

echo "Loading fixtures..."
python manage.py loaddata ./fixtures/data.json

echo "Setting up MinIO buckets and fixtures..."
python initialize-minio.py

echo "Starting server..."
gunicorn server.wsgi:application --bind 0.0.0.0:8001