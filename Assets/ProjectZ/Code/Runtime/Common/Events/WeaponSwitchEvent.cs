namespace ProjectZ.Code.Runtime.Common.Events
{
    // Cast from Character when RefreshWeaponSetup, this event is exclusively for the UI to pick up
    public struct WeaponSwitchEvent
    {
        public string Name;
        public int AmmoMagazine;
        public int AmmoInventory;
    }
}