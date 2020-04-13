using UnityEngine;
using UnityEngine.UI;
using System;

public class QRCode : MonoBehaviour
{
    public float length, width, height, weight;
    public bool enableScanPlane = false;
    public bool enableScanBox = false;

    public InputField IFLength, IFWidth, IFHeight, IFWeight;

    private GameController gameController;

    private void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void ScanQRCode()
    {
        if(enableScanPlane || enableScanBox)
        {
            ConvertUnit();
            gameController.SetQRInformation(length, width, height, weight);
        }
    }

    private void ConvertUnit()
    {
        length = (float)Convert.ToDouble(IFLength.text);
        width = (float)Convert.ToDouble(IFWidth.text);
        height = (float)Convert.ToDouble(IFHeight.text);
        weight = (float)Convert.ToDouble(IFWeight.text);
    }
}
