using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

namespace ProjectZ.Code.Runtime.UI.Character
{
    public class CharacterView : ViewBase
    {
        [SerializeField] private Canvas characterViewCanvas;
        [SerializeField] private CanvasGroup characterViewCanvasGroup;
        [SerializeField] private TextMeshProUGUI weaponNameText;
        [SerializeField] private TextMeshProUGUI roundsMagazineText;
        [SerializeField] private TextMeshProUGUI roundsInventoryText;
        [SerializeField] private TextMeshProUGUI pointsText;
        private CharacterViewModel _characterViewModel;
        private bool _isFading;

        public void Configure(CharacterViewModel characterViewModel)
        {
            _characterViewModel = characterViewModel;

            _characterViewModel
                .IsVisible
                .Subscribe(SetVisibility)
                .AddTo(disposables);
            
            _characterViewModel
                .WeaponName
                .Subscribe(SetWeaponName)
                .AddTo(disposables);
                
            _characterViewModel
                .RoundsInMagazine
                .Subscribe(SetRoundsMagazine)
                .AddTo(disposables);
                
            _characterViewModel
                .RoundsInInventory
                .Subscribe(SetRoundsInventory)
                .AddTo(disposables);
                
            _characterViewModel
                .Points
                .Subscribe(SetPoints)
                .AddTo(disposables);
        }

        private void SetVisibility(bool visible)
        {
            if (_isFading) return;
            // if (visible == characterViewCanvas.enabled) return;

            _isFading = true;
            float targetAlpha = visible ? 1.0f : 0.0f;

            characterViewCanvasGroup
                .DOFade(targetAlpha, 1.0f)
                .SetEase(Ease.InOutSine)
                .OnStart(() => { characterViewCanvas.enabled = true; })
                .OnComplete(() =>
            {
                characterViewCanvas.enabled = visible;
                _isFading = false;
            });

            // characterViewCanvas.enabled = visible;
        }

        private void SetWeaponName(string wName) => weaponNameText.text = wName;
        private void SetRoundsMagazine(int rounds) => roundsMagazineText.text = $"{rounds}";
        private void SetRoundsInventory(int rounds) => roundsInventoryText.text = $"{rounds}";
        private void SetPoints(int points) => pointsText.text = $"{points}";
    }
}