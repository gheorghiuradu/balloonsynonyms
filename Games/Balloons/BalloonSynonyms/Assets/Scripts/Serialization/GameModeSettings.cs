using System;

namespace Assets.Scripts.Serialization
{
    [Serializable]
    public class GameModeSettings
    {
        public string GameMode;
        public LevelSettings[] Levels;
        public string Dictionary;
        public string Background;
        public string LevelType;
        public LevelType LevelTypeEnum => (LevelType)Enum.Parse(typeof(LevelType), this.LevelType);
    }

    [Serializable]
    public enum LevelType
    {
        Balloon,
        Cookie
    }
}