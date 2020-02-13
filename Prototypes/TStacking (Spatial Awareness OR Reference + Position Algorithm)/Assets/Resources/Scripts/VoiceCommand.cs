using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;

public enum QuestionType
{
    scanFinish,
    reset
}

public class VoiceCommand : MonoBehaviour
{
    public float boxLength, boxWidth, boxHeight;
    public QuestionType yesNoQues;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions;
    private bool enableScan, enableRescan, enableNext, enableGenerate, enableBack,enableFinish, enableReset, enableYesNo;
    private bool enableScanBox;     //This one is for testing

    private void Start()
    {
        //Initialize components
        actions = new Dictionary<string, Action>();

        enableScan = false;
        enableRescan = false;
        enableNext = false;
        enableGenerate = false;
        enableBack = false;
        enableFinish = false;
        enableReset = false;
        enableYesNo = false;

        //Add the keyword and related functions
        actions.Add("scan", Scan);
        actions.Add("next", Next);
        actions.Add("rescan", Rescan);
        actions.Add("generate", Generate);
        actions.Add("back", Back);
        actions.Add("finish", Finish);
        actions.Add("reset", Restart);
        actions.Add("yes", Yes);
        actions.Add("no", No);

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
        if (enableScan)
        {
            //Start the spatial awareness
            GameController.instance.StartSpatialAwareness();

            GameController.instance.audioService.PlayUIAudio(Constants.audioUINext, false);
            GameController.instance.audioService.SetScanningAudio(true);

            //Disable the voice command "Scan"
            enableScan = false;
            enableReset = true;
        }
    }

    public void Rescan()
    {
        if(enableRescan)
        {
            //Destroy the current container plane, the new container plane will be generated after scan the container mark
            Destroy(GameObject.FindGameObjectWithTag("ContainerPlane"));

            //Enable the container mark to scan again
            GameController.instance.SetScanContainerMark(true);

            //Update the instruction text
            GameController.instance.SetInstructionText("Please rescan the container mark");

            GameController.instance.audioService.PlayUIAudio(Constants.audioUIBack, false);

            //Disable the voice command "Next" and "Rescan" 
            enableNext = false;
            enableRescan = false;
        }
    }

    public void Next()
    {
        if(enableNext)
        {
            //Update the instruction text  
            GameController.instance.SetInstructionText("Please say \"Set Box\" to set the box's information");

            GameController.instance.audioService.PlayUIAudio(Constants.audioUINext, false);

            enableScanBox = true;

            //Disable the voice command "Rescan" and "Next" 
            enableRescan = false;
            enableNext = false;
        }
    }

    public void Generate()
    {
        if(enableGenerate)
        {
            GameController.instance.SetInstructionText("Insert a box\n\n" +
                                                                                   "Say \"Generate\" to insert more box\n" +
                                                                                   "Say \"Back\" to undo\n" +
                                                                                   "Say \"Finish\" when you want to finish");

            GameController.instance.GenerateBox();

            GameController.instance.audioService.PlayUIAudio(Constants.audioUIScanBox, false);

            enableBack = true;
            enableFinish = true;
        }
    }

    public void Back()
    {
        if(enableBack)
        {
            GameController.instance.audioService.PlayUIAudio(Constants.audioUIBack, false);
            GameController.instance.Undo();
        }
    }

    public void Finish()
    {
        if(enableFinish)
        {
            //Update the instruction text
            GameController.instance.SetInstructionText("Scan finished\n\n" +
                                                                                   "Say \"Reset\" to replay");
            GameController.instance.FinishCalculation();
            GameController.instance.audioService.PlayUIAudio(Constants.audioUINext, false);

            //Disable the voice command  "Generate"
            enableGenerate = false;
            enableBack = false;
            enableFinish = false;
            enableReset = true;
        }
    }

    public void Restart()
    {
        if(enableReset)
        {
            GameController.instance.SetInstructionText("Are you sure you want to reset?\n\n" +
                                                                                   "Yes or No");

            GameController.instance.audioService.PlayUIAudio(Constants.audioUINext, false);

            yesNoQues = QuestionType.reset;
            enableReset = false;
            enableYesNo = true;
        }
    }

    private void InvokeReset()
    {
        //Voice Command Reset
        enableScan = false;
        enableRescan = false;
        enableNext = false;
        enableGenerate = false;
        enableFinish = false;
        enableReset = false;

        Destroy(GameObject.FindGameObjectWithTag("ContainerPlane"));

        GameController.instance.Restart();
    }

    public void Yes()
    {
        if(enableYesNo)
        {
            switch (yesNoQues)
            {
                case QuestionType.reset:
                    GameController.instance.audioService.PlayUIAudio(Constants.audioUINext, false);
                    Invoke("InvokeReset", 0.5f);
                    break;

                case QuestionType.scanFinish:
                    GameController.instance.audioService.PlayUIAudio(Constants.audioUINext, false);
                    GameController.instance.spatialAwareness.RequestFinish();
                    break;
            }

            enableYesNo = false;
        }
    }

    public void No()
    {
        if (enableYesNo)
        {
            switch (yesNoQues)
            {
                case QuestionType.reset:
                    GameController.instance.audioService.PlayUIAudio(Constants.audioUIBack, false);
                    GameController.instance.SetInstructionText("Scan finished\n\n" +
                                                                                           "Say \"Reset\" to replay");
                    enableReset = true;
                    break;

                case QuestionType.scanFinish:
                    GameController.instance.audioService.PlayUIAudio(Constants.audioUIBack, false);
                    GameController.instance.audioService.SetScanningAudio(true);
                    GameController.instance.SetInstructionText("Please Air-Tap when you finish scan");
                    GameController.instance.spatialAwareness.requestFinishScan = false;
                    break;
            }

            enableYesNo = false;
        }
    }
    #endregion

    #region REGION_ONLY_FOR_TESTING 
    public void Set()
    {
        if(enableScanBox)
        {
            GameController.instance.SetInstructionText("Insert same boxes\n" +
                                                                                   "Length: " + boxLength + "m, Width: " + boxWidth + "m, Height: " + boxHeight + "m\n" +
                                                                                   "Say \"Generate\" when you want to insert");

            GameController.instance.SetBoxInfo(boxLength, boxWidth, boxHeight);

            GameController.instance.audioService.PlayUIAudio(Constants.audioUINext, false);
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

    public void SetYesNoCommand(bool status)
    {
        enableYesNo = status;
    }

    public void SetScanBoxCommand(bool status)  //This is for testing
    {
        enableScanBox = status;
    }
    #endregion
}
