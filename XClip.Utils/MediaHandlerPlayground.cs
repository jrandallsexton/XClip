using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XClip.Utils
{
    public class MediaHandlerPlayground
    {
        //private string DIR_FFMPEG = @"C:\ffmpeg-20131202-git-e3d7a39-win64-static\bin\ffmpeg.exe";
        private string DIR_FFMPEG = @"C:\ffmpeg-20160703-d5edb6c-win64-static\bin\ffmpeg.exe";

        public void ProcessOpenBatches()
        {
            var batchMgr = new BatchManager();

            const int userId = 1;
            const int collectionId = 2;

            var openBatches = batchMgr.GetOpen(userId, collectionId);

            //openBatches.ForEach(x =>
            //{
            //    Console.WriteLine(x.Filename);
            //});

            var mhandler = new MediaHandler {FFMPEGPath = DIR_FFMPEG};

            foreach (var batch in openBatches)
            {
                #region process batches
                var args = new StringBuilder();
                args.AppendFormat("-i {0}.mp4 -i c:\\watermark.png -filter_complex \"overlay=x=(main_w-overlay_w)-25:y=(main_h-overlay_h)-25\"",
                    Path.Combine(@"H:\XClip\Collections\2", batch.SrcId.ToString()));

                for (var i = 0; i < batch.Items.Count; i++)
                {

                    var ci = batch.Items[i];

                    var outputMask = batch.SrcId + "_{0}.mp4";

                    var outFile = string.Format(outputMask, i);
                    var outputDir = $@"H:\XClip\Collections\{collectionId}";

                    outFile = Path.Combine(outputDir, outFile);

                    if (File.Exists(outFile)) { File.Delete(outFile); }

                    args.Append($" -ss {ci.Start} -t {ci.Duration} {outFile}");

                }

                mhandler.CustomCommand = args.ToString();

                batchMgr.MarkStarted(batch.UId);

                mhandler.Execute_FFMPEG();

                batchMgr.MarkCompleted(batch.UId, DateTime.UtcNow);
                //break;

                #endregion
            }
        }

        public void GetVideoInfo()
        {

            var _mhandler = new MediaHandler
            {
                FFMPEGPath = DIR_FFMPEG,
                InputPath = @"H:\XClip\Collections\2",
                OutputPath = @"H:\XClip\Collections\2\"
            };

            _mhandler.FileName = "test_0.mp4";
            var info = _mhandler.Get_Info();

            StringBuilder str = new StringBuilder();
            str.Append("File Name= " + info.FileName + "<br />");
            str.Append("Video Duration= " + info.Duration + "<br />");
            str.Append("Video Duration in Seconds= " + info.Duration_Sec + "<br />");
            // Input values str.Append("Input Values + "<br />");
            str.Append("Video Codec= " + info.Input_Vcodec + "<br />");
            str.Append("Audio Codec= " + info.Input_Acodec + "<br />");
            str.Append("Video Bitrate= " + info.Input_Video_Bitrate + "<br />");
            str.Append("Audio Bitrate= " + info.Input_Audio_Bitrate + "<br />");
            str.Append("Audio Sampling Rate= " + info.Input_SamplingRate + "<br />");
            str.Append("Audio Channel= " + info.Input_Channel + "<br />");
            str.Append("Width= " + info.Input_Width + "<br />");
            str.Append("Height= " + info.Input_Height + "<br />");
            str.Append("Video FrameRate= " + info.Input_FrameRate + "<br />");
            // Output values str.Append("Output Values + "<br />");
            str.Append("Video Codec= " + info.Vcodec + "<br />");
            str.Append("Audio Codec= " + info.Acodec + "<br />");
            str.Append("Video Bitrate= " + info.Video_Bitrate + "<br />");
            str.Append("Audio Bitrate= " + info.Audio_Bitrate + "<br />");
            str.Append("Audio Sampling Rate= " + info.SamplingRate + "<br />");
            str.Append("Audio Channel= " + info.Channel + "<br />");
            str.Append("Width= " + info.Width + "<br />");
            str.Append("Height= " + info.Height + "<br />");
            str.Append("Video FrameRate= " + info.FrameRate + "<br />");
            //str.Append(".................................+ " < br /> ");
            str.Append("FFMPEG Output:" + info.FFMPEGOutput + "<br />");
            str.Append("Error Code= " + info.ErrorCode + "<br />");
            Console.Write(str.ToString());
        }

        public void JoinRaw()
        {

            var _mhandler = new MediaHandler
            {
                FFMPEGPath = DIR_FFMPEG
            };

            /* Build the Input File */
            // Example content for input file:
            //  file 'test_0.mp4'
            //  file 'test_1.mp4'
            //  file 'test_2.mp4'

            var files = new List<string>();

            foreach (var fi in new DirectoryInfo(@"H:\XClip\Collections\2").GetFiles("*.mp4"))
            {

                if (!fi.Name.Contains("_"))
                    continue;

                files.Add(fi.Name);
            }

            files.Sort();

            var ffmpegInputFilename = "inputFiles.txt";
            using (var fs = new StreamWriter(Path.Combine(_mhandler.OutputPath, ffmpegInputFilename)))
            {
                foreach (var fileName in files)
                {
                    fs.WriteLine($"file '{fileName}'");
                }
                fs.Flush();
            }

            // Example command for FFMPEG: ffmpeg -f concat -i "H:/XClip/Collections/2/filelist.txt" -codec copy "H:\XClip\Collections\2\test.mp4"
            var cmd = "-f concat -i \"H:/XClip/Collections/2/inputFiles.txt\" -codec copy \"H:\\XClip\\Collections\\2\\output.mp4\"";

            _mhandler.CustomCommand = cmd;
            var result = _mhandler.Execute_FFMPEG();
            Console.WriteLine(result);
        }

        public void JoinTest()
        {

            var _mhandler = new MediaHandler
            {
                FFMPEGPath = DIR_FFMPEG,
                InputPath = @"H:\XClip\Collections\2",
                OutputPath = @"H:\XClip\Collections\2\"
            };

            var filesSubfiles = new Dictionary<string, SortedDictionary<string, bool>>();

            var totalFileCount = 0;

            foreach (var fi in new DirectoryInfo(_mhandler.InputPath).GetFiles("*.mp4"))
            {

                if (!fi.Name.Contains("_"))
                    continue;
                // example:
                // 31444cda-f781-4c99-a6ac-585f6ce7ccaa_0.mp4

                // 31444cda-f781-4c99-a6ac-585f6ce7ccaa_0.mp4
                var tmpName = fi.Name;

                // dot = 38
                var dot = tmpName.IndexOf(".");

                // underScoreLoc = 36
                var underScoreLoc = tmpName.LastIndexOf("_");

                var tmpIdx = tmpName.Substring(underScoreLoc + 1, (dot - underScoreLoc - 1));

                tmpName = fi.Name.Substring(0, underScoreLoc);
                if (!filesSubfiles.ContainsKey(tmpName)) { filesSubfiles.Add(tmpName, new SortedDictionary<string, bool>()); }

                //int fiIdx = int.Parse(tmpIdx);

                filesSubfiles[tmpName].Add(fi.Name, false);
                totalFileCount++;

                //Console.WriteLine("{0} --> {1}", tmpName, tmpIdx);
            }

            #region Pass two or more video clip names through FileNames property
            var t = new List<string>();

            var keepGoing = true;

            while (keepGoing)
            {
                foreach (var kvp in filesSubfiles)
                {
                    foreach (var subKvp in kvp.Value)
                    {
                        if (t.Contains(subKvp.Key)) { continue; }
                        t.Add(subKvp.Key);
                        break;
                    }
                }
                if (t.Count == totalFileCount) { keepGoing = false; }
            }
            #endregion

            //_mhandler.FileNames = new string[] { "sample_01.mp4", "sample_02.mp4" };
            _mhandler.FileNames = t.ToArray();

            // Set final output filename through OutputFileName property, it may be without extension.
            _mhandler.OutputFileName = "full_video";

            // Set final output extension e.g ".avi" will force output in .avi format.
            _mhandler.OutputExtension = ".mp4";
            _mhandler.VCodec = "mpeg4";

            //// Set width and height of final output.
            //_mhandler.Width = 320;
            //_mhandler.Height = 240;

            //// Set all settings that is required for encoding final output. e.g in case of .avi we set TargetFileType.
            //_mhandler.TargetFileType = "pal-vcd";

            //// Call join video function to process all settings and create output media.
            var info = _mhandler.Join_Videos();
            Console.WriteLine("{0}: {1}", info.ErrorCode, info.ErrorMessage);

        }

        public void ProcessOpenBatchesPerBatchItem()
        {
            var batchMgr = new BatchManager();

            const int userId = 1;
            const int collectionId = 2;

            var openBatches = batchMgr.GetOpen(userId, collectionId);

            var mhandler = new MediaHandler { FFMPEGPath = DIR_FFMPEG };

            foreach (var batch in openBatches)
            {

                for (var i = 0; i < batch.Items.Count; i++)
                {

                    var args = new StringBuilder();
                    args.AppendFormat("-i {0}.mp4 -i c:\\watermark.png -filter_complex \"overlay=x=(main_w-overlay_w)-25:y=(main_h-overlay_h)-25\"",
                        Path.Combine(@"H:\XClip\Collections\2", batch.SrcId.ToString()));

                    var ci = batch.Items[i];

                    var outputMask = batch.SrcId + "_{0}.mp4";

                    var outFile = string.Format(outputMask, i);
                    var outputDir = $@"H:\XClip\Collections\{collectionId}";

                    outFile = Path.Combine(outputDir, outFile);

                    if (File.Exists(outFile)) { File.Delete(outFile); }

                    args.Append($" -ss {ci.Start} -t {ci.Duration} {outFile}");

                    mhandler.CustomCommand = args.ToString();

                    batchMgr.MarkStarted(batch.UId);

                    mhandler.Execute_FFMPEG();

                }

                batchMgr.MarkCompleted(batch.UId, DateTime.UtcNow);

            }
        }
    }
}
