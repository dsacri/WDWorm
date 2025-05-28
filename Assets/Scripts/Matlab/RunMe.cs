using System;
using UnityEngine;

/// <summary>
/// translated (and slightly modified) matlab class
/// </summary>
public class RunMe : MatlabUtil
{
    public static RunMe Instance { get; private set; }

    //C. elegans connectome
    private Connectome connectome = new();

    //simulation setup
    //simulation parameters
    public static double T = 7e-3;
    public MatlabMatrix t;
    private int ni;
    //input signal
    private MatlabMatrix jExt;
    private double pulsewidth;
    private int pulseSamples;
    private MatlabMatrix idxInputNeurons;
    //logger data
    public MatlabMatrix u_logging;
    public MatlabMatrix eta_logging;

    //biological parameters
    //calcium imaging parameters(for GCaMP6s)
    private double alpha;
    private double tau;
    private double etaCaInf;

#region electricalParameters.m
    //electrical parameters
    //calcium channel resistance and integrator circuit for calcium concentration
    private double CI;
    private double RI;
    private double ICaMin;
    private double etaCaInfCorrected;
    //Morris-Lecar model
    private MatlabMatrix C;
    private MatlabMatrix EL;
    private MatlabMatrix GL;
    private MatlabMatrix GCa1;
    private MatlabMatrix ECa;
    private MatlabMatrix UCa1;
    private MatlabMatrix UCa2;
    private MatlabMatrix GK1;
    private MatlabMatrix EK;
    private MatlabMatrix UK1;
    private MatlabMatrix UK2;
    private MatlabMatrix FK;
    //coupling parameters
    private double gEl;
    private MatlabMatrix gGlu;
    private MatlabMatrix gACh;
    private MatlabMatrix gGABA;
    private MatlabMatrix Eexc;
    private MatlabMatrix Einh;
    private MatlabMatrix Us1;
    private MatlabMatrix Us2;
    //monoamine coupling
    private MatlabMatrix gMo;
    private MatlabMatrix EMo; //selected as excitatory
    private MatlabMatrix UMo1;
    private MatlabMatrix UMo2;
    //extrasynaptic/neuropeptide coupling
    private MatlabMatrix gExtra;
    private MatlabMatrix Eextra; //selected as excitatory
    private MatlabMatrix Uex1;
    private MatlabMatrix Uex2;
    #endregion electricalParameters.m

    //wave digital parameters
    //Morris-Lecar model
    private MatlabMatrix RC;
    private MatlabMatrix RM;
    private MatlabMatrix RL;
    private MatlabMatrix Rp;
    private MatlabMatrix Rr;
    private MatlabMatrix gamma;
    //integrator circuit for calcium concentration
    private double RCI;
    private double R;
    private MatlabMatrix RpI;
    private double RpIn;
    private MatlabMatrix gammaI;

#region initializeVariables.m
    //initialize incident waves, states, and harmonic waves
    //Morris-Lecar model
    private MatlabMatrix bC;
    private MatlabMatrix ap3;
    private MatlabMatrix zK;
    private MatlabMatrix aCI;
#endregion initializeVariables.m

    private MatlabMatrix A_Glu_new, A_ACh_new, A_GABA_new, A_extra_new, A_el_new, A_mon_new, A_np_new, S;
    public static MatlabMatrix motorNSet = new (new double[,]
    {
        {
            28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
            61, 62,
            70,
            86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 
            109,
            113, 114, 115, 116, 117, 118, 119, 120, 
            135, 136, 
            155, 156, 
            171,
            177, 178, 
            183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 
            218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 
            238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278
        }
    });
    public static MatlabMatrix interNSet = new (new double[,]
    {
        {
            0, 1, 
            10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 
            53, 54, 55, 56, 57, 58, 59, 60,
            64, 65, 66, 67, 68, 69,
            80, 81, 
            108,
            110,
            127, 128, 
            150, 151, 
            157, 158, 159, 160, 
            162, 163, 164, 165, 166, 167, 168, 169, 170, 172, 173, 174, 175, 176, 
            179, 180, 181, 182, 
            201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217,
        }
    });
    public static MatlabMatrix sensoryNSet = new(new double[,]
    {
        {
            2, 3, 4, 5, 6, 7, 8, 9,
            23, 24, 25, 26, 27,
            39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52,
            63,            
            71, 72, 73, 74, 75, 76, 77, 78, 79,
            82, 83, 84, 85,
            111, 112,
            121, 122, 123, 124, 125, 126,
            129, 130, 131, 132, 133, 134,
            137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149,
            152, 153, 154,
            161,
            230, 231, 232, 233, 234, 235, 236, 237
        }
    });
    public static MatlabMatrix allNeuronsSet = Fill(0, 278);
    public static MatlabMatrix locomotionSet = new(new double[,]
    {
        {
            23, 24,
            28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
            53, 54, 55, 56, 57, 58,
            86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107,
            135,
            145, 146,
            150, 151,
            238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260,
            266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278
        }
    });
    public static MatlabMatrix chemosensorySet = new(new double[,]
    {
        {
            8, 9, 10, 11, 12, 13,
            18, 19,
            39, 40,
            53, 54, 55, 56,
            76, 77,
            110, 111, 112,
            165, 166, 167, 168,
            177, 178,
            201, 202
        }
    });

    public int nextLoop = 0;

    public RunMe()
    {
        Instance = this;

        //C. elegans connectome
        //connectome = new();

        //simulation setup
        //simulation parameters
        //T = 0.05;                                                             //step size
        t = Fill(0, T, 20);                                        //time vector
        ni = 2;                                                                 //number of additional iteration steps for solving implicit relationships
        //input signal
        jExt = Zeros(Length(t), connectome.nN);
        //logger data
        u_logging = Zeros(Length(t), connectome.nN);               //membrane potential
        eta_logging = Zeros(Length(t), connectome.nN);             //calcium concentration

        //biological parameters
        //calcium imaging parameters(for GCaMP6s)
        alpha = 1e3;                                                            //calcium - concentration - to - current conversion factor
        tau = 0.79;                                                             //calcium decay rate
        etaCaInf = 50e-9;                                                       //resting calcium concentration

#region electricalParameters.m
        //electrical parameters
        //calcium channel resistance and integrator circuit for calcium concentration
        CI = 1 / alpha;
        RI = tau / CI;
        ICaMin = 0.233e-12;
        etaCaInfCorrected = etaCaInf - RI * ICaMin;
        //Morris-Lecar model
        C = 20e-11 * Ones(1, connectome.nN);
        EL = -60e-3 * Ones(1, connectome.nN);
        GL = 2e-9 * Ones(1, connectome.nN);
        GCa1 = 4.4e-9 * Ones(1, connectome.nN);
        ECa = 120e-3 * Ones(1, connectome.nN);
        UCa1 = -10e-3 * Ones(1, connectome.nN);
        UCa2 = 18e-3 * Ones(1, connectome.nN);
        GK1 = 7e-9 * Ones(1, connectome.nN);
        EK = -84e-3 * Ones(1, connectome.nN);
        UK1 = 2e-3 * Ones(1, connectome.nN);
        UK2 = 30e-3 * Ones(1, connectome.nN);
        FK = 1 * Ones(1, connectome.nN);
        //coupling parameters
        gEl = 0.546e-9;//(0.546e-9 * Ones(connectome.nC, 1)).Diag();
        gGlu = 0.27e-9 * Ones(connectome.nN) * 2;
        gACh = 0.27e-9* Ones(connectome.nN);
        gGABA = 0.405e-9* Ones(connectome.nN);
        Eexc = 110e-3* Ones(connectome.nN);
        Einh = -120e-3* Ones(connectome.nN);
        Us1 = -20e-3* Ones(connectome.nN);
        Us2 = 0.1e-3 * Ones(connectome.nN);
        //monoamine coupling
        gMo = 0.054e-9 * Ones(connectome.nN) / 2;
        EMo = 110e-3 * Ones(connectome.nN);             //selected as excitatory
        UMo1 = 40e-3 * Ones(connectome.nN);
        UMo2 = 1e-3 * Ones(connectome.nN);
        //extrasynaptic / neuropeptide coupling
        gExtra = 0.027e-9* Ones(connectome.nN)/2;
        Eextra = 110e-3* Ones(connectome.nN);           //selected as excitatory
        Uex1 = 50e-3* Ones(connectome.nN);
        Uex2 = 10e-3 * Ones(connectome.nN);
        #endregion electricalParameters.m

        //wave digital parameters
        //Morris - Lecar model
        RC = T / (2.0 * C);
        RM = 1 / (0.5 * (GK1 + GCa1));
        RL = 1 / GL;
        Rp = MatlabMatrix.ArrayVertical(RC, RL, RM);
        Rr = 1.0 / (SumMatrix(1.0 / Rp));
        gamma = MatlabMatrix.ExpandMatrixBelow(Rr / Rp, Ones(1, connectome.nN));
        //integrator circuit for calcium concentration
        RCI = T / (2.0 * CI);
        R = RI;
        RpI = MatlabMatrix.ExpandMatrixBelow(RCI, R);
        RpIn = 1.0 / (Sum(1.0 / RpI));
        gammaI = MatlabMatrix.ExpandMatrixBelow(RpIn / RpI, 1.0);

        //define sets of neurons
        //see above

        #region initializeVariables.m
        //initialize incident waves, states, and harmonic waves
        //Morris - Lecar model
        bC = -69e-3 * Ones(1, connectome.nN);
        ap3 = Zeros(1, connectome.nN);
        zK = Zeros(1, connectome.nN);
        aCI = 50e-9 * Ones(1, connectome.nN);
        #endregion initializeVariables.m

        ApplySettings();
    }

    public void ApplySettings()
    {
        InitializationManager.Instance.MatlabSerializedData = MatlabSettingsSerialization.CreateMatlabSerializedData();
        AllGraphsWindow.Instance.ApplySettings();

        MatlabSerializedData matlabSerializedData = InitializationManager.Instance.MatlabSerializedData;

        if (!matlabSerializedData.ParseAllValues())
        {
            Debug.Log("Parse failed!");
        }


        //set custom extrasynaptic connections between neurons
        //select neurons to be connected. Connections are made between transmitting and receiving neurons with the same position in the index arrays
        MatlabMatrix A_extra = connectome.A_extra;
        for (int i = 0; i < matlabSerializedData.connections.Length; i++)
        {
            A_extra[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = 1;

            gGlu[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].gGluValues;
            gACh[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].gAChValues;
            gGABA [matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].gGABAValues;
            gMo[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].gMoValues;
            gExtra[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].gExtraValues;
            Eexc[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].EexcValues;
            Einh[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].EinhValues;
            EMo[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].EMoValues;
            Eextra[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].EextraValues;
            Us1[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].Us1Values;
            UMo1[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].UMo1Values;
            Uex1[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].Uex1Values;
            Us2[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].Us2Values;
            UMo2[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].UMo2Values;
            Uex2[matlabSerializedData.connections[i].fromNeuron, matlabSerializedData.connections[i].toNeuron] = matlabSerializedData.connections[i].Uex2Values;
        }

        //deactivate specific chemical layers
        if (!matlabSerializedData.ActivateA_Glu)
            A_Glu_new = 0 * A_Glu_new;

        if (!matlabSerializedData.ActivateA_ACh)
            A_ACh_new = 0 * A_ACh_new;

        if (!matlabSerializedData.ActivateA_GABA)
            A_GABA_new = 0 * A_GABA_new;

        if (!matlabSerializedData.ActivateA_mon)
            A_el_new = 0 * A_mon_new;

        if (!matlabSerializedData.ActivateA_np)
            A_el_new = 0 * A_np_new;

        if (!matlabSerializedData.ActivateA_el)
            A_el_new = 0 * A_el_new;

        //select specific circuitries  (removes all other neurons)
        /*MatlabMatrix selectMatrix;
        if (matlabSerializedData.motor)
            selectMatrix = motorNSet;
        else if (matlabSerializedData.inter)
            selectMatrix = interNSet;
        else if (matlabSerializedData.sensory)
            selectMatrix = sensoryNSet;
        else if (matlabSerializedData.locomotion)
            selectMatrix = locomotionSet;
        else if (matlabSerializedData.chemosensory)
            selectMatrix = chemosensorySet;
        else //if (matlabSerializedData.allNeurons)
            selectMatrix = allNeuronsSet;

        new SelectCircuitry(new SelectCircuitry.InputPackage(selectMatrix, connectome.A_Glu, connectome.A_ACh, connectome.A_GABA, connectome.A_el, connectome.A_extra, connectome.A_mon, connectome.A_np, Rr, gEl),
            out A_Glu_new, out A_ACh_new, out A_GABA_new, out A_extra_new, out A_el_new, out A_mon_new, out A_np_new, out S);*/

        //remove specific neurons from network
        MatlabMatrix idxNeurons = new();
        for (int i = 0; i < matlabSerializedData.idxNeurons.Length; i++)
        {
            if (!matlabSerializedData.idxNeurons[i])
            {
                idxNeurons = MatlabMatrix.ExpandMatrixRight(idxNeurons, i);
            }
        }
        new DeactivateNeurons(new DeactivateNeurons.InputPackage(idxNeurons, connectome.A_Glu, connectome.A_ACh, connectome.A_GABA, connectome.A_el, connectome.A_extra, connectome.A_mon, connectome.A_np, Rr, gEl),
            out A_Glu_new, out A_ACh_new, out A_GABA_new, out A_extra_new, out A_el_new, out A_mon_new, out A_np_new, out S);

        //choose input signal
        //characterize input signal
        pulsewidth = matlabSerializedData.pulseWidthValue;
        pulseSamples = (int)Math.Floor(pulsewidth / T);
        double currentAmplitude = matlabSerializedData.currentAmplitudeValue;

        //choose neurons to stimulate
        idxInputNeurons = new();
        for (int i = 0; i < matlabSerializedData.idxInputNeurons.Length; i++)
        {
            if (matlabSerializedData.idxInputNeurons[i])
            {
                idxInputNeurons = MatlabMatrix.ExpandMatrixRight(idxInputNeurons, i);
            }
        }

        jExt.SetValues(idxInputNeurons, pulseSamples, currentAmplitude);
    }

    public void RunOneLoop(int k)
    {
        if (k != nextLoop && k != 0)
        {
            Debug.LogWarning("Wrong k: expected " + nextLoop + ", got " + k);
            return;
        }

        nextLoop = k + 1;

        //wave digital model of Morrs-Lecar model
        MatlabMatrix jk = jExt.GetRow(k);

        MatlabMatrix ap4 = new();
        MatlabMatrix u = new();
        MatlabMatrix GCa = new();

        //solve implicit relationship
        for (int i = 0; i < ni; i++)
        {
            MatlabMatrix bp4 = gamma.GetRow(0) * (Dot)bC + gamma.GetRow(1) * (Dot)EL + gamma.GetRow(2) * (Dot)ap3;
            ap4 = bp4 * S;
            MatlabMatrix bp3 = gamma.GetRow(0) * (Dot)bC + gamma.GetRow(1) * (Dot)EL + gamma.GetRow(2) * (Dot)ap3 + gamma.GetRow(3) * (Dot)ap4 - ap3;
            //calculate reflected wave of memristive voltage source
            u = 1.0 / 2.0 * (ap3 + bp3);
            MatlabMatrix dzK = (1.0 / 2.0 * (1.0 + Tanh((u - UK1) / UK2)) - zK * (Dot)FK * (Dot)Cosh((u - UK1) / 2.0 * (Dot)UK2));
            zK = zK + T / ni * dzK;                             //potassium state
            MatlabMatrix WK = zK * (Dot)GK1;                                      //potassium memductance
            GCa = GCa1 * 1.0 / 2.0 * (Dot)(1.0 + Tanh((u - UCa1) / UCa2));          //sodium conductance
            MatlabMatrix M = 1.0 / (WK + GCa + GL);                           //total memristance
            MatlabMatrix rho = (M - RM) / (M + RM);                               //reflection coefficient

            MatlabMatrix sigma = 1 / (1 + Exp(-(u.Transpose() * Ones(1, connectome.nN) - Us1) / Us2));                             //activation function of synapses
            MatlabMatrix sigmaMo = 1 / (1 + Exp(-(u.Transpose() * Ones(1, connectome.nN) - UMo1) / UMo2));                           //activation function of monoamine-based transmission
            MatlabMatrix sigmaEx = 1 / (1 + Exp(-(u.Transpose() * Ones(1, connectome.nN) - Uex1) / Uex2));                           //activation function of neuropeptide-based transmission
            MatlabMatrix iSyn = -Ones(1, connectome.nN) * (sigma * (Dot)((gGlu * (Dot)A_Glu_new                         //synaptic current due to Glu
                        + gACh * (Dot)A_ACh_new) * (Dot)(Ones(connectome.nN, 1) * u - Eexc)                      //synaptic current due to ACh
                        + gGABA * (Dot)A_GABA_new * (Dot)(Ones(connectome.nN, 1) * u - Einh))                   //synaptic current due to GABA
                        + gMo * (Dot)A_mon_new * (Dot)sigmaMo * (Dot)(Ones(connectome.nN, 1) * u - EMo)               //synaptic current due to monoamines
                        + gExtra * (Dot)(A_np_new                                           //neuropeptide - caused current(only short - range couplings)
                        + A_extra_new) * (Dot)sigmaEx * (Dot)(Ones(connectome.nN, 1) * u - Eextra));                //extrasynaptic current due to connections set by user

            MatlabMatrix j = jk + iSyn;
            MatlabMatrix en = M * (Dot)(EK * (Dot)WK + ECa * (Dot)GCa + EL * (Dot)GL + j);          //voltage after source transformation
            ap3 = en + rho * (Dot)(bp3 - en);                         //reflected wave
        }

        //update delay elements
        MatlabMatrix ap1 = bC.Clone();
        MatlabMatrix bp1 = gamma.GetRow(0) * (Dot)bC + gamma.GetRow(1) * (Dot)EL + gamma.GetRow(2) * (Dot)ap3 + gamma.GetRow(3) * (Dot)ap4 - bC;
        bC = bp1.Clone();

        //wave digital model of integrator circuit
        //calculate calcium current as input signal
        MatlabMatrix iCa = -GCa * (Dot)(u - ECa);

        //wave flow diagram for calculating calcium concentration
        MatlabMatrix bpI3 = gammaI[0, 0] * aCI + gammaI[1, 0] * etaCaInfCorrected;
        MatlabMatrix apI3 = 2.0 * RpIn * iCa + bpI3;
        MatlabMatrix bpI1 = gammaI[0, 0] * aCI + gammaI[1, 0] * etaCaInfCorrected + gammaI[2, 0] * apI3 - aCI;

        //update delay elements
        MatlabMatrix apI1 = aCI.Clone();
        aCI = bpI1.Clone();
                
        //log data - convert waves to voltages and currents
        u_logging.SetRow(k, u);
        eta_logging.SetRow(k, 1.0 / 2.0 * (apI1 + bpI1));
    }

    //matlab translation end

    /// <summary>
    /// how many points of time are measured?
    /// </summary>
    public int GetMaxTime()
    {
        return t.Size(1);
    }

    public float[] GetNeuronActivityOfAllNeurons(int time)
    {
        MatlabMatrix neuronActivity = u_logging.GetRow(time) * Data.Instance.MatlabNeuronActivityMultiply + Data.Instance.MatlabNeuronActivityAdd;
        float[] floats = neuronActivity.ConvertToFloatArray();

        return floats;
    }

    public float[] GetAllNeuronActivityOfANeuron(int index, int time)
    {
        MatlabMatrix values = u_logging.GetColumn(index);
        values = values.Transpose();

        float[] floats = new float[time];
        Array.Copy(values.ConvertToFloatArray(), floats, time);

        return floats;
    }

    public float GetSingleNeuronActivityOfANeuron(int index, int time)
    {
        return (float)u_logging[time, index];
    }

    public float[] GetAllEtaOfANeuron(int index, int time)
    {
        MatlabMatrix values = eta_logging.GetColumn(index);
        values = values.Transpose();

        float[] floats = new float[time];
        Array.Copy(values.ConvertToFloatArray(), floats, time);

        return floats;
    }

    public float GetSingleEtaOfANeuron(int index, int time)
    {
        return (float)eta_logging[time, index];
    }

}
