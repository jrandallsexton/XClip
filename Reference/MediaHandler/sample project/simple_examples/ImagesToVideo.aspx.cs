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

public partial class ImagesToVideo : System.Web.UI.Page
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

        //******************************************
        // Note:
        // i: All thumbs must be in sequest and in .jpg format e.g a_001.jpg, a_002.jpg, a_003.jpg....
        // ii: FileName property below must point actual name of thumb without .jpg and sequence e.g "a_" represent "a_001.jpg...." 
        //******************************************
        _mhandler.FileName = "a_";
        _mhandler.OutputFileName = "a";
        _mhandler.OutputExtension = ".flv";
        
        VideoInfo info = _mhandler.ImagesToVideo();
        if (info.ErrorCode > 0 && info.ErrorCode != 121)
        {
            Response.Write("Error occured, Error Code: " + info.ErrorCode + "");
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
        str.Append("Error Code= " + info.ErrorCode + "<br />");
        str.Append("FFMPEG Output= " + info.FFMPEGOutput + "<br />");
        Response.Write(str.ToString());


    }

}
