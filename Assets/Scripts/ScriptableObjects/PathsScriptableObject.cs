using UnityEngine;

/// <summary>
/// paths to serialized files
/// </summary>
[CreateAssetMenu(fileName = "Paths", menuName = "ScriptableObjects/PathsScriptableObject", order = 1)]
public class PathsScriptableObject : ScriptableObject
{
    //important: change the values in unity inspector, not in this file

    public string[] NeuronNames = new string[] { "Neurons", "map_neuron_names_int", ".txt" };
    public string[] NeuronPositions = new string[] { "Neurons", "_bluevex202203_LowResAtlasWithHighResHeadsAndTails", ".csv" };
    public string[] NeuronTypes = new string[] { "Neurons", "CE_MLN_TMAE_nodes", ".csv" };

    public string[] MatlabACh_weights = new string[] { "Matlab", "data_MAE21_ACh_weights", ".csv" };
    public string[] Matlabelectrical_weights = new string[] { "Matlab", "data_MAE21_electrical_weights", ".csv" };
    public string[] MatlabGABA_weights = new string[] { "Matlab", "data_MAE21_GABA_weights", ".csv" };
    public string[] MatlabGlu_weights = new string[] { "Matlab", "data_MAE21_Glu_weights", ".csv" };

    public string[] A_mon = new string[] { "Neurons", "A_mon", ".csv" };
    public string[] A_np = new string[] { "Neurons", "A_np", ".csv" };

}
