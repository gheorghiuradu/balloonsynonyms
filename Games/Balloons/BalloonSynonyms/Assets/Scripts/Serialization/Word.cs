using System;

namespace Assets.Scripts.Serialization
{
    [Serializable]
    public class Word : IEquatable<Word>
    {
        public int Id { get; set; }
        public string Key;
        public string Value;
        public Difficutly Difficutly;

        public bool Equals(Word other)
        {
            return this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Word);
        }

        public static bool operator ==(Word first, Word second)
        {
            return first.Id == second.Id;
        }

        public static bool operator !=(Word first, Word second)
        {
            return first.Id != second.Id;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }
    }

    public enum Difficutly
    {
        VeryEasy = 0,
        Easy = 1,
        Medium = 2,
        Hard = 3,
        VeryHard = 4
    }
}