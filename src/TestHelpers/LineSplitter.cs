using System.Text;

namespace StefanStolz.TestHelpers;

internal static class LineSplitter
{
    public static LineSplitterResult Execute(string input)
    {
        return new LineSplitterResult(Enumerate(input));
    }

    private static IEnumerable<LineSplitterItem> Enumerate(string input)
    {
        LineSplitterKind? currentKind = null;
        var queue = new Queue<char>(input);

        var resultBuilder = new StringBuilder();

        while (queue.Count > 0)
        {
            var previewChar = queue.Peek();
            LineSplitterKind charKind =
                IsLineEndingCharacter(previewChar) ? LineSplitterKind.LineEnding : LineSplitterKind.Text;

            if (currentKind.HasValue)
            {
                if (currentKind.Value == charKind)
                {
                    resultBuilder.Append(queue.Dequeue());
                }
                else
                {
                    yield return new LineSplitterItem(resultBuilder.ToString(), currentKind.Value);
                    resultBuilder.Clear();
                    currentKind = null;
                }
            }
            else
            {
                currentKind = charKind;
                resultBuilder.Append(queue.Dequeue());
            }
        }

        if (currentKind.HasValue)
        {
            yield return new LineSplitterItem(resultBuilder.ToString(), currentKind.Value);
        }
    }

    private static bool IsLineEndingCharacter(char c)
    {
        return c is '\r' or '\n';
    }
}