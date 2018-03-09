namespace TechDevs.Core.UserManagement.Utils
{
    public interface IStringNormaliser
    {
        string Normalise(string str);
    }

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
