namespace DTS.Common;

public static class TrackingCodeGenerator
{
    public static string Generate()
    {
        const string alphabetChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        const string numericChars = "0123456789";

        Random random = new Random();
        string trackingCode = new string(Enumerable.Repeat(alphabetChars, 5)
            .Concat(Enumerable.Repeat(numericChars, 5))
            .Select(s => s[random.Next(s.Length)]).ToArray());

        return trackingCode.ToUpper();
    }
}