using MVVMC;

namespace Votacao.View.Report
{
    public class BallotBoxController: Controller
    {
        public override void Initial()
        {
            InitBallotBox();
        }

        public void InitBallotBox()
        {
            ExecuteNavigation();
        }
    }
}
