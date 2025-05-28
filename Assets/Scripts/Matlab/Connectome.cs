
/// <summary>
/// translated (and slightly modified) matlab class
/// </summary>
public class Connectome : MatlabUtil
{
    private MatlabMatrix a_el;
    private MatlabMatrix a_ACh;
    private MatlabMatrix a_GABA;
    private MatlabMatrix a_Glu;
    private MatlabMatrix a_mon;
    private MatlabMatrix a_np;
    private MatlabMatrix a_extra;
    //private MatlabMatrix lp;

    public MatlabMatrix A_el { get => a_el.Clone(); private set => a_el = value; }
    public MatlabMatrix A_ACh { get => a_ACh.Clone(); private set => a_ACh = value; }
    public MatlabMatrix A_GABA { get => a_GABA.Clone(); private set => a_GABA = value; }
    public MatlabMatrix A_Glu { get => a_Glu.Clone(); private set => a_Glu = value; }
    public MatlabMatrix A_mon { get => a_mon.Clone(); private set => a_mon = value; }
    public MatlabMatrix A_np { get => a_np.Clone(); private set => a_np = value; }
    public MatlabMatrix A_extra { get => a_extra.Clone(); private set => a_extra = value; }

    //public MatlabMatrix Lp { get => lp.Clone(); private set => lp = value; }
    public int nN { get; private set; }
    //public int nC { get; private set; }

    public Connectome()
    {
        //adjacency matrices
        A_el = ReadMatrix(SerializationUtil.GetString(SerializationUtil.Paths.Matlabelectrical_weights));                           //gap junctions
        A_ACh = ReadMatrix(SerializationUtil.GetString(SerializationUtil.Paths.MatlabACh_weights));                                 //ACh synapses
        A_GABA = ReadMatrix(SerializationUtil.GetString(SerializationUtil.Paths.MatlabGABA_weights));                               //GABA synapses
        A_Glu = ReadMatrix(SerializationUtil.GetString(SerializationUtil.Paths.MatlabGlu_weights));                                 //Glu synapses
        A_mon = ReadMatrix(SerializationUtil.GetString(SerializationUtil.Paths.A_mon));                  //monoamine signaling
        A_np = ReadMatrix(SerializationUtil.GetString(SerializationUtil.Paths.A_np));    //short distance neuropeptide signaling
        A_extra = Zeros(A_ACh.Size(0), A_ACh.Size(1));                                                                              //extrasynaptic coupling, for custom connections

        //incidence and Laplacian matrix
        MatlabMatrix N = IncidenceFromAdjacency(A_el);
        //Lp = N * N.Transpose();

        //number of neurons
        nN = Size(N, 0);                                            //nN: number of neurons
        //nC = Size(N, 1);                                            //nC: number of connections via gap junctions
    }

}
