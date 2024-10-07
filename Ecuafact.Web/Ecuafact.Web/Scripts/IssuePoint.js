var IssuePoint = function () {
    var issuePoint_id,
        issuePointData,
        $issuePoint_form;
    changing = false,

        save_ssuePoint = function () {
            var formAction = $issuePoint_form.attr('action');
            var formData = $issuePoint_form.serializeArray();

            if (!$issuePoint_form.valid()) {
                toastr.error("Debe rellenar el formulario con datos válidos!");
                return false;
            }

            showLoader();

            $.ajax({
                url: formAction,
                type: "POST",
                data: formData,
                async: true,
                error: function (data, o, e) {

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
                        toastr.success(data.statusText);
                        hideModal();
                    }
                    else {
                        toastr.error(data.message);
                        Swal.fire("Oops!", data.message, "error");
                    }
                }
            }).always(function () {
                hideLoader();
            });

            return;

        },
        carplateChanged = function () {
            $plateObj = $(this);
            var text = $plateObj.val();
            $plateObj.val(formatPlate(text));
        },
        set_Carriertype = function () {
            var objIdentificacion = $('#CarrierRUC.issuePoint');
            objIdentificacion.attr("type", "number");
            objIdentificacion.attr("maxlength", "13");
            objIdentificacion.attr("minlength", "13");
            objIdentificacion.attr("pattern", "^[0-9]*$");
            objIdentificacion[0].onkeypress = function () {
                if (this.value.length >= 13) return false;
            };
        },
        handleLeadingZeros = function () {
            if (changing) {
                return;
            }
            changing = true;
            var $me = $(this);
            if ($me[0].value.length > $me[0].maxLength) {
                $me[0].value = $me[0].value.slice(0, $me[0].maxLength)
            }

            var value = $me.val();
            if (value.length > 0 && value.length < 3) {
                value = value.padStart(3, "0");
                $me.val(value);
            }
            changing = false;
        },
        set_Carrier = function () {
            var $ruc = $("#CarrierRUC.issuePoint").val();
            if ($ruc) {
                // Solo se aplica en el caso que la identificación sea cedula o ruc:
                if ($ruc.length == 13) {                   
                    var $url = $issuePoint_form.attr("action").replace("GuardarPuntoEmiAsync", 'BuscarPuntoEmiAsync');
                    var req = { ruc: $ruc };
                    $(".loading-identity").show();
                    var $btn = $(".btn-verificar-ruc");
                    KTApp.progress($btn);
                    $.post($url, req, function (data) {
                        if (data && data.IsSuccess) {                           
                            var _issuer = data.Entity.IssuerDto;
                            var _subscription = data.Entity.SubscriptionDto;
                            // Si esta tratando de modificar mismo registro entonces, no lo recarga.     
                            if (_subscription.LicenceType.Code == IssuePoint.planPro) {
                                $("#Name.issuePoint").val(_issuer.BussinesName);
                                $("#CarrierPhone.issuePoint").val(_issuer.Phone);
                                $("#CarrierPhone.issuePoint").trigger("change input");
                                $("#CarrierPhone.issuePoint").tagsinput()[0].build();
                                $("#CarrierEmail.issuePoint").val(_issuer.Email);
                                $("#CarrierEmail.issuePoint").trigger("change input");
                                $("#CarrierEmail.issuePoint").tagsinput()[0].build();
                                //var contribuyente = "";
                                //if (data.Entity.IsGeneralRegime) {
                                //    contribuyente = "Régimen General";
                                //}
                                //else if (data.Entity.IsRimpe) {
                                //    contribuyente = "Régimen RIMPE";
                                //}
                                //$("#CarrierContribuyente").val(contribuyente);

                                // Se vuelve a validar al issuer actual
                                set_Carriertype();
                            }
                            else {
                                Swal.fire("Oops!", "El transportista seleccioneado no se puede configurar como punto de emisión por que no cuenta con un plan PRO", "warning");
                                $("#CarrierRUC.issuePoint").val("");
                            }
                        }
                    }).always(function () {
                        KTApp.unprogress($btn);
                        $(".loading-identity").hide();
                    });

                }
            }
        },
        set_contribuyente = function () {
            var pnl = $('.resolution-number');
            var obj = $('.carrier-contribuyente')[0];
            if (obj && obj.value.length > 0) {
                // Obtengo la informacion del punto de emisión
                var type = $(".carrier-contribuyente").val();
                if (type.toLowerCase().includes('contribuyente especial') || type.toLowerCase().includes('agente retención')) {
                    pnl.show();
                    $("#CarrierResolutionNumber").prop("required", "required");
                }
                else {
                    pnl.hide();
                    $("#CarrierResolutionNumber").removeAttr("required");                   
                    $('#CarrierResolutionNumber').val("");
                }
            }
        },
        main_handler = function () {
            $issuePoint_form = $('#issuePointForm');
            $("#issuePointForm input").upperText();
            $issuePoint = $('.issuePoint-Code');
            $issuePoint.on("change", handleLeadingZeros);
            $('#btnSave').on("click", save_ssuePoint);
            $("#CarrierRUC.issuePoint").on("blur", set_Carrier);
            $("#CarrierEmail.issuePoint").typeahead();
            $("#CarrierPhone.issuePoint").typeahead();
            $('#CarrierRUC.issuePoint').trigger('focus');
            $("#CarPlate").on("change", carplateChanged);
            $('.btn-verificar-ruc').on("click", set_Carrier);
            $(".carrier-contribuyente").on("change", set_contribuyente);
            set_Carriertype();

        }
    return {
        planPro: "",
        Init: function () {
            main_handler();
        }
    };

}();
