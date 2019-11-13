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
    private List<Vector3> waitingQueue = new List<Vector3>();

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
        CalculatePosition();
    }

    public void CalculatePosition()
    {
        Vector3 boxPosition = Vector3.zero;
        int waitingQueueIndex = 0;
        //This is for the algorithm to calculate the position


        GenerateBox(boxPosition, waitingQueueIndex);
    }

    private void GenerateBox(Vector3 position, int waitingQueueIndex)
    {
        GameObject boxCreated = box;
        boxCreated.transform.localPosition = new Vector3(position.x, position.y, -position.z);
        boxCreated.transform.localScale = waitingQueue[waitingQueueIndex];
        GameObject.Instantiate(boxCreated, containerPlane);

        //*******************TEST****************
        waitingQueue.RemoveAt(0);
        //****************************************
    }
}
