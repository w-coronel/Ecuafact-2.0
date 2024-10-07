var Reports = function () {

    var $form = $("#formConsultas"),
        $div = $("#resultDiv"),
        $btn = $(".search-button"),
        $processing = false,
        handleSearch = function (e) {

            e.preventDefault();

            if ($processing) {
                return false;
            }

            $processing = true;

            var $data = $form.serializeJSON();
            var $url = $("#DocumentType").val();

            $btn.attr("disabled", "disabled");

            KTApp.progress($btn);
            KTApp.block($div);

            $.ajax({
                url: $url,
                method: "post",
                data: $data
            }).done(function (result) {

                if (result) {
                    if (result.includes("kt-login")) {
                        location.reload();
                        return false;
                    }

                    $div.empty();
                    $div.html(result);
                }

            }).always(function () {
                $processing = false;
                KTApp.unprogress($btn);
                KTApp.unblock($div);
                $btn.removeAttr("disabled")
            });

        },

        set_years = function () {
            var obj = $(this)[0];
            if (obj && obj.value.length > 0)
            {
                var sHtmlMonth = "";
                var itemMonths = 12;
                if (obj.value === Reports.year) {
                    itemMonths = parseInt(Reports.month);
                }                
                for (var i = 0; i < itemMonths; i++) {
                    sHtmlMonth += '<option value="' + (i + 1) + '" style = "text-align:left;width:100%;">' + Reports.months[i] + '</option>';
                }
                var listMonths = $(".select-month");
                listMonths.html(sHtmlMonth);
            }
        },

        handleReports = function () {
            var arrows = KTUtil.isRTL() ? {
                isRTL: true,
                leftArrow: '<i class="la la-angle-right"></i>',
                rightArrow: '<i class="la la-angle-left"></i>'
            } : {
                isRTL: false,
                leftArrow: '<i class="la la-angle-left"></i>',
                rightArrow: '<i class="la la-angle-right"></i>'
            };

            $("#kt_datepicker_5").datepicker({
                keyboardNavigation: true,
                format: "dd/mm/yyyy",
                language: "es",
                orientation: "bottom left",
                forceParse: false,
                autoclose: true
            });

            $(".select-year").on("change", set_years);
            $form.on("submit", handleSearch);
        };


    return {
        months: [],
        year: "",
        month:"",
        Init: function () {
            handleReports();
        }
    };


}();



