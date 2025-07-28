FROM python:3.11.13-bookworm

USER root

COPY ./server /assemblar-server

WORKDIR /assemblar-server

RUN cd /assemblar-server && \
    pip install -r requirements.txt && \
    python manage.py makemigrations

ENTRYPOINT ["./start.sh"]