using UnityEngine;

namespace ProjectZ.Code.Runtime.UI.Rounds
{
    public class RoundsMvpInstaller : MonoBehaviour
    {
        [SerializeField] private RoundsView roundsView;
        private RoundsPresenter _roundsPresenter;

        private void Start()
        {
            var roundsViewModel = new RoundsViewModel();
            roundsView.Configure(roundsViewModel);
            _roundsPresenter = new RoundsPresenter(roundsViewModel);
        }

        private void OnDestroy()
        {
            _roundsPresenter.Dispose();
        }
    }
}