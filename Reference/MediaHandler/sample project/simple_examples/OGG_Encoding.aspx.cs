using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OGG_Encoding : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        MediaHandler _mhandler = new MediaHandler();

        string RootPath = Server.MapPath(Request.ApplicationPath);
        //*********************************
        // Old approach MHP ver 5.4 or below
        //*********************************
       // _mhandler.InputPath = RootPath + "\\contents\\original";
       // _mhandler.OutputPath = RootPath + "\\contents\\mp4";

       // _mhandler.MP4BoxPath = RootPath + "\\theora\\ffmpeg2theora-0.27.exe";
       // _mhandler.FileName = "wildlife.wmv";
       // _mhandler.OutputFileName = "wildlife_old.ogv";
       //// _mhandler.Parameters = "SEND MORE OPTIONS THROUGH THIS PROPERTY";
       // _mhandler.Encode_OGG();

        //*********************************
        // New approach MHP ver 5.5 or newer
        //*********************************

        _mhandler.FFMPEGPath = RootPath + "\\theora\\ffmpeg2theora-0.27.exe"; // HttpContext.Current.Server.MapPath("~\\ffmpeg_july_2012\\bin\\ffmpeg.exe");
        _mhandler.InputPath = RootPath + "\\contents\\original";
        _mhandler.OutputPath = RootPath + "\\contents\\mp4";
        //_mhandler.BackgroundProcessing = true;
        _mhandler.FileName = "Wildlife.wmv";
        _mhandler.OutputFileName = "wildlife_new";
        _mhandler.OutputExtension = ".ogv";
        _mhandler.Process_FFMPEG2Theora();

        Response.Write("Video output generated properly");
    }
}