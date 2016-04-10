<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/main.master" CodeFile="Default.aspx.cs"
    Inherits="_Default" %>

<asp:Content ID="head" ContentPlaceHolderID="hd" runat="server">
    <title>ASP.NET Media Handler Pro Examples</title>
</asp:Content>
<asp:Content ID="pagecontent" ContentPlaceHolderID="MC" runat="server">
    <div>
        <h2>
            Media Handler Pro Background Processing With Progress Bar Examples</h2>
    </div>
    <div class="item_pad_2">
        Note: Sample video <strong>wildlife.wmv</strong> located in <strong>/contents/original/</strong> folder, you can start testing on it. final output will be generated in <strong>/contents/mp4/</strong> directory
    </div>
    <div>
        <ul>
            <li><a href="webm.aspx">WebM Encoding</a>.</li>
            <li><a href="mp4.aspx">MP4 Encoding</a>.</li>
            <li><a href="flv.aspx">FLV Encoding</a>.</li>
            <li><a href="vd_3gp.aspx">3GP Encoding</a>.</li>
            <li><a href="mp3.aspx">MP3 Encoding</a>.</li>
            <li><a href="wmv.aspx">WMV Encoding</a>.</li>
            <li><a href="avi.aspx">AVI Encoding</a>.</li>
            
        </ul>
    </div>
    <div>
        <h2>
            Media Handler Pro Simple Examples</h2>
    </div>
    <div>
        <ul>
            <li><a href="simple_examples/webm_encoding.aspx">WebM Encoding</a>.</li>
            <li><a href="simple_examples/OGG_Encoding.aspx">OGV Encoding</a>.</li>
            <li><a href="simple_examples/Process.aspx">Process Any Media</a>.</li>
            <li><a href="simple_examples/Get_Info.aspx">Get Information From Video</a>.</li>
            <li><a href="simple_examples/Grab_Thumbs.aspx">Grab Thumbnails from Video</a>.</li>
            <li><a href="simple_examples/3GP_Encoding.aspx">3GP Encoding</a>.</li>
            <li><a href="simple_examples/JoinVideos.aspx">Join Video Clips</a>.</li>
            <li><a href="simple_examples/Split_Videos.aspx">Split Video into Clips</a>.</li>
            <li><a href="simple_examples/WMV_Encoding.aspx">WMV Encoding</a>.</li>
            <li><a href="simple_examples/ImagesToVideo.aspx">Images To Video</a>.</li>
            <li><a href="simple_examples/stretch.aspx">Encode Video Without Stretching</a>.</li>
        </ul>
    </div>
</asp:Content>
