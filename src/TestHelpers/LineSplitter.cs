using System.Text;

namespace StefanStolz.TestHelpers;

internal class LineSplitter
{
    private readonly LineEnding lineEnding;

    public LineSplitter(LineEnding lineEnding)
    {
        this.lineEnding = lineEnding;
    }

    public LineSplitterResult Execute(string input)
    {
        return new LineSplitterResult(Enumerate(input));
    }

    private IEnumerable<LineSplitterItem> Enumerate(string input)
    {
        var queue = new Queue<char>(input);

        var resultBuilder = new StringBuilder();

        while (queue.Count > 0)
        {
            var previewChar = queue.Peek();

            if (this.lineEnding.IsStartCharacter(previewChar))
            {
                if (resultBuilder.Length > 0)
                {
                    yield return new LineSplitterItem(resultBuilder.ToString(), LineSplitterKind.Text);
                    resultBuilder.Clear();
                }

                resultBuilder.Append(queue.Dequeue());
                while (!this.lineEnding.IsComplete(resultBuilder.ToString()))
                {
                    previewChar = queue.Peek();
                    if (this.lineEnding.IsValidNextChar(previewChar, resultBuilder.ToString()))
                    {
                        resultBuilder.Append(queue.Dequeue());
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid line ending");
                    }
                }

                yield return new LineSplitterItem(resultBuilder.ToString(), LineSplitterKind.LineEnding);
                resultBuilder.Clear();
            }
            else
            {
                resultBuilder.Append(queue.Dequeue());
            }
        }

        if (resultBuilder.Length > 0)
        {
            yield return new LineSplitterItem(resultBuilder.ToString(), LineSplitterKind.Text);
        }
    }
}