using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithm : MonoBehaviour
{
    //Prefabs of container with anchor
    public GameObject box;

    private Transform containerPlane;
    private Vector3 containerXYZ;
    private bool findContainerPlane = false;
    private List<Vector3> waitingQueue = new List<Vector3>();   //Volume Waiting Queue

    //******************** Hard Code Testing *********************
    private List<Vector3> waitingQueuePosition = new List<Vector3>();
    private int waitingQueuePositionIndex = 0;

    private void Start()
    {
        waitingQueuePosition.Add(new Vector3(0.0f, 0.0f, 0.0f));
        waitingQueuePosition.Add(new Vector3(0.4f, 0.0f, 0.0f));
        waitingQueuePosition.Add(new Vector3(0.8f, 0.0f, 0.0f));
        waitingQueuePosition.Add(new Vector3(0.2f, 0.3f, 0.0f));
        waitingQueuePosition.Add(new Vector3(0.6f, 0.3f, 0.0f));
    }

    public void SetContainerInfo(Vector3 containerXYZ)
    {
        // x = length, y = height, z = width
        this.containerXYZ = containerXYZ;  
    }

    public void SetBoxInfo(Vector3 boxXYZ)
    {
        if(!findContainerPlane)
            this.containerPlane = GameObject.FindGameObjectWithTag("ContainerPlane").GetComponent<Transform>();

        waitingQueue.Add(boxXYZ);

        /*Uncomment if it is need
        if(waitingQueue.Count > 5)
            CalculatePosition();
        */
        CalculateBoxPosition();
    }

    private void CalculateBoxPosition()
    {
        Vector3 boxPosition = waitingQueuePosition[waitingQueuePositionIndex];

        int waitingQueueIndex = 0;
        //This is for the algorithm to calculate the position


        InstantiateBox(boxPosition, waitingQueueIndex);

        if (waitingQueuePositionIndex < 4)
            waitingQueuePositionIndex++;
    }

    private void InstantiateBox(Vector3 boxPosition, int waitingQueueIndex)
    {
        GameObject boxCreated = box;
        boxCreated.transform.localPosition = new Vector3(boxPosition.x, boxPosition.y, -boxPosition.z);
        boxCreated.transform.localScale = waitingQueue[waitingQueueIndex];
        GameObject.Instantiate(boxCreated, containerPlane);

        //*******************TEST****************
        waitingQueue.RemoveAt(0);
        //****************************************
    }
}
