using MVVMC;


namespace Votacao.View.Search
{
    public class PollingPlaceController : Controller
    {
        public override void Initial()
        {
            PollingPlaceVoter();
        }

        public void PollingPlaceVoter()
        {
            ExecuteNavigation();
        }
    }
}
