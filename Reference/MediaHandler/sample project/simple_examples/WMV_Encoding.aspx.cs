using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

public partial class WMV_Encoding : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
       MediaHandler _mhandler = new MediaHandler();

       string RootPath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath);
       _mhandler.FFMPEGPath = HttpContext.Current.Server.MapPath("~\\ffmpeg_july_2012\\bin\\ffmpeg.exe");
       _mhandler.InputPath = RootPath + "\\contents\\original";
       _mhandler.OutputPath = RootPath + "\\contents\\mp4";
       _mhandler.BackgroundProcessing = false;
       _mhandler.FileName = "futbol.avi";
       _mhandler.OutputFileName = "futbol_normal";
       _mhandler.OutputExtension = ".wmv";
       _mhandler.VCodec = "msmpeg4"; //"wmv2";
       _mhandler.Parameters = "-qscale 3";
       _mhandler.ProcessMedia();
       VideoInfo info = _mhandler.vinfo;
       // retrieve valudes
       if (info.ErrorCode > 0)
       {
           Response.Write("Video processing failed, Error code " + info.ErrorCode + " generated");
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
