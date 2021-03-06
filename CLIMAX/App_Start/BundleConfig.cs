﻿using System.Web;
using System.Web.Optimization;

namespace CLIMAX
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                     "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/jquery.min.js",
                      "~/Scripts/jquery.easing.min.js",
                      "~/Scripts/jquery.scrollTo.js",
                      "~/Scripts/jquery.appear.js",
                      "~/Scripts/stellar.js",
                      "~/Scripts/nivo-lightbox.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                       "~/Content/bootstrap-3.3.2-dist/*.css",
                       "~/Content/bootstrap-3.3.2-dist/*.map",
                       "~/Content/font-awesome.min.css",
                       "~/Content/nivo-lightbox.css",
                      "~/Content/Site.css",
                       "~/Content/style.css",
                      "~/Content/Moses_custom.css",
                      "~/Content/Logstyle.css"));
        }
    }
}
