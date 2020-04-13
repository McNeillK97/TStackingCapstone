using UnityEngine;

public class GameController : MonoBehaviour
{
    //public static GameController instance;

    public GameObject containerPlane;
    public GameObject containerMarks;
    public TextMesh InstructionTextMesh;
    public bool openSpatialAwareness;

    [Header("Mode 1")]
    public bool isMode1 = false;

    [Header("Mode 2")]
    public bool isMode2 = false;
    public bool isPregenerateRandomBoxes = false;

    [HideInInspector]
    public AudioService audioService;
    [HideInInspector]
    public VoiceCommand voiceCommand;
    private Algorithm algorithm;
    [HideInInspector]
    public SpatialAwareness spatialAwareness;

    private QRCode qRCode;
    private bool enableScanPlane = false;
    private bool enableScanBox = false;

    private void Start()
    {
        audioService = GetComponent<AudioService>();
        spatialAwareness = GetComponent<SpatialAwareness>();
        voiceCommand = GetComponent<VoiceCommand>();
        algorithm = GetComponent<Algorithm>();
        qRCode = GameObject.FindGameObjectWithTag("QRCode").GetComponent<QRCode>();

        //If open the spatial awareness or not
        if (openSpatialAwareness)
        {
            spatialAwareness.enabled = true;
            voiceCommand.SetScanCommand(true);
            InstructionTextMesh.text = "Please say \"Scan\" to start scanning the room";
        }
        else
        {
            InstructionTextMesh.text = "Please scan the virtual plane QR code";
            SetActiveQRCode(true, false);
        }

        //Play the background music
        audioService.PlayBgMusic(Constants.audioBgName, true);
    }

    public void Restart()
    {
        InstructionTextMesh.text = "Please scan the virtual plane QR code";
        SetActiveQRCode(true, false);
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
        InstructionTextMesh.text = "Please drag the virtual plane to a comfortable position\n\n" +
                                                     "Say \"Next\" when you finish\n" +
                                                     "Say \"Rescan\" to rescan the virtual plane QR code";

        // Enable Voice command to say "Rescan" and "Next"
        voiceCommand.SetRescanCommand(true);
        voiceCommand.SetNextCommand(true);
    }

    public void AddBoxInfo(float length, float width, float height, float weight)
    {
        algorithm.AddBox(new Vector3(length, height, width));
        voiceCommand.SetScanBoxCommand(false);
        voiceCommand.SetGenerateCommand(true);
    }

    public void GenerateBox()
    {
        //Mode 1 use FIFO
        if (isMode1)
        {
            algorithm.CalculatePosition();
        }
        //Mode 2 use 3D array to check position
        else if (isMode2)
        {
            algorithm.CalculatePosition2();
        }
    }

    public void SetActiveQRCode(bool enableScanPlane, bool enableScanBox)
    {
        qRCode.enableScanPlane = enableScanPlane;
        qRCode.enableScanBox = enableScanBox;

        this.enableScanPlane = enableScanPlane;
        this.enableScanBox = enableScanBox;
    }

    public void SetQRInformation(float length, float width, float height, float weight)
    {
        if(enableScanPlane)
        {
            GenerateContainerPlane(length, width, height);
            audioService.PlayUIAudio(Constants.audioUINext, false);
            SetActiveQRCode(false, false);
        }
        else if(enableScanBox)
        {
            audioService.PlayUIAudio(Constants.audioUINext, false);
            AddBoxInfo(length, width, height, weight);
        }
    }

    public void Undo()
    {
        algorithm.Undo();
        audioService.PlayUIAudio(Constants.audioUIBack, false);
    }

    public void FinishCalculation()
    {
        algorithm.Finish();
    }

    public void SetInstructionText(string info)
    {
        InstructionTextMesh.text = info;
    }
}
