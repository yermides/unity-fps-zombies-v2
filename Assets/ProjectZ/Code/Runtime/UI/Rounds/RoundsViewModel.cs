using UniRx;

namespace ProjectZ.Code.Runtime.UI.Rounds
{
    public class RoundsViewModel
    {
        public readonly ReactiveProperty<bool> IsVisible;
        public readonly ReactiveProperty<int> Round;

        public RoundsViewModel()
        {
            IsVisible = new BoolReactiveProperty(true);
            Round = new IntReactiveProperty(1);
        }
    }
}