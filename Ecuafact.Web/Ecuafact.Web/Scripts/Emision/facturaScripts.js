/* eslint-disable */
var invoiceSaveAction = "Factura/SaveInvoiceAsync";
var printInvoiceAction = "Factura/PrintDocument";
var documentoFactura = {};
var ivaList = [];
var productData = [];
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
var valor_propina = 0.00;
var totalPagos = 0.00;
var saldoPagos = 0.00;
var initialized = false;
var calculating = 0;


var paymentItemCount = -1;
var itemListCount = -1;
var adicionalItemCount = 0;

//////////////////////////////// ACTIONS ////////////////////////////////
var getUrlContribAction = function (id) { return "/Contribuyentes/Nuevo"; };
var getUrlSearchContrib = function () { return "/Factura/SearchContribAsync"; };
var getLoginFormUrl = function () { return "/Auth"; };
var getUrlAuthorization = function (docTypeCode, docNumber) { return '/Factura/GetDocumentAuthSync?docType=' + docTypeCode + '&docNumber=' + docNumber; };
var getFormasDePago = function () { return []; };
var getSearchProductUrl = function () { return "/Factura/SearchProductAsync"; };  // se requiere implementacion 
var getCreateProductUrl = function () { return "/Cliente/Producto"; };  // se requiere implementacion 



 function actualizarTotales() {
    if (calculating == 0) {
        // este es un control para evitar multiples llamadas
        // al proceso de calculo por los procesos propios de la funcion
        calculating = 1;
        subtotal_12 = 0.00;
        subtotal_0 = 0.00;
        total_descuento = 0.00;
        total_iva = 0.00;
        subtotal_nosujeto = 0.00;
        subtotal_exento = 0.00;
        subtotal_sin_impuestos = 0.00;
        total = 0.00;
        valor_propina = 0.00;
        totalPagos = 0.00;
        saldoPagos = 0.00;

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


                var ivaItem = ivaList.find(o => o.id == ivaSelect.value);

                if (typeof ivaItem !== 'undefined' && ivaItem !== null) {
                    ivaRate = ivaItem.rate;
                }

                var dsc = getValueSafe(dscObject.value, 2);

                if (dsc < 0.000000) {
                    dsc = 0.00;
                }
                else {
                    if (dsc>100) {
                        dsc = 100.00;
                    }
                }

                var subtotal = getValueSafe(qty * prc, 6);

                var valdesc = getValueSafe(subtotal * (dsc / 100), 2);

                subtotal = getValueSafe( subtotal - valdesc, 2 );  // el valor menos el descuento

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


                if (ivaItem.id == '2' || ivaItem.id == '3') {
                    subtotal_12 = getValueSafe( subtotal_12 + subtotal, 2 );
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

        var propinaObj = $("#documento_valor_propina");

        if (typeof propinaObj !== 'undefined') {
            valor_propina = getValueSafe(propinaObj.val());

            if (typeof valor_propina !== 'undefined')
                setValueSafe(propinaObj, valor_propina);
        }




        // Calculo del IVA de la factura

        if (typeof total_iva !== 'undefined') {
            total_iva = getValueSafe(total_iva, 2);
        }

        // Calculo del subtotal sin Impuestos
        subtotal_sin_impuestos = getValueSafe( subtotal_0 + subtotal_12 + subtotal_nosujeto + subtotal_exento, 2 );

        // Total general de la factura
        total = getValueSafe( subtotal_sin_impuestos + total_iva + valor_propina, 2 );
         

        // Realizo el calculo de Pagos
        var paymentObj = $(".payment-item").find("input");

        // Calculo los pagos realizados
        for (var p = 0; p < paymentObj.length; p++) {
            var opay = paymentObj[p];
            if (opay && typeof opay != 'undefined') {

                var paymentValue = getValueSafe(opay.value, 2);

                if (paymentValue < 0) {
                    valError("El valor de los pagos no puede menor un valor negativo!");
                    paymentValue = 0;
                }

                totalPagos = totalPagos + paymentValue;

                setValueSafe(opay, paymentValue);

            }
        }

        // Si solo existe una linea de pagos entonces
        // debe llenarse con el valor total de la factura
        if (paymentObj.length > 0) {
            if (paymentObj.length == 1) {
                totalPagos = total;
                setValueSafe(paymentObj, total);
            }
            else {
                var primerPago = getValueSafe(paymentObj[0].value);
                var totalPrimerPago = total - (totalPagos - primerPago);
                if (totalPrimerPago > 0) {
                    totalPagos = totalPagos - primerPago + (totalPrimerPago);
                    setValueSafe(paymentObj, totalPrimerPago);    
                }
            }
        }

        // Calculo el saldo de valor por pagos
        saldoPagos = total - totalPagos;


        // Actualizo los totales con los valores actuales
        var objsubtotal = $("#documento_subtotal_sin_impuestos");
        var objsubtotal_12 = $("#documento_subtotal_12");
        var objsubtotal_0 = $("#documento_subtotal_0");
        var objtotobjiva = $("#documento_subtotal_no_iva");
        var objtotexciva = $("#documento_subtotal_exento_iva");
        var objtotdesc = $("#documento_total_descuento");
        var objtotiva = $("#documento_iva");
        var objtotal = $("#documento_valor_total");
        var objvalorTotal = $("#ValorTotal");
        var objvalorIva = document.getElementById("ValorIVA");

        // Objetos de calculo de pagos
        var objtotpagos = $("#total_payment");
        var objsalpagos = $("#balance_payment");

        setValueSafe(objsubtotal, subtotal_sin_impuestos);
        setValueSafe(objsubtotal_12, subtotal_12);
        setValueSafe(objsubtotal_0, subtotal_0);
        setValueSafe(objtotobjiva, subtotal_nosujeto);
        setValueSafe(objtotexciva, subtotal_exento);
        setValueSafe(objtotdesc, total_descuento);
        setValueSafe(objtotiva, total_iva);
        setValueSafe(objtotal, total);

        setValueSafe(objvalorTotal, subtotal_sin_impuestos);

        objvalorIva.innerHTML = total_iva.toFixed(2);

        setValueSafe(objtotpagos, totalPagos);
        setValueSafe(objsalpagos, saldoPagos);


        if (totalPagos < 0) {
            valError("El valor de los pagos no puede ser valor negativo!");
            $(objtotpagos).focus();
            $(objtotpagos).addClass("btn-danger");
        }
        else {
            $(objtotpagos).removeClass("btn-danger");
        }

        if (saldoPagos < 0) {
            valError("El valor de los pagos no puede ser mayor al total de la factura!");
            $(objsalpagos).addClass("btn-danger");
        }
        else {
            $(objsalpagos).removeClass("btn-danger");
        }


        if (valor_propina < 0) {
            valError("El valor de la propina no puede menor que 0 (cero)!");
            $(propinaObj).addClass("btn-danger");
        }
        else {
            $(propinaObj).removeClass("btn-danger");
        }


        // flag para evitar sobrecalculos en procesos
        calculating = 0;
    }

}


function generateDocument(emitirDocumento) {
     
    var fecha_Documento = $('#fecha_documento').val();
    var guia_remision = $("#guia_remision").val();
    var contributor_Id = $("#inputClienteId").val();
    var contributor_Identification = $("#cliente_identificacion").val();
    var contributor_Name = $("#cliente_nombre").val();
    var contributor_Address = $("#cliente_address").val();
    var contributor_Phone = $("#cliente_phone").val();
    var contributor_Email = $("#cliente_email").val();

    var model = {
        ContributorId: contributor_Id,
        Identification: contributor_Identification,
        ContributorName: contributor_Name,
        Address: contributor_Address,
        Phone: contributor_Phone,
        EmailAddresses: contributor_Email,
        Reason: "**DOCUMENTO GENERADO POR ECUAFACT EXPRESS**",
        DocumentTypeCode: "01", // FACTURA
        Currency: "DOLAR",
        IssuedOn: fecha_Documento,
        ReferralGuide: guia_remision,
        Subtotal: subtotal_sin_impuestos,
        SubtotalVat: subtotal_12,
        SubtotalVatZero: subtotal_0,
        SubtotalNotSubject: subtotal_nosujeto,  // Subtotal no sujeto
        SubtotalExempt: subtotal_exento, // Subtotal exento a la retencion
        TotalDiscount: total_descuento,
        SpecialConsumTax: 0.00, // Valor del ICE
        ValueAddedTax: total_iva,
        Tip: valor_propina, // PROPINA
        Total: total,
        Details: [],
        AdditionalFields: [],
        Payments: []
    };

    if (typeof emitirDocumento !== 'undefined' && emitirDocumento) {
        model.Status = 1;
    }
    else {
        model.Status = 0;
    }

    if (total < 0) {
        return valError("El total de la factura no puede ser un valor negativo!");
    }

    if (valor_propina < 0) {
        return valError("El valor de la propina no puede ser un valor negativo!");
    }

    if (total != totalPagos) {
        return valError("El total de la factura debe ser igual al valor de las formas de pago!");
    }

    if (contributor_Id == 0 && total > 200) {
        return valError("No se puede facturar a nombre de -CONSUMIDOR FINAL- un valor superior a $200.00");
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
        //var iva = getValueSafe(inputs[2].value);
        var dsc = getValueSafe(inputs[3].value);
        var tot = getValueSafe(inputs[4].value);
         
        if (qty <= 0.000000) {
            return valError("El valor para el campo Cantidad no puede ser 0.00 ", inputs[0]);
        }

        if (prc <= 0.000000) {
            return valError("El valor para el campo Precio no puede ser 0.00 ", inputs[1]);
        }

        if (dsc < 0 || dsc > 100) {
            return valError("El valor para el campo Descuento no puede ser un valor negativo o mayor que 100%. ", inputs[3]);
        }

        if (tot <= 0.00000) {
            return valError("El total para el item no puede ser un valor negativo. ", inputs[1]);
        }

        /// PROCESO DE CALCULO E INGRESO DE IMPUESTOS
        var ivaSelect = $(itemObj[i]).find("select")[1];
        var ivaItem = ivaList.find(o => o.id == ivaSelect.value);

        var ivaRate = 0.00;
        var ivaCode = "2"; // CODIGO SEGUN LA TABLA TECNICA DEL SRI IVA=="2"
        var ivaPercentage = "0";

        if (typeof ivaItem !== 'undefined' && ivaItem !== null) {
            ivaRate = ivaItem.rate * 1;
            ivaPercentage = ivaItem.id;
        }
        
        var iva = getValueSafe(tot * (ivaRate>0 ? ivaRate/100 : 0), 2);

        var obj = {
            "ProductId": itemId,
            "Amount": qty,
            "UnitPrice": prc,
            "Discount": dsc,
            "SubTotal": tot,
            "Taxes": [
                {
                    "Code": ivaCode,
                    "PercentageCode": ivaPercentage, // Codigo del IVA
                    "Rate": ivaRate,
                    "TaxableBase": tot,
                    "TaxValue": iva 
                }
            ]
        };
         

        model.Details.push(obj);
    }


    // Grabo los pagos en el modelo de datos;
    var paymentObj = $(".payment-item");
    var termObj = $("#payment-term");
    var timeUnitObj = $("#payment-timeunit");
    var termValue = termObj.val();
    var timeUnitValue = timeUnitObj.val();

    if (!termValue) {
        termValue = 0;
    }

    if (!timeUnitValue || timeUnitValue=='') {
        timeUnitValue = "dias";
    }

    for (var p = 0; p < paymentObj.length; p++) {
        var payment = $(paymentObj[p]);

        var paymentItem = payment.find("select");
        var paymentCode = paymentItem.val();
        var paymentName = paymentItem[0].children[paymentItem[0].selectedIndex].text;
        var paymentValue = payment.find("input").val();

        if (paymentValue) {
            paymentValue = paymentValue.replace(",", "").replace("$", "");
            paymentValue = getValueSafe(paymentValue);
        }
        else {
            paymentValue = 0;
        }

        if (paymentValue <= 0.00000) {
            return valError("El valor de la forma de pago debe ser superior a cero (0.00) ", payment);
        }

        model.Payments.push({
            "PaymentMethodCode": paymentCode,
            "Name": paymentName,
            "Total": paymentValue,
            "Term": termValue, // en caso que se facture a credito
            "TimeUnit": timeUnitValue // la unidad de tiempo del credito: meses, semanas, dias
        });
    }

    var line = 1;


    // Grabo los pagos en el modelo de datos;
    var additionalObj = $(".additional-item");

    for (i = 0; i < additionalObj.length; i++) {

        inputs = $(additionalObj[i]).find("input");

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
    }

    var dirx = model.AdditionalFields.find(o => o.Name == "Direccion");
    if (typeof dirx !== 'undefined') {
        model.Address = dirx.Value;
    } 

    var telx = model.AdditionalFields.find(o => o.Name == "Telefono");
    if (typeof telx !== 'undefined') {
        model.Phone = telx.Value;
    } 

    var emailx = model.AdditionalFields.find(o => o.Name == "Email");
    if (typeof emailx !== 'undefined') {
        model.EmailAddresses = emailx.Value;
    }

    var obsx = model.AdditionalFields.find(o => o.Name == "Observaciones");
    if (typeof obsx !== 'undefined') {
        model.Reason = obsx.Value;
    } 
     

    return model;
}


 
// Muestra el mensaje de validacion de la factura
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

function guardarFactura(emitirDocumento) {
    if (guardandoDocumento) {
        return;
    }

    guardandoDocumento = true;

    // Hack para volver la pantalla al inicio de la pagina
    // y los mensajes se ubiquen en su lugar adecuado.
    window.scrollTo(0, 0);


    showLoader("<label><H1 >Guardando Registro... Por favor Espere.</H1><label>");

    var facturaModel = generateDocument(emitirDocumento);

    if (facturaModel == null) {
        debugger;
        return;
    }

    $.ajax({
        url: invoiceSaveAction,
        type: "POST",
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            model: facturaModel
        }),
        async: true,
        error: function (data, msg) {
            guardandoDocumento = false;
            hideLoader();

            // Esto es para saber si el problema tiene que ver con la session
            if (data && data.responseText && data.responseText.includes("login")) {
                Swal.fire("Su Sesion ha Caducado", "Por favor inicie sesion", "error");
                var w = open(location.href, "_top", "height=770,width=520");
            }
            else {
                Swal.fire("Factura", msg, "error");
            }

            debugger;

        },
        success: function (data, usr, obj, etc) {
            guardandoDocumento = false;

            if (data.id > 0) // Si el ID del Registro es mayor que cero, es un registro real
            {
                documentoFactura = data.result;

                Swal.fire("Factura", "El registro se guardo con Exito!", "success");

                // Si los datos fueron guardados correctamente entonces realizo el proceso de bloqueo de todos los controles
                $('.factura')
                    .find('input, textarea, button, select')
                    .attr('disabled', 'disabled', 'div');

                //$("#numero_documento")[0].innerText = (documentoFactura.DocumentNumber);

                //// ejecuta el proceso de carga de los datos de autorizacion

                //$("#numero_documento").focus();

                if (typeof redirectAction !== 'undefined') {
                    redirectAction('Ver', documentoFactura.Id);
                }
            }
            else {
                if (data.error) {
                    if (data.error.UserMessage) {
                        Swal.fire("Factura", data.error.UserMessage, "error");
                    }
                    else if (data.message) {
                        Swal.fire("Factura", data.message, "error");
                    }
                    else {
                        Swal.fire("Factura", data, "error");
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
            "<div class='select2-result-repository__description'><i class='fas fa-chart-area'></i> Nombre Comercial: " + data.TradeName + "</div>" +
            "<div class='select2-result-repository__address'><i class='fas fa-map-marker'></i> Direccion: " + data.Address + " </div>" +
            "<div class='select2-result-repository__email'><i class='fas fa-envelope'></i> E-mail:" + data.EmailAddresses + " </div>" +
            "<div class='select2-result-repository__phone'><i class='fas fa-phone'></i> Telefono:" + (data.Phone ? data.Phone : "") + "</div>" +
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
            "<div class='select2-result-repository__description'><i class='fas fa-chart-area'></i> Nombre Comercial: " + data.TradeName + "</div>" +
            "<div class='select2-result-repository__address'><i class='fas fa-map-marker'></i> Direccion: " + data.Address + " </div>" +
            "<div class='select2-result-repository__email'><i class='fas fa-envelope'></i> E-mail:" + data.EmailAddresses + " </div>" +
            "<div class='select2-result-repository__phone'><i class='fas fa-phone'></i> Telefono:" + data.Phone + "</div>" +
            "</div>" +
            "</div></div>";

        $("#bodyContrib").css("height", "150px");
        $("#cliente_identificacion").val(data.Identification);
        $("#cliente_nombre").val(data.TradeName);
        $("#cliente_address").val(data.Address);
        $("#cliente_phone").val(data.Phone);
        $("#cliente_email").val(data.EmailAddresses);

        $("#actualizarClienteButton").removeClass("kt-hide");
        /// Cargo la informacion en los elementos

        agregarAditional("Direccion", data.Address);
        agregarAditional("Email", data.EmailAddresses);
        agregarAditional("Telefono", data.Phone);
        agregarAditional("Observaciones", "");
          
        return markup;
    }
    else {
        $("#bodyContrib").css("height", "");
        $("#actualizarClienteButton").addClass("kt-hide");

        if (typeof agregarAditional !== 'undefined') {
            $("#cliente_nombre").val("CONSUMIDOR FINAL");
            $("#cliente_identificacion").val("9999999999999");
            $("#cliente_address").val("Av. Principal");
            $("#cliente_phone").val("999999");
            $("#cliente_email").val("facturacion@ecuafact.com");

            agregarAditional("Direccion", "");
            agregarAditional("Email", "");
            agregarAditional("Telefono", "");
            agregarAditional("Observaciones", "");
        }
    }

    return repo.text;

}








function agregarPayment() {
    paymentItemCount++;
    var itemListId = "selectPayment" + paymentItemCount;
    var markup =
        '<tr id="payment' + paymentItemCount + '" class="payment-item">' +
        '<td data-title="Detalle">' +
        '<div class="row">' +

            '<div data-title="FormaPago" class="col-lg-8 col-md-12 col-sm-8 col-xs-12 bold" style="padding-bottom:10px">' +
                '<select id="' + itemListId + '" class="col-md-12 col-sm-12 col-xs-12 js-paymentSelector " id="inputBuscarPago" style="width:100%;"></select>' +
            '</div>' +
            '<div data-title="Valor" class="col-lg-4 col-md-12 col-sm-4 col-xs-12 numeric" style="text-align:right;">' +
                '<input  id="pago' + paymentItemCount + '" onchange="calcularPagos()" class="form-control" style="text-align:right ;" type="text" placeholder="Valor Pagado" />' +
        '</div>' +
        '</div>' +
        '</td>' +
        '<td data-title=""><button id="trash' + paymentItemCount + '" title="Eliminar Registro" type="button" onclick="quitarPago(this)" class="tabledit-delete-button btn" style="width:auto;float:none;"><span class="la la-trash-o"></span></button></td>' +
        '</tr>';

    var result = $("#payment-items").append(markup);
    var obj = $("#" + itemListId);

    startSelectPayment(obj);

    if (typeof initialized !== 'undefined' && initialized) {
        obj.focus();
    }

    if (paymentItemCount > 0) {
        obj.select2("open");
    }
}

function quitarPago(obj) {
    if (obj.parentElement.parentElement.parentElement.childElementCount == 1) {
        Swal.fire("Factura", "Se requiere al menos una forma de pago", "warning");
        return;
    }

    obj.parentElement.parentElement.remove();
    calcularPagos();
}



function startSelectPayment(obj) {
    obj.select2({
        allowClear: false,
        placeholder: 'Seleccione un tipo de pago',
        data: getFormasDePago()
    });
}

function calcularPagos() {
    actualizarTotales();
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

    if (typeof advalue !== 'string' || !advalue) {
        advalue = '';
    }

    if ($("#adname" + adCount).length > 0) {
        $("#addet" + adCount).val(advalue);
        return;
    }

    var markup =
        '<tr id="additional' + adCount + '" class="additional-item">' +
        '<td>' +
        '<div class="row">' +

        '<div data-title="Detalle" class="col-lg-6 col-md-12 col-sm-6 col-xs-12 bold" style="padding-bottom:5px">' +
            '<input class="form-control bold"  id="adname' + adCount + '"  style="text-align:left; width:100%;" type="text" placeholder="Nombre" list="list-references" value="' + adname + '" />' +
        '</div>' +
        '<div data-title="Valor" class="col-lg-6 col-md-12 col-sm-6 col-xs-12">' +
            '<input class="form-control" style="text-align:left; " id="addet' + adCount + '" type="text" placeholder="Detalle" value="' + advalue + '" />' +
        '</div>' +
        '</div>' +
        '</td>' +
        '<td data-title=""><button id="trash' + adCount + '" title="Eliminar Registro"  type="button" onclick="quitarAdicional(this)" class="tabledit-delete-button btn" style="float: none;width:auto;"><span class="la la-trash-o"></span></button></td>' +
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

function agregarItem() {
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
                        '<div class="col-md-4 col-sm-12" data-title="Detalle" class="numeric" style="padding-bottom:5px"><select id = "' + itemListId + '" class="form-control" data-parent="item' + itemListCount + '" style = "width:100%;font-size:11px;background:none;border:none;" class="js-productSelector" id = "inputBuscarProducto" ></select></div> ' +

                        '<div class="col-md-2 col-sm-6 col-xs-12" data-title="Cantidad" class="numeric" style="padding-bottom:5px"> ' +
                            '<input id="qty' + itemListCount + '" onchange="actualizarTotales()" class="form-control" style="text-align:right; background:none;" type="number" value="1" min="1" step="1" placeholder="Cantidad" />' +
                        '</div>' +

                        '<div class="col-md-2 col-sm-6 col-xs-12" data-title="Precio" class="numeric" style="padding-bottom:5px">' +
                            '<input id="price' + itemListCount + '" onchange="actualizarTotales()" class="form-control" style="text-align:right;background:none;  " type="number" value="0.00" min="0.01" step="0.01" placeholder="Precio" />' +
                        '</div>'+

                        '<div class="col-md-2 col-sm-6 col-xs-12" data-title="IVA" class="numeric" style="padding-bottom:5px">' +
                            '<select id="ivaitem' + itemListCount + '" onchange="actualizarTotales()" class="form-control" style="text-align-last:right; direction: rtl; text-align:right; background:none;" type="text">' + ivaItemsHtml + '</select>' +
                            '<input id="iva' + itemListCount + '" value="0.00" type="hidden"></input>' +
                        '</div>' +

                        '<div class="col-md-2 col-sm-6 col-xs-12" data-title="Desc" class="numeric" style="padding-bottom:5px">' +
                            '<input id="dsc' + itemListCount + '"  class="form-control" onchange="actualizarTotales()" onblur="selectAddButton()" style="text-align:right; background:none; " type="number" value="0.00" min="0.00"  max="100.00" step="1.00" placeholder="% Descuento" />' +
                        '</div>'+
                    '</div>' +
                '</div>' +

                '<div class="col-md-2 col-sm-4 col-xs-12" style="padding-bottom:5px">' +
                    '<div class="row">' +
                        '<div class="col-md-12 col-sm-12 col-xs-12 numeric center" data-title="Total">' +
                            '<h4><input  class="form-control" id="tot' + itemListCount + '" style="width:100%;text-align:center;border:none;background:none;" type="text" value="0.00" min="0.00" step="0.01" readonly disabled /></h4>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
        '</div>' +
            '</td>' +
            '<td data-title="">' +
        '<button id="trash' + itemListCount + '" title="Eliminar Registro" type="button" onfocus="enfocarLista(this)" onclick="quitarItem(this)" class="tabledit-delete-button btn " style="width:auto;float:none;" ><span class="la la-trash-o "></span></button>' +
            '</td>' +
        '</tr>';


    var result = $("#product-items").append(markup);
    var obj = $("#" + itemListId);
    var iva = $("#ivaitem" + itemListCount);
    var descObj = $("#dsc" + itemListCount);
    
    startSelectProduct(obj);
    actualizarTotales();

    if (typeof initialized !== 'undefined' && initialized)
        if (itemListCount > 0) {
            obj.focus();
            obj.select2("open");
        }
}

function enfocarLista(objparent) {
    if (typeof objparent !== 'undefined') {
        var objName = objparent.parentElement.parentElement.id;
        if (objName) {
            var obj = $("#" + objName + " select");

            if (typeof initialized !== 'undefined' && initialized)
                if (typeof obj !== 'undefined' && obj.length > 0) {
                    obj[0].focus();
                }
        }
    }
}

function quitarItem(obj) {

    if (obj.parentElement.parentElement.parentElement.childElementCount == 1) {
        Swal.fire("Factura", "Se requiere al menos un item en la lista!", "warning");
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
                    q: params.term || "", // search term
                    page: params.page || 1
                };
            },
            error: function (data, a, e, i) {

                if (data !== null && typeof data.responseText == 'string' &&
                    data.responseText.includes("login-form")) {
                    Swal.fire("Su Sesion ha Caducado", "Por favor inicie sesion", "error");
                    var w = open(location.href, "_top", "height=770,width=520");
                }
                else {
                    //Swal.fire("Error","Hubo un error al obtener los datos","error");
                }

                debugger;
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
                    Swal.fire("Su Sesion ha Caducado", "Por favor inicie sesion", "error");
                    var w = open(location.href, "_top", "height=770,width=520");
                }
                else {
                    //Swal.fire("Error","Hubo un error al obtener los datos","error");
                }
            },
            cache: true
        },
        allowClear: true,
        placeholder: '999999999999 - CONSUMIDOR FINAL',
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        templateResult: formatContrib,
        templateSelection: formatContribSelection,
        language: {
            inputTooShort: function (args) {
                var remainingChars = args.minimum - args.input.length;

                var message = 'Por favor escriba al menos ' + remainingChars + ' caracteres' +
                    "&nbsp; <button type='button'  data-toggle='modal' data-target='#modal-cliente' onclick='javascript:agregarComprador();' class='btn btn-outline blue'> Agregar <i class='fa fa-plus'></i></button>";

                return message;

            },
            noResults: function () {
                return "Registro no encontrado &nbsp; <button type='button'  data-toggle='modal' data-target='#modal-cliente' onclick='javascript:agregarComprador();' class='btn btn-outline blue'> Agregar <i class='fa fa-plus'></i></button>";
            },
            searching: function () {
                return "Buscando...";
            }
        }
    });

}


$("#guia_remision").on("change", function () {
    var $me = $(this);
    var guia = $me.val();

    $me.val(formatNumDoc(guia));
});


window.scrollTo(0, 0);