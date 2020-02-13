using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public GameObject containerPlane;
    public GameObject containerMarks;
    public TextMesh InstructionTextMesh;
    public bool openSpatialAwareness;

    [HideInInspector]
    public AudioService audioService;
    private VoiceCommand voiceCommand;
    private Algorithm algorithm;
    private SpatialAwareness spatialAwareness;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioService = this.GetComponent<AudioService>();
        spatialAwareness = this.GetComponent<SpatialAwareness>();
        voiceCommand = this.GetComponent<VoiceCommand>();
        algorithm = this.GetComponent<Algorithm>();

        if (openSpatialAwareness)
        {
            spatialAwareness.enabled = true;
            voiceCommand.SetScanCommand(true);
            InstructionTextMesh.text = "Please Say \"Scan\" to Start the Spatial Awareness";
        }
        else
        {
            InstructionTextMesh.text = "Please scan the container mark";
            SetScanContainerMark(true);
        }

        audioService.PlayBgMusic(Constants.audioBgName, true);
    }

    public void Restart()
    {
        InstructionTextMesh.text = "Please scan the container mark";
        SetScanContainerMark(true);
    }

    public void GenerateContainerPlane(float length, float width, float height)
    {
        // Generate the container plane
        GameObject containerPlaneCreated = containerPlane;
        containerPlaneCreated.transform.localScale = new Vector3(length, 0.01f, width);
        GameObject.Instantiate(containerPlaneCreated, GameObject.FindGameObjectWithTag("MainCamera").transform.position + (Vector3.down * 0.25f), containerPlane.transform.rotation);

        // Set the container plane info to the algorithm script
        algorithm.SetContainerInfo(new Vector3(length, height, width));

        // Show the instruction message
        InstructionTextMesh.text = "Please drag the virtual plane to the position\n" +
                                                     "right above the real container or a comfortable position\n" +
                                                     "Say \"Next\" when you finish\n" +
                                                     "Say \"Rescan\" if you want to rescan the plane mark";

        // Enable Voice command to say "Rescan" and "Next"
        voiceCommand.SetRescanCommand(true);
        voiceCommand.SetNextCommand(true);
    }

    public void SetBoxInfo(float length, float width, float height)
    {
        algorithm.SetBoxInfo(new Vector3(length, height, width));
        voiceCommand.SetScanBoxCommand(false);
        voiceCommand.SetGenerateCommand(true);
    }

    public void GenerateBox()
    {
        algorithm.CalculatePosition();
    }

    public void Undo()
    {
        algorithm.Undo();
    }

    public void FinishCalculation()
    {
        algorithm.Finish();
    }

    public void StartSpatialAwareness()
    {
        spatialAwareness.ScanStateStart();
    }

    public void SetInstructionText(string info)
    {
        InstructionTextMesh.text = info;
    }

    public void SetScanContainerMark(bool status)
    {
        containerMarks.GetComponent<TrackableEventHandler>().SetEnableScanContainerMark(status);
    }
}
