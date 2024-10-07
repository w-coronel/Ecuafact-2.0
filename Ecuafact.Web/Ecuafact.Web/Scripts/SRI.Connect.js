var SRI = function () {

    var handleConnect = function () {

        $('.sri-form').on("submit", function (e) {
            e.preventDefault();
            return false;
        });

        $('.sri-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "",
            rules: {

                Identification: {
                    required: true,
                },
                Password: {
                    required: true,
                    minlength: 4
                },

                checkme: {
                    required: true
                }
            },

            messages: { // custom messages for radio buttons and checkboxes
                User: {
                    required: "No ha iniciado sesión!",
                },
                Password: {
                    required: "La clave del SRI es obligatoria",
                    minlength: "Su contraseña debe contener al menos 4 caracteres"
                },
                checkme: {
                    required: "Debe aceptar los términos y condiciones para continuar!"
                }
            },


            submitHandler: function (form) {
                form.submit();
            }
        });

        $('.sri-form input').keypress(function (e) {
            if (e.which == 13) {
                if ($('.sri-form').validate().form()) {
                    $('.sri-form').submit();
                }
                return false;
            }
        });


        $(".login-sri").on("click", function () {
            var $sri = $(".sri-modal");
            $sri.find("input").css("disabled", "");
            KTApp.block($sri);

        });

    };


    return {
        SRIConnectUrl: "/SRI/Connect",
        LoginUrl:"/Auth",

        Init: function () {
            //handleConnect();
            


            $(".login-sri").on("click", function () {


                if (!$("#checkme").is(":checked")) {
                    Swal.fire("", "Debe aceptar los términos y condiciones del servicio!", "error");
                    return false;
                }

                var myruc = $('#identificacion').val();

                if (myruc == "") {
                    window.open(SRI.LoginUrl);
                }
                else {
                    var myclave = $('#password2').val();

                    if (!myclave) {
                        Swal.fire("", "Debe escribir una clave válida!", "error");
                        return false;
                    }

                    KTApp.block("#kt_modal_1");

                    $.post(SRI.ConnectURL, {
                        RUC: myruc,
                        SRIPassword: myclave
                    }, function (data) {
                        if (data && data.id == 1) {
                            Swal.fire("", data.description, "success");
                            hideModal();
                            location.reload();
                        }
                        else {
                            Swal.fire("", (data && data.description ? data.description : "Hubo un error al cambiar su clave del SRI"), "error");
                        }
                    }).always(function () {
                        KTApp.unblock("#kt_modal_1");
                    });
                }


            });
 
        }

    };

}();
