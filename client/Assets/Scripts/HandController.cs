using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using NUnit.Framework.Constraints;

public class HandController : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float rayOffset = 0.2f;

    private bool aHeld = false;
    private bool xHeld = false;

    void Start()
    {

    }

    void Update()
    {
        Interact();
    }

    private void Interact()
    {
        var rightDevices = new List<InputDevice>();
        var leftDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightDevices);
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftDevices);

        // A Button (Right Controller)
        if (rightDevices.Count > 0)
        {
            InputDevice rightController = rightDevices[0];
            if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed))
            {
                if (aPressed && !aHeld)
                {
                    aHeld = true;
                    SpawnObject(rightHand);
                }
                else if (!aPressed)
                {
                    aHeld = false;
                }
            }
        }

        // X Button (Left Controller)
        if (leftDevices.Count > 0)
        {
            InputDevice leftController = leftDevices[0];
            if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool xPressed))
            {
                if (xPressed && !xHeld)
                {
                    xHeld = true;
                    SpawnObject(leftHand);
                }
                else if (!xPressed)
                {
                    xHeld = false;
                }
            }
        }
    }

    private void SpawnObject(GameObject obj)
    {
        Ray ray = new Ray(obj.transform.position + obj.transform.forward * rayOffset, obj.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject newObj = Instantiate(prefab);
            newObj.transform.position = hit.point;
        }
    }
}
