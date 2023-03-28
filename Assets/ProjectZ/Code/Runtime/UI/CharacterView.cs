using TMPro;
using UniRx;
using UnityEngine;

namespace ProjectZ.Code.Runtime.UI
{
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI weaponNameText;
        [SerializeField] private TextMeshProUGUI roundsMagazineText;
        [SerializeField] private TextMeshProUGUI roundsInventoryText;
        private CharacterViewModel _characterViewModel;

        public void Configure(CharacterViewModel characterViewModel)
        {
            _characterViewModel = characterViewModel;

            _characterViewModel.WeaponName.Subscribe(SetWeaponName);
            _characterViewModel.RoundsInMagazine.Subscribe(SetRoundsMagazine);
            _characterViewModel.RoundsInInventory.Subscribe(SetRoundsInventory);
        }

        private void SetWeaponName(string wName) => weaponNameText.text = wName;
        private void SetRoundsMagazine(int rounds) => roundsMagazineText.text = $"{rounds}";
        private void SetRoundsInventory(int rounds) => roundsInventoryText.text = $"{rounds}";
    }
}