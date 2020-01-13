using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceCommand : MonoBehaviour
{
    public float boxLength, boxWidth, boxHeight;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions;
    private GameController gameController;
    private bool enableScan, enableRescan, enableNext, enableGenerate, enableFinish;
    private bool enableScanBox;     //This one is for testing
    private void Start()
    {
        //Initialize components
        actions = new Dictionary<string, Action>();
        gameController = this.GetComponent<GameController>();
        enableScan = false;
        enableRescan = false;
        enableNext = false;
        enableGenerate = false;
        enableFinish = false;

        //Add the keyword and related functions
        actions.Add("scan", Scan);
        actions.Add("next", Next);
        actions.Add("rescan", Rescan);
        actions.Add("generate", Generate);
        actions.Add("finish", Finish);
        actions.Add("restart", Restart);

        #region REGION_ONLY_FOR_TESTING 
        enableScanBox = false;
        actions.Add("set box", Set);
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

    #region VOICE_COMMAND_LIST 
    private void Scan()
    {
        if(enableScan)
        {
            //Start the spatial awareness
            gameController.StartSpatialAwareness();

            //Disable the voice command "Scan"
            enableScan = false;
        }
    }

    private void Rescan()
    {
        if(enableRescan)
        {
            //Destroy the current container plane, the new container plane will be generated after scan the container mark
            Destroy(GameObject.FindGameObjectWithTag("ContainerPlane"));

            //Enable the container mark to scan again
            gameController.SetScanContainerMark(true);

            //Update the instruction text
            string info = "Please rescan the container mark";
            gameController.SetInstructionText(info);

            //Disable the voice command "Next" and "Rescan" 
            enableNext = false;
            enableRescan = false;
        }
    }

    private void Next()
    {
        if(enableNext)
        {
            //Update the instruction text
            string info = "Please say \"Set Box\" to set the box's information";
            gameController.SetInstructionText(info);

            enableScanBox = true;
            enableFinish = true;

            //Disable the voice command "Rescan" and "Next" 
            enableRescan = false;
            enableNext = false;
        }
    }

    private void Generate()
    {
        if(enableGenerate)
        {
            string info = "Insert a box\n" +
                                 "Say \"Finish\" when you want to finish";
            gameController.SetInstructionText(info);

            gameController.GenerateBox();
        }
    }

    private void Finish()
    {
        if(enableFinish)
        {
            //Update the instruction text
            string info = "Scan finished\n" + 
                                 "Say \"Restart\" to replay";
            gameController.SetInstructionText(info);

            //Disable the voice command  "Generate"
            enableGenerate= false;  
            enableFinish = false;
        }
    }

    private void Restart()
    {
        //TBC
    }
    #endregion

    #region REGION_ONLY_FOR_TESTING 
    private void Set()
    {
        if(enableScanBox)
        {
            string info = "Insert same boxes\n" +
                                 "Length: " + boxLength + "m, Width: " + boxWidth + "m, Height: " + boxHeight  + "m\n" +
                                 "Say \"Generate\" when you want to insert";
            gameController.SetInstructionText(info);

            gameController.SetBoxInfo(boxLength, boxWidth, boxHeight);
        }
    }
    #endregion

    #region SET_VOICE_COMMANDS_STATUS 
    public void SetScanCommand(bool status)
    {
        enableScan = status;
    }

    public void SetRescanCommand(bool status)
    {
        enableRescan = status;
    }

    public void SetNextCommand(bool status)
    {
        enableNext = status;
    }

    public void SetGenerateCommand(bool status)
    {
        enableGenerate = status;
    }

    public void SetFinishCommand(bool status)
    {
        enableFinish = status;
    }

    public void SetScanBoxCommand(bool status)  //This is for testing
    {
        enableScanBox = status;
    }
    #endregion
}
