using UnityEngine;
using UnityEngine.UI;
using System;
using MySql.Data.MySqlClient;
using System.Collections;

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

    public void readFromServer()
    {
        MySql.Data.MySqlClient.MySqlConnection connection;
        string server = "remotemysql.com";
        string database = "CEWfsvQgZu";
        string uid = "CEWfsvQgZu";
        string password = "UdelU1N7ef";
        string connectionString;
        connectionString = "SERVER=" + server + ";" + "DATABASE=" +
        database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
        connection = new MySqlConnection(connectionString);

        Queue myBoxQueue = new Queue();

        try
        {
            connection.ConnectionString = connectionString;
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM box_info", connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            Box myBox;
            while (reader.Read())
            {
                string test1 = reader.GetString("id");
                string test2 = reader.GetString("length");
                string test3 = reader.GetString("width");
                string test4 = reader.GetString("height");
                string test5 = reader.GetString("weight");
                myBox = new Box(int.Parse(test1), float.Parse(test2), float.Parse(test3), float.Parse(test4), float.Parse(test5));
                myBoxQueue.Enqueue(myBox);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error occured. Please try again later.");
        }
        connection.Close();

        Debug.Log(myBoxQueue.Count);
        for(int i = 0; i < 3; i++)
        {
            Debug.Log(i);
            Box box = (Box)myBoxQueue.Dequeue();
            Debug.Log("Length: " + box.getLength() + " Width: " + box.getWidth() + " Height:" + box.getHeight());
        }
    }
}

class Box
{
    private int id;
    private float length;
    private float width;
    private float height;
    private float weight;

    public Box()
    {
        id = 0;
        length = 0;
        width = 0;
        height = 0;
        weight = 0;
    }

    public Box(int i, float len, float wid, float hei, float wei)
    {
        id = i;
        length = len;
        width = wid;
        height = hei;
        weight = wei;
    }

    public int getID()
    {
        return id;
    }

    public float getLength()
    {
        return length;
    }

    public float getWidth()
    {
        return width;
    }

    public float getHeight()
    {
        return height;
    }

    public float getWeight()
    {
        return weight;
    }
}
