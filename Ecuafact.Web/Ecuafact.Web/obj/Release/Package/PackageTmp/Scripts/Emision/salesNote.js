var SalesNote = function () {

    var initialized = false,
        calculating = 0,
        saving = false,
        itemsCount = 0,
        paymentCount = 0,
        fecha_mes_atras = new Date(new Date() - (30 * 24 * 60 * 60 * 60 * 15)).toLocaleDateString("es"),
        fecha_actual = new Date().toLocaleDateString("es"),
        changing = false,

        main = function () {

            itemsCount = $("tr.product-item").length;
            paymentCount = $("tr.payment-item").length;

            $(".saleNote-form").on("submit", function (e) {
                e.preventDefault();
                save_handler();
            });

            $(".add-detail").on("click", add_detail);
            $(".add-payment").on("click", add_payment);
            $(".btn-saleNote-save").on("click", save_handler);

            $("[name='Term']").on("change", refresh_totals);
            $("[name='TimeUnit']").on("change", refresh_totals);
            $("[name='ReferralGuide']").on("change", set_referralguide)

            $(".establishment-Code").on("change", handleLeadingZeros);
            $(".issuePointCode-Code").on("change", handleLeadingZeros);
            $(".sequential-number").on("change", handleSequential);
            $(".inputfile-pdf-doc").on("change", handlerFile);
            $(".btn-select-pdf").on("click", handlerSelectFile);

            if (itemsCount == 0) {
                add_detail();
            }
            else {
                configure_details();
            }

            if (paymentCount == 0) {
                add_payment();
            }
            else {
                configure_payments();
            }

            configure_contrib();
            refresh_totals();
        },
        handleLeadingZeros = function () {
            if (changing) {
                return;
            }

            changing = true;
            var $me = $(this);

            var value = $me.val();
            if (value.length > 0 && value.length < 3) {
                value = value.padStart(3, "0");
                $me.val(value);
            }
            changing = false;
        },
        handleSequential = function () {
            if (changing) {
                return;
            }

            changing = true;
            var $me = $(this);

            var value = $me.val();
            if (value.length > 0 && value.length < 9) {
                value = value.padStart(9, "0");
                $me.val(value);
            }
            changing = false;
        },
        configure_payments = function () {
            $(".payment-total").on("change", change_payment);
        },
        set_referralguide = function () {
            var $obj = $(this);
            var text = formatNumDoc($obj.val());

            if (text != '' && text.length < 17) {
                toastr.error("El número de Guía de Remisión es Inválido.");
                $obj.focus();
                $obj.blur();
            }
            else {
                $obj.val(text);
            }
        },
        configure_details = function () {
            //configure_product();

            $(".item-quantity").on("change", refresh_totals);
            $(".item-unitprice").on("change", refresh_totals);
            $(".delete-item").on("click", delete_item);
        },
        handlerSelectFile = function () {
            $(".inputfile-pdf-doc").click();
        },
        add_payment = function () {
            paymentCount++;

            var paymentItemCount = paymentCount;
            var itemListId = "selectPayment" + paymentItemCount;

            var paymentHtml = '';

            var $payment = SalesNote.paymentTypes;
            if ($payment) {
                for (var i = 0; i < $payment.length; i++) {
                    paymentHtml += "<option value='" + $payment[i].SriCode + "'>" + $payment[i].Name + "</option>";
                }
            }

            var $term = $("[name='Term']").val();
            var $unit = $("[name='TimeUnit']").val();

            var markup =
                '<tr id="payment' + paymentItemCount + '" class="payment-item">' +
                ' <td data-title="Detalle">' +
                '  <div class="row">' +
                '   <input name="Payments.Index" value="' + paymentItemCount + '" type="hidden" />' +
                '   <input name="Payments[' + paymentItemCount + '].TimeUnit" class="payment-timeunit" value="' + $unit + '" type="hidden" />' +
                '   <input name="Payments[' + paymentItemCount + '].Term" class="payment-term" value="' + $term + '" type="hidden" />' +
                '   <div data-title="FormaPago" class="col-lg-8 col-md-12 col-sm-8 col-xs-12 bold" style="padding-bottom:10px">' +
                '    <select id="' + itemListId + '" name="Payments[' + paymentItemCount + '].PaymentMethodCode" class="form-control" style="width:100%;">' + paymentHtml + '</select>' +
                '   </div>' +
                '   <div data-title="Valor" class="col-lg-4 col-md-12 col-sm-4 col-xs-12 numeric" style="text-align:right;">' +
                '    <input id="pago' + paymentItemCount + '" name="Payments[' + paymentItemCount + '].Total" class="form-control payment-total text-right" type="text" placeholder="Valor Pagado" />' +
                '   </div>' +
                '  </div>' +
                ' </td>' +
                ' <td data-title=""><button id="pay_trash' + paymentItemCount + '" title="Eliminar Registro" type="button" class="delete-item btn" style="width:auto;float:none;"><span class="la la-trash-o"></span></button></td>' +
                '</tr>';

            var result = $("#payment-items").append(markup);
            var obj = $("#" + itemListId);

            $("#pago" + paymentItemCount).on("change", change_payment);
            $("#pay_trash" + paymentItemCount).on("click", delete_item);

            if (typeof initialized !== 'undefined' && initialized) {
                obj.focus();
            }
        },
        change_payment = function () {
            refresh_totals();
        },
        add_detail = function () {

            itemsCount++;
            var itemListId = "selectProducto" + itemsCount;

            var markup =
                '<tr id="item' + itemsCount + '" class="product-item">' +
                '<td data-title="Product" style="width:100%">' +
                '<div class="row">' +

                '<input id="item' + itemsCount + '"  name="Details.Index" value="' + itemsCount + '" type="hidden"></input>' +
                '<input id="mainCode' + itemsCount + '"  name="Details[' + itemsCount + '].MainCode" class="item-maincode"  value="" type="hidden"></input>' +
                '<input id="auxCode' + itemsCount + '"  name="Details[' + itemsCount + '].AuxCode" class="item-auxcode"  value="" type="hidden"></input>' +               

                '<div class="col-md-10 col-sm-8 col-xs-12" >' +
                '<div class="row">' +
                '<div class="col-md-7 col-sm-12" data-title="Detalle" class="numeric" style="padding-bottom:5px">' +
                '<input id="desc' + itemsCount + '"  name="Details[' + itemsCount + '].Description" class="form-control item-description"  value="" type="text"></input>' +
                '</div> ' +

                '<div class="col-md-2 col-sm-6 col-xs-12" data-title="Cantidad" class="numeric" style="padding-bottom:5px"> ' +
                '<input id="qty' + itemsCount + '" name="Details[' + itemsCount + '].Amount"  class="form-control item-quantity" style="text-align:right; background:none;" type="number" value="1" min="1" placeholder="Cantidad" />' +
                '</div>' +

                '<div class="col-md-3 col-sm-6 col-xs-12" data-title="Valor Unitario" class="numeric" style="padding-bottom:5px">' +
                '<input id="price' + itemsCount + '" name="Details[' + itemsCount + '].UnitPrice"  class="form-control item-unitprice" style="text-align:right;background:none;  " type="number" value="0.00" min="0.01" placeholder="Valor Unitario" />' +
                '</div>' +


                '</div>' +
                '</div>' +

                '<div class="col-md-2 col-sm-4 col-xs-12" style="padding-bottom:5px">' +
                '<div class="row">' +
                '<div class="col-md-12 col-sm-12 col-xs-12 numeric text-center" data-title="Total">' +
                ' <h4 style="margin-top: 10px;font-weight:300;">$&nbsp;<span data-field="Details[' + itemsCount + '].SubTotal">0.00</span></h4> ' +
                '<input class="invoice-subtotal item-subtotal" data-val="true" data-val-number="El campo valor Total debe ser un número." data-val-required="The Valor Total field is required." id="tot' + itemsCount + '" name="Details[' + itemsCount + '].SubTotal" type="hidden" value="0.00">' +

                '</div>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</td>' +
                '<td data-title="">' +
                '<button id="trash' + itemsCount + '" title="Eliminar Registro" type="button" class="delete-item btn " style="width:auto;float:none;" ><span class="la la-trash-o "></span></button>' +
                '</td>' +
                '</tr>';

            var x = `<div class="col-md-12 col-sm-12 col-xs-12 numeric text-center" data-title="Total">
                <h4 style="margin-top: 10px;font-weight:300;">$&nbsp;<span data-field="Details[7].SubTotal">3.00</span></h4>
                <input class="invoice-subtotal item-subtotal" data-val="true" data-val-number="El campo Valor Total debe ser un número." data-val-required="The SubTotal field is required." id="Details_7__SubTotal" name="Details[7].SubTotal" type="hidden" value="3.00">
                                                        </div>`

            var result = $(".product-items").append(markup);
            var obj = $("#" + itemListId);

            $("#qty" + itemsCount).on("change", refresh_totals);
            $("#price" + itemsCount).on("change", refresh_totals);
            $("#trash" + itemsCount).on("click", delete_item);
            $("#tot" + itemsCount).totalText();


            //configure_product(obj);
            refresh_totals();

            if (typeof initialized !== 'undefined' && initialized)
                if (itemsCount > 0) {
                    obj.focus();
                    obj.select2("open");
                }
        },
        delete_item = function () {
            $(this).parents("tr").remove();
            refresh_totals();
        },
        refresh_totals = function () {
            if (calculating == 0) {


                // este es un control para evitar multiples llamadas
                // al proceso de calculo por los procesos propios de la funcion
                calculating = 1;
                total = 0.00;
                item_qty = 0.00;
                total_item = 0.00;
                valor_propina = 0.00;
                totalPagos = 0.00;
                saldoPagos = 0.00;

                // PROCESO DE CALCULO DE LOS VALORES POR ITEM
                var itemObj = $(".product-item");

                // Busco dentro de los items de la lista
                for (var i = 0; i < itemObj.length; i++) {
                    var itemInput = $(itemObj[i]);
                    var ivaSelect = itemInput.find(".item-taxcode")[0];

                    if (typeof itemInput !== 'undefined' && itemInput.length > 0) {
                        var qtyObject = itemInput.find(".item-quantity")[0];
                        var prcObject = itemInput.find(".item-unitprice")[0];
                        var totObject = itemInput.find(".item-subtotal");

                        // obtengo la informacion:
                        var qty = getValueSafe(qtyObject.value);
                        if (qty < 0) {
                            qty = 0;
                            show_error("La cantidad del producto no puede ser un valor negativo", qtyObject);
                        }
                        item_qty = getValueSafe(item_qty + qty, 6);
                        var prc = getValueSafe(prcObject.value);
                        if (prc < 0) {
                            prc = 0;
                            show_error("El precio del producto no puede ser menor un valor negativo", qtyObject);
                        }

                        var subtotal = getValueSafe(qty * prc, 6);
                        total_item = getValueSafe(total_item + subtotal, 6);

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

                        subtotal = getValueSafe(subtotal, 2);

                        qtyObject.value = qtyFixed;
                        prcObject.value = priceFixed;

                        totObject.val(subtotal.toFixed(2));
                        totObject.trigger("change");


                    }
                }

                // Total general de la factura
                total = getValueSafe(total_item, 2);

                // Realizo el calculo de Pagos
                var $payment_list = $("input.payment-total");
                if ($payment_list.length > 0) {

                    var $pay_term = $("[name='Term']").val();
                    var $time_unit = $("[name='TimeUnit']").val();

                    var $payment_unit = $("input.payment-timeunit");
                    var $payment_term = $("input.payment-term");

                    // Si solo existe una linea de pagos entonces
                    // debe llenarse con el valor total de la factura
                    if ($payment_list.length == 1) {
                        $payment_term.val($pay_term);
                        $payment_unit.val($time_unit);

                        totalPagos = total;
                        setValueSafe($payment_list.first(), totalPagos);
                    }
                    else {
                        totalPagos = 0.00;
                        // Calculo los pagos realizados
                        for (var p = 0; p < $payment_list.length; p++) {
                            var $payment_item = $payment_list[p];

                            if ($payment_item && typeof $payment_item != 'undefined') {
                                var $payment_value = getValueSafe($payment_item, 2);

                                if ($payment_value < 0) {
                                    show_error("El valor de los pagos no puede ser un valor negativo!");
                                    $payment_value = 0;
                                }

                                totalPagos = totalPagos + $payment_value;
                                $payment_term[p].value = $pay_term;
                                $payment_unit[p].value = $time_unit;
                                setValueSafe($payment_item, $payment_value);
                            }
                        }

                        var primerPago = getValueSafe($payment_list.first(), 2);
                        totalPagos = getValueSafe(totalPagos - primerPago, 2);
                        var primerPago = getValueSafe(total - totalPagos, 2);
                        if (primerPago < 0) {
                            primerPago = 0.00;
                            show_error("El valor de los pagos no puede menor un valor negativo!");
                        }
                        setValueSafe($payment_list.first(), primerPago);
                        totalPagos = getValueSafe(totalPagos + primerPago, 2);
                    }
                }

                // Calculo el saldo de valor por pagos
                saldoPagos = total - totalPagos;

                // Actualizo los totales con los valores actuales               
                var objtotal = $("#Total");
                var objvalorTotal = $("#Total");
                var objsubtotal = $("#Subtotal");
                var objtotalItem = document.getElementById("TotalItem");
                var objitemQuantity = document.getElementById("ItemQuantity");

                // Objetos de calculo de pagos
                var objtotpagos = $("#TotalPayment");
                var objsalpagos = $("#Balance");

                setValueSafe(objsubtotal, total);
                setValueSafe(objtotal, total);
                setValueSafe(objvalorTotal, total);
                objtotalItem.innerHTML = total_item.toFixed(2);
                objitemQuantity.innerHTML = fixUp(item_qty);

                if (totalPagos < 0) {
                    show_error("El valor de los pagos no puede ser valor negativo!");
                    $("[data-field='TotalPayment']").focus();
                    $("[data-field='TotalPayment']").addClass("text-danger h4");
                }
                else {
                    $("[data-field='TotalPayment']").removeClass("text-danger h4");
                }

                if (saldoPagos < 0) {
                    show_error("El valor de los pagos no puede ser mayor al total de la factura!");
                    $("[data-field='Balance']").addClass("text-danger h4");
                }
                else {
                    $("[data-field='Balance']").removeClass("text-danger h4");
                }

                setValueSafe(objtotpagos, totalPagos);
                setValueSafe(objsalpagos, saldoPagos);

                // flag para evitar sobrecalculos en procesos
                calculating = 0;

            }

        },
        // Muestra el mensaje de validacion de la factura
        show_error = function (msg, obj) {
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
        },
        configure_product = function (obj) {
            if (!obj) {
                obj = $(".product-item").find("select[name$='ProductId']");
            }

            obj.select2({
                ajax: {
                    url: SalesNote.productsUrl,
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

                    },
                    cache: true
                },
                allowClear: false,
                placeholder: 'Seleccione un producto',
                escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                templateResult: format_product,
                templateSelection: select_product,
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
        },
        format_product = function (repo) {
            if (repo.loading) {
                return repo.text;
            }

            return repo.text;
        },
        select_product = function (repo) {

            // Si existe el objeto de datos se muestra la informacion para agregarlo a la lista
            if (typeof repo.data !== 'undefined') {
                var data = repo.data;

                // Buscamos id del elemento TR
                var $parentTR = $(repo.element).parents("tr");

                $parentTR.find(".item-unitprice").val(repo.data.UnitPrice);
                $parentTR.find(".item-maincode").val(repo.data.MainCode);
                $parentTR.find(".item-auxcode").val(repo.data.AuxCode);
                $parentTR.find(".item-description").val(repo.data.Name);

                refresh_totals();

                if (typeof initialized !== 'undefined' && initialized) {
                    trItem[0].focus();
                }
            }

            return repo.text;

        },
        configure_contrib = function () {
            var searchContributorUrl = SalesNote.contributorsUrl + "/SearchContribAsync";

            $("#IssuingId").select2({
                ajax: {
                    url: searchContributorUrl,
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
                            var w = open(SalesNote.loginUrl, "_top", "height=770,width=520");
                        }
                        else {
                            //Swal.fire("Error","Hubo un error al obtener los datos","error");
                        }
                    },
                    cache: true
                },
                allowClear: true,
                placeholder: 'Seleccione un emisor',
                escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                templateResult: format_contrib,
                templateSelection: select_contrib,
                language: {
                    inputTooShort: function (args) {
                        var remainingChars = args.minimum - args.input.length;

                        var message = 'Por favor escriba al menos ' + remainingChars + ' caracteres' +
                            "&nbsp; <button type='button'  data-toggle='modal' data-target='#modal-cliente' onclick='javascript:crearContribuyente();' class='btn btn-outline blue'> Agregar <i class='fa fa-plus'></i></button>";

                        return message;

                    },
                    noResults: function () {
                        return "Registro no encontrado &nbsp; <button type='button'  data-toggle='modal' data-target='#modal-cliente' onclick='javascript:crearContribuyente();' class='btn btn-outline blue'> Agregar <i class='fa fa-plus'></i></button>";
                    },
                    searching: function () {
                        return "Buscando...";
                    }
                }
            });

        },
        format_contrib = function (repo) {
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
                return markup;
            }

            return repo.text;
        },
        select_contrib = function (repo) {

            // Si existe el objeto de datos se muestra la informacion
            if (typeof repo.data !== 'undefined') {
                var data = repo.data;

                if ($("#IssuingRuc").val() != data.Identification) {
                    $("#IssuingRuc").val(data.Identification);
                    $("#IssuingBussinesName").val(data.BussinesName || data.TradeName);
                    $("#IssuingAddress").val(data.Address);
                    $("#IssuingPhone").val(data.Phone);
                    $("#IssuingEmailAddresses").val(data.EmailAddresses);
                }

                return repo.text;
            }

            return repo.text;

        },
        valError = function (msg, obj) {
            hideLoader();

            Swal.fire("Validacion de Datos", msg, "warning");

            if (typeof obj !== 'undefined') {
                if (obj !== null && typeof obj.focus !== 'undefined')
                    obj.focus();

                if (obj !== null && typeof obj.blur !== 'undefined')
                    obj.blur();
            }

            return false;
        },
        ValidarDocument = function () {

            var objestablishment = $(".establishment-Code");
            var _establishment = objestablishment.val();
            if (_establishment == null || _establishment == "") {
                return valError("Por favor digité el codigo del establecimiento!", objestablishment);
            }

            var objissuePoint = $(".issuePointCode-Code");
            var _issuePoint = objissuePoint.val();
            if (_issuePoint == null || _issuePoint == "") {
                return valError("Por favor digité el codigo del punto de emisión!", objestablishment);
            }

            var objsequential = $(".sequential-number");
            var _sequential = objsequential.val();
            if (_sequential == null || _sequential == "") {
                return valError("Por favor digité el secuencial", objestablishment);
            }

            var objauthorization = $(".authorization-Number");
            var _authorization = objauthorization.val();
            if (_authorization == null || _authorization == "") {
                return valError("Por favor digité el número de autorización del SRI", objestablishment);
            }
            else if (_authorization.length > 10) {
                return valError("El número de autorización del SRI es de 10 digitos");
            }

            var objIssuingId = $("#IssuingId");
            var _issuingId = objIssuingId.val();
            if (_issuingId == null || _issuingId == "") {
                return valError("Por favor seleccione un emisor!", objIssuingId);
            }

            // Busco dentro de los items de la lista
            var itemObj = $(".product-item");
            for (var i = 0; i < itemObj.length; i++) {
                var itemInput = $(itemObj[i]);
                var prodDescObject = itemInput.find(".item-description")[0];
                var porducto = prodDescObject.value;
                if (porducto == null || porducto == "") {
                    return valError("La descripción del producto es obligatorio!", prodDescObject);
                }
                var qtyObject = itemInput.find(".item-quantity")[0];
                var cant = getValueSafe(qtyObject.value);
                if (cant <= 0.00000) {
                    return valError("El valor no puede ser menor que 0", qtyObject);
                }
                var prcObject = itemInput.find(".item-unitprice")[0];
                var vlr = getValueSafe(prcObject.value);
                if (vlr <= 0.00000) {
                    return valError("El valor no puede ser 0.00 ", prcObject);
                }
            }

            return true;
        },
        handlerFile = function () {
            $(".file-types").val("");
            var ext = '';           
            if ($(this)[0].files.length > 0) {               
                var styleOK = "fa fa-check text-success kt-icon-lg";                             
                var labelElement = $("#pdf_doc_file");
                var iSize = ($(this)[0].files[0].size / 1024);
                var namefile = $(this)[0].files[0].name;
                ext = namefile.split('.');
                ext = ext[ext.length - 1];
                var info = "Archivo: " + namefile + "</br>" + "Tamaño de archivo:";
                if (ext === "pdf" || ext === "png" || ext === "jpeg" || ext === "jpg" || ext === "gif") {
                    $(".file-types").val(ext);                   
                    if (iSize / 1024 > 1) {
                        iSize = (Math.round((iSize / 1024) * 100) / 100);
                        if (iSize > 5) {
                            $(this).value = '';
                            this.files[0].name = '';
                            Swal.fire("Oops!", 'El tamaño del archivo no debe ser mayor a  ' + iSize + 'Mb', "error");                            
                            return false;                            
                        } else {
                            labelElement.html(info + ' ' + iSize + "Mb");
                            labelElement.attr('style', 'color: #9c9c9c');
                            
                        }

                    } else {
                        iSize = (Math.round(iSize * 100) / 100)
                        labelElement.html(info + ' ' + iSize + "kb");
                        labelElement.attr('style', 'color: #9c9c9c');
                    }
                }
                else {
                    $(this).value = '';
                    this.files[0].name = '';
                    Swal.fire("Oops!", 'El archivo debe ser extension pdf', "error");                    
                    return false;
                }
            }
        },
        save_handler = function () {
            if (saving) {
                return;
            }
            var $form = $(".saleNote-form");
            if (!ValidarDocument()) {
                return;
            }
            showLoader();
            saving = true;
            var $action = $form.attr("action");
            //var $data = $form.serializeArray();
            var $data = new FormData($form[0]);            
                
            $.ajax({
                url: $action,
                type: "post",
                dataType: "html",
                data: $data,
                cache: false,
                contentType: false,
                processData: false,
                error: function (e, d, t) {
                    toastr.error("Problemas al procesar la petición " + e.statusText);
                },
                success: function (result) {
                    if (result) {
                        result = JSON.parse(result);
                        if (result.id > 0) {
                            toastr.success(result.statusText);
                            location.assign(result.url);
                        }
                        else if (result.id == -999) {
                            toastr.warning(result.error.UserMessage);
                            location.assign(result.url);
                        }
                        else if (result.id == -1) {
                            show_error(result.error.DevMessage);
                        }
                        else {
                            if (result.error) {
                                toastr.warning(result.error.DevMessage || "", result.error.UserMessage || result.statusText + " " + result.error.Message);
                            }
                            else {
                                toastr.warning(result.statusText);
                            }
                        }
                    }
                    else {
                        toastr.warning("Hubo un error al guardar el documento");
                    }
                }
            }).fail(function (error, data, obj) {                
                toastr.warning("Hubo un error al guardar el documento");
            }).always(function () {
                saving = false;
                hideLoader();
            });
        };

    return {
        productsUrl: "/Productos",
        contributorsUrl: "/Contribuyentes",
        loginUrl: "/Auth",
        paymentTypes: [],
        establishments: [],
        init: function () {
            main();
        }
    }
}();