var Dashboard = function () {
    var generateDocumentTable = function (elementId, urlAjax) {
        if ($(elementId)) {
            $.ajax({
                method: "GET",
                url: urlAjax
            }).done(function (data) {

                $(elementId).DataTable({
                    "destroy": true,
                    "data": data,
                    "order": [[4, 'asc']],
                    "language": {
                        "url": "./scripts/dt_spanish.json"
                    },
                    "columns": [
                        {
                            data: 'authorizationNumber',
                            "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                                var retencionHtml = "<button class='btn btn-danger'  data-toggle='tooltip' title='PDF' role='button' target='_blank' tooltip='PDF' " +
                                    "onclick=\"window.open('" + oData.pdf + "')\"><i class='fa fa-file-pdf-o'></i></button>";

                                $(nTd).html(retencionHtml);
                            }
                        },
                        {
                            data: 'docNumber',
                            "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                                $(nTd).html("<a  data-toggle='tooltip' title='Descargar PDF' role='button' target='_blank' tooltip='Descargar PDF' " +
                                    "href='" + oData.url + "'>" + oData.docNumber + "</a>");
                            }
                        },
                        {
                            data: 'identificationNumber',
                            "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                                $(nTd).html("<span title='RUC: " + oData.identificationNumber + "'>" + oData.name + "</span>");
                            }
                        },
                        { data: 'date' },
                        { data: 'total' }
                    ]
                });

                $(elementId).css("width", "100%");
            });
        }
    };
     
    var configureDocTables = function (urlEmitidos, urlRecibidos) {
        generateDocumentTable('#table_emitidos_facturas', urlEmitidos +"/01");
        generateDocumentTable('#table_emitidos_retenciones', urlEmitidos + "/07");
        generateDocumentTable('#table_emitidos_notascredito', urlEmitidos + "/04");
        generateDocumentTable('#table_emitidos_guiasremision', urlEmitidos + "/06");

        generateDocumentTable('#table_recibidos_facturas', urlRecibidos + "/01");
        generateDocumentTable('#table_recibidos_retenciones', urlRecibidos + "/07");
        generateDocumentTable('#table_recibidos_notascredito', urlRecibidos + "/04");
        generateDocumentTable('#table_recibidos_guiasremision', urlRecibidos + "/06");
    };

    var generatePieChart = function (element, data) {
        if (data && document.getElementById(element)) {
            var chartEmitidos = am4core.create(element, am4charts.PieChart);
            chartEmitidos.data = data;

            // Add and configure Series
            var pieSeries = chartEmitidos.series.push(new am4charts.PieSeries());
            pieSeries.dataFields.value = "Total";
            pieSeries.dataFields.category = "Tipo";

            // Set up tooltips
            pieSeries.tooltip.label.interactionsEnabled = true;
            pieSeries.tooltip.keepTargetHover = true;

        }
    };

    var configureEmitidos = function (data) {
        generatePieChart("statistics_1", data); 
    }

    var configureRecibidos = function (data) {
        if (data) {
            var rec_fa = data.find(p => p.Tipo.includes("FACT"));
            var rec_nc = data.find(p => p.Tipo.includes("CR"));
            var rec_rt = data.find(p => p.Tipo.includes("RET"));
            var rec_gr = data.find(p => p.Tipo.includes("GUI"));

            rec_fa && $("#statistics_2factura") && $("#statistics_2factura").html("$ " + rec_fa.Total);
            rec_nc && $("#statistics_2notacredito") && $("#statistics_2notacredito").html("$ " + rec_nc.Total);
            rec_rt && $("#statistics_2retencion") && $("#statistics_2retencion").html("$ " + rec_rt.Total);
            rec_gr && $("#statistics_2guia") && $("#statistics_2guia").html("$ " + rec_gr.Total);
             
            generatePieChart("statistics_2", data);  
        }
    }


    return {

        UrlEmitidos: "",
        UrlRecibidos: "",
        Recibidos: null,
        Emitidos: null,
        Init: function () {

            configureDocTables(this.UrlEmitidos, this.UrlRecibidos);

            configureEmitidos(this.Emitidos);

            configureRecibidos(this.Recibidos); 
        }
    };
}();
