namespace ProjectZ.Code.Runtime.Common.Events
{
    // Cast from Character when RefreshWeaponSetup, this event is exclusively for the UI to pick up
    public struct WeaponSwitchEvent
    {
        public string Name;
        public int AmmoMagazine;
        public int AmmoInventory;
    }

    public struct WeaponFiredEvent
    {
        public int RoundsMagazine;
    }

    public struct WeaponReloadedEvent
    {
        public int RoundsMagazine;
        public int RoundsInventory;
    }
}