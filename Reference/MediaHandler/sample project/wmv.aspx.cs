using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Threading;
using System.Web.Services;
using System.Web.Script.Serialization;

public partial class wmv : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    public static double ProgressValue = 0;
    public static MediaHandler _mhandler = new MediaHandler();

    [WebMethod]
    public static string EncodeVideo()
    {
        // MediaHandler _mhandler = new MediaHandler();
        string RootPath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath);
        _mhandler.FFMPEGPath = HttpContext.Current.Server.MapPath("~\\ffmpeg_july_2012\\bin\\ffmpeg.exe");
        _mhandler.InputPath = RootPath + "\\contents\\original";
        _mhandler.OutputPath = RootPath + "\\contents\\mp4";
        _mhandler.BackgroundProcessing = true;
        _mhandler.FileName = "futbol.avi";
        _mhandler.OutputFileName = "futbol";
        _mhandler.OutputExtension = ".wmv";
        _mhandler.VCodec = "msmpeg4"; //"wmv2";
        _mhandler.Parameters = "-qscale 3"; 
        _mhandler.ProcessMedia();
        return _mhandler.vinfo.ErrorCode.ToString();
    }
   

    [WebMethod]
    public static string GetProgressStatus()
    {
        return Math.Round(_mhandler.vinfo.ProcessingCompleted, 2).ToString();
        // if vinfo.processingcomplete==100, then you can get complete information from vinfo object and store it in database and perform other processing.
    }

    [WebMethod]
    public static string GetInformation()
    {
        //Response.Write(pstr.ToString());
        StringBuilder str = new StringBuilder();
        str.Append("<div class=\"item_pad_4\">\n");
        str.Append("<table class=\"table table-bordered table-striped\">\n");
        str.Append("<thead><tr>\n");
        str.Append("<th>Property</th>\n");
        str.Append("<th>Information</th>\n");
        str.Append("</tr></thead>\n");
        str.Append("<tbody>\n");

        if (_mhandler.vinfo.ErrorCode > 0)
        {
            // error occurs in processing
            str.Append("<tr><td>Error Code</td><td>" + _mhandler.vinfo.ErrorCode + "</td></tr>\n");
            str.Append("<tr><td>Error Message</td><td>" + _mhandler.vinfo.ErrorMessage + "</td></tr>\n");
            str.Append("<tr><td>FFMPEG Output</td><td>" + _mhandler.vinfo.FFMPEGOutput + "</td></tr>\n");
        }
        else
        {
            str.Append("<tr><td>Error Code</td><td>No Error Occured</td></tr>\n");
        }
        str.Append("<tr><td>File Name</td><td>" + _mhandler.vinfo.FileName + "</td></tr>\n");
        str.Append("<tr><td>Video Duration</td><td>" + _mhandler.vinfo.Duration + "(" + _mhandler.vinfo.Duration_Sec + " seconds)</td></tr>\n");
        str.Append("<tr><td colspan=\"2\">Output Values</td></tr>\n");
        if (_mhandler.vinfo.Vcodec != "")
        {
            str.Append("<tr><td>Video Codec</td><td>" + _mhandler.vinfo.Vcodec + "</td></tr>\n");
        }
        if (_mhandler.vinfo.Acodec != "")
        {
            str.Append("<tr><td>Audio Codec</td><td>" + _mhandler.vinfo.Acodec + "</td></tr>\n");
        }
        if (_mhandler.vinfo.Video_Bitrate != "")
        {
            str.Append("<tr><td>Video Bitrate</td><td>" + _mhandler.vinfo.Video_Bitrate + "</td></tr>\n");
        }
        if (_mhandler.vinfo.Audio_Bitrate != "")
        {
            str.Append("<tr><td>Audio Bitrate</td><td>" + _mhandler.vinfo.Audio_Bitrate + "</td></tr>\n");
        }
        if (_mhandler.vinfo.SamplingRate != "")
        {
            str.Append("<tr><td>Audio Sampling Rate</td><td>" + _mhandler.vinfo.SamplingRate + "</td></tr>\n");
        }
        if (_mhandler.vinfo.Channel != "")
        {
            str.Append("<tr><td>Audio Channel</td><td>" + _mhandler.vinfo.Channel + "</td></tr>\n");
        }
        if (_mhandler.vinfo.Width > 0)
        {
            str.Append("<tr><td>Width</td><td>" + _mhandler.vinfo.Width + "</td></tr>\n");
        }
        if (_mhandler.vinfo.Height > 0)
        {
            str.Append("<tr><td>Height</td><td>" + _mhandler.vinfo.Height + "</td></tr>\n");
        }
        if (_mhandler.vinfo.FrameRate != "")
        {
            str.Append("<tr><td>Framerate</td><td>" + _mhandler.vinfo.FrameRate + "</td></tr>\n");
        }
        str.Append("<tr><td colspan=\"2\">Optional Information</td></tr>\n");
        //str.Append("<tr><td>Title</td><td>" + info.Title + "</td></tr>\n");
        if (_mhandler.vinfo.Producer != "")
        {
            str.Append("<tr><td>Producer</td><td>" + _mhandler.vinfo.Producer + "</td></tr>\n");
        }
        if (_mhandler.vinfo.Music != "")
        {
            str.Append("<tr><td>Music</td><td>" + _mhandler.vinfo.Music + "</td></tr>\n");
        }
        if (_mhandler.vinfo.Footage != "")
        {
            str.Append("<tr><td>Footage</td><td>" + _mhandler.vinfo.Footage + "</td></tr>\n");
        }
        str.Append("</tbody>\n");
        str.Append("</table>\n");
        str.Append("</div>\n");

        return str.ToString();
    }
}