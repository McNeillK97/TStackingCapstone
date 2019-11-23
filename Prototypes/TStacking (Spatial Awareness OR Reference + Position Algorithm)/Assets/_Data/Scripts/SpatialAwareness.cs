using System;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class SpatialAwareness : MonoBehaviour, IInputClickHandler
{
    public Material wireFrame20;
    public Material wireFrame75;
    
    private GameController gameController;
    private bool requestFinishScan;
    private bool finishScan;

    private void Start()
    {
        //initialize components
        requestFinishScan = false;
        finishScan = false;
        gameController = this.GetComponent<GameController>();
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
        SpatialUnderstanding.Instance.ScanStateChanged -= ScanStateChanged;
    }

    void Update()
    {
        if(!finishScan)
        {
            switch (SpatialUnderstanding.Instance.ScanState)
            {
                case SpatialUnderstanding.ScanStates.None:
                case SpatialUnderstanding.ScanStates.ReadyToScan:
                    break;
                case SpatialUnderstanding.ScanStates.Scanning:
                    string info = "State: Scanning";
                    gameController.SetInstructionText(info);

                    this.LogSurfaceState();
                    break;
                case SpatialUnderstanding.ScanStates.Finishing:
                    string info2 = "State: Finishing Scan";
                    gameController.SetInstructionText(info2);

                    break;
                case SpatialUnderstanding.ScanStates.Done:
                    string info3 = "State: Scan Finished\n" +
                                           "Please scan the container plane mark\n";
                    gameController.SetInstructionText(info3);

                    GameObject.FindGameObjectWithTag("SpatialUnderstanding").GetComponent<SpatialUnderstandingCustomMesh>().MeshMaterial = wireFrame20;

                    gameController.SetScanContainerMark(true);
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

            string info = string.Format("Please Air-Tap when you are finished");
            gameController.SetInstructionText(info);
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if(!requestFinishScan)
        {
            string info = "Requested Finish Scan";
            gameController.SetInstructionText(info);

            SpatialUnderstanding.Instance.RequestFinishScan();
            requestFinishScan = true;
        }
    }
}