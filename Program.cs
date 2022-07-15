using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Converter;
using System.Diagnostics;

class GetVideo {

    static async Task Main() {
        var youtube = new YoutubeClient();
        Console.Write("Paste the link: ");
        string? link = Console.ReadLine();
        if (link != null) { 
            try {
                Console.Clear();
                await CheckLink(link, youtube);
            }
            catch (Exception error) {
                Console.ResetColor();
                Console.CursorVisible = true;
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error.Message);
                Console.ResetColor();
                await Main();
            }
        }
        Process.Start("explorer", "downloads");
        Console.Clear();
        Console.Write("Continue? (Y/N): ");
        string? response = Console.ReadLine()?.ToUpper();
        if (response == "Y") {
            Console.Clear();
            await Main();
        }
        Process.GetCurrentProcess().Kill();
    }

    static async Task CheckLink(string link, YoutubeClient youtube) {
        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(link);
        var video = await youtube.Videos.GetAsync(link);
        await GetQualities(streamManifest, youtube, video.Title);
    }

    static async Task GetQualities(StreamManifest streamManifest, YoutubeClient youtube, string name) {
        var videoStreams = streamManifest.GetVideoStreams();
        var qualities = new List<string>();
        foreach (var videoStream in videoStreams) {
            if (!qualities.Contains(videoStream.VideoQuality.Label)) {
                qualities.Add(videoStream.VideoQuality.Label);
            }
        }
        var sortedQualities = qualities.OrderByDescending(e => Convert.ToInt32(e.Replace("p", ""))).ToList();
        Console.Clear();
        await GetQuality(sortedQualities, name, streamManifest, youtube);
    }

    static async Task GetQuality(List<string> qualitites, string name, StreamManifest streamManifest, YoutubeClient youtube) {
        for (int count = 0; count < qualitites.Count; count++) {
            Console.WriteLine($"{count+1} - {qualitites[count]}");
        }
        Console.Write("Choose quality: ");
        int quality = Convert.ToInt32(Console.ReadLine());
        if (quality < 1 || quality > qualitites.Count) {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid number");
            Console.ResetColor();
            await GetQuality(qualitites, name, streamManifest, youtube);
        }
        Console.Clear();
        await Download(qualitites[quality-1], name, streamManifest, youtube);
    }

    static async Task Download(string quality, string name, StreamManifest streamManifest, YoutubeClient youtube) {
        var videoStreamInfo = streamManifest.GetVideoStreams().First(e => e.VideoQuality.Label == quality);
        var audioStreamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();
        var streamInfos = new IStreamInfo[] { videoStreamInfo, audioStreamInfo };
        if (!Directory.Exists(@"downloads")) Directory.CreateDirectory(@"downloads");
        Console.CursorVisible = false;
        Console.Write("Progress: ");
        Console.BackgroundColor = ConsoleColor.White;
        Console.Write("                                                                                                    ");
        Console.BackgroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(10, 0);
        int lastPercent = 0;
        var progress = new Progress<double>(p => {
            var percent = Convert.ToInt32(p * 100);;
            if (percent != lastPercent) {
                Console.Write(" ");
                lastPercent = percent;
            }
            if (percent == 100) {
                Console.ResetColor();
                Console.CursorVisible = true;
            }
        });
        await youtube.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder($@"downloads\{name}.mp4").Build(), progress);
    }
}