
// Initialize form wizard
wizard = new KTWizard('config_wizard', {
    startStep: 1, // initial active step number
    clickableSteps: true  // allow step clicking
});

var avatar = new KTAvatar("logo")


var Perfil = function () {

    var configure = function () {
        $("#perfil_form").validate();

        $("#perfil_form").on("submit", function (e) {
            e.preventDefault();

            var $form = $(this);

            if (!$form.valid()) {
                return false;
            }

            showLoader();

            var $el = $form[0];
            var $action = $form.attr("action");
            var $data = new FormData($el);

            $.ajax({
                url: $action,
                data: $data,
                type: "post",
                dataType: "html",
                cache: false,
                contentType: false,
                processData: false,
                success: function (result) {
                    console.log(result)

                    if (!result || result.includes("<html>")) {
                        // Hubo un error
                        toastr.error("Hubo un error al guardar su solicitud...");
                        return;
                    }

                    result = JSON.parse(result);

                    if (result) {
                        if (result.IsSuccess) {
                            toastr.success(result.UserMessage);

                            setTimeout(function () {
                                location.href = location.href.toLowerCase().replace("perfil", "");
                            }, 2000);

                        }
                        else {
                            toastr.warning(result.UserMessage);
                        }
                    }
                    else {
                        toastr.warning("Hubo un error al guardar el perfil!");
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log("Estado: " + textStatus + ", error: " + errorThrown);
                }

            }).always(function () {
                hideLoader();
            });

        });


        $("#btnSave").on("click", function () {
            $("#perfil_form").submit();
        });
    };

    return {
        Init: function () {
            configure();
        }
    };

}();