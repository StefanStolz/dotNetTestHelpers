namespace StefanStolz.TestHelpers;

internal sealed class LineSplitterItem
{
    public string Value { get; }
    public LineSplitterKind Kind { get; }

    public LineSplitterItem(string value, LineSplitterKind kind)
    {
        this.Value = value;
        this.Kind = kind;
    }
}