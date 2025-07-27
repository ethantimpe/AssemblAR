from os import getenv

from django.http import JsonResponse, HttpResponse, HttpRequest, HttpResponseNotAllowed, HttpResponseNotFound
from rest_framework.viewsets import ModelViewSet
from django_filters.rest_framework import DjangoFilterBackend

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
    filter_backends = [DjangoFilterBackend]
    filterset_fields = ['organization']

class InstructionStepViewSet(ModelViewSet):
    queryset = InstructionStep.objects.all()
    serializer_class = InstructionStepSerializer
    filter_backends = [DjangoFilterBackend]
    filterset_fields = ['instruction_set']

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
    def connect_minio(self):
        try:
            self.mc = Minio(
                    getenv('MINIO_HOSTNAME'),
                    access_key=getenv('MINIO_ACCESS_KEY'),
                    secret_key=getenv('MINIO_SECRET_KEY'),
                    secure=False
                  )
            
            self.mc.list_buckets()
            print('MinIO connection successful')
        except Exception as e:
            print(f'MinIO connection failed: {e}')


    def file_view(self, request: HttpRequest, file_id: int):
        self.connect_minio()

        if request.method != 'GET':
            return HttpResponseNotAllowed(['GET'])
        
        part = Part.objects.get(id=file_id)
        response = None
        # Get data of specified object
        try:
            response = self.mc.get_object(part.file_bucket, part.file_path)
            content_type = response.headers.get("Content-Type")

            return HttpResponse(response.data, content_type=content_type)
        except Part.DoesNotExist:
            return HttpResponseNotFound("Part not found")
        except Exception as e:
            return HttpResponseNotFound(f"File not found at '{part.file_bucket}/{part.file_path}': {e}")
        finally:
            if response:
                response.close()
                response.release_conn()