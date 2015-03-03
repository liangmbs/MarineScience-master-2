using UnityEngine;
using System.Collections;

public class ThermalCurve : MonoBehaviour {

    static float MAX = 273 + 40;
    static float MIN = 273 + 0;

    public float interval = 0.1f;
    public float[] data;
    public dataChart chart;

    public float optimalTemp = 295.15f;
    public float arrhenBreadth = 4258;
    public float arrhenLower = 7457;
    public float arrhenUpper = 19664;
    public float lowerBound = 286;
    public float upperBound = 298;

	// Use this for initialization
	void Start () {

	}

    public float getCurve(float temp)
    {
        return Mathf.Exp(arrhenBreadth / optimalTemp - arrhenBreadth / temp) *
                (1 + Mathf.Exp(arrhenLower / optimalTemp - arrhenLower / lowerBound) +
                    Mathf.Exp(arrhenUpper / upperBound - arrhenUpper / optimalTemp)) /
                (1 + Mathf.Exp(arrhenLower / temp - arrhenLower / lowerBound) +
                    Mathf.Exp(arrhenUpper / upperBound - arrhenUpper / temp));
    }

    void updateChart()
    {
        int length = 2 + (int)((MAX - MIN) * (1 / interval));
        data = new float[length];
        for (int i = 0; i < length; i++)
        {
            float temp = MIN + i * interval;
            data[i] = getCurve(temp);
        }

        if (chart != null)
        {
            chart.data = data;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnDrawGizmos()
    {
        updateChart();
    }
}
