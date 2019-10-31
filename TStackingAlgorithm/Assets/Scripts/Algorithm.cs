using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithm : MonoBehaviour {

    //Prefabs of container with anchor
    public GameObject box;

    private Transform container;
    private Vector3 containerXYZ;
    private List<Vector3> waitingQueue = new List<Vector3>();

    public void SetContainer(Vector3 containerXYZ)
    {
        this.containerXYZ = containerXYZ;
        //This component doesn't need for right now, maybe need for future, Dont worry about it
        this.container = GameObject.FindGameObjectWithTag("Container").GetComponent<Transform>();
    }

    public void SetBox(Vector3 boxXYZ)
    {
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
        boxCreated.transform.position = new Vector3(position.x, position.y, -position.z);
        boxCreated.transform.localScale = waitingQueue[waitingQueueIndex];
        GameObject.Instantiate(boxCreated, container, true);
    }
}
