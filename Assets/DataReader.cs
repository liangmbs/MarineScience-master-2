using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;


public class DataReader : MonoBehaviour
{

    public TextAsset foodSourceText;
    public TextAsset waterSourceText;
    protected StringReader reader = null;
    protected string text1 = " "; // To Read the Water Temperature
    protected string text2 = " "; // To Read the Food Concentration
    char[] delimiterchar = { ' ', '/', '\t' };
    string[] words1; // temptorary variable to load the data
    string[] words2; // temptorary variable to load the data
    public List<float> WaterTemplist = new List<float>();
    public List<float> FoodConcentlist = new List<float>();
    public dataChart waterOutput;

    void Start()
    {
        inputwatertemperature();
        waterOutput.data = WaterTemplist.ToArray();
        //inputfoodconcentration();
    }

    public void inputwatertemperature()
    {
        //open file
        reader = new StringReader(waterSourceText.text);
        int i = 0;
        text1 = "test";
        while (text1 != null)
        {
            //skip the first line
            if (i > 1)
            {
                words1 = text1.Split(delimiterchar);
                string watertemp = words1[7];   //obtain the data from the txt
                float WaterD = float.Parse(watertemp);  // convert to double
                WaterTemplist.Add(WaterD); // add the data to the list
            }
            text1 = reader.ReadLine();
            i++;
        }
    }

    void inputfoodconcentration()
    {
        reader = new StringReader(foodSourceText.text);
        int i = -1;
        do
        {
            if (i < 0)
            {
                reader.ReadLine();
                i++;
            }
            text2 = reader.ReadLine();
            words2 = text2.Split(delimiterchar);
            string foodtemp = words2[7];
            float FoodC = float.Parse(foodtemp);
            FoodConcentlist.Add(FoodC);
        } while (reader.Peek() > -1);
    }
}
