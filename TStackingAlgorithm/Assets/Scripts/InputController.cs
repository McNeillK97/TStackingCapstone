using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    //Recieve the data of the container or box X, Y, Z
    public InputField x;
    public InputField y;
    public InputField z;

    //Prefabs of container with anchor
    public GameObject container;

    //Box button to be interactable
    public Button btnGC;
    public Button btnGB;

    //Link the algorithm script
    private Algorithm algorithm;

    private void Start()
    {
        algorithm = GameObject.FindGameObjectWithTag("GameController").GetComponent<Algorithm>();
    }

    public void GenerateContainer()
    {
        if(x.text != "" && y.text != "" && z.text != "")
        {
            GameObject containerCreated = container;
            containerCreated.transform.localScale = new Vector3(float.Parse(x.text), float.Parse(y.text), float.Parse(z.text));
            GameObject.Instantiate(containerCreated);

            algorithm.SetContainer(containerCreated.transform.localScale);

            btnGC.interactable = false;
            btnGB.interactable = true;
        }
        else
        {
            Debug.Log("Input TextField cannot be empty");
        }
    }

    public void GsenerateBox()
    {
        algorithm.SetBox(new Vector3(float.Parse(x.text), float.Parse(y.text), float.Parse(z.text)));
    }
}
