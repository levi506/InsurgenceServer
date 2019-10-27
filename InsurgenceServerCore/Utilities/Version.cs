namespace InsurgenceServerCore.Utilities
{
    public struct Version
    {
        public readonly byte Major;
        public readonly byte Minor;

        public Version(byte major, byte minor)
        {
            Major = major;
            Minor = minor;
        }

        public static bool TryParse(string s, out Version b)
        {
            b = default;
            if (string.IsNullOrWhiteSpace(s))
                return false;
            var split = s.Split(".");
            if (split.Length != 2) return false;
            if (!byte.TryParse(split[0], out var major))
            {
                return false;
            }
            if (!byte.TryParse(split[1], out var minor))
            {
                return false;
            }
            b = new Version(major, minor);
            return true;
        }
    }
}