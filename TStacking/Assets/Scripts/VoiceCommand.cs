using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceCommand : MonoBehaviour
{
    public GameObject BoxScan;
    public Transform ContainerPlane;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    private void Start()
    {
        //Add the keyword and related functions
        actions.Add("next", Next);
        actions.Add("large", AddLargeBox);

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void Next()
    {
        BoxScan.SetActive(true);   
    }

    private void AddLargeBox()
    {
        GameObject largeBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        largeBox.transform.SetParent(ContainerPlane, true);
        largeBox.transform.position = ContainerPlane.position;

    }
}
