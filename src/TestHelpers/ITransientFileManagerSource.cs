namespace StefanStolz.TestHelpers;

public interface ITransientFileManagerSource
{
    string FileName { get; }
    Stream GetDataStream();
}