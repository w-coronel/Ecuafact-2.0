"use strict";

// Class definition
var Emisor = function () {
    // Base elements 
    $('[data-toggle="tooltip"]').tooltip();
    var $validator,
        $ktwizard,
        $wizard = KTUtil.get('kt_wizard_v2'),
        $form = $('#issuer_form'),
        $ruc = $("#RUC"),
        $logo = new KTAvatar("logo"),
        $accounting = $("#IsAccountingRequired"),
        $special = $("#IsSpecialContributor"),
        $resolution = $("#ResolutionNumber"),
        $establishmentCode = $("#establishment_Code"),
        $establishmentCode_item = $(".item-Establishment-Code"),
        $issuePointCode = $(".item-IssuePoint-Code"),
        /*$issuePointCode = $("#issuePoint_Code"),*/
        $agent = $("#IsRetentionAgent"),
        $isRimpe = $("#IsRimpe"),
        $isPopularBusiness = $("#IsPopularBusiness"),
        $agentNumb = $("#AgentResolutionNumber"),
        $microBusines = $("#MicroBusiness"),
        $add_issuePoint = $(".add-issuePoint"),
        $add_establishment = $(".add-detail"),
        $craftsman = $("#IsSkilledCraftsman"),
        $craftsmanNumb = $("#SkilledCraftsmanNumber"),
        changing = false,
        initialized = false,
        issuePointCount = 0,
        establishmentCount = 0,
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

        handleSubmitForm = function (event) {

            if (!$form.validate()) {
                event.preventDefault();
                toastr.warning("Debe completar la informacion requerida!");
                return false;
            }

        },

        handleAccounting = function () {
            var accReq = $accounting.val();
            $special.val(false);

            if (isTrue(accReq)) {
                $special.removeAttr("readonly");
                $special.removeAttr("disabled");
                $special.removeClass("readonly");
                $special.removeClass("disabled");
            }
            else {
                $special.attr("readonly", "readonly");
                $special.attr("disabled", "disabled");
                $special.addClass("readonly");
                $special.addClass("disabled");
            }

            handleSpecialContrib();

        },

        handleSpecialContrib = function () {
            var specReq = $special.val();

            if (isTrue(specReq)) {
                $resolution.removeAttr("readonly");
                $resolution.removeAttr("disabled");
                $resolution.removeClass("readonly");
                $resolution.removeClass("disabled");
            }
            else {
                $resolution.val("");
                $resolution.attr("readonly", "readonly");
                $resolution.attr("disabled", "disabled");
                $resolution.addClass("readonly");
                $resolution.addClass("disabled");
            }            
        },
        handleIsPopularBusiness = function () {
            var isPopularBusiness = $isPopularBusiness.val();
            if (isTrue(isPopularBusiness)) {
                $isRimpe[0].checked = false;
            }            
        },
        handleIsRimpe = function () {
            var isRimpe = $isRimpe.val();
            if (isTrue(isRimpe)) {
                $isPopularBusiness[0].checked = false;
            }
        },
        handleRetentionAgent = function () {
            var retAgen = $agent.val();
            if (isTrue(retAgen)) {
                $agentNumb.removeAttr("readonly");
                $agentNumb.removeAttr("disabled");
                $agentNumb.removeClass("readonly");
                $agentNumb.removeClass("disabled");
            }
            else {
                $agentNumb.val("");
                $agentNumb.attr("readonly", "readonly");
                $agentNumb.attr("disabled", "disabled");
                $agentNumb.addClass("readonly");
                $agentNumb.addClass("disabled");
            }
        },

        handleSkilledCraftsman = function () {
            var retCraftsman = $craftsman.val();
            if (isTrue(retCraftsman)) {
                $craftsmanNumb.removeAttr("readonly");
                $craftsmanNumb.removeAttr("disabled");
                $craftsmanNumb.removeClass("readonly");
                $craftsmanNumb.removeClass("disabled");
            }
            else {
                $craftsmanNumb.val("");
                $craftsmanNumb.attr("readonly", "readonly");
                $craftsmanNumb.attr("disabled", "disabled");
                $craftsmanNumb.addClass("readonly");
                $craftsmanNumb.addClass("disabled");
            }
        },

        initCertificate = function () {
            $("#CertificateFile").on("change", handleValidation);
            $("#CertificatePass").on("change", handleValidation);

        },

        add_establishment = function () {
            var $obj = $(this);
            if (establishmentCount == 0) {
                //establishmentCount = $(".product-items").find('tbody tr').length - 1;
                establishmentCount = $("tr.establishment-item").length - 1;
            }
            var amountEstablishment = $("tr.establishment-item").length + 1;
            if (amountEstablishment > 5) {
                Swal.fire("Establecimientos", "Se permiti un maximo la configuraci&oacute;n de 5 establecimientos!", "warning");
                return
            }

            establishmentCount++;
            var $itemsCount = establishmentCount;
            var $items = 0;

            var markup =
                '<tr id="est_item' + $itemsCount + '" class="establishment-item">' +
                '<td data-title="Product" style="width:100%">' +
                '<div class="row">' +
                '<input id = "estab_Index' + $itemsCount + '"  name = "Establishments.Index" value = "' + $itemsCount + '" type = "hidden" ></input>' +
                '<div class="col-md-3 col-sm-12 col-xs-12" data-title="Codigo" title="C&oacute;digo establecimiento" style="padding-bottom:5px">' +
                '<div class="kt-input-icon kt-input-icon--right">' +
                '<span class="kt-input-icon__icon kt-input-icon__icon--right">' +
                '<span>' +
                '<i class="flaticon-home-2"></i>' +
                '</span>' +
                '</span>' +
                '<input id="estab_code' + $itemsCount + '" name="Establishments[' + $itemsCount + '].Code"  class="form-control placeholder-no-fix item-Establishment-Code" type = "number" min = "1" max = "999" minlength = "3" maxlength = "3" placeholder = "Establecimiento" required data-msg = "C&oacute;digo del establecimiento es requerido" />' +
                '<span class="field-validation-valid text-danger" data-valmsg-for="Establishments[' + $items + '].Code" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>' +
                '<div class="col-md-4 col-sm-12 col-xs-12" data-title="Nombre" style="padding-bottom:5px">' +
                '<div class="col-12 kt-input-icon kt-input-icon--right">' +
                '<span class="kt-input-icon__icon kt-input-icon__icon--right">' +
                '<span>' +
                '<i class="flaticon-profile"></i>' +
                '</span>' +
                '</span>' +
                '<input id="estab_name' + $itemsCount + '" name="Establishments[' + $itemsCount + '].Name" title="Nombre Comercial Establecimiento"  class="form-control item-Establishment-Name"  placeholder = "Nombre comercial" required data-msg = "Nombre comercial del establecimiento es requerido" />' +
                '<span class="field-validation-valid text-danger" data-valmsg-for="Establishments[' + $itemsCount + '].Name" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>' +
                '<div class="col-md-5 col-sm-12 col-xs-12" data-title="Direccion" style="padding-bottom:5px">' +
                '<div class="kt-input-icon kt-input-icon--right">' +
                '<span class="kt-input-icon__icon kt-input-icon__icon--right">' +
                '<span>' +
                '<i class="flaticon-placeholder-3"></i>' +
                '</span>' +
                '</span>' +
                '<input id="estab_address' + $itemsCount + '" name="Establishments[' + $itemsCount + '].Address" title="Direcci&oacute;n Comercial Establecimiento"  class="form-control item-Establishment-address" placeholder = "Direcci&oacute;n" required data-msg = "La direcci&oacute;n del establecimiento es requerido"/>' +
                '<span class="field-validation-valid text-danger" data-valmsg-for="Establishments[' + $itemsCount + '].Address" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>' +
                '<div class="col-md-12 col-sm-12 col-xs-12">' +
                '<div class="accordion accordion-solid accordion-toggle-plus" id="accordIssuePointItem' + $itemsCount + '">' +
                '<div class="card">' +
                '<div class="card-header" id="itemHeading' + $itemsCount + '">' +
                '<div class="card-title collapsed" data-toggle="collapse" data-target="#itemsCollapse' + $itemsCount + '" aria-expanded="false" aria-controls="itemsCollapse' + $itemsCount + ')">' +
                '<br /><i class="flaticon2-add"></i>  Configuraci&oacute;n Puntos de Emisi&oacute;n ' +
                '</div>' +
                '</div>' +
                '<div id="itemsCollapse' + $itemsCount + '" class="collapse" aria-labelledby="itemHeading' + $itemsCount + '" data-parent="#accordIssuePointItem' + $itemsCount + '" style="">' +
                '<div class="card-body">' +
                '<table id="issuePoint-items' + $itemsCount + '" data-content="' + $itemsCount + '" class="table kt-datatable__table table-adicional table-hover table-striped table-condensed cf">' +
                '<thead class="cf card-title kt-datatable__head">' +
                '<tr class="kt-datatable__row">' +
                '<th>' +
                '<div class="row">' +
                '<div class="col-md-6 col-sm-8 col-xs-10" data-title="Codigo" style="padding-bottom:5px">C&oacute;digo</div>' +
                '<div class="col-md-2 col-sm-2 col-xs-2" data-title="Eliminar" style="padding-bottom:5px"></div>' +
                '</div>' +
                '</th>' +
                '</tr>' +
                '</thead>' +
                '<tbody>' +
                '<tr id="issuePoint-item' + $itemsCount + '" class="product-item' + $itemsCount + '">' +
                '<td>' +
                '<div class="row">' +
                '<input id = "issuePoint_Index' + $itemsCount + '_' + $items + '"  name = "IssuePoint.Index" value = "' + $itemsCount + '" type = "hidden" ></input>' +
                '<div class="col-md-6 col-sm-8 col-xs-10" data-title="Codigo" style="padding-bottom:5px">' +
                '<div class="kt-input-icon kt-input-icon--right">' +
                '<span class="kt-input-icon__icon kt-input-icon__icon--right">' +
                '<span>' +
                '<i class="flaticon-map"></i>' +
                '</span>' +
                '</span>' +
                '<input id="code_' + $itemsCount + '_' + $items + '" name="Establishments[' + $itemsCount + '].IssuePoint[' + $items + '].Code" data-field="' + $itemsCount + '"  class="form-control placeholder-no-fix item-IssuePoint-Code", type = "number" min = "1" max = "999" minlength = "3" maxlength = "3" placeholder = "Punto emisi&oacute;n"  required data-msg = "El punto de emisi&oacute;n es requerido" />' +
                '<span class="field-validation-valid text-danger" data-valmsg-for="Establishments[' + $itemsCount + '].IssuePoint[' + $items + '].Code" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>' +
                '<div class="col-md-2 col-sm-2 col-xs-2" data-title="Eliminar" style="padding-bottom:5px">' +
                '<button id="add_trash_' + $itemsCount + '_' + $items + '" title="Eliminar Punto emisi&oacute;n" type="button" class="delete-item btn" data-field="issuePoint-items' + $itemsCount + '" style="width:auto;float:none;">' +
                '<span class="la la-trash-o "></span>' +
                '</button>' +
                '</div>' +
                '</div>' +
                '</td>' +
                '<td data-title="">' +
                '</td>' +
                '</tr>' +
                '</tbody>' +
                '<tfoot>' +
                '<tr>' +
                '<td align="left" colspan="2">' +
                '<div class="row">' +
                '<div class="col-md-12 col-sm-12 col-xs-12" align="left">' +
                '<button type="button" id = "add-issuePoint_' + $itemsCount + '_' + $items + '" data-field="issuePoint-items' + $itemsCount + '" title="Agregar puntos de emisi&oacute;n" class="btn btn-brand add-issuePoint btn-sm"><i class="fa fa-plus"></i> P. Emisi&oacute;n</button>' +
                '</div>' +
                '</div>' +
                '</td>' +
                '</tr>' +
                '</tfoot>' +
                '</table>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</td>' +
                '<td data-title="">' +
                '<button id="estab_add_trash' + $itemsCount + '" title="Eliminar Registro" type="button" class="delete-item btn " style="width:auto;float:none;">' +
                '<span class="la la-trash-o"></span>' +
                '</button>' +
                '</td>' +
                '</tr>';

            var result = $(".product-items").append(markup);
            var obj = $("#estab_code" + $itemsCount);
            $("#estab_add_trash" + $itemsCount).on("click", delete_item);
            $("#estab_code" + $itemsCount).on("change", handleLeadingZeros);
            if (typeof initialized !== 'undefined' && initialized) {
                obj.focus();
            }
            // eventos de las listas de puntos de emisión
            $("#add-issuePoint_" + $itemsCount + '_' + $items).on("click", add_issuePoint);
            var _obj = $("#code_" + $itemsCount + '_' + $items);
            $("#add_trash_" + $itemsCount + '_' + $items).on("click", delete_issuePoint_item);
            $("#code_" + $itemsCount + '_' + $items).on("change", handleLeadingZeros);
            if (typeof initialized !== 'undefined' && initialized) {
                _obj.focus();
            }

        },

        add_issuePoint = function () {
            var $obj = $(this);
            var $itemsId = $obj[0].dataset.field;
            var $objItems = $('#' + $itemsId);
            var $items = parseInt($objItems[0].dataset.content);
            if (issuePointCount == 0) {
                //issuePointCount = $objItems.find('tbody tr').length - 1;
                issuePointCount = $("tr.product-item" + $items).length - 1;
            }
            var amountIssuePoint = $("tr.product-item" + $items).length + 1;
            // validar los puntos de emisión de cuando el usuario es coperativa de transporte.
            if (Emisor.validateIssuePoint && Emisor.amountIssuePoints > 0) {
                if (Emisor.issuePoints >= Emisor.amountIssuePoints) {                    
                    var _msj = "La cantidad de puntos de emisi&oacute;n del plan adquirido, ha llegado al l&iacute;mite de " + Emisor.amountIssuePoints + " puntos de emisi&oacute;n !";
                    Swal.fire("Punto de emisi&oacute;n", _msj, "warning");
                    return
                }
                else {
                    Emisor.issuePoints++;
                }
            }
            else {
                if (amountIssuePoint > 10) {
                    Swal.fire("Punto de emisi&oacute;n", "Se permite un m&aacute;ximo la configuraci&oacute;n de 10 puntos de emisi&oacute;n!", "warning");
                    return
                }
            }           
            issuePointCount++;
            var adCount = issuePointCount;
            var markup =
                '<tr id="issuePoint-item' + $items + '_' + adCount + '" class="product-item' + $items + '"><td><div class="row">' +
                '<input id = "item' + $items + '_' + adCount + '"  name = "IssuePoint.Index" value = "' + adCount + '" type = "hidden" ></input>' +
                '<div class="col-md-6 col-sm-8 col-xs-10" data-title="Codigo" style="padding-bottom:5px">' +
                '<div class="kt-input-icon kt-input-icon--right">' +
                '<span class="kt-input-icon__icon kt-input-icon__icon--right"><span><i class="flaticon-map"></i></span></span>' +
                '<input id="code_' + $items + '_' + adCount + '" name="Establishments[' + $items + '].IssuePoint[' + adCount + '].Code" data-field="' + $items + '"  class="form-control placeholder-no-fix item-IssuePoint-Code" type = "number"  min = "1" max = "999" minlength = "3" maxlength = "3" placeholder = "Punto emisi&oacute;n" required data-msg = "El punto de emisión es requerido"/>' +
                '<span class="field-validation-valid text-danger" data-valmsg-for="Establishments[' + $items + '].IssuePoint[' + adCount + '].Code" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>' +
                '<div class="col-md-2 col-sm-4 col-xs-6" data-title="Nombre" style="padding-bottom:5px">' +
                '<button id="add_trash_' + $items + '_' + adCount + '" title="Eliminar Punto emisi&oacute;n" type="button"  class="delete-item btn " style="width:auto;float:none;">' +
                '<span class="la la-trash-o "></span>' +
                '</button>' +
                '</div>' +
                '</div>' +
                '</td>' +
                '<td data-title="">' +
                '</td>' +
                '</tr>';

            var result = $("#" + $itemsId).append(markup);
            var obj = $("#code_" + $items + "_" + adCount);
            $("#add_trash_" + $items + "_" + adCount).on("click", delete_issuePoint_item);
            $("#code_" + $items + "_" + adCount).on("change", handleLeadingZeros);
            if (typeof initialized !== 'undefined' && initialized) {
                obj.focus();
            }

        },

        delete_issuePoint_item = function () {
            var $obj = $(this);
            if ($obj[0].parentElement.parentElement.parentElement.parentElement.parentElement.childElementCount == 1) {
                Swal.fire("Punto de emisi&oacute;n", "Se requiere al menos un item en la lista!", "warning");
                return;
            }
            $obj[0].parentElement.parentElement.parentElement.parentElement.remove();
            if (Emisor.validateIssuePoint && Emisor.amountIssuePoints > 0) {
                Emisor.issuePoints--;
            }

        },

        delete_item = function () {
            var $obj = $(this);
            if ($obj[0].parentElement.parentElement.parentElement.childElementCount == 1) {
                Swal.fire("Establecimiento", "Se requiere al menos un item en la lista!", "warning");
                return;
            }
            $obj[0].parentElement.parentElement.remove();
            //$(this).parents("tr").remove();            
        },

        handleValidation = function (e) {
            if (!Emisor.urlCertValidator) {
                return; // No se procesa si no se incluye la URL
            }

            $("#CertificateExpirationDate").val("");
            $("#CertificateIssuedTo").val("");
            $("#CertificateSubject").val("");
            $("#CertificateSubject").removeClass("text-danger");
            $("#CertificateUsage").val("");
            $("#CertificateRUC").val("");

            var $form = document.getElementById("configForm");
            var $pass = $("#CertificatePass").val();
            var $file = $("#CertificateFile")[0].files[0];

            if (!$file || !$pass) {
                // No se puede procesar;
                return;
            }

            var $data = new FormData();
            $data.append("CertificatePass", $pass);
            $data.append("CertificateRaw", $file);

            $.ajax({
                url: Emisor.urlCertValidator,
                type: "POST",
                data: $data,
                success: function (data) {
                    //console.log(data);
                    if (data && data.Entity) {
                        if (data.Entity.ExpirationDate) {
                            $("#CertificateExpirationDate").val(data.Entity.ExpirationDate);
                        }

                        if (data.Entity.FriendlyName) {
                            $("#CertificateIssuedTo").val(data.Entity.FriendlyName);
                        }

                        if (data.Entity.Subject) {
                            $("#CertificateSubject").val(data.Entity.Subject);
                        }

                        if (data.Entity.CertificateRUC) {
                            $("#CertificateRUC").val(data.Entity.CertificateRUC);
                        }

                        if (data.Entity.KeyUsages) {
                            $("#CertificateUsage").val(data.Entity.KeyUsages);
                        }

                    }
                    else {
                        console.log(data);
                        if (typeof data == 'string' && data.includes("kt-login")) {
                            toastr.error("Su sesión ha caducado... por favor vuelva a iniciar sesión!")
                            window.open(location.href, "_blank");
                        }
                        else {
                            var message = "Hubo un error al cargar la firma, la clave o el archivo especificado no son correctos!";

                            $("#CertificateSubject").addClass("text-danger");
                            $("#CertificateSubject").val(message);
                            toastr.danger(message);

                        }
                    }
                },
                error: function (data) {
                    console.log(data);

                    var message = "Hubo un error al cargar la firma, la clave o el archivo especificado no son correctos!";

                    $("#CertificateSubject").addClass("text-danger");
                    $("#CertificateSubject").val(message);
                    toastr.error(message);

                },
                cache: false,
                contentType: false,
                processData: false
            });


        },

        initHandlers = function () {
            // handlers:
            $accounting.on("change keyup click", handleAccounting);
            $special.on("change keyup click", handleSpecialContrib);
            $form.on("submit", handleSubmitForm);
            $establishmentCode.on("change", handleLeadingZeros);
            $establishmentCode_item.on("change", handleLeadingZeros);
            $issuePointCode.on("change", handleLeadingZeros);
            $agent.on("change keyup click", handleRetentionAgent);
            $isRimpe.on("change keyup click", handleIsRimpe);
            $isPopularBusiness.on("change keyup click", handleIsPopularBusiness);
            $craftsman.on("change keyup click", handleSkilledCraftsman);
            $add_issuePoint.on("click", add_issuePoint);
            $add_establishment.on("click", add_establishment);
            $(".delete-item").on("click", delete_issuePoint_item);
            $(".estab-delete-item").on("click", delete_item);
            handleAccounting();
            handleSkilledCraftsman();
        },

        // Private functions
        initWizard = function () {
            // Initialize form $ktwizard
            $ktwizard = new KTWizard('kt_wizard_v2', {
                startStep: 1, // initial active step number
                clickableSteps: true  // allow step clicking
            });

            // Validation before going to next page
            $ktwizard.on('beforeNext', function (wizardObj) {
                validateIssuePoint();
                if ($validator.form() !== true) {
                    wizardObj.stop();  // don't go to the next step
                }
            });

            $ktwizard.on('beforePrev', function (wizardObj) {
                validateIssuePoint();
                if ($validator.form() !== true) {
                    wizardObj.stop();  // don't go to the next step
                }
            });

            // Change event
            $ktwizard.on('change', function (wizard) {
                KTUtil.scrollTop();
            });
        },

        initValidation = function () {
            var $isNew = !($("#Id").val() > 0);

            var rucOptions = $isNew ?
                { required: true, minlength: 13, maxlength: 13, digits: true } :
                { required: true, digits: true };

            $validator = $form.validate({
                // Validate only visible fields
                ignore: ":hidden",

                // Validation rules
                rules: {
                    //= Step 1
                    Id: { required: true },
                    RUC: rucOptions,
                    TradeName: { required: true },
                    BussinesName: { required: true },
                    Email: { required: true },
                    EstablishmentAddress: { required: true },
                    /*EstablishmentCode: { required: true },*/
                    /*IssuePointCode: { required: true },*/
                    MainAddress: { required: true },
                    Phone: { required: true },

                },

                // Display error
                invalidHandler: function (event, $validator) {
                    KTUtil.scrollTop();
                    swal.fire({
                        "title": "",
                        "text": mensajeError($validator),
                        "type": "error",
                        "confirmButtonClass": "btn btn-secondary"
                    });
                },

                // Submit valid form
                submitHandler: function (form) {
                    showLoader();
                    //form.submit();
                    save_handler();
                }
            });

            //if ($isNew) {
            //    // Validate RUC
            //    $ruc.on("change keyup", function () {
            //        var rucValue = $ruc.val();

            //        // La validacion se realiza solo si se cumple las especificaciones del RUC
            //        if (rucValue && rucValue.length === 13) {
            //            $.get(Emisor.urlRUCValidator, { RUC: rucValue }, function (result) {
            //                if (result) {
            //                    location.assign(Emisor.urlConfiguration + "?ruc=" + $ruc.val());
            //                }
            //            });
            //        }
            //    });
            //}
        },

        mensajeError = function (data) {
            var msj = "Hay algunos errores en los datos ingresados. Por favor corregirlos antes de continuar. ";
            if(data && data.errorList.length > 0)
            {
                for (var i = 0; i < data.errorList.length; ++i) {
                    if (!data.errorList[i].message.includes("Este campo es obligatorio.")) {
                        msj += data.errorList[i].message + ", ";
                    }                   
                }                
            }
            msj = msj.substr(0, msj.length - 2);          
            return msj;
        },

        validateUserRUC = function () {
            var $user = Emisor.user;
            var $ruc = $("#RUC").val()

            if ((($ruc && $ruc.includes($user)) || ($user && $user.includes($ruc)))) {

                $("#sri_password").hide();
                $("#SRIPassword").val($ruc);
            }
            else {

                $("#sri_password").show();

                $("input#SRIPassword").val("");
                $("input#SRIPassword").focus();
            }
        },

        newValidation = function () {
            $("#RUC").on("change", validateUserRUC);

            $validator = $form.validate({
                // Validate only visible fields
                ignore: ":hidden",

                // Validation rules
                rules: {
                    //= Step 1
                    RUC: { required: true, minlength: 13, maxlength: 13, digits: true },
                    SRIPassword: { required: true }
                },

                // Display error
                invalidHandler: function (event, $validator) {
                    KTUtil.scrollTop();

                    swal.fire({
                        "title": "",
                        "text": "Hay algunos errores en los datos ingresados. Por favor corregirlos antes de continuar.",
                        "type": "error",
                        "confirmButtonClass": "btn btn-secondary"
                    });
                },

                // Submit valid form
                submitHandler: function (form) {
                    vincularEmisor(form);
                    //showLoader();
                    //form.submit();
                }
            });

            validateUserRUC();
        },

        initSubmit = function () {
            $(".firma-button").on("click", function () {
                $(".certificate").val(1);
                //$form.submit();
                validateIssuePoint();
                save_handler();
            });

            $(".save-button").on("click", function () {
                //$form.submit();
                validateIssuePoint();
                save_handler();
            });

            $(".solicitud-esign").on("click", function () {
                var $obj = $(this);
                validateSubscription($obj);               
            });
        },

        validateSubscription = function (obj) {
            var $obj = obj;
            var $option = parseInt($obj.data("options").option);
            var $valor = $obj.data("options").valor;
            switch ($option) {
                case 1:
                    Swal.fire('Informaci&oacute;n', $valor, 'info');
                    event.preventDefault();
                    break;
                case 2:
                    Swal.fire('Informaci&oacute;n', $valor, 'info');
                    $obj.data("options", { option: 3, valor: Emisor.urlPlans})
                    $obj[0].innerText = "SOLICITAR PLAN";
                    event.preventDefault();
                    break;
                default:
                    event.preventDefault();
                    showLoader();
                    location.assign($valor);
                    break;
            }
        },

        Serialize = function () { 
            var establishment = $("tr.establishment-item");
            for (var i = 0; i < establishment.length; i++) {                
                var row = $(establishment[i]);
                var issuePoints = row.find(".item-IssuePoint-Code");
                for (var k = 0; k < issuePoints.length; k++) {
                    var $items = parseInt(issuePoints[k].dataset.field);
                    var inputCode = issuePoints[k].id.replace(/Establishments_|__IssuePoint|__Code/g, "").split('_');
                    var x = 0;
                    var y = 0;
                    if (inputCode.length == 2) {
                        x = parseInt(inputCode[0]);
                        y = parseInt(inputCode[1]);
                    }
                    else if (inputCode.length == 3) {
                        x = parseInt(inputCode[1]);
                        y = parseInt(inputCode[2]);
                    }
                    var issuePointName = $("#Establishments_" + x + "__IssuePoint_" + y + "__Name");
                    var issuePointCaRUC = $("#Establishments_" + x + "__IssuePoint_" + y + "__CarrierRUC");
                    var issuePointCaPhone =$("#Establishments_" + x + "__IssuePoint_" + y + "__CarrierPhone");
                    var issuePointCaEmail =$("#Establishments_" + x + "__IssuePoint_" + y + "__CarrierEmail");
                    var issuePointCarPlate = $("#Establishments_" + x + "__IssuePoint_" + y + "__CarPlate");                    
                    issuePoints[k].name = "Establishments[" + $items + "].IssuePoint[" + k + "].Code";
                    //validar si existe los campos
                    if (issuePointName.length > 0) {
                        issuePointName[0].name = "Establishments[" + $items + "].IssuePoint[" + k + "].Name";
                    }
                    if (issuePointCaRUC.length > 0) {
                        issuePointCaRUC[0].name = "Establishments[" + $items + "].IssuePoint[" + k + "].CarrierRUC";
                    }
                    if (issuePointCaPhone.length > 0) {
                        issuePointCaPhone[0].name = "Establishments[" + $items + "].IssuePoint[" + k + "].CarrierPhone";
                    }
                    if (issuePointCaEmail.length > 0) {
                        issuePointCaEmail[0].name = "Establishments[" + $items + "].IssuePoint[" + k + "].CarrierEmail";
                    }
                    if (issuePointCarPlate.length > 0) {
                        issuePointCarPlate[0].name = "Establishments[" + $items + "].IssuePoint[" + k + "].CarPlate";
                    }
                }
            }           
            var $data = new FormData($form[0]);
            return $data;
        },

        validateIssuePoint = function () {
            var establishment = $("tr.establishment-item");
            for (var i = 0; i < establishment.length; i++) {
                var row = $(establishment[i]);
                var issuePoints = row.find(".item-IssuePoint-Code");
                for (var k = 0; k < issuePoints.length; k++) {
                    var $items = parseInt(issuePoints[k].dataset.field);
                    if (issuePoints[k].value == "") {
                        var accordion = $("#itemsCollapse" + $items);
                        accordion.collapse('show');
                        return;
                    }
                }
            }           
        },

        vincularEmisor = function (form) {
            var txt = "Desea continuar con el proceso? ";
            Swal.fire({
                title: 'Vincular Emisor',
                text: txt,
                icon: 'info',
                type: 'warning',
                showCancelButton: true,
                cancelButtonText: 'Cancelar',
                confirmButtonText: 'Aceptar'

            }).then(function (result) {
                if (result.value) {
                    showLoader();
                    form.submit();
                }
            });
        },

        save_handler = function () {           
            var $action = $form.attr("action");            
            if ($validator.form() && $form.valid()) {
                var $data = Serialize();
                showLoader();
                $.ajax({
                    url: $action,
                    type: "POST",
                    data: $data,
                    success: function (data) {                       
                        if (data && data.id > 0) {
                            toastr.success(data.statusText);
                            location.assign(data.url);
                        }
                        else {                           
                            if (typeof data == 'string' && data.includes("kt-login")) {
                                toastr.error("Su sesión ha caducado... por favor vuelva a iniciar sesión!")
                                window.open(data.url, "_blank");
                            }
                            else {
                                if (data.error) {
                                    toastr.warning(data.error.DevMessage || "", data.error.UserMessage || data.statusText);
                                }
                                else {
                                    toastr.warning(data.statusText);
                                }
                            }
                        }
                        hideLoader();
                    },
                    error: function (data) {                       
                        var message = "Hubo un error al guardar el emisor!";
                        toastr.error(message);
                        hideLoader();

                    },                    
                    cache: false,
                    contentType: false,
                    processData: false
                });
               
            }
            //else {
            //    swal.fire({
            //        "title": "",
            //        "text": mensajeError,
            //        "type": "error",
            //        "confirmButtonClass": "btn btn-secondary"
            //    });   
            //}         
        };        

    return {
        user: "",
        validateIssuePoint: false,
        amountIssuePoints: 0,
        issuePoints: 0,
        urlRUCValidator : "",
        urlConfiguration: "",
        urlCertValidator: "",
        urlPlans: "",
        // public functions
        init: function() {
            initHandlers();
            initWizard();
            initCertificate();
            initValidation();
            initSubmit();
        },

        new: function () {
            initHandlers();
            initWizard();
            newValidation(); 
        }
    };
}();
 