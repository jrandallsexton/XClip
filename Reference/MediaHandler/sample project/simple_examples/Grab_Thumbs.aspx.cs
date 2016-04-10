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

public partial class Grab_Thumbs : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        MediaHandler _mhandler = new MediaHandler();

        string RootPath = Server.MapPath(Request.ApplicationPath);
        string WatermarkPath = RootPath + "\\contents\\watermark\\logo.png";
        _mhandler.FFMPEGPath = HttpContext.Current.Server.MapPath("~\\ffmpeg_july_2012\\bin\\ffmpeg.exe");
        _mhandler.InputPath = RootPath + "\\contents\\original";
        _mhandler.OutputPath = RootPath + "\\contents\\thumbs";

        ////***************************************************
        //// Multiple Thumbs: Thumb Mod: Normal // Recommended for multiple thumbs as it support long length videos too
        ////***************************************************

        //string thumb_start_index = "sample_";
        //_mhandler.FileName = "6e8fce2c8a5c.mp4";
        //_mhandler.Image_Format = "jpg";
        //_mhandler.VCodec = "image2";
        //_mhandler.ImageName = thumb_start_index;
        //_mhandler.Multiple_Thumbs = true;
        //_mhandler.ThumbMode = 0;
        //_mhandler.No_Of_Thumbs = 15;
        //_mhandler.Thumb_Start_Position = 10; // start grabbing thumbs from 5th second
        ////_mhandler.Width = 160;
        ////_mhandler.Height = 120;
        //_mhandler.Width = 640;
        //_mhandler.Height = 480;
        //// _mhandler.Parameters = "-vf \"movie=" + WatermarkPath + " [watermark]; [in][watermark] overlay=main_w-overlay_w-10:main_h-overlay_h-10 [out]\"";
        //_mhandler.Parameters = "-vf movie=logo.png [watermark]; [in][watermark] overlay=main_w-overlay_w-10:main_h-overlay_h-10 [out]";
        //VideoInfo info = _mhandler.Grab_Thumb();
        //if (info.ErrorCode > 0)
        //{
        //    Response.Write("Error occured while grabbing thumbs from video");
        //}

   
        ////***************************************************
        //// Multiple Thumbs: Thumb Mod: Fast // Not working in long length videos
        ////***************************************************

        //string thumb_start_index = "Joachims_";
        //_mhandler.FileName = "Joachims.avi";
        //_mhandler.Image_Format = "jpg";
        //_mhandler.VCodec = "image2";
        //_mhandler.ImageName = thumb_start_index;
        //_mhandler.Multiple_Thumbs = true;
        //_mhandler.ThumbMode = 1; // fast mode
        //_mhandler.No_Of_Thumbs = 15;
        //_mhandler.Thumb_Start_Position = 10; // start grabbing thumbs from 5th second
        //_mhandler.Auto_Transition_Time = true;
        //_mhandler.Width = 160;
        //_mhandler.Height = 120;
        //VideoInfo info = _mhandler.Grab_Thumb();
        //if (info.ErrorCode > 0)
        //{
        //    Response.Write("Error occured while grabbing thumbs from video");
        //}

        ////******************************************************
        //// Single Thumb
        ////******************************************************

        //_mhandler.FileName = "sample.mp4";
        //_mhandler.ImageName = "sample.jpg";
        //_mhandler.Frame_Time = "10";
        ////OR
        ////_mhandler.Thumb_Start_Position = 10;

        //_mhandler.Image_Format = "jpg";
        //_mhandler.VCodec = "image2";

        //_mhandler.Width = 160;
        //_mhandler.Height = 120;
        //info = _mhandler.Grab_Thumb();
        //if (info.ErrorCode > 0)
        //{
        //    Response.Write("Error occured while grabbing thumbs from video");
        //}
        
        Response.Write("Thumbs have been captured");

    }

 
}