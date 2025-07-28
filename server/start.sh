#!/usr/bin/env bash

python manage.py migrate -v 3 --noinput 
gunicorn server.wsgi:application --bind 0.0.0.0:8001