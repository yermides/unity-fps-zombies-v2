using System;
using ProjectZ.Code.Runtime.Common.Events;
using ProjectZ.Code.Runtime.Patterns.Events;
using UnityEngine;
using ServiceLocator = ProjectZ.Code.Runtime.Patterns.ServiceLocator;

namespace ProjectZ.Code.Runtime.UI.Character
{
    public class CharacterPresenter : IDisposable
    {
        private readonly CharacterViewModel _characterViewModel;

        public CharacterPresenter(CharacterViewModel characterViewModel)
        {
            _characterViewModel = characterViewModel;
            
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Subscribe<WeaponSwitchEvent>(OnWeaponSwitch);
            eventQueue.Subscribe<WeaponFiredEvent>(OnWeaponFired);
            eventQueue.Subscribe<WeaponReloadedEvent>(OnWeaponReloaded);
            eventQueue.Subscribe<PointsUpdatedEvent>(OnPointsUpdated);
        }

        private void OnPointsUpdated(PointsUpdatedEvent data)
        {
            _characterViewModel.Points.Value = data.Points;
        }

        // TODO:
        private void OnWeaponFired(WeaponFiredEvent data)
        {
            _characterViewModel.RoundsInMagazine.Value = data.RoundsMagazine;
            // _viewModel.IsVisible.Value = !_viewModel.IsVisible.Value;
        }
        
        private void OnWeaponReloaded(WeaponReloadedEvent data) 
        { 
            _characterViewModel.RoundsInMagazine.Value = data.RoundsMagazine;
            _characterViewModel.RoundsInInventory.Value = data.RoundsInventory;
        }

        private void OnWeaponSwitch(WeaponSwitchEvent data)
        {
            _characterViewModel.WeaponName.Value = data.Name;
            _characterViewModel.RoundsInMagazine.Value = data.AmmoMagazine;
            _characterViewModel.RoundsInInventory.Value = data.AmmoInventory;
        }

        public void Dispose()
        {
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Unsubscribe<WeaponSwitchEvent>(OnWeaponSwitch);
            eventQueue.Unsubscribe<WeaponFiredEvent>(OnWeaponFired);
            eventQueue.Unsubscribe<WeaponReloadedEvent>(OnWeaponReloaded);
            eventQueue.Unsubscribe<PointsUpdatedEvent>(OnPointsUpdated);
        }
    }
}