using UnityEngine;

public class BasicAnimation : MonoBehaviour
{
    public Vector3 initialPos = Vector3.zero;
    public Vector3 initialRot = Vector3.zero;
    public Vector3 goalPos = Vector3.zero;
    public Vector3 goalRot = Vector3.zero;
    public float animTime = 1;
    private float animTimer = 1;
    private bool isStopped = false;

    void Start()
    {
        transform.localPosition = initialPos;
        transform.rotation = Quaternion.Euler(initialRot);
    }

    void Update()
    {
        if (!isStopped)
        {
            animTimer -= Time.deltaTime;

            if (animTimer < 0)
            {
                animTimer = animTime;
            }

            transform.localPosition = Vector3.Lerp(goalPos, initialPos, animTimer / animTime);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(goalRot), Quaternion.Euler(initialRot), animTimer / animTime);
        }
    }

    public void Stop()
    {
        isStopped = true;
        transform.localPosition = goalPos;
        transform.rotation = Quaternion.Euler(goalRot);
    }

    public void Restart()
    {
        isStopped = false;
        transform.localPosition = initialPos;
        transform.rotation = Quaternion.Euler(initialRot);
    }
}
