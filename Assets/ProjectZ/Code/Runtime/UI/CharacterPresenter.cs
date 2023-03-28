using System;
using ProjectZ.Code.Runtime.Common.Events;
using ProjectZ.Code.Runtime.Patterns.Events;
using UnityEngine;
using ServiceLocator = ProjectZ.Code.Runtime.Patterns.ServiceLocator;

namespace ProjectZ.Code.Runtime.UI
{
    public class CharacterPresenter : IDisposable
    {
        private CharacterViewModel _viewModel;

        public CharacterPresenter(CharacterViewModel viewModel)
        {
            _viewModel = viewModel;
            
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Subscribe<WeaponSwitchEvent>(OnWeaponSwitch);
            eventQueue.Subscribe<WeaponFiredEvent>(OnWeaponFired);
            eventQueue.Subscribe<WeaponReloadedEvent>(OnWeaponReloaded);
        }

        // TODO:
        private void OnWeaponFired(WeaponFiredEvent data)
        {
            _viewModel.RoundsInMagazine.Value = data.RoundsMagazine;
        }
        
        private void OnWeaponReloaded(WeaponReloadedEvent data) 
        { 
            _viewModel.RoundsInMagazine.Value = data.RoundsMagazine;
            _viewModel.RoundsInInventory.Value = data.RoundsInventory;
        }

        private void OnWeaponSwitch(WeaponSwitchEvent data)
        {
            _viewModel.WeaponName.Value = data.Name;
            _viewModel.RoundsInMagazine.Value = data.AmmoMagazine;
            _viewModel.RoundsInInventory.Value = data.AmmoInventory;
        }

        public void Dispose()
        {
            Debug.LogWarning("Disposed");
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Unsubscribe<WeaponSwitchEvent>(OnWeaponSwitch);
            eventQueue.Unsubscribe<WeaponFiredEvent>(OnWeaponFired);
            eventQueue.Unsubscribe<WeaponReloadedEvent>(OnWeaponReloaded);
        }
    }
}