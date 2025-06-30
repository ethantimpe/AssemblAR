import os

from django.http import JsonResponse, HttpResponse, HttpRequest, HttpResponseNotAllowed, HttpResponseNotFound
from rest_framework.viewsets import ModelViewSet

from minio import Minio

from .models import *
from .serializers import *

##############################
#    REST Framework Views    #
##############################

class PartViewSet(ModelViewSet):
    queryset = Part.objects.all()
    serializer_class = PartSerializer

class OrganizationViewSet(ModelViewSet):
    queryset = Organization.objects.all()
    serializer_class = OrganizationSerializer

class InstructionSetViewSet(ModelViewSet):
    queryset = InstructionSet.objects.all()
    serializer_class = InstructionSetSerializer

class InstructionStepViewSet(ModelViewSet):
    queryset = InstructionStep.objects.all()
    serializer_class = InstructionStepSerializer

class UserMetricViewSet(ModelViewSet):
    queryset = UserMetric.objects.all()
    serializer_class = UserMetricSerializer

class InstructionStepMetricViewSet(ModelViewSet):
    queryset = InstructionStepMetric.objects.all()
    serializer_class = InstructionStepMetricSerializer

##############################
#    File Retrieval View     #
##############################

class FileView():
    def __init__(self):
        #Initialize MinIO connection
        try:
            self.mc = Minio(
                    os.getenv('MINIO_HOSTNAME'),
                    access_key=os.getenv('MINIO_ACCESS_KEY'),
                    secret_key=os.getenv('MINIO_SECRET_KEY'),
                  )
            
            self.mc.list_buckets()
            print('MinIO connection successful')
        except Exception as e:
            print(f'MinIO connection failed: {e}')


    def file_view(self, request: HttpRequest, file_id: int):
        if request.method != 'GET':
            return HttpResponseNotAllowed(['GET'])
        
        part = Part.objects.get(id=file_id)

        # Get data of specified object
        try:
            response = self.mc.get_object(part.bucket, part.path)
            content_type = response.headers.get("Content-Type")

            return HttpResponse(response.data, content_type=content_type)
        except Part.DoesNotExist:
            return HttpResponseNotFound("Part not found")
        except:
            return HttpResponseNotFound("File not found")
        finally:
            if response:
                response.close()
                response.release_conn()