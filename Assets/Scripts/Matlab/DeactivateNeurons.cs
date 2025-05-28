
/// <summary>
/// translated (and slightly modified) matlab class
/// </summary>
public class DeactivateNeurons : MatlabUtil
{
    public struct InputPackage
    {
        public MatlabMatrix idxNeurons, A_Glu, A_ACh, A_GABA, A_el, A_extra, A_mon, A_np, Rr;
        public double gEl;

        public InputPackage(MatlabMatrix idxNeurons, MatlabMatrix A_Glu, MatlabMatrix A_ACh, MatlabMatrix A_GABA, MatlabMatrix A_el, MatlabMatrix A_extra, MatlabMatrix A_mon, MatlabMatrix A_np, MatlabMatrix Rr, double gEl)
        {
            this.idxNeurons = idxNeurons;
            this.A_Glu = A_Glu;
            this.A_ACh = A_ACh;
            this.A_GABA = A_GABA;
            this.A_el = A_el;
            this.A_extra = A_extra;
            this.A_mon = A_mon;
            this.A_np = A_np;
            this.Rr = Rr;
            this.gEl = gEl;
        }
    }

    public DeactivateNeurons(InputPackage package, out MatlabMatrix A_Glu, out MatlabMatrix A_ACh, out MatlabMatrix A_GABA, out MatlabMatrix A_extra, out MatlabMatrix A_el, out MatlabMatrix A_mon, out MatlabMatrix A_np, out MatlabMatrix S)
    {
        A_Glu = package.A_Glu;
        A_ACh = package.A_ACh;
        A_GABA = package.A_GABA;
        A_extra = package.A_extra;
        A_mon = package.A_mon;
        A_np = package.A_np;
        A_el = package.A_el;


        //update connectivity matrices
        for (int i = 0; i < package.idxNeurons.Size(1); i++)
        {
            int value = (int)package.idxNeurons[i];

            A_Glu.SetRow(value, 0);
            A_Glu.SetColumn(value, 0);
            A_ACh.SetRow(value, 0);
            A_ACh.SetColumn(value, 0);
            A_GABA.SetRow(value, 0);
            A_GABA.SetColumn(value, 0);
            A_extra.SetRow(value, 0);
            A_extra.SetColumn(value, 0);
            A_mon.SetRow(value, 0);
            A_mon.SetColumn(value, 0);
            A_np.SetRow(value, 0);
            A_np.SetColumn(value, 0);
            A_el.SetRow(value, 0);
            A_el.SetColumn(value, 0);
        }

        MatlabMatrix N = IncidenceFromAdjacency(A_el);

        MatlabMatrix nonDiag = N * package.gEl * N.Transpose();
        MatlabMatrix diag = (1 / package.Rr).Diag();
        S = MlDivide(diag + nonDiag, diag - nonDiag);

    }
}
