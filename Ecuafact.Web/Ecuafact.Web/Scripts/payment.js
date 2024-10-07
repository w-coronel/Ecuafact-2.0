var Payment = function () {


    var handlePayments = function () {
        $form = $(".payment-form");
        $form.on("submit", function (e) {
            e.preventDefault();
            var $data = $form.serialize();

            $.post(Payment.CheckoutUrl, $data)
                .done(function (result) {
                    // Creo el iframe para los pagos:
                    var iFrame = $('<iframe id="_payment" name="_payment" width="780" height="520" allow="payment" frameborder="0" style="border:none"  ></iframe>');
                    $('.payment-content')
                        .empty()
                        .append(iFrame);

                    // asigno el html del pago al iframe:
                    var iFrameDoc = iFrame[0].contentDocument || iFrame[0].contentWindow.document;
                    iFrameDoc.write(result);
                    iFrameDoc.close();

                    // muestro el modal
                    $(".payment-modal").modal("show");
                })
                .fail(function (e, d, t) {
                    debugger;
                });
        });
    }



    return {

        CheckoutUrl :"",

        Configure: function () {
            handlePayments();
        }
    };


}();