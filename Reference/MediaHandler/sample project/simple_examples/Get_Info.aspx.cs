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

public partial class Get_Info : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btn_submit_Click(object sender, EventArgs e)
    {
        MediaHandler _mhandler = new MediaHandler();
        string RootPath = Server.MapPath(Request.ApplicationPath);
        _mhandler.FFMPEGPath = HttpContext.Current.Server.MapPath("~\\ffmpeg_july_2012\\bin\\ffmpeg.exe");
        _mhandler.InputPath = RootPath + "\\contents\\original";

        _mhandler.FileName = "http://s3.amazonaws.com/media.digitalsignage.net/1/0a571f.mov";
        VideoInfo info = _mhandler.Get_Info();
        if (info.ErrorCode > 0 && info.ErrorCode != 121)
        {
            Response.Write("Video processing failed, Error code " + info.ErrorCode + " generated");
            return;
        }
       
        StringBuilder str = new StringBuilder();
        str.Append("File Name= " + info.FileName + "<br />");
        str.Append("Video Codec= " + info.Vcodec + "<br />");
        str.Append("Audio Codec= " + info.Acodec + "<br />");
        str.Append("Video Bitrate= " + info.Video_Bitrate + "<br />");
        str.Append("Audio Bitrate= " + info.Audio_Bitrate + "<br />");
        str.Append("Audio Sampling Rate= " + info.SamplingRate + "<br />");
        str.Append("Audio Channel= " + info.Channel + "<br />");
        str.Append("Width= " + info.Width + "<br />");
        str.Append("Height= " + info.Height + "<br />");
        str.Append("Video FrameRate= " + info.FrameRate + "<br />");
        str.Append("Video Duration= " + info.Duration + "<br />");
        str.Append("Video Duration in Seconds= " + info.Duration_Sec + "<br />");
        str.Append("Hours= " + info.Hours + "<br />");
        str.Append("Minutes= " + info.Minutes + "<br />");
        str.Append("Seconds= " + info.Seconds + "<br />");
        str.Append("Error Code= " + info.ErrorCode + "<br />");
        str.Append("Ffmpeg output= " + info.FFMPEGOutput + "<br />");
        Response.Write(str.ToString());
    }
}
