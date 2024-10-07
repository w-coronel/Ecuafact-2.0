var ReferralGuide = function () {

    var initialized = false,
        calculating = 0,
        saving = false,
        itemsCount = 0,
        additionalCount = 0,
        fecha_mes_atras = new Date(new Date() - (30 * 24 * 60 * 60 * 60 * 15)).toLocaleDateString("es"),
        fecha_actual = new Date().toLocaleDateString("es"),
        fecha_mes_fin = new Date(new Date() + (30 * 24 * 60 * 60 * 60 * 15)).toLocaleDateString("es"),       
        fechaFiscfObj,
        retencionInformativa = false,
        valor_original = 0.0,

        main = function () {
           
            var fechaInicioObj = $('input[name="ShippingStartDate"]');
            var fechaFinObj = $('input[name="ShippingEndDate"]');
            var fechaRefObj = $('input[name="ReferenceDocumentDate"]');
                       

            fechaInicioObj.datepicker({
                startDate: fecha_mes_atras,
                endDate: fecha_mes_fin,
                keyboardNavigation: false,
                format: "dd/mm/yyyy",
                language: "es",
                forceParse: false,
                autoclose: true
            });           

            fechaFinObj.datepicker({
                startDate: fecha_mes_atras,
                keyboardNavigation: false,
                format: "dd/mm/yyyy",
                language: "es",
                forceParse: false,
                autoclose: true
            });
            

            fechaRefObj.datepicker({
                startDate: fecha_mes_atras,
                keyboardNavigation: false,
                format: "dd/mm/yyyy",
                language: "es",
                forceParse: false,
                autoclose: true
            });


            itemsCount = $("tr.product-item").length;
            additionalCount = $("tr.additional-item").length;

            $("#CarPlate").on("change", carplateChanged);
            $("#DAU").on("change", dauNumberChanged);
            $("#ref_numero_comprobante").on("change", docNumberChanged);
            $("#ref_autorizacion_comprobante").on("change", refautorizacionChanged);

            $(".invoice-control").on("change", refresh_totals);
            $(".invoice-form").on("submit", function (e) {
                e.preventDefault();
                save_handler();
            });

            $(".add-detail").on("click", add_detail);
            $(".add-additional").on("click", add_additional);
            $(".btn-invoice-save").on("click", save_handler);
            $(".btn-invoice-issue").on("click", save_handler);

            $(".js-documentSelector").on("change", function () { documentListChanged(); });

            $("[name='Term']").on("change", refresh_totals);
            $("[name='TimeUnit']").on("change", refresh_totals);

            $(".select-establishment").on("change", set_establishment);

            $("#ref_numero_comprobante").on("change", docNumberChanged);
            $("#ref_autorizacion_comprobante").on("change", refautorizacionChanged);

            if (itemsCount > 0) {
                configure_details();
            }
            else {
                add_detail();
            }

            startDocumentSelector();
            configure_additionals();
            configure_contrib();
            refresh_totals();
            initialized = true;
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
                var est = ReferralGuide.establishments.find(
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
                //add_additional("EMAIL", $(this).val(), true);
            });

        },

        configure_details = function () {
            configure_product();
            $(".item-quantity").on("change", refresh_totals);
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

        add_detail = function (detalleItem) {

            itemsCount++;
            var itemListId = "selectProducto" + itemsCount;

            var markup =
                '<tr id="item' + itemsCount + '" class="product-item">' +
                '<td data-title="Product" style="width:100%">' +
                '<div class="row">' +

                '<input id="item' + itemsCount + '"  name="Details.Index" value="' + itemsCount + '" type="hidden"></input>' +
                '<input id="mainCode' + itemsCount + '"  name="Details[' + itemsCount + '].MainCode" class="item-maincode"  value="" type="hidden"></input>' +
                '<input id="auxCode' + itemsCount + '"  name="Details[' + itemsCount + '].AuxCode" class="item-auxcode"  value="" type="hidden"></input>' +
                '<input id="desc' + itemsCount + '"  name="Details[' + itemsCount + '].Description" class="item-description"  value="" type="hidden"></input>' +

                '<div class="col-md-12 col-sm-10 col-xs-12" >' +
                '<div class="row">' +

                '<div class="col-md-5 col-sm-12" data-title="Product" style="width:100%">' +
                '<select id = "' + itemListId + '" name="Details[' + itemsCount + '].ProductId"  class="form-control item-productid" data-parent="item' + itemsCount + '" style = "width:100%;"></select>' +
                '</div>' +

                '<div class="col-md-2 col-sm-6 col-xs-12 input-group" data-title="Detalle1" class="numeric" style="padding-bottom:5px;width:100%"> ' +
                '<input id="detname1_' + itemsCount + '" name="Details[' + itemsCount + '].Name1"  class="form-control item-name1 bold" style="text-align:left;background:none;font-size:10px;" placeholder="Nombre1" />' +
                '<input id="detval1_' + itemsCount + '" name="Details[' + itemsCount + '].Value1"  class="form-control item-value1 bold" style="text-align:left;background:none;font-size:10px;" placeholder="Valor1" />' +
                '</div>' +

                '<div class="col-md-2 col-sm-6 col-xs-12 input-group" data-title="Detalle1" class="numeric" style="padding-bottom:5px;width:100%"> ' +
                '<input id="detname2_' + itemsCount + '" name="Details[' + itemsCount + '].Name2"  class="form-control item-name2 bold" style="text-align:left;background:none;font-size:10px;" placeholder="Nombre2" />' +
                '<input id="detval2_' + itemsCount + '" name="Details[' + itemsCount + '].Value2"  class="form-control item-value2 bold" style="text-align:left;background:none;font-size:10px;" placeholder="Valor2" />' +
                '</div>' +

                '<div class="col-md-2 col-sm-6 col-xs-12 input-group" data-title="Detalle1" class="numeric" style="padding-bottom:5px;width:100%"> ' +
                '<input id="detname3_' + itemsCount + '" name="Details[' + itemsCount + '].Name3"  class="form-control item-name3 bold" style="text-align:left;background:none;font-size:10px;" placeholder="Nombre3" />' +
                '<input id="detval3_' + itemsCount + '" name="Details[' + itemsCount + '].Value3"  class="form-control item-value3 bold" style="text-align:left;background:none;font-size:10px;" placeholder="Valor3" />' +
                '</div>' +

                '<div class="col-md-1 col-sm-4 col-xs-12" data-title="Cantidad">' +
                '<input id="qty' + itemsCount + '" name="Details[' + itemsCount + '].Quantity"  class="form-control item-quantity" style="text-align:right; background:none;" type="number" value="1" min="1" placeholder="Cantidad" />' +
                '</div>' +

                '</div>' +
                '</div>' +
                '</div>' +
                '</td>' +

                '<td data-title="">' +
                '<button id="trash' + itemsCount + '" title="Eliminar Registro" type="button" class="delete-item btn " style="width:auto;float:none;" ><span class="la la-trash-o "></span></button>' +
                '</td>' +
                '</tr>';

            var result = $(".product-items").append(markup);
            var obj = $("#" + itemListId);
            $("#qty" + itemsCount).on("change", refresh_totals);
            $("#trash" + itemsCount).on("click", delete_item);

            // Se actualiza la informacion del detalle
            if (typeof detalleItem !== 'undefined') {

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

                    configure_product(obj);
                    refresh_totals();
                }
                else {

                    //cargarItem(obj, detalleItem.MainCode).always(function () {
                    //    configure_product($el);
                    //});

                    configure_product(obj);
                    refresh_totals();
                    if (typeof initialized !== 'undefined' && initialized) {
                        if (itemsCount > 0) {
                            obj.focus();
                            obj.select2("open");
                        }
                    }

                }

                //var $parentTR = $(obj).parents("tr");
                //$parentTR.find(".item-maincode").val(detalleItem.MainCode);
                //$parentTR.find(".item-auxcode").val(detalleItem.AuxCode);
                //$parentTR.find(".item-description").val(detalleItem.Description);
                //$parentTR.find(".item-name1").val(detalleItem.Name1);
                //$parentTR.find(".item-value1").val(detalleItem.Value1);
                //$parentTR.find(".item-name2").val(detalleItem.Name2);
                //$parentTR.find(".item-value2").val(detalleItem.Value2);
                //$parentTR.find(".item-name3").val(detalleItem.Name3);
                //$parentTR.find(".item-value3").val(detalleItem.Value3);

                //refresh_totals();

            }
            else {
                configure_product(obj);
                refresh_totals();
                if (typeof initialized !== 'undefined' && initialized) {
                    if (itemsCount > 0) {
                        obj.focus();
                        obj.select2("open");
                    }
                }
            }
        },

        cargarItem = function ($el, $code) {
            // Primero cargamos los documentos
            return $.ajax({
                type: 'GET',
                url: ReferralGuide.productsUrl
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

                // PROCESO DE CALCULO DE LOS VALORES POR ITEM
                var itemObj = $(".product-item");

                // Busco dentro de los items de la lista
                for (var i = 0; i < itemObj.length; i++) {
                    var itemInput = $(itemObj[i]);
                    if (typeof itemInput !== 'undefined' && itemInput.length > 0) {
                        var qtyObject = itemInput.find(".item-quantity")[0];
                        var qty = getValueSafe(qtyObject.value);

                        if (qty < 0) {
                            qty = 0;
                            show_error("La cantidad del producto no puede menor que 0 ", qtyObject);
                        }
                        var cantidad = getValueSafe(qtyObject.value);
                        if (typeof cantidad != 'undefined' && cantidad > 0) {
                            total = total + cantidad;
                        }
                    }
                }
                // Actualizamos los detalles
                var objvalorTotal = $("#Total");
                var objitemQuantity = document.getElementById("ItemQuantity");
                objitemQuantity.innerHTML = fixUp(total);
                setValueSafe(objvalorTotal, total);

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
            var objAuth = $(this);
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

            Swal.fire("Validación de Datos", msg, "warning");

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
            $obj = $(".js-customerSelector");
           
            $obj.select2({
                ajax: {
                    url: ReferralGuide.contributorsUrl,
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
                            var w = open(ReferralGuide.loginUrl, "_top", "height=770,width=520");
                        }
                        else {
                            //Swal.fire("Error","Hubo un error al obtener los datos","error");
                        }
                    },
                    cache: true
                },
                allowClear: true,
                /*placeholder: 'Por favor seleccione un cliente',*/
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
                return repo.text;
            }
            return repo.text;

        },

        validate_additional = function (data) {

            var $ReferralGuideId = $("#Id");
            if ($ReferralGuideId.length > 0 && $ReferralGuideId.val() != "") {
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
                                    $val.attr('value', data.InvoiceInfo.Address);
                                }
                                else if ($nam.val() === "TELEFONO") {
                                    $val.attr('value', data.InvoiceInfo.Phone);
                                    $phone = 1;
                                }
                                else if ($nam.val() === "EMAIL") {
                                    $val.attr('value', data.Emails);
                                    $emailAddresses = 1;
                                }
                            }
                        }
                    }

                    if ($address === 0)
                        add_additional("DIRECCION", data.InvoiceInfo.Address, true);
                    if ($phone === 0)
                        add_additional("TELEFONO", data.InvoiceInfo.Phone, true);
                    if ($emailAddresses === 0)
                        add_additional("EMAIL", data.Emails, true);
                }
                else {
                    add_additional("DIRECCION", data.InvoiceInfo.Address, true);
                    add_additional("TELEFONO", data.InvoiceInfo.Phone, true);
                    add_additional("EMAIL", data.Emails, true);
                }
            }
            else {
                for (var i = 1; i <= 3; i++) {
                    var $add_name = "DIRECCION";
                    var $add_value = data.InvoiceInfo.Address;
                    if (i === 2) {
                        $add_name = "TELEFONO";
                        $add_value = data.InvoiceInfo.Phone;
                    }
                    else if (i === 3) {
                        $add_name = "EMAIL";
                        $add_value = data.Emails;
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

        //configuración de los productos

        configure_product = function (obj) {
            if (!obj) {
                obj = $(".product-item").find("select[name$='ProductId']");
            }

            obj.select2({
                ajax: {
                    url: ReferralGuide.productsUrl,
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

                $parentTR.find(".item-maincode").val(repo.data.MainCode);
                $parentTR.find(".item-auxcode").val(repo.data.AuxCode);
                $parentTR.find(".item-description").val(repo.data.Description);
                $parentTR.find(".item-name1").val(repo.data.Name1);
                $parentTR.find(".item-value1").val(repo.data.Value1);
                $parentTR.find(".item-name2").val(repo.data.Name2);
                $parentTR.find(".item-value2").val(repo.data.Value2);
                $parentTR.find(".item-name3").val(repo.data.Name3);
                $parentTR.find(".item-value3").val(repo.data.Value3);

                refresh_totals();

                if (typeof initialized !== 'undefined' && initialized) {
                    $parentTR.find(".product-item").focus();
                    //trItem[0].focus();
                }
            }

            return repo.text;

        },

        // lista de documentos
        startDocumentSelector = function () {
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
                    data: function (params) {
                        return {
                            search: params.term || "", // search term
                            page: params.page || 1
                        };
                    },
                    //processResults: function (data, params) {
                    //     parse the results into the format expected by Select2
                    //     since we are using custom formatting functions we do not need to
                    //     alter the remote JSON data, except to indicate that infinite
                    //     scrolling can be used
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
                            Swal.fire("Error", "Su sesión ha caducado. Vuelva a iniciar sesión", "warning");

                            window.location.href = getLoginFormUrl();
                        }
                        else {
                            //Swal.fire("Error","Hubo un error al obtener los datos","error");
                        }
                    },
                    cache: true
                },
                allowClear: true,
                //escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                templateResult: formatDoc,
                templateSelection: formatDocSelection,
                language: {
                    searching: function () {
                        return "Buscando...";
                    }
                },
                placeholder: 'Por favor seleccione un documento'

            });



        },
        formatDoc = function (repo) {
            if (repo) {
                if (repo.id == "-1") {
                    sinDocumentoSustento(false);
                }
                return repo.text;
            }
            return "No hay documentos que mostrar...";
        },
        formatDocSelection = function (repo) {

            if (typeof repo.data !== 'undefined') {
                documentListChanged(repo);
            }
            return formatDoc(repo);
        },
        documentListChanged = function(repo) {
            if (initialized) {

                var docListObj = $(".js-documentSelector");

                if (docListObj && docListObj.select2) {

                    var dataSelect = docListObj.select2("data");

                    if (!repo && dataSelect) {
                        repo = dataSelect[0];
                    }

                    if (repo && repo.id > 0) {
                        var url = getUrlGetDocument(repo.id);
                        habilitatControles();
                        $.get(url, {}, cargarDocumento);
                    }
                    else if (repo && repo.id == -1 && repo.data.DocumentTypeCode == "-1") {
                        sinDocumentoSustento(true);
                    }
                    else {
                        habilitatControles();
                    }
                }
            }
        },
        cargarDocumento = function(doc) {
            if (doc) {
                // Convertir dato obj a fecha:
                var docDateString = doc.IssuedOn.replace("/Date(", "").replace(")/", "");
                var docDate = new Date(parseInt(docDateString)).toLocaleDateString("es");

                //$("#fecha_documento").val(docDate);

                $("#ContributorId").val(doc.ContributorId);               
                $("#IdentificationType").val(doc.ContributorIdentificationType);
                $("#Identification").val(doc.ContributorIdentification);
                $("#ContributorName").val(doc.ContributorName);
                
                $("#ref_fecha_documento").val(doc.InvoiceInfo.IssuedOn);
                $("#ref_numero_comprobante").val(doc.DocumentNumber);               
                $("#ref_tipo_comprobante").val(doc.DocumentTypeCode);
                $("#ref_autorizacion_comprobante").val(doc.AuthorizationNumber);

                $("#ref_phone").val(doc.InvoiceInfo.Phone);
                $("#ref_email").val(doc.Emails);
                $("#ref_address").val(doc.InvoiceInfo.Address);

                //if (docDate) $("#ref_fecha_documento").attr('disabled', 'disabled');
                //if (doc.ContributorIdentificationType) $("#IdentificationType").attr('disabled', 'disabled');
                //if (doc.ContributorIdentification) $("#Identification").attr('readonly', 'readonly');
                //if (doc.ContributorName) $("#ContributorName").attr('readonly', 'readonly');
                //if (doc.DocumentTypeCode) $("#ref_tipo_comprobante").attr('disabled', 'disabled');
                //if (doc.DocumentNumber) $("#ref_numero_comprobante").attr('readonly', 'readonly');
                //if (doc.AuthorizationNumber) $("#ref_autorizacion_comprobante").attr('readonly', 'readonly');

                validate_additional(doc);

                $(".product-items tbody").empty();

                for (var i = 0; i < doc.InvoiceInfo.Details.length; i++) {
                    var item = doc.InvoiceInfo.Details[i];
                    add_detail(item);
                }

                //refresh_totals();
            }
            else {

                $("#fecha_documento").removeAttr('disabled', 'disabled');
                $("#ContributorId").attr('disabled', 'disabled');
                $("#IdentificationType").attr('disabled', 'disabled');
                $("#Identification").attr('disabled', 'disabled');
                $("#ContributorName").attr('disabled', 'disabled');
                $("#ref_comprobante_id").attr('disabled', 'disabled');
                $("#ref_tipo_comprobante").attr('disabled', 'disabled');
                $("#ref_numero_comprobante").attr('disabled', 'disabled');
                $("#ref_fecha_documento").attr('disabled', 'disabled');
                $("#ref_autorizacion_comprobante").attr('disabled', 'disabled');
            }

        },
        sinDocumentoSustento = function (empty) {
            $("#IdentificationType").val('');
            $("#ReferenceDocumentCode").val('');
            $("#ref_fecha_documento").val('');            
            $("#ContributorId").val('');           
            $("#Identification").val('');
            $("#ContributorName").val('');
            $("#ref_comprobante_id").val('');
            $("#ref_tipo_comprobante").val('');
            $("#ref_numero_comprobante").val('');
            $("#ref_autorizacion_comprobante").val('');

            $("#IdentificationType").attr('disabled', 'disabled');
            $("#ReferenceDocumentCode").attr('disabled', 'disabled');
            $("#ContributorId").attr('disabled', 'disabled');
            $("#Identification").attr('disabled', 'disabled');
            $("#ContributorName").attr('disabled', 'disabled');
            $("#ref_comprobante_id").attr('disabled', 'disabled');
            $("#ref_tipo_comprobante").attr('disabled', 'disabled');
            $("#ref_numero_comprobante").attr('disabled', 'disabled');
            $("#ref_autorizacion_comprobante").attr('disabled', 'disabled');
            $("#ref_fecha_documento").attr('disabled', 'disabled');

            $("#IdentificationType").css("background-color",'#d0d1d5');
            $("#ReferenceDocumentCode").css("background-color",'#d0d1d5');
            $("#ContributorId").css("background-color",'#d0d1d5');
            $("#Identification").css("background-color",'#d0d1d5');
            $("#ContributorName").css("background-color",'#d0d1d5');
            $("#ref_comprobante_id").css("background-color",'#d0d1d5');
            $("#ref_tipo_comprobante").css("background-color",'#d0d1d5');
            $("#ref_numero_comprobante").css("background-color",'#d0d1d5');
            $("#ref_autorizacion_comprobante").css("background-color",'#d0d1d5');
            $("#ref_fecha_documento").css("background-color",'#d0d1d5');
            
            if(empty) {
                $(".additional-item tbody").empty();
                $(".product-items tbody").empty();
            }           
        },
        habilitatControles = function () {
            $("#IdentificationType").removeAttr('disabled');
            $("#ReferenceDocumentCode").removeAttr('disabled');
            $("#fecha_documento").removeAttr('disabled');
            $("#ContributorId").removeAttr('disabled');
            $("#IdentificationType").removeAttr('disabled');
            $("#Identification").removeAttr('disabled');
            $("#ContributorName").removeAttr('disabled');
            $("#ref_comprobante_id").removeAttr('disabled');
            $("#ref_tipo_comprobante").removeAttr('disabled');
            $("#ref_numero_comprobante").removeAttr('disabled');
            $("#ref_fecha_documento").removeAttr('disabled');
            $("#ref_autorizacion_comprobante").removeAttr('disabled');

            $("#IdentificationType").css("background-color", '#fff');
            $("#ReferenceDocumentCode").css("background-color", '#fff');
            $("#ContributorId").css("background-color", '#fff');
            $("#Identification").css("background-color", '#fff');
            $("#ContributorName").css("background-color", '#fff');
            $("#ref_comprobante_id").css("background-color", '#fff');
            $("#ref_tipo_comprobante").css("background-color", '#fff');
            $("#ref_numero_comprobante").css("background-color", '#fff');
            $("#ref_autorizacion_comprobante").css("background-color", '#fff');
            $("#ref_fecha_documento").css("background-color", '#fff');

            $("#IdentificationType").val("05");
            $("#ReferenceDocumentCode").val("01");
            $(".additional-item tbody").empty();
            $(".product-items tbody").empty();

            
        },
        dauNumberChanged= function() {
            $dauObj = $(this);
            var text = $dauObj.val();
            $dauObj.val(formatNumDAU(text));
        },
        carplateChanged = function () {
            $plateObj = $(this);
            var text = $plateObj.val();
            $plateObj.val(formatPlate(text));
        },
        getUrlSearchDocument = function (id) {
            return `${ReferralGuide.documentUrl}`;
        },

        getUrlGetDocument = function (id) {
            return `${ReferralGuide.documentbyIdUrl}/${id}`;
        },

        valError = function (msg, obj) {
            hideLoader();

            Swal.fire("Validación de Datos", msg, "warning");

            if (typeof obj !== 'undefined') {
                if (obj !== null && typeof obj.focus !== 'undefined')
                    obj.focus();

                if (obj !== null && typeof obj.blur !== 'undefined')
                    obj.blur();
            }

            return false;
        },

        ValidarDocument = function () {

            var docSustento = true;
            var objIdentification = $("#Identification");
            var identification = objIdentification.val();
            var objContributorName = $("#ContributorName");
            var contributorName = objContributorName.val();
            var refDocumentNumber = $("#ref_numero_comprobante").val();
            var reffechadocumento = $("#ref_fecha_documento");
            var refDocumentAuth = $("#ref_autorizacion_comprobante").val();
            var objestablishment = $(".select-establishment")[0];

            if (objestablishment.value.length <= 0) {
                return valError("Por favor seleccione un establecimiento!", objestablishment);
            }           

            // validamos si selecciono sin documento sustento
            var docListObj = $(".js-documentSelector");
            if (docListObj && docListObj.select2) {
                var dataSelect = docListObj.select2("data");
                var obj = dataSelect[0];
                if (obj && obj.id == -1 && obj.text == "Sin Documento Sustento") {
                    docSustento = false;
                }
            }
            if (docSustento)
            {                
                if (identification == null || identification == "") {
                    return valError("Por favor ingrese No. Identificación del cliente!", objIdentification);
                }               
                if (contributorName == null || contributorName == "") {
                    return valError("Por favor ingrese Nombre/Razón Social del cliente!", objIdentification);
                }                
                if (refDocumentNumber == null || refDocumentNumber == "") {
                    return valError("Por favor ingrese un número de documento valido!");
                }                
                if (reffechadocumento.val() == null || reffechadocumento.val() == "") {
                    return valError("Por favor ingrese la fecha del documento!");
                }                
                if (refDocumentAuth == null || refDocumentAuth == "") {
                    return valError("Por favor ingrese el número de autorización del comprobante sustento de la guia de remisión!");
                }
                else if (refDocumentAuth.length < 49) {
                    return valError("El número de autorización del comprobante sustento de la guia de remisión debe ser de 49 digitos");
                }
            }           

            //valida si hay información del docuemnto soporte
            //if (refDocumentNumber.length > 0) {
            //    if (reffechadocumento.val().length <= 0) {
            //        return valError("Por favor ingrese la fecha  del documento sustento para la guia remisión!");
            //    }
            //    if (refDocumentAuth.length <= 0) {
            //        return valError("Por favor ingrese el número de autorización del comprobante sustento para la guia remisión!");
            //    }
            //    else if (refDocumentAuth.length < 49) {
            //        return valError("El número de autorización del comprobante sustento de la retencón debe ser de 49 digitos");
            //    }
            //}
            //else if (reffechadocumento.val().length > 0) {
            //    if (refDocumentNumber.length <= 0) {
            //        return valError("Por favor ingrese un número de documento de sustento valido para la guia remisión!");
            //    }
            //    if (refDocumentAuth.length <= 0) {
            //        return valError("Por favor ingrese el número de autorización del comprobante sustento para la guia remisión!")
            //    }
            //    else if (refDocumentAuth.length < 49) {
            //        return valError("El número de autorización del comprobante sustento de la retencón debe ser de 49 digitos");
            //    }
            //}
            //else if (refDocumentAuth.length > 0) {
            //    if (refDocumentNumber.length <= 0) {
            //        return valError("Por favor ingrese un número de documento de sustento valido para la guia remisión!");
            //    }
            //    if (reffechadocumento.val().length <= 0) {
            //        return valError("Por favor ingrese la fecha  del documento sustento para la guia remisión!");
            //    }
            //    if (refDocumentAuth.length < 49) {
            //        return valError("El número de autorización del comprobante sustento de la retencón debe ser de 49 digitos");
            //    }
            //}

            var objRecipientId = $("#RecipientId");
            if (objRecipientId.val() == null || objRecipientId.val() == "") {
                return valError("Por favor seleccione un destinatario!", objRecipientId);
            }

            var objDriverId = $("#DriverId");
            if (objDriverId.val() == null || objDriverId.val() == "") {
                return valError("Por favor seleccione un transportista!", objDriverId);
            }

            var ShipmentRoute = $("#ShipmentRoute");
            if (ShipmentRoute.val() == null || ShipmentRoute.val() == "") {
                return valError("Por favor ingrese la ruta de env&iacute;o!");
            }

            var DestinationAddress = $("#DestinationAddress");
            if (DestinationAddress.val() == null || DestinationAddress.val() == "") {
                return valError("Por favor ingrese la direcci&oacute;n de destino!");
            }

            var OriginAddress = $("#OriginAddress");
            if (OriginAddress.val() == null || OriginAddress.val() == "") {
                return valError("Por favor ingrese la direcci&oacute;n de origen!");
            }

            var OricarPlates = $("#CarPlate");
            if (OricarPlates.val() == null || OricarPlates.val() == "") {
                return valError("Por favor ingrese la placa del veh&iacute;culo!");
            }            

            // Busco dentro de los items de la lista
            var itemObj = $(".product-item");
            if (itemObj.length == 0) {
                return valError("Debe seleccionar por lo menos un producto!");
            }

            //Busco dentro de los items de la lista
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

                if (qty < 0) {
                    qty = 0;
                    return valError("La cantidad del producto no puede ser 0", qtyObject);
                }
            }           

            var objReason = $("#Reason");
            var documentReason = objReason.val();
            if (documentReason == null || documentReason == '') {
                return valError("El motivo de la guia de remisión esta vacío!", objReason);
            }

            return true;

        },

        save_validate = function () {
            if (!ValidarDocument()) {
                return;
            }
            save_handler();
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
            var $form = $(".referralGuide-form");
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
        productsUrl: "",
        loginUrl: "/Auth",
        establishments: [],
        init: function () {
            main();
        }
    }
}();