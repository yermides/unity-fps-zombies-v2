using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

namespace ProjectZ.Code.Runtime.UI.Rounds
{
    public class RoundsView : ViewBase
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI roundsText;
        private RoundsViewModel _roundsViewModel;

        public void Configure(RoundsViewModel roundsViewModel)
        {
            _roundsViewModel = roundsViewModel;
            
            _roundsViewModel
                .IsVisible
                .Subscribe(SetVisibility)
                .AddTo(disposables);
            
            _roundsViewModel
                .Round
                .Subscribe(SetRoundsText)
                .AddTo(disposables);
        }

        private void SetRoundsText(int round) => roundsText.text = $"{round}";

        private void SetVisibility(bool visible)
        {
            float targetAlpha = visible ? 1.0f : 0.0f;
            canvasGroup.DOFade(targetAlpha, 1.0f);
        }
    }
}