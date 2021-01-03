using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Xabe.FFmpeg;

namespace Video_Transcode
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			SourceFiles = new List<string>();
			SourceFiles.Add("c:\\aaa\\DJI_0052.MP4");

			ProcessOutput = new List<string>();
		}

		List<string> SourceFiles { get; }
		List<string> ProcessOutput { get; }

		const string FFMPEG_PATH = "C:\\aaa\\ffmpeg\\bin\\";

		private async void Start_Click(object sender, RoutedEventArgs e)
		{
			foreach (var sourceFile in SourceFiles)
			{
				var res = "1080";
				var destFile = Path.Combine(Path.GetDirectoryName(sourceFile), $"{Path.GetFileNameWithoutExtension(sourceFile)}x{res}{Path.GetExtension(sourceFile)}");
				//var logProcessResults = await ProcessEx.RunAsync(FFMPEG_PATH, $"-y -vsync 0 -hwaccel cuda -hwaccel_output_format cuda -i \"{sourceFile}\" -vf scale_cuda=-1:{res} -c:a copy -c:v h264_nvenc -b:v 5M \"{destFile}\"");
				//if (logProcessResults.ExitCode != 0)
				//{
				//	return;
				//}

				var processInfo = new ProcessStartInfo(FFMPEG_PATH, $"-y -vsync 0 -hwaccel cuda -hwaccel_output_format cuda -i \"{sourceFile}\" -vf scale_cuda=-1:{res} -c:a copy -c:v h264_nvenc -b:v 5M \"{destFile}\"");
				processInfo.UseShellExecute = false;
				processInfo.CreateNoWindow = true;

				using (var process = new Process())
				{
					process.StartInfo = processInfo;
					process.Start();
					process.BeginOutputReadLine();
				}
			}
		}

		private async void Start2_Click(object sender, RoutedEventArgs e)
		{
			FFmpeg.SetExecutablesPath(FFMPEG_PATH);

			foreach (var sourceFile in SourceFiles)
			{
				string outputFileName = Path.Combine(Path.GetDirectoryName(sourceFile), $"{Path.GetFileNameWithoutExtension(sourceFile)}x{nameof(VideoSize.Hd1080)}{Path.GetExtension(sourceFile)}");
				string res = "1080";
				//var conversion = await FFmpeg.Conversions.FromSnippet.ChangeSize(sourceFile, outputFileName, VideoSize.Hd1080);
				var conversion = FFmpeg.Conversions.New();
				//conversion.SetOverwriteOutput(true);
				conversion.OnProgress += Conversion_OnProgress;
				conversion.OnDataReceived += Conversion_OnDataReceived;
				await conversion.Start($"-y -vsync 0 -hwaccel cuda -hwaccel_output_format cuda -i \"{sourceFile}\" -vf scale_cuda=-1:{res} -c:a copy -c:v h264_nvenc -b:v 5M \"{outputFileName}\"");
				Debug.WriteLine($"Finished converion file [{sourceFile}]");
			}
		}

		private void Conversion_OnDataReceived(object sender, DataReceivedEventArgs e)
		{
			Debug.WriteLine(e.Data);
		}

		private void Conversion_OnProgress(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs args)
		{
			Debug.WriteLine($"Percent Complete - {args.Percent}");
		}
	}
}
