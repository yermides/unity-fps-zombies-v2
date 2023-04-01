using System;
using ProjectZ.Code.Runtime.Character;
using UnityEngine;

namespace ProjectZ.Code.Runtime.UI.Character
{
    public class CharacterMvpInstaller : MonoBehaviour
    {
        [SerializeField] private CharacterBehaviour characterBehaviour;
        [SerializeField] private CharacterView characterView;
        private CharacterPresenter _characterPresenter;

        private void Start()
        {
            var characterViewModel = new CharacterViewModel();
            characterView.Configure(characterViewModel);
            _characterPresenter = new CharacterPresenter(characterViewModel);

            int pointsToShow = characterBehaviour ? characterBehaviour.GetPoints() : 500;

            characterViewModel.Points.Value = pointsToShow;
            // characterViewModel.IsVisible.Value = false;
        }

        private void OnDestroy()
        {
            _characterPresenter.Dispose();
        }
    }
}