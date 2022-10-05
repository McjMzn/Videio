namespace Videio.FFmpeg;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Videio.FFmpeg.Enums;

public record FfmpegMapSelection
{
    public bool IsPositive { get; set; }
    public uint InputIndex { get; set; }
    public uint? StreamIndex { get; set; }
    public StreamType? Stream { get; set; }
    public string LanguageCode { get; set; }

    public override string ToString()
    {
        var argument = new StringBuilder("-map ");
        argument.Append(this.IsPositive ? $"{this.InputIndex}" : $"-{this.InputIndex}");

        if (this.StreamIndex is not null)
        {
            argument.Append($":{this.StreamIndex}");
        }

        if (this.Stream is StreamType streamType and not StreamType.Any)
        {
            argument.Append($":{this.GetStreamSelector(streamType)}");
        }

        if (this.LanguageCode is not null)
        {
            argument.Append($":m:language:{this.LanguageCode}");
        }

        return argument.ToString();
    }

    private string GetStreamSelector(StreamType streamType)
    {
        return
            streamType switch
            {
                StreamType.Any => string.Empty,
                StreamType.Video => ":v",
                StreamType.Audio => ":a",
                StreamType.Subtitle => ":s",
                StreamType.Data => ":d",
                StreamType.Attachements => ":a",
            };
    }
}

public class FfmpegMap
{
    private uint inputIndex;

    private List<FfmpegMapSelection> selections = new List<FfmpegMapSelection>();

    internal FfmpegMap(uint inputIndex)
    {
        this.inputIndex = inputIndex;
    }

    private FfmpegMap AddSelection(FfmpegMapSelection selection)
    {
        this.selections.RemoveAll(s =>
            s.LanguageCode == selection.LanguageCode &&
            s.Stream == selection.Stream &&
            s.InputIndex == selection.InputIndex &&
            s.StreamIndex == selection.StreamIndex
        );

        this.selections.Add(selection);

        return this;
    }

    public FfmpegMap SelectAll() => this.AddSelection(new FfmpegMapSelection { IsPositive = true, InputIndex = this.inputIndex });

    public FfmpegMap SelectAll(StreamType streamType) => this.AddSelection(new FfmpegMapSelection { IsPositive = true, InputIndex = this.inputIndex, Stream = streamType });

    public FfmpegMap Select(uint streamIndex) => this.AddSelection(new FfmpegMapSelection { IsPositive = true, InputIndex = this.inputIndex, StreamIndex = streamIndex });

    public FfmpegMap DeselectAll() => this.AddSelection(new FfmpegMapSelection { IsPositive = false, InputIndex = this.inputIndex });

    public FfmpegMap DeselectAll(StreamType streamType) => this.AddSelection(new FfmpegMapSelection { IsPositive = false, InputIndex = this.inputIndex, Stream = streamType });

    public FfmpegMap Deselect(uint streamIndex) => this.AddSelection(new FfmpegMapSelection { IsPositive = false, InputIndex = this.inputIndex, StreamIndex = streamIndex });

    public FfmpegMap SelectSubtitles(string languageCode) => this.AddSelection(new FfmpegMapSelection { IsPositive = true, InputIndex = this.inputIndex, LanguageCode = languageCode });
    
    public FfmpegMap DeselectSubtitles(string languageCode) => this.AddSelection(new FfmpegMapSelection { IsPositive = false, InputIndex = this.inputIndex, LanguageCode = languageCode });

    public FfmpegMap Toggle(FfmpegMapSelection selection)
    {
        if(!this.selections.Remove(selection))
        {
            this.selections.Add(selection);
        }

        return this;
    }

    public string Build()
    {
        return string.Join(' ', this.selections.Select(s => s.ToString()));
    }
}
