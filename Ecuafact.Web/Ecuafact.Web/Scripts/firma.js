var Firma = function () {



    var onChangeTipoPersona = function () {
        tipofirma = $("select#TipoFirma").val();
        switch (tipofirma) {
            case '1':
                $("#tipo_persona_title").text('Persona Natural');
                $('#doc_juridico').hide();
                $('.doc_juridico').hide();
                $('.doc_natural').show();
                break;
            case '2':
                $("#tipo_persona_title").text('Persona Jurídica');
                $('#doc_juridico').show();
                $('.doc_juridico').show();
                $('.doc_natural').hide();
                break;
        }
    };


    var onChangeTipoVerificacion = function () {
        tipoVerificacion = $("select#TipoVerificacion").val();

        switch (tipoVerificacion) {
            case '1':
                $('div.dir-skype').hide();
                $('div.dir-fisica').hide();
                break;
            case '2':
                $('div.dir-skype').show();
                $('div.dir-fisica').hide();
                break;
            case '3':
                $('div.dir-skype').hide();
                $('div.dir-fisica').show();
                break;
        }
    }

    var onSelectFile = function (input, label, max) {
        var inputElement = $("#" + input);
        var labelElement = $("#" + label);
         
        if (inputElement[0].files.length > 0) {
            var iSize = (inputElement[0].files[0].size / 1024);
            if (iSize / 1024 > 1) {
                iSize = (Math.round((iSize / 1024) * 100) / 100);
                if (iSize > max) {
                    labelElement.html('Tamaño de archivo: ' + iSize + "Mb" + " (Disminuya el tamaño del archivo)");
                    labelElement.attr('style', 'color: #FC5555');
                    inputElement.css("border-color", "#FC5555")
                    return false;
                } else {
                    labelElement.html('Tamaño de archivo: ' + iSize + "Mb");
                    labelElement.attr('style', 'color: #9c9c9c');
                    inputElement.css("border-color", "");
                }

            } else {
                iSize = (Math.round(iSize * 100) / 100)
                labelElement.html('Tamaño de archivo: ' + iSize + "kb");
                labelElement.attr('style', 'color: #9c9c9c');
            }
        } else {
            labelElement.html('Tamaño de archivo: 0Mb');
            labelElement.attr('style', 'color: #FC5555');
            inputElement.css("border-color", "#FC5555")
            return false;
        }

        return true;
    }


    var fieldErrors = [];

    var validateForm = function () {
        fieldErrors = [];
        validateField("TipoFirma", "Seleccione el Tipo de firma");
        validateField("RazonSocial", "Ingrese la Razon Social");
        validateField("Nombre", "Ingrese el nombre");
        validateField("Apellido", "Ingrese el apellido");
        debugger;
    }

    var validateField = function (field, msg) {
        var $field = $(field);
        var $valid = ($field.val() != '');

        $field.css("border-color", $valid ? "#FC5555" : "");

        if (!$valid) {
            fieldErrors.push({ message: msg });
        }
    }

    var validateFile = function (field, label, max, msg) {
        var $valid = onSelectFile(field, label, max);

        if (!$valid) {
            $(label).html($(label).html() + "<br/>- " + msg);
            fieldErrors.push({ message: msg });
        }
    }


    var validField = function () {
        arrayValid = [];
        validateForm();

        if ($('#TipoFirma').val() === "") {
            valid = { message: 'Seleccione tipo de firma' };
            $('#TipoFirma').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        }
        if ($('#RazonSocial').val() === "") {
            valid = { message: 'Ingrese la Razon Social' };
            $('#RazonSocial').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        }
        if ($('#Nombre').val() === "") {
            valid = { message: 'Ingrese un nombre' };
            $('#Nombre').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        }
        if ($('#Apellido').val() === "") {
            valid = { message: 'Ingrese apellidos' };
            $('#Apellido').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        }
        if ($('#TipoDocumento').val() === "") {
            valid = { message: 'Seleccione tipo de documento' };
            $('#TipoDocumento').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        }
        if ($('#Identificacion').val() === "") {
            valid = { message: 'Ingrese un número de documento' };
            $('#Identificacion').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        } else {
            if ($('#TipoDocumento').val() === 'cedula') {
                vcedula = validarCedula($('#Identificacion').val());
                if (vcedula === false) {
                    valid = { message: 'Número de cédula inválido' };
                    $('#Identificacion').css("border-color", valid ? "#FC5555" : "");
                    arrayValid.push(valid);
                }
            }
        }
        if ($('#Email').val() === "") {
            valid = { message: 'Ingrese el correo' };
            $('#Email').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        } else {
            if (validarEmail($('#Email').val()) === false) {
                valid = { message: 'Correo inválido' };
                $('#Email').css("border-color", valid ? "#FC5555" : "");
                arrayValid.push(valid);
            }
        }
        if ($('#Telefono').val() === "") {
            valid = { message: 'Ingrese el teléfono' };
            $('#Telefono').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        } else {
            if ($('#Telefono').val().length > 10) {
                valid = { message: 'El número de teléfono no debe superar los 10 dígitos' };
                $('#Telefono').css("border-color", valid ? "#FC5555" : "");
                arrayValid.push(valid);
            }
        }
        if ($('#VigenciaFirma').val() === "") {
            valid = { message: 'Seleccione vigencia de la firma' };
            $('#VigenciaFirma').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        }
        if ($('#TipoVerificacion').val() === "") {
            valid = { message: 'Seleccione un tipo de verificación' };
            $('#TipoVerificacion').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        }

        if ($('#CopiaCedulaRaw').val() === "") {
            valid = { message: 'Seleccione una copia de Cédula o Pasaporte' };
            $('#CopiaCedulaRaw').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        } else {
            if (!validSizeFile($('#CopiaCedulaRaw'), 2)) {
                valid = { message: 'Copia de Cédula o Pasaporte demasiado grande.' };
                $('#CopiaCedulaRaw').css("border-color", valid ? "#FC5555" : "");
                arrayValid.push(valid);
            }
        }
        if ($('#CopiaVotacionRaw').val() === "") {
            valid = { message: 'Seleccione una copia de Papeleta de Votación' };
            $('#CopiaVotacionRaw').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        } else {
            if (!validSizeFile($('#CopiaVotacionRaw'), 2)) {
                valid = { message: 'Copia de Papeleta de votación demasiado grande.' };
                $('#CopiaVotacionRaw').css("border-color", valid ? "#FC5555" : "");
                arrayValid.push(valid);
            }
        }
        if ($('#CopiaRUCRaw').val() === "") {
            valid = { message: 'Seleccione una copia de RUC' };
            $('#CopiaRUCRaw').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        } else {
            if (!validSizeFile($('#CopiaRUCRaw'), 2)) {
                valid = { message: 'Copia de RUC demasiado grande.' };
                $('#CopiaRUCRaw').css("border-color", valid ? "#FC5555" : "");
                arrayValid.push(valid);
            }
        }
        if ($('#AutorizacionRepLegalRaw').val() === "") {
            valid = { message: 'Seleccione una copia de la Autorización del Representante Legal' };
            $('#AutorizacionRepLegalRaw').css("border-color", valid ? "#FC5555" : "");
            arrayValid.push(valid);
        } else {
            if (!validSizeFile($('#AutorizacionRepLegalRaw'), 2)) {
                valid = {
                    message: 'Copia de Autorización de Representante Legal demasiado grande.'
                };
                $('#AutorizacionRepLegalRaw').css("border-color", valid ? "#FC5555" : "");
                arrayValid.push(valid);
            }
        }
        if ($('#TipoFirma').val() == '2') {
            if ($('#NombramientoRaw').val() === "") {
                valid = {
                    message: 'Seleccione una copia del nombramiento del representante legal'
                };
                $('#NombramientoRaw').css("border-color", valid ? "#FC5555" : "");
                arrayValid.push(valid);
            } else {
                if (!validSizeFile($('#NombramientoRaw'), 2)) {
                    valid = {
                        message: 'Copia de Nombramiento de Representante Legal demasiado grande.'
                    };
                    $('#NombramientoRaw').css("border-color", valid ? "#FC5555" : "");
                    arrayValid.push(valid);
                }
            }
            if ($('#ConstitucionRaw').val() === "") {
                valid = {
                    message: 'Seleccione una copia del acta de Constitución de la Compañia'
                };
                $('#ConstitucionRaw').css("border-color", valid ? "#FC5555" : "");
                arrayValid.push(valid);
            } else {
                if (!validSizeFile($('#ConstitucionRaw'), 5)) {
                    valid = { message: 'Copia de Constitución de Compañia demasiado grande.' };
                    $('#ConstitucionRaw').css("border-color", valid ? "#FC5555" : "");
                    arrayValid.push(valid);
                }
            }
        }

        if (arrayValid.length > 0) {
            var html = '<ul>';
            for (i = 0; i < arrayValid.length; i++) {
                html += '<li> &nbsp;' + arrayValid[i].message + '</li> ';
                //toastr.error(arrayValid[i].message);
            }
            html += "</ul>";
            $('#error').html(html);
            $('#error').show();
            return false;
        } else {
            $('#error').hide();
            return true;
        }
    }

    var validSizeFile = function (inputElement, max) {
        if (inputElement[0].files.length > 0) {
            var iSize = (inputElement[0].files[0].size / 1024);
            if (iSize / 1024 > 1) {
                iSize = (Math.round((iSize / 1024) * 100) / 100);
                if (iSize > max) {
                    return false;
                }
            }
        }
        return true;
    }



    var handleProcessor = function (e) {
        e.preventDefault();

        var formData = new FormData(document.getElementById("form_orden_emision"));

        $.ajax({
            url: Firma.ProcessorUrl,
            data: formData,
            type: "post",
            dataType: "html",
            cache: false,
            contentType: false,
            processData: false,
            beforeSend: function (xhr, setting) {
                if (!validField()) {
                    //window.location.replace(Firma.HomeUrl + '#title_form');
                    xhr.abort();
                    toastr.error("Debe completar los datos requeridos!");
                } else {
                    showLoader('Espere por favor...');
                }
            },
            error: function (e,d,t) {
                hideLoader();
                toastr.error("Problemas al procesar la petición");
            },
            success: function (data) {
                console.log(data);
                hideLoader();
                
                if (!data || data.includes("<html>")) {
                    // Hubo un error
                    toastr.error("Hubo un error al guardar su solicitud...");
                    return;
                }

                data = JSON.parse(data);

                if (data.IsSuccess && data.Entity !== null) {
                    toastr.success("Su orden ha sido generada!");
                    window.location.replace(Firma.PaymentUrl + "?purchaseorderid=" + data.Entity.PurchaseOrderId);
                } else {
                    var msg = data.UserMessage ? data.UserMessage : "Hubo un error al guardar su solicitud ";
                    toastr.error(msg);
                    if (msg.includes("Ya existe")) {
                        window.location.replace(Firma.HomeUrl);
                    }
                }
            }
        }).fail(function () {
            e.preventDefault();
        });
    };



    return {
        ProcessorUrl: "",
        HomeUrl: "",
        PaymentUrl:"",
        Init: function () {


            $(document).ready(function () {

                // $.ajaxSetup({
                //     headers: {
                //         'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
                //     }
                // });

                hideLoader();

                $(".btn-enviar").on("click", function () {
                    $("#form_orden_emision").submit();
                });

                $("#TipoFirma").on("change", onChangeTipoPersona);
                $("#CopiaCedulaRaw").on("change", function () { onSelectFile('CopiaCedulaRaw', 'size_file_cedula', 2); });
                $("#CopiaVotacionRaw").on("change", function () { onSelectFile('CopiaVotacionRaw', 'size_file_pvotacion', 2); });
                $("#CopiaRUCRaw").on("change", function () { onSelectFile('CopiaRUCRaw', 'size_file_ruc', 2); });
                $("#AutorizacionRepLegalRaw").on("change", function () { onSelectFile('AutorizacionRepLegalRaw', 'size_file_reprelegal', 2); });
                $("#NombramientoRaw").on("change", function () { onSelectFile('NombramientoRaw', 'size_file_nombramiento', 2); });
                $("#ConstitucionRaw").on("change", function () { onSelectFile('ConstitucionRaw', 'size_file_constcomp', 5); });
                 
                $("#form_orden_emision").on("submit", handleProcessor);
                 
                if (!validarRUCNatural($("#Identificacion").val())) {
                    $("#TipoFirma").val(2);
                    $("#Nombre").val("");
                    $("#Apellido").val("");
                    $("#TipoFirma").trigger("change");
                }


            });

        }
    };
}();


