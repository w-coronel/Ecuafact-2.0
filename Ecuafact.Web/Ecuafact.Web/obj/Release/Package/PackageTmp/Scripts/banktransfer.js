var banktransfer = function () {
	// Base elements
	
	var formEl;
	var btnSave;
	var UrlImg;
	var ext;

	var initSubmit = function () {

		formEl.on("submit", function (e) {
			e.preventDefault();

			var actionUrl = formEl[0].action;
			var formData = new FormData(formEl[0]);			

			showLoader();

			$.ajax({
				url: actionUrl,
				type: "post",
				dataType: "html",
				data: formData,
				cache: false,
				contentType: false,
				processData: false,
				error: function (e, d, t) {
					toastr.error("Problemas al procesar la petición " + e.statusText);
				},
				success: function (data) {
					if (data.includes("<html") || data.includes("html>")) {
						if (data.includes("kt-login")) {
							window.open(location.origin)
						}

						// Hubo un error
						toastr.error("Hubo un error al enviair comprobante...");
						return;
					}

					if (data.includes("UserMessage")) {
						data = JSON.parse(data);
						if (data.IsSuccess && data.UserMessage == "OK") {
							toastr.success("Se ha enviado el comprobante!");

							$('.frm_principal').hide();
							$('.frm_respuesta').show();
							if (ext == "pdf") {								
								var pdf = $('#pdfRecicbido');								
								pdf.attr('src', UrlImg);
								$('#pPdf').show();
								$('#pImg').height();
							}
							else {								
								var img = $('#imgRecicbido');
								img.attr('src', UrlImg);
								$('#pPdf').height();
								$('#pImg').show();
                            }

						} else {
							var msg = data.UserMessage ? data.UserMessage : "Hubo un error al enviar comprobante...";
							toastr.error(msg);

							if (msg.includes("Ya existe")) {
								window.location.replace(banktransfer.HomeUrl);
							}
						}
					}
					else {
						toastr.error(data || "Hubo un error al enviar comprobante...");
					}
				}
			}).fail(function () {
				e.preventDefault();

			}).always(function () {
				KTApp.unprogress(btnSave);
				hideLoader();

			});
		});


	}	

	var viewImage = function (file) {
		var img = $('#imgVoucher');
		UrlImg = URL.createObjectURL(file[0])
		img.attr('src', UrlImg);		
		img.height = 60;
		img.onload = function () {URL.revokeObjectURL(this.src);}
	}

	var viewPdf = function (file) {		
		var pdf = $('#pdfVoucher');		
		UrlImg = URL.createObjectURL(file[0])
		pdf.attr('src', UrlImg);		
		pdf.onload = function () { URL.revokeObjectURL(this.src); }
	}

	var onSelectFile = function (input, label) {
		var inputElement = $("#" + input);		
		if (inputElement[0].files.length > 0){
		    ext = inputElement[0].files[0].name.split('.').pop();
			ext = ext.toLowerCase();
			switch (ext) {
				case 'jpg':
				case 'jpeg':
				case 'png':
				case 'pdf': break;
				default:
					toastr.error('El archivo no tiene la extensión adecuada, solo se permite los formatos jpg, jpeg, png, pdf');					
					inputElement[0].value = ''; 
					inputElement[0].files[0].name = '';
			}
			if (ext == 'pdf') {
				viewPdf(inputElement[0].files);
				$('#pnlImg').hide();
				$('#pnlPdf').show();
			}
			else {
				viewImage(inputElement[0].files);
				$('#pnlImg').show();
				$('#pnlPdf').hide();
			}
			$('#Ext').val(ext);
			$('.btn_subir').show();
			
		} else {
			
			$('#pnlImg').hide();
			$('#pnlPdf').hide();
			$('.btn_subir').hide();
			return false;
		}

		return true;
	};

	var SelectFile = function () {
		$('#PaymentVoucher').click();
	}

	var goHome = function () {
		window.location.replace(banktransfer.HomeUrl);
    }

	var configure_main = function () {
		
		formEl = $('#kt_form');
		btnSave = formEl.find('[data-ktwizard-type="action-submit"]');
		$(".btn_select").on("click", SelectFile);
		$("#PaymentVoucher").on("change", function () { onSelectFile('PaymentVoucher'); });
		$(".btn_volver").on("click", goHome);
	};




	return {
		PaymentUrl: "",
		HomeUrl: "",	
		// public functions
		Init: function () {
			configure_main();			
			initSubmit();

		}
	};
}();