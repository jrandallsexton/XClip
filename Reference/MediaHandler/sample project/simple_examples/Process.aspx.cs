using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Collections;


public partial class Process : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        MediaHandler _mhandler = new MediaHandler();

        string RootPath = Server.MapPath(Request.ApplicationPath);
        _mhandler.FFMPEGPath = HttpContext.Current.Server.MapPath("~\\ffmpeg_july_2012\\bin\\ffmpeg.exe");
        _mhandler.FLVToolPath = HttpContext.Current.Server.MapPath("~\\flvtool\\flvtool2.exe");
        _mhandler.InputPath = RootPath + "\\contents\\original";
        _mhandler.OutputPath = RootPath + "\\contents\\flv";

        //_mhandler.MP4BoxPath = RootPath + "\\theora\\ffmpeg2theora-0.27.exe";
        //_mhandler.FileName = "Recruit HD vs Wood.mp4";
        //_mhandler.OutputFileName = "Recruit HD vs Wood.ogv";
        //_mhandler.Encode_OGG();
        //// MP4 Encoding
        //_mhandler.FileName = "Wildlife.wmv"; 
        //_mhandler.OutputFileName = "Wildlife"; 
        //_mhandler.OutputExtension = ".mov";
        //_mhandler.TargetFileType = "ntsc-dvd";
        //// other settings here
        //VideoInfo info = _mhandler.Process(); 
        //_mhandler.Parameters = "-strict experimental -threads 0"; 
        //_mhandler.ACodec = "aac"; 
        //_mhandler.Audio_Bitrate = 160; 
        //_mhandler.Width = 480; 
        //_mhandler.Height = 320; 
        //_mhandler.VCodec = "libx264"; 
        //_mhandler.Video_Bitrate = 450; 
        //_mhandler.Force = "mov"; 
        
    
        //****************************
        // Example 1: FLV Encoding
        //****************************
       // _mhandler.FileName = "howto_knovialsocial.flv";
       // _mhandler.OutputFileName = "howto_knovialsocial";
       // _mhandler.OutputExtension = ".mp4";
       // _mhandler.Video_Bitrate = 512;
       // _mhandler.VCodec = "libx264";
       // _mhandler.Width = 480;
       // _mhandler.Height = 272;
       // _mhandler.AspectRatio = "480:272";
       // _mhandler.FrameRate = 29.97;
       // //_mhandler.Parameters = "-strict experimental -threads 0 -vpre fast -vpre ipod640";
       //// _mhandler.Parameters = "-strict experimental -bt 1024k -maxrate 4M -flags +loop -cmp +chroma -me_range 16 -g 300 -keyint_min 25 -sc_threshold 40 -i_qfactor 0.71 -rc_eq \"blurCplx^(1-qComp)\" -qcomp 0.6 -qmin 10 -qmax 51 -qdiff 4 -coder 0 -refs 1 -bufsize 4M -level 21 -partitions parti4x4+partp8x8+partb8x8 -subq 5";
   
       //// _mhandler.ACodec = "aac"; 
       // _mhandler.Audio_Bitrate = 160; 
       // VideoInfo info = _mhandler.Process();

        //**********************************
        // Example: F4V Encoding
        //**********************************
        _mhandler.FileName = "sampleavi.avi";
        _mhandler.OutputFileName = "sampleavi";
        _mhandler.OutputExtension = ".flv";
        _mhandler.Video_Bitrate = 500;
        _mhandler.Audio_Bitrate = 128;
        _mhandler.Force = "flv";
        _mhandler.Width = 320;
        _mhandler.Height = 240;
        _mhandler.Audio_SamplingRate = 44100;
        VideoInfo info = _mhandler.Process();
        //****************************
        // Example 2: Extracting sound from a video, and save it as Mp3
        //****************************
        //_mhandler.FileName = "sample.mp4";
        //_mhandler.OutputFileName = "sample";
        //_mhandler.OutputExtension = ".mp3";
        //_mhandler.DisableVideo = true;
        //_mhandler.Audio_SamplingRate = 44100;
        //_mhandler.Channel = 2;
        //_mhandler.Audio_Bitrate = 192;
        //_mhandler.Force = "mp3";
        // VideoInfo info = _mhandler.Process();
        //****************************
        // Example 3: Convert .avi to dv
        //****************************
        // _mhandler.FileName = "sample.avi";
        //_mhandler.OutputFileName = "sample";
        //_mhandler.OutputExtension = ".dv";
        //_mhandler.AspectRatio = "4:3";
        //_mhandler.Parameters = "-s pal -r pal";
        //_mhandler.Audio_SamplingRate = 48000;
        //_mhandler.Channel = 2;
        //VideoInfo info = _mhandler.Process();
         //****************************
        // Example 4: Convert .avi to wmv
         //****************************
        // _mhandler.FileName = "sample.avi";
        // _mhandler.OutputFileName = "sample";
        //_mhandler.OutputExtension = ".wmv";
        //_mhandler.VCodec = "wmv2";
        //_mhandler.ACodec = "wmav2";
        //_mhandler.Audio_Bitrate = 128;
        //_mhandler.Video_Bitrate = 700;
       
        //VideoInfo info = _mhandler.Process();
           
        //if (info.ErrorCode > 0 && info.ErrorCode != 121)
        //{
        //    Response.Write("Error occured, Error Code: " + info.ErrorCode + "");
        //    return;
        //}

        StringBuilder str = new StringBuilder();
        str.Append("File Name= " + info.FileName + "<br />");
        str.Append("Video Duration= " + info.Duration + "<br />");
        str.Append("Video Duration in Seconds= " + info.Duration_Sec + "<br />");
        // Input values
        str.Append("<strong>Input Values</strong><br />");
        str.Append("Video Codec= " + info.Input_Vcodec + "<br />");
        str.Append("Audio Codec= " + info.Input_Acodec + "<br />");
        str.Append("Video Bitrate= " + info.Input_Video_Bitrate + "<br />");
        str.Append("Audio Bitrate= " + info.Input_Audio_Bitrate + "<br />");
        str.Append("Audio Sampling Rate= " + info.Input_SamplingRate + "<br />");
        str.Append("Audio Channel= " + info.Input_Channel + "<br />");
        str.Append("Width= " + info.Input_Width + "<br />");
        str.Append("Height= " + info.Input_Height + "<br />");
        str.Append("Video FrameRate= " + info.Input_FrameRate + "<br />");


        // Output values
        str.Append("<strong>Output Values</strong><br />");
        str.Append("Video Codec= " + info.Vcodec + "<br />");
        str.Append("Audio Codec= " + info.Acodec + "<br />");
        str.Append("Video Bitrate= " + info.Video_Bitrate + "<br />");
        str.Append("Audio Bitrate= " + info.Audio_Bitrate + "<br />");
        str.Append("Audio Sampling Rate= " + info.SamplingRate + "<br />");
        str.Append("Audio Channel= " + info.Channel + "<br />");
        str.Append("Width= " + info.Width + "<br />");
        str.Append("Height= " + info.Height + "<br />");
        str.Append("Video FrameRate= " + info.FrameRate + "<br />");
        str.Append(".................................<br />");
        str.Append("FFMPEG Output:" + info.FFMPEGOutput + "");

        str.Append("Error Code= " + info.ErrorCode + "<br />");
        Response.Write(str.ToString());


    }

}
