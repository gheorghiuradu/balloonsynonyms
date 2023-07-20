namespace Assets.Scripts.Serialization
{
    [System.Serializable]
    public class LevelSettings
    {
        public int Level;
        public float BalloonSpeed;
        public WordPick[] Words;
        public WordPick[] FakeWords;
    }
}