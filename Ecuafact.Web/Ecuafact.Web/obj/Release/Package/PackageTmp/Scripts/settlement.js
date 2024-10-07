var Settlement = function () {

    var initialized = false,
        calculating = 0,
        saving = false,
        itemsCount = 0,
        additionalCount = 0,
        paymentCount = 0,
        fecha_mes_atras = new Date(new Date() - (30 * 24 * 60 * 60 * 60 * 15)).toLocaleDateString("es"),
        fecha_actual = new Date().toLocaleDateString("es"),

        main = function () {

            itemsCount = $("tr.product-item").length;
            additionalCount = $("tr.additional-item").length;
            paymentCount = $("tr.payment-item").length;

            $(".settlement-control").on("change", refresh_totals);
            $(".settlement-form").on("submit", function (e) {
                e.preventDefault();
                save_handler();
            });

            $(".add-detail").on("click", add_detail);
            $(".add-payment").on("click", add_payment);
            $(".add-additional").on("click", add_additional);
            $(".btn-settlement-save").on("click", save_handler);
            $(".btn-settlement-issue").on("click", save_handler);

            $("[name='Term']").on("change", refresh_totals);
            $("[name='TimeUnit']").on("change", refresh_totals);
            $("[name='ReferralGuide']").on("change", set_referralguide)

            $(".settlement-subtotal").totalText();

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
        },

        set_establishment = function () {
            var obj = $('.select-establishment')[0];
            if (obj && obj.value.length > 0) {
                // Obtengo la informacion del impuesto
                var est = Settlement.establishments.find(
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
               // add_additional("TELEFONO", $(this).val(), true);
            });

            $("[name='EmailAddresses']").on("change", function () {
                exists_additional("EMAIL", $(this).val(), true);
                //add_additional("EMAIL", $(this).val(), true);
            });

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
            configure_product();

            $(".item-quantity").on("change", refresh_totals);
            $(".item-unitprice").on("change", refresh_totals);
            $(".item-taxcode").on("change", refresh_totals);
            $(".item-discount").on("change", refresh_totals);

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

            var $payment = Settlement.paymentTypes;
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

            var $taxes = Settlement.taxes;

            for (var i = 0; i < $taxes.length; i++) {
                ivaItemsHtml += "<option style='text-align:right;' value='" + $taxes[i].SriCode + "'>" + $taxes[i].Name + "</option>";
            }

            var markup =
                '<tr id="item' + itemsCount + '" class="product-item">' +
                '<td data-title="Product" style="width:100%">' +
                '<div class="row">' +

                '<input id="item' + itemsCount + '"  name="Details.Index" value="' + itemsCount + '" type="hidden"></input>' +
                '<input id="mainCode' + itemsCount + '"  name="Details[' + itemsCount + '].MainCode" class="item-maincode"  value="" type="hidden"></input>' +
                '<input id="auxCode' + itemsCount + '"  name="Details[' + itemsCount + '].AuxCode" class="item-auxcode"  value="" type="hidden"></input>' +
                '<input id="desc' + itemsCount + '"  name="Details[' + itemsCount + '].Description" class="item-description"  value="" type="hidden"></input>' +

                '<div class="col-md-10 col-sm-8 col-xs-12" >' +
                '<div class="row">' +
                '<div class="col-md-4 col-sm-12" data-title="Descripcion" class="numeric" style="padding-bottom:5px">' +
                '<select id = "' + itemListId + '" name="Details[' + itemsCount + '].ProductId"  class="form-control item-productid" data-parent="item' + itemsCount + '" style = "width:100%;"></select></div> ' +
                               

                '<div class="col-md-2 col-sm-6 col-xs-12" data-title="Cantidad" class="numeric" style="padding-bottom:5px"> ' +
                '<input id="qty' + itemsCount + '" name="Details[' + itemsCount + '].Amount"  class="form-control item-quantity" style="text-align:right; background:none;" type="number" value="1" min="1" placeholder="Cantidad" />' +
                '</div>' +

                '<div class="col-md-2 col-sm-6 col-xs-12" data-title="Precio" class="numeric" style="padding-bottom:5px">' +
                '<input id="price' + itemsCount + '" name="Details[' + itemsCount + '].UnitPrice"  class="form-control item-unitprice" style="text-align:right;background:none;  " type="number" value="0.00" min="0.01" placeholder="Precio" />' +
                '</div>' +

                '<div class="col-md-2 col-sm-6 col-xs-12" data-title="Desc" class="numeric" style="padding-bottom:5px">' +
                '<input id="dsc' + itemsCount + '"  name="Details[' + itemsCount + '].Discount"  class="form-control item-discount" style="text-align:right; background:none; " type="number" value="0.00" min="0.00"  max="100.00" placeholder="% Descuento" />' +
                '</div>' +


                '<div class="col-md-2 col-sm-6 col-xs-12" data-title="IVA" class="numeric" style="padding-bottom:5px">' +
                '<select id="ivaitem' + itemsCount + '" name="Details[' + itemsCount + '].ValueAddedTaxCode"  class="form-control item-taxcode" type="text">' + ivaItemsHtml + '</select>' +
                '<input id="iva' + itemsCount + '"  name="Details[' + itemsCount + '].ValueAddedTaxValue" class="item-taxvalue" value="0.00" type="hidden"></input>' +
                '</div>' +

                '<div class="col col-md-10 col-sm-8 col-xs-12">' +
                '<div class="accordion accordion-solid accordion-toggle-plus" id="accordAdditionalsItem' + itemsCount + '">' +
                '<div class="card">' +
                '<div class="card-header" id="itemHeading' + itemsCount + '">' +
                '<div class="card-subtitle collapsed" data-toggle="collapse" data-target="#itemsCollapse' + itemsCount + '" aria-expanded="false" aria-controls="itemsCollapse' + itemsCount + '">' +
                '<br /><i class="flaticon2-add"></i> Detalles Adicionales' +
                '</div>' +
                '</div>' +
                '<div id="itemsCollapse' + itemsCount + '" class="collapse" aria-labelledby="itemHeading' + itemsCount + '" data-parent="#accordAdditionalsItem' + itemsCount + '" style="">' +
                '<div class="card-body">' +
                '<table class="table table-payment col-md-12 table-hover table-striped table-condensed kt-datatable__table">' +
                
                '<tbody id = "additionals-items' + itemsCount + '" class="kt-datatable__body">' +
                '<tr><td> <div class="row">' +

                '<div class="col-md-4 col-sm-12 col-xs-12 input-group" data-title="Detalle1" class="numeric" style="padding-bottom:5px;width:100%"> ' +
                '<input id="detname1_' + itemsCount + '" name="Details[' + itemsCount + '].Name1"  class="form-control item-name1 bold" style="text-align:left;background:none;font-size:11px;" placeholder="Nombre1" />' +
                '<input id="detval1_' + itemsCount + '" name="Details[' + itemsCount + '].Value1"  class="form-control item-value1 bold" style="text-align:left;background:none;font-size:11px;" placeholder="Valor1" />' +
                '</div>' +

                '<div class="col-md-4 col-sm-12 col-xs-12 input-group" data-title="Detalle1" class="numeric" style="padding-bottom:5px;width:100%"> ' +
                '<input id="detname2_' + itemsCount + '" name="Details[' + itemsCount + '].Name2"  class="form-control item-name2 bold" style="text-align:left;background:none;font-size:11px;" placeholder="Nombre2" />' +
                '<input id="detval2_' + itemsCount + '" name="Details[' + itemsCount + '].Value2"  class="form-control item-value2 bold" style="text-align:left;background:none;font-size:11px;" placeholder="Valor2" />' +
                '</div>' +

                '<div class="col-md-4 col-sm-12 col-xs-12 input-group" data-title="Detalle1" class="numeric" style="padding-bottom:5px;width:100%"> ' +
                '<input id="detname3_' + itemsCount + '" name="Details[' + itemsCount + '].Name3"  class="form-control item-name3 bold" style="text-align:left;background:none;font-size:11px;" placeholder="Nombre3" />' +
                '<input id="detval3_' + itemsCount + '" name="Details[' + itemsCount + '].Value3"  class="form-control item-value3 bold" style="text-align:left;background:none;font-size:11px;" placeholder="Valor3" />' +

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
                '</div>' +

                '<div class="col-md-2 col-sm-2 col-xs-12" style="padding-bottom:5px">' +
                '<div class="row">' +
                '<div class="col-md-12 col-sm-12 col-xs-12 numeric text-center" data-title="Total">' +
                ' <h4 style="margin-top: 10px;font-weight:300;">$&nbsp;<span data-field="Details[' + itemsCount + '].SubTotal">0.00</span></h4> ' +
                '<input class="settlement-subtotal item-subtotal" data-val="true" data-val-number="El campo SubTotal debe ser un número." data-val-required="The SubTotal field is required." id="tot' + itemsCount + '" name="Details[' + itemsCount+'].SubTotal" type="hidden" value="0.00">'+
                
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</td>' +
                '<td data-title="">' +
                '<button id="trash' + itemsCount + '" title="Eliminar Registro" type="button" class="delete-item btn " style="width:auto;float:none;" ><span class="la la-trash-o "></span></button>' +
                '</td>' +
                '</tr>';

            var x =  `<div class="col-md-12 col-sm-12 col-xs-12 numeric text-center" data-title="Total">
                <h4 style="margin-top: 10px;font-weight:300;">$&nbsp;<span data-field="Details[7].SubTotal">3.00</span></h4>
                <input class="settlement-subtotal item-subtotal" data-val="true" data-val-number="El campo SubTotal debe ser un número." data-val-required="The SubTotal field is required." id="Details_7__SubTotal" name="Details[7].SubTotal" type="hidden" value="3.00">
                                                        </div>`

            var result = $(".product-items").append(markup);
            var obj = $("#" + itemListId);
            var iva = $("#ivaitem" + itemsCount);
            var descObj = $("#dsc" + itemsCount);

            $("#qty" + itemsCount).on("change", refresh_totals);
            $("#price" + itemsCount).on("change", refresh_totals);
            $("#ivaitem" + itemsCount).on("change", refresh_totals);
            $("#dsc" + itemsCount).on("change", refresh_totals);
            $("#trash" + itemsCount).on("click", delete_item);
            $("#tot" + itemsCount).totalText();


            configure_product(obj);
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
                total_descuento = 0.00;
                total_iva = 0.00;
                subtotal_nosujeto = 0.00;
                subtotal_exento = 0.00;
                subtotal_sin_impuestos = 0.00;
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
                        var ivaObject = itemInput.find(".item-taxvalue")[0];
                        var dscObject = itemInput.find(".item-discount")[0];
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

                        var ivaRate = 0.00;

                        var ivaItem = Settlement.taxes.find(o => o.SriCode == ivaSelect.value);

                        if (typeof ivaItem !== 'undefined' && ivaItem !== null) {
                            ivaRate = ivaItem.RateValue;
                        }

                        var dsc = getValueSafe(dscObject.value, 2);

                        if (dsc < 0.000000) {
                            dsc = 0.00;
                        }
                        else {
                            if (dsc > 100) {
                                dsc = 100.00;
                            }
                        }

                        var subtotal = getValueSafe(qty * prc, 6);

                        total_item = getValueSafe(total_item + subtotal, 6);

                        var valdesc = getValueSafe(subtotal * (dsc / 100), 2);

                        subtotal = getValueSafe(subtotal - valdesc, 6);  // el valor menos el descuento

                        var iva = getValueSafe(subtotal * (ivaRate / 100), 2);

                        total_descuento = getValueSafe(total_descuento + valdesc, 2);

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
                        dscObject.value = getValueSafe(dsc, 2);
                        ivaObject.value = getValueSafe(iva, 2);

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

                var propinaObj = $("#Tip");

                valor_propina = getValueSafe(propinaObj);

                if (typeof valor_propina !== 'undefined') {
                    setValueSafe(propinaObj, valor_propina);
                }

                // Calculo del IVA de la liquidación
                if (typeof total_iva !== 'undefined') {
                    total_iva = getValueSafe(total_iva, 2);
                }

                // Calculo del subtotal sin Impuestos
                subtotal_sin_impuestos = getValueSafe(subtotal_0 + subtotal_12 + subtotal_nosujeto + subtotal_exento, 2);

                // Total general de la liquidación
                total = getValueSafe(subtotal_sin_impuestos + total_iva + valor_propina, 2);

                // Realizo el calculo de Pagos
                var $payment_list = $("input.payment-total");

                if ($payment_list.length > 0) {

                    var $pay_term = $("[name='Term']").val();
                    var $time_unit = $("[name='TimeUnit']").val();

                    var $payment_unit = $("input.payment-timeunit");
                    var $payment_term = $("input.payment-term");

                    // Si solo existe una linea de pagos entonces
                    // debe llenarse con el valor total de la liquidación
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
                var objtotdesc = $("#TotalDiscount");
                var objtotiva = $("[name='ValueAddedTax']");

                var objtotal = $("#Total");
                var objvalorTotal = $("#Total");
                var objvalorIva = document.getElementById("ValueAddedTax");
                var objtotalItem = document.getElementById("TotalItem");
                var objitemQuantity = document.getElementById("ItemQuantity");

                // Objetos de calculo de pagos
                var objtotpagos = $("#TotalPayment");
                var objsalpagos = $("#Balance");

                setValueSafe(objsubtotal, subtotal_sin_impuestos);
                setValueSafe(objsubtotal_12, subtotal_12);
                setValueSafe(objsubtotal_0, subtotal_0);
                setValueSafe(objtotobjiva, subtotal_nosujeto);
                setValueSafe(objtotexciva, subtotal_exento);
                setValueSafe(objtotdesc, total_descuento);
                setValueSafe(objtotiva, total_iva);
                setValueSafe(objtotal, total);
                setValueSafe(objvalorTotal, total);

                objvalorIva.innerHTML = total_iva.toFixed(2);

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
                    show_error("El valor de los pagos no puede ser mayor al total de la liquidación de compra!");
                    $("[data-field='Balance']").addClass("text-danger h4");
                }
                else {
                    $("[data-field='Balance']").removeClass("text-danger h4");
                }

                setValueSafe(objtotpagos, totalPagos);
                setValueSafe(objsalpagos, saldoPagos);

                if (valor_propina < 0) {
                    show_error("El valor de la propina no puede ser un valor negativo!");
                    $(propinaObj).addClass("btn-danger");
                }
                else {
                    $(propinaObj).removeClass("btn-danger");
                }


                // flag para evitar sobrecalculos en procesos
                calculating = 0;

            }

        },

        // Muestra el mensaje de validacion de la liquidación
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
                    url: Settlement.productsUrl,
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
                $parentTR.find(".item-description").val(repo.data.Description);
                $parentTR.find(".item-name1").val(repo.data.Name1);
                $parentTR.find(".item-value1").val(repo.data.Value1);
                $parentTR.find(".item-name2").val(repo.data.Name2);
                $parentTR.find(".item-value2").val(repo.data.Value2);
                $parentTR.find(".item-name3").val(repo.data.Name3);
                $parentTR.find(".item-value3").val(repo.data.Value3);

                var ivaitem = Settlement.taxes.find(o => o.Id == repo.data.IvaRateId);

                if (typeof ivaitem !== 'undefined' && ivaitem !== null) {
                    $parentTR.find(".item-taxcode").val(ivaitem.SriCode);
                    $parentTR.find(".item-taxcode").trigger('change');
                }

                refresh_totals();

                if (typeof initialized !== 'undefined' && initialized) {
                    trItem[0].focus();
                }
            }

            return repo.text;

        },

        configure_contrib = function () {
            var searchContributorUrl = Settlement.contributorsUrl + "/SearchContribAsync";

            $("#ContributorId").select2({
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
                            var w = open(Settlement.loginUrl, "_top", "height=770,width=520");
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

                    validate_additional(data);
                    //add_additional("DIRECCION", data.Address, true);
                    //add_additional("TELEFONO", data.Phone, true);
                    //add_additional("EMAIL", data.EmailAddresses, true);
                }

                return repo.text;
            }
            else {
                var contribId = $("#ContributorId").val();

                if (!contribId) {

                    $("#ContributorName").val("CONSUMIDOR FINAL");
                    $("#Identification").val("9999999999999");
                    $("#Address").val("Av. Principal");
                    $("#Phone").val("999999");
                    $("#EmailAddresses").val("facturacion@ecuafact.com");

                }
            }

            return repo.text;

        },

        validate_additional = function (data) {

            var $invoiceId = $("#Id");
            if ($invoiceId.length > 0 && $invoiceId.val() > 0) {
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

        save_handler = function () {
            if (saving) {
                return;
            }

            
            if ($(this).hasClass("btn-settlement-issue")) {
                $("#Status").val("1");
            } else {
                if (!$("#Id").val()) {
                    $("#Status").val("0");
                } 
            }

            var $form = $(".settlement-form");

            //if (!$form.valid()) {
            //    show_error("Por favor corriga los errores en el documento para continuar!");
            //    return;
            //}

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
        productsUrl: "/Productos",
        contributorsUrl: "/Contribuyentes",
        loginUrl: "/Auth",
        taxes: [],
        paymentTypes: [],
        establishments: [],
        init: function () {
            main();
        }
    }
}();