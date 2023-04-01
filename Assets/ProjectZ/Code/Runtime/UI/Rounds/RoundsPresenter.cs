using System;
using ProjectZ.Code.Runtime.Common.Events;
using ProjectZ.Code.Runtime.Patterns;
using ProjectZ.Code.Runtime.Patterns.Events;

namespace ProjectZ.Code.Runtime.UI.Rounds
{
    public class RoundsPresenter : IEventListener<RoundStartedEvent>, IEventListener<RoundFinishedEvent>, IDisposable
    {
        private readonly RoundsViewModel _roundsViewModel;

        public RoundsPresenter(RoundsViewModel roundsViewModel)
        {
            _roundsViewModel = roundsViewModel;
            
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Subscribe<RoundStartedEvent>(OnEventRaised);
            eventQueue.Subscribe<RoundFinishedEvent>(OnEventRaised);
        }

        public void OnEventRaised(RoundStartedEvent data)
        {
            _roundsViewModel.IsVisible.Value = true;
            _roundsViewModel.Round.Value = data.RoundNumber;
        }

        public void OnEventRaised(RoundFinishedEvent data)
        {
            _roundsViewModel.IsVisible.Value = false;
        }

        public void Dispose()
        {
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Unsubscribe<RoundStartedEvent>(OnEventRaised);
            eventQueue.Unsubscribe<RoundFinishedEvent>(OnEventRaised);
        }
    }
}