using System.Text;

namespace StefanStolz.TestHelpers;

public class RandomNameGenerator
{
    private readonly Random random = new();

    public string GetRandomName(int length)
    {
        StringBuilder result = new StringBuilder(length);
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        for (int i = 0; i < length; i++)
        {
            int index = this.random.Next(characters.Length);
            result.Append(characters[index]);
        }

        result.Append(DateTime.Now.Ticks.ToString("X8").TrimStart('0'));

        return result.ToString();
    }
}