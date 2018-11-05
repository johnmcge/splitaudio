using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace splitaudio
{
    class Program
    {
        public static string SuffixVidNoAudio = "-an";
        public static string SuffixAudioNoVid = "-audio";

        static void Main(string[] args)
        {
            string usageString = GetUsageString();
            if (args.Length < 1)
            {
                Console.WriteLine("too few arguments, need at least one video file.");
                Console.WriteLine(Environment.NewLine + usageString + Environment.NewLine);
                return;
            }

            int i = 0;
            foreach (string fileToProcess in args)
            {
                i++;
                if (!File.Exists(fileToProcess))
                    Console.WriteLine(Environment.NewLine + "File not found: " + fileToProcess);
                else
                {
                    Console.WriteLine("  Processing " + i.ToString() + " of " + args.Length.ToString() + " videos");
                    ProcessVideoFile(fileToProcess);
                }
            }
        }


        public static void ProcessVideoFile(string fileName)
        {
            int idx = fileName.LastIndexOf('.');
            string ext = fileName.Substring((idx), (fileName.Length - idx));

            if (ext.ToLower() != ".mp4")
            {
                Console.WriteLine(Environment.NewLine + "only mp4 supported at this time. File skipped: " + fileName);
                return;
            }

            string outputFNAudio = fileName.Substring(0, idx) + SuffixAudioNoVid + ext;
            string outputFNVideo = fileName.Substring(0, idx) + SuffixVidNoAudio + ext;

            // Extract audio
            // ffmpeg -i input-video.avi -vn -acodec copy output-audio.aac
            // Read the output to see what codec it is, to set the right filename extension

            StringBuilder cmdText = new StringBuilder();

            cmdText.Append(" -i " + fileName);
            cmdText.Append(" -vn");
            cmdText.Append(" -acodec copy");
            cmdText.Append(" " + outputFNAudio);

            Console.WriteLine("  " + DateTime.Now.ToString("hh':'mm':'ss") + "  Making Audio Stream: " + outputFNAudio);

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "ffmpeg.exe";
                process.StartInfo.Arguments = cmdText.ToString();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();
                process.Dispose();
            }


            // strip audio from video
            // ffmpeg -i example.mkv -c copy -an example-nosound.mkv
            cmdText.Clear();
            cmdText.Append(" -i " + fileName);
            cmdText.Append(" -c copy");
            cmdText.Append(" -an");
            cmdText.Append(" " + outputFNVideo);

            Console.WriteLine("  " + DateTime.Now.ToString("hh':'mm':'ss") + "  Stripping Audio: " + outputFNVideo);

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "ffmpeg.exe";
                process.StartInfo.Arguments = cmdText.ToString();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();
                process.Dispose();
            }

            cmdText.Clear();
            Console.WriteLine("  " + DateTime.Now.ToString("hh':'mm':'ss") + "  done" + Environment.NewLine);

        }


        public static string GetUsageString()
        {
            StringBuilder sbUsage = new StringBuilder();
            sbUsage.Append(Environment.NewLine);
            sbUsage.Append(" splitaudio file1 file2 file3");
            sbUsage.Append(Environment.NewLine);
            sbUsage.Append(" you can list as many files as you like, it will do them one at a time.");
            sbUsage.Append(Environment.NewLine);
            return sbUsage.ToString();
        }
    }
}
