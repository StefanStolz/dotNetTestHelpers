namespace UnitTestHelpers;

public interface ITempFileManagerSource
{
    Stream GetDataStream();
    string FileName { get; }
}