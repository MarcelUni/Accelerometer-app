using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.UI;

public class ListOfData //Class used for saving data to json
{
    public List<Vector3> AccelerometerData;
}

public class SaveAccelerometerData : MonoBehaviour
{
    // Accelerometer stuff
    private List<Vector3> data = new List<Vector3>();
    public float recordTime = 5f;   // How long it should record data
    private float elapsedTime = 0f;

    // JSON stuff
    private string datapath;    // Path to save data
    private string datafile;    // File to save data

    //Console stuff
    public Text Console;        // Text to show whats going on

    void Awake()
    {
        Application.targetFrameRate = 60;   //Setting the fps to match accelerometer 
        datapath = Application.persistentDataPath + "/AccelerometerData/";
        datafile = datapath + "Accelerometer_Data.json";

        Console.text = "Path for the saved data: " + datafile;    //Show the path to the user
    }

    void Start()
    {
        NewDirectory();             //Create a new directory
    }

    public void StartRecording()    //Button to start recording
    {
        Console.text += "\nRecording Started";
        StartCoroutine(RecordData());
    }

    private IEnumerator RecordData()    //Record data for a certain time
    {
        Console.text += "\nRecording for " + recordTime + " seconds";

        while (elapsedTime < recordTime)// Record data for a certain time
        {
            elapsedTime += .5f;
            WriteToList();              // Add data to list
            yield return new WaitForSeconds(.5f);
        }
        if(elapsedTime >= recordTime)
        {
            elapsedTime = 0f;
            Console.text += "\nData points:";
            foreach (Vector3 v in data) // Show the data points
            {
                Console.text += "\n" + v;
            }
            SaveData();                 //When time is up, save the data
        }
    }

    private void WriteToList()          //Write data to list
    {
        Vector3 currentData = new Vector3(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z);
        data.Add(currentData); 
    }

    private void NewDirectory()        //Create a new directory if it doesn't exist
    {
        if (!Directory.Exists(datapath))
        {
            Directory.CreateDirectory(datapath);
        }
    }

    private void SaveData()           //Save the data to a json file
    {
        Console.text += "\nRecording finished and saving Data";

        ListOfData dataToSave = new ListOfData();   //Create a in instance of the Listofdata class

        dataToSave.AccelerometerData = data;        // Add the data in the list to the instance

        string jsonString = JsonUtility.ToJson(dataToSave, true);   //Convert the instance to a json string

        using(StreamWriter stream = File.CreateText(datafile))      //Write the json string to a file
        {
            stream.WriteLine(jsonString);
        }
    }
}
