using UnityEngine;

namespace ProjectZ.Code.Runtime.UI
{
    public class CharacterMvpInstaller : MonoBehaviour
    {
        [SerializeField] private CharacterView characterView;
        
        private void Start()
        {
            var characterViewModel = new CharacterViewModel();
            characterView.Configure(characterViewModel);
            var characterPresenter = new CharacterPresenter(characterViewModel);
        }
    }
}