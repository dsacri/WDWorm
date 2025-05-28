using System;

/// <summary>
/// savable settings
/// </summary>
public class MatlabSerializedData
{

    #region graph settings
    public bool lineGraph;
    public bool heatmap;
    public bool membranePotential;
    public bool calciumConcentration;
    public bool alphanumeric;
    public bool byType;
    public bool[] graphNeurons;
    #endregion

    #region deactivate specific neurons          //Toggle neuron
    public bool[] idxNeurons;                  //Deactivated Neuron

    public bool motor;
    public bool inter;
    public bool sensory;
    public bool allNeurons;
    public bool locomotion;
    public bool chemosensory;
    public bool custom;
    #endregion

    #region choose input signal                     //External current stimuli
    public string currentAmplitude;                 //Current Amplitude 
    [NonSerialized] public double currentAmplitudeValue;
    public string pulseWidth;                       //Pulse Width 
    [NonSerialized] public double pulseWidthValue;
    public bool[] idxInputNeurons;                  //Stimulated Neuron
    #endregion

    #region set custom extrasynaptic connections between neurons        //Modify Connectome
    public bool ActivateA_Glu;                      //Glutamat
    public bool ActivateA_ACh;                      //Acetylcholine
    public bool ActivateA_GABA;                     //Gamma-Aminobutyric Acid
    public bool ActivateA_mon;                      //Monoamines
    public bool ActivateA_np;                       //Short-Ranged Neuropeptides
    public bool ActivateA_el;                       //Gap junctions
    public NeuronConnection[] connections;          //Connection between X and Y

    [Serializable]
    public class NeuronConnection
    {
        public int fromNeuron;
        public int toNeuron;
        public bool connected;
        [NonSerialized] public double gGluValues;
        public string gGlu;
        [NonSerialized] public double gAChValues;
        public string gACh;
        [NonSerialized] public double gGABAValues;
        public string gGABA;
        [NonSerialized] public double gMoValues;
        public string gMo;
        [NonSerialized] public double gExtraValues;
        public string gExtra;
        [NonSerialized] public double EexcValues;
        public string Eexc;
        [NonSerialized] public double EinhValues;
        public string Einh;
        [NonSerialized] public double EMoValues;
        public string EMo;
        [NonSerialized] public double EextraValues;
        public string Eextra;
        [NonSerialized] public double Us1Values;
        public string Us1;
        [NonSerialized] public double UMo1Values;
        public string UMo1;
        [NonSerialized] public double Uex1Values;
        public string Uex1;
        [NonSerialized] public double Us2Values;
        public string Us2;
        [NonSerialized] public double UMo2Values;
        public string UMo2;
        [NonSerialized] public double Uex2Values;
        public string Uex2;
    }
    #endregion

    #region Morris-Lecar model                      //Neuron Model Parameters
    public string[] C;
    [NonSerialized] public double[] CValues;
    public string[] EL;
    [NonSerialized] public double[] ELValues;
    public string[] GL;
    [NonSerialized] public double[] GLValues;
    public string[] GCa1;
    [NonSerialized] public double[] GCa1Values;
    public string[] ECa;
    [NonSerialized] public double[] ECaValues;
    public string[] UCa1;
    [NonSerialized] public double[] UCa1Values;
    public string[] UCa2;
    [NonSerialized] public double[] UCa2Values;
    public string[] GK1;
    [NonSerialized] public double[] GK1Values;
    public string[] EK;
    [NonSerialized] public double[] EKValues;
    public string[] UK1;
    [NonSerialized] public double[] UK1Values;
    public string[] UK2;
    [NonSerialized] public double[] UK2Values;
    public string[] FK;
    [NonSerialized] public double[] FKValues;
    #endregion


    public bool ParseAllValues()
    {
        if (!double.TryParse(currentAmplitude, out currentAmplitudeValue))
            return false;
        if (!double.TryParse(pulseWidth, out pulseWidthValue))
            return false;

        if (!ParseArray(C, out CValues))
            return false;
        if (!ParseArray(EL, out ELValues))
            return false;
        if (!ParseArray(GL, out GLValues))
            return false;
        if (!ParseArray(GCa1, out GCa1Values))
            return false;
        if (!ParseArray(ECa, out ECaValues))
            return false;
        if (!ParseArray(UCa1, out UCa1Values))
            return false;
        if (!ParseArray(UCa2, out UCa2Values))
            return false;
        if (!ParseArray(GK1, out GK1Values))
            return false;
        if (!ParseArray(EK, out EKValues))
            return false;
        if (!ParseArray(UK1, out UK1Values))
            return false;
        if (!ParseArray(UK2, out UK2Values))
            return false;
        if (!ParseArray(FK, out FKValues))
            return false;

        foreach (NeuronConnection connection in connections)
        {
            if (!double.TryParse(connection.gGlu, out connection.gGluValues))
                return false;
            if (!double.TryParse(connection.gACh, out connection.gAChValues))
                return false;
            if (!double.TryParse(connection.gGABA, out connection.gGABAValues))
                return false;
            if (!double.TryParse(connection.gMo, out connection.gMoValues))
                return false;
            if (!double.TryParse(connection.gExtra, out connection.gExtraValues))
                return false;
            if (!double.TryParse(connection.Eexc, out connection.EexcValues))
                return false;
            if (!double.TryParse(connection.Einh, out connection.EinhValues))
                return false;
            if (!double.TryParse(connection.EMo, out connection.EMoValues))
                return false;
            if (!double.TryParse(connection.Eextra, out connection.EextraValues))
                return false;
            if (!double.TryParse(connection.Us1, out connection.Us1Values))
                return false;
            if (!double.TryParse(connection.UMo1, out connection.UMo1Values))
                return false;
            if (!double.TryParse(connection.Uex1, out connection.Uex1Values))
                return false;
            if (!double.TryParse(connection.Us2, out connection.Us2Values))
                return false;
            if (!double.TryParse(connection.UMo2, out connection.UMo2Values))
                return false;
            if (!double.TryParse(connection.Uex2, out connection.Uex2Values))
                return false;
        }

        return true;
    }

    private static bool ParseArray(string[] stringArray, out double[] doubleArray)
    {
        doubleArray = new double[stringArray.Length];

        for (int i = 0; i < stringArray.Length; i++)
        {
            if (!double.TryParse(stringArray[i], out doubleArray[i]))
                return false;
        }

        return true;
    }

}
