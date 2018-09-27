using System.Web;
using System.Web.Optimization;

namespace Spotless
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/uploader").Include(
                                    "~/Scripts/blueimp/tmpl.mn.js",
                                    "~/Scripts/blueimp/load-image.mn.js",
                                    "~/Scripts/uploader/jquery.fileupload.js",
                                    "~/Scripts/uploader/jquery.fileupload-process.js",
                                    "~/Scripts/uploader/jquery.fileupload-image.js",
                                    "~/Scripts/uploader/jquery.fileupload-audio.js",
                                    "~/Scripts/uploader/jquery.fileupload-video.js",
                                    "~/Scripts/uploader/jquery.fileupload-validate.js",
                                    "~/Scripts/uploader/jquery.fileupload-ui.js"
                                    ));
        }
    }
}