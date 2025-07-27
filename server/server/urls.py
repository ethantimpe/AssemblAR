"""
URL configuration for server project.

The `urlpatterns` list routes URLs to views. For more information please see:
    https://docs.djangoproject.com/en/4.2/topics/http/urls/
Examples:
Function views
    1. Add an import:  from my_app import views
    2. Add a URL to urlpatterns:  path('', views.home, name='home')
Class-based views
    1. Add an import:  from other_app.views import Home
    2. Add a URL to urlpatterns:  path('', Home.as_view(), name='home')
Including another URLconf
    1. Import the include() function: from django.urls import include, path
    2. Add a URL to urlpatterns:  path('blog/', include('blog.urls'))
"""
from django.contrib import admin
from django.urls import path, include
from rest_framework import routers

from models.views import *

router = routers.DefaultRouter()
router.register(r'part', PartViewSet)
router.register(r'organization', OrganizationViewSet)
router.register(r'instruction_set', InstructionSetViewSet)
router.register(r'instruction_step', InstructionStepViewSet)
router.register(r'metrics/user', UserMetricViewSet)
router.register(r'metrics/instruction_step', InstructionStepMetricViewSet)

file_view = FileView()

urlpatterns = [
    path('admin/', admin.site.urls),
    path('', include(router.urls)),
    path('file/<int:file_id>/', file_view.file_view),
]
