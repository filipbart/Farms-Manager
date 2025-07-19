#nullable enable
namespace FarmsManager.Application.FileSystem;

public class FileModel
{
    /// <summary>
    /// Nazwa pliku
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Czy jest plikiem
    /// </summary>
    public bool IsFile { get; set; }

    /// <summary>
    /// Czy jest folderem
    /// </summary>
    public bool IsDirectory { get; set; }

    /// <summary>
    /// Data utworzenia pliku
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Data ostatniej modyfikacji pliku
    /// </summary>
    public DateTime? LastModifyDate { get; set; }

    /// <summary>
    /// Content-type pliku
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// Dane pliku
    /// </summary>
    public byte[]? Data { get; set; }

    /// <summary>
    /// Czy plik posiada jakiekolwiek dane
    /// </summary>
    public bool HasData => Data?.LongLength > 0;
}