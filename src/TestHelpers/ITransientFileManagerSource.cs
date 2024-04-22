namespace StefanStolz.TestHelpers;

public interface ITransientFileManagerSource
{
    Stream GetDataStream();
    string FileName { get; }
}