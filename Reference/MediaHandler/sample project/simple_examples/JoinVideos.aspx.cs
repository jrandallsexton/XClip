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

public partial class JoinVideos : System.Web.UI.Page
{
    protected void Button1_Click(object sender, EventArgs e)
    {
        MediaHandler _mhandler = new MediaHandler();

        string RootPath = Server.MapPath(Request.ApplicationPath);
        _mhandler.FFMPEGPath = HttpContext.Current.Server.MapPath("~\\ffmpeg_july_2012\\bin\\ffmpeg.exe");
        _mhandler.InputPath = RootPath + "\\contents\\original";
        _mhandler.OutputPath = RootPath + "\\contents\\join";

        //****************************************************
        // JOIN VIDEOS
        //***************************************************

        _mhandler.FileNames = new string[] { "sample_01.mp4", "sample_02.mp4" };
        _mhandler.OutputFileName = "sample_full"; // output filename without extension recommended.
        _mhandler.OutputExtension = ".avi"; // set output media extension here.
        _mhandler.Width = 320;  // width / height recommended to avoid any mismanagement in width and height of different video clips
        _mhandler.Height = 240;
        _mhandler.TargetFileType = "pal-vcd"; // depend on target video type.

        VideoInfo info = _mhandler.Join_Videos();
        if (info.ErrorCode > 0)
        {
            Response.Write("Error occured: Error Code: " + info.ErrorCode + "<br />Error Message: " + info.ErrorMessage);
            return;
        }

        Response.Write("Video has been successfully joined");

    }
}
