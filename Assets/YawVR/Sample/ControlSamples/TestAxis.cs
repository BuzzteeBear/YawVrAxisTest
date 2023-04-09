using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YawVR;
using System;
using System.IO;
using TMPro;

public class TestAxis : MonoBehaviour
{
    protected int axis;
    protected StreamWriter fileOut;
    protected YawController yawController;
    protected IEnumerator coRoutine;
    protected float totalTime;
    protected float timeIncrement;
    protected float angleIncrement;
    protected float maxAngle;
    protected float currentAngle;
    protected int stepCount;
    bool running = false;

    void Start()
    {
        axis = 0;
        yawController = YawController.Instance();
        timeIncrement = 0.01f;
        totalTime = 120f;
        maxAngle = 15f;
        currentAngle = 0f;
        stepCount = 0;
    }

    public void TestButtonPressed()
    {
        if (running)
        {
            StopAllCoroutines();
            //Vector3 rotation = new Vector3(0f, 0f, 0f);
            //yawController.TrackerObject.SetRotation(rotation);
            try
            {
                yawController.StopDevice(true);
            }
            catch { }
            fileOut.Close();
            running = false;
        }
        else
        {
            DateTime time = DateTime.Now;
            string timestamp = time.ToString("yyyy-MM-dd-HH-mm-ss");
            string axisname = axis == 0 ? "Pitch" : (axis == 1 ? "Yaw" : "Roll");
            string path = @"Test_" + timestamp + "_" + axisname + ".csv";
            if (File.Exists(path))
            {
                fileOut = File.CreateText(path);
            }
            else
            {
                fileOut = File.AppendText(path);
            }
                  
            coRoutine = AxisTestRoutine();
            StartCoroutine(coRoutine);
            
        }
    }

    IEnumerator AxisTestRoutine()
    {
        running = true;
        try
        {
            yawController.StartDevice();
        }
        catch { }
        stepCount = 0;
        currentAngle = 0;
        angleIncrement = (4f * maxAngle * timeIncrement)/ totalTime;
        float stepsPerQuadrant = maxAngle / angleIncrement;
  
        while (stepCount <= 4f * stepsPerQuadrant + 1f)
        {
            yield return new WaitForSeconds(timeIncrement);

            if (stepCount < stepsPerQuadrant)
            {
                currentAngle +=  angleIncrement;

            }
            else if (stepCount <= 3f * stepsPerQuadrant)
            {
                currentAngle -= angleIncrement;
            }
            else
            {
                currentAngle += angleIncrement;
            }
            Vector3 rotation = new Vector3(0f, 0f, 0f);
            rotation[axis] = currentAngle;
            yawController.TrackerObject.SetRotation(rotation);
            fileOut.WriteLine((stepCount * timeIncrement).ToString() + "," + currentAngle.ToString() + "," + (axis == 0 ? yawController.Device.ActualPosition.pitch : (axis == 1 ? yawController.Device.ActualPosition.yaw : yawController.Device.ActualPosition.roll)).ToString());
            stepCount++;
        }
        fileOut.Close();
        yawController.StopDevice(true);
        running = false;
    }

    public void setAxis(int index)
    {
        axis = index;
    }
    public void setStepsPerSecond(string steps)
    {
        float stepsPerSecond;
        float.TryParse(steps, out stepsPerSecond); 
        
        if (stepsPerSecond > 200f)
        {
            stepsPerSecond = 200f;
        }
        if (stepsPerSecond <= 0.1f)
        {
            stepsPerSecond = 0.1f;
        }
        timeIncrement = 1.0f / stepsPerSecond;
    }

    public void setTotalTime(string time)
    {
        float.TryParse(time, out totalTime);
        if (totalTime < 1f)
        {
            totalTime = 1f;
        }

    }

    public void setMaxAngle(string angle)
    {
        float.TryParse(angle, out maxAngle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}