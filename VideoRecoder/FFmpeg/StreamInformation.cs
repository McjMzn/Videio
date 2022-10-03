using VideIO.FFmpeg.Enums;

namespace VideIO.FFmpeg;

public record StreamInformation
{
    public uint InputIndex { get; set; }
    public uint? StreamIndex { get; set; }
    public StreamType StreamType { get; set; }
    public string LanguageCode { get; set; }
    public string Format { get; set; }
    public string Title { get; set; }

    public override string ToString()
    {
        var streamIndex = this.StreamIndex is null ? string.Empty : $":{this.StreamIndex}";
        var languageCode = this.LanguageCode is null ? string.Empty : $"({this.LanguageCode})";
        return $"#{this.InputIndex}{streamIndex}{languageCode} {this.StreamType}";
    }
}
