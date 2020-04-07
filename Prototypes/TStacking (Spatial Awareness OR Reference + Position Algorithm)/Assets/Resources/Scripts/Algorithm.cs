using System.Collections.Generic;
using UnityEngine;
using System;

public class Algorithm : MonoBehaviour
{
    //Public Components
    public GameObject box;      //Prefabs of container with anchor
    public Material matCurrentBox;
    public Material matLastBox;

    //Private Components
    private Transform containerPlane;
    private Transform lastBox;
    private Transform currentBox;

    private Vector3 containerXYZ;
    private Vector3 boxDimension;
    private List<Vector3> waitingQueue = new List<Vector3>();
	private int queueCount = 0; //only used for now

    //***************** For Undo fuctionality ************************
    private List<Transform> allBoxQueue = new List<Transform>();
    private List<List<float>> allBoxData = new List<List<float>>();
    private List<List<bool>> allCheckData = new List<List<bool>>();
    private List<Vector3> allContainerXYZ = new List<Vector3>();
    private List<Vector3> allBoxDimension = new List<Vector3>();
    //*****************************************************************

	//variables for same-size box algorithm
    private float xOff = 0, yOff = 0, zOff = 0; //coordinates for the next box
    private float xZero = 0, zZero = 0; //right now only holds value for 0, may change later on
    private float xMax = 0, zMax = 0; //used for keeping track of how far boxes can be stacked for x and z
	private float xTemp = 0, zTemp = 0; //used to keep track of coordinates

	//checks for when calculating position of same-size boxes
	private bool regularstacking = false; //determine whether or not T-stacking is possible
    private bool check1 = false;
    private bool check2 = false;
    private bool check3 = false;
    private bool check4 = false;

    private bool findContainerPlane = false;
    private bool isFirstBox = true;

	//for testing program
	private bool testingmode1 = false; //same-size
	private bool testingmode2 = true; //random-size

	//For random-size box algorithm
	Pallet pallet; //for random-size boxes

	//This is the unit for converting 3d array cells into unity units
	//Every unity unit is a meter in the real world
	//Every 3d array cell = 1/conversionNum in unity
	//conversion is currently set at 100, so every 3d array cell is a 0.01 in unity or 1 cm in the real world
	private float conversionNum = 100f; 

    public void SetContainerInfo(Vector3 containerXYZ)
    {
        this.containerXYZ = containerXYZ;
		pallet = new Pallet ((int)(containerXYZ.x*conversionNum), (int)(containerXYZ.y*conversionNum), (int)(containerXYZ.z*conversionNum));
		if (testingmode2) {
			pallet = new Pallet (300, 300, 300);
		}
        //this.containerPlane = GameObject.FindGameObjectWithTag("ContainerPlane").GetComponent<Transform>();
    }

	//add box to waiting queue
	public void AddBox(Vector3 boxXYZ){
		waitingQueue.Add(boxXYZ);
	}

    public void SetBoxInfo(Vector3 boxXYZ)
    {
        if (!findContainerPlane)
        {
            containerPlane = GameObject.FindGameObjectWithTag("ContainerPlane").GetComponent<Transform>();
            findContainerPlane = true;
        }

		//set box size
		if (testingmode1) {
			boxDimension = new Vector3 (0.3f, 0.3f, 0.3f);
		} else {
			boxDimension = boxXYZ;
		}

		//determine whether t-stacking is possible for same-size algorithm
		if (boxDimension.x*2 == boxDimension.z || boxDimension.z*2 == boxDimension.x) {
			regularstacking = false;
		} else {
			regularstacking = true;
		}

		//generate 50 random boxes that can be used to test random-size algorithm
		if (testingmode2) {
			System.Random rnd = new System.Random (); //random numbers for testing
			Vector3 newItem;
			int numTests = 50;
			for (int i = 0; i < numTests; i++) {
				newItem.x = (float)rnd.NextDouble()+0.3f;
				newItem.y = (float)rnd.NextDouble()+0.3f;
				newItem.z = (float)rnd.NextDouble()+0.3f;
				waitingQueue.Add (newItem);
			}
		}
    }

	//This is for the algorithm to calculate the position
	public void CalculatePosition()
	{
		Vector3 boxPosition = Vector3.zero;

		//0.0000001f are added to the container size when checking because sometimes 0.01f <= 0.01f may not return true
		//check y
		if (yOff + boxDimension.y <= containerXYZ.y + 0.0000001f) {

			//check x
			if (xOff + boxDimension.x <= containerXYZ.x + 0.0000001f) {
				
				//check z
				if (zOff + boxDimension.z <= containerXYZ.z + 0.0000001f) {

					check3 = true;
					//generate box
					boxPosition.x = xOff;
					boxPosition.y = yOff;
					boxPosition.z = zOff;
					GenerateBox (boxPosition);
					if (check1) { //this is for trying to see if boxes fit into side of pallet after flipping boxes
						//increment z
						zOff += boxDimension.z;
						//keeping track of how far boxes can be stacked for z coordinate
						if (zOff > zMax) {
							zMax = zOff;
						}
						//also keeping track of how far box can be stacked for x coordinate
						if (xOff + boxDimension.x > xMax) {
							xMax = xOff + boxDimension.x;
						}
					} else {
						//increment x
						xOff += boxDimension.x;
						//keep track of how far boxes have been stacked for x coordinate
						if (xOff > xMax) {
							xMax = xOff;
						}
					}
						

				} else { //z does not fit
					if (!check2) { //flip boxes x and z and try again
						if (!regularstacking) {
							float temp = boxDimension.x;
							boxDimension.x = boxDimension.z;
							boxDimension.z = temp;
						}
						check2 = true;
					} else if (!check1) {
						//checking size of pallet(toward x)
						check1 = true;
						zOff = zZero;
						xOff = xTemp;
					} else {
						//change container sizes to new max sizes
						containerXYZ.x = xMax;
						containerXYZ.z = zMax;
						xMax = 0;
						zMax = 0;


						xOff = xZero;
						zOff = zZero;
						yOff += boxDimension.y; //start new row (y)
						xTemp = 0;
						//reset all checks
						check2 = false;
						check1 = false;
						check3 = false;
						check4 = false;
					}
					CalculatePosition ();
				}
			} else { //x does not fit
				
				if (xOff > xTemp) { //keep track of how far boxes could be stacked for x value
					xTemp = xOff;
				}

				if (!check3) { 
					if (!regularstacking) {
						//flip box x and z
						float temp = boxDimension.x;
						boxDimension.x = boxDimension.z;
						boxDimension.z = temp;
					}
					check3 = true;
				} else if (!check1 && !check4) {
					xOff = xZero;
					if (zOff + boxDimension.z > containerXYZ.z + 0.0000001f) { 
						check4 = true; //boxes can no longer be stacked by increasing the z coordinate
					} else {
						//start new row of boxes (z)
						zOff += boxDimension.z;
						//keep track of how far boxes could be stacked for z dimension
						if (zOff > zMax) {
							zMax = zOff;
						}
					}
				} else {
					containerXYZ.x = xMax;
					containerXYZ.z = zMax;
					xMax = 0;
					zMax = 0;
					xOff = xZero;
					zOff = zZero;
					yOff += boxDimension.y; //start new row (y)
					xTemp = 0;
					//reset all checks
					check2 = false;
					check1 = false;
					check3 = false;
					check4 = false;
				}
				CalculatePosition ();
			}
		} else { //y does not fit
			Debug.Log("Not enough space");
		}
	}

	public void CalculatePosition2(){
		//get next box from waiting queue
		Vector3 bDimension = waitingQueue[queueCount];
		//create box and convert size to be used in 3d array
		Box box1 = new Box ((int)(bDimension.z*conversionNum), (int)(bDimension.x*conversionNum), (int)(bDimension.y*conversionNum));
		Debug.Log("Box in cm " + box1.getWidth() + " " + box1.getHeight()+ " " + box1.getLength());
		//Call the placeBox function to calculate position in 3d array
		Box position = pallet.placeBox (box1);
		Debug.Log("Pos in cm " + position.getWidth() + " " + position.getHeight()+ " " + position.getLength());

		//Check to make sure position was calculated and call generate box function
		if (position.getWidth () > -1) {
			//Convert coordinates from 3d array into unity coordinates
			Vector3 bPosition = new Vector3 ((float)(position.getWidth ()) / conversionNum, (float)(position.getHeight ()) / conversionNum, (float)(position.getLength ()) / conversionNum);
		
			//increment queue and create box
			queueCount++;
			GenerateBox (bPosition, bDimension);
		} else {
			Debug.Log ("Unable to place box");
		}
	}

	//generate box function for same-size box positions
    private void GenerateBox(Vector3 position)
    {
        StoreBoxData();

        GameObject boxCreated = box;
        boxCreated.transform.localPosition = new Vector3(position.x, position.y, -position.z);
        //boxCreated.transform.localScale = waitingQueue[waitingQueueIndex];
        boxCreated.transform.localScale = boxDimension;

        if (isFirstBox)
        {
            currentBox = GameObject.Instantiate(boxCreated, containerPlane).transform;

            allBoxQueue.Add(currentBox);

            isFirstBox = false;
        }
        else
        {
            try
            {
                lastBox = currentBox;
                currentBox = GameObject.Instantiate(boxCreated, containerPlane).transform;

                allBoxQueue.Add(currentBox);

                lastBox.GetChild(0).GetComponent<MeshRenderer>().material = matLastBox;
                lastBox.GetChild(0).GetComponent<Animation>().enabled = false;
                lastBox.GetChild(0).transform.localPosition = new Vector3(0.5f, 0.5f, -0.5f);
            }
            catch
            {
            }
        }
    }

	//generate function for random-size box positions
	private void GenerateBox(Vector3 position, Vector3 dimension)
	{
		StoreBoxData();

		GameObject boxCreated = box;
		boxCreated.transform.localPosition = new Vector3(position.x, position.y, -position.z);
		boxCreated.transform.localScale = dimension;

		if (isFirstBox)
		{
			currentBox = GameObject.Instantiate(boxCreated, containerPlane).transform;

			allBoxQueue.Add(currentBox);

			isFirstBox = false;
		}
		else
		{
			try
			{
				lastBox = currentBox;
				currentBox = GameObject.Instantiate(boxCreated, containerPlane).transform;

				allBoxQueue.Add(currentBox);

				lastBox.GetChild(0).GetComponent<MeshRenderer>().material = matLastBox;
				lastBox.GetChild(0).GetComponent<Animation>().enabled = false;
				lastBox.GetChild(0).transform.localPosition = new Vector3(0.5f, 0.5f, -0.5f);
			}
			catch
			{
			}
		}
	}

    public void Finish()
    {
        if (currentBox != null)
        {
            currentBox.GetChild(0).GetComponent<MeshRenderer>().material = matLastBox;
            currentBox.GetChild(0).GetComponent<Animation>().enabled = false;
            currentBox.GetChild(0).transform.localPosition = new Vector3(0.5f, 0.5f, -0.5f);

            //Clear the data(Reset)
            findContainerPlane = false;
            isFirstBox = true;

            xOff = 0; yOff = 0; zOff = 0;
            xZero = 0; zZero = 0;
            xTemp = 0; zTemp = 0;
            xMax = 0; zMax = 0;

            check2 = false;
            check1 = false;
            check3 = false;
            check4 = false;

            allBoxQueue.Clear();
            allBoxData.Clear();
        }
    }

    public void Undo()
    {
        if (allBoxQueue.Count > 0)
        {
            //Undo box gameobject
            Transform temp = allBoxQueue[allBoxQueue.Count - 1];
            allBoxQueue.RemoveAt(allBoxQueue.Count - 1); 
            Destroy(temp.gameObject);
            //Try activate the last box animation
            try
            {
                Transform last = allBoxQueue[allBoxQueue.Count - 1];
                currentBox = last;
                last.GetChild(0).GetComponent<MeshRenderer>().material = matCurrentBox;
                last.GetChild(0).GetComponent<Animation>().enabled = true;
            }
            catch { }

            //Undo box data
            if (allBoxQueue.Count == 0)
            {
                isFirstBox = true;

                xOff = 0; yOff = 0; zOff = 0;
                xTemp = 0; zTemp = 0;
                xMax = 0; zMax = 0;

                check2 = false;
                check1 = false;
                check3 = false;
                check4 = false;
            }
            else
            {
                List<float> lastBoxData = allBoxData[allBoxData.Count - 1];
                List<bool> lastCheckData = allCheckData[allCheckData.Count - 1];

                //Undo box data
                xOff = lastBoxData[0];
                yOff = lastBoxData[1];
                zOff = lastBoxData[2];

                xZero = lastBoxData[3];
                zZero = lastBoxData[4];

                xTemp = lastBoxData[5];
                zTemp = lastBoxData[6];

                xMax = lastBoxData[7];
                zMax = lastBoxData[8];

                //Undo check data
                check1 = lastCheckData[0];
                check2 = lastCheckData[1];
                check3 = lastCheckData[2];
                check4 = lastCheckData[3];

                //Undo container data
                containerXYZ = allContainerXYZ[allContainerXYZ.Count - 1];
                allContainerXYZ.RemoveAt(allContainerXYZ.Count - 1);

                //Undo box dimension data
                boxDimension = allBoxDimension[allBoxDimension.Count - 2];
                Debug.Log(boxDimension);
                allBoxDimension.RemoveAt(allBoxData.Count - 1);
            }
            allCheckData.RemoveAt(allCheckData.Count - 1);
            allBoxData.RemoveAt(allBoxData.Count - 1);
        }
    }

    public void StoreBoxData()
    {
        //Store all  box data
        List<float> boxData = new List<float>();
        boxData.Add(xOff);
        boxData.Add(yOff);
        boxData.Add(zOff);

        boxData.Add(xZero);
        boxData.Add(zZero);

        boxData.Add(xTemp);
        boxData.Add(zTemp);

        boxData.Add(xMax);
        boxData.Add(zMax);

        allBoxData.Add(boxData);

        //Store all check data
        List<bool> checkData = new List<bool>();
        checkData.Add(check1);
        checkData.Add(check2);
        checkData.Add(check3);
        checkData.Add(check4);

        allCheckData.Add(checkData);

        //Store container data
        allContainerXYZ.Add(containerXYZ);

        //Store all box dimesion data
        allBoxDimension.Add(boxDimension);
    }

	//classes for multidimension stacking
	//class for boxes
	public class Box
	{
		private int height;
		private int length;
		private int width;

		public Box(int l, int w, int h)
		{
			height = h;
			length = l;
			width = w;
		}

		public int getHeight()
		{
			return height;
		}

		public int getLength()
		{
			return length;
		}

		public int getWidth()
		{
			return width;
		}
	}

	//class for pallet
	public class Pallet
	{
		private int maxLength;
		private int maxWidth;
		private int maxHeight;

		/*
        *   For the storage array:
        *       0 means empty space
        *       1 means filled space
        *       2 means edge space
        */

		private int[,,] storage;

		public Pallet(int l, int w, int h)
		{
			maxHeight = h;
			maxLength = l;
			maxWidth = w;
			storage = new int[maxLength, maxWidth, maxHeight];
			initStorage();
		}

		private void initStorage()
		{
			for(int i=0; i<maxHeight; i++)
			{
				for(int j=0; j<maxLength; j++)
				{
					for(int k=0; k<maxWidth; k++)
					{
						storage[j, k, i] = 0;
					}
				}
			}
		}

		public Box placeBox(Box newBox)
		{
			Box position = new Box (-1, -1, -1);
			for(int k=0; k<maxHeight; k++)
			{
				for(int i=0; i < maxLength; i++)
				{
					for(int j=0; j<maxWidth; j++)
					{
						if(storage[i,j,k] == 0)
						{
							if(searchSpace(newBox, i, j, k))
							{
								//fill area
								fillSpace(i, j, k, i + newBox.getLength(), j + newBox.getWidth(), k + newBox.getHeight());

								Debug.Log ("TEST" + i + " " + j + " " + k);
								Debug.Log ("TEST2" + newBox.getHeight() + " " + newBox.getLength() + " " + newBox.getWidth());
								position = new Box (i, j, k);

								return position;
							}
							else
							{
								//for now skip the space, in the future try differnet orientations
								j += newBox.getWidth();
							}
						}
					}
				}
			}



			return position;
		}

		private bool searchSpace(Box newBox, int l, int w, int h)
		{
			int lengthSearch = l + newBox.getLength();
			int widthSearch = w + newBox.getWidth();
			int heightSearch = h + newBox.getHeight();

			if(widthSearch > maxWidth || lengthSearch > maxLength || heightSearch > maxHeight)
			{
				return false;
			}

			for(int k=h; k<heightSearch; k++)
			{
				for (int i=l; i<lengthSearch; i++)
				{
					for(int j=w; j<widthSearch; j++)
					{
						if(storage[i,j,k] != 0)
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		private void fillSpace(int startX, int startY, int startZ, int endX, int endY, int endZ)
		{
			for(int k=startZ; k<endZ; k++)
			{
				for(int i=startX; i<endX; i++)
				{
					for(int j=startY; j<endY; j++)
					{
						storage[i, j, k] = 1;
					}
				}
			}

		}

		//temp print function
		public void print()
		{
			String outPut = "";
			for(int i=0; i<maxLength; i++)
			{
				outPut = "";
				for(int j=0; j<maxWidth; j++)
				{
					outPut += storage[i, j, 0] + " ";
				}
			}
		}
	}

}
