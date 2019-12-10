using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithm : MonoBehaviour
{

    //Prefabs of container with anchor
    public GameObject box;

    private Transform container;
    private Vector3 containerXYZ;
    private List<Vector3> waitingQueue = new List<Vector3>();
    private float xOff = 0;
    private float yOff = 0;
    private float zOff = 0;
    private float xZero = 0;
    private float zZero = 0;
    private float xTemp = 0;
    private float zTemp = 0;
    private Vector3 boxDimension;
    private bool check1 = false;
    private bool check2 = false;

    public void SetContainer(Vector3 containerXYZ)
    {
        this.containerXYZ = containerXYZ;
        //This component doesn't need for right now, maybe need for future, Dont worry about it
        this.container = GameObject.FindGameObjectWithTag("Container").GetComponent<Transform>();
    }

    public void SetBox(Vector3 boxXYZ)
    {
        //waitingQueue.Add(boxXYZ);

        /*Uncomment if it is need
        if(waitingQueue.Count > 5)
            CalculatePosition();
        */
        //CalculatePosition();
        
        boxDimension = boxXYZ;
        /*
        while(xZero + boxDimension.x <= containerXYZ.x)
        {
            xZero += boxDimension.x;
        }
        xZero = (boxDimension.x - xZero) / 2;
        while (zZero + boxDimension.z <= containerXYZ.z)
        {
            zZero += boxDimension.z;
        }
        zZero = (boxDimension.z - zZero) / 2;
        xOff = xZero;
        zOff = zZero;
        */

    }

    public void CalculatePosition()
    {
        Vector3 boxPosition = Vector3.zero;
        int waitingQueueIndex = 0;
        //This is for the algorithm to calculate the position
        //check y
        if (yOff + boxDimension.y <= containerXYZ.y)
        {
            //check x
            if (xOff + boxDimension.x <= containerXYZ.x)
            {
                //check z
                if (zOff + boxDimension.z <= containerXYZ.z)
                {
                    boxPosition.x = xOff;
                    boxPosition.y = yOff;
                    boxPosition.z = zOff;
                    GenerateBox(boxPosition);
                    if (check1)
                    {
                        zOff += boxDimension.z;
                    }
                    else {
                        xOff += boxDimension.x;
                    }
                }
                else
                {
                    //containerXYZ.x = xOff;
                    //containerXYZ.z = zOff;
                    if (!check2)
                    {
                        float temp = boxDimension.x;
                        boxDimension.x = boxDimension.z;
                        boxDimension.z = temp;
                        check2 = true;
                    }
                    else if (!check1)
                    {
                        check1 = true;
                        zOff = zZero;
                        xOff = xTemp;
                        GameObject testing3 = new GameObject("check 1; xOff: " + xOff);
                    }
                    else
                    {
                        xOff = xZero;
                        zOff = zZero;
                        yOff += boxDimension.y;
                        xTemp = 0;
                        
                        check2 = false;
                        check1 = false;
                    }
                    CalculatePosition();
                }
            }
            else
            {
                if (xOff > xTemp)
                {
                    xTemp = xOff;
                    GameObject testing4 = new GameObject("max = " +xTemp);

                }
                if (!check1)
                {
                    xOff = xZero;
                    zOff += boxDimension.z;
                }
                else
                {
                    xOff = xZero;
                    zOff = zZero;
                    yOff += boxDimension.y;
                    xTemp = 0;

                    check2 = false;
                    check1 = false;
                }
                CalculatePosition();
            }


            //GenerateBox(boxPosition);
        }
    }

    private void GenerateBox(Vector3 position)
    {
        GameObject boxCreated = box;
        boxCreated.transform.position = new Vector3(position.x, position.y, -position.z);
        //boxCreated.transform.localScale = waitingQueue[waitingQueueIndex];
        boxCreated.transform.localScale = boxDimension;
        GameObject.Instantiate(boxCreated, container, true);
    }
}
