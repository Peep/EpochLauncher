using System;
using Newtonsoft.Json;

namespace EpochLauncher
{
    public class Version : IEquatable<Version>, IComparable<Version>
    {
        private readonly int _release;
        public int Release { get { return _release; } }

        private readonly int _subversion;
        public int Subversion { get { return _subversion; } }

        private readonly int _build;
        public int Build { get { return _build; }}

        public Version(string versionString)
        {
            var numbers = versionString.Split(new[] {'.'}, 3);
            if (numbers.Length > 3 || numbers.Length < 1)
                throw new FormatException("Could not parse version number. ");

            if (!int.TryParse(numbers[0], out _release))
                throw new FormatException("The first octet was not an integer.");
            if (!int.TryParse(numbers[1], out _subversion)) 
                throw new FormatException("The second octet was not an integer.");
            if (!int.TryParse(numbers[2], out _build)) 
                throw new FormatException("The third octet was not an integer.");
        }

        [JsonConstructor]
        public Version(int release, int subversion, int build)
        {
            _release = release;
            _subversion = subversion;
            _build = build;
        }

        public bool Equals(Version other)
        {
            return other.Release == Release
                && other.Subversion == Subversion
                && other.Build == Build;
        }

        public int CompareTo(Version other)
        {
            if (other.Release - this.Release > 0) return 1;
            if (other.Release - this.Release < 0) return -1;

            if (other.Subversion - this.Subversion > 0) return 1;
            if (other.Subversion - this.Subversion < 0) return -1;

            if (other.Build - this.Build > 0) return 1;
            if (other.Build - this.Build < 0) return -1;

            return 0;
        }

        public static bool operator < (Version first, Version second)
        {
            return first.CompareTo(second) < 0;
        }

        public static bool operator > (Version first, Version second)
        {
            return first.CompareTo(second) > 0;
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}.{2}", Release, Subversion, Build);
        }
    }
}
