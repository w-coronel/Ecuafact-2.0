/* eslint-disable */
var creditNoteSaveAction = "NotaCredito/SaveCreditNoteAsync";
var printCreditNoteAction = "NotaCredito/PrintDocument";
var documentoNotaCredito = {};
var ivaList = [];
var productData = [];

var data_documents = [];

var fecha_mes_atras = new Date(new Date() - (30 * 24 * 60 * 60 * 60 * 15)).toLocaleDateString("es");
var fecha_actual = new Date().toLocaleDateString("es");

var subtotal_12 = 0.00;
var subtotal_0 = 0.00;
var subtotal_nosujeto = 0.00;
var subtotal_exento = 0.00;
var subtotal_sin_impuestos = 0.00;

var total_descuento = 0.00;
var total_iva = 0.00;
var total = 0.00;
var valor_modificado = 0.00;
var valor_original = 0.00;
var valor_propina = 0.00;
var initialized = false;
var calculating = 0;

var itemListCount = -1;
var adicionalItemCount = 0;

//////////////////////////////// ACTIONS ////////////////////////////////
var getUrlContribAction = function (id) { return "/Contribuyentes/Nuevo"; };
var getUrlSearchContrib = function () { return "/NotaCredito/SearchContribAsync"; };
var getUrlSearchDocument = function () { return "/NotaCredito/SearchDocumentAsync"; };


var getLoginFormUrl = function () { return "/Auth"; };
var getUrlAuthorization = function (docTypeCode, docNumber) { return '/NotaCredito/GetDocumentAuthSync?docType=' + docTypeCode + '&docNumber=' + docNumber; };
var getSearchProductUrl = function () { return "/NotaCredito/SearchProductAsync"; };  // se requiere implementacion 
var getCreateProductUrl = function () { return "/NotaCredito/GetProducto"; };  // se requiere implementacion 
var getProductByCode = function (code) { return "/Product/GetProductoByCodeAsync"; };



function docNumberChanged() {
    var numDocObj = $("#ref_numero_comprobante");
    var text = numDocObj.val();

    numDocObj.val(formatNumDoc(text));

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

function actualizarTotales() {
    if (calculating == 0) {
        // este es un control para evitar multiples llamadas
        // al proceso de calculo por los procesos propios de la funcion
        calculating = 1;
        var oldtotal = total;
        subtotal_12 = 0.00;
        subtotal_0 = 0.00;
        total_descuento = 0.00;
        total_iva = 0.00;
        subtotal_nosujeto = 0.00;
        subtotal_exento = 0.00;
        subtotal_sin_impuestos = 0.00;
        total = 0.00;
        valor_modificado = 0.00;
        valor_propina = 0.00;

        // PROCESO DE CALCULO DE LOS VALORES POR ITEM
        var itemObj = $(".product-item");

        // Busco dentro de los items de la lista
        for (var i = 0; i < itemObj.length; i++) {
            var itemInput = $(itemObj[i]).find("input");
            var ivaSelect = $(itemObj[i]).find("select")[1];

            if (typeof itemInput !== 'undefined' && itemInput.length > 0) {
                var qtyObject = itemInput[0];
                var prcObject = itemInput[1];
                var ivaObject = itemInput[2];
                var dscObject = itemInput[3];
                var totObject = itemInput[4];

                // obtengo la informacion:
                var qty = getValueSafe(qtyObject.value);
                var prc = getValueSafe(prcObject.value);
                var ivaRate = 0.00;

                if (qty < 0) {
                    qty = -qty;
                }

                if (prc < 0) {
                    prc = -prc;
                }

                var ivaItem = ivaList.find(o => o.id == ivaSelect.value);

                if (typeof ivaItem !== 'undefined' && ivaItem !== null && ivaItem.rate > 0) {
                    ivaRate = ivaItem.rate;
                }

                var dsc = getValueSafe(dscObject.value);

                if (dsc < 0) {
                    dsc = 0;
                }

                if (dsc > 100) {
                    dsc = 100;
                }

                var subtotal = getValueSafe(qty * prc, 6);

                var valdesc = getValueSafe(subtotal * (dsc / 100), 2);

                subtotal = getValueSafe(subtotal - valdesc, 2);  // el valor menos el descuento
                 
                var iva = getValueSafe(subtotal * (ivaRate / 100), 2);

                total_descuento = total_descuento + valdesc;

                var priceFixed = prc.toFixed(6);

                if (priceFixed.endsWith("0000")) {
                    priceFixed = prc.toFixed(2);
                }

                var qtyFixed = qty.toFixed(6);

                if (qtyFixed.endsWith("000000")) {
                    qtyFixed = qty.toFixed(0);
                }
                else if (qtyFixed.endsWith("0000")) {
                    qtyFixed = qty.toFixed(2);
                }

                qtyObject.value = qtyFixed;
                prcObject.value = priceFixed;
                dscObject.value = dsc.toFixed(2);
                ivaObject.value = iva.toFixed(2);

                totObject.value = subtotal.toLocaleString('en-US', {
                    style: 'currency',
                    currency: 'USD'
                });

                // Acumulamos el valor del IVA
                total_iva = total_iva + iva;

                if (typeof ivaItem !== 'undefined' && ivaItem !== null) {

                    if (ivaItem.id == '2' || ivaItem.id == '3' || ivaItem.id == '8') {
                        subtotal_12 = subtotal_12 + subtotal;
                    }
                    else if (ivaItem.id == '6') {
                        subtotal_nosujeto = subtotal_nosujeto + subtotal;
                    }
                    else if (ivaItem.id == '7') {
                        subtotal_exento = subtotal_exento + subtotal;
                    }
                    else {
                        subtotal_0 = subtotal_0 + subtotal;
                    }

                }

            }
        }

        var propinaObj = $("#documento_valor_propina");

        if (typeof propinaObj !== 'undefined') {
            valor_propina = getValueSafe(propinaObj.val());

            if (typeof valor_propina == 'undefined' || valor_propina < 0)
                valor_propina = 0;

            setValueSafe(propinaObj, valor_propina);
        }


        // Calculo del IVA de la notaCredito

        if (typeof total_iva !== 'undefined') {
            total_iva = getValueSafe(total_iva);
        }

        // Calculo del subtotal sin Impuestos
        subtotal_sin_impuestos = subtotal_12 + subtotal_0 + subtotal_nosujeto + subtotal_exento;

        // Total general de la notaCredito
        total = getValueSafe(subtotal_sin_impuestos + total_iva + valor_propina, 2);

        // Si el valor modificado esta cero o si valor modificado es mayor que el total
        // o si el valor modificado es igual al total anterior, entonces debe actualizar el valor
        if (valor_modificado == 0.00 || valor_modificado > total || valor_modificado == oldtotal) {
            valor_modificado = total;
        }

        // Actualizo los totales con los valores actuales
        var objsubtotal = $("#documento_subtotal_sin_impuestos");
        var objsubtotal_12 = $("#documento_subtotal_12");
        var objsubtotal_0 = $("#documento_subtotal_0");
        var objtotobjiva = $("#documento_subtotal_no_iva");
        var objtotexciva = $("#documento_subtotal_exento_iva");
        var objtotdesc = $("#documento_total_descuento");
        var objtotiva = $("#documento_iva");
        var objtotal = $("#documento_valor_total");
        var objvalorModificado = $("#documento_valor_modificado");
        var objvalorTotal = $("#ValorTotal");

        // Actualizamos los detalles
        setValueSafe(objsubtotal, subtotal_sin_impuestos);
        setValueSafe(objsubtotal_12, subtotal_12);
        setValueSafe(objsubtotal_0, subtotal_0);
        setValueSafe(objtotobjiva, subtotal_nosujeto);
        setValueSafe(objtotexciva, subtotal_exento);
        setValueSafe(objtotdesc, total_descuento);
        setValueSafe(objtotiva, total_iva);
        setValueSafe(objtotal, total);

        objvalorModificado.val(valor_modificado.toFixed(2));

        setValueSafe(objvalorTotal, subtotal_sin_impuestos);

        $("#ValorIVA").text(total_iva)
        // flag para evitar sobrecalculos en procesos
        calculating = 0;
    }

}

function generateDocument(emitirDocumento) {

    var fecha_Documento = $('#fecha_documento').val();

    var objcontributor = $("#inputClienteId");

    var contributor_id = objcontributor.val();

    if (contributor_id < 1) {
        return valError("Por favor seleccione un proveedor!", objcontributor);
    }

    var razonSocial = $("#razon_social").val();
    var identificacion = $("#identificacion_proveedor").val();
    var tipoIdentificacion = $("#tipo_identificacion").val();


    var objReason = $("#motivo");
    var documentReason = objReason.val();

    if (documentReason == null || documentReason == '') {
        return valError("El motivo o explicacion de la nota de credito esta vacio!", objReason);
    }

    var refDocumentCode = $("#ref_tipo_comprobante").val();

    if (refDocumentCode == null || refDocumentCode == "") {
        return valError("Por favor seleccione un tipo de comprobante!");
    }

    var refDocumentNumber = $("#ref_numero_comprobante").val();

    if (refDocumentNumber == null || refDocumentNumber == "") {
        return valError("Por favor ingrese un numero de documento valido!");
    }

    var refDocumentDate = $("#ref_fecha_documento").val();

    var refDocumentAuth = $("#ref_autorizacion_comprobante").val();
    if (refDocumentAuth == null || refDocumentAuth == "") {
        return valError("Por favor ingrese el numero de autorizacion del comprobante sustento de la nota de credito!");
    }

    if (valor_modificado <= 0.00) {
        return valError("El valor modificado no puede ser menor o igual a cero!");
    }

    if (valor_original > 0 && total > valor_original) {
        return valError("El valor total del documento no puede ser mayor al comprobante sustento de la nota de credito!");
    }

    var model = {
        ContributorId: contributor_id,
        DocumentTypeCode: "01", // FACTURA
        Currency: "DOLAR",
        IssuedOn: fecha_Documento,
        Subtotal: subtotal_sin_impuestos,
        SubtotalVat: subtotal_12,
        SubtotalVatZero: subtotal_0,
        SubtotalNotSubject: subtotal_nosujeto,  // Subtotal no sujeto
        SubtotalExempt: subtotal_exento, // Subtotal exento a la nota de credito
        TotalDiscount: total_descuento,
        SpecialConsumTax: 0.00, // Valor del ICE
        ValueAddedTax: total_iva,
        Tip: valor_propina, // PROPINA
        Total: total,
        ModifiedValue: valor_modificado,
        Details: [],
        AdditionalFields: [],

        Reason: documentReason,

        ReferenceDocumentCode: refDocumentCode,
        ReferenceDocumentNumber: refDocumentNumber,
        ReferenceDocumentDate: refDocumentDate,
        ReferenceDocumentAuth: refDocumentAuth

    };

    if (typeof emitirDocumento !== 'undefined' && emitirDocumento) {
        model.Status = 1;
    }
    else {
        model.Status = 0;
    }

    if (total < 0) {
        return valError("El total de la notaCredito no puede ser menor a cero!");
    }


    // Busco dentro de los items de la lista
    var itemObj = $(".product-item");
    for (var i = 0; i < itemObj.length; i++) {
        var itemobj = $(itemObj[i]).find("select");
        var itemId = itemobj.val();

        if (itemId == null || itemId == 0) {
            return valError("Debe especificar el producto o servicio!", itemobj);
        }

        var inputs = $(itemObj[i]).find("input");

        var qty = getValueSafe(inputs[0].value);
        var prc = getValueSafe(inputs[1].value);
        var iva = getValueSafe(inputs[2].value);
        var dsc = getValueSafe(inputs[3].value);
        var tot = getValueSafe(((qty * prc)).toFixed(2));


        if (qty <= 0.00000) {
            return valError("El valor para el campo Cantidad no puede ser 0.00 ", inputs[0]);
        }

        if (prc <= 0.00000) {
            return valError("El valor para el campo Precio no puede ser 0.00 ", inputs[1]);
        }

        if (dsc < 0 || dsc > 100) {
            return valError("El valor para el campo Descuento no puede ser menor a cero o mayor que 100%. ", inputs[3]);
        }

        if (tot <= 0.00000) {
            return valError("El total para el item no puede ser menor a cero. ", inputs[0]);
        }

        if (dsc > 0) {
            tot = getValueSafe(tot - (tot * (dsc / 100)), 2);
        }


        /// PROCESO DE CALCULO E INGRESO DE IMPUESTOS
        var ivaSelect = $(itemObj[i]).find("select")[1];
        var ivaItem = ivaList.find(o => o.id == ivaSelect.value);

        var ivaRate = 0.00;
        var ivaCode = "2"; // CODIGO SEGUN LA TABLA TECNICA DEL SRI IVA=="2"
        var ivaPercentage = "0";

        if (typeof ivaItem !== 'undefined' && ivaItem !== null) {
            ivaRate = ivaItem.rate;
            ivaPercentage = ivaItem.id;
        }

        var obj = {
            "ProductId": itemId,
            "Amount": qty,
            "UnitPrice": prc,
            "Discount": dsc,
            "SubTotal": tot,
            "ValueAddedTaxCode": ivaPercentage,
            "ValueAddedTaxValue": iva, 
        };

        model.Details.push(obj);
    }



    var line = 1;


    // Grabo los detalles adicionales en el modelo de datos;
    var additionalObj = $(".additional-item");

    for (i = 0; i < additionalObj.length; i++) {
        inputs = $(additionalObj[i]).find("input");
        var name = inputs[0].value;
        var value = inputs[1].value;
        if (typeof value == 'string' && value !== "") {
            obj = {
                "Name": name,
                "Value": value,
                "LineNumber": line
            };

            if (typeof value == 'string' && value && value !== "") {
                model.AdditionalFields.push(obj);
                line++;
            }
        }
    }


    return model;
}

 // Muestra el mensaje de validacion de la notaCredito
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

function guardarNotaCredito(emitirDocumento) {
    if (guardandoDocumento) {
        return;
    }

    guardandoDocumento = true;

    // Hack para volver la pantalla al inicio de la pagina
    // y los mensajes se ubiquen en su lugar adecuado.
    window.scrollTo(0, 0);


    showLoader("<label><H1 >Guardando Registro... Por favor Espere.</H1><label>");

    var notaCreditoModel = generateDocument(emitirDocumento);

    if (notaCreditoModel == null) {        
        return;
    }

    $.ajax({
        url: creditNoteSaveAction,
        type: "POST",
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            model: notaCreditoModel
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
                    Swal.fire("Nota de Credito", msg, "error");
                }

            debugger;

        },
        success: function (data, usr, obj, etc) {
            guardandoDocumento = false;

            if (data.id > 0) // Si el ID del Registro es mayor que cero, es un registro real
            {
                documentoNotaCredito = data.result;

                Swal.fire("Nota de Credito", "El registro se guardo con Exito!", "success");

                // Si los datos fueron guardados correctamente entonces realizo el proceso de bloqueo de todos los controles
                $('.notaCredito')
                    .find('input, textarea, button, select')
                    .attr('disabled', 'disabled', 'div');

                //$("#numero_documento")[0].innerText = (documentoNotaCredito.DocumentNumber);

                //// ejecuta el proceso de carga de los datos de autorizacion

                //$("#numero_documento").focus();

                if (typeof redirectAction !== 'undefined') {
                    redirectAction('Ver', documentoNotaCredito.Id);
                }
            }
            else {
                if (data.error) {
                    if (data.error.UserMessage) {
                        Swal.fire("Nota de Credito", data.error.UserMessage, "error");
                    }
                    else if (data.message) {
                        Swal.fire("Nota de Credito", data.message, "error");
                    }
                    else {
                        Swal.fire("Nota de Credito", data.error.Message, "error");
                    }
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
    var urlAction = getUrlContribAction();

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

        var urlAction = getUrlContribAction(cliente_id);

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

function formatDoc(repo) {
    if (repo) {
        return repo.text;
    }

    return "No hay documentos que mostrar...";
}

function formatDocSelection(repo) {
    return formatDoc(repo);
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
            "<div class='select2-result-repository__phone'><i class='fa fa-phone'></i> Telefono:" + (data.Phone ? data.Phone : "") + "</div>" +
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

        $("#bodyContrib").css("height", "150px");
        $("#cliente_tipo_identificacion").val("02");
        $("#actualizarClienteButton").removeClass("kt-hide");

        /// Cargo la informacion en los elementos
        $("#headDocument").removeClass("kt-hide");
        $("#headDocument").fadeIn();

        ///Limpiamos los campos del documento sustento                    
        $("#ref_numero_comprobante").val('');
        $("#ref_fecha_documento").val('');
        $("#ref_autorizacion_comprobante").val('');
        const $select = $("#inputDocumentId");
        $select.empty();

        agregarAditional("Direccion", data.Address);
        agregarAditional("Email", data.EmailAddresses);
        agregarAditional("Telefono", data.Phone);

        startDocumentSelector(repo.id);

        return markup;
    }
    else {
        $("#bodyContrib").css("height", "");
        $("#actualizarClienteButton").addClass("kt-hide");
        $("#headDocument").addClass("kt-hide");
        $("#headDocument").fadeOut();

        if (typeof agregarAditional !== 'undefined') {
            agregarAditional("Direccion", "");
            agregarAditional("Email", "");
            agregarAditional("Telefono", "");
        }
    }

    return repo.text;

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
        '<tr id="additional' + adCount + '" class="additional-item kt-datatable__row">' +
        '<td class="kt-datatable__cell" style="width:100%">' + '<div class="row">' +
        '<div data-title="Detalle" class="col-lg-6 col-md-12 col-sm-6 col-xs-12 bold" style="padding-bottom:5px">' +
        '<input class="form-control bold"  id="adname' + adCount + '"  style="text-align:left; width:100%;" type="text" placeholder="Nombre" list="list-references" value="' + adname + '" />' +
        '</div>' +
        '<div data-title="Valor" class="col-lg-6 col-md-12 col-sm-6 col-xs-12">' +
        '<input class="form-control" style="text-align:left; " id="addet' + adCount + '" type="text" placeholder="Detalle" value="' + advalue + '" />' +
        '</div>' + '</div>' +
        '</td>' +
        '<td class="kt-datatable__cell" data-title=""><button id="trash' + adCount + '"  type="button" onclick="quitarAdicional(this)" class="tabledit-delete-button btn btn-sm btn-dark" style="float: none;width:auto; back;"><span class="la la-trash-o text-light"></span></button></td>' +
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

function formatProduct(repo) {
    if (repo.loading) {
        return repo.text;
    }

    return repo.text;
}

function formatProductSelection(repo) {

    // Si existe el objeto de datos se muestra la informacion para agregarlo a la lista
    if (typeof repo.data !== 'undefined') {
        var data = repo.data;

        // Buscamos id del elemento TR
        var itemElementId = repo.element.parentElement.attributes["data-parent"].value;

        var trItem = $("#" + itemElementId + " input");
        trItem[1].value = repo.data.UnitPrice;

        var ivaitem = ivaList.find(o => o.data.Id == repo.data.IvaRateId);

        if (typeof ivaitem !== 'undefined' && ivaitem !== null) {
            $("#iva" + itemElementId).val(ivaitem.id);
            $("#iva" + itemElementId).trigger('change');
        }

        actualizarTotales();


        if (typeof initialized !== 'undefined' && initialized) {
            trItem[0].focus();
        }
    }

    return repo.text;

}

function agregarItem(detalleItem) {
    itemListCount++;

    var itemListId = "selectProducto" + itemListCount;
    var ivaItemsHtml = '';

    for (var i = 0; i < ivaList.length; i++) {
        ivaItemsHtml += "<option style='text-align:right;' value='" + ivaList[i].id + "'>" + ivaList[i].text + "</option>";
    } 

    var markup =
        '<tr id="item' + itemListCount + '" class="product-item">' +
        '<td data-title="Product" style="width:100%">' +
        '<div class="row">' +
        '<div class="col-md-10 col-sm-8 col-xs-12" >' +

        '<div class="row">' +

        '<div class="col-md-4 col-sm-12" data-title="Detalle" class="numeric" style="padding-bottom:5px">' +
        '<select id = "' + itemListId + '" class="form-control" data-parent="item' + itemListCount + '" style = "width:100%;font-size:11px;background:none;border:none;" class="js-productSelector" id = "inputBuscarProducto" ></select>' +
        '</div> ' +

        '<div class="col-md-2 col-sm-6 col-xs-12" data-title="Cantidad" class="numeric" style="padding-bottom:5px"> ' +
        '<input id="qty' + itemListCount + '" onchange="actualizarTotales()" class="form-control" style="text-align:right; background:none;" type="number" value="1" min="1" placeholder="Cantidad" />' +
        '</div>' +

        '<div class="col-md-2 col-sm-6 col-xs-12" data-title="Precio" class="numeric" style="padding-bottom:5px">' +
        '<input id="price' + itemListCount + '" onchange="actualizarTotales()"   class="form-control" style="text-align:right;background:none;  " type="text" value="0.00" min="0.01" step="0.01" placeholder="Precio" />' +
        '</div>' +

        '<div class="col-md-2 col-sm-6 col-xs-12" data-title="IVA" class="numeric" style="padding-bottom:5px">' +
        '<select id="ivaitem' + itemListCount + '"  class="form-control" style="text-align-last:right; direction: rtl; text-align:right; background:none;" onchange="actualizarTotales()" type="text">' + ivaItemsHtml + '</select>' +
        '<input id="iva' + itemListCount + '" value="0.00" type="hidden"></input>' +
        '</div>' +

        '<div class="col-md-2 col-sm-6 col-xs-12" data-title="Desc" class="numeric" style="padding-bottom:5px">' +
        '<input id="dsc' + itemListCount + '"  class="form-control" onchange="actualizarTotales()" onblur="selectAddButton()" style="text-align:right; background:none; " type="number" value="0" min="0"  max="100" step="1" placeholder="% Descuento" />' +
        '</div>' +

        '</div>' +

        '</div>' +

        '<div class="col-md-2 col-sm-4 col-xs-12" style="padding-bottom:5px">' +

        '<div class="col-md-12 col-sm-12 col-xs-12 numeric center" data-title="Total">' +
        '<h4><input  class="" id="tot' + itemListCount + '" style="width:100%;text-align:center;border:none;background:none;" type="text" value="0.00" min="0.00" step="0.01" readonly disabled /></h4>' +
        '</div>' +

        '</div>' +

        '</div>' +

        '</td>' +

        '<td data-title="">' +
        '<button id="trash' + itemListCount + '"  type="button" onfocus="enfocarLista(this)" onclick="quitarItem(this)" class="tabledit-delete-button btn btn-sm btn-dark" style="float: none;"><span class="la la-trash-o"></span></button>' +
        '</td>' +
        '</tr>';


    var result = $("#product-items").append(markup);
    var obj = $("#" + itemListId);
    var iva = $("#ivaitem" + itemListCount);



    // Se actualiza la informacion del detalle
    if (detalleItem) {

        if (detalleItem.Description != '' && detalleItem.ProductId > 0) {

            var option = new Option(detalleItem.MainCode + ' - ' + detalleItem.Description, detalleItem.ProductId, true, true);
            obj.append(option).trigger('change');
            obj.val(detalleItem.ProductId);

            // manually trigger the `select2:select` event
            obj.trigger(
                {
                    type: 'select2:select',
                    params: { data: detalleItem }
                });

            startSelectProduct(obj);
        }
        else {
            cargarItem(obj, detalleItem.MainCode).always(function () {
                startSelectProduct($el);
            });
        }

        $('#qty' + itemListCount).val(detalleItem.Amount);
        $('#dsc' + itemListCount).val(detalleItem.Discount);
        var subtotal = (detalleItem.Amount * detalleItem.UnitPrice);
        if (detalleItem.Discount > 0) {
            subtotal = subtotal - (subtotal * (detalleItem.Discount / 100));
        }

        setValueSafe($('#price' + itemListCount), detalleItem.UnitPrice);
        setValueSafe($('#tot' + itemListCount), subtotal);

        // Revisamos los impuestos
        if (detalleItem.Taxes && detalleItem.Taxes.length > 0) {
            var itemtax = detalleItem.Taxes[0];
            if (itemtax) {
                var taxcode = itemtax.PercentageCode;
                var taxvalue = itemtax.TaxValue;
                $('#ivaitem' + itemListCount).val(taxcode);
                $("#iva" + itemListCount).val(taxvalue);
            }
        }

        actualizarTotales();

    }
    else {
        startSelectProduct(obj);

        if (typeof initialized !== 'undefined' && initialized) {
            if (itemListCount > 0) {
                obj.focus();
                obj.select2("open");
            }
        }
    }



}

function cargarItem($el, $code) {
    // Primero cargamos los documentos
    return $.ajax({
        type: 'GET',
        url: getProductByCode($code)
    }).then(function (result) {
        if (result && result !== null && result.length > 0) {
            var dataItem = result[0];

            // create the option and append to Select2
            var option = new Option(dataItem.text, dataItem.id, true, true);
            $el.append(option).trigger('change');
            $el.val(dataItem.id);

            // manually trigger the `select2:select` event
            $el.trigger({
                type: 'select2:select',
                params: { data: result }
            });
        }

    });

}

function enfocarLista(objparent) {
    if (typeof objparent !== 'undefined') {
        var objName = objparent.parentElement.parentElement.id;

        var obj = $("#" + objName + " select");

        if (typeof initialized !== 'undefined' && initialized)
            if (typeof obj !== 'undefined' && obj.length > 0) {
                obj[0].focus();
            }
    }
}

function quitarItem(obj) {

    if (obj.parentElement.parentElement.parentElement.childElementCount == 1) {
        Swal.fire("Nota de Credito", "Se requiere al menos un item en la lista!", "warning");
        return;
    }

    obj.parentElement.parentElement.remove();
    actualizarTotales();
}

function selectAddButton() {
    if (typeof initialized !== 'undefined' && initialized)
        $("#addItemButton").focus();
}

function startSelectProduct(obj) {
    obj.select2({
        ajax: {
            url: getSearchProductUrl(),
            dataType: 'json',
            async: true,
            delay: 1000,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page
                };
            },
            error: function (data, a, e, i) {

                if (data !== null && typeof data.responseText == 'string' &&
                    data.responseText.includes("login-form")) {
                    Swal.fire("Error", "Su sesion ha caducado. Vuelva a iniciar sesion", "warning");

                    location.href = getLoginFormUrl() + '?ReturnURL=' + location.href;
                }
                else {
                    //Swal.fire("Error","Hubo un error al obtener los datos","error");
                }
            },
            cache: true
        },
        allowClear: false,
        placeholder: 'Seleccione un producto',
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        templateResult: formatProduct,
        templateSelection: formatProductSelection,
        language: {
            inputTooShort: function (args) {
                var remainingChars = args.minimum - args.input.length;

                var message = 'Por favor escriba al menos ' + remainingChars + ' caracteres' +
                    "&nbsp; <button type='button' onclick='javascript:crearProducto(this);' class='btn btn-outline blue'> Agregar <i class='fa fa-plus'></i></button>";

                return message;

            },
            noResults: function () {
                return "Registro no encontrado &nbsp; <button type='button' onclick='javascript:crearProducto(this);' class='btn btn-outline blue'> Agregar <i class='fa fa-plus'></i></button>";
            },
            searching: function () {
                return "Buscando...";
            }
        }
    });
}

function actualizarValorModificado() {
    var objvalorModificado = $("#documento_valor_modificado");
    valor_modificado = getValueSafe(objvalorModificado.val());
    if (valor_modificado <= 0.00) {
        valError("El valor modificado no puede ser menor o igual a cero!", objvalorModificado);
    }

    if (valor_modificado > total || valor_modificado <= 0.00) {
        valor_modificado = total;
    }

    setValueSafe(objvalorModificado, valor_modificado);
}

function crearProducto() {
    showLoader();

    $.get(getCreateProductUrl(), {}, function (data) {
        $("#myModal").html(data);

        hideLoader();

        $("#myModal").fadeIn();
        $("#myModal").modal("show");
    });
}

function startCustomerSelector() {

    $(".js-customerSelector").select2({
        ajax: {
            url: getUrlSearchContrib(),
            dataType: 'json',
            async: true,
            delay: 1000,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page
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
        allowClear: true,
        placeholder: 'Por favor seleccione un cliente',
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        templateResult: formatContrib,
        templateSelection: formatContribSelection,
        language: {
            inputTooShort: function (args) {
                var remainingChars = args.minimum - args.input.length;

                var message = 'Por favor escriba al menos ' + remainingChars + ' caracteres';
                //+ "&nbsp; <button type='button'  data - toggle='modal' data - target='#modal-cliente' onclick='javascript:agregarComprador();' class='btn btn-outline blue'> Agregar <i class='fa fa-plus'></i></button>";

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

// tiene que ver con el Bundling del Javascript, el parametro tiene el mismo nombre que la data>params.
function startDocumentSelector(cliente_id) {

    if (initialized && cliente_id && cliente_id > 0) {
        var $url = getUrlSearchDocument(cliente_id);
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
                url: $url,
                dataType: 'json',
                //async: true,
                delay: 1000,
                //data: function (data) {
                //    return {
                //        search: data.term, // search term
                //        page: data.page
                //    };
                //},
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

function documentListChanged(repo) {
    if (initialized) {

        var docListObj = $(".js-documentSelector");

        if (docListObj && docListObj.select2) {

            var dataSelect = docListObj.select2("data");

            if (!repo && dataSelect) {
                repo = dataSelect[0];
            }

            if (repo && repo.id > 0) {
                var url = getUrlGetDocument(repo.id);

                $.get(url, {}, cargarDocumento);
            }

        }
    }
}

function cargarDocumento(doc) {
    if (doc) {
        console.log(doc);

        
        $("#ref_tipo_comprobante").val(doc.DocumentTypeCode);
        $("#ref_numero_comprobante").val(doc.DocumentNumber);
        $("#ref_autorizacion_comprobante").val(doc.AuthorizationNumber);

        $("#ref_fecha_documento").val(doc.InvoiceInfo.IssuedOn);

        valor_original = doc.Total;

        $("#product-items tbody").empty();

        if (doc.InvoiceInfo) {

            var details = doc.InvoiceInfo.Details;
            var detailCount = details.length;

            for (var i = 0; i < detailCount; i++) {
                var item = details[i];

                agregarItem(item);
            }
        }

        actualizarTotales();
    }
    else {
        valor_original = 0.00;
    }
}

window.scrollTo(0, 0);