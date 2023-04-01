using UniRx;

namespace ProjectZ.Code.Runtime.UI.Character
{
    public class CharacterViewModel
    {
        public readonly ReactiveProperty<bool> IsVisible;
        public readonly ReactiveProperty<string> WeaponName;
        public readonly ReactiveProperty<int> RoundsInMagazine;
        public readonly ReactiveProperty<int> RoundsInInventory;
        public readonly ReactiveProperty<int> Points;

        public CharacterViewModel()
        {
            IsVisible = new BoolReactiveProperty(true);
            WeaponName = new StringReactiveProperty("Weapon_Name");
            RoundsInMagazine = new IntReactiveProperty(30);
            RoundsInInventory = new IntReactiveProperty(300);
            Points = new IntReactiveProperty(500);
        }
    }
}