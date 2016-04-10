using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.IO;

public partial class Split_Videos : System.Web.UI.Page
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
        _mhandler.OutputPath = RootPath + "\\contents\\splitvideos";

        //****************************************************
        // Split Videos
        //***************************************************
        _mhandler.FileName = "sample.avi";
        _mhandler.OutputFileName = "split_video";
        _mhandler.OutputExtension = ".avi";
        _mhandler.Width = 320;
        _mhandler.Height = 240;
        _mhandler.TargetFileType = "pal-vcd";

        int length_of_video = 20; // in seconds;
        int total_clips = 10;
        //VideoInfo info = _mhandler.Split_Video(20);
        VideoInfo info = _mhandler.Split_Video(length_of_video,total_clips);
        if (info.ErrorCode > 0)
        {
            Response.Write("Error occured: Error Code: " + info.ErrorCode + "<br />Error Message: " + info.ErrorMessage);
            return;
        }

        Response.Write("Message: " + info.ErrorMessage);

    }
}
