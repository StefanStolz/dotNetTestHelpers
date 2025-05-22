namespace StefanStolz.TestHelpers
{
    public abstract class LineEnding
    {
        private sealed class TwoCharLineEnding : LineEnding
        {
            private readonly char first;
            private readonly char second;

            public TwoCharLineEnding(char first, char second)
            {
                this.first = first;
                this.second = second;
            }

            public override bool IsStartCharacter(char character)
            {
                return character == this.first;
            }

            public override bool IsComplete(string text)
            {
                return text.Length == 2 && text[0] == this.first && text[1] == this.second;
            }

            public override bool IsValidNextChar(char c, string text)
            {
                return text.Length == 1 && text[0] == this.first && c == this.second;
            }
        }

        private sealed class SingleCharLineEnding : LineEnding
        {
            private readonly char character;

            public SingleCharLineEnding(char character)
            {
                this.character = character;
            }

            public override bool IsStartCharacter(char character)
            {
                return character == this.character;
            }

            public override bool IsComplete(string text)
            {
                return text.Length == 1 && text[0] == this.character;
            }

            public override bool IsValidNextChar(char c, string text)
            {
                return false;
            }
        }

        public static LineEnding Windows { get; } = new TwoCharLineEnding('\r', '\n');
        public static LineEnding Unix { get; } = new SingleCharLineEnding('\n');
        public static LineEnding Mac { get; } = new SingleCharLineEnding('\r');

        public static LineEnding FromEnvironment()
        {
            switch (Environment.NewLine.Length)
            {
                case 1:
                    return new SingleCharLineEnding(Environment.NewLine[0]);
                case 2:
                    return new TwoCharLineEnding(Environment.NewLine[0], Environment.NewLine[1]);
                default:
                    throw new NotSupportedException("Unknown ending character count");
            }
        }

        public abstract bool IsStartCharacter(char character);
        public abstract bool IsComplete(string text);
        public abstract bool IsValidNextChar(char c, string text);
    }
}