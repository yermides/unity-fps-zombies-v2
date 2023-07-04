using System.Threading.Tasks;
using ProjectZ.Code.Runtime.Core.Audio;
using ProjectZ.Code.Runtime.Patterns;
using ProjectZ.Code.Runtime.Patterns.Commands;

namespace ProjectZ.Code.Runtime.Common.Commands
{
    public class PlayOneShotClipCommand : ICommand
    {
        private static ServiceLocator Locator = ServiceLocator.Instance;
        private readonly AudioClipID _clipID;
        private readonly AudioSettings _audioSettings;
        private readonly float _delay;

        public PlayOneShotClipCommand(AudioClipID clipID, AudioSettings audioSettings = default, float delay = 0.0f)
        {
            _clipID = clipID;
            _audioSettings = audioSettings;
            _delay = delay;
        }
        
        public Task Execute()
        {
            var factory = Locator.GetService<AudioClipFactory>();
            var clip = factory.Get(_clipID);
            var service = Locator.GetService<IAudioManagerService>();

            if (_delay > 0.0f)
            {
                service.PlayOneShotDelayed(clip, _audioSettings, _delay);
            }
            else
            {
                service.PlayOneShot(clip, _audioSettings);
            }

            return Task.CompletedTask;
        }
    }
}