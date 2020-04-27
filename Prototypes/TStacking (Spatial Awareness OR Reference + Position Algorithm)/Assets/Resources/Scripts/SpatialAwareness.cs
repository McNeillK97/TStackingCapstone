using System;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class SpatialAwareness : MonoBehaviour, IInputClickHandler
{
    //Materials to distinguish the scanning state finished scanning state
    public Material wireFrame20;
    public Material wireFrame75;

    private GameController gameController;
    [HideInInspector]
    public bool requestFinishScan;
    private bool finishScan;

    public void Initialization()
    {
        //Initialize components
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        requestFinishScan = false;
        finishScan = false;
    }

    public void ScanStateStart()
    {
        GameObject.FindGameObjectWithTag("SpatialUnderstanding").GetComponent<SpatialUnderstandingCustomMesh>().MeshMaterial = wireFrame75;

        InputManager.Instance.PushFallbackInputHandler(this.gameObject);
        SpatialUnderstanding.Instance.RequestBeginScanning();
        SpatialUnderstanding.Instance.ScanStateChanged += ScanStateChanged;
    }

    private void ScanStateChanged()
    {
        if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning)
        {
            LogSurfaceState();
        }
        else if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
        {

        }
    }

    private void OnDestroy()
    {
        try
        {
            SpatialUnderstanding.Instance.ScanStateChanged -= ScanStateChanged;
        }
        catch
        {
            Debug.Log("Cannot destroy");
        }
    }

    void Update()
    {
        if(!finishScan)
        {
            switch (SpatialUnderstanding.Instance.ScanState)
            {
                case SpatialUnderstanding.ScanStates.None:
                    break;

                case SpatialUnderstanding.ScanStates.ReadyToScan:
                    break;

                case SpatialUnderstanding.ScanStates.Scanning:

                    gameController.SetInstructionText("State: Scanning\n" +
                                                                             "Please look around");
                    Invoke("LogSurfaceState", 5f);
                    break;

                case SpatialUnderstanding.ScanStates.Finishing:

                    gameController.SetInstructionText("State: Finishing Scan");
                    break;

                case SpatialUnderstanding.ScanStates.Done:

                    gameController.SetInstructionText("State: Scan Finished\n" +
                                                                             "Please scan the virtual plane QR code");

                    GameObject.FindGameObjectWithTag("SpatialUnderstanding").GetComponent<SpatialUnderstandingCustomMesh>().MeshMaterial = wireFrame20;

                    //Activate the mechanism to scan the QR code of the virtual plane/container
                    gameController.SetActiveQRCode(true, false);

                    finishScan = true;
                    break;

                default:
                    break;
            }
        }
    }

    private void LogSurfaceState()
    {
        IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();

        if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) != 0)
        {
            var stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();

            gameController.SetInstructionText("Please Air-Tap when you finish scan");
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        //When the user air-tap and try to finish the scanning
        if(!requestFinishScan)
        {
            gameController.SetInstructionText("Are you sure you finish scan? Scan room cannot be restart.\n\n" +
                                                                     "Yes or No");

            //Shut down the audio FX of Scanning and play the audio FX of Next
            gameController.audioService.SetScanningAudio(false);
            gameController.audioService.PlayUIAudio(Constants.audioUINext, false);

            gameController.voiceCommand.SetYesNoCommand(true);
            gameController.voiceCommand.yesNoQues = QuestionType.scanFinish;

            requestFinishScan = true;
        }
    }

    public void RequestFinish()
    {
        SpatialUnderstanding.Instance.RequestFinishScan();
    }
}