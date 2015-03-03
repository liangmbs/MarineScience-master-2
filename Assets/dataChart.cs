using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dataChart : MonoBehaviour {

    public float xScale = 0.01f;
    public float yOffset = 0f;
    public float yScale = 1.0f;
    public int dataCrop = 100;
    public float[] data;
    public Color color;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmos()
    {
        float x = gameObject.transform.position.x;
        float y = gameObject.transform.position.y;

        for (int i = 1; i < data.Length; i++)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(new Vector3(
                    x + (i - 1) * xScale, 
                    y + data[i - 1] * yScale + yOffset * yScale, 0),
                new Vector3(
                    x + i * xScale, 
                    y + data[i] * yScale + yOffset * yScale, 0));
        }
    }

    public void setData(float[] dat)
    {
        data = new float[(int) (dat.Length / dataCrop) - 1];
        for (int i = 0; i < (dat.Length / dataCrop) - 1; i++)
        {
            data[i] = dat[i * dataCrop];
        }
    }
}
