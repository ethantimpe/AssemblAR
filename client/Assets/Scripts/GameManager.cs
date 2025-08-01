using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Sim")]
    [SerializeField] private GameState state = GameState.LOADING;
    [SerializeField] private GameObject objectParent;
    [SerializeField] private Vector3 objectParentOffset;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private float rayOffset = 0.2f;
    [SerializeField] private Material defaultMaterial;

    private bool aHeld = false;
    private bool xHeld = false;

    [Header("Server")]
    [SerializeField] private string backendHost = "http://127.0.0.1";
    [SerializeField] private int backendPort = 8000;
    [SerializeField] private string organizationEndpoint = "organization";
    [SerializeField] private string instructionSetEndpoint = "instruction_set";
    [SerializeField] private string instructionStepEndpoint = "instruction_step";
    [SerializeField] private string partEndpoint = "part";
    [SerializeField] private string userMetricEndpoint = "metrics/user";
    [SerializeField] private string instructionStepMetricEndpoint = "metrics/instruction_step";
    [SerializeField] private string fileEndpoint = "file";

    [Header("UI")]
    [SerializeField] private GameObject stepIconParent;
    [SerializeField] private GameObject stepIconPrefab;
    private List<Image> stepIconList = new List<Image>();
    [SerializeField] private Color currentColor;
    [SerializeField] private Color finishedColor;
    [SerializeField] private Color todoColor;
    [SerializeField] private Text stepText;
    [SerializeField] private Button startButton;
    [SerializeField] private Image loadingBar;
    [SerializeField] private Button nextStepButton;
    [SerializeField] private Button exitButton;

    private List<InstructionStep> instructionSteps;
    private int currentStep = -1;

    private float stepTimer = 0;

    private enum GameState
    {
        LOADING,
        PLACING,
        PLAYING,
        FINISHED
    }

    void Start()
    {
        StartCoroutine(InitializeInstructionSet());

        loadingBar.gameObject.SetActive(true);
        nextStepButton.gameObject.SetActive(false);
    }

    void Update()
    {
        try
        {
            // Game loop state machine
            switch (state)
            {
                case GameState.LOADING:
                    Loading();
                    break;
                case GameState.PLACING:
                    Placing();
                    break;
                case GameState.PLAYING:
                    Playing();
                    break;
                case GameState.FINISHED:
                    Finished();
                    break;
            }
        }
        catch (Exception e)
        {
            stepText.text = e.Message;
        }
    }

    private void Loading()
    {

    }

    private void Placing()
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
                    // Place where pointing
                    aHeld = true;
                    state = GameState.PLAYING;

                    stepText.text = instructionSteps[currentStep].text;
                    stepIconList[currentStep].color = currentColor;
                    instructionSteps[currentStep].Part.gameObject.GetComponent<BasicAnimation>().enabled = true;
                    instructionSteps[currentStep].Part.gameObject.GetComponent<BasicAnimation>().Restart();
                    nextStepButton.gameObject.SetActive(true);
                }
                else if (!aPressed)
                {
                    aHeld = false;

                    PreviewObject(rightHand);
                }
            }
        }
    }

    private void Playing()
    {
        stepTimer += Time.deltaTime;

        // var rightDevices = new List<InputDevice>();
        // var leftDevices = new List<InputDevice>();
        // InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightDevices);
        // InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftDevices);

        // // A Button (Right Controller)
        // if (rightDevices.Count > 0)
        // {
        //     InputDevice rightController = rightDevices[0];
        //     if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed))
        //     {
        //         if (aPressed && !aHeld)
        //         {
        //             aHeld = true;

        //             AdvanceStep();
        //         }
        //         else if (!aPressed)
        //         {
        //             aHeld = false;
        //         }
        //     }
        // }
    }

    private void Finished()
    {
        nextStepButton.gameObject.SetActive(false);
        stepIconParent.SetActive(false);
        stepText.text = "Finished.";
        exitButton.gameObject.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit(0);
    }

    public void AdvanceStep()
    {
        StartCoroutine(PostMetric());
        instructionSteps[currentStep].Part.gameObject.GetComponent<BasicAnimation>().Stop();
        stepIconList[currentStep].color = finishedColor;

        if (currentStep == instructionSteps.Count - 1)
        {
            Finish();
        }
        else
        {
            currentStep++;
            instructionSteps[currentStep].Part.gameObject.SetActive(true);
            stepIconList[currentStep].color = currentColor;
            BasicAnimation anim = instructionSteps[currentStep].Part.gameObject.GetComponent<BasicAnimation>();
            anim.enabled = true;
            anim.Restart();
            stepText.text = instructionSteps[currentStep].text;
        }
    }

    private void PreviewObject(GameObject obj)
    {
        Ray ray = new Ray(obj.transform.position + obj.transform.forward * rayOffset, obj.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            objectParent.transform.position = hit.point + objectParentOffset;
        }
    }

    private void Finish()
    {
        state = GameState.FINISHED;
        Debug.Log("Finished");
    }

    IEnumerator InitializeInstructionSet()
    {
        try
        {

        }
        catch (Exception e)
        {
            
        }
        float tic = Time.time;

        loadingBar.fillAmount = 0.1f;

        // Get instruction set
        UnityWebRequest req = UnityWebRequest.Get($"{backendHost}:{backendPort}/{instructionSetEndpoint}/1");
        yield return req.SendWebRequest();
        Debug.Log(req.downloadHandler.text);
        req.Dispose();
        loadingBar.fillAmount = 0.2f;

        // Get instruction steps
        req = UnityWebRequest.Get($"{backendHost}:{backendPort}/{instructionStepEndpoint}?instruction_set=1");
        yield return req.SendWebRequest();
        instructionSteps = JsonConvert.DeserializeObject<List<InstructionStep>>(req.downloadHandler.text);
        instructionSteps.Sort((a, b) => a.sequence.CompareTo(b.sequence));
        req.Dispose();
        loadingBar.fillAmount = 0.3f;

        // Get parts
        foreach (InstructionStep step in instructionSteps)
        {
            // Get part location
            req = UnityWebRequest.Get($"{backendHost}:{backendPort}/{partEndpoint}/{step.part}");
            yield return req.SendWebRequest();
            Part part = JsonConvert.DeserializeObject<Part>(req.downloadHandler.text);
            step.Part = part;

            req.Dispose();

            Debug.Log($"Pulled instruction_step \"{step.text}\" with part \"{part.name}\"");

            // Get part model
            req = UnityWebRequest.Get($"{backendHost}:{backendPort}/{fileEndpoint}/{part.id}");
            yield return req.SendWebRequest();

            // Create part GameObject
            try
            {
                GameObject obj = GLB.ToGameObject(req.downloadHandler.data);
                obj.transform.SetParent(objectParent.transform);
                obj.SetActive(false);
                obj.name = part.name;

                MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer mesh in meshes){
                    mesh.material = defaultMaterial;
                }

                // Assign animation
                BasicAnimation anim = obj.AddComponent<BasicAnimation>();
                anim.initialPos = step.initial_pos;
                anim.goalPos = step.goal_pos;
                anim.initialRot = step.initial_rot;
                anim.goalRot = step.goal_rot;
                anim.enabled = false;
                // anim.scale = step.scale;
                part.gameObject = obj;
                req.Dispose();
            }
            catch (Exception e)
            {
                stepText.text = e.ToString();
            }

            loadingBar.fillAmount += 0.7f / instructionSteps.Count;
            GameObject newStepIcon = Instantiate(stepIconPrefab);
            newStepIcon.transform.SetParent(stepIconParent.transform, false);
            Image img = newStepIcon.GetComponent<Image>();
            stepIconList.Add(img);
            img.color = todoColor;

            Debug.Log($"Created GameObject for \"{step.Part.name}\"");
        }

        Debug.Log($"Loaded instruction set in {(Time.time - tic).ToString("f6")} seconds");

        currentStep = 0;
        instructionSteps[currentStep].Part.gameObject.SetActive(true);
        loadingBar.gameObject.SetActive(false);
        stepIconParent.gameObject.SetActive(true);
        stepText.text = "Point to a spot on the floor and press A to begin instructions.";
        state = GameState.PLACING;
    }

    IEnumerator PostMetric()
    {
        float time = stepTimer;
        int id = instructionSteps[currentStep].id;
        stepTimer = 0;

        InstructionStepMetric metric = new InstructionStepMetric(id, time);
        string metricJson = JsonConvert.SerializeObject(metric);
        UnityWebRequest req = UnityWebRequest.Post($"{backendHost}:{backendPort}/{instructionStepMetricEndpoint}/",
                                                   metricJson,
                                                   "application/json");
        yield return req.SendWebRequest();
        Debug.Log($"POSTed metric \"{metricJson}\" to endpoint \"{backendHost}:{backendPort}/{instructionStepMetricEndpoint}/\": {req.responseCode}");
        req.Dispose();
    }
}
