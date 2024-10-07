
var invoiceOrder = function () {
	// Base elements

	var formEl;
	var btnSave;
	var UrlProm;

	var set_codeProm = function () {
		var objDisCode = $(".DiscountCode");
		var $code = objDisCode[0].value;
		var $msg_code = $(".msg-code");
		$msg_code.hide();
		if ($code) {
			var styleOK = "fa fa-check text-success kt-icon-lg";
			var styleError = "fa fa-times text-danger kt-icon-lg";
			var _mensajeCode = $("#msg_code");
			var _imgCode = $("#imgCode");
			var btn = $(".btnAplicar");
			var $processing = false;
			var mensaje = "";
			var req = { code: $code, newPlanCode: invoiceOrder.LicenceCode};
			$msg_code.show();
			btn.attr('disabled', 'disabled');			
			KTApp.progress(btn);
			KTApp.block(_imgCode);

			if ($processing) {
				return false;
			}
			$processing = true;

			$.post(invoiceOrder.CodePromUrl, req, function (data) {				
				if (data && data.IsSuccess) {					
					_mensajeCode.attr('style', 'color:#9c9c9c');
					_imgCode.css("border-color", "#9c9c9c");
					_imgCode.removeClass(styleError);
					_imgCode.addClass(styleOK);					
					aplicarPromocion(data.Entity);
					mensaje = data.UserMessage;
				}
				else {
					if (typeof data == 'string' && data.includes("kt-login")) {
						toastr.error("Su sesión ha caducado... por favor vuelva a iniciar sesión!")
						location.reload();
						return false;
					}
					else {
						_mensajeCode.attr('style', 'color: #FC5555');
						_imgCode.css("border-color", "#FC5555");
						_imgCode.removeClass(styleOK);
						_imgCode.addClass(styleError);
						mensaje = data.UserMessage;
						btn.removeAttr("disabled");
						objDisCode[0].value = "";
					}					
				}
				_mensajeCode[0].innerHTML = mensaje;

			}).always(function () {
				$processing = false;
				KTApp.unprogress(btn);
				KTApp.unblock(_imgCode);				
			});
		}		
	};

	var salir = function () { window.location.replace(invoiceOrder.HomeUrl); };

	var aplicarPromocion = function (data) {

		price = 0.00;
		subtotal = 0.00;
		total_descuento = 0.00;
		total = 0.00;
		iva = 0.00;
		descuentoRate = 0.00;
		descuentoValue = 0.00;
		ivaRate = 0.00;
		valdesc = 0.00;

		var objprice = $(".Price");
		var objivaRate = $(".IvaRate");

		if (objprice)
		{
			var objDisCode = $(".DiscountCode");
			objDisCode.attr('readonly', 'true');
			price = getValueSafe(objprice[0].value);
			if (data.DiscountType == 2)
			{
				descuentoValue = getValueSafe(data.DiscountValue);
				valdesc = getValueSafe(descuentoValue, 2);
			}
			else {
				descuentoRate = getValueSafe(data.DiscountRate);
				valdesc = getValueSafe(price * (descuentoRate / 100), 2);
			}		
			ivaRate = getValueSafe(objivaRate[0].value);			
			subtotal = getValueSafe(price - valdesc, 6);
			iva = getValueSafe(subtotal * (ivaRate / 100), 4);
			total = getValueSafe(subtotal + iva, 2);
		}

		var objdiscount = $(".totalDiscount");
		var objsubtotal = $(".subtotal");
		var objtotalIva = $(".totalIva");
		var objtotal = $(".total");
		var _objtotal = $("._total");

		objdiscount[0].innerHTML = valdesc.toFixed(2);
		objsubtotal[0].innerHTML = subtotal.toFixed(2);
		objtotalIva[0].innerHTML = iva.toFixed(2);
		objtotal[0].innerHTML = total.toFixed(2);
		_objtotal[0].innerHTML = total.toFixed(2);

	};

	var validateData = function () {


	};

	var makePayment = function () {

		var $identification = $(".invoice-in-Identificacion").val();
		if ($identification.length != 10 && $identification.length != 13) {
			toastr.error("El numero de identificación, si es cédula debe ser de 10 dígitos y si es ruc debe ser de 13 dígitos!");
			return false
        }
		var txt = "Usted ha seleccionado el PLAN " + invoiceOrder.LicenceName + " que incluye : Firma electrónica y " + (invoiceOrder.LicenceAmount === "0000000000" ? "emisión ilimitada de documentos" : invoiceOrder.LicenceAmount + " documentos para emitir") + ". Desea continuar ? ";
		Swal.fire({
			title: 'Realizar Pago',
			text: txt,
			icon: 'info',
			type: 'warning',
			showCancelButton: true,
			cancelButtonText: 'Cancelar',
			confirmButtonText: 'Aceptar'

		}).then(function (result) {
			if (result.value) {
				var _form = $('#_kt_form');
				if (_form.valid()){
					_form.submit();
				}
				//$("#_kt_form").submit();				
			}
		});

	};

	var set_contrib = function () {
	var $identification = $(".invoice-in-Identificacion").val();
		if ($identification) {
			// Solo se aplica en el caso que la identificación sea cedula o ruc:
			if ($identification.length == 10 || $identification.length == 13) {
				var $token = $("#RefID").val();
				var $url = invoiceOrder.BuscarUrl;
				var req = {
					uid: $identification					
				};
				$(".loading-identity").show();
				$.post($url, req, function (data) {
					if (data && data.IsSuccess) {
						$(".invoice-in-name").val(data.Entity.BussinesName);
						$(".invoice-in-Address").val(data.Entity.MainAddress);
						$(".invoice-in-Email").val("");
						$(".invoice-in-Phone").val("");						
					}
				}).always(function () {
					$(".loading-identity").hide();
				});
			}
		}
	};

	var configure_main = function () {

		$(".btnAplicar").on("click", set_codeProm);
		$(".btnsalir").on("click", salir);
		$("#_kt_form").submit(function () { showLoader() });
		$(".btn-make-payment").on("click", makePayment);
		$(".invoice-in-Identificacion").on("blur", set_contrib);
	};

	return {
		CodePromUrl: "",
		BuscarUrl:"",
		LicenceName: "",
		LicenceAmount: "",
		LicenceCode:"",
		HomeUrl: "",
		loginUrl: "/Auth",
		// public functions
		Init: function () {
			configure_main();			
		}
	};
}();