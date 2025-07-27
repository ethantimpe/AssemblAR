from rest_framework import serializers
from rest_framework.serializers import ModelSerializer
import django_filters

from .models import *

class PartSerializer(ModelSerializer):
    class Meta:
        model = Part
        fields = '__all__'

class OrganizationSerializer(ModelSerializer):
    class Meta:
        model = Organization
        fields = '__all__'

class InstructionSetSerializer(ModelSerializer):
    class Meta:
        model = InstructionSet
        fields = '__all__'

class InstructionStepSerializer(ModelSerializer):

    class Meta:
        model = InstructionStep
        fields = '__all__'

class UserMetricSerializer(ModelSerializer):
    class Meta:
        model = UserMetric
        fields = '__all__'
    
class InstructionStepMetricSerializer(ModelSerializer):
    class Meta:
        model = InstructionStepMetric
        fields = '__all__'
