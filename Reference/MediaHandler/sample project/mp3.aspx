<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mp3.aspx.cs" MasterPageFile="~/main.master" Inherits="mp3" %>
<asp:Content ID="head" ContentPlaceHolderID="hd" runat="server">
    <title>ASP.NET Media Handler Pro - MP3 Audio Encoding</title>
</asp:Content>
<asp:Content ID="pagecontent" ContentPlaceHolderID="MC" runat="server">
 <div style="padding-left: 30px;">
      <script type="text/javascript">
          // Custom example logic
          $(function () {
              $("#vprocess").on({
                  click: function (e) {
                      ProcessEncoding();
                      var IntervalID = setInterval(function () {
                          GetProgressValue(IntervalID);
                      }, 1000);
                      return false;
                  }
              }, '#btn_process');

          });
          function ProcessEncoding() {
              $.ajax({
                  type: "POST",
                  url: "mp3.aspx/EncodeVideo",
                  data: "{}",
                  contentType: "application/json; charset=utf-8",
                  dataType: "json",
                  success: function (msg) {
                      if (msg.d == "0") {
                          $("#pbar").show();
                          // processing started, no error reported in start
                          $("#btn_process").hide();
                      }
                      else {
                          Display_Message('#msg', "Error occured while processing video", 0);
                          FetchInfo();
                      }
                  }
              });
          }

          function GetProgressValue(intervalid) {
              $.ajax({
                  type: "POST",
                  url: "mp3.aspx/GetProgressStatus",
                  data: "{}",
                  contentType: "application/json; charset=utf-8",
                  dataType: "json",
                  success: function (msg) {
                      // Do something interesting here.
                      $("#pstats").text(msg.d);
                      $("#pbar_int_01").attr('style', 'width: ' + msg.d + '%;');
                      if (msg.d == "100") {
                          $('#pbar01').removeClass("progress-danger");
                          $('#pbar01').addClass("progress-success");
                          if (intervalid != 0) {
                              clearInterval(intervalid);
                          }
                          FetchInfo();
                      }
                  }
              });
          }

          function FetchInfo() {
              $.ajax({
                  type: "POST",
                  url: "mp3.aspx/GetInformation",
                  data: "{}",
                  contentType: "application/json; charset=utf-8",
                  dataType: "json",
                  success: function (msg) {
                      // Do something interesting here.
                      $("#info").html(msg.d);
                  }
              });
          }

          /* Display Message */
          function Display_Message(id, msg, tp) {
              switch (tp) {
                  case 0:
                      $(id).prepend("<div class='alert alert-error'><button class='close' data-dismiss='alert'>×</button>" + msg + "</div>");
                      break;
                  case 1:
                      $(id).prepend("<div class='alert alert-success'><button class='close' data-dismiss='alert'>×</button>" + msg + "</div>");
                      break;
                  case 2:
                      $(id).prepend("<div class='item_c' style='padding:20px 0px;'>" + msg + "</div>");
                      break;
                  case 3:
                      $(id).prepend("<div class='alert alert-info'><button class='close' data-dismiss='alert'>×</button>" + msg + "</div>");
                      break;
                  case 4:
                      $(id).prepend("<div class='alert'><button class='close' data-dismiss='alert'>×</button>" + msg + "</div>");
                      break;
              }
          }
    </script>
    <div class="item_pad_2">
            <h2>
                MP3 Encoding</h2>
        </div>
        <div class="item_pad_2">
            Source File Name: /contents/original/wildlife.wmv
            <br />
            Output File Name: /contents/mp4/wildlife.mp3
        </div>
    <div id="vprocess" style="padding-top: 20px; width: 400px;">
        <div id="msg">
        </div>
        <div id="pbar" style="display: none;">
            <div class="item_pad_2">
                <div id="pbar01" class="progress progress-danger" style="margin-bottom: 0px;">
                    <div id="pbar_int_01" class="bar" style="width: 0%;">
                    </div>
                </div>
            </div>
            <div class="item_pad_2 medium-text bold black">
                <span id="pstats">0</span>% audio processing completed.
            </div>
        </div>
        <div id="info">
        </div>
        <div class="item_pad_2">
            <button id="btn_process" class="btn btn-small btn-warning">
                Start Processing</button>
        </div>
    </div>
    </div>
</asp:Content>
