
var Gastos = function () {
     


    var generatePieChartMethod = function (chartData, divName) {

        if (!divName) {
            divName = "chartDiv";
        }
        if (!chartData || !chartData.length || chartData.length < 1) {
            chartData = [
                { "label": "Alimentacion", "value": 0, "color": "#fec107"},
                { "label": "Educacion", "value": 0, "color": "#9a29ad" },
                { "label": "Salud", "value": 0, "color": "#2097f5" },
                { "label": "Sin Clasificar", "value": 0, "color": "#9e9e9e" },
                { "label": "Vestimenta", "value": 0, "color": "#f34638" },
                { "label": "Vivienda", "value": 0, "color": "#009788" }
            ];
        }

        $(document).ready(function () {

            var chart = AmCharts.makeChart(divName, {
                "type": "pie",
                "startEffect": "elastic",
                "startDuration": 1,
                "labelRadius": "1%",
                "theme": "light",
                "radius" : "30%",
                "innerRadius": "50%",
                "addClassNames": true,

                "legend": {
                    "position": "bottom",
                    "marginBottom": 0,
                    "valueText": "$ [[value]]",
                    "autoMargins": false,
                    "maxColumns": "2",
                    "align" : "center"
                },
                "balloon": {
                    "drop": true,
                    "adjustBorderColor": false,
                    "color": "#FFFFFF",
                    "fontSize": "8"
                },
                "dataProvider": chartData,
                "balloonText": "[[title]]: $ [[value]]",
                "titleText": "",
                "labelText": "",
                "valueField": "value",
                "titleField": "label",
                "colorField": "color",
                "export": {
                    "enabled": true,
                    "menu": [{
                        "class": "export-main",
                        menu: [{
                            label: "Descargar como ...",
                            menu: ["PNG", "JPG", "SVG", "PDF"]
                        }, {
                            label:"Guardar como ...",
                            menu: ["CSV", "XLSX", "JSON"]
                        }]
                    }]
                },
                "language" : "es"
            });

           

            var handleInit = function () {
                if (chart.legend) {
                    chart.legend.addListener("rollOverItem", handleRollOver);
                }
            };

            var handleRollOver = function (e) {
                if (e.dataItem && e.dataItem.wedge) {
                    var wedge = e.dataItem.wedge.node;
                    wedge.parentNode.appendChild(wedge);
                }
            };

            chart.addListener("init", handleInit);

            chart.addListener("rollOverSlice", function (e) {
                handleRollOver(e);
            });

              

            //Morris.Donut({
            //    element: divName,
            //    data: chartData,
            //    labelColor: '#040029',
            //    colors: [
            //        '#fff203', // Alimentacion
            //        '#e100ff', // Educacion
            //        '#006eff', // Salud
            //        '#9c9c9c', // Sin Clasificar
            //        '#ff0000', // Vestimenta
            //        '#09b300', // Vivienda
            //        '#ffcc99',
            //        '#ff99fa',
            //        '#e299ff'
            //    ],
            //    resize: true,
            //    formatter: function (x) { return '$ '+(x*1).toFixed(2) }
            //});
        });

    };


    var getDeductibleChart = function (options) {
        var url = options.url, element = options.element;

        var $grid = $("#" + element);

        KTApp.block($grid, { message: "Cargando..." });


        $.ajax({
            url: url,
            type: 'POST',
            data: options,
            success: function (dataReceived) {
                if (dataReceived) {
                    if (dataReceived.includes("graficoDeducibles")) {
                        $grid.fadeOut("fast");
                        $grid.empty().html(dataReceived);
                        $grid.fadeIn("slow");
                    }
                    else {
                        window.open(urlLogin, "_login")
                    }
                }
                else {
                    Swal.fire("Error", "No se pudo realizar la consulta", "error");
                }
            },
            error: function (result, doc, meta) {
                debugger;
                Swal.fire("Error", "No se pudo realizar la consulta", "error");
            }
        }).always(function () {
            KTApp.unblock($grid);

        });
    };

    var handleDeductibles = function () {
        var formControls = $(".form-control");
        formControls.on("change", function () {
            var $form = $("#deductiblesForm");

            $form.submit();
        });
    };


    return {
        deductibleReport: {
            url: "",
            year: 0,
            month: 0,
            element: "deductibleDiv"
        },

        init: function () {
            //// this.refreshDeductibles();
            handleDeductibles();
        },

        generatePieChart: function (data, element) {
            return generatePieChartMethod(data, element);
        },

        refreshDeductibles: function () {
            getDeductibleChart(this.deductibleReport);
        },

        refreshDetails: function ($url) {

            $.get($url + "?year=" + Gastos.deductibleReport.year + "&month=" + Gastos.deductibleReport.month)
                .done(function (data) {
                    $(".budget-details").empty().html(data);
                });
        }
    };

}();