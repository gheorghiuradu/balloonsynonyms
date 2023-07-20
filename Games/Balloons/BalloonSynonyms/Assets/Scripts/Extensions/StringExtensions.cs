public static class StringExtensions
{
    public static string ToUpperFirstChar(this string input)
    {
        return char.ToUpper(input[0]) + input.Substring(1);
    }
}