using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

public partial class webm_encoding : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {

        MediaHandler _mhandler = new MediaHandler();
        string RootPath = Server.MapPath(Request.ApplicationPath);
        _mhandler.FFMPEGPath = HttpContext.Current.Server.MapPath("~\\ffmpeg_july_2012\\bin\\ffmpeg.exe");
        _mhandler.InputPath = RootPath + "\\contents\\original";
        _mhandler.OutputPath = RootPath + "\\contents\\webm";
        _mhandler.FileName = "Wildlife.wmv";
        string presetpath = RootPath + "\\ffmpeg\\presets\\libvpx-360p.ffpreset";
        _mhandler.OutputFileName = "wildlife";
        _mhandler.OutputExtension = ".webm";
        _mhandler.Video_Bitrate = 800;
        _mhandler.Audio_Bitrate = 64;
        _mhandler.VCodec = "libvpx";
        _mhandler.ACodec = "libvorbis";
        _mhandler.Audio_SamplingRate = 44100;
        _mhandler.Parameters = "-f webm -aspect 4:3 -fpre \"" + presetpath + "\"";
        _mhandler.Width = 480;
        _mhandler.Height = 360;
        VideoInfo info = _mhandler.Process();

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