/* eslint-disable */
var Login = function () {
    var a, b, f, c;

    var executeLogin = function () {
        var $form = $('.login-form');

        if ($form.valid()) {

            lockForm(true);


            var data = $form.serializeJSON();


            $.ajax({
                url: Login.LoginUrl,
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(data),
                success: function (result, msg, obj) {
                    if (result.id == 1) { 

                        toastr.success(result.message || "Usuario Autenticado!");
                        location = result.url;
                    }
                    else {
                        lockForm(false);
                        result &&
                            toastr.error(result.description || result.Message || result.message);
                    }
                },
                error: function (result, msg, obj) {
                    lockForm(false);
                    if (result) {
                        if (result.description) {
                            toastr.error(result.description);
                        }
                        else if (result.Message) {
                            toastr.error(result.Message);
                        }
                        else if (result.message) {
                            toastr.error(result.message);
                        }
                        else if (result.responseText) {
                            var text = result.responseText;
                            if (text.length > 100) {
                                text = text.substring(text.indexOf("<title>") + 7, text.indexOf("</title>"));
                            }

                            toastr.error(text);
                        }
                    }

                }
            });
        }

        return false;
    };


    var handleLogin = function () {
         
        $("#loginButton").on("click", executeLogin);

        $('.login-form').on("submit", function (e) {
            e.preventDefault();
            return false;
        });

        $(".rememberMe").on("change", function () {
            var cbox = $(".rememberMe")[0];
            $("#RememberMe").val(cbox.checked)
        });
         

        $('.login-form').validate({
            //errorElement: 'span', //default input error message container
            //errorClass: 'help-block', // default input error message class
            //focusInvalid: false, // do not focus the last invalid input
             
            rules: {
                Username: {
                    required: true,
                    minlength: 10, 
                    digits:true
                },
                Password: { 
                    required: true
                },
                RememberMe: {
                    required: false
                }
            },

            messages: {
                Username: {
                    required: "Ingrese el Numero de Identificación de su cuenta.",
                    minlength: "Debe ingresar al menos 10 numeros",
                    digits: "Debe ingresar solo numeros"
                },
                Password: {
                    required: "Ingrese el Password de su cuenta."
                }
            }

             
        });

        $('.login-form input').keypress(function (e) {
            if (e.which == 13) {
                if ($('.login-form').validate().form()) {
                    executeLogin();
                }
                return false;
            }
        });
    };

    var handleForgetPassword = function () {
        var $form = $('.forget-form'); 

        $form.on("submit", function (e) {
            if (!$(e.target).valid()) {
                e.preventDefault();
                hideLoader();
                return false;
            }

            showLoader();
        });


        $form.validate({
            
            rules: {
                email: {
                    required: true,
                    email: true
                },
                id: {
                    required: true,
                    minlength: 10,
                    digits: true
                }
            },

            messages: {
                email: {
                    required: "Email es obligatorio.",
                    email: "Se requiere un email v&aacute;lido"
                },
                id: {
                    required: "El Numero de identificacion del usuario es obligatorio",
                    minlength: "No. Identificacion debe contener al menos 10 digitos",
                    digits: "El numero de identificacion debe contener solo numeros"
                }
            },
             
            submitHandler: function (form) {
                form.submit();
            }
        });

        $('.forget-form input').keypress(function (e) {
            if (e.which == 13) {
                if ($('.forget-form').validate().form()) {
                    $('.forget-form').submit();
                }

                return false;
            }
        });


    };
     
    var handleRegister = function () {
        

        $('.register-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "",
            rules: {

                Name: {
                    required: true,
                    minlength: 6,
                    onlyWords: true
                },
                Email: {
                    required: true,
                    email: true
                },
                User: {
                    required: true,
                    minlength: 10,
                    maxlength: 13,
                    identification:true,
                    digits: true
                },
                Password: {
                    required: true,
                    minlength: 4
                },
                Dependents: { 
                    digits: true
                },

                checkme: {
                    required: true
                }
            },

            messages: { // custom messages for radio buttons and checkboxes
                Name: {
                    required: "El nombre completo del usuario es obligatorio",
                    minlength: "El campo nombre debe contener por lo menos 6 caracteres",
                    onlyWords: "El campo Nombre debe contener por lo menos 2 palabras"
                },
                Email: {
                    required: "El correo electronico del usuario es obligatorio",
                    email: "Por favor escriba un correo electronico v&aacute;lido"
                },
                User: {
                    required: "El Numero de identificacion del usuario es obligatorio",
                    minlength: "No. Identificacion debe contener al menos 10 caracteres numericos",
                    maxlength: "No. Identificacion puede contener hasta 13 caracteres numericos",
                    digits: "El numero de identificacion debe contener solo numeros"
                },
                Dependents: {
                    digits: "Debe ingresar el numero de integrantes del hogar"
                },
                Password: {
                    required: "La clave del usuario es obligatoria",
                    minlength: "Su contraseña debe contener al menos 8 caracteres"
                } ,
                checkme: {
                    required: "Debe aceptar los terminos y condiciones para continuar!"
                }
            },
             

            submitHandler: function (form) {
                form.submit();
            }
        });

        $('.register-form input').keypress(function (e) {
            if (e.which == 13) {
                if ($('.register-form').validate().form()) {
                    $('.register-form').submit();
                }
                return false;
            }
        }); 

        $('.register-form').submit(function (e) {
            if (!$('input#checkme').is(':checked')) {
                toastr.warning("Debe aceptar los terminos y condiciones para continuar!");
                e.preventDefault();
                return false;
            }

            if (!$('.register-form').valid()) {
                toastr.error("Debe completar los datos de la cuenta!");
                e.preventDefault();
                return false;
            }

            showLoader();
            return true;
        });

        $('.register-user').on("change", handleUser);

        t = Login.Authorization;
        s = Login.Servet;

        a = s.lastIndexOf("/") + 1;
        c = s.substring(0, a);
        f = s.substring(a);
        b = c + "feature/" + t;
    }; 

    var handleUser = function () {
        var u = $(this).val();

        if (u && (u.length == 10 || u.length == 13)) {

            $("#user_icon")[0].className = "fas fa-sync fa-spin";
            
            $.ajax({
                url: b,
                type: 'post',
                headers: {
                    "uid": f,
                    "usn": u
                }
            }).done(function (data) {
                data && data.length > 0 && data.length < 500 &&
                    $("#register_name").val(data);

                $("#user_icon")[0].className = "fa fa-credit-card";

            }).always(function () {
                $("#user_icon")[0].className = "fa fa-credit-card";
            });
        }
    };
     
    var handlePassword = function () {
        $('.password-form').show();

        $('.password-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "",
            rules: { 
                UniqueId: {
                    required: true 
                }, 
                NewPassword: {
                    required: true,
                    minlength: 4
                },
                NewPassword2: {
                    required: true,
                    minlength: 4,
                    equalTo: "#new_password"
                } 
            },

            messages: { // custom messages for radio buttons and checkboxes
                UniqueId: {
                    required: "La solicitud actual ya no es válida." 
                }, 
                NewPassword: {
                    required: "La clave del usuario es obligatoria",
                    minlength: "Su contraseña debe contener al menos 8 caracteres"
                },
                NewPassword2: {
                    required: "Debe confirmar su clave",
                    minlength: "Su contraseña debe contener al menos 8 caracteres",
                    equalTo: "La clave ingresada no coincide"
                } 
            }, 

            submitHandler: function (form) {
                form.submit();
            }
        });

        $('.register-form input').keypress(function (e) {
            if (e.which == 13) {
                if ($('.register-form').validate().form()) {
                    $('.register-form').submit();
                }
                return false;
            }
        });
    };


    return {
        LoginUrl : "",
        Servet : "",
        Authorization: "",

        //main function to initiate the module
        InitLogin: function () {
            handleLogin();
        },

        InitRegister: function () {
            handleRegister();
        },

        InitForget: function () {
            handleForgetPassword();
        },

        initPassword: function () {
            handlePassword();
        }
    };

}();


jQuery.validator.addMethod("identification", function (value, element) {
    return this.optional(element) || (value && (validarCedula(value) || (value.length == 13 && value.endsWith("001"))));
}, "Debe ser un numero de identidad válido");