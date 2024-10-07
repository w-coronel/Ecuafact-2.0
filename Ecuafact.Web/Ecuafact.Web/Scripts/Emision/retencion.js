var Retention = function () {

    var initialized = false,
        calculating = 0,
        saving = false,
        itemsCount = 0,
        additionalCount = 0,
        fecha_mes_atras = new Date(new Date() - (30 * 24 * 60 * 60 * 60 * 15)).toLocaleDateString("es"),
        fecha_actual = new Date().toLocaleDateString("es"),
        fechaRefObj,
        fechaFiscfObj,
        fechaRegContObj,
        $nameFile,
        $fileRaw,
        retencionInformativa = false,
        valor_original = 0.0,
        documentSupport = {},
        selectedDocument = false,
        paymentCount = 0,

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

            fechaRegContObj = $("#AccountingRegistrationDate");
            fechaRegContObj.datepicker({
                endDate: fecha_actual,
                keyboardNavigation: false,
                format: "dd/mm/yyyy",
                language: "es",
                orientation: "bottom left",
                forceParse: false,
                autoclose: true
            });

            //fechaRefObj.val(fecha_actual);
            issuedOnObj = $("#IssuedOn");
            issuedOnObj.on("change", fechaDocumentoChanged);
            itemsCount = $("tr.product-item").length;
            additionalCount = $("tr.additional-item").length;
            $(".invoice-control").on("change", refresh_totals);
            $(".invoice-form").on("submit", function (e) {
                e.preventDefault();
                save_handler();
            });
            $(".add-detail").on("click", add_detail);
            $(".add-additional").on("click", add_additional);
            $(".btn-invoice-save").on("click", save_handler);
            $(".btn-invoice-issue").on("click", save_handler);
            $(".btn-document-support").on("click", modalDocumentSupport);
            $(".btn-document-manual").on("click", enableDocSoporteFisico);
            $(".js-documentSelector").on("change", function () { documentListChanged(); });
            $("[name='Term']").on("change", refresh_totals);
            $("[name='TimeUnit']").on("change", refresh_totals);
            $(".select-establishment").on("change", set_establishment);
            $("#ref_numero_comprobante").on("change", docNumberChanged);
            $("#ref_autorizacion_comprobante").on("change", refautorizacionChanged);
            $(".ref-iva-comprobante").on("change", refresh_totals);
            $(".ref-exempt-comprobante").on("change", subt_refresh_total);
            $(".ref-nosubjeto-comprobante").on("change", subt_refresh_total);
            $(".ref-base0-comprobante").on("change", subt_refresh_total);
            $(".ref-base12-comprobante").on("change", subt_refresh_total);
            $("#ref_total_comprobante").on("change", refresh_totals);
            $(".select-documnet-Support").on("click", selectDocumnetSupport);
            $(".file-document-xml").on("change blur", importDocumentSupport);
            $(".subir-documnet-support").on("click", handleDocumentSupport);
            $(".add-payment").on("click", add_payment);
            if (itemsCount > 0) {
                configure_details();
            }
            configure_additionals();
            configure_contrib();
            refresh_totals();
            initialized = true;
            startDocumentSelector(null);
            editingEnableDocSoporte();
        },

        subt_refresh_total = function () {

            var objImporteTotal = $("#ref_total_comprobante")[0];
            importeTotal = getValueSafe(objImporteTotal.value);
            var objBaseIVA = $("#ref_iva_comprobante")[0];
            baseIVA = getValueSafe(objBaseIVA.value);
            var objBaseImponible = $("#ReferenceDocumentAmount")[0];
            baseImponible = importeTotal - baseIVA;
            setValueSafe(objBaseImponible, baseImponible);
            refresh_totals();
        },

        fechaDocumentoChanged = function () {
            var fecha = $(this).val();
            if (fecha && typeof fecha == "string") {
                var periodoFiscal = fecha.substring(3, 10);
                $("#FiscalPeriod").val(periodoFiscal);
            }
        },

        set_establishment = function () {
            var obj = $('.select-establishment')[0];
            if (obj && obj.value.length > 0) {
                // Obtengo la informacion del impuesto
                var est = Retention.establishments.find(
                    function (i) { return i.Code == obj.value; }
                );
                var issuepoint = est.IssuePoint;
                var sHtmlRates = "";
                for (var i = 0; i < issuepoint.length; i++) {
                    sHtmlRates += '<option value="' + issuepoint[i].Code + '" style = "text-align:right;width:100%;">' + issuepoint[i].Code + '</option>';
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
                //add_additional("EMAIL", $(this).val(), true);
            });

        },

        configure_details = function () {
            startSelectTax();
            $(".item-taxbase").on("change", refresh_totals);
            $(".item-taxrate").on("change", refresh_totals);
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

        add_detail = function () {

            itemsCount++;
            var itemListId = "selectProducto" + itemsCount;

            var markup =
                '<tr id="item' + itemsCount + '" class="product-item">' +
                '<td data-title="Product" style="width:100%">' +
                '<div class="row">' +
                '<div class="col-md-10 col-sm-8 col-xs-12" >' +

                '<div class="row">' +
                '<input id="item' + itemsCount + '"  name="Details.Index" value="' + itemsCount + '" type="hidden"></input>' +
                '<input id="retetaxcode' + itemsCount + '"  name="Details[' + itemsCount + '].RetentionTaxCode" class=" item-retentiontaxcode"  type="hidden"></input>' +

                '<div class="col-md-8 col-sm-12 col-xs-12" data-title="Descripción" class="text" style="padding-bottom:5px">' +
                '<select id = "' + itemListId + '" name="Details[' + itemsCount + '].RetentionTaxId"  class="form-control item-retentiontaxid" data-parent="item' + itemsCount + '" style = "width:100%;">' + Retention.listaImpuestos + '</select></div> ' +

                '<div class="col-md-2 col-sm-6 col-xs-12" data-title="Value" class="numeric" style="padding-bottom:5px"> ' +
                '<input id="baseitem' + itemsCount + '" name="Details[' + itemsCount + '].TaxBase"  class="form-control item-taxbase" style="text-align:right; background:none;" type="number" value="0.00" min="0.01" placeholder="Base Imponible" />' +
                '</div>' +

                '<div class="col-md-2 col-sm-6 col-xs-12" data-title="Porcentaje" class="numeric" style="padding-bottom:5px">' +
                '<select id="taxrate' + itemsCount + '" name="Details[' + itemsCount + '].TaxRate"  class="form-control item-taxrate" type="text"></select>' +
                '<input id="taxtypecode' + itemsCount + '"  name="Details[' + itemsCount + '].TaxTypeCode" class=" item-taxtypecode"  type="hidden"></input>' +
                '</div>' +


                '</div>' +
                '</div>' +

                '<div class="col-md-2 col-sm-4 col-xs-12" style="padding-bottom:5px">' +
                '<div class="row">' +

                '<div class="col-md-12 col-sm-12 col-xs-12 numeric text-center" data-title="Total">' +
                ' <h4 style="margin-top: 10px;font-weight:300;">$&nbsp;<span data-field="Details[' + itemsCount + '].TaxValue">0.00</span></h4> ' +
                '<input class="invoice-subtotal item-taxvalue" data-val="true" data-val-number="El campo Total debe ser un número." data-val-required="The SubTotal field is required." id="taxvalue' + itemsCount + '" name="Details[' + itemsCount + '].TaxValue" type="hidden" value="0.00">' +
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
            var iva = $("#taxrate" + itemsCount);

            $("#baseitem" + itemsCount).on("change", refresh_totals);
            $("#taxrate" + itemsCount).on("change", refresh_totals);
            $("#trash" + itemsCount).on("click", delete_item);
            $("#taxvalue" + itemsCount).totalText();


            startSelectTax(obj);
            refresh_totals();

            if (typeof initialized !== 'undefined' && initialized)
                if (itemsCount > 0) {
                    obj.focus();
                    obj.select2("open");
                }
        },

        add_payment = function () {
            paymentCount++;

            var paymentItemCount = paymentCount;
            var itemListId = "selectPayment" + paymentItemCount;

            var paymentHtml = '';

            var $payment = Retention.paymentTypes;
            if ($payment) {
                for (var i = 0; i < $payment.length; i++) {
                    paymentHtml += "<option value='" + $payment[i].SriCode + "'>" + $payment[i].Name + "</option>";
                }
            }

            //var $term = $("[name='Term']").val();
            //var $unit = $("[name='TimeUnit']").val();
            var $term = 0;
            var $unit = "dias";

            var markup =
                '<tr id="payment' + paymentItemCount + '" class="payment-item">' +
                ' <td data-title="Detalle">' +
                '  <div class="row">' +
                '   <input name="ReferenceDocumentPayments.Index" value="' + paymentItemCount + '" type="hidden" />' +
                '   <input name="ReferenceDocumentPayments[' + paymentItemCount + '].TimeUnit" class="payment-timeunit" value="' + $unit + '" type="hidden" />' +
                '   <input name="ReferenceDocumentPayments[' + paymentItemCount + '].Term" class="payment-term" value="' + $term + '" type="hidden" />' +
                '   <div data-title="FormaPago" class="col-lg-8 col-md-12 col-sm-8 col-xs-12 bold" style="padding-bottom:10px">' +
                '    <select id="' + itemListId + '" name="ReferenceDocumentPayments[' + paymentItemCount + '].PaymentMethodCode" class="form-control" style="width:100%;">' + paymentHtml + '</select>' +
                '   </div>' +
                '   <div data-title="Valor" class="col-lg-4 col-md-12 col-sm-4 col-xs-12 numeric" style="text-align:right;">' +
                '    <input id="pago' + paymentItemCount + '" name="ReferenceDocumentPayments[' + paymentItemCount + '].Total" class="form-control payment-total text-right" type="text" placeholder="Valor Pagado" />' +
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

        delete_item = function () {
            $(this).parents("tr").remove();
            refresh_totals();
        },

        refresh_totals = function () {
            if (calculating == 0) {
                // este es un control para evitar multiples llamadas
                // al proceso de calculo por los procesos propios de la funcion
                calculating = 1;
                validateerror = true;

                // Inicializamos el total:
                total = 0.00;
                saldo = 0.00;
                importeTotal = 0.00;

                var objImporteTotal = $("#ref_total_comprobante")[0];
                importeTotal = getValueSafe(objImporteTotal.value);

                var objBaseImponible = $("#ReferenceDocumentAmount")[0];
                baseImponible = getValueSafe(objBaseImponible.value);

                var objBaseIVA = $("#ref_iva_comprobante")[0];
                baseIVA = getValueSafe(objBaseIVA.value);

                var objBase12 = $(".ref-base12-comprobante")[0];
                var objBase10 = $(".ref-base0-comprobante")[0];
                var objNosubjeto = $(".ref-nosubjeto-comprobante")[0];
                var objExempt = $(".ref-exempt-comprobante")[0];

                base12 = getValueSafe(objBase12.value);
                base0 = getValueSafe(objBase10.value);
                baseNosubjeto = getValueSafe(objNosubjeto.value);
                baseExempt = getValueSafe(objExempt.value);

                var itemObj = $(".product-item"); // Lista de Detalles <TR>

                // Analizo registro x registro
                for (var i = 0; i < itemObj.length; i++) {
                    var itemInput = $(itemObj[i]);
                    var taxSelect = itemInput.find(".item-taxrate")[0];

                    if (typeof itemInput !== 'undefined' && itemInput.length > 0) {
                        var objBase = itemInput.find(".item-taxbase")[0];
                        //var ivaObject = itemInput.find(".item-taxvalue")[0];                       
                        var objVal = itemInput.find(".item-taxvalue");

                        //var objInputs = $(objDetalle).find("input");
                        //var objBase = objInputs[0];
                        //var objRate = objInputs[1];
                        //var objVal = objInputs[2];

                        // obtengo la informacion:
                        var base = getValueSafe(objBase.value);

                        //valida el valor base: tiene que ser positivo
                        if (base < 0.00) {
                            base = -base;
                        }

                        var rate = getValueSafe(taxSelect.value);

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
                        //if (taxSelect.value !== null || taxSelect.value !== '') {
                        //    taxSelect.value = rate + " %";
                        //}

                        total += value;

                    }
                }

                // Realizo el calculo de Pagos
                var itemObj = $(".payment-item");
                if (itemObj.length > 0) {
                    var $payment_list = $("input.payment-total");

                    if ($payment_list.length > 0) {

                        var $pay_term = 0;//$("[name='Term']").val();
                        var $time_unit = "dias";//$("[name='TimeUnit']").val();

                        var $payment_unit = $("input.payment-timeunit");
                        var $payment_term = $("input.payment-term");

                        // Si solo existe una linea de pagos entonces
                        // debe llenarse con el valor total de la factura
                        if ($payment_list.length == 1) {
                            $payment_term.val($pay_term);
                            $payment_unit.val($time_unit);

                            totalPagos = importeTotal;
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

                            var primerPago = getValueSafe(importeTotal - totalPagos, 2);

                            if (primerPago < 0) {
                                primerPago = 0.00;
                                show_error("El valor de los pagos no puede menor un valor negativo!");
                            }

                            setValueSafe($payment_list.first(), primerPago);

                            totalPagos = getValueSafe(totalPagos + primerPago, 2);
                        }
                    }

                }

                //saldo = (baseImponible + baseIVA) - total; 
                saldo = importeTotal - total;

                // Actualizar el total
                var objTotal = $("#FiscalAmount")[0];
                var objSaldoTotal = $("#Balance")[0];

                setValueSafe(objImporteTotal, importeTotal);
                setValueSafe(objBaseImponible, baseImponible);
                setValueSafe(objBaseIVA, baseIVA);

                setValueSafe(objBase12, base12);
                setValueSafe(objBase10, base0);
                setValueSafe(objNosubjeto, baseNosubjeto);
                setValueSafe(objExempt, baseExempt);

                setValueSafe(objTotal, total);
                setValueSafe(objSaldoTotal, saldo);

                if (baseImponible + baseIVA < total) {
                    Swal.fire("Validacion", "El total de la retención no puede ser mayor al valor del documento sustento!", "warning");
                }

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

        // Seccion de impuestos del documento
        startSelectTax = function (obj) {

            if (!obj) {
                obj = $(".product-item").find("select[name$='RetentionTaxId']");
            }
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
        },

        formatTaxSelection = function (repo) {

            if (typeof repo.id !== 'undefined') {

                // Buscamos id del elemento TR
                var $parentTR = $(repo.element).parents("tr");

                // Obtengo la informacion del impuesto
                var imp = Retention.impuestos.find(
                    function (i) { return i.Id == repo.id; }
                );

                var rates = imp.RetentionRate;
                var sHtmlRates = "";

                for (var i = 0; i < rates.length; i++) {
                    sHtmlRates += '<option value="' + rates[i].RateValue + '" style = "text-align:right;width:100%;">' + rates[i].RateValue + ' %</option>';
                }

                // Obtenemos el identificador de la linea
                /*var idtext = repo.element.parentElement.parentElement.id*/
                //var listObject = $("#taxitem" + idtext);
                var listObject = $parentTR.find(".item-taxrate");
                listObject.html(sHtmlRates);
                //var rateObject = $("#taxrate" + idtext);

                // Buscar Impuesto
                var imp = Retention.impuestos.find(function (imp) { return imp.Id == repo.id; });

                // Buscar Tipo de Impuesto
                var typ = Retention.tipos.find(function (tipo) { return tipo.Id == imp.TaxTypeId; });


                //if (rates.length == 1) {
                //    // Si solo hay uno establece el valor x default:
                //    rateObject.val(rates[0].RateValue + " %");
                //}
                //else {
                //    rateObject.val("");
                //}                         

                $parentTR.find(".item-retentiontaxcode").val(imp.SriCode);
                $parentTR.find(".item-taxtypecode").val(typ.SriCode);
                var taxBase = $parentTR.find(".item-taxbase");
                var taxCode = getValueSafe(repo.text.substring(0, 5));

                if (taxCode > 100) {
                    var baseValue = getValueSafe($("#ReferenceDocumentAmount").val());
                    taxBase.val(baseValue);
                }
                else {
                    var ivaValue = getValueSafe($("#ref_iva_comprobante").val());
                    taxBase.val(ivaValue);
                }

                refresh_totals();
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
                    url: Retention.contributorsUrl,
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
                    var _identificationType = Retention.tiposIdentificacion.find(o => o.Id == data.IdentificationTypeId);
                    $("#IdentificationType").val(_identificationType.SriCode);
                    if (_identificationType.SriCode == "08") {
                        $("#headSujetoRetenido").show();
                    }
                    else {
                        $("#headSujetoRetenido").hide();
                    }
                    ///Limpiamos los campos del documento sustento    
                    refreshControls();
                    //$("#ref_numero_comprobante").val('');
                    //$("#ref_autorizacion_comprobante").val('');
                    //$("#ref_fecha_documento").val('');
                    const $select = $("#inputDocumentId");
                    $select.empty();

                    /// Validad si detalle de la retencion
                    if (itemsCount > 0) {
                        configure_details();
                    }

                    /// Validad si hay informacion adicional para agregar o modificar (dirección, teléfono, email)
                    validate_additional(data)

                    /// Cargo la informacion en los elementos
                    $("#headDocument").removeClass("kt-hide");
                    $("#headDocument").fadeIn();

                    $("#headSustento").removeClass("kt-hide");
                    $("#headSustento").fadeIn();

                    ///cargar los documentos del cliente
                    startDocumentSelector(data.Identification);
                }

                return repo.text;
            }
            else if (repo.id !== "") {

                /// Cargo la informacion en los elementos
                $("#headDocument").removeClass("kt-hide");
                $("#headDocument").fadeIn();

                $("#headSustento").removeClass("kt-hide");
                $("#headSustento").fadeIn();

                initialized = true;

                ///cargar los documentos del cliente
                startDocumentSelector(repo.id);
            }
            else {

                $("#headDocument").addClass("kt-hide");
                $("#headDocument").fadeOut();

                $("#headSustento").addClass("kt-hide");
                $("#headSustento").fadeIn();

                if (typeof agregarAditional !== 'undefined') {
                    add_additional("Direccion", "");
                    add_additional("Email", "");
                    add_additional("Telefono", "");
                }
                refreshControls();
            }

            return repo.text;

        },

        validate_additional = function (data) {

            var $RetentionId = $("#Id");
            if ($RetentionId.length > 0 && $RetentionId.val() > 0) {
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
                        //cargarDocumento(repo);
                    }
                    else if (repo && repo.id == -1) {
                        enableDocSoporteFisico();
                    }
                    else {
                        refreshControls();
                        editingEnableDocSoporte();
                    }
                }
            }
        },

        cargarDocumento = function (doc) {
            if (doc) {
                editingEnableDocSoporte();
                refreshControls();
                var subTotal = 0.00;
                var subTotalexempt = 0.00;
                var subTotalnosubjeto = 0.00;
                var subTotaIva = 0.00;
                var ivaDoc = 0.00;
                //var date = doc.data;
                $("#ReferenceDocumentId").val(doc.pk);
                $("#ref_tipo_comprobante").val(doc.codTypeDoc);
                $("#ref_numero_comprobante").val(doc.sequence);
                $("#ref_autorizacion_comprobante").val(doc.authorizationNumber);
                $("#ref_fecha_documento").val(doc.date);
                if (getValueSafe(doc.subTotal15) > 0) {
                    subTotaIva = getValueSafe(doc.subTotal15);
                    doc.subTotal12 = "$ 0.00";
                } else {
                    subTotaIva = getValueSafe(doc.subTotal15);
                }
                if (doc.subTotal == null) {
                    subTotal = getValueSafe(doc.subTotal0) + getValueSafe(doc.subTotal12) + getValueSafe(doc.subTotal15);
                }
                else {
                    subTotal = getValueSafe(doc.subTotal);
                }
                if (getValueSafe(doc.total) > 0 && getValueSafe(doc.subTotal12) > 0) {

                    ivaDoc = getValueSafe(doc.total) - getValueSafe(doc.subTotal12);
                }
                else {
                    ivaDoc = doc.iva;
                }
                $("#ref_total_comprobante").val(doc.total);
                $("#ReferenceDocumentAmount").val(subTotal);
                $("#ref_iva_comprobante").val(ivaDoc);
                $(".ref-exempt-comprobante").val(subTotalexempt);
                $(".ref-nosubjeto-comprobante").val(subTotalnosubjeto);
                $(".ref-base0-comprobante").val(doc.subTotal0);
                $(".ref-base12-comprobante").val(subTotaIva);
                valor_original = getValueSafe(doc.total);
                refresh_totals();
                validarTotalTaxDocSoporte(doc);
                paymetTypeDocSoporte(doc);
                /// Validad si detalle de la retencion
                if (itemsCount > 0) {
                    configure_details();
                }
            }
            else {
                valor_original = 0.00;
                editingEnableDocSoporte();
                refreshControls();
            }
        },

        cargarDocumentoXml = function (doc) {
            if (doc) {
                var clientRuc = $("#Identification").val();
                if (doc.IdentificationNumber != clientRuc) {
                    selectedDocument = false;
                    Swal.fire("Validación", "El ruc del cliente seleccionado es diferente al ruc del xml del documento sustento adjuntado, por favor verifique.", "warning");
                    return;
                }
                editingEnableDocSoporte();
                refreshControls();
                ///----------
                var _subTotalIva = 0
                var _subTotal0 = 0;
                var _subTotalexempt = 0;
                var _subTotalnosubjeto = 0;
                var _valorIva = 0;
                ///----------
                var subTotal = 0.00;
                var subTotalexempt = 0.00;
                var subTotalnosubjeto = 0.00;
                $("#ReferenceDocumentId").val(doc.Pk);
                $("#ref_tipo_comprobante").val(doc.CodTypeDoc);
                $("#ref_numero_comprobante").val(doc.Sequence);
                $("#ref_autorizacion_comprobante").val(doc.AuthorizationNumber);
                $("#ref_fecha_documento").val(doc.Date);
                if (doc.SubTotal != null) {
                    subTotal = getValueSafe(doc.SubTotal);
                    subTotal = getValueSafe(doc.SubTotal0) + getValueSafe(doc.SubTotal12);
                }

                if (doc.TotalTax) {
                    for (let i = 0; i < doc.TotalTax.length; i++) {
                        var _code = doc.TotalTax[i].PercentageTaxCode;
                        switch (_code) {
                            case "0":
                                _subTotal0 = doc.TotalTax[i].TaxableBase;
                                break;
                            case "2":
                                _subTotalIva = doc.TotalTax[i].TaxableBase;
                                _valorIva = doc.TotalTax[i].TaxValue;
                                break;
                            case "4":
                                _subTotalIva = doc.TotalTax[i].TaxableBase;
                                _valorIva = doc.TotalTax[i].TaxValue;
                                break;
                            case "5":
                                _subTotalIva = doc.TotalTax[i].TaxableBase;
                                _valorIva = doc.TotalTax[i].TaxValue;
                                break;
                            case "6":
                                _subTotalnosubjeto = doc.TotalTax[i].TaxableBase;
                                break;
                            case "7":
                                _subTotalexempt = doc.TotalTax[i].TaxableBase;
                                break;
                            case "8":
                                _subTotalIva = doc.TotalTax[i].TaxableBase;
                                _valorIva = doc.TotalTax[i].TaxValue;
                                break;
                        }
                    }
                }
                if (subTotal == 0) {
                    subTotal = _subTotalIva + _subTotal0 + _subTotalexempt + _subTotalnosubjeto;
                    if (subTotal == 0)
                        subTotal = getValueSafe(doc.Total)
                }
                $("#ref_total_comprobante").val(doc.Total);
                $("#ReferenceDocumentAmount").val(subTotal);
                $(".ref-exempt-comprobante").val(_subTotalexempt);
                $(".ref-nosubjeto-comprobante").val(_subTotalnosubjeto);
                $(".ref-base0-comprobante").val(_subTotal0);
                $(".ref-base12-comprobante").val(_subTotalIva);
                $("#ref_iva_comprobante").val(_valorIva);
                valor_original = getValueSafe(doc.Total);
                refresh_totals();
                /// Validad si detalle de la retencion
                if (itemsCount > 0) {
                    configure_details();
                }

                // Total de impuestos del documento sustento
                /* var $totalTaxes = [];*/
                var $totalTaxes = $(".totalTaxes-items");
                $totalTaxes.empty();
                //se registran los totales de los impuestos
                if (doc.TotalTax) {
                    if (doc.TotalTax.length > 0) {
                        for (let i = 0; i < doc.TotalTax.length; i++) {
                            var $totalTax = doc.TotalTax[i];
                            var mackup = '<input id="item' + (i + 1) + '"  name="ReferenceDocumentTotalTax.Index" value="' + (i + 1) + '" type="hidden"></input>';
                            for (const [key, value] of Object.entries($totalTax)) {
                                mackup += `<input  name="ReferenceDocumentTotalTax[${i + 1}].${key}" value="${value}" type="hidden" />`;
                                /*$totalTaxes.push({ name: `TotalTax[${i + 1}].${key}`, value: `'${value}'` });*/
                            }
                            $totalTaxes.append(mackup);
                            mackup = "";
                        }
                    }
                }

                // Metodos de pago del documento sustento
                var $paymentType = [];
                var $paymentType = $(".payment-type-items");
                $paymentType.empty();
                if (doc.PaymentTypes != null) {
                    for (let i = 0; i < doc.PaymentTypes.length; i++) {
                        var obj = doc.PaymentTypes[i];
                        var mackup = '<input id="item' + (i + 1) + '"  name="ReferenceDocumentPayments.Index" value="' + (i + 1) + '" type="hidden"></input>';
                        mackup += `<input  name="ReferenceDocumentPayments[${i + 1}].PaymentMethodCode" value="${obj.Code}" type="hidden" />`;
                        mackup += `<input  name="ReferenceDocumentPayments[${i + 1}].Term" value="${obj.Term}" type="hidden" />`;
                        mackup += `<input  name="ReferenceDocumentPayments[${i + 1}].TimeUnit" value="${obj.UnitTime}" type="hidden" />`;
                        mackup += `<input  name="ReferenceDocumentPayments[${i + 1}].Total" value="${obj.Total}" type="hidden" />`;
                        $paymentType.append(mackup);
                        mackup = "";
                    }
                }
            }
            else {
                valor_original = 0.00;
                refreshControls();
                editingEnableDocSoporte();
            }
        },

        editingEnableDocSoporte = function (editing = true) {
            $("#ref_tipo_comprobante").prop({ disabled: editing });
            $("#ref_numero_comprobante").prop({ readonly: editing });
            $("#ref_autorizacion_comprobante").prop('readonly', editing);
            $("#ref_fecha_documento").prop({ disabled: editing });
            $("#ref_total_comprobante").prop('readonly', editing);
            $("#ref_iva_comprobante").prop('readonly', editing);
            $(".ref-exempt-comprobante").prop('readonly', editing);
            $(".ref-nosubjeto-comprobante").prop('readonly', editing);
            $(".ref-base0-comprobante").prop('readonly', editing);
            $(".ref-base12-comprobante").prop('readonly', editing);
            enableDisabledColor(editing);

        },

        enableDisabledColor = function (_disable) {
            var color = ''
            if (_disable) {
                color = '#DEDEDE'
            }
            $("#ref_tipo_comprobante").css("background-color", color);
            $("#ref_numero_comprobante").css("background-color", color);
            $("#ref_autorizacion_comprobante").css("background-color", color);
            $("#ref_fecha_documento").css("background-color", color);
            $("#ref_total_comprobante").css("background-color", color);
            $("#ref_iva_comprobante").css("background-color", color);
            $(".ref-exempt-comprobante").css("background-color", color);
            $(".ref-nosubjeto-comprobante").css('background-color', color);
            $(".ref-base0-comprobante").css('background-color', color);
            $(".ref-base12-comprobante").css('background-color', color);
        },

        enableDocSoporteFisico = function () {
            refreshControls();
            editingEnableDocSoporte(false);
            $("#ReferenceDocumentId").val("-1");
            var accordionPayment = $("#accordionPayment");
            accordionPayment.show()
            paymentCount = $("tr.payment-item").length;
            if (paymentCount == 0) {
                add_payment();
                refresh_totals();
            }

        },

        refreshControls = function () {
            $("#ReferenceDocumentId").val("");
            $("#ref_tipo_comprobante").val("");
            $("#ref_numero_comprobante").val("");
            $("#ref_autorizacion_comprobante").val("");
            $("#ref_fecha_documento").val("");
            $("#ref_total_comprobante").val("0.00");
            $("#ref_iva_comprobante").val("0.00");
            $("#ReferenceDocumentAmount").val("0.00");
            $(".ref-exempt-comprobante").val("0.00");
            $(".ref-nosubjeto-comprobante").val("0.00");
            $(".ref-base0-comprobante").val("0.00");
            $(".ref-base12-comprobante").val("0.00");
            var accordionPayment = $("#accordionPayment");
            accordionPayment.hide();
            var $paymentType = $(".payment-type-items");
            $paymentType.empty();
            var $paymentItems = $("#payment-items");
            $paymentItems.empty();
            var $totalTaxes = $(".totalTaxes-items");
            $totalTaxes.empty();
            valor_original = 0.00;
            refresh_detail();
            refresh_totals();
        },

        refresh_detail = function () {
            var itemObj = $(".product-item"); // Lista de Detalles <TR>

            // Analizo registro x registro
            for (var i = 0; i < itemObj.length; i++) {
                var itemInput = $(itemObj[i]);
                var taxSelect = itemInput.find(".item-taxrate")[0];
                if (typeof itemInput !== 'undefined' && itemInput.length > 0) {
                    var objBase = itemInput.find(".item-taxbase")[0];
                    var base = getValueSafe(0.00);
                    setValueSafe(objBase, base);
                }
            }
        },

        //delete_item = function () {
        //    $(this).parents("tr").remove();
        //},

        change_payment = function () {
            refresh_totals();
        },

        getUrlSearchDocument = function (id) {
            return `${Retention.documentUrl}/${id}`;
        },

        getUrlGetDocument = function (id) {
            return `${Retention.documentbyIdUrl}/${id}`;
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

        validarPhysicalDocSoporte = function () {
            var msj = "";
            var objdocSopTotal = $("#ref_total_comprobante")[0];
            var objdocSopIva = $("#ref_iva_comprobante")[0];
            var objdocSopBase12 = $("#ref_base12_comprobante")[0];
            var objdocSopBase0 = $("#ref_base0_comprobante")[0];
            var objdocSopNosubjeto = $("#ref_nosubjeto_comprobante")[0];
            var objdocSopExempt = $("#ref_exempt_comprobante")[0];

            docSopTotal = getValueSafe(objdocSopTotal.value);
            docSopIva = getValueSafe(objdocSopIva.value);
            docSopBase12 = getValueSafe(objdocSopBase12.value);
            docSopBase0 = getValueSafe(objdocSopBase0.value);
            docSopNosubjeto = getValueSafe(objdocSopNosubjeto.value);
            docSopExempt = getValueSafe(objdocSopExempt.value);

            docSopSubTotal = getValueSafe((docSopBase12 + docSopBase0 + docSopNosubjeto + docSopExempt), 2);
            tempTotal = getValueSafe((docSopSubTotal + docSopIva), 2);
            if (docSopTotal != tempTotal) {
                return "La suma de los subtotales + iva. debe ser igual a total del documento sustento!";
            }
            if (docSopIva > 0) {
                if (docSopBase12 == 0) {
                    return "Por favor debe ingresar el valor subtotal base Iva!";
                }
                //Realizar la validación para todos los ivas 12,15 y 8
                //var taxRat12 = Retention.taxes.find(o => o.SriCode == '2');
                //tempBase12 = getValueSafe((docSopBase12 * (taxRat12.RateValue / 100)), 2)
                //if (tempBase12 != docSopIva)
                //{
                //    return "Por favor verificar que el subtotal base 12% debe concordar con el valor del iva del documento sustento!";
                //}
            }
            if (docSopBase12 > 0) {
                if (docSopIva == 0) {
                    return "Por favor debe ingresar el valor del iva del documento sustento!";
                }
            }

            // Busco dentro de los items de la lista
            var itemObj = $(".payment-item");
            var totPagar = 0;
            for (var i = 0; i < itemObj.length; i++) {
                var vlr = getValueSafe($(itemObj[i]).find("input.payment-total")[0].value);
                if (vlr <= 0) {
                    return "Por favor ingresa la forma de pago del documento sustento";
                }

                totPagar += vlr;
            }
            if (totPagar > 0) {
                if (totPagar != docSopTotal) {
                    return "El valor pagado no puede ser mayor o menor al total del del documento sustento";
                }
            }
            // total de los impuestos del toducmento sustento
            var model = {
                subTotal0: docSopBase0,
                subTotal12: docSopBase12,
                subTotalexempt: docSopExempt,
                subTotalnosubjeto: docSopNosubjeto,
                docSopIva: docSopIva
            };
            handleTotalTaxPhysicalDocSoporte(model);

            return null;
        },

        ValidarDocument = function () {

            var objestablishment = $(".select-establishment")[0];
            if (objestablishment.value.length <= 0) {
                return valError("Por favor seleccione un establecimiento!", objestablishment);
            }

            var objtipoComprobante = $("#ref_tipo_comprobante")[0];
            if (objtipoComprobante.value.length <= 0) {
                return valError("Por favor seleccione un tipo de comprobante!", objtipoComprobante);
            }

            var objcontributor = $("#ContributorId");
            var contributor_id = objcontributor.val();
            if (contributor_id == null || contributor_id == "") {
                return valError("Por favor seleccione un cliente!", objcontributor);
            }

            var objidentificationType = $("#IdentificationType");            
            if (objidentificationType.val() == "08") {
                var objretentionObjectType = $("#RetentionObjectType")[0];
                if (objretentionObjectType.value.length <= 0) {
                    return valError("Por favor seleccione un tipo sujeto retenido!", objretentionObjectType);
                }
            }

            var reffechadocumento = $("#ref_fecha_documento");
            if (reffechadocumento.val() == null || reffechadocumento.val() == "") {
                return valError("Por favor ingrese la fecha  del documento sustento!");
            }

            var refDocumentNumber = $("#ref_numero_comprobante").val();
            if (refDocumentNumber == null || refDocumentNumber == "") {
                return valError("Por favor ingrese un número de documento valido!");
            }

            var refDocumentAuth = $("#ref_autorizacion_comprobante").val();
            if (refDocumentAuth == null || refDocumentAuth == "") {
                return valError("Por favor ingrese el número de autorización del comprobante sustento de la retención!");
            }
            else if (refDocumentAuth.length != 49 && refDocumentAuth.length != 10 && refDocumentAuth.length != 37) {
                return valError("El número de autorización del comprobante sustento de la retención debe ser de 10, 37 o 49 digitos");
            }
            // por el momento se maneja esta validación
            //if (refDocumentAuth == null || refDocumentAuth == "") {
            //    return valError("Por favor ingrese el número de autorización del comprobante sustento de la retención!");
            //}

            var sutentoComprobante = $("#SupportCode").val();
            if (sutentoComprobante == null || sutentoComprobante == "") {
                return valError("Por favor debe seleccionar el tipo de sustento del comprobante!");
            }

            //validaciones cunado el documento sustento es fisico
            var refdocumentoId = $("#ReferenceDocumentId")[0];
            if (refdocumentoId.value == "-1") {
                var resutl = validarPhysicalDocSoporte();
                if (resutl !== null) {
                    return valError(resutl);
                }
            }

            // Busco dentro de los items de la lista
            var itemObj = $(".product-item");
            if (itemObj.length == 0) {
                return valError("Debe seleccionar por lo menos un impuesto!");
            }

            var objtotal = $("#FiscalAmount")[0];
            var total = objtotal.value;

            var objReason = $("#Reason");
            var documentReason = objReason.val();
            if (documentReason == null || documentReason == '') {
                return valError("El motivo o explicación de la retención esta vacio!", objReason);
            }


            if (!retencionInformativa && total == 0) {
                Swal.fire({
                    title: "Retención",
                    text: "EL TOTAL DE LA RETENCIÓN ES CERO. \r\n ¿Esta usted seguro de generar esta Retención como INFORMATIVA? ",
                    type: "warning",
                    showCancelButton: true
                }).then(function (confirm) {

                    // BAJO RESPONSABILIDAD DE LOS USUARIOS SE HABILITA LA RETENCION INFORMATIVA
                    if (confirm.value) {
                        console.log("GENERANDO RETENCIÓN INFORMATIVA: Con valores en cero.");
                        retencionInformativa = true;
                        save_handler();
                    }
                });

                return false
            }

            return true;

        },

        modalDocumentSupport = function () {
            resetControlesModal();
            $("#modal_docSupport").modal("show");
        },

        selectDocumnetSupport = function () {
            $(".file-document-xml").click();
        },

        importDocumentSupport = function () {
            var ext = '';
            $nameFile = '';
            $fileRaw = '';
            var $docSupport = $(this)[0];
            if ($docSupport.files.length > 0) {
                var styleOK = "fa fa-check text-success kt-icon-lg";
                var styleError = "fa fa-times text-danger kt-icon-lg";
                var inputElement = $("#file_import");
                var labelElement = $("#size_file_import");
                var iSize = ($docSupport.files[0].size / 1024);
                var namefile = $docSupport.files[0].name;
                ext = namefile.split('.');
                ext = ext[ext.length - 1];
                var info = "Archivo: " + namefile + "</br>" + "Tamaño de archivo:";

                if (ext === "xml") {
                    labelElement.html(info + ' ' + iSize + "Mb");
                    labelElement.attr('style', 'color: #9c9c9c');
                    $nameFile = namefile;
                    $fileRaw = $docSupport.files[0];

                }
                else {
                    Swal.fire("Oops!", 'El archivo debe ser extension xml', "error");
                    $docSupport.value = '';
                    $docSupport.files[0].name = '';
                    return false;
                }

                $(".subir-documnet-support").show();
                inputElement.removeClass(styleError);
                inputElement.addClass(styleOK);
            }
        },

        resetControlesModal = function () {
            var file = $(".file-document-xml");
            var inputElement = $("#file_import");
            var labelElement = $("#size_file_import");
            file.val("");
            if (file[0].files.length > 0) {
                file[0].files[0].name = '';
            };
            labelElement.html("");
            inputElement.removeAttr("class");
            $(".subir-documnet-support").hide();
        },

        handleDocumentSupport = function () {
            if (!$nameFile || !$fileRaw) {
                // No se puede procesar;
                return;
            }
            var $data = new FormData();
            $data.append("Name", $nameFile);
            $data.append("XmlRaw", $fileRaw);
            showLoader();
            $.ajax({
                url: Retention.docSupportXmlUrl,
                type: "POST",
                data: $data,
                success: function (data) {
                    if (data.Pk != null) {
                        cargarDocumentoXml(data);
                    }
                    else {
                        toastr.warning("Hubo un error al procesar el XML del documento sustento!");
                    }
                },
                error: function (data) {
                    console.log(data);
                    var message = "Hubo un error al procesar el XML del documento sustento!";

                },
                cache: false,
                contentType: false,
                processData: false
            }).fail(function (error, data, obj) {
                toastr.warning("Hubo un error al procesar el documento sustento");
            }).always(function () {
                hideLoader();
                resetControlesModal();
                $("#modal_docSupport").modal("hide");
            });


        },

        validarTotalTaxDocSoporte = function (data) {

            /* var $totalTaxes = [];*/
            var subTotalexempt = 0;
            var subTotalnosubjeto = 0
            var totalTax = [];
            var $totalTaxes = $(".totalTaxes-items");
            $totalTaxes.empty();
            // validamos si tiene iva del 0 porciento
            var subTotal0 = getValueSafe(data.subTotal0, 2);
            if (subTotal0 > 0) {
                var taxRate0 = Retention.taxes.find(o => o.SriCode == '0');
                var _subTotal0 = {
                    'TaxCode': '2',
                    'PercentageTaxCode': taxRate0.SriCode,
                    'TaxRate': taxRate0.RateValue,
                    'TaxableBase': subTotal0,
                    'TaxValue': getValueSafe((subTotal0 * (taxRate0.RateValue / 100)), 2)
                }
                totalTax.push(_subTotal0);
            }
            // validamos si tiene iva del 12 porciento
            var subTotal12 = getValueSafe(data.subTotal12, 2);
            if (subTotal12 > 0) {
                var taxRate12 = Retention.taxes.find(o => o.SriCode == '2');
                var _subTotal12 = {
                    'TaxCode': '2',
                    'PercentageTaxCode': taxRate12.SriCode,
                    'TaxRate': taxRate12.RateValue,
                    'TaxableBase': subTotal12,
                    'TaxValue': getValueSafe((subTotal12 * (taxRate12.RateValue / 100)), 2)
                }
                totalTax.push(_subTotal12);
            }
            // validamos si tiene iva del 12 porciento
            var subTotal15 = getValueSafe(data.subTotal15, 2);
            if (subTotal15 > 0) {
                var taxRate15 = Retention.taxes.find(o => o.SriCode == '4');
                var _subTotal15 = {
                    'TaxCode': '2',
                    'PercentageTaxCode': taxRate15.SriCode,
                    'TaxRate': taxRate15.RateValue,
                    'TaxableBase': subTotal15,
                    'TaxValue': getValueSafe((subTotal15 * (taxRate15.RateValue / 100)), 2)
                }
                totalTax.push(_subTotal15);
            }
            // validamos si tiene excento
            if (subTotalexempt > 0) {
                var taxExempt = Retention.taxes.find(o => o.SriCode == '7');
                var _subTotalExempt = {
                    'TaxCode': '2',
                    'PercentageTaxCode': taxExempt.SriCode,
                    'TaxRate': taxExempt.RateValue,
                    'TaxableBase': subTotalexempt,
                    'TaxValue': getValueSafe(0, 2)
                }
                _totalTax.push(_subTotalExempt);
            }
            // validamos si no subjeto
            if (subTotalnosubjeto > 0) {
                var nosubjeto = Retention.taxes.find(o => o.SriCode == '6');
                var _subTnosubjeto = {
                    'TaxCode': '2',
                    'PercentageTaxCode': nosubjeto.SriCode,
                    'TaxRate': nosubjeto.RateValue,
                    'TaxableBase': subTotalnosubjeto,
                    'TaxValue': getValueSafe(0, 2)
                }
                _totalTax.push(_subTnosubjeto);
            }
            if (totalTax.length > 0) {
                for (let i = 0; i < totalTax.length; i++) {
                    var obj = totalTax[i];
                    var mackup = '<input id="item' + (i + 1) + '"  name="ReferenceDocumentTotalTax.Index" value="' + (i + 1) + '" type="hidden"></input>';
                    for (const [key, value] of Object.entries(obj)) {
                        mackup += `<input  name="ReferenceDocumentTotalTax[${i + 1}].${key}" value="${value}" type="hidden" />`;
                        /*$totalTaxes.push({ name: `TotalTax[${i + 1}].${key}`, value: `'${value}'` });*/
                    }
                    $totalTaxes.append(mackup);
                    mackup = "";
                }
            }

        },

        handleTotalTaxPhysicalDocSoporte = function (data) {

            /* var $totalTaxes = [];*/
            var totalTax = [];
            var $totalTaxes = $(".totalTaxes-items");
            $totalTaxes.empty();
            // validamos si tiene iva del 0 porciento
            var subTotal0 = getValueSafe(data.subTotal0, 2);
            if (subTotal0 > 0) {
                var taxRate0 = Retention.taxes.find(o => o.SriCode == '0');
                var _subTotal0 = {
                    'TaxCode': '2',
                    'PercentageTaxCode': taxRate0.SriCode,
                    'TaxRate': taxRate0.RateValue,
                    'TaxableBase': subTotal0,
                    'TaxValue': getValueSafe((subTotal0 * (taxRate0.RateValue / 100)), 2)

                }
                totalTax.push(_subTotal0);
            }
            // validamos si tiene iva del 12 porciento            
            var subTotal12 = getValueSafe(data.subTotal12, 2);
            if (subTotal12 > 0) {
                var rateValue = getValueSafe((data.docSopIva * 100) / data.subTotal12, 2)
                var code = '4';
                if (rateValue <= 5 && rateValue >= 4.95) {
                    code = '5'
                }
                else if (rateValue <= 8 && rateValue >= 7.95) {
                    code = '8'
                }
                var taxRate12 = Retention.taxes.find(o => o.SriCode == code);
                var _subTotal12 = {
                    'TaxCode': '2',
                    'PercentageTaxCode': taxRate12.SriCode,
                    'TaxRate': taxRate12.RateValue,
                    'TaxableBase': subTotal12,
                    'TaxValue': getValueSafe((subTotal12 * (taxRate12.RateValue / 100)), 2)
                }
                totalTax.push(_subTotal12);
            }
            // validamos si tiene excento
            var subTotalexempt = getValueSafe(data.subTotalexempt, 2);
            if (subTotalexempt > 0) {
                var taxExempt = Retention.taxes.find(o => o.SriCode == '7');
                var _subTotalExempt = {
                    'TaxCode': '2',
                    'PercentageTaxCode': taxExempt.SriCode,
                    'TaxRate': taxExempt.RateValue,
                    'TaxableBase': subTotalexempt,
                    'TaxValue': getValueSafe(0, 2)
                }
                totalTax.push(_subTotalExempt);
            }
            // validamos si no subjeto
            var subTotalnosubjeto = getValueSafe(data.subTotalnosubjeto, 2);
            if (subTotalnosubjeto > 0) {
                var nosubjeto = Retention.taxes.find(o => o.SriCode == '6');
                var _subTnosubjeto = {
                    'TaxCode': '2',
                    'PercentageTaxCode': nosubjeto.SriCode,
                    'TaxRate': nosubjeto.RateValue,
                    'TaxableBase': subTotalnosubjeto,
                    'TaxValue': getValueSafe(0, 2)
                }
                totalTax.push(_subTnosubjeto);
            }
            if (totalTax.length > 0) {
                for (let i = 0; i < totalTax.length; i++) {
                    var obj = totalTax[i];
                    var mackup = '<input id="item' + (i + 1) + '"  name="ReferenceDocumentTotalTax.Index" value="' + (i + 1) + '" type="hidden"></input>';
                    for (const [key, value] of Object.entries(obj)) {
                        mackup += `<input  name="ReferenceDocumentTotalTax[${i + 1}].${key}" value="${value}" type="hidden" />`;
                        /*$totalTaxes.push({ name: `TotalTax[${i + 1}].${key}`, value: `'${value}'` });*/
                    }
                    $totalTaxes.append(mackup);
                    mackup = "";
                }
            }

        },

        paymetTypeDocSoporte = function (data) {

            // Metodos de pago del documento sustento
            var $paymentType = [];
            var $paymentType = $(".payment-type-items");
            $paymentType.empty();
            if (data.PaymentTypes != null) {
                for (let i = 0; i < data.PaymentTypes.length; i++) {
                    var obj = data.PaymentTypes[i];
                    var mackup = '<input id="item' + (i + 1) + '"  name="ReferenceDocumentPayments.Index" value="' + (i + 1) + '" type="hidden"></input>';
                    mackup += `<input  name="ReferenceDocumentPayments[${i + 1}].PaymentMethodCode" value="${obj.code}" type="hidden" />`;
                    mackup += `<input  name="ReferenceDocumentPayments[${i + 1}].Term" value="${obj.term}" type="hidden" />`;
                    mackup += `<input  name="ReferenceDocumentPayments[${i + 1}].TimeUnit" value="${obj.unitTime}" type="hidden" />`;
                    mackup += `<input  name="ReferenceDocumentPayments[${i + 1}].Total" value="${obj.total}" type="hidden" />`;
                    $paymentType.append(mackup);
                    mackup = "";
                }
            }

        },

        save_validate = function () {
            if (!ValidarDocument()) {
                return;
            }
            save_handler();
        },

        inputdisabled = function () {
            $("#ref_tipo_comprobante").prop({ disabled: false });
            $("#ref_fecha_documento").prop({ disabled: false });
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
            if (!ValidarDocument()) {
                return;
            }
            inputdisabled();
            var $form = $(".Retention-form");
            showLoader();
            saving = true;
            var $action = $form.attr("action");
            var $data = $form.serializeArray();
            $.post($action, $data, function (result) {
                if (result) {
                    if (result.id > 0) {
                        toastr.success(result.statusText);
                        location.assign(result.Url);
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
        tiposIdentificacion: [],
        tiposIdentificacionSujeto: [],
        tipos:[],
        impuestos:[],
        listaImpuestos: [],
        establishments: [],
        taxes:[],
        init: function () {
            main();
        }
    }
}();