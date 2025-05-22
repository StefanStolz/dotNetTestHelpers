namespace StefanStolz.TestHelpers;

public sealed class StringComparerBuilder
{
    private bool trimLines = false;
    private bool ignoreLineEndings = false;

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
            var detectLineEndings = new DetectLineEnding();

            var le = detectLineEndings.DetectFromString(input);

            var lineEnding = le switch
            {
                LineEndingKind.Windows => LineEnding.Windows,
                LineEndingKind.Unix => LineEnding.Unix,
                LineEndingKind.Mac => LineEnding.Mac,
                _=> LineEnding.FromEnvironment()
            };

            var lineSplitter = new LineSplitter(lineEnding);
            var parts = lineSplitter.Execute(input);
            var transformed = parts.TransformTextItems(t=>t.Trim());
            return transformed.ToString();
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