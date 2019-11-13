using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceCommand : MonoBehaviour
{
    public GameObject containerMarks;
    public TextMesh InstructionTextMesh;
    public bool enableScan;
    public bool enableRescan;
    public bool enableNext;
    public bool enableFinish;

    //************* For Testing **************
    public bool enableScanBox;
    private GameController gameController;
    //****************************************

    private SpatialAwareness spatialAwareness;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    private void Start()
    {
        spatialAwareness = this.GetComponent<SpatialAwareness>();

        //*********** For Testing ***************
        gameController = this.GetComponent<GameController>();
        //***************************************

        enableScan = false;
        enableRescan = false;
        enableNext = false;
        enableFinish = false;

        //************* For Testing **************
        enableScanBox = false;
        //****************************************

        //Add the keyword and related functions
        actions.Add("scan", Scan);
        actions.Add("next", Next);
        actions.Add("rescan", Rescan);
        actions.Add("finish", Finish);
        actions.Add("restart", Restart);

        #region REGION_ONLY_FOR_TESTING 
        actions.Add("large", large);
        actions.Add("medium", medium);
        actions.Add("small", small);
        #endregion

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void Scan()
    {
        if(enableScan)
        {
            spatialAwareness.ScanStateStart();

            enableScan = false;
        }
    }

    private void Rescan()
    {
        if(enableRescan)
        {
            Destroy(GameObject.FindGameObjectWithTag("ContainerPlane"));
            containerMarks.GetComponent<TrackableEventHandler>().enableScanContainerMark = true;
            InstructionTextMesh.text = "Please rescan the container mark";

            enableNext = false;
            enableRescan = false;
        }
    }

    private void Next()
    {
        if(enableNext)
        {
            InstructionTextMesh.text = "You can scan the box mark right now";

            enableScanBox = true;
            enableFinish = true;
            enableRescan = false;

            enableNext = false;
        }
    }

    private void Finish()
    {
        if(enableFinish)
        {
            InstructionTextMesh.text = "Scan finished\n" + 
                                                         "Say \"Restart\" to replay";

            enableScanBox = false;  //TEST

            enableFinish = false;
        }
    }

    private void Restart()
    {
        //Need to finish
    }

    #region REGION_ONLY_FOR_TESTING 
    private void large()
    {
        if(enableScanBox)
        {
            InstructionTextMesh.text = "Insert a large box\n" +
                                                         "Say \"Finish\" when you want to finish";
            this.gameController.generateBox(0.45f, 0.45f, 0.45f);
        }
    }

    private void medium()
    {
        if(enableScanBox)
        {
            InstructionTextMesh.text = "Insert a medium box\n" +
                                                         "Say \"Finish\" when you want to finish";
            this.gameController.generateBox(0.3f, 0.3f, 0.4f);
        }
    }

    private void small()
    {
        if(enableScanBox)
        {
            InstructionTextMesh.text = "Insert a small box\n" +
                                                         "Say \"Finish\" when you want to finish";
            this.gameController.generateBox(0.2f, 0.2f, 0.2f);
        }
    }
    #endregion
}
