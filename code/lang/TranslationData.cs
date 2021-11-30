namespace TTTReborn.Globalization
{
    public class TranslationData
    {
        public string Key;
        public object[] Args;
        public bool ReturnError;

        public TranslationData(string translationKey = "", bool returnError = false, params object[] args)
        {
            Key = translationKey;
            Args = args;
            ReturnError = returnError;
        }
    }
}
