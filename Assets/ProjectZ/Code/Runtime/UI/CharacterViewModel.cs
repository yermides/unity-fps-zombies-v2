using UniRx;

namespace ProjectZ.Code.Runtime.UI
{
    public class CharacterViewModel
    {
        public readonly ReactiveProperty<string> WeaponName;
        public readonly ReactiveProperty<int> RoundsInMagazine;
        public readonly ReactiveProperty<int> RoundsInInventory;

        public CharacterViewModel()
        {
            WeaponName = new StringReactiveProperty();
            RoundsInMagazine = new IntReactiveProperty();
            RoundsInInventory = new IntReactiveProperty();
        }
    }
}