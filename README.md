.NET application, which helps you to download video from youtube with choosed quality
# How it works?
choose quality of video
video and audio are downloaded separately due to the specifics of YouTube, then merging into 1 .mp4 file
# How to use it?
at first, build and run application
```console 
$ dotnet run
```
then, paste the video link
```console 
? paste the link: https://www.youtube.com/watch?v=mkggXE5e2yk&t=32s
```
```console
√ paste the link: ... https://www.youtube.com/watch?v=mkggXE5e2yk&t=32s
? choose the quality: 
>   720p
    480p
    360p
    240p
```
after choosing video will be downloading 
# important
you must have ffmpeg.exe and ffprobe.exe in the same folder
