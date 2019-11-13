using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateBoxes : MonoBehaviour
{
    public Button enter;
    public Button enterC;
    public float offsetX;
    public float offsetY;
    public float offsetZ;
    public int boxNum;
    public int queueSize;
    public GameObject waitQueue;
    public bool conCreated;
    public InputField inputX;
    public InputField inputY;
    public InputField inputZ;
    public float inX;
    public float inY;
    public float inZ;
    public float conX;
    public float conY;
    public float conZ;

    public GameObject rows;
    public GameObject currentRow;
    int row;
    // Start is called before the first frame update
    void Start()
    {
        waitQueue = GameObject.Find("waitQueue");
        rows = GameObject.Find("Rows");
        offsetX = 0;
        offsetY = 0;
        offsetZ = 0;
        boxNum = 1;
        queueSize = 0;
        row = 1;
        conCreated = false;

        GameObject newRow = new GameObject();
        newRow.name = "row " + row;
        row++;
        newRow.transform.parent = rows.transform;
        currentRow = newRow;

        enter = GameObject.Find("EnterButton").GetComponent<Button>();
        enter.onClick.AddListener(() => addBox());

        enterC = GameObject.Find("EnterC").GetComponent<Button>();
        enterC.onClick.AddListener(() => createContainer());

        inputX = GameObject.Find("InputX").GetComponent<InputField>();
        inputY = GameObject.Find("InputY").GetComponent<InputField>();
        inputZ = GameObject.Find("InputZ").GetComponent<InputField>();

    }

    // Update is called once per frame
    void Update()
    {
        //check if container has been created and if there are 5 boxes in queue
        if (conCreated && queueSize >= 5)
        {
            addToFormation();
        }
    }

    //adding box to queue
    void addBox()
    {
        //get input
        inX = float.Parse(inputX.text);
        inY = float.Parse(inputY.text);
        inZ = float.Parse(inputZ.text);
        //create box and add to queue
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(inX, inY, inZ);
        cube.transform.position = new Vector3(-20, -20, -20);
        cube.name = "Box " + boxNum;
        boxNum++;
        cube.transform.parent = waitQueue.transform;
        queueSize++;
    }

    //creating container
    void createContainer()
    {

        if (conCreated == false)
        {
            //get input
            inX = float.Parse(inputX.text);
            inY = float.Parse(inputY.text);
            inZ = float.Parse(inputZ.text);
            //create container
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(inX, inY, inZ);
            cube.transform.position = new Vector3(0 + cube.transform.localScale.z / 2, 0 + cube.transform.localScale.y / 2, 0 + cube.transform.localScale.z / 2);
            cube.name = "Container";
            //cube.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            //cube.GetComponent<MeshRenderer>().material.shader = new Shader(Shader.Find("Transparent/Diffuse"));
            conX = inX;
            conY = inY;
            conZ = inZ;
        }
        conCreated = true;
    }

    //choosing next box from queue to add to formation
    public void addToFormation()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(0, 0, 0);
        Transform chosenOne = cube.transform;
        //finding largest box in queue
        foreach (Transform child in waitQueue.transform)
        {
            if (child.localScale.x > chosenOne.localScale.x)
            {
                chosenOne = child;
            }
        }

        if (offsetX + chosenOne.localScale.x > conX)
        {
            Transform temp = chosenOne;
            //find a smaller box
            bool smallerExists = false;
            foreach (Transform child in waitQueue.transform)
            {
                if (child.localScale.x + offsetX <= conX)
                {
                    chosenOne = child;
                    smallerExists = true;
                }
            }
            //creating new row if smaller box not found
            if (!smallerExists)
            {
                chosenOne = temp;
                GameObject newRow = new GameObject();
                newRow.name = "row " + row;
                row++;
                newRow.transform.parent = rows.transform;
                currentRow = newRow;
                offsetX = 0;
                offsetY += chosenOne.localScale.y;
            }
        }

        chosenOne.position = new Vector3(offsetX + chosenOne.localScale.x / 2, offsetY + chosenOne.localScale.y / 2, offsetZ + chosenOne.localScale.z / 2);
        chosenOne.parent = currentRow.transform;
        Destroy(cube);
        queueSize--;
        offsetX += chosenOne.localScale.x;
    }


}