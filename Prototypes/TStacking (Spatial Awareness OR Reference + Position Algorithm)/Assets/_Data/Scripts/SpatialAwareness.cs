using System;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class SpatialAwareness : MonoBehaviour, IInputClickHandler
{
    public GameObject ContainerPlane;
    public GameObject containerMarks;
    public TextMesh InstructionTextMesh;
    public Material wireFrame20;
    public Material wireFrame75;
    public bool requestFinishScan;
    public bool finishScan;

    private void Start()
    {
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
                    this.InstructionTextMesh.text = "State: Scanning";
                    this.LogSurfaceState();
                    break;
                case SpatialUnderstanding.ScanStates.Finishing:
                    this.InstructionTextMesh.text = "State: Finishing Scan";
                    break;
                case SpatialUnderstanding.ScanStates.Done:
                    this.InstructionTextMesh.text = "State: Scan Finished\n" +
                                                                       "Please scan the container plane mark\n";
                    GameObject.FindGameObjectWithTag("SpatialUnderstanding").GetComponent<SpatialUnderstandingCustomMesh>().MeshMaterial = wireFrame20;
                    this.containerMarks.GetComponent<TrackableEventHandler>().enableScanContainerMark = true;
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
            this.InstructionTextMesh.text = string.Format("Please Air-Tap when you are finished");
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if(!requestFinishScan)
        {
            this.InstructionTextMesh.text = "Requested Finish Scan";
            SpatialUnderstanding.Instance.RequestFinishScan();
            requestFinishScan = true;
        }
    }
}