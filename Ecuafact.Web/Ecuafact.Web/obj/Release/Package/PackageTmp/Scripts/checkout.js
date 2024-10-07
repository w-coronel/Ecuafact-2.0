
var checkout = function () {

    var app_code;
    var app_key;
    var paymentCheckout;
    var result =[];

    var OpenCheckout = function () {        
        ValidatePayment();
    }

    var getUrlValidatePayment = function (id) {
        return `${checkout.validateUrl}?purchaseOrderId=${id}`;
    }

    var ValidatePayment = function () {
        var $url = getUrlValidatePayment(checkout.Order.PurchaseOrder.PurchaseOrderId); 
        showLoader();
        $.get($url, {}, function (data) {
            if (data.statusCode === "00" && data.status === "success")
            {                
                location.assign(data.url);
                toastr.warning(data.statusText);
            }
            else {
                OpenPaymentCheckout();
            }
        }).always(function () {
            hideLoader();
        });
    }

    var OpenPaymentCheckout = function () {
        paymentCheckout.open({
            user_id: checkout.Order.PurchaseOrder.IssuerId.toString(),
            user_email: checkout.Order.PurchaseOrder.Email,
            user_phone: checkout.Order.PurchaseOrder.Phone,
            order_description: checkout.Order.PurchaseOrder.Products,
            order_amount: checkout.Order.PurchaseOrder.Total,
            order_vat: 0.00,
            order_taxable_amount: 0.00,
            order_tax_percentage: 0,
            order_reference: checkout.Order.PurchaseOrder.PurchaseOrderId.toString()
        });
    }

    var iniOpenCheckout = function ()
    {
        if (checkout.app_key && checkout.app_code)
        {
            paymentCheckout = new PaymentCheckout.modal({
                client_app_code:checkout.app_code,
                client_app_key:checkout.app_key,
                locale: 'es',
                env_mode: checkout.env_mode, 
                onResponse: function (response) {
                    if (response) { 
                        result = {
                            "transaction": response.transaction,
                            "card": response.card,
                            "PurchaseOrder": checkout.Order.PurchaseOrder,
                        }
                        showLoader();
                        paymentStatus(result)
                    }

                }
            })
        }
    }

    var transactionResult = function (data) {

        var $divOK = $('.procesado-ok');
        var $divError = $('.procesado-error');
        if (data.transaction.carrier_code.trim() == "00")
        {
            toastr.success("El pago se realizo con exito!");
            $("#numRecibo").html(data.purchaseOperationNumber);
            $("#date").html();
            $("#status").html("APROBADA");
            $("#numAutorizacion").html();
            $("#valor").html()
            $divOK.show();
            $(".btn_volver").attr("href", checkout.UrlIndex);
        }
        else {
            toastr.error("No se pudo procesar el pago");
            $(".btn_volver").html("Volver opción de pagos")
            $(".btn_volver").attr("href", );
            $('.procesado-error').show();
        }

    }

    var paymentStatus = function (model) {        
        $.ajax({
            url: checkout.PaymentUrl,
            type: "POST",
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({model: model}),
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
                if (data.IsSuccess && data.Entity !== null) {
                    window.location.replace(checkout.UrlFinist + "?paymentId=" + data.Entity.paymentId);
                }
                else {
                    toastr.error("No se pudo procesar el pago");
                    window.location.replace(checkout.UrlTypePayment + "?purchaseOrderId=" + data.Entity.PurchaseOrder.PurchaseOrderId );
                }
            }
        }).always(function () {
            hideLoader();
        });

       
    }

    initHandlers = function () {        
        $(".payment-checkout").on("click", OpenCheckout);
        //window.addEventListener('popstate', function () {
        //    paymentCheckout.close();
        //});
    };

    return {
        PaymentUrl: "",
        UrlTypePayment: "",
        UrlFinist: "",
        Order: [],
        app_code: "",
        app_key: "",
        env_mode: "",
        key: "",
        Init: function () {
            initHandlers();
            iniOpenCheckout();
        }
    };
}();