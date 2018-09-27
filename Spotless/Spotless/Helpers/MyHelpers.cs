using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Configuration;
using System.IO;
using System.Web.Script.Serialization;

namespace Spotless.Helpers
{
    public partial class MultiSelectResult
    {
        public string TEXT;
        public string VALUE;
    }
    public static class MyHelpers
    {
        public static MvcHtmlString UploaderFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> attributes = null)
        {
            var data = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            string propertyName = data.PropertyName;
            var Url = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            string controller = htmlHelper.ViewContext.RouteData.Values["controller"].ToString().ToLower();

            #region Default Attributes
            string mainFilesPath = ConfigurationManager.AppSettings["MainFilesPath"].ToString();
            string uploaderDefaultText = ConfigurationManager.AppSettings["UploaderDefaultText"].ToString();
            string uploaderDefaultNote = ConfigurationManager.AppSettings["UploaderDefaultNote"].ToString();
            string uploadTemplateId = ConfigurationManager.AppSettings["UploadTemplateId"].ToString();
            string downloadTemplateId = ConfigurationManager.AppSettings["DownloadTemplateId"].ToString();

            string directory = (ConfigurationManager.AppSettings[controller + "Directory"] ?? controller).ToString();
            string allowedExtensions = (ConfigurationManager.AppSettings[controller + "AllowedExtensions"] ?? ConfigurationManager.AppSettings["AllowedExtensions"]).ToString();
            int maxFileSize = Convert.ToInt32(ConfigurationManager.AppSettings[controller + "MaxFileSize"] ?? ConfigurationManager.AppSettings["MaxFileSize"]);
            string maxNumberOfFiles = (ConfigurationManager.AppSettings[controller + "MaxNumberOfFiles"] ?? ConfigurationManager.AppSettings["MaxNumberOfFiles"]).ToString();
            #endregion

            #region Attributes
            if (attributes != null && attributes.Any())
            {
                foreach (var item in attributes)
                {
                    if (item.Key.ToLower() == "uploadtemplateid")
                    {
                        uploadTemplateId = item.Value.ToString();
                    }
                    if (item.Key.ToLower() == "downloadtemplateid")
                    {
                        downloadTemplateId = item.Value.ToString();
                    }
                    if (item.Key.ToLower() == "uploaderdefaultnote")
                    {
                        uploaderDefaultText = item.Value.ToString();
                    }
                    if (item.Key.ToLower() == "uploaderdefaulttext")
                    {
                        uploaderDefaultNote = item.Value.ToString();
                    }
                    if (item.Key.ToLower() == "mainfilespath")
                    {
                        mainFilesPath = item.Value.ToString();
                    }
                    if (item.Key.ToLower() == "directory")
                    {
                        directory = item.Value.ToString();
                    }
                    if (item.Key.ToLower() == "allowedextensions")
                    {
                        allowedExtensions = item.Value.ToString();
                    }
                    if (item.Key.ToLower() == "maxfilesize")
                    {
                        maxFileSize = Convert.ToInt32(item.Value.ToString());
                    }
                    if (item.Key.ToLower() == "maxnumberoffiles")
                    {
                        maxNumberOfFiles = item.Value.ToString();
                    }
                }
            }
            #endregion

            bool isInEditMode = false;
            var existingFiles = "var files = [";
            if (data.Model != null)
            {
                isInEditMode = true;
                var arrayOfFiles = (data.Model + "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in arrayOfFiles)
                {
                    var fullrelativePath = Url.Content(mainFilesPath + directory + "/" + item);
                    if (File.Exists(HttpContext.Current.Server.MapPath(fullrelativePath)))
                    {
                        var fileVar = new FileInfo(HttpContext.Current.Server.MapPath(fullrelativePath));
                        existingFiles += (existingFiles == "var files = [" ? "" : ",") + @"{
                                        'name': '" + item + @"',
                                        'prettyName': '" + item.Split('~')[1] + @"',
                                        'size': " + fileVar.Length + @",
                                        'type': '" + fileVar.GetType() + @"',
                                        'url': '" + fullrelativePath + @"',
                                        'deleteUrl': '/UploadHandler/Delete?id=" + HttpUtility.UrlEncode(fullrelativePath) + @"',
                                        'thumbnailUrl': '" + fullrelativePath + @"?width=48&height=48&mode=max',
                                        'deleteType': 'DELETE',
                                        'inputName': '" + propertyName + @"'
                                        }";
                    };
                }
                existingFiles += "];";
            }

            MvcHtmlString result = new MvcHtmlString("<div  id='fileupload_" + propertyName + @"' class='row fileupload-buttonbar'>
                    <div class='col-lg-9'>
                        <span class='btn btn-success fileinput-button green' style='width: 300px;'>
                            <i class='glyphicon glyphicon-plus'></i>
                            <span>" + uploaderDefaultText + @"</span>
                            <input type='file' multiple/>
                            <input type='hidden' id='" + propertyName + "' name='" + propertyName + "' value='" + data.Model + @"'/>
                        </span>" + uploaderDefaultNote + @"                        
                    </div>
                    <table role='presentation' class='table table-striped'><tbody class='files'></tbody></table>
                </div>
                <script>
                 $(function(){
                    'use strict'; 
                   $('#fileupload_" + propertyName + @"').fileupload({
                    uploadTemplateId: '" + uploadTemplateId + @"',
                    downloadTemplateId: '" + downloadTemplateId + @"',
                    autoUpload: true,
                    maxNumberOfFiles: " + maxNumberOfFiles + @",
                    maxFileSize: " + maxFileSize + @",
                    acceptFileTypes: /(\.|\/)(" + allowedExtensions + @")$/i,
                    url: '" + Url.Content("~/UploadHandler/UploadFiles?inputname=" + propertyName + "&directory=" + mainFilesPath + directory) + @"'
                        });
                " + (isInEditMode ? existingFiles + "$('#fileupload_" + propertyName + @"').fileupload('option', 'done').call($('#fileupload_" + propertyName + @"'), $.Event('done'), {result: {files: files},editMode:true});" : "") +
                @"});
                </script>
                ");


            return result;
        }

        public static MvcHtmlString GoogleMap(this HtmlHelper htmlHelper, double? latValue = null, double? lngValue = null, int? zoomValue = null, string mapContainerId = null, bool showcurrentLocation = false, string latInput = null, string lngInput = null, string zoomInput = null, bool isDragable = true, bool inputsAreHidden = true)
        {
            #region set default values
            mapContainerId = mapContainerId ?? "mapCanvas";
            latInput = latInput ?? "googleMapLat";
            latValue = latValue ?? 33.88692186799784;
            lngInput = lngInput ?? "googleMapLong";
            lngValue = lngValue ?? 35.533215332031205;
            zoomInput = zoomInput ?? "googleMapZoom";
            zoomValue = zoomValue ?? 8;
            #endregion
            MvcHtmlString result = new MvcHtmlString(@"<div id='mapCanvas'></div>
                <input type='" + (inputsAreHidden ? "hidden" : "text") + "' id='" + latInput + "' name='" + latInput + "' value='" + latValue + @"'/>
                <input type='" + (inputsAreHidden ? "hidden" : "text") + "' id='" + lngInput + "' name='" + lngInput + "' value='" + lngValue + @"'/>
                <input type='" + (inputsAreHidden ? "hidden" : "text") + "' id='" + zoomInput + "' name='" + zoomInput + "' value='" + zoomValue + @"'/>
                <script type='text/javascript'>
                var latValue = " + latValue + @";
                var lngValue = " + lngValue + @";
                function getLocation()
                  {
                  if (navigator.geolocation)
                    {
                        navigator.geolocation.getCurrentPosition(showPosition,showError);
                    }
                  else{
                        x.innerHTML='Geolocation is not supported by this browser.';
    
                    }
                  }

                function showPosition(position)
                  {
                      var lat=position.coords.latitude;
                      var lon=position.coords.longitude;
                      InitializeGoogleMap('" + mapContainerId + "', '" + latInput + "', lat, '" + lngInput + "', lon, '" + zoomInput + "', " + zoomValue + ", " + (isDragable ? "true" : "false") + @");

                  }

                function showError(error)
                  {
                  setTimeout(() => {
                          showPosition({'coords':{'latitude':latValue ,'longitude':lngValue}}) ;
                    }, 200);
                  switch(error.code) 
                    {
                    case error.PERMISSION_DENIED:
    
                      x.innerHTML='User denied the request for Geolocation.'
      
                      break;
                    case error.POSITION_UNAVAILABLE:
                      x.innerHTML='Location information is unavailable.'
                      break;
                    case error.TIMEOUT:
                      x.innerHTML='The request to get user location timed out.'
                      break;
                    case error.UNKNOWN_ERROR:
                      x.innerHTML='An unknown error occurred.'
                      break;
                    }
                  } 
                    $(window).load(function () { LoadGoogleMapScript('FirstInitializeGoogleMap" + mapContainerId + @"'); });
                    function FirstInitializeGoogleMap" + mapContainerId + @"() {
                        " + ((!showcurrentLocation) ? @"InitializeGoogleMap('" + mapContainerId + "', '" + latInput + "', " + latValue + ", '" + lngInput + "', " + lngValue + ", '" + zoomInput + "', " + zoomValue + ", " + (isDragable ? "true" : "false") + @");" : "getLocation();") + @"
                    }

                </script>");
            return result;
        }

        public static MvcHtmlString TextEditor(this HtmlHelper htmlHelper, string name, object value, IDictionary<string, object> htmlAttributes = null)
        {
            return MvcHtmlString.Create(htmlHelper.Kendo().Editor()
                      .Name(name)
                      .HtmlAttributes(new { style = "width: 563px;height:200px" })
                      .Tools(tools => tools
                          .InsertFile()
                          .SubScript()
                          .SuperScript()
                          .ViewHtml()
                          .FontName()
                          .FontSize()
                          .FontColor().BackColor()
                      )
                      .Value(value.ToString())
                      .ImageBrowser(imageBrowser => imageBrowser
                        .Image("~/MCContent/Uploads/Editor/{0}")
                        .Read("Read", "Editor")
                        .Create("Create", "Editor")
                        .Destroy("Destroy", "Editor")
                        .Upload("Upload", "Editor")
                        .Thumbnail("Thumbnail", "Editor"))
                     .FileBrowser(fileBrowser => fileBrowser
                         .File("~/MCContent/Uploads/Editor/{0}")
                         .Read("Read", "Editor")
                         .Create("Create", "Editor")
                         .Destroy("Destroy", "Editor")
                         .Upload("Upload", "Editor")
                      ).ToString());
        }

        public static MvcHtmlString MultiSelect(this HtmlHelper htmlHelper, string name, dynamic value, string datacontroller = "", string dataaction = "")
        {
            var controller = "";
            var action = "";
            #region Attributes
            if (!string.IsNullOrEmpty(datacontroller))
            {
                controller = datacontroller.ToString();
            }
            if (!string.IsNullOrEmpty(dataaction))
            {
                action = dataaction.ToString();
            }
            #endregion
            return MvcHtmlString.Create(htmlHelper.Kendo().MultiSelect()
                .Name(name)
                .DataTextField("TEXT")
                .DataValueField("VALUE")
                .AutoBind(false)
                .DataSource(source =>
                {
                    source.Read(read =>
                    {
                        read.Action(action, controller);
                    }).ServerFiltering(false);
                })
                .Value((List<MultiSelectResult>)value).ToString()
                      );
        }



        public static MvcHtmlString TextEditorFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
        {
            var data = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            string propertyName = data.PropertyName;
            var Url = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            return MvcHtmlString.Create(htmlHelper.Kendo().Editor()
                      .Name(propertyName)
                      .HtmlAttributes(new { style = "width: auto;height:200px" })
                      .Tools(tools => tools
                          .Clear()
                          .Italic()
                          .Bold()
                          .Underline()
                          .JustifyLeft()
                          .JustifyRight()
                          .JustifyCenter()
                          .JustifyFull()
                          .InsertOrderedList()
                          .InsertUnorderedList()
                          .Indent()
                          .Formatting(formatting => formatting
                                .Add("Header content", ".time")
                                .Add("Sub Title", ".right_absolute")
                            )
                          .InsertFile()
                          .InsertImage()
                          .CreateLink()
                          .SubScript()
                          .SuperScript()
                          .FontName()
                          .FontSize()
                          .FontColor()
                          .BackColor()
                          .ViewHtml()
                      )
                      .StyleSheets(css => css
                        .Add(Url.Content("~/MCContent/css/editorStyles.css"))
                      )
                      .Value(HttpUtility.HtmlDecode(data.Model + ""))
                      .ImageBrowser(imageBrowser => imageBrowser
                        .Image("~/MCContent/Uploads/Editor/{0}")
                        .Read("Read", "Editor")
                        .Create("Create", "Editor")
                        .Destroy("Destroy", "Editor")
                        .Upload("Upload", "Editor")
                        .Thumbnail("Thumbnail", "Editor"))
                     .FileBrowser(fileBrowser => fileBrowser
                         .File("~/MCContent/Uploads/Editor/{0}")
                         .Read("Read", "Editor")
                         .Create("Create", "Editor")
                         .Destroy("Destroy", "Editor")
                         .Upload("Upload", "Editor")
                      ).ToString());
        }
    }
}
