using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

public partial class stretch : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        // Get video information
        MediaHandler _mhandler = new MediaHandler();
        string RootPath = Server.MapPath(Request.ApplicationPath);
        _mhandler.FFMPEGPath = HttpContext.Current.Server.MapPath("~\\ffmpeg_july_2012\\bin\\ffmpeg.exe");
        _mhandler.InputPath = RootPath + "\\contents\\original";
        _mhandler.OutputPath = RootPath + "\\contents\\flv";

        string filename = "6e8fce2c8a5c.mp4";
        _mhandler.FileName = filename;
        VideoInfo info = _mhandler.Get_Info();
        if (info.ErrorCode > 0 && info.ErrorCode != 121)
        {
            Response.Write("Video processing failed, Error code " + info.ErrorCode + " generated");
            return;
        }

        string duration = info.Duration;
        int duration_in_second = info.Duration_Sec;

        if (info.Width == 0 || info.Height == 0)
        {
            Response.Write("Source Video Information Failed");
            return;
        }

        double source_aspect = (double)info.Width / info.Height;
      
        MediaHandler _encode = new MediaHandler();
        _encode.FFMPEGPath = HttpContext.Current.Server.MapPath("~\\ffmpeg_march_2012\\bin\\ffmpeg.exe");
        _encode.InputPath = RootPath + "\\contents\\original";
        _encode.OutputPath = RootPath + "\\contents\\mp4";
        _encode.FileName = filename;
        _encode.OutputExtension = ".mp4";
        _encode.OutputFileName = "stretch_video";
        _encode.VCodec = "libx264";
        _encode.ACodec = "libvo_aacenc";
        _encode.Channel = 2;
        _encode.Audio_SamplingRate = 44100;
        _encode.Audio_Bitrate = 96;
        _encode.Video_Bitrate = 500;
        _encode.FrameRate = 29.97;
     
        _encode.Width = 640;
        _encode.Height = 480;

        // widescreen
        //_encode.Width = 640;
        //_encode.Height = 360;
        string _fullwidth = _encode.Width + ":" + _encode.Height;
        double publish_aspect = (double)_encode.Width / _encode.Height;
        if (publish_aspect < 1.5)
        {
            // Video to be published with normal aspect ratio
            // Check source video aspect if its normal, process directly, if its wide screen then perform widescreen to normal video conversion without stretching
            if (source_aspect > 1.5)
            {
                // Source video aspect is widescreen.
                // Widescreen to normal video conversion logic required.
               
                // Calculate actual widescreen height
                int widescreen_height = Convert.ToInt32(_encode.Height / 1.7777);
                // Calculate Difference between normal height and wide screen height
                int difference = _encode.Height - widescreen_height;
                // Divide difference in two pieces for top and bottom padding
                int pad_width = difference / 2;
                // Replace source height with widescreen height
                _encode.Height = widescreen_height;
               
                _encode.Parameters = "-vf pad=" + _fullwidth + ":0:" + pad_width + "";
            }
        }
        else
        {
            // Video to be published with wide screen aspect ratio
            // Check source video aspect if its widescreen, process directly, if its normal then perform normal to widescreen video conversion without stretching
            if (source_aspect < 1.5)
            {
                // Source video aspect is normal.
                // Normal to widescreen video conversion logic required

                // Calculate normal height of vieo based on widescreen published height
                int normal_width = Convert.ToInt32(_encode.Height * 1.333);
                // Calcuale the difference between normal and widescreen widths
                int difference = _encode.Width - normal_width;
                // Divide difference in two pieces for left, right padding
                int pad_width = difference / 2;
                // Replace Source Width with Calculated Width
                _encode.Width = normal_width;
               
                _encode.Parameters = "-vf pad=" + _fullwidth + ":" + pad_width + ":0";
            }
        }

        VideoInfo encodeinfo = _encode.Process();
        // retrieve valudes
        if (info.ErrorCode > 0)
        {
            Response.Write("Video processing failed, Error code " + info.ErrorCode + " generated");
            Response.Write("<br />" + info.FFMPEGOutput + "");
            return;
        }
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