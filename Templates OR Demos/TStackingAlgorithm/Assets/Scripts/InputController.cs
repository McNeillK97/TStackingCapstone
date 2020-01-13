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

    //I added another button for generating the boxes so you are going to need to add this to the unity scene to make it work
    //Box button to be interactable
    public Button btnGC; //Generate container button
    public Button btnGB; //Generate Box button, clicking this button should keep generating boxes of the same size
    public Button btnSB; //Set box button, this sets the sizes of all box, all boxes will be the same size

    //Link the algorithm script
    private Algorithm algorithm;

    private void Start()
    {
        algorithm = GameObject.FindGameObjectWithTag("GameController").GetComponent<Algorithm>();
    }

    public void GenerateContainer()
    {
        if (x.text != "" && y.text != "" && z.text != "")
        {
            GameObject containerCreated = container;
            containerCreated.transform.localScale = new Vector3(float.Parse(x.text), float.Parse(y.text), float.Parse(z.text));
            GameObject.Instantiate(containerCreated);

            algorithm.SetContainer(containerCreated.transform.localScale);

            btnGC.interactable = false;
            btnSB.interactable = true;
        }
        else
        {
            Debug.Log("Input TextField cannot be empty");
        }
    }

    public void SetBox()
    {
        algorithm.SetBox(new Vector3(float.Parse(x.text), float.Parse(y.text), float.Parse(z.text)));
        btnSB.interactable = false;
        btnGB.interactable = true;
    }

    public void GenerateBox()
    {
        Debug.Log("1");
        algorithm.CalculatePosition();
    }
}
