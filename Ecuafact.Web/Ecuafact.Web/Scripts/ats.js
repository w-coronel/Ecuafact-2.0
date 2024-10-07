var Ats = function () {

    var $btn;
    var ext =".xlsx"
    var $form = $("#formConsultas"),
        $div = $("#resultDiv"),
        $btn = $(".search-button"),
        $btn2 = $(".btn-report-xml"),
        $btn1 = $(".btn-report-excel"),
        $processing = false,
        handleSearch = function (e) {

            e.preventDefault();
            if ($processing) {
                return false;
            }
            $processing = true;
            var $data = $form.serializeJSON();
            var $url = Ats.urlCompras;
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
                    handleEstadistica();
                }

            }).always(function () {
                $processing = false;
                KTApp.unprogress($btn);
                KTApp.unblock($div);
                $btn.removeAttr("disabled")
            });

        },
        handleEstadistica = function(){
            $.get(Ats.urlEstadistica, {}, function (result) {
                if (result)
                {
                    $(".document-total").text(result.CountDocument);
                    $(".document-factura").text(result.Factura);
                    $(".document-notacredito").text(result.NotaCredito);
                    $(".document-notadebito").text(result.NotaDebito);
                    $(".document-retention").text(result.Retencion);                   
                }
            });
        },
        handleExcel = function (e) {
            var urlDescarga = Ats.urlAts;
            var formato = "excel";
            var periodType = $("#QueryType").val();
            var year = $("#Year").val();
            var month = $("#Month").val();
            var semester = $("#Period").val();
            urlDescarga += "?format=" + formato + "&periodType=" + periodType + "&year=" + year + "&month=" + month + "&semester=" + semester;
            $(".btn-report-excel").attr("href", urlDescarga);
        },
        valError = function (msg) {           
            Swal.fire("Validacion de Datos", msg, "warning");
            return false;
        },
        handleXml = function (e) {
            var resutl = handleDataValidate();
            if (resutl !== null) {
                return valError(resutl);
            }
            handleDataSendRequest();            
        },
        handleDownloadXml = function (e) {            
            var urlDescarga = Ats.urlAts;
            var formato = "xml";
            var periodType = $("#QueryType").val();
            var year = $("#Year").val();
            var month = $("#Month").val();
            var semester = $("#Period").val();
            urlDescarga += "?format=" + formato + "&periodType=" + periodType + "&year=" + year + "&month=" + month + "&semester=" + semester;           
            window.open(urlDescarga, '_xml');            
        },
        set_years = function () {
            var obj = $(this)[0];
            if (obj && obj.value.length > 0) {
                var sHtmlMonth = "";
                var itemMonths = 12;
                if (obj.value === Ats.year) {
                    itemMonths = parseInt(Ats.month);
                }
                for (var i = 0; i < itemMonths; i++) {
                    sHtmlMonth += '<option value="' + (i + 1) + '" style = "text-align:left;width:100%;">' + Ats.months[i] + '</option>';
                }
                var listMonths = $(".select-month");
                listMonths.html(sHtmlMonth);
            }
        },
        set_periods = function () {
            var obj = $(this)[0];
            $(".periodo-label").text("Mes");
            $(".periodo-mes").show();
            $(".periodo-semestral").hide();
            if (obj && obj.value.length > 0) {
                if (obj.value === "2") {
                    $(".periodo-semestral").show();
                    $(".periodo-label").text("Semestre");
                    $(".periodo-mes").hide();
                }
            }
        },
        handleData = function (id, cod) {
            Ats.documents.map(function (data) {
                if (data.Id == parseInt(id)) {
                    data.CodSustento = cod;
                }
            });
        },
        handleDataValidate = function () {           
            for (var i = 0; i < Ats.documents.length; i++) {
                if (Ats.documents[i].CodSustento == "") {
                    return "Para generar el xml del ATS todos los documentos (facturas, notas de venta y liquidaciones) deben estar clasificados según identificación sustento tributario";
                }                
            }
            return null;
        },
        handleDataSendRequest = function () {          
            var txt = "Recuarda verificar la información antes de generar el xml para el ATS. Desea continuar ? ";
            Swal.fire({
                title: 'Generar xml del ATS',
                text: txt,
                icon: 'info',
                type: 'warning',
                showCancelButton: true,
                cancelButtonText: 'Cancelar',
                confirmButtonText: 'Aceptar'

            }).then(function (result) {
                if (result.value) {
                    handleDownloadXml();
                }               
            });
        },
        handleAts = function () {
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

            $(".tipo-periodo").on("change", set_periods);
            $(".select-year").on("change", set_years);
            $form.on("submit", handleSearch);
            $(".btn-report-excel").on("click", handleExcel);
            $(".btn-report-xml").on("click", handleXml);           
        };

    return {
        documents:[],
        months: [],
        year: "",
        month: "",
        urlAts: "",
        urlCompras: "",
        urlTipoSustento: "",
        urlSustento: "",
        urlImgan: "",
        urlEstadistica:"",
        sustenanceTypes:[],
        SetTipoSustento: function (documentId, emissionType) {
            if (!documentId) {
                console.log("El número de documento no existe");
                return false;
            }
            showLoader();
            $.get(Ats.urlTipoSustento + "?id=" + documentId + "&emissionType=" + emissionType, {}, function (data) {

                if (data.includes("modal-dialog")) {
                    $("#myModal").html(data);
                    $("#myModal").fadeIn();
                    $("#myModal").modal("show");
                }
                else {
                    window.open(urlLogin, "_login");
                }

            }).always(function () {
                hideLoader();
            });
        },
        Sustento: function (pkDoc, codSustento, emissionType) {
            if (!pkDoc) {
                toastr.error("El número de documento no existe");
                return false;
            }
            if (!codSustento) {
                toastr.error("Debe seleccionar el tipo sustento del documento");
                return false;
            }
            showLoader();
            $.post(Ats.urlSustento, { "id": pkDoc, "supTypeCode": codSustento, "emissionType": emissionType }, function (data) {
                var $sustento = Ats.sustenanceTypes.find(o => o.Codigo == codSustento);
                handleData(pkDoc, codSustento);
                $("#pk_" + pkDoc).attr("title", $sustento.Nombre);
                $("#pk_" + pkDoc).attr("data-order", $sustento.Codigo);
                $("#img_" + pkDoc).attr("src", Ats.urlImgan);
                toastr.success("Se ha guardado con éxito el registro!");
                $("#myModal").fadeOut();
                $("#myModal").modal("hide");
                $("#myModal").empty();

            }).fail(function (e, d, x) {
                toastr.error("No se guardo el registro!");
            }).always(function () {
                hideLoader();
            });
        },
        Init: function () {
            handleAts();           
        }
    };


}();