using UnityEngine;
using System.Collections;
using UnityEngine .UI ;

public class HeaterInterface : MonoBehaviour {
	
	public Slider Heater;
	public Slider BaseLine;
	public float Amplitude;
	public float temperature;
	public float temp;
	public float Angle;
	public float i = 0.0f;
	public float frequency = 1000.0f;
	public float baseline;
	
	
	// Use this for initialization
	void Start () {
	}
	
	float Gettingtemperature(float amplitude, float angle, float baseline){
		temp = amplitude * Mathf.Sin (angle);
		temp = temp + baseline;
		return temp;
	}
	
	
	// Update is called once per frame
	void Update () {
		Amplitude = Heater.value;
		baseline = BaseLine.value;
		Angle = (2 * Mathf.PI * i * frequency)/8000;	
		temperature = Gettingtemperature (Amplitude, Angle, baseline);
		i = i + 0.3f;
		print (temperature);
		
	}
}
