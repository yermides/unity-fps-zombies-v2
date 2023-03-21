using ProjectZ.Code.Runtime.Map;

namespace ProjectZ.Code.Runtime.Common.Events
{
    public struct PlayerLeftAreaEvent
    {
        public AreaID AreaId;
        public int CharacterCount; // How many characters are still there
    }
}