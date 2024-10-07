using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Optimization;

namespace EcuafactExpress.Web
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterStyles(bundles);
            RegisterScripts(bundles);
        }

        private static void RegisterScripts(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js/jquery").Include("~/assets/plugins/general/jquery/dist/jquery.js"));

            //.Include("~/assets/plugins/general/jquery/dist/jquery.js"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/js/modernizr").Include("~/scripts/modernizr-*"));


            bundles.Add(new ScriptBundle("~/js/vendor")
                
                .Include("~/assets/plugins/general/popper.js/dist/umd/popper.js",
                    "~/assets/plugins/general/bootstrap/dist/js/bootstrap.min.js")
                
                .Include("~/assets/plugins/general/jquery-validation/dist/jquery.validate.js",
                    "~/assets/plugins/general/jquery-validation/dist/additional-methods.js")
                
                .Include("~/assets/plugins/general/js-cookie/src/js.cookie.js",
                    "~/assets/plugins/general/moment/min/moment.min.js",
                    "~/assets/plugins/general/tooltip.js/dist/umd/tooltip.min.js",
                    "~/assets/plugins/general/perfect-scrollbar/dist/perfect-scrollbar.js",
                    "~/assets/plugins/general/wnumb/wNumb.js",
                    "~/assets/plugins/general/jquery-form/dist/jquery.form.min.js",
                    "~/assets/plugins/general/block-ui/jquery.blockUI.js",
                    "~/assets/plugins/general/bootstrap-touchspin/dist/jquery.bootstrap-touchspin.js",
                    "~/assets/plugins/general/bootstrap-maxlength/src/bootstrap-maxlength.js",
                    "~/assets/plugins/general/handlebars/dist/handlebars.js",
                    "~/assets/plugins/general/autosize/dist/autosize.js",
                    "~/assets/plugins/general/clipboard/dist/clipboard.min.js",
                    "~/assets/plugins/general/toastr/build/toastr.min.js")

                .Include("~/scripts/scripts.bundle.min.js"));

            bundles.Add(new ScriptBundle("~/js/datepicker").Include("~/assets/plugins/general/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js",
                    "~/assets/plugins/general/bootstrap-datetime-picker/js/bootstrap-datetimepicker.min.js",
                    "~/assets/plugins/general/bootstrap-timepicker/js/bootstrap-timepicker.min.js",
                    "~/assets/plugins/general/js/global/integration/plugins/bootstrap-datepicker.init.js",
                    "~/assets/plugins/general/js/global/integration/plugins/bootstrap-timepicker.init.js"));

            bundles.Add(new ScriptBundle("~/js/daterangepicker").Include("~/assets/plugins/general/bootstrap-daterangepicker/daterangepicker.js"));

            bundles.Add(new ScriptBundle("~/js/select2").Include("~/assets/plugins/general/select2/dist/js/select2.full.js",
                    "~/assets/plugins/general/plugins/bootstrap-multiselectsplitter/bootstrap-multiselectsplitter.min.js",
                    "~/assets/plugins/general/bootstrap-select/dist/js/bootstrap-select.js",
                    "~/assets/plugins/general/typeahead.js/dist/typeahead.bundle.js"));

            bundles.Add(new ScriptBundle("~/js/dropzone").Include("~/assets/plugins/general/dropzone/dist/dropzone.js",
                    "~/assets/plugins/general/js/global/integration/plugins/dropzone.init.js"));

            bundles.Add(new ScriptBundle("~/js/inputmask").Include("~/assets/plugins/general/inputmask/dist/jquery.inputmask.bundle.js",
                    "~/assets/plugins/general/inputmask/dist/inputmask/inputmask.date.extensions.js",
                    "~/assets/plugins/general/inputmask/dist/inputmask/inputmask.numeric.extensions.js",
                    "~/assets/plugins/general/typeahead.js/dist/typeahead.bundle.js",
                    "~/assets/plugins/bootstrap-tagsinput/dist/bootstrap-tagsinput.js"));

            bundles.Add(new ScriptBundle("~/js/daterangepicker").Include("~/assets/plugins/general/bootstrap-daterangepicker/daterangepicker.js"));

            bundles.Add(new ScriptBundle("~/js/richtexteditor").Include("~/assets/plugins/general/quill/dist/quill.js",
                    "~/assets/plugins/general/summernote/dist/summernote.js",
                    "~/assets/plugins/general/markdown/lib/markdown.js",
                    "~/assets/plugins/general/bootstrap-markdown/js/bootstrap-markdown.js",
                    "~/assets/plugins/general/js/global/integration/plugins/bootstrap-markdown.init.js"));

            bundles.Add(new ScriptBundle("~/js/charts").Include("~/assets/plugins/general/owl.carousel/dist/owl.carousel.js",
                    "~/assets/plugins/general/amcharts/amcharts.js",
                    /*"~/assets/plugins/general/chart.js/dist/Chart.bundle.js",*/

                    "~/assets/plugins/general/amcharts/pie.js",
                    "~/assets/plugins/general/amcharts/xy.js",
                    "~/assets/plugins/general/amcharts/serial.js",
                    "~/assets/plugins/general/amcharts/plugins/responsive/responsive.js",
                    "~/assets/plugins/general/amcharts/plugins/dataloader/dataloader.js",
                    "~/assets/plugins/general/amcharts/plugins/export/export.js",
                    "~/assets/plugins/general/amcharts/plugins/export/lang/es.js",

                    "~/assets/plugins/general/amcharts/themes/dark.js",
                    "~/assets/plugins/general/amcharts/themes/light.js",
                    "~/assets/plugins/general/amcharts/themes/patterns.js",
                    "~/assets/plugins/general/amcharts/lang/es.js",

                    "~/assets/plugins/general/raphael/raphael.js",
                    "~/assets/plugins/general/morris.js/morris.js",
                    "~/assets/plugins/general/counterup/jquery.counterup.js" ));

            bundles.Add(new ScriptBundle("~/js/plugins").Include("~/assets/plugins/general/dompurify/dist/purify.js",
                    "~/assets/plugins/general/plugins/bootstrap-session-timeout/dist/bootstrap-session-timeout.min.js",
                    "~/assets/plugins/general/plugins/jquery-idletimer/idle-timer.min.js",
                    "~/assets/plugins/general/waypoints/lib/jquery.waypoints.js",
                    "~/assets/plugins/general/es6-promise-polyfill/promise.min.js",
                    "~/assets/plugins/general/sweetalert2/dist/sweetalert2.min.js",
                    "~/assets/plugins/general/js/global/integration/plugins/sweetalert2.init.js",
                    "~/assets/plugins/general/jquery.repeater/src/lib.js",
                    "~/assets/plugins/general/jquery.repeater/src/jquery.input.js",
                    "~/assets/plugins/general/jquery.repeater/src/repeater.js",
                    "~/assets/plugins/general/js/global/integration/plugins/jquery-validation.init.js"));

            bundles.Add(new ScriptBundle("~/js/jqueryui").Include("~/assets/plugins/custom/plugins/jquery-ui/jquery-ui.min.js"));

            bundles.Add(new ScriptBundle("~/js/fullcalendar").Include(
                    "~/assets/plugins/custom/fullcalendar/core/main.js",
                    "~/assets/plugins/custom/fullcalendar/daygrid/main.js",
                    "~/assets/plugins/custom/fullcalendar/google-calendar/main.js",
                    "~/assets/plugins/custom/fullcalendar/interaction/main.js",
                    "~/assets/plugins/custom/fullcalendar/list/main.js",
                    "~/assets/plugins/custom/fullcalendar/timegrid/main.js"));

            bundles.Add(new ScriptBundle("~/js/jquery.flot").Include("~/assets/plugins/custom/flot/dist/es5/jquery.flot.js",
                    "~/assets/plugins/custom/flot/source/jquery.flot.resize.js",
                    "~/assets/plugins/custom/flot/source/jquery.flot.categories.js",
                    "~/assets/plugins/custom/flot/source/jquery.flot.pie.js",
                    "~/assets/plugins/custom/flot/source/jquery.flot.stack.js",
                    "~/assets/plugins/custom/flot/source/jquery.flot.crosshair.js",
                    "~/assets/plugins/custom/flot/source/jquery.flot.axislabels.js"));

            bundles.Add(new ScriptBundle("~/js/datatables.net").Include(
                    "~/assets/plugins/custom/jszip/dist/jszip.min.js",
                    "~/assets/plugins/custom/pdfmake/build/pdfmake.min.js",
                    "~/assets/plugins/custom/pdfmake/build/vfs_fonts.js",
                    "~/assets/plugins/custom/datatables.net/js/jquery.dataTables.js",
                    "~/assets/plugins/custom/datatables.net-bs4/js/dataTables.bootstrap4.js",
                    "~/assets/plugins/custom/js/global/integration/plugins/datatables.init.js",
                    "~/assets/plugins/custom/datatables.net-autofill/js/dataTables.autoFill.min.js",
                    "~/assets/plugins/custom/datatables.net-autofill-bs4/js/autoFill.bootstrap4.min.js",
                    "~/assets/plugins/custom/datatables.net-buttons/js/dataTables.buttons.min.js",
                    "~/assets/plugins/custom/datatables.net-buttons-bs4/js/buttons.bootstrap4.min.js",
                    "~/assets/plugins/custom/datatables.net-buttons/js/buttons.colVis.js",
                    "~/assets/plugins/custom/datatables.net-buttons/js/buttons.flash.js",
                    "~/assets/plugins/custom/datatables.net-buttons/js/buttons.html5.js",
                    "~/assets/plugins/custom/datatables.net-buttons/js/buttons.print.js",
                    "~/assets/plugins/custom/datatables.net-colreorder/js/dataTables.colReorder.min.js",
                    "~/assets/plugins/custom/datatables.net-fixedcolumns/js/dataTables.fixedColumns.min.js",
                    "~/assets/plugins/custom/datatables.net-fixedheader/js/dataTables.fixedHeader.min.js",
                    "~/assets/plugins/custom/datatables.net-keytable/js/dataTables.keyTable.min.js",
                    "~/assets/plugins/custom/datatables.net-responsive/js/dataTables.responsive.min.js",
                    "~/assets/plugins/custom/datatables.net-responsive-bs4/js/responsive.bootstrap4.min.js",
                    "~/assets/plugins/custom/datatables.net-rowgroup/js/dataTables.rowGroup.min.js",
                    "~/assets/plugins/custom/datatables.net-rowreorder/js/dataTables.rowReorder.min.js",
                    "~/assets/plugins/custom/datatables.net-scroller/js/dataTables.scroller.min.js",
                    "~/assets/plugins/custom/datatables.net-select/js/dataTables.select.min.js"));

            bundles.Add(new ScriptBundle("~/js/jstree").Include("~/assets/plugins/custom/jstree/dist/jstree.js"));
             
            bundles.Add(new ScriptBundle("~/js/maps").Include("~/assets/plugins/custom/gmaps/gmaps.js",
                    "~/assets/plugins/custom/jqvmap/dist/jquery.vmap.js",
                    "~/assets/plugins/custom/jqvmap/dist/maps/jquery.vmap.world.js",
                    "~/assets/plugins/custom/jqvmap/dist/maps/jquery.vmap.russia.js",
                    "~/assets/plugins/custom/jqvmap/dist/maps/jquery.vmap.usa.js",
                    "~/assets/plugins/custom/jqvmap/dist/maps/jquery.vmap.germany.js",
                    "~/assets/plugins/custom/jqvmap/dist/maps/jquery.vmap.europe.js"));

            bundles.Add(new ScriptBundle("~/js/layout").Include("~/scripts/layout.js"));

            bundles.Add(new ScriptBundle("~/js/auth").Include("~/scripts/auth.js"));
            //bundles.Add(new ScriptBundle("~/js/issuers").Include("~/scripts/emisor.js"));

            bundles.Add(new ScriptBundle("~/js/profile").Include("~/scripts/profile.js"));
            bundles.Add(new ScriptBundle("~/js/documents").Include("~/scripts/comprobantes.js"));
            
            bundles.Add(new ScriptBundle("~/js/invoices").Include("~/scripts/invoice.js"));
            bundles.Add(new ScriptBundle("~/js/settlement").Include("~/scripts/settlement.js"));

            bundles.Add(new ScriptBundle("~/js/referralguides").Include("~/scripts/Emision/guiaRemisionScripts.js"));
            bundles.Add(new ScriptBundle("~/js/retentions").Include("~/scripts/Emision/retencionScripts.js"));
            bundles.Add(new ScriptBundle("~/js/settlements").Include("~/scripts/Emision/liquidacionScripts.js"));
            bundles.Add(new ScriptBundle("~/js/creditnotes").Include("~/scripts/Emision/notaCreditoScripts.js"));
            bundles.Add(new ScriptBundle("~/js/facturas").Include("~/scripts/Emision/facturaScripts.js"));
            bundles.Add(new ScriptBundle("~/js/debitnote").Include("~/scripts/Emision/notaDebito.js"));
            bundles.Add(new ScriptBundle("~/js/creditnote").Include("~/scripts/Emision/notaCredito.js"));
            bundles.Add(new ScriptBundle("~/js/retention").Include("~/scripts/Emision/retencion.js"));
            bundles.Add(new ScriptBundle("~/js/referralguide").Include("~/scripts/Emision/guiaRemision.js"));
            bundles.Add(new ScriptBundle("~/js/salesNote").Include("~/scripts/Emision/salesNote.js"));

            bundles.Add(new ScriptBundle("~/js/contributors").Include("~/scripts/contribuyentes.js"));
            bundles.Add(new ScriptBundle("~/js/config").Include("~/scripts/config.js"));
            bundles.Add(new ScriptBundle("~/js/invoiceOrder").Include("~/scripts/InvoiceOrder.js"));

            bundles.Add(new ScriptBundle("~/js/issuer").Include("~/scripts/emisor.js"));


            bundles.Add(new ScriptBundle("~/js/layerslider").Include(
                "~/assets/slider/layerslider/js/jquery.js",
                "~/assets/slider/layerslider/js/greensock.js",
                "~/assets/slider/layerslider/js/layerslider.transitions.js",
                "~/assets/slider/layerslider/js/layerslider.kreaturamedia.jquery.js"));
        }

        private static void RegisterStyles(BundleCollection bundles)
        {

            bundles.Add(new StyleBundle("~/css/datepicker").Include("~/assets/plugins/general/bootstrap-datepicker/dist/css/bootstrap-datepicker3.css",
                    "~/assets/plugins/general/bootstrap-datetime-picker/css/bootstrap-datetimepicker.css",
                    "~/assets/plugins/general/bootstrap-timepicker/css/bootstrap-timepicker.css"));

            bundles.Add(new StyleBundle("~/css/fonts/socicon").Include("~/assets/plugins/general/socicon/css/socicon.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/css/fonts/line-awesome").Include("~/assets/plugins/general/plugins/line-awesome/css/line-awesome.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/css/fonts/flaticon").Include("~/assets/plugins/general/plugins/flaticon/flaticon.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/css/fonts/flaticon2").Include("~/assets/plugins/general/plugins/flaticon2/flaticon.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/css/fonts/fontawesome").Include("~/assets/plugins/general/fortawesome/fontawesome-free/css/all.min.css", new CssRewriteUrlTransform()));


            bundles.Add(new StyleBundle("~/css/plugins").Include("~/assets/plugins/general/perfect-scrollbar/css/perfect-scrollbar.css",
                    "~/assets/plugins/general/tether/dist/css/tether.css",
                    "~/assets/plugins/general/bootstrap-touchspin/dist/jquery.bootstrap-touchspin.css",
                    "~/assets/plugins/general/animate.css/animate.css",
                    "~/assets/plugins/general/toastr/build/toastr.css",
                    "~/assets/plugins/general/sweetalert2/dist/sweetalert2.css"));
            

            bundles.Add(new StyleBundle("~/css/charts").Include("~/assets/plugins/general/morris.js/morris.css",
                    "~/assets/plugins/general/amcharts/plugins/export/export.css",
                    "~/assets/plugins/general/owl.carousel/dist/assets/owl.carousel.css",
                    "~/assets/plugins/general/owl.carousel/dist/assets/owl.theme.default.css"));

            bundles.Add(new StyleBundle("~/css/jqueryui").Include("~/assets/plugins/custom/plugins/jquery-ui/jquery-ui.min.css"));
            bundles.Add(new StyleBundle("~/css/select2").Include("~/assets/plugins/general/select2/dist/css/select2.css"));
            bundles.Add(new StyleBundle("~/css/dropzone").Include("~/assets/plugins/general/dropzone/dist/dropzone.css"));
            bundles.Add(new StyleBundle("~/css/daterangepicker").Include("~/assets/plugins/general/bootstrap-daterangepicker/daterangepicker.css"));
            bundles.Add(new StyleBundle("~/css/fullcalendar").Include("~/assets/plugins/custom/fullcalendar/core/main.css",
                    "~/assets/plugins/custom/fullcalendar/daygrid/main.css",
                    "~/assets/plugins/custom/fullcalendar/list/main.css",
                    "~/assets/plugins/custom/fullcalendar/timegrid/main.css"));

            bundles.Add(new StyleBundle("~/css/datatables").Include("~/assets/plugins/custom/datatables.net-bs4/css/dataTables.bootstrap4.css",
                    "~/assets/plugins/custom/datatables.net-buttons-bs4/css/buttons.bootstrap4.min.css",
                    "~/assets/plugins/custom/datatables.net-colreorder-bs4/css/colReorder.bootstrap4.min.css",
                    "~/assets/plugins/custom/datatables.net-fixedcolumns-bs4/css/fixedColumns.bootstrap4.min.css",
                    "~/assets/plugins/custom/datatables.net-fixedheader-bs4/css/fixedHeader.bootstrap4.min.css",
                    "~/assets/plugins/custom/datatables.net-responsive-bs4/css/responsive.bootstrap4.min.css",
                    "~/assets/plugins/custom/datatables.net-keytable-bs4/css/keyTable.bootstrap4.min.css",
                    "~/assets/plugins/custom/datatables.net-rowgroup-bs4/css/rowGroup.bootstrap4.min.css",
                    "~/assets/plugins/custom/datatables.net-rowreorder-bs4/css/rowReorder.bootstrap4.min.css",
                    "~/assets/plugins/custom/datatables.net-scroller-bs4/css/scroller.bootstrap4.min.css",
                    "~/assets/plugins/custom/datatables.net-select-bs4/css/select.bootstrap4.min.css",
                    "~/assets/plugins/custom/datatables.net-autofill-bs4/css/autoFill.bootstrap4.min.css"));

            bundles.Add(new StyleBundle("~/css/inputmask").Include("~/assets/plugins/bootstrap-tagsinput/dist/bootstrap-tagsinput.css"));

            bundles.Add(new StyleBundle("~/css/jstree").Include("~/assets/plugins/custom/jstree/dist/themes/default/style.css"));
            bundles.Add(new StyleBundle("~/css/maps").Include("~/assets/plugins/custom/jqvmap/dist/jqvmap.css"));
            bundles.Add(new StyleBundle("~/css/site").Include("~/content/style.min.css", "~/content/site.css"));
            bundles.Add(new StyleBundle("~/css/auth").Include("~/Content/login.min.css", "~/Content/auth.css"));
            bundles.Add(new StyleBundle("~/css/issuers").Include("~/content/configStyles.css", "~/assets/css/pages/wizard/wizard-2.css" ));
            bundles.Add(new StyleBundle("~/css/invoice").Include("~/content/invoice-1.css"));

            bundles.Add(new StyleBundle("~/css/pricing").Include("~/assets/css/pages/pricing/pricing-1.css",
                   "~/assets/css/pages/pricing/pricing-2.css",
                   "~/assets/css/pages/pricing/pricing-3.css",
                   "~/assets/css/pages/pricing/pricing-4.css"));

            bundles.Add(new StyleBundle("~/css/layerslider").Include(
                "~/assets/slider/documentation/assets/css/doc.css",
                "~/assets/slider/documentation/assets/css/font.css",
                "~/assets/slider/layerslider/css/layerslider.css",
                "~/assets/slider/layerslider/skins/fullwidth/skin.css"));
        }
    }
}
