using ProjectZ.Code.Runtime.Map;

namespace ProjectZ.Code.Runtime.Common.Events
{
    public struct PlayerEnteredAreaEvent
    {
        public AreaID AreaID;
        public int CharacterCount;
    }
}