namespace TechDevs.Accounts
{

    public class LowerStringNormaliser : IStringNormaliser
    {
        public string Normalise(string str)
        {
            return str.ToLowerInvariant();
        }
    }

    public class UpperStringNormaliser : IStringNormaliser
    {
        public string Normalise(string str)
        {
            return str.ToUpperInvariant();
        }
    }
}
