from os import getenv, listdir, path
from minio import Minio

BUCKET_NAME = 'test'
MODEL_FIXTURE_DIR = './fixtures/models'

try:
    mc = Minio(
        getenv('MINIO_HOSTNAME'),
        access_key=getenv('MINIO_ACCESS_KEY'),
        secret_key=getenv('MINIO_SECRET_KEY'),
        secure=False
    )
    
    mc.list_buckets()
    print('MinIO connection successful')

    if BUCKET_NAME in [bucket.name for bucket in mc.list_buckets()]:
        print(f'Bucket "{BUCKET_NAME}" already exists - skipping...')

    for filename in listdir(MODEL_FIXTURE_DIR):
        local_filename = path.join(MODEL_FIXTURE_DIR, filename)

        if path.isfile(local_filename):
            mc.fput_object(
                BUCKET_NAME,
                filename,
                local_filename
            )
            print(f'Uploaded "{local_filename}" to "{BUCKET_NAME}/{filename}"')

except Exception as e:
    print(f'MinIO connection failed: {e}')