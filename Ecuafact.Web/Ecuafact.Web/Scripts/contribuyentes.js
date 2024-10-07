var Contribuyentes = function () {
    var contribuyente_id,
        contribuyenteData,
        $contrib_form;

        save_contrib = function () {
            var formAction = $contrib_form.attr('action');
            var formData = $contrib_form.serializeArray();

            if (!$contrib_form.valid()) {
                toastr.error("Debe rellenar el formulario con datos válidos!");
                return false;
            }

            var $idType = $("#IdentificationType.contrib").val();
            var $identification = $("#Identification.contrib").val();

            if ($idType == 1 && !validarCedula($identification)) {
                toastr.error("El numero de cédula no es válido!");
                return false;
            }

            showLoader();

            $.ajax({
                url: formAction,
                type: "POST",
                data: formData,
                async: true,
                error: function (data, o, e) {
                    console.log(data);

                    if (data && data.responseText && data.responseText.includes("login")) {
                        Swal.fire("Su Sesion ha Caducado", "Por favor inicie sesion", "error");
                        var w = open(location.href, "_top", "height=770,width=520");
                    }
                    else {
                        toastr.error(data.statusText);
                    }
                },
                success: function (data, o, e) {
                    if (data.id > 0) {
                        set_data(data);
                        toastr.success(data.statusText);
                        hideModal();
                    }
                    else {
                        toastr.error(data.statusText)
                    }
                }
            }).always(function () {
                hideLoader();
            });

            return;

        },

        set_data = function (data) {

            // Si el proceso es una actualizacion debemos modificar los datos de la pantalla
            if (contribuyente_id > 0 && $(".select2-result-repository__meta").length > 0) {
                var customer = data.result;
                $(".select2-result-repository__title").html(customer.Identification + " - " + customer.BussinesName)
                $(".select2-result-repository__description").html("<i class='fa fa-area-chart'></i> Nombre Comercial: " + customer.TradeName)
                $(".select2-result-repository__address").html("<i class='fa fa-map-marker'></i> Direccion: " + customer.Address)
                $(".select2-result-repository__email").html("<i class='fa fa-envelope-o'></i> E-mail:" + customer.EmailAddresses)
                $(".select2-result-repository__phone").html("<i class='fa fa-phone'></i> Telefono:" + customer.Phone)

                if (typeof agregarAditional != 'undefined') {
                    agregarAditional("DIRECCION", customer.Address);
                    agregarAditional("EMAIL", customer.EmailAddresses);
                    agregarAditional("TELEFONO", customer.Phone);
                }
            }

        },

        set_contribtype = function () {
            var objIdentidad = $('#IdentificationTypeId.contrib')[0];

            if (objIdentidad) {
                var tipoIdentidad = objIdentidad[objIdentidad.selectedIndex].text;
                var objRazonSocial = $('#BussinesName.contrib');
                var objNombreComercial = $('#TradeName.contrib');
                var objIdentificacion = $('#Identification.contrib');

                var labRazonSocial = $("#title_razon_social");

                if (tipoIdentidad.includes('RUC')) {
                    $('.contribuyente_nombre_comercial_group').removeClass("kt-hide");
                    objRazonSocial.attr("placeholder", "Razón Social")
                    labRazonSocial.html("<b>Razón Social:</b>");
                    //objIdentificacion.val("");
                    objIdentificacion.attr("type", "number");
                    objIdentificacion.attr("maxlength", "13");
                    objIdentificacion.attr("minlength", "13");
                    objIdentificacion.attr("pattern", "^[0-9]*$");
                    objIdentificacion[0].onkeypress = function () {
                        if (this.value.length >= 13) return false;
                    };

                    //objIdentificacion.inputmask({
                    //    "mask": "9999999999999",
                    //    "placeholder": ""
                    //});

                }
                else {
                    $('.contribuyente_nombre_comercial_group').addClass("kt-hide");

                    //objNombreComercial.val("");
                    if (tipoIdentidad.includes('PASAPORTE') || tipoIdentidad.includes('EXTERIOR')) {
                        objRazonSocial.attr("placeholder", "Nombres Completos")
                        labRazonSocial.html("<b>Nombres y Apellidos:</b>");
                        //objIdentificacion.val("");
                        objIdentificacion.attr("type", "text");                                                
                        objIdentificacion.attr("maxlength", "20");
                        objIdentificacion.attr("minlength", "6");
                        objIdentificacion.attr("pattern", "^[A-Za-z0-9,-]*$");
                        objIdentificacion[0].onkeypress = function () {
                            if (this.value.length >= 20) return false;
                        };

                        //objIdentificacion.inputmask({
                        //    "regex": "^[a-zA-Z0-9]*$",
                        //    "mask": null,
                        //    "placeholder": ""
                        //});
                    }
                    else if (tipoIdentidad.includes('PLACA')) {
                        objRazonSocial.attr("placeholder", "Nombres Completos")
                        labRazonSocial.html("<b>Nombre del Chofer:</b>");

                        //objIdentificacion.attr("type", "text");
                        //objIdentificacion.val("");
                        objIdentificacion.attr("maxlength", "9");
                        objIdentificacion.attr("minlength", "9");
                        objIdentificacion.attr("pattern", "aaa-999[9]");
                        objIdentificacion[0].onkeypress = function () {
                            if (this.value.length >= 9) return false;
                        };

                        //objIdentificacion.inputmask({
                        //    "regex": null,
                        //    "mask": "aaa-999[9]",
                        //    "placeholder": ""
                        //});
                    }
                    else {
                        objRazonSocial.attr("placeholder", "Nombres Completos")
                        labRazonSocial.html("<b>Nombres y Apellidos:</b>");
                        //objIdentificacion.val("");
                        objIdentificacion.attr("type", "number");
                        objIdentificacion.attr("minlength", "10");
                        objIdentificacion.attr("maxlength", "10");
                        objIdentificacion.attr("pattern", "^[0-9]*$");
                        objIdentificacion[0].onkeypress = function () {
                            if (this.value.length >= 10) return false;
                        };

                        //objIdentificacion.inputmask({
                        //    "regex": null,
                        //    "mask": "9999999999",
                        //    "placeholder": ""
                        //});
                    }
                }

             }
        },

        set_contribname = function () {
            var objIdentidad = $('select[name="IdentificationTypeId"]')[0];

            if (objIdentidad) {
                var tipoIdentidad = objIdentidad[objIdentidad.selectedIndex].text;
                var objRazonSocial = $('#BussinesName.contrib');
                var objNombreComercial = $('#TradeName.contrib');

                if (!tipoIdentidad.includes('RUC')) {
                    objNombreComercial.val("-");
                }
            }
        },

        set_contrib = function () {
            var $identification = $("#Identification.contrib").val();
            
            if ($identification) {
                // Solo se aplica en el caso que la identificación sea cedula o ruc:
                if ($identification.length == 10 || $identification.length == 13) {
                    var $token = $("#RefID").val();
                    var $url = $contrib_form.attr("action").replace("ActualizarAsync", 'BuscarAsync');

                    var req = {
                        uid: $identification,
                        token: $token
                    };

                    if ($identification.length == 10 && !validarCedula($identification)) {
                        toastr.error("El numero de cédula no es válido!");
                    }

                    $(".loading-identity").show();

                    $.post($url, req, function (data) {
                        if (data && data.IsSuccess) {
                            // Si esta tratando de modificar mismo registro entonces, no lo recarga.
                            var $id = $("#Id").val();
                            if ($id > 0 && data.Entity.Id == $id) {
                                return;
                            }

                            $("#BussinesName.contrib").val(data.Entity.BussinesName);
                            $("#TradeName.contrib").val(data.Entity.TradeName);
                            $("#Address.contrib").val(data.Entity.Address);
                            $("#ContributorTypeId.contrib").val(data.Entity.ContributorTypeId);

                            $("#Phone.contrib").val(data.Entity.Phone);
                            $("#Phone.contrib").trigger("change input");
                            $("#Phone.contrib").tagsinput()[0].build();

                            $("#EmailAddresses.contrib").val(data.Entity.EmailAddresses);
                            $("#EmailAddresses.contrib").trigger("change input");
                            $("#EmailAddresses.contrib").tagsinput()[0].build();

                            if ($("#Identification.contrib").val() == data.Entity.Identification) {
                                $("#Id.contrib").val(data.Entity.Id);
                                toastr.warning("Hemos encontrado un registro para este contribuyente. Cargando los datos existentes...");
                            }

                            // Se vuelve a validar al contribuyente actual
                            set_contribtype();

                        }
                    }).always(function () {
                        $(".loading-identity").hide();
                    });


                }
            }
        },

        main_handler = function () {
            $contrib_form = $('#contribForm');

            $("#contribForm input").upperText();

            $('#btnsave_contrib').on("click", save_contrib);

            //$('#contribForm').on("submit", function (e) {
            //    e.preventDefault();
            //    save_contrib();
            //    return false;
            //});

            $("#IdentificationTypeId.contrib").on("change", set_contribtype);
            $("#Identification.contrib").on("blur", set_contrib);
            $("#BusinessName.contrib").on("change", set_contribname);
            $("#EmailAddresses.contrib").typeahead();
            $("#Phone.contrib").typeahead();

            set_contribtype();

            $('#Identification.contrib').trigger('focus');
        };

    return {
        Init: function () {
            main_handler();
        }
    };

}();

