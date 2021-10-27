namespace TTTReborn.Globalization
{
    public class TranslationData
    {
        public string Key;
        public object[] Data;

        public TranslationData(string translationKey, params object[] translationData)
        {
            Key = translationKey;
            Data = translationData;
        }
    }
}
