var DebitNote = function () {

    var initialized = false,
        calculating = 0,
        saving = false,
        itemsCount = 0,
        additionalCount = 0,
        paymentCount = 0,
        fecha_mes_atras = new Date(new Date() - (30 * 24 * 60 * 60 * 60 * 15)).toLocaleDateString("es"),
        fecha_actual = new Date().toLocaleDateString("es"),
        fechaRefObj,

        main = function () {

            fechaRefObj = $("#ref_fecha_documento");
            fechaRefObj.datepicker({
                endDate: fecha_actual,
                keyboardNavigation: false,
                format: "dd/mm/yyyy",
                language: "es",
                orientation: "bottom left",
                forceParse: false,
                autoclose: true
            });

            fechaRefObj.val(fecha_actual);


            itemsCount = $("tr.product-item").length;
            additionalCount = $("tr.additional-item").length;
            paymentCount = $("tr.payment-item").length;

            $(".invoice-control").on("change", refresh_totals);
            $(".invoice-form").on("submit", function (e) {
                e.preventDefault();
                save_handler();
            });

            $(".add-detail").on("click", add_detail);
            $(".add-payment").on("click", add_payment);
            $(".add-additional").on("click", add_additional);
            $(".btn-invoice-save").on("click", save_handler);
            $(".btn-invoice-issue").on("click", save_handler);

            $(".js-documentSelector").on("change", function () { documentListChanged(); });

            $("[name='Term']").on("change", refresh_totals);
            $("[name='TimeUnit']").on("change", refresh_totals);


            $("#ref_numero_comprobante").on("change", docNumberChanged);
            $("#ref_autorizacion_comprobante").on("change", refautorizacionChanged);

            $(".select-establishment").on("change", set_establishment);

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

            configure_additionals();
            configure_contrib();
            refresh_totals();
            initialized = true;
            startDocumentSelector(null);
        },
        set_establishment = function () {
            var obj = $('.select-establishment')[0];
            if (obj && obj.value.length > 0) {
                // Obtengo la informacion del impuesto
                var est = DebitNote.establishments.find(
                    function (i) { return i.Code == obj.value; }
                );
                var issuepoint = est.IssuePoint;
                var sHtmlRates = "";
                for (var i = 0; i < issuepoint.length; i++) {
                    sHtmlRates += '<option value="' + issuepoint[i].Code + '" style = "text-align:left;width:100%;">' + issuepoint[i].Code + '</option>';
                }
                var listObject = $(".select-issuePointCode");
                listObject.html(sHtmlRates);
                var address = obj[obj.selectedIndex].text.substring(6);
                $("#EstablishmentAddress").val(address);
            }
            else {
                $(".select-issuePointCode").empty();
                $("#EstablishmentAddress").val("");
            }
        },


        configure_additionals = function () {

            // Seteo los predeterminados internos, cuando se cambia 
            // la direccion, telefono o email se replica en los campos adicionales
            $("[name='Address']").on("change", function () {
                exists_additional("DIRECCION", $(this).val(), true);
                //add_additional("DIRECCION", $(this).val(), true);
            });

            $("[name='Phone']").on("change", function () {
                exists_additional("TELEFONO", $(this).val(), true);
                //add_additional("TELEFONO", $(this).val(), true);
            });

            $("[name='EmailAddresses']").on("change", function () {
                exists_additional("EMAIL", $(this).val(), true);
               // add_additional("EMAIL", $(this).val(), true);
            });

        },

        configure_payments = function () {
            $(".payment-total").on("change", change_payment);
        },

        configure_details = function () {

            $(".item-value").on("change", refresh_totals);
            $(".item-taxcode").on("change", refresh_totals);
            $(".delete-item").on("click", delete_item);
        },
        exists_additional = function (adname, advalue, hidden) {
            var add = true;
            if (additionalCount > 0) {
                for (var i = 0; i <= additionalCount; i++) {
                    var $det = $("#adname" + i);
                    if ($det.length > 0) {
                        var $val = $det.parents("tr").find(".additional-value");
                        var $nam = $det.parents("tr").find(".additional-name");
                        if ($nam.length > 0) {
                            if ($nam.val() === adname) {
                                $val.attr('value', advalue);
                                add = false;
                            }
                        }
                    }
                }
            }

            if (add) {
                add_additional(adname, advalue, hidden);
            }
        },

        add_additional = function (adname, advalue, hidden) {
            additionalCount++;

            var adCount = additionalCount;

            if (typeof adname !== 'string') {
                adname = '';
            }

            if (typeof advalue !== 'string' || !advalue) {
                advalue = '';
            }

            var $det = $("#adname" + adCount);

            if ($det.length > 0) {
                var $val = $det.parents("tr").find(".additional-value");
                if ($val.length > 0) {
                    $val.val(advalue);
                    return;
                }
            }

            if (hidden) {
                hidden = "hidden";
            } else {
                hidden = "";
            }

            var markup =
                '<tr id="additional' + adCount + '" class="additional-item" ' + hidden + '>' +
                ' <td>' +
                ' <div class="row">' +
                '  <input name="AdditionalFields.Index" value="' + additionalCount + '" type="hidden"></input>' +
                '  <input name="AdditionalFields[' + additionalCount + '].LineNumber" value="' + additionalCount + '" type="hidden"></input>' +

                '  <div data-title="Información Adicional" class="col-lg-6 col-md-12 col-sm-6 col-xs-12 bold" style="padding-bottom:5px">' +
                '   <input id="adname' + adCount + '" type="text" class="form-control additional-name bold" name="AdditionalFields[' + additionalCount + '].Name" style="text-align:left; width:100%;" placeholder="Nombre" list="list-references" value="' + adname + '" />' +
                '  </div>' +

                '  <div data-title="Valor" class="col-lg-6 col-md-12 col-sm-6 col-xs-12">' +
                '   <input id="addet' + adCount + '" type="text" class="form-control additional-value"  name="AdditionalFields[' + additionalCount + '].Value" style="text-align:left; " placeholder="Detalle" value="' + advalue + '" />' +
                '  </div>' +
                ' </div>' +
                '</td>' +
                '<td data-title=""><button id="add_trash' + adCount + '" title="Eliminar Registro"  type="button" class="tabledit-delete-button btn" style="float: none;width:auto;"><span class="la la-trash-o"></span></button></td>' +
                '</tr>';

            var result = $("#adicional-items").append(markup);
            var obj = $("#adname" + adCount);

            $("#add_trash" + adCount).on("click", delete_item);

            if (typeof initialized !== 'undefined' && initialized) {
                obj.focus();
            }

        },

        add_payment = function () {
            paymentCount++;

            var paymentItemCount = paymentCount;
            var itemListId = "selectPayment" + paymentItemCount;

            var paymentHtml = '';

            var $payment = DebitNote.paymentTypes;
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

            var ivaItemsHtml = '';

            var $taxes = DebitNote.taxes;

            for (var i = 0; i < $taxes.length; i++) {
                ivaItemsHtml += "<option style='text-align:right;' value='" + $taxes[i].SriCode + "'>" + $taxes[i].Name + "</option>";
            }

            var markup =
                '<tr id="item' + itemsCount + '" class="product-item">' +
                '<td data-title="Product" style="width:100%">' +
                '<div class="row">' +
                '<div class="col-md-10 col-sm-8 col-xs-12" >' +

                '<div class="row">' +
                '<input id="item' + itemsCount + '"  name="Details.Index" value="' + itemsCount + '" type="hidden"></input>' +

                '<div class="col-md-8 col-sm-12 col-xs-12" data-title="Reason" class="text" style="padding-bottom:5px">' +
                '<input id="reason' + itemsCount + '" name="Details[' + itemsCount + '].Reason"  class="form-control item-reason" style="text-align:left; background:none;" type="text"  placeholder="Razón de modificación" />' +
                '</div>' +

                '<div class="col-md-2 col-sm-6 col-xs-12" data-title="Value" class="numeric" style="padding-bottom:5px"> ' +
                '<input id="vlr' + itemsCount + '" name="Details[' + itemsCount + '].Value"  class="form-control item-value" style="text-align:right; background:none;" type="number" value="0.00" min="0.01" placeholder="Valor" />' +
                '</div>' +

                '<div class="col-md-2 col-sm-6 col-xs-12" data-title="IVA" class="numeric" style="padding-bottom:5px">' +
                '<select id="ivaitem' + itemsCount + '" name="Details[' + itemsCount + '].PercentageCode"  class="form-control item-taxcode" type="text">' + ivaItemsHtml + '</select>' +
                '<input id="iva' + itemsCount + '"  name="Details[' + itemsCount + '].TaxValue" class="item-taxvalue" value="0.00" type="hidden"></input>' +
                '<input id="ivarate' + itemsCount + '"  name="Details[' + itemsCount + '].TaxRate" class="item-taxrate" value="0.00" type="hidden"></input>' +
                '</div>' +


                '</div>' +
                '</div>' +

                '<div class="col-md-2 col-sm-4 col-xs-12" style="padding-bottom:5px">' +
                '<div class="row">' +

                '<div class="col-md-12 col-sm-12 col-xs-12 numeric text-center" data-title="Total">' +
                ' <h4 style="margin-top: 10px;font-weight:300;">$&nbsp;<span data-field="Details[' + itemsCount + '].SubTotal">0.00</span></h4> ' +
                '<input class="invoice-subtotal item-subtotal" data-val="true" data-val-number="El campo SubTotal debe ser un número." data-val-required="The SubTotal field is required." id="tot' + itemsCount + '" name="Details[' + itemsCount + '].SubTotal" type="hidden" value="0.00">' +
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
                <input class="invoice-subtotal item-subtotal" data-val="true" data-val-number="El campo SubTotal debe ser un número." data-val-required="The SubTotal field is required." id="Details_7__SubTotal" name="Details[7].SubTotal" type="hidden" value="3.00">
                                                        </div>`

            var result = $(".product-items").append(markup);
            var obj = $("#" + itemListId);
            var iva = $("#ivaitem" + itemsCount);

            $("#vlr" + itemsCount).on("change", refresh_totals);
            $("#ivaitem" + itemsCount).on("change", refresh_totals);
            $("#dsc" + itemsCount).on("change", refresh_totals);
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
                subtotal_12 = 0.00;
                subtotal_0 = 0.00;
                total_iva = 0.00;
                subtotal_nosujeto = 0.00;
                subtotal_exento = 0.00;
                subtotal_sin_impuestos = 0.00;
                total = 0.00;
                total_item = 0.00;
                totalPagos = 0.00;
                saldoPagos = 0.00;

                // PROCESO DE CALCULO DE LOS VALORES POR ITEM
                var itemObj = $(".product-item");

                // Busco dentro de los items de la lista
                for (var i = 0; i < itemObj.length; i++) {
                    var itemInput = $(itemObj[i]);
                    var ivaSelect = itemInput.find(".item-taxcode")[0];

                    if (typeof itemInput !== 'undefined' && itemInput.length > 0) {
                        var valorObject = itemInput.find(".item-value")[0];
                        var ivaObject = itemInput.find(".item-taxvalue")[0];
                        var totObject = itemInput.find(".item-subtotal");
                        var ivaRateObject = itemInput.find(".item-taxrate")[0];

                        var vlr = getValueSafe(valorObject.value);

                        if (vlr < 0) {
                            vlr = 0;
                            show_error("El valor de la modificación no puede ser menor un valor negativo", valorObject);
                        }


                        var ivaRate = 0.00;

                        var ivaItem = DebitNote.taxes.find(o => o.SriCode == ivaSelect.value);

                        if (typeof ivaItem !== 'undefined' && ivaItem !== null) {
                            ivaRate = ivaItem.RateValue;
                        }

                        var subtotal = getValueSafe(valorObject, 6);
                        total_item = getValueSafe(total_item + subtotal, 6);

                        var iva = getValueSafe(subtotal * (ivaRate / 100), 4);

                        subtotal = getValueSafe(subtotal, 2);
                        ivaObject.value = getValueSafe(iva, 2);
                        ivaRateObject.value = getValueSafe(ivaRate, 2);


                        var valueFixed = vlr.toFixed(6);

                        if (valueFixed.endsWith("0000")) {
                            valueFixed = vlr.toFixed(2);
                        }

                        valorObject.value = valueFixed;

                        totObject.val(subtotal.toFixed(2));
                        totObject.trigger("change");

                        // Acumulamos el valor del IVA
                        total_iva = total_iva + iva;

                        if (ivaItem.SriCode == '2' || ivaItem.SriCode == '3' || ivaItem.SriCode == '4' || ivaItem.SriCode == '5' || ivaItem.SriCode == '8') {
                            subtotal_12 = getValueSafe(subtotal_12 + subtotal, 2);
                        }
                        else if (ivaItem.SriCode == '6') {
                            subtotal_nosujeto = getValueSafe(subtotal_nosujeto + subtotal, 2);
                        }
                        else if (ivaItem.SriCode == '7') {
                            subtotal_exento = getValueSafe(subtotal_exento + subtotal, 2);
                        }
                        else {
                            subtotal_0 = getValueSafe(subtotal_0 + subtotal, 2);
                        }

                    }
                }

                // Calculo del IVA de la factura
                if (typeof total_iva !== 'undefined') {
                    total_iva = getValueSafe(total_iva, 2);
                }

                // Calculo del subtotal sin Impuestos
                subtotal_sin_impuestos = getValueSafe(subtotal_0 + subtotal_12 + subtotal_nosujeto + subtotal_exento, 2);

                // Total general de la factura
                total = getValueSafe(subtotal_sin_impuestos + total_iva, 2);

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
                var objsubtotal = $("#Subtotal");
                var objsubtotal_12 = $("#SubtotalVat");
                var objsubtotal_0 = $("#SubtotalVatZero");
                var objtotobjiva = $("#SubtotalNotSubject");
                var objtotexciva = $("#SubtotalExempt");
                var objtotiva = $("[name='ValueAddedTax']");

                var objtotal = $("#Total");
                var objvalorTotal = $("#Total");
                var objvalorIva = document.getElementById("ValueAddedTax");
                var objtotalItem = document.getElementById("TotalItem");

                // Objetos de calculo de pagos
                var objtotpagos = $("#TotalPayment");
                var objsalpagos = $("#Balance");

                setValueSafe(objsubtotal, subtotal_sin_impuestos);
                setValueSafe(objsubtotal_12, subtotal_12);
                setValueSafe(objsubtotal_0, subtotal_0);
                setValueSafe(objtotobjiva, subtotal_nosujeto);
                setValueSafe(objtotexciva, subtotal_exento);
                setValueSafe(objtotiva, total_iva);
                setValueSafe(objtotal, total);
                setValueSafe(objvalorTotal, total);

                objvalorIva.innerHTML = total_iva.toFixed(2);

                objtotalItem.innerHTML = total_item.toFixed(2);
                //objitemQuantity.innerHTML = fixUp(item_qty);

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

        docNumberChanged = function () {
            var $obj = $(this);
            var text = formatNumDoc($obj.val());
            if (text != '' && text.length > 17) {
                text = text.substr(0, 17);                
            }
            $obj.val(text);
        },

        refautorizacionChanged = function () {
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

        configure_contrib = function () {

            $("#ContributorId").select2({
                ajax: {
                    url: DebitNote.contributorsUrl,
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
                            var w = open(Invoice.loginUrl, "_top", "height=770,width=520");
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

                if ($("#Identification").val() != data.Identification) {
                    $("#Identification").val(data.Identification);
                    $("#ContributorName").val(data.BussinesName || data.TradeName);
                    $("#Address").val(data.Address);
                    $("#Phone").val(data.Phone);
                    $("#EmailAddresses").val(data.EmailAddresses);

                    ///Limpiamos los campos del documento sustento                    
                    $("#ref_numero_comprobante").val('');
                    $("#ref_autorizacion_comprobante").val('');
                    $("#ref_fecha_documento").val('');                  
                    const $select = $("#inputDocumentId");
                    $select.empty();


                    /// Validad si hay informacion adicional para agregar o modificar (dirección, teléfono, email)
                    validate_additional(data)

                    /// Cargo la informacion en los elementos
                    $("#headDocument").removeClass("kt-hide");
                    $("#headDocument").fadeIn();

                    ///cargar los documentos del cliente
                    startDocumentSelector(data.Id);
                }

                return repo.text;
            }
            else if (repo.id !== "") {

                /// Cargo la informacion en los elementos
                $("#headDocument").removeClass("kt-hide");
                $("#headDocument").fadeIn();
                initialized = true;

                ///cargar los documentos del cliente
                startDocumentSelector(repo.id);
            }
            else {

                $("#headDocument").addClass("kt-hide");
                $("#headDocument").fadeOut();

                if (typeof agregarAditional !== 'undefined') {
                    add_additional("Direccion", "");
                    add_additional("Email", "");
                    add_additional("Telefono", "");
                }
            }

            return repo.text;

        },

        validate_additional = function (data) {

            var $debitnoteId = $("#Id");
            if ($debitnoteId.length > 0 && $debitnoteId.val() > 0) {
                var $address = 0;
                var $phone = 0;
                var $emailAddresses = 0;
                if (additionalCount > 0) {
                    for (var i = 0; i < additionalCount; i++) {
                        var $det = $("#adname" + i);
                        if ($det.length > 0) {
                            var $val = $det.parents("tr").find(".additional-value");
                            var $nam = $det.parents("tr").find(".additional-name");
                            if ($nam.length > 0) {
                                if ($nam.val() === "DIRECCION") {
                                    $address = 1;
                                    $val.attr('value', data.Address);
                                }
                                else if ($nam.val() === "TELEFONO") {
                                    $val.attr('value', data.Phone);
                                    $phone = 1;
                                }
                                else if ($nam.val() === "EMAIL") {
                                    $val.attr('value', data.EmailAddresses);
                                    $emailAddresses = 1;
                                }
                            }
                        }
                    }

                    if ($address === 0)
                        add_additional("DIRECCION", data.Address, true);
                    if ($phone === 0)
                        add_additional("TELEFONO", data.Phone, true);
                    if ($emailAddresses === 0)
                        add_additional("EMAIL", data.EmailAddresses, true);
                }
                else {
                    add_additional("DIRECCION", data.Address, true);
                    add_additional("TELEFONO", data.Phone, true);
                    add_additional("EMAIL", data.EmailAddresses, true);
                }
            }
            else {
                for (var i = 1; i <= 3; i++) {
                    var $add_name = "DIRECCION";
                    var $add_value = data.Address;
                    if (i === 2) {
                        $add_name = "TELEFONO";
                        $add_value = data.Phone;
                    }
                    else if (i === 3) {
                        $add_name = "EMAIL";
                        $add_value = data.EmailAddresses;
                    }
                    var $det = $("#adname" + i);
                    if ($det.length > 0) {

                        var $val = $det.parents("tr").find(".additional-value");
                        var $nam = $det.parents("tr").find(".additional-name");

                        if ($nam.length > 0 && $nam.val() === $add_name)
                            $val.attr('value', $add_value);
                        else
                            add_additional($add_name, $add_value, true);
                    }
                    else { add_additional($add_name, $add_value, true); }
                }
            }
        },

        // tiene que ver con el Bundling del Javascript, el parametro tiene el mismo nombre que la data>params.
        startDocumentSelector = function (cliente_id) {

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
        },

        formatDoc = function (repo) {
            if (repo) {
                return repo.text;
            }

            return "No hay documentos que mostrar...";
        },

        formatDocSelection = function (repo) {
            return formatDoc(repo);
        },

        documentListChanged = function (repo) {
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
        },

        cargarDocumento = function (doc) {
            if (doc) {               

                $("#ref_tipo_comprobante").val(doc.DocumentTypeCode);
                $("#ref_numero_comprobante").val(doc.DocumentNumber);
                $("#ref_autorizacion_comprobante").val(doc.AuthorizationNumber);
                $("#ref_fecha_documento").val(doc.InvoiceInfo.IssuedOn);

                //valor_original = doc.Total;

                //$("#product-items tbody").empty();

                //if (doc.InvoiceInfo) {

                //    var details = doc.InvoiceInfo.Details;
                //    var detailCount = details.length;

                //    for (var i = 0; i < detailCount; i++) {
                //        var item = details[i];

                //        agregarItem(item);
                //    }
                //}

                //actualizarTotales();
            }
            else {
                valor_original = 0.00;
            }
        },

        getUrlSearchDocument = function (id) {
            return `${DebitNote.documentUrl}/${id}`;
        },

        getUrlGetDocument = function (id) {
            return `${DebitNote.documentbyIdUrl}/${id}`;
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

            var objestablishment = $(".select-establishment")[0];
            if (objestablishment.value.length <= 0) {
                return valError("Por favor seleccione un establecimiento!", objestablishment);
            }

            var objcontributor = $("#ContributorId");
            var contributor_id = objcontributor.val();
            if (contributor_id == null || contributor_id == "") {
                return valError("Por favor seleccione un cliente!", objcontributor);
            }

            var reffechadocumento = $("#ref_fecha_documento");
            if (reffechadocumento.val() == null || reffechadocumento.val() == "") {
                return valError("Por favor ingrese la fecha  del documento!");
            }

            var refDocumentNumber = $("#ref_numero_comprobante").val();
            if (refDocumentNumber == null || refDocumentNumber == "") {
                return valError("Por favor ingrese un número de documento valido!");
            }           

            var refDocumentAuth = $("#ref_autorizacion_comprobante").val();
            if (refDocumentAuth == null || refDocumentAuth == "") {
                return valError("Por favor ingrese el número de autorización del comprobante sustento de la nota de débito!");
            }
            else if (refDocumentAuth.length != 49 && refDocumentAuth.length != 10 && refDocumentAuth.length != 37) {
                return valError("El número de autorización del comprobante sustento de la nota de débito debe ser de 10, 37 o 49 digitos");
            }

            // Busco dentro de los items de la lista
            var itemObj = $(".product-item");
            for (var i = 0; i < itemObj.length; i++) {
                var inputs = $(itemObj[i]).find("input");
                var razon = inputs[1].value;
                if (razon == null || razon =="") {
                    return valError("Debe especificar la razon o motivo de la nota débito!", inputs[1]);
                }        
                var vlr = getValueSafe(inputs[2].value);
                if (vlr <= 0.00000) {
                    return valError("El valor no puede ser 0.00 ", inputs[2]);
                }               
            }

            // Motivo de la nota debito 
            var objReason = $("#Reason");
            var documentReason = objReason.val();
            if (documentReason == null || documentReason == '') {
                return valError("El motivo o explicacion de la nota de credito esta vacio!", objReason);
            }


            return true;
            
        },

        save_handler = function () {
            if (saving) {
                return;
            }

            if ($(this).hasClass("btn-invoice-issue")) {
                $("#Status").val("1");
            } else {
                if (!$("#Id").val()) {
                    $("#Status").val("0");
                }
            }

            var $form = $(".debitNote-form");         

            if (!ValidarDocument()) {                
                return;
            }

            showLoader();
            saving = true;

            var $action = $form.attr("action");
            var $data = $form.serializeArray();

            $.post($action, $data, function (result) {
                if (result) {
                    if (result.id > 0) {
                        toastr.success(result.statusText);
                        location.assign(result.url);
                    }
                    else if (result.id == -999) {
                        toastr.warning(result.error.UserMessage);
                        location.assign(result.url);
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
            }).fail(function (error, data, obj) {
                console.log(error);
                console.log(data);
                toastr.warning("Hubo un error al guardar el documento");
            }).always(function () {
                saving = false;
                hideLoader();
            });
        };

    return {
        contributorsUrl: "",
        documentUrl: "",
        documentbyIdUrl: "",
        loginUrl: "/Auth",
        taxes: [],
        paymentTypes: [],
        establishments: [],
        init: function () {
            main();
        }
    }
}();