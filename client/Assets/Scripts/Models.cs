using System;
using UnityEngine;

[Serializable]
public class Organization
{
    public int id;
    public string name;
}

[Serializable]
public class InstructionSet
{
    public int id;
    public int organization;
    public Organization Organization = null;
    public string name;
}

[Serializable]
public class InstructionStep
{
    public int id;
    public int instruction_set;
    public InstructionSet InstructionSet = null;
    public int sequence;
    public string text;
    public int part;
    public Part Part;
    public Vector3 initial_pos;
    public Vector3 initial_rot;
    public Vector3 goal_pos;
    public Vector3 goal_rot;
    public float scale;
}

[Serializable]
public class Part
{
    public int id;
    public string name;
    public string file_bucket;
    public string file_path;
    public GameObject gameObject = null;
}

[Serializable]
public class UserMetric
{
    public int id;
    public int age;
}

[Serializable]
public class InstructionStepMetric
{
    public int instruction_step;
    public float duration;

    public InstructionStepMetric(int step, float duration)
    {
        this.instruction_step = step;
        this.duration = duration;
    }
}