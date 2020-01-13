using System.Collections.Generic;
using UnityEngine;
public class Algorithm : MonoBehaviour
{
    //Public Components
    public GameObject box;      //Prefabs of container with anchor

    //Private Components
    private Transform containerPlane;

    private Vector3 containerXYZ;
    private Vector3 boxDimension;
    private List<Vector3> waitingQueue = new List<Vector3>();

    private float xOff = 0, yOff = 0, zOff = 0;
    private float xZero = 0, zZero = 0;
    private float xTemp = 0, zTemp = 0;

    private bool check1 = false;
    private bool check2 = false;
    private bool findContainerPlane = false;

    public void SetContainerInfo(Vector3 containerXYZ)
    {
        this.containerXYZ = containerXYZ;
        //this.containerPlane = GameObject.FindGameObjectWithTag("ContainerPlane").GetComponent<Transform>();
    }

    public void SetBoxInfo(Vector3 boxXYZ)
    {
        if (!findContainerPlane)
        {
            containerPlane = GameObject.FindGameObjectWithTag("ContainerPlane").GetComponent<Transform>();
            findContainerPlane = true;
        }

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
                    else
                    {
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
                        //GameObject testing3 = new GameObject("check 1; xOff: " + xOff);
                        Debug.Log("check 1; xOff: " + xOff);
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
                    //GameObject testing4 = new GameObject("max = " +xTemp);
                    Debug.Log("max = " + xTemp);
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
        boxCreated.transform.localPosition = new Vector3(position.x, position.y, -position.z);
        //boxCreated.transform.localScale = waitingQueue[waitingQueueIndex];
        boxCreated.transform.localScale = boxDimension;
        GameObject.Instantiate(boxCreated, containerPlane);
    }
}