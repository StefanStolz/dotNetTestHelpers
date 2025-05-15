using System.Text;

namespace StefanStolz.TestHelpers;

internal static class LineSplitter
{
    public static IEnumerable<StringTokenizerResult> Execute(string input)
    {
        StringTokenKind? currentKind = null;
        var queue = new Queue<char>(input);

        var resultBuilder = new StringBuilder();

        while (queue.Count > 0)
        {
            var previewChar = queue.Peek();
            StringTokenKind charKind =
                IsLineEndingCharacter(previewChar) ? StringTokenKind.LineEnding : StringTokenKind.Text;

            if (currentKind.HasValue)
            {
                if (currentKind.Value == charKind)
                {
                    resultBuilder.Append(queue.Dequeue());
                }
                else
                {
                    yield return new StringTokenizerResult(resultBuilder.ToString(), currentKind.Value);
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
            yield return new StringTokenizerResult(resultBuilder.ToString(), currentKind.Value);
        }
    }

    private static bool IsLineEndingCharacter(char c)
    {
        return c is '\r' or '\n';
    }
}

internal enum StringTokenKind
{
    Text,
    LineEnding
}

internal sealed class StringTokenizerResult
{
    public string Value { get; }
    public StringTokenKind Kind { get; }

    public StringTokenizerResult(string value, StringTokenKind kind)
    {
        this.Value = value;
        this.Kind = kind;
    }
}

public sealed class StringComparerBuilder
{
    private bool trimLines = false;
    private bool ignoreLineEndings = false;

    public static StringComparerBuilder Create()
    {
        return new StringComparerBuilder();
    }


    public StringComparerBuilder IgnoreLineEndings()
    {
        this.ignoreLineEndings = true;
        return this;
    }

    public StringComparerBuilder TrimLines()
    {
        this.trimLines = true;
        return this;
    }

    public IEqualityComparer<string> Build()
    {
        var preProcessors = new List<IStringPreprocessor>();

        if (this.trimLines)
        {
            preProcessors.Add(new TrimLinesStringProcessor());
        }

        if (this.ignoreLineEndings)
        {
            preProcessors.Add(new IgnoreLineEndingsStringPreprocessor());
        }

        return new Comparer(preProcessors);
    }

    private sealed class TrimLinesStringProcessor : IStringPreprocessor
    {
        public string Preprocess(string input)
        {
            using var stringReader = new StringReader(input);
            var lines = new List<string>();
            while (stringReader.ReadLine() is { } line)
            {
                lines.Add(line.Trim());
            }

            return string.Join(Environment.NewLine, lines);
        }
    }

    private sealed class IgnoreLineEndingsStringPreprocessor : IStringPreprocessor
    {
        public string Preprocess(string input)
        {
            using var stringReader = new StringReader(input);
            string? line;
            var lines = new List<string>();
            while ((line = stringReader.ReadLine()) is not null)
            {
                lines.Add(line);
            }

            return string.Join(Environment.NewLine, lines);
        }
    }

    private interface IStringPreprocessor
    {
        string Preprocess(string input);
    }

    private sealed class Comparer : IEqualityComparer<string>
    {
        private readonly IReadOnlyList<IStringPreprocessor> preprocessors;

        public Comparer(IEnumerable<IStringPreprocessor> preprocessors)
        {
            this.preprocessors = preprocessors.ToList().AsReadOnly();
        }

        public bool Equals(string? x, string? y)
        {
            switch ((x, y))
            {
                case (null, null):
                    return true;
                case (_, null):
                case (null, _):
                    return false;
                default:
                {
                    foreach (IStringPreprocessor stringPreprocessor in this.preprocessors)
                    {
                        x = stringPreprocessor.Preprocess(x);
                        y = stringPreprocessor.Preprocess(y);
                    }

                    return x.Equals(y);
                }
            }
        }

        public int GetHashCode(string obj)
        {
            var value = obj;

            foreach (IStringPreprocessor stringPreprocessor in this.preprocessors)
            {
                value = stringPreprocessor.Preprocess(value);
            }

            return value.GetHashCode();
        }
    }
}