using System;
using System.Text;
using System.Collections.Generic;

public static class StringActions 
{
    private static readonly Random random = new Random();
    private const string chars =
        "0123456789" +
        "!@#$%^&*()_-+=<>?/{}[]|\\:;\"'.,~`";

    public static string RandomizeString(string input, int replaceCount)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        replaceCount = Math.Min(replaceCount, input.Length);
        StringBuilder result = new StringBuilder(input);
        HashSet<int> usedIndexes = new HashSet<int>();
        while (usedIndexes.Count < replaceCount)
        {
            int index = random.Next(0, input.Length);
            if (!usedIndexes.Add(index))
                continue;
            result[index] = chars[random.Next(chars.Length)];
        }

        return result.ToString();
    }
}
