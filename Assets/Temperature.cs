using UnityEngine;
using System.Collections;

public class Temperature : MonoBehaviour {

    public float baseTemp = 20;
    public float variability = 20;
    public float cycleTime = 3;

    private float currentTime = 0;

    public float currentTemp;

	private float heaterTemp;

	public HeaterInterface heater;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		print (heater.temperature);
		heaterTemp = heater.temperature;
        currentTime += Time.deltaTime;

        float realCycle = cycleTime / (Mathf.PI * 2);

        if (currentTime / realCycle > Mathf.PI * 2)
        {
            currentTime -= Mathf.PI * 2 * realCycle;
        }
        currentTemp = baseTemp + heaterTemp + variability * Mathf.Sin(currentTime / realCycle);
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float x = currentTemp * transform.localScale.x;
        Vector3 pos = transform.position;
        Gizmos.DrawLine(pos + new Vector3(x, 0, 0), pos + new Vector3(x, transform.localScale.y, 0));
    }
}
