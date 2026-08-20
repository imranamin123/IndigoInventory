using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace GL
{
    // System.Web.Optimization ships no "keep Include() order" orderer of its own: the only built-in
    // implementation is DefaultBundleOrderer, which reorders files by matching filenames against a
    // fixed set of "name-{version}" patterns for well-known libraries (jquery-{version}.js, etc). Bower
    // vendor files are named without a version (jquery.min.js, jquery-ui.js) so they don't match those
    // patterns and get shuffled unpredictably instead of left alone. This orderer just passes files
    // through in the order Bundle.Include() specified them.
    public class AsIsBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }

    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Force real bundling/minification even when <compilation debug="true">,
            // since the whole point of these bundles is to collapse the ~35 separate
            // requests _Layout.cshtml used to issue into a handful of combined ones.
            BundleTable.EnableOptimizations = true;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            // Combined stylesheet for the shared _Layout. Uses plain Bundle (no minifier) plus
            // CssRewriteUrlTransform per file so each stylesheet's relative url()/@font-face
            // references (fonts, background images) keep resolving correctly once everything
            // is served from the single ~/bundles/appcss virtual path.
            var appCss = new Bundle("~/bundles/appcss");
            // The bower/vendor files here (jquery-ui.css, AdminLTE.min.css, etc.) don't follow the
            // "name-{version}" convention System.Web.Optimization's DefaultBundleOrderer expects, so
            // its heuristic reordering silently scrambles cascade order (e.g. jquery-ui.css ends up
            // before bootstrap.min.css). Preserve the exact Include() order below instead.
            appCss.Orderer = new AsIsBundleOrderer();
            appCss.Include(
                "~/admin-lte/bower_components/bootstrap/dist/css/bootstrap.min.css",
                new CssRewriteUrlTransform());
            appCss.Include(
                "~/css/select2.min.css",
                new CssRewriteUrlTransform());
            appCss.Include(
                "~/admin-lte/bower_components/font-awesome/css/font-awesome.min.css",
                new CssRewriteUrlTransform());
            appCss.Include(
                "~/admin-lte/bower_components/Ionicons/css/ionicons.min.css",
                new CssRewriteUrlTransform());
            appCss.Include(
                "~/admin-lte/bower_components/datatables.net-bs/css/dataTables.bootstrap.min.css",
                new CssRewriteUrlTransform());
            appCss.Include(
                "~/admin-lte/dist/css/AdminLTE.min.css",
                new CssRewriteUrlTransform());
            appCss.Include(
                "~/admin-lte/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css",
                new CssRewriteUrlTransform());
            appCss.Include(
                "~/admin-lte/dist/css/skins/_all-skins.min.css",
                new CssRewriteUrlTransform());
            appCss.Include(
                "~/admin-lte/bower_components/jquery-ui/themes/base/jquery-ui.css",
                new CssRewriteUrlTransform());
            appCss.Include(
                "~/admin-lte/dist/css/skins/skin-blue.min.css",
                new CssRewriteUrlTransform());
            appCss.Include(
                "~/Style/style.css",
                new CssRewriteUrlTransform());
            bundles.Add(appCss);

            // Combined script bundle for the shared _Layout. Plain Bundle (no minifier) since
            // every file here is already a vendor .min.js (or, for jquery-ui.js/common.js/etc,
            // small enough that re-minifying isn't worth the risk of the minifier mangling
            // already-processed code). Order matches each library's real dependency order.
            var appJs = new Bundle("~/bundles/appjs");
            // Same issue as appCss: unversioned bower filenames (jquery.min.js, jquery-ui.js) don't
            // match DefaultBundleOrderer's "name-{version}" patterns, so its reordering was putting
            // jquery-ui.js ahead of jquery.min.js and leaving jQuery undefined on every page.
            appJs.Orderer = new AsIsBundleOrderer();
            appJs.Include(
                "~/admin-lte/bower_components/jquery/dist/jquery.min.js",
                "~/admin-lte/bower_components/jquery-ui/jquery-ui.js",
                "~/admin-lte/bower_components/bootstrap/dist/js/bootstrap.min.js",
                "~/admin-lte/dist/js/adminlte.min.js",
                "~/admin-lte/bower_components/datatables.net/js/jquery.dataTables.min.js",
                "~/admin-lte/bower_components/datatables.net-bs/js/dataTables.bootstrap.min.js",
                "~/js/select2.min.js",
                "~/admin-lte/bower_components/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/admin-lte/bower_components/fastclick/lib/fastclick.js",
                "~/admin-lte/dist/js/demo.js",
                "~/admin-lte/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js",
                "~/js/Common/common.js",
                "~/admin-lte/bower_components/jquery-ui/ui/widgets/sortable.js");
            bundles.Add(appJs);
        }
    }
}
