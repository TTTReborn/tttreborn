namespace TTTReborn.Globalization
{
    public class TranslationData
    {
        public string Key;
        public object[] Args;

        public TranslationData(string translationKey = null, params object[] args)
        {
            Key = translationKey ?? string.Empty;
            Args = args ?? System.Array.Empty<object>();
        }
    }
}
