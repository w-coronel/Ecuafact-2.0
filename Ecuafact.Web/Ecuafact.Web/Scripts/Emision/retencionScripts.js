

/* eslint-disable */
var retencionSaveAction = "Retencion/SaveRetencionAsync";
var printretencionAction = "Retencion/PrintDocument";
var getActionUrl = function (controller, action) { return "/"; };
var documentoRetencion = {};
var model = {};
var getImpuestosHtml = function () { return ""; };
var taxData = [];
var fecha_mes_atras = new Date(new Date() - (30 * 24 * 60 * 60 * 60 * 15)).toLocaleDateString("es");
var fecha_actual = new Date().toLocaleDateString("es");
var data_documents = [];

var total = 0.00;
var saldo = 0.00;
var baseImponible = 0.00;
var baseIVA = 0.00;

var initialized = false;
var calculating = 0;

var paymentItemCount = -1;
var itemListCount = -1;
var adicionalItemCount = 0;
var retencionInformativa = false;

var tiposIdentificacion = [];
var tipos = [];
var impuestos = [];
var Contributor;

//////////////////////////////// ACTIONS ////////////////////////////////

var getUrlSupplierAction = function (id) { return "/Contribuyentes/Nuevo"; };
var getUrlSearchContrib = function () { return "/Retencion/SearchContribAsync"; };
var getLoginFormUrl = function () { return "/Auth"; };
var getUrlAuthorization = function (docTypeCode, docNumber) { return '/Retencion/GetDocumentAuthSync?docType=' + docTypeCode + '&docNumber=' + docNumber; };
var getImpuestoData = function () { return []; };
var getSearchTaxUrl = function () { return "/Retencion/SearchTaxAsync"; };  // se requiere implementacion 
var getCreateTaxUrl = function () { return "/Impuesto/Nuevo"; };  // se requiere implementacion 
var getUrlSearchDocument = function (id) { return "/Retencion/GetDocumentById/" + id; }; // se requiere implementacion



function actualizarTotales() {
    if (calculating == 0) {
        // este es un control para evitar multiples llamadas
        // al proceso de calculo por los procesos propios de la funcion
        calculating = 1;
        validateerror = true;
          
        // Inicializamos el total:
        total = 0.00;
        saldo = 0.00;

        var objBaseImponible = $("#ref_valor_comprobante")[0];
        baseImponible = getValueSafe(objBaseImponible.value);

        var objBaseIVA = $("#ref_iva_comprobante")[0];
        baseIVA = getValueSafe(objBaseIVA.value);

        var objLista = $(".tax-item"); // Lista de Detalles <TR>

        // Analizo registro x registro
        for (var i = 0; i < objLista.length; i++) {
            var objDetalle = objLista[i];

            var objInputs = $(objDetalle).find("input");
            var objBase = objInputs[0];
            var objRate = objInputs[1];
            var objVal = objInputs[2];

            var base = getValueSafe(objBase.value);

            //valida el valor base: tiene que ser positivo
            if (base < 0.00) {
                base = -base;
            }


            var rate = getValueSafe(objRate.value);

            // valida el valor del rate
            if (rate > 100) {
                rate = 100.00;
            }

            if (rate < 0) {
                rate = 0.00;
            }


            var value = 0.00;

            if (rate > 0) {
                value = getValueSafe(base * (rate / 100));
            }
             
            setValueSafe(objBase, base);
            setValueSafe(objVal, value);

            // Este valor se actualiza solo si hay un valor definido
            if (objRate.value !== null || objRate.value !== '') {
                objRate.value = rate + " %";
            }

            total += value;
        }

        saldo = (baseImponible + baseIVA) - total;

        // Actualizar el total
        var objTotal = $("#documento_valor_total")[0];
        var objSaldoTotal = $("#documento_saldo_total")[0];

        setValueSafe(objBaseImponible, baseImponible);
        setValueSafe(objBaseIVA, baseIVA);

        setValueSafe(objTotal, total);
        setValueSafe(objSaldoTotal, saldo);

        if (baseImponible + baseIVA < total) {
            Swal.fire("Validacion", "El total de la retencion no puede ser mayor al valor de la factura!", "warning");
        }

        // flag para evitar sobrecalculos en procesos
        calculating = 0;
    }

}

function generateDocument(emitirDocumento) {

    

    if (saldo <= 0) {
        return valError("El total de la Retencion debe ser menor al valor total de la factura!");
    }

    var objcontributor = $("#inputClienteId");
    var contributorId = objcontributor.val();

    if (contributorId < 1) {
        return valError("Por favor seleccione un proveedor!", objcontributor);
    }

    var razonSocial = $("#razon_social").val();
    var identificacion = $("#identificacion_proveedor").val();
    var tipoIdentificacion = $("#tipo_identificacion").val();

    var fechaDocumento = $('#fecha_documento').val();

    var objReason = $("#motivo");
    var documentReason = objReason.val();

    if (documentReason==null || documentReason=='') {
        return valError("El motivo o explicacion de la retencion esta vacio!", objReason);
    }


    var fiscalPeriod = $("#periodo_fiscal").val();

    var refDocumentCode = $("#ref_tipo_comprobante").val();

    if (refDocumentCode==null || refDocumentCode=="") {
        return valError("Por favor seleccione un tipo de comprobante!");
    }

    var refDocumentNumber = $("#ref_numero_comprobante").val();

    if (refDocumentNumber == null || refDocumentNumber == "") {
        return valError("Por favor ingrese un numero de documento valido!");
    }

    var refDocumentDate = $("#ref_fecha_documento").val();

    var refDocumentAuth = $("#ref_autorizacion_comprobante").val();
    if (refDocumentAuth==null || refDocumentAuth=="") {
        return valError("Por favor ingrese el numero de autorizacion del comprobante sustento de la retencion!");
    }

    var refDocumentAmount = getValueSafe($("#ref_valor_comprobante").val());
    if (refDocumentAmount <= 0) {
        return valError("Por favor ingrese valor del comprobante sustento de la retencion!");
    }

    var refDocumentVat = getValueSafe($("#ref_iva_comprobante").val());

    var idDocument = $("#Id").val();   
    

    var model = {
        Id: idDocument,
        ContributorId: contributorId,
        ContributorName: razonSocial,
        BusinessName: razonSocial,
        Identification: identificacion,
        IdentificationType: tipoIdentificacion,
        DocumentTypeCode: "01", // Retencion
        IssuedOn: fechaDocumento,
        FiscalPeriod: fiscalPeriod,
        Reason: documentReason,
        FiscalAmount: total,
        Balance: saldo,
        Details: [],
        AdditionalFields: [],

        ReferenceDocumentCode: refDocumentCode,
        ReferenceDocumentNumber: refDocumentNumber,
        ReferenceDocumentDate: refDocumentDate,
        ReferenceDocumentAuth: refDocumentAuth,
        ReferenceDocumentAmount: refDocumentAmount,
        ReferenceDocumentVat: refDocumentVat,

        Status: 0
    };

    if (typeof emitirDocumento !== 'undefined' && emitirDocumento) {
        model.Status = 1;
    }
    else {
        model.Status = 0;
    }

    var itemObj = $(".tax-item");

    for (var i = 0; i < itemObj.length; i++) {

        var itemobj = $(itemObj[i]).find("select");
        var itemId = itemobj.val();

        // Buscar Impuesto
        var imp = impuestos.find(function (imp) { return imp.Id==itemId; });

        // Buscar Tipo de Impuesto
        var typ = tipos.find(function (tipo) { return tipo.Id==imp.TaxTypeId; });


        if (itemId == null || itemId == 0) {
            return valError("Debe especificar el impuesto!", itemobj);
        }

        var inputs = $(itemObj[i]).find("input");

        var base = getValueSafe(inputs[0].value);
        var rate = getValueSafe(inputs[1].value);
        var val = getValueSafe(inputs[2].value);

        if (base <= 0) {
            return valError("No se puede generar el documento: la base imponible debe ser mayor a 0 (cero)!");
        }


        obj = {
            RetentionTaxId: itemId,
            TaxTypeCode: imp.SriCode,
            RetentionTaxCode: typ.SriCode,
            TaxBase: base,
            TaxRate: rate,
            TaxValue: val,
            ReferenceDocumentCode: refDocumentCode,
            ReferenceDocumentNumber: refDocumentNumber,
            ReferenceDocumentDate: refDocumentDate
        };

        model.Details.push(obj);
    }


    // Grabo los pagos en el modelo de datos;
    var paymentObj = $(".payment-item");

    for (var p = 0; i < paymentObj.length; p++) {
        var payment = $(paymentObj[p]);

        var paymentItem = payment.find("select");
        var paymentCode = paymentItem.val();
        var paymentName = paymentItem[0].children[paymentItem[0].selectedIndex].text;

        var paymentValue = getValueSafe(payment.find("input").val());
         
        model.Payments.push({
            "PaymentMethodCode": paymentCode,
            "Name": paymentName,
            "Total": paymentValue,
            "Term": 30, // en caso que se facture a credito
            "TimeUnit": "dias" // la unidad de tiempo del credito: meses, semanas, dias
        });
    }

    var line = 1;


    // Grabo los pagos en el modelo de datos;
    var additionalObj = $(".additional-item");

    for (var a = 0; a < additionalObj.length; a++) {
        inputs = $(additionalObj[a]).find("input");
        var name = inputs[0].value;
        var value = inputs[1].value;
         
        if (typeof value == 'string' && value && value !== "") {

            model.AdditionalFields.push({
                "Name": name,
                "Value": value,
                "LineNumber": line
            });

            line++;

        }

        if (name == "Direccion") {
            model.Address = value;
        }

        if (name == "Telefono") {
            model.Phone = value;
        }

        if (name == "Email") {
            model.EmailAddresses = value;
        }

        if (name == "Observaciones") {
            model.Reason = value;
        }
    }


    return model;
} 

// Muestra el mensaje de validacion de la Retencion
function valError(msg, obj) {
    hideLoader();

    Swal.fire("Validacion de Datos", msg, "warning");

    if (typeof obj !== 'undefined') {
        if (obj !== null && typeof obj.focus !== 'undefined')
            obj.focus();

        if (obj !== null && typeof obj.blur !== 'undefined')
            obj.blur();
    }

    guardandoDocumento = false;
    return null;
}

var guardandoDocumento = false;

function guardarRetencion(emitirDocumento) {

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // VALIDACION PARA LAS RETENCIONES INFORMATIVAS //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Antes que nada la retencion debe contener detalles
    var itemObj = $(".tax-item");
    if (itemObj.length == 0) {
        return valError("Debe seleccionar por lo menos un impuesto!");
    }

    if (!retencionInformativa && total == 0) {
        Swal.fire({
            title: "Retencion",
            text: "EL TOTAL DE LA RETENCION ES CERO. \r\n ¿Esta usted seguro de generar esta Retencion como INFORMATIVA? ",
            type: "warning",
            showCancelButton: true 
        }).then(function (confirm) {
            emitir = emitirDocumento;

            // BAJO RESPONSABILIDAD DE LOS USUARIOS SE HABILITA LA RETENCION INFORMATIVA
            if (confirm.value) {
                console.log("GENERANDO RETENCION INFORMATIVA: Con valores en cero.");
                retencionInformativa = true;
                guardarDocumento(emitir);
            }
        });

        // return valError("El total de la Retencion no puede ser menor a cero!")
    }
    else {
        guardarDocumento(emitirDocumento);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    retencionInformativa = false; // VUELVE A SU VALOR PREDETERMINADO
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


}

function guardarDocumento(emitirDocumento){

    if (guardandoDocumento) {
        return;
    }

    guardandoDocumento = true;

    // Hack para volver la pantalla al inicio de la pagina
    // y los mensajes se ubiquen en su lugar adecuado.
    window.scrollTo(0, 0);


    showLoader("<label><H1 >Guardando Registro... Por favor Espere.</H1><label>");


    var RetencionModel = generateDocument(emitirDocumento);

    if (RetencionModel==null) {
        debugger;
        return;
    }

    $.ajax({
        url: retencionSaveAction,
        type: "POST",
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            model: RetencionModel
        }),
        async: true,
        error: function (data, msg) {
            guardandoDocumento = false;
            hideLoader();

            // Esto es para saber si el problema tiene que ver con la session
            if (typeof data.responseText !== 'undefined')
                if (data.responseText !== null && data.responseText.includes("login")) {
                    Swal.fire("Sesion Caducada", "Por favor inicie sesion", "error");
                    var w = open(getActionUrl("Auth", "Index"), "_top", "height=770,width=520");
                }
                else {
                    Swal.fire("Retencion", msg, "error");
                }

            debugger;

        },
        success: function (data, usr, obj, etc) {
            guardandoDocumento = false;

            if (data.id > 0) // Si el ID del Registro es mayor que cero, es un registro real
            {
                documentoRetencion = data.result;

                Swal.fire("Retencion", "El registro se guardo con Exito!", "success");

                // Si los datos fueron guardados correctamente entonces realizo el proceso de bloqueo de todos los controles
                $('.Retencion')
                    .find('input, textarea, button, select')
                    .attr('disabled', 'disabled', 'div');

                //$("#numero_documento")[0].innerText = (documentoRetencion.DocumentNumber);

                //// ejecuta el proceso de carga de los datos de autorizacion

                //$("#numero_documento").focus();

                if (typeof redirectAction !== 'undefined') {
                    redirectAction('Ver', documentoRetencion.Id);
                    //redirectAction('Index', documentoRetencion.Id);
                }
            }
            else {
                if (data.error) {
                    if (data.error.UserMessage) {
                        Swal.fire("Retencion", data.error.UserMessage, "error");
                    }
                    else if (data.error.message) {
                        Swal.fire("Retencion", data.error.message, "error");
                    }
                    else if (data.error.Message) {
                        Swal.fire("Retencion", data.error.Message, "error");
                    }
                    else {
                        Swal.fire("Retencion", "Ocurrio un error al guardar la retencion.", "error");
                    }
                }
                else if (data.message) {
                    Swal.fire("Retencion", data.message, "error");
                }
                else {
                    Swal.fire("Retencion", data, "error");
                }


                debugger;
            }

            hideLoader();
            window.scrollTo(0, 0);
        }
    });
}

function agregarComprador() {
    showLoader();
    var urlAction = getUrlSupplierAction();

    $.get(urlAction, {}, function (data) {
        $("#myModal").html(data);
        hideLoader();

        $("#myModal").fadeIn();
        $("#myModal").modal("show");
    });

}

function editarComprador() {

    var cliente_id = $("#inputClienteId").val();

    if (cliente_id > 0) {
        showLoader();

        var urlAction = getUrlSupplierAction(cliente_id);

        $.get(urlAction, {}, function (data) {
            $("#myModal").html(data);
            $("#myModal").fadeIn();
            $("#myModal").modal("show");
        });
        hideLoader();

    }
    else {
        Swal.fire("Mensaje", "No se puede modificar este cliente!", "warning");
    }
}

function startSupplierSelector() {

    $(".js-supplierSelector").select2({
        ajax: {
            url: getUrlSearchContrib(),
            dataType: 'json',
            async: true,
            delay: 1000,
            data: function (params) {
                return {
                    q: params.term || "", // search term
                    page: params.page || 1
                };
            },
            //processResults: function (data, params) {
            //    // parse the results into the format expected by Select2
            //    // since we are using custom formatting functions we do not need to
            //    // alter the remote JSON data, except to indicate that infinite
            //    // scrolling can be used
            //    params.page = params.page || 1;

            //    return {
            //        results: data,
            //        pagination: {
            //            more: (params.page * 30) < data.length
            //        }
            //    };
            //},
            error: function (data, a, e, i) {

                if (data !== null && typeof data.responseText == 'string' &&
                    data.responseText.includes("login-form")) {
                    Swal.fire("Error", "Su sesion ha caducado. Vuelva a iniciar sesion", "warning");

                    window.location.href = getLoginFormUrl();
                }
                else {
                    //Swal.fire("Error","Hubo un error al obtener los datos","error");
                }
            },
            cache: true
        },
        allowClear: true,
        placeholder: 'SELECCIONE UN CONTRIBUYENTE',
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        templateResult: formatContrib,
        templateSelection: formatContribSelection,
        language: {
            inputTooShort: function (args) {
                var remainingChars = args.minimum - args.input.length;

                var message = '<div class="row"> <span class="col-9">' + 'Por favor escriba al menos ' + remainingChars + ' caracteres' +
                    "&nbsp; </span> <button type='button'  data-toggle='modal' data-target='#modal-cliente' onclick='javascript:agregarComprador();' class='col-3 btn btn-outline btn-primary text-light'> Agregar <i class='fa fa-plus'></i></button> </div>";

                return message;

            },
            noResults: function () {
                return "Registro no encontrado &nbsp; <button type='button'  data - toggle='modal' data - target='#modal-cliente' onclick='javascript:agregarComprador();' class='btn btn-outline blue'> Agregar <i class='fa fa-plus'></i></button>";
            },
            searching: function () {
                return "Buscando...";
            }
        }
    });

}

function formatContrib(repo) {
    if (repo.loading) {
        return repo.text;
    }

    if (typeof repo.data !== 'undefined') {
        var data = repo.data;

        var markup = "<div class='select2-result-repository clearfix'>" +
            "<div class='select2-result-repository__meta'>" +
            "<div class='select2-result-repository__title'>" + repo.text + "</div>" +
            "<div class='select2-result-repository__statistics'>" +
            "<div class='select2-result-repository__description'><i class='fa fa-area-chart'></i> Nombre Comercial: " + data.TradeName + "</div>" +
            "<div class='select2-result-repository__address'><i class='fa fa-map-marker'></i> Direccion: " + data.Address + " </div>" +
            "<div class='select2-result-repository__email'><i class='fa fa-envelope-o'></i> E-mail:" + data.EmailAddresses + " </div>" +
            "<div class='select2-result-repository__phone'><i class='fa fa-phone'></i> Telefono:" + (data.Phone ? data.Phone : "" ) + "</div>" +
            "</div>" +
            "</div></div>";
        return markup;
    }

    return repo.text;
}

function formatContribSelection(repo) {

    // Si existe el objeto de datos se muestra la informacion
    if (typeof repo.data !== 'undefined') {
        var data = repo.data;


        var markup = "<div class='select2-result-repository clearfix'>" +
            "<div class='select2-result-repository__meta'>" +
            "<div class='select2-result-repository__title'>" + repo.text + "</div>" +
            "<div class='select2-result-repository__statistics'>" +
            "<div class='select2-result-repository__description'><i class='fa fa-area-chart'></i> Nombre Comercial: " + data.TradeName + "</div>" +
            "<div class='select2-result-repository__address'><i class='fa fa-map-marker'></i> Direccion: " + data.Address + " </div>" +
            "<div class='select2-result-repository__email'><i class='fa fa-envelope-o'></i> E-mail:" + data.EmailAddresses + " </div>" +
            "<div class='select2-result-repository__phone'><i class='fa fa-phone'></i> Telefono:" + data.Phone + "</div>" +
            "</div>" +
            "</div></div>";

        $("#bodyContrib").css("height", "200px");
        $("#actualizarClienteButton").removeClass("kt-hide");
        $("#proveedor_nombre").val(data.BussinesName);

        $("#razon_social").val(data.BussinesName);
        $("#identificacion_proveedor").val(data.Identification);

        var tipoIdentidad = tiposIdentificacion.find(
            function (o) { return o.Id == data.IdentificationTypeId; }).SriCode;
        $("#tipo_identificacion").val(tipoIdentidad);

        /// Cargo la informacion en los elementos

        agregarAditional("Direccion", data.Address);
        agregarAditional("Email", data.EmailAddresses);
        agregarAditional("Telefono", data.Phone);


        // OK AHORA CARGAMOS LA INFORMACION DEL BOX DE SUSTENTO:


        startDocumentSelector(data.Identification);

        $("#inputDocumentId").removeClass("hidden");

        return markup;
    }
    else if (typeof Contributor !== 'undefined'  )
    {
        var markup = "<div class='select2-result-repository clearfix'>" +
            "<div class='select2-result-repository__meta'>" +
            "<div class='select2-result-repository__title'>" + repo.text + "</div>" +
            "<div class='select2-result-repository__statistics'>" +
            "<div class='select2-result-repository__description'><i class='fa fa-area-chart'></i> Nombre Comercial: " + Contributor.TradeName + "</div>" +
            "<div class='select2-result-repository__address'><i class='fa fa-map-marker'></i> Direccion: " + Contributor.Address + " </div>" +
            "<div class='select2-result-repository__email'><i class='fa fa-envelope-o'></i> E-mail:" + Contributor.EmailAddresses + " </div>" +
            "<div class='select2-result-repository__phone'><i class='fa fa-phone'></i> Telefono:" + Contributor.Phone + "</div>" +
            "</div>" +
            "</div></div>";

        $("#bodyContrib").css("height", "200px");
        $("#actualizarClienteButton").removeClass("kt-hide");
        $("#proveedor_nombre").val(Contributor.BussinesName);

        $("#razon_social").val(Contributor.BussinesName);
        $("#identificacion_proveedor").val(Contributor.Identification);       
        $("#tipo_identificacion").val(Contributor.IdentificationType);

        // OK AHORA CARGAMOS LA INFORMACION DEL BOX DE SUSTENTO:
        startDocumentSelector(Contributor.Identification);
        $("#inputDocumentId").removeClass("hidden");

        return markup;

    }
    else {
        $("#bodyContrib").css("height", "");
        $("#actualizarClienteButton").addClass("kt-hide");
        $("#inputDocumentId").addClass("hidden");

        if (typeof agregarAditional !== 'undefined') {
            $("#proveedor_nombre").val("NO DISPONIBLE");
            agregarAditional("Direccion", "");
            agregarAditional("Email", "");
            agregarAditional("Telefono", "");
        }
    }

    return repo.text;

}

function addAditional(datos) {
    if (datos) {
        for (var i = 0; i < datos.length; i++) {
            var name = datos[i].Name;
            var value = datos[i].Value;
            agregarAditional(name, value);
        }
    }
}

function agregarAditional(adname, advalue) {
    adicionalItemCount++;

    var adCount = adicionalItemCount;

    if (typeof adname !== 'string') {
        adname = '';
    }
    else {
        adCount = adname;
    }

    if (typeof advalue !== 'string') {
        advalue = '';
    }

    if ($("#adname" + adCount).length > 0) {
        $("#addet" + adCount).val(advalue);
        return;
    }

    var markup =
        '<tr id="payment' + adCount + '" class="additional-item col-12 col-xl-6" style:"margin-bottom:5px">' +
        '<td style="width:100%">' + '<div class="row">' +
        '<div data-title="Detalle" class="col-lg-6 col-md-6 col-sm-12 col-xs-12 bold" style="padding-bottom:1px">' +
            '<input class="form-control bold"  id="adname' + adCount + '"  style="text-align:left; width:100%;" type="text" placeholder="Nombre" list="list-references" value="' + adname + '" />' +
        '</div>' +
        '<div data-title="Valor" class="col-lg-6 col-md-6 col-sm-12 col-xs-12">' +
            '<input class="form-control" style="text-align:left; " id="addet' + adCount + '" type="text" placeholder="Detalle" value="' + advalue + '" />' +
        '</div>' + '</div>' +
        '</td>' +
        '<td data-title="">' +
            '<button id="trash' + adCount + '"  type="button" onclick="quitarAdicional(this)" class="tabledit-delete-button btn btn-sm" style="float: none;width:auto;background-color:#0E172D;"><span class="la la-trash-o text-light"></span></button>' +
        '</td>' +
        '</tr>';

    var result = $("#adicional-items").append(markup);
    var obj = $("#adname" + adCount);

    if (typeof initialized !== 'undefined' && initialized) {
        obj.focus();
    }

}

function quitarAdicional(obj) {
    obj.parentElement.parentElement.remove();
}

function formatTax(repo) {
    if (repo.loading) {
        return repo.text;
    }

    return repo.text;
}

function formatTaxSelection(repo) {

    if (typeof repo.id !== 'undefined') {

        // Obtengo la informacion del impuesto
        var imp = impuestos.find(
            function (i) { return i.Id==repo.id; }
            );

        var rates = imp.RetentionRate;
        var sHtmlRates = "";

        for (var i = 0; i < rates.length; i++) {
            sHtmlRates += '<option>' + rates[i].RateValue + ' %</option>';
        }

        // Obtenemos el identificador de la linea
        var idtext = repo.element.parentElement.parentElement.id.replace("RetentionTaxCode", "");
        var listObject = $("#TaxRateList" + idtext);
        listObject.html(sHtmlRates);
        var rateObject = $("#TaxRate" + idtext);

        if (rates.length==1) {
            // Si solo hay uno establece el valor x default:
            rateObject.val(rates[0].RateValue + " %");
        }
        else {
            rateObject.val("");
        }

        var taxBase = $("#TaxBase" + idtext);

        var taxCode = getValueSafe(repo.text.substring(0, 5));

        if (taxCode > 100) {
            var baseValue = getValueSafe($("#ref_valor_comprobante").val());
            taxBase.val(baseValue);
        }
        else {
            var ivaValue = getValueSafe($("#ref_iva_comprobante").val());
            taxBase.val(ivaValue);
        }

        actualizarTotales();
    }


    // Si existe el objeto de datos se muestra la informacion para agregarlo a la lista
    //if (typeof repo.data !== 'undefined') {
    //    var data = repo.data;

    //    // Buscamos id del elemento TR
    //    var itemElementId = repo.element.parentElement.attributes["data-parent"].value;

    //    var trItem = $("#" + itemElementId + " input");
    //    trItem[1].value = repo.data.UnitPrice;

    //    var ivaitem = ivaList.find(o => o.data.Id == repo.data.IvaRateId);

    //    if (typeof ivaitem !== 'undefined' && ivaitem !== null) {
    //        $("#iva" + itemElementId).val(ivaitem.id);
    //        $("#iva" + itemElementId).trigger('change');
    //    }

    //    actualizarTotales();


    //    if (typeof initialized !== 'undefined' && initialized) {
    //        trItem[0].focus();
    //    }
    //}

    return repo.text;

}

function agregarItem(item) {
    itemListCount++;
    var itemListId = "RetentionTaxCode" + itemListCount;
    var tipoItemsHtml = getImpuestosHtml();


    var markup =
        '<tr id="item' + itemListCount + '" class="tax-item kt-datatable__row">' +
            '<td>' + '<div class="row">' +
                '<div  data-title="Detalle" class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="padding-bottom:5px">' +
                    '<select class="form-control RetentionTaxCode m-b" style="width:100%;font-size:11px;background:none;border:none;height:36px;" id="' + itemListId + '"  onchange="tipoImpuestoChanged(this)" required=""  >' + tipoItemsHtml + '</select>' +
                '</div>' +

                '<div  data-title="TaxBase" class="col-lg-4 col-md-4 col-sm-6 col-xs-12" style="padding-bottom:5px">' +
                    '<input class="form-control TaxBase" id="TaxBase' + itemListCount + '" required  style="border:none none solid none; text-align:right;" onchange="actualizarTotales()" >' +
                '</div>' +
                '<div  data-title="TaxRate" class=" col-lg-4 col-md-4 col-sm-6 col-xs-12" style="padding-bottom:5px">' +
                    '<input class="form-control TaxRate" id="TaxRate' + itemListCount + '" list="TaxRateList' + itemListCount + '" placeholder="Seleccione uno" onclick="listarPorcentajes(this)"  style="border:none none solid none; text-align:right;" value="0%" onchange="actualizarTotales()" > ' +
                    '<datalist id="TaxRateList' + itemListCount + '"><option>0 %</option><option>100 %</option></datalist>' +
                '</div>' +
                '<div  data-title="TaxValue" class=" col-lg-4  col-md-4 col-sm-6 col-xs-12" style="padding-bottom:5px">' +
                    '<input class="form-control TaxValue" id="TaxValue' + itemListCount + '"  style="border:none none solid none; text-align:right;" readonly disabled >' +
                '</div>' + '</div>' +
            '</td>' +
            '<td data-title=""><button id="trash' + itemListCount + '" type="button" onfocus="enfocarLista(this)" onclick="quitarItem(this)" class="tabledit-delete-button btn btn-sm" style="float: none;background-color:#0E172D;"><span class="la la-trash-o text-light"></span></button></td>' +
        '</tr>';


    $("#tax-items").append(markup);

    var obj = $("#" + itemListId)[0];
    if (typeof item !== 'undefined') {
        obj.value = item.RetentionTaxId;
    }
        

    startSelectTax(obj);   

    actualizarTotales();
}

function listarPorcentajes(obj) {
    if (obj !== null) {
        obj.value = "";
    }
}

function refautorizacionChanged() {
    var objAuth = $("#ref_autorizacion_comprobante");
    var authText = objAuth.val();
    if (authText !== null && authText !== '') {
        var x = authText.match(/\d/g);
        var result = "";
        if (x) {
            result = x.join("");
        }

        objAuth.val(result);
    }
}

function tipoImpuestoChanged() {

}

function enfocarLista(objparent) {
    //if (typeof objparent !== 'undefined') {
    //    //$(objparent).next().focus();


    //var objName = objparent.parentElement.parentElement.id;

    //var obj = $("#" + objName + " select");

    //if (typeof initialized !== 'undefined' && initialized)
    //    if (typeof obj !== 'undefined' && obj.length > 0) {
    //        obj[0].focus();
    //    }
    //}
}

function quitarItem(obj) {

    if (obj.parentElement.parentElement.parentElement.childElementCount == 1) {
        Swal.fire("Retencion", "Se requiere al menos un item en la lista!", "warning");
        return;
    }

    obj.parentElement.parentElement.remove();
    actualizarTotales();
}

function selectAddButton() {
    if (typeof initialized !== 'undefined' && initialized)
        $("#addItemButton").focus();
}

function startSelectTax(obj) {


    $(obj).select2({
        //data: getImpuestosHtml(),
        allowClear: false,
        placeholder: 'Seleccione un Impuesto',
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        //minimumInputLength: 2,
        //templateResult: formatTax,
        templateSelection: formatTaxSelection,
        language: {
            //inputTooShort: function (args) {
            //var remainingChars = args.minimum - args.input.length;

            //var message = 'Por favor escriba al menos ' + remainingChars + ' caracteres' +
            //    "&nbsp; <button type='button' onclick='javascript:crearImpuesto(this);' class='btn btn-outline blue'> Agregar <i class='fa fa-plus'></i></button>";

            //    return message;

            //},
            noResults: function () {
                return "Registro no encontrado "; //&nbsp; <button type='button' onclick='javascript:crearImpuesto(this);' class='btn btn-outline blue'> Agregar <i class='fa fa-plus'></i></button>";
            },
            searching: function () {
                return "Buscando...";
            }
        }
    });

    $("option[value='undefined']").remove();
};

function crearImpuesto() {
    showLoader();

    $.get(getCreateTaxUrl(), {}, function (data) {
        $("#myModal").html(data);
        hideLoader();

        $("#myModal").fadeIn();
        $("#myModal").modal("show");
    });
}

function startDocumentSelector(cliente_id) {

    if (initialized && cliente_id && cliente_id > 0) {
        var url = getUrlSearchDocument(cliente_id);
        var objSelector = $(".js-documentSelector");

        try {
            if ($(objSelector).data('select2')) {
                objSelector.select2("destroy");
            }
        } catch (e) {
            // default
            if (console.log) {
                console.log(e);
            }
        }

        objSelector.select2({
            ajax: {
                url: url,
                dataType: 'json',
                //async: true,
                delay: 1000,
                data: function (params) {
                    return {
                        search: params.term, // search term
                        page: params.page
                    };
                },
                processResults: function (data, params) {
                    // parse the results into the format expected by Select2
                    // since we are using custom formatting functions we do not need to
                    // alter the remote JSON data, except to indicate that infinite
                    // scrolling can be used
                    params.page = params.page || 1;

                    return {
                        results: data,
                        pagination: {
                            more: (params.page * 30) < data.length
                        }
                    };
                },
                error: function (data, a, e, i) {

                    if (data !== null && typeof data.responseText == 'string' &&
                        data.responseText.includes("login-form")) {
                        Swal.fire("Error", "Su sesion ha caducado. Vuelva a iniciar sesion", "warning");

                        window.location.href = getLoginFormUrl();
                    }
                    else {
                        //Swal.fire("Error","Hubo un error al obtener los datos","error");
                    }
                },
                cache: true
            },
            escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
            allowClear: true,
            templateResult: formatDoc,
            templateSelection: formatDocSelection,
            language: {
                searching: function () {
                    return "Buscando...";
                }
            },
            placeholder: 'Por favor seleccione un documento'

        });


    }
}

function formatDocSelection(repo) {
    if ( typeof repo.data !== 'undefined') {
        var data = repo.data;

        cargarDocumento(data);
    }

    return repo.text;
}

function formatDoc(repo) {
    if (repo) {
        return repo.text;
    }

    return "No hay documentos que mostrar...";
} 


function cargarDocumento(doc) {
    if (doc) {

        // Convertir dato obj a fecha:
        $("#ref_tipo_comprobante").val(doc.codTypeDoc);
        $("#ref_numero_comprobante").val(doc.sequence);
        $("#ref_fecha_documento").val(doc.date);
        $("#ref_autorizacion_comprobante").val(doc.authorizationNumber);
         
        $("#ref_valor_comprobante").val(doc.subTotal);
        $("#ref_iva_comprobante").val(doc.iva);

 
        actualizarTotales();
    }
}

function llenarControles(model) {
    if (model) {
        if (model.Id > 0) {
            adicionalItemCount = model.AdditionalFields.length;            
            var obj = $(".js-supplierSelector");            
            var option = new Option(model.Identification + ' - ' + model.ContributorName, model.ContributorId, true, true);
            obj.append(option).trigger('change');
            for (var i = 0; i < model.Details.length; i++) {
                var item = model.Details[i];
                agregarItem(item);
            }
            addAditional(model.AdditionalFields);          

        }
    }
}

window.scrollTo(0, 0);