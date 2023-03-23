using System.Threading.Tasks;
using ProjectZ.Code.Runtime.Core.Audio;
using ProjectZ.Code.Runtime.Patterns;
using ProjectZ.Code.Runtime.Patterns.Commands;

namespace ProjectZ.Code.Runtime.Common.Commands
{
    public class PlayOneShotClipCommand : ICommand
    {
        private readonly AudioClipID _clipID;
        private readonly AudioSettings _audioSettings;

        public PlayOneShotClipCommand(AudioClipID clipID, AudioSettings audioSettings = default)
        {
            _clipID = clipID;
            _audioSettings = audioSettings;
        }
        
        public Task Execute()
        {
            var locator = ServiceLocator.Instance;
            var factory = locator.GetService<AudioClipFactory>();
            var clip = factory.Get(_clipID);
            var service = locator.GetService<IAudioManagerService>();
            service.PlayOneShot(clip, _audioSettings);
            return Task.CompletedTask;
        }
    }
}