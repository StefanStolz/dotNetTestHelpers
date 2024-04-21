namespace StefanStolz.TestHelpers;

public interface ITempFileManagerSource
{
    Stream GetDataStream();
    string FileName { get; }
}