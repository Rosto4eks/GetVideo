using VideoLibrary;
using System.Diagnostics;
using FFMpegCore;

class GetVideo {
    static void Main() {    
        Console.Write("Paste the link: ");
        string? link = Console.ReadLine();
        if (!Directory.Exists(@"downloads\parts")) Directory.CreateDirectory(@"downloads\parts");
        DirectoryInfo directory = new DirectoryInfo(@".\downloads\");
        YouTube youtube = new YouTube();
        IEnumerable<YouTubeVideo> videos = youtube.GetAllVideos(link);
        ChooseQuality(directory, videos);
    }

    static List<int> GetQualities(IEnumerable<YouTubeVideo> videos) {
        List<int> qualities = new List<int>();
        foreach (YouTubeVideo video in videos) {
            if (!qualities.Contains(video.Resolution) && video.Resolution >= 144) qualities.Add(video.Resolution);
        }
        List<int> sorted = qualities.OrderByDescending(e => e).ToList();
        return sorted;
    }

    static void ChooseQuality(DirectoryInfo directory, IEnumerable<YouTubeVideo> videos) {
        List<int> qualities = GetQualities(videos);
        Console.WriteLine("Choose the quality:");
        Console.ForegroundColor = ConsoleColor.Yellow;
        for (int e = 0; e < qualities.Count; e++) {
            Console.WriteLine($"{e + 1} - {qualities[e]}");
        }
        Console.ResetColor();
        Console.Write("Enter the number: ");
        int quality = 1;
        try {
             quality = Convert.ToInt32(Console.ReadLine());
        }
        catch {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Enter the correct number");
            Console.ResetColor();
            Thread.Sleep(1000);
            ChooseQuality(directory, videos);
        }
        if (quality < 1 || quality > qualities.Count) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Enter the correct number");
            Console.ResetColor();
            Thread.Sleep(1000);      
            ChooseQuality(directory, videos);
        }
        Console.WriteLine($"Choosed quality: {qualities[quality - 1]}");
        YouTubeVideo video = videos.Where(e =>
            e.Format == VideoFormat.Mp4 && 
            e.Resolution == qualities[quality-1] && 
            e.AudioFormat == AudioFormat.Unknown
        ).First();
        Download(videos, directory, video);
    }

    static YouTubeVideo GetAudio(IEnumerable<YouTubeVideo> videos) {
        YouTubeVideo video = videos.Where(e => e.AdaptiveKind == AdaptiveKind.Audio).First();
        return video;
    }

    static void Download(IEnumerable<YouTubeVideo> videos, DirectoryInfo directory, YouTubeVideo video) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Downloading video...");
        File.WriteAllBytes($@"{directory}parts\{video.FullName}", video.GetBytes());
        YouTubeVideo audio = GetAudio(videos);
        Console.WriteLine("Downloading audio...");
        File.WriteAllBytes($@"{directory}parts\{audio.FullName}.mp3", audio.GetBytes());
        Console.WriteLine("merging video and audio...");
        Console.ResetColor();
        FFMpeg.ReplaceAudio($@"{directory}parts\{video.FullName}", $@"{directory}parts\{audio.FullName}.mp3", $"{directory}{video.FullName}");
        File.Delete($@"{directory}parts\{video.FullName}");
        File.Delete($@"{directory}parts\{audio.FullName}.mp3");
        Console.WriteLine("Complete");
        Continue();
    }

    static void Continue() {
        Console.WriteLine("continue? (Y/N)");
        string? answer = Console.ReadLine()?.ToUpper();
        if (answer == "Y") Main();
        else if (answer == "N") Process.GetCurrentProcess().Kill();
        else {
            Console.WriteLine("Incorrect input");
            Thread.Sleep(1000);
            Continue();
        }
    }

}