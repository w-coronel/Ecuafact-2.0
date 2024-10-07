var Config = function () {
    var wizard,

        initWizard = function () {
            // Initialize form wizard
            wizard = new KTWizard('config_wizard', {
                startStep: 1, // initial active step number
                clickableSteps: true  // allow step clicking                
            });
        },

        handleConfigure = function () {

            $('.configure-form').validate({
                errorElement: 'span', //default input error message container
                errorClass: 'help-block', // default input error message class
                focusInvalid: false, // do not focus the last invalid input
                ignore: "",
                rules: {

                    RUC: {
                        required: true,
                        digits: true,
                        minlength: 13
                    },
                    BussinesName: {
                        required: true
                    },
                    TradeName: {
                        required: true
                    },
                    MainAddress: {
                        required: true
                    },

                    Phone: {
                        required: true,
                        digits: true
                    },
                    Email: {
                        required: true,
                        email: true
                    },

                    EnvironmentType: {
                        required: true
                    },
                    IssueType: {
                        required: true
                    },

                    EstablishmentCode: {
                        required: true
                    },
                    IssuePointCode: {
                        required: true
                    }


                },

                messages: { // custom messages for radio buttons and checkboxes
                    RUC: {
                        required: "Debe especificar un número de RUC ",
                        digits: "Debe especificar solo Números",
                        minlength: "Debe contener un número autorizado por el SRI de 13 dígitos"
                    },
                    BussinesName: {
                        required: "Debe especificar la Razon Social",
                        onlyLetters: "Debe especificar solo letras"
                    },
                    TradeName: {
                        required: "Debe especificar el Nombre Comercial",
                        onlyLetters: "Debe especificar solo letras"
                    },
                    MainAddress: {
                        required: "Debe especificar una dirección válida"
                    },

                    Phone: {
                        required: "Debe especificar un número de telefono",
                        digits: "Debe ingresar un número de telefono válido"
                    },
                    Email: {
                        required: "Debe especificar un Correo Electrónico",
                        email: "Debe ingresar un Correo Electrónico válido"
                    },

                    EnvironmentType: {
                        required: "Debe especificar el tipo de Ambiente que va a utilizar"
                    },
                    IssueType: {
                        required: "Debe especificar el tipo de emisión"
                    },

                    EstablishmentCode: {
                        required: "Debe especificar un código de Establecimiento"
                    },
                    IssuePointCode: {
                        required: "Debe especificar un código de emisión"
                    }
                },

                invalidHandler: function (event, validator) { //display error alert on form submit   

                },

                highlight: function (element) { // hightlight error inputs
                    $(element).closest('.form-group').addClass('has-error'); // set error class to the control group
                },

                success: function (label) {
                    label.closest('.form-group').removeClass('has-error');
                    label.remove();
                },

                submitHandler: function (form) {
                    form.submit();
                }
            });

            $('.configure-form input').keypress(function (e) {
                if (e.which == 13) {
                    if ($('.button-submit').is(':visible') && $('.configure-form').validate().form()) {
                        $('.configure-form').submit();
                    }
                    return false;
                }
            });

            $('#configForm .button-submit').click(function () {
                if ($('.configure-form').validate().form()) {
                    $('.configure-form').submit();
                }
                return false;
            }).hide();

            $('.configure-form').submit(function (e) {
                if (!$('.configure-form').validate().valid()) {
                    toastr.error("Debe completar con la informacion del Emisor!");
                    e.preventDefault();
                    return false;
                }

                showLoader();
                return true;
            });

            $('.budget-limits').submit(function (e) {
                e.preventDefault();
            });

            $('.solicitud-esign').on("click", validateSubscription);

            $('.delete-item').on("click", handleDesvincular);

            $('.limits-value').on("change", function () {
                var $me = $(this);
                var $value = ($me.val() * 1);

                if ($value < 0) {
                    $value = 0.00;
                }

                var $name = $me.prop("name").replace("maxValue", "total");
                var $tot = $("[name='" + $name + "']");

                setValueSafe($tot, ($value * 12).toFixed(2));

                $me.val($value.toFixed(2));

                var $total1 = $("input.limits-value")
                var $total2 = $("input.limits-total")

                var $total_mensual = 0.00;
                var $total_anual = 0.00;

                for (var i = 0; i < $total1.length; i++) {
                    var val1 = $($total1[i]).val() * 1;
                    var val2 = $($total2[i]).val() * 1;

                    $total_mensual += val1;
                    $total_anual += val2;
                }

                setValueSafe($("[name='limits.total_mensual']"), $total_mensual.toFixed(2));

                setValueSafe($("[name='limits.total_anual']"), $total_anual.toFixed(2));

            });

            $('.save-budget').on("click", function () {
                var $form = $(".budget-limits");
                var $action = $form.prop("action");
                var $data = $form.serializeArray();

                showLoader();

                $.ajax({
                    url: $action,
                    type: "POST",
                    data: $data,
                    async: true,
                    error: function (data, o, e) {
                        console.log(data);

                        if (data && data.responseText && data.responseText.includes("login")) {
                            Swal.fire("Su Sesion ha Caducado", "Por favor inicie sesion", "error");
                            var w = open(location.origin, "_top", "height=770,width=520");
                        }
                        else {
                            toastr.error(data.statusText);
                        }
                    },
                    success: function (data) {
                        if (data.IsSuccess) {
                            toastr.success(data.UserMessage);
                        }
                        else {
                            toastr.error(data.UserMessage || "Hubo un error al guardar");
                        }
                    }
                }).always(function () {
                    hideLoader();
                });

                return;

            });

            if ($.fn.DataTable)
                $('.datatable').DataTable({
                    responsive: true,
                    pageLength: 5,
                    lengthMenu: [5, 10, 25, 50],
                    searching: false
                });

            var photo = new KTAvatar("kt_user_edit_avatar");

            handleWizard();

        },

        validateSubscription = function () {
            var $obj = $(this);
            var $option = parseInt($obj.data("options").option);
            var $valor = $obj.data("options").valor;
            switch ($option) {
                case 1:
                    Swal.fire('Información', $valor, 'info');
                    break;
                case 2:
                    Swal.fire('Información', $valor, 'info');
                    $obj.data("options", { option: 3, valor: Config.plansUrl })
                    $obj[0].innerText = "SOLICITAR PLAN";
                    break;
                default:
                    showLoader();
                    location.assign($valor);
                    break;
            }
        },

        handleDesvincular = function () {
            var item = $(this);
            var ruc = item.data("ruc");
            var urlAction = Config.urlDesvincular + '/' + ruc;
            Swal.fire({
                title: 'Cuenta',
                text: "¿Esta usted seguro de desvincular la cuenta?",
                icon: 'info',
                type: 'warning',
                showCancelButton: true,
                cancelButtonText: 'Cancelar',
                confirmButtonText: 'Eliminar'

            }).then(function (result) {
                if (result.value) {
                    // done function
                    showLoader();

                    $.ajax({
                        url: urlAction,
                        type: "post",
                        async: true,
                        cache: false,
                        error: function (error) {
                            console.log(error);
                            toastr.error("No se pudo desvincular la cuenta!");
                        },
                        success: function (result) {
                            if (result.id > 0) {
                                toastr.success(result.message);
                                // refresh page
                                item.parents("tr").remove();
                            }
                            else { toastr.success(result.message); }
                        }
                    }).always(function () {
                        hideLoader();
                    });
                }
            });

        },

        handleWizard = function () {

            var form = $('#configForm');
            var error = $('.alert-danger', form);
            var success = $('.alert-success', form);

            var handleTitle = function (tab, navigation, index) {

                var total = navigation.find('li').length;
                var current = index + 1;
                // set wizard title
                $('.step-title', $('#configForm')).text('Paso ' + (index + 1) + ' de ' + total);
                // set done steps
                jQuery('li', $('#configForm')).removeClass("done");
                var li_list = navigation.find('li');
                for (var i = 0; i < index; i++) {
                    jQuery(li_list[i]).addClass("done");
                }

                if (current == 1) {
                    $('#configForm').find('.button-previous').hide();
                } else {
                    $('#configForm').find('.button-previous').show();
                }

                if (current >= total) {
                    $('#configForm').find('.button-next').hide();
                    $('#configForm').find('.button-submit').show();
                    // displayConfirm();
                } else {
                    $('#configForm').find('.button-next').show();
                    $('#configForm').find('.button-submit').hide();
                }
                App.scrollTo($('.page-title'));
            };

        };

    return {
        plansUrl: "",
        urlDesvincular: "",
        Init: function () {
            initWizard();
            handleConfigure();
        }
    };
}();
