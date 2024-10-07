/* eslint-disable */
var referralGuideSaveAction = "GuiaRemision/SaveReferralGuideAsync";
var printReferralGuideAction = "GuiaRemision/PrintDocument";
var documentoGuiaRemision = {};
var ivaList = [];
var productData = [];
var referralGuide = [];
var data_documents = [];

var fecha_mes_atras = new Date(new Date() - (30 * 24 * 60 * 60 * 60 * 15)).toLocaleDateString("es");
var fecha_actual = new Date().toLocaleDateString("es");
 
var total = 0.00; 

var initialized = false;
var calculating = 0;

var itemListCount = -1;
var adicionalItemCount = 0;

//////////////////////////////// ACTIONS ////////////////////////////////
var getUrlContribAction = function (id) { return "/Contribuyentes/Nuevo"; };
var getUrlSearchContrib = function () { return "/GuiaRemision/SearchContribAsync"; };
var getUrlSearchDocument = function () { return "/GuiaRemision/SearchDocumentsAsync"; };


var getLoginFormUrl = function () { return "/Auth"; };
var getUrlAuthorization = function (docTypeCode, docNumber) { return '/GuiaRemision/GetDocumentAuthSync?docType=' + docTypeCode + '&docNumber=' + docNumber; };
var getSearchProductUrl = function () { return "/GuiaRemision/SearchProductAsync"; };  // se requiere implementacion 
var getCreateProductUrl = function () { return "/GuiaRemision/GetProducto"; };  // se requiere implementacion 
 

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
 
        total = 0.00; 

        // PROCESO DE CALCULO DE LOS VALORES POR ITEM
        var itemObj = $(".product-item");

        // Busco dentro de los items de la lista
        for (var i = 0; i < itemObj.length; i++) {
            var itemInput = $(itemObj[i]);
            if (typeof itemInput !== 'undefined' && itemInput.length > 0) {
                var qtyObject = itemInput.find(".item-quantity")[0];
                var cantidad = getValueSafe(qtyObject.value);
                if (typeof cantidad != 'undefined' && cantidad > 0) {
                    total = total + cantidad;
                }
            }
        }
         
        var objtotal = $("#documento_valor_total"); 

        // Actualizamos los detalles
         
        objtotal.val(total); 

        // flag para evitar sobrecalculos en procesos
        calculating = 0;
    }

}


function generateDocument(emitirDocumento) {

    var IdDoc = $('#Id').val();

    var fecha_Documento = $('#fecha_documento').val();

    var objcontributor = $("#ref_contribuyente_id");

    var contributor_id = objcontributor.val();
      
    var razonSocial = $("#ref_nombre_contribuyente").val();
    var identificacion = $("#ref_identificacion_contribuyente").val();
    var tipoIdentificacion = $("#ref_tipoidentificacion_contribuyente").val();

    var phone = $("#ref_phone").val();
    var address = $("#ref_address").val();
    var emailaddress = $("#ref_email").val();

    var placa = $("#transporte_placa").val();
    var ruta = $("#transporte_ruta").val();
    var dau = $("#numero_dau").val();


    var fechaenvio = $("#fecha_envio").val();
    var fechallegada = $("#fecha_llegada").val();
    var direccionOrigen = $("#direccion_envio").val();
    var direccionLlegada = $("#direccion_llegada").val();
     
    var establecimientoDestino = $("#establecimiento_destino").val();

    var objReason = $("#motivo");
    var documentReason = objReason.val();

    if (documentReason == null || documentReason == '') {
        return valError("El motivo o explicacion de la Guia de Remision esta vacio!", objReason);
    }

    var driverId = $("#driver_id").val();

    if (driverId == null || driverId == "") {
        return valError("Por favor seleccione un tranportista!");
    }

    var destinatarioId = $("#recipient_id").val();

    if (destinatarioId == null || destinatarioId == "") {
        return valError("Por favor seleccione un destinatario!");
    }

    var refDocumentId = $("#ref_comprobante_id").val();  //$("#ref_documento_id").val();

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
        return valError("Por favor ingrese el numero de autorizacion del comprobante sustento de la Guia de Remision!");
    }
     

    var model = {
        Id : IdDoc,
        //ContributorId: contributor_id,
        IdentificationType: tipoIdentificacion,
        Identification: identificacion,
        ContributorName: razonSocial,
        Phone: phone,
        Address: address,
        EmailAddresses: emailaddress,


        DriverId: driverId,
        //DriverIdentificationType: valor,
        //DriverIdentification: valor,
        //DriverName: valor,

        CarPlate: placa,

        ShippingStartDate: fechaenvio,
        ShippingEndDate: fechallegada,
        OriginAddress: direccionOrigen,
        DestinationAddress: direccionLlegada,

        RecipientId: destinatarioId,
        //RecipientIdentificationType: valor,
        //RecipientIdentification: valor,
        //RecipientName: valor,
        RecipientEstablishment: establecimientoDestino,
        DAU: dau,
        ShipmentRoute: ruta,
         
        Currency: "DOLAR",
        IssuedOn: fecha_Documento,
       
        Total: total,
        Details: [],
        AdditionalFields: [],

        Reason: documentReason,

        ReferenceDocumentId: refDocumentId,
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

    if (total <= 0) {
        return valError("El total de items transportados debe ser mayor a cero!");
    }


    // Busco dentro de los items de la lista
    var itemObj = $(".product-item");
    for (var i = 0; i < itemObj.length; i++) {
        var itemobj = $(itemObj[i]).find("select");
        var itemId = itemobj.val();

        if (itemId == null || itemId == 0) {
            return valError("Debe especificar el producto o servicio!", itemobj);
        }

        var inputs = $(itemObj[i]);
        var qtyObject = inputs.find(".item-quantity")[0];
        var qty = getValueSafe(qtyObject.value);
        var det1 = inputs.find(".item-name1")[0].value;
        var val1 = inputs.find(".item-value1")[0].value;
        var det2 = inputs.find(".item-name2")[0].value;
        var val2 = inputs.find(".item-value2")[0].value;
        var det3 = inputs.find(".item-name3")[0].value;
        var val3 = inputs.find(".item-value3")[0].value;
        if (qty == 0.00000) {
            return valError("El valor para el campo Cantidad no puede ser 0.00 ", qtyObject);
        } 

        model.Details.push({
            "ProductId": itemId,
            "Quantity": qty,
            "Name1": det1,
            "Value1": val1,
            "Name2": det2,
            "Value2": val2,
            "Name3": det3,
            "Value3": val3

        });

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

            model.AdditionalFields.push(obj);
            line++;
        }
    }


    return model;
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

// Muestra el mensaje de validacion de la GuiaRemision
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

function guardarGuiaRemision(emitirDocumento) {
    if (guardandoDocumento) {
        return;
    }

    guardandoDocumento = true;

    // Hack para volver la pantalla al inicio de la pagina
    // y los mensajes se ubiquen en su lugar adecuado.
    window.scrollTo(0, 0);


    showLoader("<label><H1 >Guardando Registro... Por favor Espere.</H1><label>");

    var GuiaRemisionModel = generateDocument(emitirDocumento);

    if (GuiaRemisionModel == null) {       
        return;
    }

    $.ajax({       
        url: referralGuideSaveAction,
        type: "POST",
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            model: GuiaRemisionModel
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
                    Swal.fire("Guia de Remision", msg, "error");
                }

           

        },
        success: function (data, usr, obj, etc) {
            guardandoDocumento = false;

            if (data.id > 0) // Si el ID del Registro es mayor que cero, es un registro real
            {
                documentoGuiaRemision = data.result;

                Swal.fire("Guia de Remision", "El registro se guardo con Exito!", "success");

                // Si los datos fueron guardados correctamente entonces realizo el proceso de bloqueo de todos los controles
                $('.GuiaRemision')
                    .find('input, textarea, button, select')
                    .attr('disabled', 'disabled', 'div');

                //$("#numero_documento")[0].innerText = (documentoGuiaRemision.DocumentNumber);

                //// ejecuta el proceso de carga de los datos de autorizacion

                //$("#numero_documento").focus();

                if (typeof redirectAction !== 'undefined') {
                    redirectAction('Ver', documentoGuiaRemision.Id);
                }
            }
            else {
                if (data.error) {
                    if (data.error.UserMessage) {
                        Swal.fire("Guia de Remision", data.error.UserMessage, "error");
                    }
                    else if (data.message) {
                        Swal.fire("Guia de Remision", data.message, "error");
                    }
                    else {
                        Swal.fire("Guia de Remision", data.error.Message, "error");
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

    var cliente_id = $("#inputCustomerId").val();

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

    if (typeof repo.data !== 'undefined') {
         documentListChanged(repo);
    }

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


        //var markup = "<div class='select2-result-repository clearfix'>" +
        //    "<div class='select2-result-repository__meta'>" +
        //    "<div class='select2-result-repository__title'>" + repo.text + "</div>" +
        //    "<div class='select2-result-repository__statistics'>" +
        //    "<div class='select2-result-repository__description'><i class='fa fa-area-chart'></i> Nombre Comercial: " + data.TradeName + "</div>" +
        //    "<div class='select2-result-repository__address'><i class='fa fa-map-marker'></i> Direccion: " + data.Address + " </div>" +
        //    "<div class='select2-result-repository__email'><i class='fa fa-envelope-o'></i> E-mail:" + data.EmailAddresses + " </div>" +
        //    "<div class='select2-result-repository__phone'><i class='fa fa-phone'></i> Telefono:" + data.Phone + "</div>" +
        //    "</div>" +
        //    "</div></div>";


        //$("#bodyContrib").css("height", "200px");
        var markup = repo.text;


        $("#cliente_tipo_identificacion").val("02");
        $("#actualizarClienteButton").removeClass("hidden");
        /// Cargo la informacion en los elementos
        $("#headDocument").removeClass("hidden");
        $("#headDocument").fadeIn();

        agregarAditional("Direccion", data.Address);
        agregarAditional("Email", data.EmailAddresses);
        agregarAditional("Telefono", data.Phone);

        startDocumentSelector(repo.id);

        return markup;
    }
    else {
        $("#bodyContrib").css("height", "");
        $("#actualizarClienteButton").addClass("hidden");
        $("#headDocument").addClass("hidden");
        $("#headDocument").fadeOut();

        if (typeof agregarAditional !== 'undefined') {
            agregarAditional("Direccion", "");
            agregarAditional("Email", "");
            agregarAditional("Telefono", "");
        }
    }

    return repo.text;

}

 function configure_details() {

    $(".item-quantity").on("change", actualizarTotales);
   //$(".delete-item").on("click", delete_item);
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
        '<tr id="additional' + adCount + '" class="additional-item kt-datatable__row col-12" style="margin-bottom:5px">' +
        '<td class="col-8">' + '<div clas="col-12">' + '<div class="row">' +
        '<div data-title="Detalle" class="col-lg-6 col-md-6 col-12 bold" style="padding-bottom:1px">' +
        '<input class="form-control bold"  id="adname' + adCount + '"  style="text-align:left; width:100%;" type="text" placeholder="Nombre" list="list-references" value="' + adname + '" />' +
        '</div>' +
        '<div data-title="Valor" class="col-lg-6 col-md-6 col-12">' +
        '<input class="form-control" style="text-align:left; " id="addet' + adCount + '" type="text" placeholder="Detalle" value="' + advalue + '" />' +
        '</div>' +
        '</div>' + '</div>' +
        '</td>' +
        '<td data-title="" class="col-4"><button id="trash' + adCount + '"  type="button" onclick="quitarAdicional(this)" class="tabledit-delete-button btn btn-sm btn-outline-hover-danger btn-pill" style="float: none;width:auto; "><span class="la la-trash-o"></span></button></td>' +
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

        var $parentTR = $(repo.element).parents("tr");      
        $parentTR.find(".item-name1").val(repo.data.Name1);
        $parentTR.find(".item-value1").val(repo.data.Value1);
        $parentTR.find(".item-name2").val(repo.data.Name2);
        $parentTR.find(".item-value2").val(repo.data.Value2);
        $parentTR.find(".item-name3").val(repo.data.Name3);
        $parentTR.find(".item-value3").val(repo.data.Value3);
         
        actualizarTotales();


        if (typeof initialized !== 'undefined' && initialized) {
            // trItem[0].focus();
        }
    }

    return repo.text;

}

function agregarItem(detalleItem) {
    itemListCount++;

    var itemListId = "selectProducto" + itemListCount;
  

    var markup =

        '<tr id="item' + itemListCount + '" class="product-item">' +

        '<td>' + '<div class="row">' +
        '<div class="col-7" data-title="Product" style="width:100%">' +
        '<select id="' + itemListId + '" class="form-control" data-parent="item' + itemListCount + '" style="width:100%;font-size:11px;background:none;border:none;" class="js-productSelector" id="inputBuscarProducto"></select>' +
        '</div>' +     

        '<div class="col-3" data-title="Product">' +
        '<input id="qty' + itemListCount + '" onchange="actualizarTotales()" class="form-control item-quantity" style="width:100%;text-align:right; background:none;" type="number" value="1" min="1" placeholder="Cantidad" />' +
        '</div>' +       

        '<div class="col-2 col-md-1" data-title="">' +
        '<button id="trash' + itemListCount + '" type="button" onfocus="enfocarLista(this)" onclick="quitarItem(this)" class="tabledit-delete-button btn btn-sm btn-outline-hover-danger btn-pill" style="float: none;"><span class="la la-trash-o"></span></button>' +
        '</div>' +


        '<div class="col col-md-10 col-sm-8 col-xs-12">' +
        '<div class="accordion accordion-solid accordion-toggle-plus" id="accordAdditionalsItem' + itemListCount + '">' +
        '<div class="card">' +
        '<div class="card-header" id="itemHeading' + itemListCount + '">' +
        '<div class="card-subtitle collapsed" data-toggle="collapse" data-target="#itemsCollapse' + itemListCount + '" aria-expanded="false" aria-controls="itemsCollapse' + itemListCount + '">' +
        '<br /><i class="flaticon2-add"></i> Detalles Adicionales' +
        '</div>' +
        '</div>' +
        '<div id="itemsCollapse' + itemListCount + '" class="collapse" aria-labelledby="itemHeading' + itemListCount + '" data-parent="#accordAdditionalsItem' + itemListCount + '" style="">' +
        '<div class="card-body">' +
        '<table class="table table-payment col-md-12 table-hover table-striped table-condensed kt-datatable__table">' +
        '<tbody id = "additionals-items' + itemListCount + '" class="kt-datatable__body">' +
        '<tr><td> <div class="row">' +

        '<div class="col-md-4 col-sm-12 col-xs-12 input-group" data-title="Detalle1" class="numeric" style="padding-bottom:5px;width:100%"> ' +
        '<input id="detname1_' + itemListCount + '" name="Details[' + itemListCount + '].Name1"  class="form-control item-name1 bold" style="text-align:left;background:none;font-size:10px;" placeholder="Nombre1" />' +
        '<input id="detval1_' + itemListCount + '" name="Details[' + itemListCount + '].Value1"  class="form-control item-value1 bold" style="text-align:left;background:none;font-size:10px;" placeholder="Valor1" />' +
        '</div>' +

        '<div class="col-md-4 col-sm-12 col-xs-12 input-group" data-title="Detalle1" class="numeric" style="padding-bottom:5px;width:100%"> ' +
        '<input id="detname2_' + itemListCount + '" name="Details[' + itemListCount + '].Name2"  class="form-control item-name2 bold" style="text-align:left;background:none;font-size:10px;" placeholder="Nombre2" />' +
        '<input id="detval2_' + itemListCount + '" name="Details[' + itemListCount + '].Value2"  class="form-control item-value2 bold" style="text-align:left;background:none;font-size:10px;" placeholder="Valor2" />' +
        '</div>' +

        '<div class="col-md-4 col-sm-12 col-xs-12 input-group" data-title="Detalle1" class="numeric" style="padding-bottom:5px;width:100%"> ' +
        '<input id="detname3_' + itemListCount + '" name="Details[' + itemListCount + '].Name3"  class="form-control item-name3 bold" style="text-align:left;background:none;font-size:10px;" placeholder="Nombre3" />' +
        '<input id="detval3_' + itemListCount + '" name="Details[' + itemListCount + '].Value3"  class="form-control item-value3 bold" style="text-align:left;background:none;font-size:10px;" placeholder="Valor3" />' +

        '</div>' +
        '</div>' +
        '</td>' +

        '</tbody>' +
        '</table>' +

        '</div>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '</div>' +


        '</div>' +
        '</td>' +
        '</tr>';

    var result = $("#product-items").append(markup);
    var obj = $("#" + itemListId);
    var iva = $("#ivaitem" + itemListCount);


    startSelectProduct(obj);


    // Se actualiza la informacion del detalle
    if (detalleItem) {

        // Primero cargamos los documentos
        $.ajax({
            type: 'GET',
            url: getSearchProductUrl() + "?search=" + detalleItem.MainCode
        }).then(function (data) {
            if (data && data !== null && data.length > 0) {
                var dataItem = data[0];


                // create the option and append to Select2
                var option = new Option(dataItem.text, dataItem.id, true, true);
                obj.append(option).trigger('change');

                obj.val(dataItem.id);

                // manually trigger the `select2:select` event
                obj.trigger(
                    {
                        type: 'select2:select',
                        params: { data: data }
                    });

                obj.select2();
            }
        });

        $('#qty' + itemListCount).val(detalleItem.Amount);



        actualizarTotales();

        return;
    }

    actualizarTotales();

    if (typeof initialized !== 'undefined' && initialized)
        if (itemListCount > 0) {
            obj.focus();
            obj.select2("open");
        }
}

function enfocarLista(objparent) {
    if (typeof objparent !== 'undefined') {
        var objName = $(objparent).parents("tr").attr("id");

        var obj = $("#" + objName + " select");

        if (typeof initialized !== 'undefined' && initialized)
            if (typeof obj !== 'undefined' && obj.length > 0) {
                obj[0].focus();
            }
    }
}

function quitarItem(obj) {

    if (obj.parentElement.parentElement.parentElement.childElementCount == 1) {
        Swal.fire("Guia de Remision", "Se requiere al menos un item en la lista!", "warning");
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
                    var w = open(getActionUrl("Auth", "Index"), "_top", "height=770,width=520");
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
                    "&nbsp; <button type='button' onclick='javascript:crearProducto(this);' class='btn btn-outline-brand'> Agregar <i class='fa fa-plus'></i></button>";

                return message;

            },
            noResults: function () {
                return "Registro no encontrado &nbsp; <button type='button' onclick='javascript:crearProducto(this);' class='btn btn-outline-brand'> Agregar <i class='fa fa-plus'></i></button>";
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
                    var w = open(getActionUrl("Auth", "Index"), "_top", "height=770,width=520");
                }
                else {
                    //Swal.fire("Error","Hubo un error al obtener los datos","error");
                }
            },
            cache: true
        }, 
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        templateResult: formatContrib,
        templateSelection: formatContribSelection,
        language: {
            inputTooShort: function (args) {
                var remainingChars = args.minimum - args.input.length;

                var message = 'Por favor escriba al menos ' + remainingChars + ' caracteres' +
                    "&nbsp; <button type='button'  data-toggle='modal' data-target='#modal-cliente' onclick='javascript:agregarComprador();' class='btn btn-outline-brand'> Agregar <i class='fa fa-plus'></i></button>";

                return message;

            },
            noResults: function () {
                return "Registro no encontrado &nbsp; <button type='button'  data-toggle='modal' data-target='#modal-cliente' onclick='javascript:agregarComprador();' class='btn btn-outline-brand'> Agregar Persona <i class='fa fa-plus'></i></button>";
            },
            searching: function () {
                return "Buscando...";
            }
        }
    });

}


function startDocumentSelector() {

         var url = getUrlSearchDocument();
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
                async: true,
                delay: 1000,
                //data: function (params) {
                //    return {
                //        search: params.term, // search term
                //        page: params.page
                //    };
                //},
                //processResults: function (data, params) {
                //    // parse the results into the format expected by Select2
                //    // since we are using custom formatting functions we do not need to
                //    // alter the remote JSON data, except to indicate that infinite
                //    // scrolling can be used
                //    params.page = params.page || 1;

                //    return {
                //        results: data,
                //        pagination: {
                //            more: true
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
        // Convertir dato obj a fecha:
        var docDateString = doc.IssuedOn.replace("/Date(", "").replace(")/", "");
        var docDate = new Date(parseInt(docDateString)).toLocaleDateString("es");

        //$("#fecha_documento").val(docDate);

        $("#ref_contribuyente_id").val(doc.ContributorId);
        $("#ref_fecha_documento").val(docDate);
        $("#ref_tipoidentificacion_contribuyente").val(doc.ContributorIdentificationType);
        $("#ref_identificacion_contribuyente").val(doc.ContributorIdentification);

        $("#ref_comprobante_id").val(doc.Id);     
        $("#ref_numero_comprobante").val(doc.DocumentNumber);
        $("#ref_nombre_contribuyente").val(doc.ContributorName);
        $("#ref_tipo_comprobante").val(doc.DocumentTypeCode);
        $("#ref_autorizacion_comprobante").val(doc.AuthorizationNumber);

        $("#ref_phone").val(doc.Phone);
        $("#ref_email").val(doc.EmailAddresses);
        $("#ref_address").val(doc.Address);

        agregarAditional("Telefono", doc.Phone);
        agregarAditional("Email", doc.EmailAddresses);
        agregarAditional("Direccion", doc.Address);
        
        if (docDate) $("#ref_fecha_documento").attr('disabled', 'disabled');
        if (doc.ContributorIdentificationType) $("#ref_tipoidentificacion_contribuyente").attr('disabled', 'disabled');
        if (doc.ContributorIdentification) $("#ref_identificacion_contribuyente").attr('readonly', 'readonly');
        if (doc.ContributorName) $("#ref_nombre_contribuyente").attr('readonly', 'readonly');
        if (doc.DocumentTypeCode) $("#ref_tipo_comprobante").attr('disabled', 'disabled');
        if (doc.DocumentNumber) $("#ref_numero_comprobante").attr('readonly', 'readonly');
        if (doc.AuthorizationNumber)$("#ref_autorizacion_comprobante").attr('readonly', 'readonly');
        
        $("#product-items tbody").empty();
        for (var i = 0; i < doc.InvoiceInfo.Details.length; i++) {
            var item = doc.InvoiceInfo.Details[i];
            agregarItem(item);
        }
        
        actualizarTotales();
    }
    else {

        $("#fecha_documento").removeAttr('readonly', '');
        $("#ref_contribuyente_id").attr('readonly', '');
        $("#ref_tipoidentificacion_contribuyente").attr('readonly', '');
        $("#ref_identificacion_contribuyente").attr('readonly', '');
        $("#ref_nombre_contribuyente").attr('readonly', '');
        $("#ref_comprobante_id").attr('readonly', '');
        $("#ref_tipo_comprobante").attr('readonly', '');
        $("#ref_numero_comprobante").attr('readonly', '');
        $("#ref_fecha_documento").attr('readonly', '');
        $("#ref_autorizacion_comprobante").attr('readonly', '');
    }
     
}

 
function dauNumberChanged() {
    var numDocObj = $("#numero_dau");
    var text = numDocObj.val();

    numDocObj.val(formatNumDAU(text));
}


function docNumberChanged() {
    var numDocObj = $("#ref_numero_comprobante");
    var text = numDocObj.val();

    numDocObj.val(formatNumDoc(text));

}
 

function carplateChanged() {
    var numDocObj = $("#transporte_placa");
    var text = numDocObj.val();

    numDocObj.val(formatPlate(text));

}


dauNumberChanged();
window.scrollTo(0, 0);