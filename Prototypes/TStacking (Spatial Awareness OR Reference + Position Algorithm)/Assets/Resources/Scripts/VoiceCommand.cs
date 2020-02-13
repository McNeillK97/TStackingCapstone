using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;

public enum QuestionType
{
    reset
}

public class VoiceCommand : MonoBehaviour
{
    public float boxLength, boxWidth, boxHeight;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions;
    private bool enableScan, enableRescan, enableNext, enableGenerate, enableBack,enableFinish, enableReset, enableYesNo;
    private bool enableScanBox;     //This one is for testing
    private QuestionType yesNoQues;

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
        actions.Add("reset", Reset);
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

    private void Rescan()
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

    private void Next()
    {
        if(enableNext)
        {
            //Update the instruction text  
            GameController.instance.SetInstructionText("Please say \"Set Box\" to set the box's information");

            GameController.instance.audioService.PlayUIAudio(Constants.audioUINext, false);

            enableScanBox = true;
            enableFinish = true;

            //Disable the voice command "Rescan" and "Next" 
            enableRescan = false;
            enableNext = false;
        }
    }

    public void Generate()
    {
        if(enableGenerate)
        {
            GameController.instance.SetInstructionText("Insert a box\n" +
                                                                                   "Say \"Generate\" to insert more box\n" +
                                                                                   "Say \"Back\" to undo\n" +
                                                                                   "Say \"Finish\" when you want to finish");

            GameController.instance.GenerateBox();

            GameController.instance.audioService.PlayUIAudio(Constants.audioUIScanBox, false);

            enableBack = true;
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

    private void Finish()
    {
        if(enableFinish)
        {
            //Update the instruction text
            GameController.instance.SetInstructionText("Scan finished\n" +
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

    private void Reset()
    {
        if(enableReset)
        {
            GameController.instance.SetInstructionText("Are you sure you want to reset?\n" +
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

    private void Yes()
    {
        if(enableYesNo)
        {
            switch (yesNoQues)
            {
                case QuestionType.reset:
                    GameController.instance.audioService.PlayUIAudio(Constants.audioUINext, false);
                    Invoke("InvokeReset", 0.5f);
                    break;
            }
        }
    }

    private void No()
    {
        if (enableYesNo)
        {
            switch (yesNoQues)
            {
                case QuestionType.reset:
                    GameController.instance.audioService.PlayUIAudio(Constants.audioUIBack, false);
                    GameController.instance.SetInstructionText("Scan finished\n" +
                                                                                           "Say \"Reset\" to replay");
                    enableReset = true;
                    break;
            }
        }
    }
    #endregion

    #region REGION_ONLY_FOR_TESTING 
    private void Set()
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

    public void SetScanBoxCommand(bool status)  //This is for testing
    {
        enableScanBox = status;
    }
    #endregion
}
