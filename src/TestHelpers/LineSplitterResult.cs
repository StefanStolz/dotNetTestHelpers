using System.Collections;
using System.Collections.ObjectModel;

namespace StefanStolz.TestHelpers;

internal sealed class LineSplitterResult : IEnumerable<LineSplitterItem>
{
    private readonly ReadOnlyCollection<LineSplitterItem> items;

    public LineSplitterResult(IEnumerable<LineSplitterItem> items)
    {
        this.items = items.ToList().AsReadOnly();
    }

    public int Count => this.items.Count;

    public LineSplitterItem this[int index] => this.items[index];

    public IEnumerator<LineSplitterItem> GetEnumerator()
    {
        return this.items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public LineSplitterResult TransformTextItems(Func<string, string> transformation)
    {
        return new LineSplitterResult(this.items.Select(item =>
        {
            if (item.Kind == LineSplitterKind.Text)
            {
                return new LineSplitterItem(transformation(item.Value), LineSplitterKind.Text);
            }

            return item;
        }));
    }

    public override string ToString()
    {
        return string.Concat(this.items.Select(i => i.Value));
    }
}