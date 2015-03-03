using UnityEngine;
using System.Collections;

public class debModel : MonoBehaviour {

    public int dataLength = 100;
    float[] waterTempData;
    float[] foodData;

    //temperature parameters
    float rTemp = 295.15f; //K, Reference temperature / T_opt ;
    float arhT = 4258; //K, Arrhenius temperature ;
    float arhLoT = 7457; //K, Arrhenius temperature for lower boundary
    float arrUpT = 19664; //K, Arrhenius temperature for upper boundary
    float loTBnd = 286; //Lower temperature boundary
    float upTBnd = 298; //Upper temperature boundary

    //feeding parameters
    float mSpSrRt = 128f / 24f; // l/d.cm^2, {F_m} max spec searching rate
    float dgstEncy = 0.8f; // digestion efficiency of food to reserve
    float mSrfAsmRt = 28.4982f / 24f; // J/cm^2/d, maximum surface-specific assimilation rate = z * p_M/ kap

    //creature parameters
    public float enCnd = 0.01431f / 24f; // cm / day
    public float somFr = 0.9885f; //allocation fraction to soma = growth + somatic maintenance
    public float repEncy = 0.9165f;
    public float volSmMnt = 15.15f / 24f; // J/d.cm^3 volume-specific somatic maintenance
    public float srfSmMnt = 0;// J/d.cm^2 surface-specific som maintenance
    public float matMntRtCo = 0.002f / 24f; // 1/d maturity maint rate coefficient
    public float spStrCst = 3140f; // J/cm^3 spec cost for structure  (whatever that means...)

    //life stages
    public float brtMat = 0.000002125f; // J maturity at birth
    public float pubMat = 74.55f; // J maturity at puberty

    //for computing observable quantities
    public float shpCo = 0.2417f; // shape coefficient to convert vol-length into physical length
    public float strSpDen = 0.12f; // g/cm^3, specific density of structure (dry weight)
    public float strChmPot = 500000; // J/mol chemical potential of structure
    public float resChmPot = 550000; // J/mol chemical potential of reserve
    public float strMolMass = 23.9f; // g/mol molecular weight of structure
    public float resMolMass = 23.9f; // g/mol molecular weight of reserve
    public float wetDryCo = 12; // wet/dry weight coefficient

    //starting values
    public float initReserveEnergy = 0.0002911f; //J, initial reserve energy
    public float initStructuralVolume = 0.000000082347f; //cm^3, initial structural volume
    public float initCumulativeMaturationEnergy = 0; //J, initial cum. energy invested into maturation
    public float initReproductionBuffer = 0; //J, reproduction buffer

    public dataChart waterGraph;
    public dataChart foodGraph;

    public dataChart reproductionGraph;
    public dataChart structuralVolumeGraph;
    public dataChart wetWeightGraph;

    void randomData()
    {
        waterTempData = new float[dataLength];
        float drift = 0;
        for (int i = 0; i < dataLength; i++)
        {
            drift += Random.Range(-3f, 3f);
            drift = drift * .6f;
            waterTempData[i] = 13.5f + Mathf.Sin(i / 2000f) * 4.5f + Mathf.Abs(drift);
            //add 273 to change from celcius into kelvin
            waterTempData[i] = waterTempData[i] + 273;
        }


        foodData = new float[dataLength];
        for (int i = 0; i < dataLength; i++)
        {
            drift += Random.Range(-2f, 2f);
            drift = drift * .8f;
            foodData[i] = 14f + Mathf.Sin(i / 500f) * 14 + Mathf.Abs(drift);
        }
    }

    void computeResults()
    {
        //initial values
        float[][] stateData = new float[foodData.Length][];
        stateData[0] = new float[] 
            {initReserveEnergy, 
            initStructuralVolume, 
            initCumulativeMaturationEnergy, 
            initReproductionBuffer};

        float[][] stateDifferential = new float[foodData.Length][];
        stateDifferential[0] = new float[] { 0, 0, 0, 0 };

        float K = mSrfAsmRt / (dgstEncy * mSpSrRt);

        float[] foodDnsy = new float[foodData.Length];

        //stepwise calculations
        for (int i = 1; i < foodData.Length; i++)
        {
            float c_T = Mathf.Exp(arhT / rTemp - arhT / waterTempData[i]) *
                (1 + Mathf.Exp(arhLoT / rTemp - arhLoT / loTBnd) +
                    Mathf.Exp(arrUpT / upTBnd - arrUpT / rTemp)) /
                (1 + Mathf.Exp(arhLoT / waterTempData[i] - arhLoT / loTBnd) +
                    Mathf.Exp(arrUpT / upTBnd - arrUpT / waterTempData[i]));

            foodDnsy[i] = foodData[i] / (foodData[i] + K);

            float p_Amt = c_T * mSrfAsmRt;
            float v_T = c_T * enCnd;
            float p_MT = c_T * volSmMnt;
            float p_TT = c_T * srfSmMnt;
            float k_JT = c_T * matMntRtCo;
            float p_XmT = p_Amt / dgstEncy;

            stateData[i] = new float[4];
            for (int j = 0; j < 4; j++)
            {
                stateData[i][j] = stateData[i - 1][j] + stateDifferential[i - 1][j];
            }

            if (i % 8760 == 0 && stateData[i - 1][2] >= pubMat)
            {
                    stateData[i][3] = 0;
            }

            float E = stateData[i][0];
            float V = stateData[i][1];
            float H = stateData[i][2];
            float R = stateData[i][3];

            //fluxes
            float pX = 0;
            if (H < brtMat)
            {
                pX = 0;
            }
            else
            {
                pX = foodDnsy[i] * p_XmT * Mathf.Pow(V, (2f / 3f));
            }

            float pA = dgstEncy * pX;
            float pM = p_MT * V;
            float pT = p_TT * Mathf.Pow(V, (2f / 3f));
            float pS = pM + pT;
            float pC = (E / V) * (spStrCst * v_T * Mathf.Pow(V, (2f / 3f)) + pS) /
                (somFr * E / V + spStrCst);
            float pJ = k_JT * H;

            //differential equations
            float dE = pA - pC; // dE/dt
            float dV = (somFr * pC - pS) / spStrCst; // dV/dt

            float dH = 0;
            float dR = 0;
            if (H < pubMat)
            {
                dH = (1 - somFr) * pC - pJ; //dEH/dt
            }
            else
            {
                dR = (1 - somFr) * pC - pJ;
            }

            stateDifferential[i] = new float[] { dE, dV, dH, dR };
        }

        //compute physical stats

        //split up outputs
        float[] E_Arr = new float[foodData.Length];
        float[] V_Arr = new float[foodData.Length];
        float[] H_Arr = new float[foodData.Length];
        float[] R_Arr = new float[foodData.Length];

        for (int i = 0; i < foodData.Length; i++)
        {
            E_Arr[i] = stateData[i][0];
            V_Arr[i] = stateData[i][1];
            H_Arr[i] = stateData[i][2];
            R_Arr[i] = stateData[i][3];
        }

        //compute things
        float[] L_w = new float[foodData.Length]; // cm physical length
        float[] W_V = new float[foodData.Length]; // dry weight of structure
        float[] W_E_and_R = new float[foodData.Length]; // dry W of E and E_R
        float[] W = new float[foodData.Length]; //total dry weight
        float[] W_w = new float[foodData.Length]; //assume the same water content in structure and reserves
        float[] E_V = new float[foodData.Length];
        float[] E_w = new float[foodData.Length];
        float[] F = new float[foodData.Length]; // Fecundity = egg number

        for (int i = 0; i < foodData.Length; i++)
        {
            L_w[i] = Mathf.Pow(V_Arr[i], 1f / 3f) / shpCo;

            W_V[i] = strSpDen * V_Arr[i];
            W_E_and_R[i] = resMolMass / resChmPot * (E_Arr[i] + R_Arr[i]);
            W[i] = W_V[i] + W_E_and_R[i];
            W_w[i] = wetDryCo * W[i];

            E_V[i] = strChmPot * strSpDen / strMolMass * V_Arr[i];
            E_w[i] = (E_V[i] + E_Arr[i] + R_Arr[i]) / W_w[i];

            F[i] = repEncy * R_Arr[i] / initReserveEnergy;
        }

        reproductionGraph.setData(R_Arr);
        structuralVolumeGraph.setData(V_Arr);
        wetWeightGraph.setData(W_w);
    }

	// Use this for initialization
	void Start () {
        randomData();
        waterGraph.setData(waterTempData);
        foodGraph.setData(foodData);
        computeResults();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
