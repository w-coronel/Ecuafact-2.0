
var Comprobantes = function () {
    var EmailUrl = "",
        urlSearch = "",
        tableSearch = "",
        urlLogin = "",
        cancelDocumentlUrl = "",
        htmlmsg = "";
        performSearch = false;

    var $grid = $("#divConsultas");
    var $form = $('#formConsultas'); 

    var SendEmail = function (id) {
        var uid;
        if (performSearch) {
            var $me = $(this);
            uid = $me.attr("data-uid");
        }
        else {
             uid = id;
        }

        if (uid) {           
            Swal.fire({
                title: 'Reenviar correo',
                text: 'Ingrese el correo electrónico al que usted desea enviar el documento:',
                input: 'text',
                type: 'info',
                inputAttributes: {
                    autocapitalize: 'off'
                },
                showCancelButton: true,
                cancelButtonText: 'Volver',
                confirmButtonText: 'Enviar',
                showLoaderOnConfirm: true,
                preConfirm: (data) => {

                    if (!validarEmail(data)) {
                        Swal.showValidationMessage("El correo electrónico no es válido");
                        return false;
                    }

                    return $.post(EmailUrl, { email: data, uid: uid }).then(response => {
                        console.log(response);
                        if (!response.IsSuccess) {
                            throw new Error(response.UserMessage)
                        }

                        return response.IsSuccess;
                    }).catch(error => {
                        Swal.showValidationMessage(
                            `No se pudo enviar el mensaje: ${error}`
                        )
                    });
                },
                allowOutsideClick: () => !Swal.isLoading()
            }).then((result) => {
                if (result.value) {
                    Swal.fire({
                        title: 'Mensaje fue enviado con éxito!',
                        type: 'success'
                    })
                }
            })
        }
    };

    var SendCancelDocument = function (id) {
        var uid;
        uid = id;
        if (id) {
            Swal.fire({
                title: '¿Desea usted anular este documento?',
                html: Comprobantes.htmlmsg, 
                type: 'warning',
                inputAttributes: {
                    autocapitalize: 'off'
                },
                showCancelButton: true,
                cancelButtonText: 'Cancelar', 
                confirmButtonText: 'Continuar',
                showLoaderOnConfirm: true,
                preConfirm: (data) => {                  
                    return $.post(cancelDocumentlUrl, {id: uid}).then(response => {
                        console.log(response);
                        if (!response.IsSuccess) {
                            throw new Error(response.UserMessage)
                        }

                        return response.IsSuccess;
                    }).catch(error => {
                        Swal.showValidationMessage(
                            `No se pudo anular el documento: ${error}`
                        )
                    });
                },
                allowOutsideClick: () => !Swal.isLoading()
            }).then((result) => {
                if (result.value) {
                    refreshMethod();
                    Swal.fire({
                        title: 'El documento fue anulado correctamente!',
                        type: 'success'
                    })
                }
            })
        }
    };

    var SearchTable = function () {
        if ($form.valid()) {           
            Comprobantes.IssuedTable.draw();
           //Comprobantes.IssuedTable.ajax.reload();
            //KTApp.block($grid, {
            //    boxed: !0,
            //    message: "Cargando..."
            //});
            RefreshSearchFilter();
        }
    };

    var SearchPerform = function () {
        if ($form.valid()) {
            //showLoader();            
            KTApp.block($grid, {
                boxed: !0,
                message: "Cargando..."
            });

            $.ajax({
                url: urlSearch,
                type: 'POST',
                data: $form.serializeJSON(),
                success: function (dataReceived) {
                    if (dataReceived) {
                        if (dataReceived.includes(tableSearch)) {
                            $grid.fadeOut("fast");
                            $grid.empty().html(dataReceived);
                            $grid.fadeIn("slow");
                        }
                        else {
                            window.open(urlLogin, "_login");
                        }
                    }
                    else {
                        toastr.error("No se pudo realizar la consulta");
                    }
                },
                error: function (result, doc, meta) {                  
                    toastr.error("No se pudo realizar la consulta");
                }
            }).always(function () {
                KTApp.unblock($grid);
            });

        }

        RefreshSearchFilter();
    };

    var RefreshSearchFilter = function () {
        var term = $("#searchTerm").val();

        if ($("#filterTitle").length == 0) {
            $(".page-head").append("<span id='filterTitle' class='pull-right'></span>");
        }

        $("#filterTitle").empty();

        if (term) {
            $("#filterTitle").html("<small>Filtrado por: " + term + " <a href='javascript:;' onclick='" + 'Comprobantes.Search("")' + "'>(limpiar)</a></small>");
        }

    };
     
    var HandleRangePicker = function (start, end) {

        ( start && $("#dateFrom").val(start.format("YYYY-MM-DD")) ) || ( start = moment().subtract(30, 'days').locale("es") );
        ( end && $("#dateTo").val(end.format("YYYY-MM-DD")) ) || ( end = moment().locale("es") );
        
        $("#kt_dashboard_daterangepicker_date").html(start.format("D MMMM YYYY") + " - " + end.format("D MMMM YYYY"));
    }; 

    var HandleTypeFilter = function () {
        var $filtercontrol = $(".filter-control");

        if ($filtercontrol && $filtercontrol.length > 0) {

            $filtercontrol.on("change", function () {
                var $ctrl = $(this);
                var value = $ctrl.val();
                $ctrl.val(value);

                // Ejecutamos el proceso de busqueda
                if (performSearch) {
                    SearchPerform();
                }
                else {
                    SearchTable();
                }
               
            });
        }
    };

    var HandleEstablishment = function () {
        var obj = $(this)[0];
        if (obj && obj.value.length > 0) {
            var sHtmlRates = "";
            if (obj.value === "0") {

                sHtmlRates += '<option value="0" style = "text-align:left;width:100%;">Todos</option>';
            }
            else {
                var est = Comprobantes.Establishment.find(
                    function (i) { return i.Code == obj.value; }
                );
                var issuepoint = est.IssuePoint;
                for (var i = 0; i < issuepoint.length; i++) {
                    sHtmlRates += '<option value="' + issuepoint[i].Code + '" style = "text-align:left;width:100%;">' + issuepoint[i].Code + '</option>';
                }
            }
            // Obtengo la informacion del establecmiento
            
            var listObject = $(".select-issuePoint");
            listObject.html(sHtmlRates);           
        }
    };

    return {
        //main function to initiate the module
       
        Configure: function (url, table, loginUrl, execute, searchType) {
            urlSearch = url;
            tableSearch = table;
            urlLogin = loginUrl;
            performSearch = searchType;
            moment().locale("es");             
            var $form = $('#formConsultas');            
            $form.submit(function (event) {
                event.preventDefault();
                if ($form.validate().valid()) {
                    if (performSearch) {
                        SearchPerform();
                    }
                    else {
                        SearchTable();
                    }                    
                    return;
                }
            });

            $(".select-establishment").on("change", HandleEstablishment);            

            var rangeStartDate = convertToMoment($("#dateFrom").val());
            var rangeEndDate = convertToMoment($("#dateTo").val());

            createRangePicker("#kt_dashboard_daterangepicker", function (start, end) {
                HandleRangePicker(start, end);
                if (performSearch) {
                    SearchPerform();
                }
                else {
                    SearchTable();
                }
            }, rangeStartDate, rangeEndDate); 

            HandleRangePicker(rangeStartDate, rangeEndDate);

            HandleTypeFilter();

            RefreshSearchFilter();

            searchMethod = Comprobantes.Search;
            refreshMethod = Comprobantes.Refresh;
            if (execute) {
                if (performSearch) {
                    SearchPerform();
                }
                else {
                    SearchTable();
                }                
            }
        },
        Search: function (searchTerm) {
            $("#searchTerm").val(searchTerm);
            if (performSearch) {
                SearchPerform();
            }
            else {
                SearchTable();
            }
        },
        SetMail: function (url) {
            EmailUrl = url;
            if (performSearch) {
                $(".email-button").on("click", SendEmail);
            }           
        },
        SetCancelDocument: function (url) {
            cancelDocumentlUrl = url;
            if (performSearch) {
                $(".cancel-document-button").on("click", SendCancelDocument);
            }
        },
        GetEmail: function (id) {           
            SendEmail(id);
        },
        GetCancelDocument: function (id) {
            SendCancelDocument(id);
        },
        Refresh: function () { 
            if (performSearch) {
                SearchPerform();
            }
            else {
                SearchTable();
            }
        },
        IssuedTable: "",
        Establishment:[],
        PreviewURL: "~/Comprobantes/Preliminar",
        ImportData: function () {
            showLoader();
            $.get(Comprobantes.ImportarDataUrl, {}, function (data) {
                $("#myModal").html(data);               
                $("#myModal").fadeIn();
                $("#myModal").modal("show");
            }).always(function () {
                hideLoader();
            });
        },
        Sincronizar: function () {
            showLoader();
            $.get(Comprobantes.SincronizarUrl, {}, function (data) {

                if (data.id > 0 && data.status) {
                    Swal.fire("Informaci&oacute;n", data.message, "info");
                }
                else {
                    Swal.fire("Oops!", data.message, "warning");
                }

            }).always(function () {
                hideLoader();
            });
        },
        Preview: function (documentId) {
            if (!documentId) {
                console.log("El número de documento no existe");
                return false;
            }

            showLoader();
            $.get(Comprobantes.PreviewURL + "/" + documentId, {}, function (data) {
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
        DeductibleURL: "~/Comprobantes",
        SustentoURL: "~/Comprobantes",
        ImportarDataUrl: "",
        SincronizarUrl: "",
        SetDeductibles: function (documentId) {
            if (!documentId) {
                console.log("El número de documento no existe");
                return false;
            }

            showLoader();
            $.get(Comprobantes.DeductibleURL + "/ClasificarDocumento/" + documentId, {}, function (data) {
                
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
        SetTipoSustento: function (documentId) {
            if (!documentId) {
                console.log("El número de documento no existe");
                return false;
            }

            showLoader();
            $.get(Comprobantes.SustentoURL + "/TipoSustentoDocumento/" + documentId, {}, function (data) {

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
        SetDocumento: function () {

            $("#btnImprimir").click(function () {
                if (!$("#btnImprimir").hasClass("disabled")) {
                    imprimirFactura();
                }
            });

            $("#btnCancelar").click(function () {
                if (!$("#btnCancelar").hasClass("disabled")) {
                    history.go(-1);
                }
            });

            $("#btnEliminar").on("click", function () {

                Swal.mixin({
                    customClass: {
                        confirmButton: 'btn btn-primary',
                        cancelButton: 'btn btn-secondary'
                    },
                    buttonsStyling: false
                }).fire({
                    title: '¿Desea usted anular este documento?',
                    html: Comprobantes.htmlmsg,                     
                    type: 'warning',                   
                    showCancelButton: true,
                    confirmButtonText: 'Continuar',
                    cancelButtonText: 'Cancelar'
                }).then((result) => { 
                    if (result.value) {
                        showLoader();
                        $("#deleteReason").val(result.value);
                        $("#eliminarDocumento").submit();
                    }
                });
            });
        },       
        Clasificar: function (pkDoc, idDeductible) {
            if (!pkDoc) {
                toastr.error("El número de documento no existe");
                return false;
            }

            showLoader();

            $.post(Comprobantes.DeductibleURL + "Clasificar", { "id": pkDoc , "deducible" : idDeductible}, function (data) {
                refreshMethod();               
                toastr.success("Se ha guardado con éxito el registro!");
                $("#myModal").fadeOut();
                $("#myModal").modal("hide");
                $("#myModal").empty(); 

            }).fail(function (e,d,x) {
                toastr.error("No se guardo el registro!");
            }).always(function () {
                hideLoader();
            });
        },
        Sustento: function (pkDoc, SustentoId) {
            if (!pkDoc) {
                toastr.error("El número de documento no existe");
                return false;
            }
            if (!SustentoId) {
                toastr.error("Debe seleccionar el tipo sustento del documento");
                return false;
            }
            showLoader();
            $.post(Comprobantes.SustentoURL + "Sustento", { "id": pkDoc, "sustento": SustentoId }, function (data) {
                refreshMethod();
                console.log(data);
                toastr.success("Se ha guardado con éxito el registro!");
                $("#myModal").fadeOut();
                $("#myModal").modal("hide");
                $("#myModal").empty();

            }).fail(function (e, d, x) {
                toastr.error("No se guardo el registro!");
            }).always(function () {
                hideLoader();
            });
        }
        
    };   
}();
 