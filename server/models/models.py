from django.db import models

class Part(models.Model):
    id = models.AutoField(auto_created=True, primary_key=True)

    name = models.CharField(max_length=255)

    file_bucket = models.CharField(max_length=255, help_text='S3 bucket where the file is located')

    file_path = models.CharField(max_length=255, help_text='Path to the S3 model file')

class Organization(models.Model):
    id = models.AutoField(auto_created=True, primary_key=True)

    name = models.CharField(max_length=255, help_text='Name of the organization')

class InstructionSet(models.Model):
    id = models.AutoField(auto_created=True, primary_key=True)

    organization = models.ForeignKey(Organization, on_delete=models.DO_NOTHING)
    
    name = models.CharField(max_length=255, help_text='Name of the instruction set')
    
class InstructionStep(models.Model):
    id = models.AutoField(auto_created=True, primary_key=True)

    instruction_set = models.ForeignKey(InstructionSet, on_delete=models.DO_NOTHING)

    sequence = models.PositiveIntegerField()
    
    text = models.CharField(max_length=1024, default='')
    
    part = models.ForeignKey(Part, on_delete=models.DO_NOTHING)

    initial_pos = models.JSONField(default=dict, help_text='Initial 3D position of the part in cartesian coordinates')

    initial_rot = models.JSONField(default=dict, help_text='Initial 3D rotation of the part')

    goal_pos = models.JSONField(default=dict, help_text='Goal 3D position of the part in cartesian coordinates')

    goal_rot = models.JSONField(default=dict, help_text='Goal 3D rotation of the part')
    
    scale = models.FloatField(default=1)

class UserMetric(models.Model):
    id = models.AutoField(auto_created=True, primary_key=True)

    age = models.PositiveSmallIntegerField()
    
class InstructionStepMetric(models.Model):
    id = models.AutoField(auto_created=True, primary_key=True)

    user = models.ForeignKey(UserMetric, on_delete=models.DO_NOTHING)

    instruction_step = models.ForeignKey(InstructionStep, on_delete=models.DO_NOTHING)

    duration = models.FloatField()