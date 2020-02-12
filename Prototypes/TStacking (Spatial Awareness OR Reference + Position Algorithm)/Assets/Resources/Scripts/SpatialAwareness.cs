using System;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class SpatialAwareness : MonoBehaviour, IInputClickHandler
{
    public Material wireFrame20;
    public Material wireFrame75;
    
    private bool requestFinishScan;
    private bool finishScan;

    private void Start()
    {
        //initialize components
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

                    GameController.instance.SetInstructionText("State: Scanning");
                    this.LogSurfaceState();
                    break;

                case SpatialUnderstanding.ScanStates.Finishing:

                    GameController.instance.SetInstructionText("State: Finishing Scan");
                    break;

                case SpatialUnderstanding.ScanStates.Done:

                    GameController.instance.SetInstructionText("State: Scan Finished\n" +
                                                                                           "Please scan the container plane mark\n");
                    GameObject.FindGameObjectWithTag("SpatialUnderstanding").GetComponent<SpatialUnderstandingCustomMesh>().MeshMaterial = wireFrame20;
                    GameController.instance.SetScanContainerMark(true);
                    this.finishScan = true;
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

            GameController.instance.SetInstructionText("Please Air-Tap when you are finished");
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if(!requestFinishScan)
        {
            GameController.instance.SetInstructionText("Requested Finish Scan");

            GameController.instance.audioService.SetScanningAudio(false);
            GameController.instance.audioService.PlayUIAudio(Constants.audioUINext, false);

            SpatialUnderstanding.Instance.RequestFinishScan();
            requestFinishScan = true;
        }
    }
}