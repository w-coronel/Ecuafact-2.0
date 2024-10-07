var provinciasCantones = [{
    provincia: "AZUAY",
    cantones: ["CAMILO PONCE ENRIQUEZ", "CHORDELEG", "CUENCA", "EL PAN", "GIRON", "GUACHAPALA", "GUALACEO", "NABON", "OÑA", "PAUTE", "PUCARA", "SAN FERNANDO", "SANTA ISABEL", "SEVILLA DE ORO", "SIGSIG"]
}, {
    provincia: "BOLIVAR",
    cantones: ["CALUMA", "CHILLANES", "CHIMBO", "ECHEANDIA", "GUARANDA", "LAS NAVES", "SAN MIGUEL"]
}, {
    provincia: "CAÑAR",
    cantones: ["AZOGUES", "BIBLIAN", "CAÑAR", "DELEG", "EL TAMBO", "LA TRONCAL", "SUSCAL"]
}, {
    provincia: "CARCHI",
    cantones: ["BOLIVAR", "ESPEJO", "MIRA", "MONTUFAR", "SAN PEDRO DE HUACA", "TULCAN"]
}, {
    provincia: "CHIMBORAZO",
    cantones: ["ALAUSI", "CHAMBO", "CHUNCHI", "COLTA", "CUMANDA", "GUAMOTE", "GUANO", "PALLATANGA", "PENIPE", "RIOBAMBA"]
}, {
    provincia: "COTOPAXI",
    cantones: ["LA MANA", "LATACUNGA", "PANGUA", "PUJILI", "SALCEDO", "SAQUISILI", "SIGCHOS"]
}, {
    provincia: "EL ORO",
    cantones: ["ARENILLAS", "ATAHUALPA", "BALSAS", "CHILLA", "EL GUABO", "HUAQUILLAS", "LAS LAJAS", "MACHALA", "MARCABELI", "PASAJE", "PIÑAS", "PORTOVELO", "SANTA ROSA", "ZARUMA"]
}, {
    provincia: "ESMERALDAS",
    cantones: ["ATACAMES", "ELOY ALFARO", "ESMERALDAS", "MUISNE", "QUININDE", "RIO VERDE", "SAN LORENZO"]
}, {
    provincia: "GALAPAGOS",
    cantones: ["ISABELA", "SAN CRISTOBAL", "SANTA CRUZ"]
}, {
    provincia: "GUAYAS",
    cantones: ["ALFREDO BAQUERIZO MORENO", "BALAO", "BALZAR", "COLIMES", "CORONEL MARCELINO MARIDUEÑA", "DAULE", "DURAN", "EL EMPALME", "EL TRIUNFO", "GENERAL ANTONIO ELIZALDE", "GUAYAQUIL", "ISIDRO AYORA", "LOMAS DE SARGENTILLO", "MILAGRO", "NARANJAL", "NARANJITO", "NOBOL (VICENTE PIEDRAHITA)", "PALESTINA", "PEDRO CARBO", "PLAYAS (GENERAL VILLAMIL)", "SALITRE", "SAMBORONDON", "SAN JACINTO DE YAGUACHI", "SANTA LUCIA", "SIMON BOLIVAR"]
}, {
    provincia: "IMBABURA",
    cantones: ["ANTONIO ANTE", "COTACACHI", "IBARRA", "OTAVALO", "PIMAMPIRO", "SAN MIGUEL DE URCUQUI"]
}, {
    provincia: "LOJA",
    cantones: ["CALVAS", "CATAMAYO", "CELICA", "CHAHUARPAMBA", "ESPINDOLA", "GONZANAMA", "LOJA", "MACARA", "OLMEDO", "PALTAS", "PINDAL", "PUYANGO", "QUILANGA", "SARAGURO", "SOZORANGA", "ZAPOTILLO"]
}, {
    provincia: "LOS RIOS",
    cantones: ["BABA", "BABAHOYO", "BUENA FE", "MOCACHE", "MONTALVO", "PALENQUE", "PUEBLO VIEJO", "QUEVEDO", "QUINSALOMA", "URDANETA", "VALENCIA", "VENTANAS", "VINCES"]
}, {
    provincia: "MANABI",
    cantones: ["24 DE MAYO", "BOLIVAR", "CHONE", "EL CARMEN", "FLAVIO ALFARO", "JAMA", "JARAMIJO", "JIPIJAPA", "JUNIN", "MANTA", "MONTECRISTI", "OLMEDO", "PAJAN", "PEDERNALES", "PICHINCHA", "PORTOVIEJO", "PUERTO LOPEZ", "ROCAFUERTE", "SAN VICENTE", "SANTA ANA", "SUCRE", "TOSAGUA"]
}, {
    provincia: "MORONA SANTIAGO",
    cantones: ["GUALAQUIZA", "HUAMBOYA", "LIMON - INDANZA", "LOGROÑO", "MORONA", "PABLO SEXTO", "PALORA", "SAN JUAN BOSCO", "SANTIAGO", "SUCUA", "TAISHA", "TIWINTZA"]
}, {
    provincia: "NAPO",
    cantones: ["ARCHIDONA", "CARLOS JULIO AROSEMENA TOLA", "EL CHACO", "QUIJOS", "TENA"]
}, {
    provincia: "ORELLANA",
    cantones: ["AGUARICO", "FRANCISCO DE ORELLANA", "LA JOYA DE LOS SACHAS", "LORETO"]
}, {
    provincia: "PASTAZA",
    cantones: ["ARAJUNO", "MERA", "PASTAZA", "SANTA CLARA"]
}, {
    provincia: "PICHINCHA",
    cantones: ["CAYAMBE", "MEJIA", "PEDRO MONCAYO", "PEDRO VICENTE MALDONADO", "PUERTO QUITO", "QUITO", "RUMIÑAHUI", "SAN MIGUEL DE LOS BANCOS"]
}, {
    provincia: "SANTA ELENA",
    cantones: ["LA LIBERTAD", "SALINAS", "SANTA ELENA"]
}, {
    provincia: "SANTO DOMINGO DE LOS TSACHILAS",
    cantones: ["LA CONCORDIA", "SANTO DOMINGO"]
}, {
    provincia: "SUCUMBIOS",
    cantones: ["CASCALES", "CUYABENO", "GONZALO PIZARRO", "LAGO AGRIO", "PUTUMAYO", "SHUSHUFINDI", "SUCUMBIOS"]
}, {
    provincia: "TUNGURAHUA",
    cantones: ["AMBATO", "BAÑOS DE AGUA SANTA", "CEVALLOS", "MOCHA", "PATATE", "QUERO", "SAN PEDRO DE PELILEO", "SANTIAGO DE PILLARO", "TISALEO"]
}, {
    provincia: "ZAMORA CHINCHIPE",
    cantones: ["CENTINELA DEL CONDOR", "CHINCHIPE", "EL PANGUI", "NANGARITZA", "PALANDA", "PAQUISHA", "YACUAMBI", "YANTZAZA", "ZAMORA"]
}];

 
// Class definition
var electronicSign = function () {
	// Base elements
	var wizardEl;
	var formEl;
	var btnSave;
	var validator;
	var wizard;	

	// Private functions
	var initWizard = function () {
		// Initialize form wizard
		wizard = new KTWizard('kt_wizard_v1', {
			startStep: 1, // initial active step number
			clickableSteps: true  // allow step clicking
		});

		// Validation before going to next page
		wizard.on('beforeNext', function (wizardObj) {
			validator.form();
			if (validator.form() !== true ) {
				wizardObj.stop();  // don't go to the next step
			}			
			/*enter = false;*/
		});

		wizard.on('beforePrev', function (wizardObj) {
			//validator.form();
			//if (validator.form() !== true) {
			//	wizardObj.stop();  // don't go to the next step
			//}			
		});

		// Change event
		wizard.on('change', function (wizard) {
			setTimeout(function () {
				KTUtil.scrollTop();
			}, 500);
		});
	}
	var initValidation = function () {
		validator = formEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {

				RUC: {
					required: true
				},
				BusinessName: {
					required: true
				},
				BusinessAddress: {
					required: true
				},
				City: {
					required: true
				},
				Province: {
					required: true
				},
				Country: {
					required: true
				},
				Email: {
					required: true,
					email: true				
				},
				Email2: {					
					email: false
				},
				Phone: {
					required: true
				},	
				CedulaFrontFile: {
					required: true,
					fileSize: 2
				},
				CedulaBackFile: {
					required: true,
					fileSize: 2
				},
				SelfieFile: {					
					fileSize: 2
				},
				RucFile: {
					required: true,
					fileSize: 2
				},
				DesignationFile: {
					fileSize: 5
				},
				ConstitutionFile: {
					fileSize: 5
				},
				ConfirmEmail: {
					required: true,
					equalTo: "#Email"
				},
				ConfirmEmail2: {
					equalTo: "#Email2",
					email: false
				},
				ConfirmPhone: {
					required: true,
					equalTo: "#Phone"
				},
				ConfirmPhone2: {					
					equalTo: "#Phone2"
				},	
			},
			messages: {
				ConfirmEmail: {
					equalTo: "Por favor, escribe el mismo email"					
				},
				ConfirmEmail2: {
					equalTo: "Por favor, escribe el mismo email2"
				},
				ConfirmPhone: {
					equalTo: "Por favor, escribe el mismo número de celular",
					pattern: "Debe ingresar un número de celular válido (09XXXXXXXXXX)"
				},
				ConfirmPhone2: {
					equalTo: "Por favor, escribe el mismo número de celular2",
					pattern: "Debe ingresar un número de celular válido (09XXXXXXXXXX)"
				},
				Phone: {					
					pattern: "Debe ingresar un número de celular válido (09XXXXXXXXXX)"
				},
				Phone2: {					
					pattern: "Debe ingresar un número de celular válido (09XXXXXXXXXX)"
				}
			},
			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "Firma Electrónica",
					"html": "Hay algunos errores en la solicitud<br> por favor corrijalos y presione continuar.",
					"type": "error",
					"confirmButtonClass": "btn btn-secondary"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});
	}	
	var initSubmit = function () {

		formEl.on("submit", function (e) {
			e.preventDefault();			

			if (validator.form() && formEl.valid()) {
				// See: src\js\framework\base\app.js				

				KTApp.progress(btnSave);
				showLoader();

				var actionUrl = formEl[0].action;
				var formData = new FormData(formEl[0]);
				if (electronicSign.SelfieTomar) {
					electronicSign.SelfieFiles.forEach(function (file, idx) {
						formData.append('SelfieStatFile', file);
					});
                }

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
							toastr.error("Hubo un error al guardar su solicitud...");
							return;
						}

						if (data.includes("UserMessage")) {
							data = JSON.parse(data);							
							if (data.IsSuccess && data.Entity !== null) {
								toastr.success("Su orden ha sido generada!");
								//window.location.replace(electronicSign.PaymentUrl + "?purchaseorderid=" + data.Entity.PurchaseOrderId);								
								window.location.replace(data.Entity.PurchaseOrder.UrlRedirect);

							} else {
								var msg = data.UserMessage ? data.UserMessage : data.DevMessage ?? 'Hubo un error al guardar su solicitud';
								toastr.error(msg);

								if (msg.includes("Ya existe")) {
									window.location.replace(electronicSign.HomeUrl);
								}
							}
						}
                        else {
							toastr.error(data || "Hubo un error al guardar su solicitud...");
                        }
					}
				}).fail(function () {
					e.preventDefault();

				}).always(function () {
					KTApp.unprogress(btnSave);
					hideLoader();

				});
            } else {
				KTUtil.scrollTop();

				swal.fire({
					"title": "Firma Electrónica",
					"html": "Hay algunos errores en la solicitud<br> por favor corrijalos y presione continuar.",
					"type": "error",
					"confirmButtonClass": "btn btn-secondary"
				});
            }
		});


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
	var handleSelectFile = function (input) {
		if (input) {
			 $("#" + input).click();			
		}
	};
	var onChangeTipoPersona = function () {
		tipofirma = $("#SignType").val();
		switch (tipofirma) {
			case '1':
				$('#doc_juridico').hide();				
				$('.doc_juridico').hide();
				$('.legal-person').hide();
				$('.doc_natural').show();
				$('.natural-person').show();

				$("#DesignationFile").removeProp("required");
				$("#ConstitutionFile").removeProp("required");
				$('#WorkPosition').removeProp("required");
				$('#carg_juridico').hide();
				$('#WorkPosition').hide();
				
				break;
			case '2':
				$('#doc_juridico').show();				
				$('.doc_juridico').show();
				$('.legal-person').show();
				$('.doc_natural').hide();
				$('.natural-person').hide();

				$("#DesignationFile").prop("required", "required");
				$("#ConstitutionFile").prop("required", "required");
				$("#WorkPosition").prop("required", "required");
				$('#WorkPosition').show();
				$('#carg_juridico').show();
				break;
		}

		onChangeFechaNacimiento();
	};
	var onChangeFechaNacimiento = function () {
		var _ageAutorice = $('#video_autorizacion');
		var dateNow = new Date();
		var birthDate = $("#BirthDate").val();
		var _birthDate = new Date();
		var edad = 0;
		if (birthDate.length === 10)
		{
			if (birthDate.match(/^\d{1,2}\/\d{1,2}\/\d{2,4}$/)) {
				var _fecha = birthDate.split("/");				
				var years = new Date(_fecha[0], _fecha[1] - 1, _fecha[2]);
				_birthDate = years;
				edad = dateNow.getUTCFullYear() - _birthDate.getUTCFullYear();
				var meses = dateNow.getUTCMonth() - _birthDate.getUTCMonth();
				if (meses < 0) {edad--;}
				
			}
			else if (birthDate.match(/^\d{1,2}\-\d{1,2}\-\d{2,4}$/)) {
				var _fecha = birthDate.split("-");
				var years = new Date(_fecha[0], _fecha[1] - 1, _fecha[2]);
				_birthDate = years;
				edad = dateNow.getUTCFullYear() - _birthDate.getUTCFullYear();
				var meses = dateNow.getUTCMonth() - _birthDate.getUTCMonth();
				if (meses < 0) {edad--;}
			}
			else if (birthDate.match(/^\d{2,4}\/\d{1,2}\/\d{1,2}$/)) {

				var _fecha = birthDate.split("/");
				var years = new Date(_fecha[0], _fecha[1] - 1, _fecha[2]);
				_birthDate = years;
				edad = dateNow.getUTCFullYear() - _birthDate.getUTCFullYear();
				var meses = dateNow.getUTCMonth() - _birthDate.getUTCMonth();
				if (meses < 0) {edad--;}
			}
			else if (birthDate.match(/^\d{2,4}\-\d{1,2}\-\d{1,2}$/)) {
				var _fecha = birthDate.split("-");				
				var years = new Date(_fecha[0], _fecha[1] - 1, _fecha[2]);
				_birthDate = years;
				edad = dateNow.getUTCFullYear() - _birthDate.getUTCFullYear();
				var meses = dateNow.getUTCMonth() - _birthDate.getUTCMonth();
				if (meses < 0) {edad--;}

            }
			//var _birthDate = new Date(birthDate);
			//var edad = dateNow.getFullYear() - _birthDate.getFullYear();
			var tipofirma = $("#SignType").val();
			if (edad >= 65 && tipofirma ==="1") {
				$("#AuthorizationAgeFile").prop("required", "required");
				_ageAutorice.show();
			}
			else {
				$("#AuthorizationAgeFile").removeProp("required");
				_ageAutorice.hide();
				LimpiarVideo();
			}
		}
		else {
			$("#AuthorizationAgeFile").removeProp("required");
			_ageAutorice.hide();
			LimpiarVideo();
        }
	};
	var onChangeSelfieTomar = function () {				
		if (electronicSign.SelfieTomar) {
			$('#SelfieFile').val('')
			$('#SelfieFile').removeAttr("required");
			$("#SelfieFile").prop('required', false);		
			var labelElement = $("#size_file_selfie");			
			var iSize = electronicSign.SelfieTomaSize <= 0 ? 1024 : electronicSign.SelfieTomaSize;
			var Id = $("#Id").val();
			iSize = (Math.round(iSize * 100) / 100)			
			if (parseInt(Id) > 0) {
				SelectFileEditar("SelfieFile", "size_file_selfie", "Selfie.png");
			} else {
				labelElement.html('Tamaño de archivo: ' + iSize + "kb, Selfie.png");
				labelElement.attr('style', 'color: #9c9c9c');
			}
		}
	};
	var onSelectFile = function (input, label, max) {
		var inputElement = $("#" + input);
		var labelElement = $("#" + label);		
		inputElement.css("border-color", "");
		var info = "";
		var ext = '';
		if (inputElement[0].files.length > 0) {

			var iSize = (inputElement[0].files[0].size / 1024);
			var namefile = inputElement[0].files[0].name;
			resetTomarSelfie(input);

			if (iSize/1024 > 1) {
				iSize = (Math.round((iSize / 1024) * 100) / 100);
				if (iSize > max) {
					labelElement.html('Tamaño de archivo: ' + iSize + "Mb");
					labelElement.attr('style', 'color: #FC5555');
					inputElement.css("border-color", "#FC5555")
					return false;
				} else {					
					if (input === "AuthorizationFile") {
						info = "Archivo: " + namefile + "</br>" + 'Tamaño de archivo: ' + iSize + "Mb";
					}
					else {
						info = 'Tamaño:' + iSize + "Kb";
					} //"Archivo: " + namefile + "</br>" + 'Tamaño de archivo: ' + iSize + "Mb";

					//validar si es un video
					if (input === "AuthorizationAgeFile") {
						validarVideo(inputElement);
					}
					labelElement.html(info);
					labelElement.attr('style', 'color: #9c9c9c');
					SelectFileEditar(input, label, namefile);
					viewArchive(inputElement);
				}

			} else {
				iSize = (Math.round(iSize * 100) / 100)
				if (input === "AuthorizationFile") {
					info = "Archivo: " + namefile + "</br>" + 'Tamaño de archivo: ' + iSize + "Mb";
				}
				else {
					info = 'Tamaño:' + iSize + "Kb";
				} //"Archivo: " + namefile + "</br>" + 'Tamaño de archivo: ' + iSize + "Mb";

				//validar si es un video
				if (input === "AuthorizationAgeFile") {
					validarVideo(inputElement);
                }
				labelElement.html(info);				
				labelElement.attr('style', 'color: #9c9c9c');
				SelectFileEditar(input, label, namefile);
				viewArchive(inputElement);
			}

		} else {
			labelElement.html('Tamaño de archivo: 0Mb');
			labelElement.attr('style', 'color: #FC5555');
			inputElement.css("border-color", "#FC5555")
			return false;
		}

		return true;
	};
	var validarVideo = function (input) {
		var ext = '';
		var formatos = "mp4, mov, avi, flv, mkv, 3gp, mov, mpeg";		
		if (input[0].files.length > 0) {
			var namefile = input[0].files[0].name;
			ext = namefile.split('.');
			ext = ext[ext.length - 1];
			if (formatos.search(ext) < 0) {
				Swal.fire("Oops!", 'El archivo debe ser extensión mp4, mov, avi, flv, mkv, 3gp, mov, mpeg', "error");
				input.value = '';
				input.files[0].name = '';
				return false;
			}
			else {
				$("#AuthorizationAgeFormat").val(ext);
            }
        }
	};
	var viewArchive = function (input) {
		var ext;
		var nameInput;
		if (input[0].files.length > 0) {
			ext = input[0].files[0].name.split('.').pop();
			ext = ext.toLowerCase();
			if (input[0].name === "CedulaFrontFile") {
				nameInput = "img_file_cedfront";				
			}
			else if (input[0].name === "CedulaBackFile") {
				nameInput = "img_file_cedBack";				
			}
			else if (input[0].name === "SelfieFile") {
				nameInput = "img_file_selfie";				
			}
			else if (input[0].name === "RucFile") {
				nameInput = "pdf_file_ruc";				
			}
			else if (input[0].name === "DesignationFile") {
				nameInput = "pdf_file_nombramiento";				
			}
			else if (input[0].name === "ConstitutionFile") {
				nameInput = "pdf_file_constcomp";				
			}
			else if (input[0].name === "AuthorizationAgeFile") {
				nameInput = "vid_file_edadAut";
			}

			if (ext == 'pdf') {
				if (nameInput) {
					viewPdf(nameInput, input[0].files)
				}
			}
			else if (ext == 'jpeg' || ext == 'png') {
				viewImage(nameInput, input[0].files)
			}
			else {
				viewVideo(nameInput, input[0].files)
            }
		}	
		
	};
	var viewImage = function (input, file) {
		var img = $("#" + input);
		img.show();
		UrlImg = URL.createObjectURL(file[0]);
		img.attr('src', UrlImg);
		img.height = 50;
		img.onload = function () { URL.revokeObjectURL(this.src); }
	};
	var viewPdf = function (input, file) {
		var pdf = $("#" + input);
		pdf.show();
		UrlImg = URL.createObjectURL(file[0]);
		pdf.attr('src', UrlImg);
		pdf.onload = function () { URL.revokeObjectURL(this.src); }
	};
	var viewVideo = function (input, file) {
		var video = $("#" + input);
		//const video = document.getElementById("video");
		video.show();
		Urlvid = URL.createObjectURL(file[0]);
		video.attr('src', Urlvid);
	};
	var LimpiarVideo = function () {
		$("#AuthorizationAgeFile").val("");
		var video = $("#vid_file_edadAut");
		video.attr('src', "");
	};
	var SelectFileEditar = function (input, label, nameFile) {

		var styleOK = "fa fa-check text-success kt-icon-lg";
		var styleError = "fa fa-times text-danger kt-icon-lg";
		var Id = $("#Id").val();

		if (parseInt(Id) > 0) {

			var nameInput;
			var labelElement = $("#" + label);
			var info = "Archivo: " + nameFile + "</br>"+ labelElement.text();
			labelElement.html(info);
			labelElement.attr('style', 'color: #9c9c9c');

			if (input === "CedulaFrontFile") {
				nameInput = "Cedula_Front_File";				
				$("#Name_" + input).removeAttr("required");
				$("#Name_" + input + "-error").hide();
			}
			else if (input === "CedulaBackFile") {
				nameInput = "Cedula_Back_File";
				$("#Name_" + input).removeAttr("required");
				$("#Name_" + input + "-error").hide();
			}
			else if (input === "SelfieFile") {
				nameInput = "Selfie_File";
				$("#Name_" + input).removeAttr("required");
				$("#Name_" + input + "-error").hide();
			}
			else if (input === "RucFile") {
				nameInput = "Ruc_File";
				$("#Name_" + input).removeAttr("required");
				$("#Name_" + input + "-error").hide();
			}
			else if (input === "DesignationFile") {
				nameInput = "Designation_File";
				$("#Name_" + input).removeAttr("required");
				$("#Name_" + input + "-error").hide();
			}
			else if (input === "ConstitutionFile") {
				nameInput = "Constitution_File";
				$("#Name_" + input).removeAttr("required");
				$("#Name_" + input + "-error").hide();
			}
			else if (input === "AuthorizationAgeFile") {
				nameInput = "AuthorizationAge_File";
				$("#Name_" + input).removeAttr("required");
				$("#Name_" + input + "-error").hide();
			}

			var inputElement = $("#" + nameInput);
			inputElement.removeClass(styleError);
			inputElement.addClass(styleOK);
		}
	};
	var configure_adress = function () {
		var $province = $("#Province").val();
		$("#Province").empty();

		$("#Province").append("<option value>Seleccione la Provincia</option>");
		$province && $("#Province").append("<option selected value='" + $province + "'>" + $province + "</option>");

		provinciasCantones.forEach(function (obj) {
			$("#Province").append("<option value='" + obj.provincia + "'>" + obj.provincia + "</option>");
		});

		$("#Province").on("change", function () {
			var $val = $(this).val();
			var $province = provinciasCantones.find(obj => obj.provincia == $val);

			$("#City").empty();
			$("#City").append("<option value>Seleccione la Ciudad</option>");
			if ($province && $province.cantones && $province.cantones.length > 0) {
				$province.cantones.forEach(function (canton) {
					$("#City").append("<option value='" + canton + "'>" + canton + "</option>");
				});
			}
		});
	};
	var onChangeDocType = function () {
		var type = $("#DocumentType").val();
		var $id = $("#Identification");

		$id.prop("type", "text");
		$id.removeProp("maxlength");
		$id.removeProp("minlength");

		if (type == '1') {			
			$id.prop("type", "number");
			$id.prop("minlength", "10");
			$id.prop("maxlength", "10");			
			$id.attr("pattern", "^[0-9]*$");
			$('.fingerPrintCode-panel').show();
			$("#FingerPrintCode").prop("required", "required");
		} else if (type == '4') {
			$id.prop("type", "number");
			$id.prop("minlength", "13");
			$id.prop("maxlength", "13");			
		}
		else if (type == '6') {			
			$id.prop("type", "text");
			$id.prop("minlength", "6");
			$id.prop("maxlength", "12");			
			$id.attr("pattern", "^[A-Za-z0-9]*$");
			$('.fingerPrintCode-panel').hide();
			$("#FingerPrintCode").removeProp("required");
		}
	};
	var resetTomarSelfie = function (input) {
		if (input == 'SelfieFile') {
			electronicSign.SelfieTomar = false;
			electronicSign.SelfieFiles = [];
		}
	};
	var clickSelectFile = function () {
		var id = $(this).attr('value');
		if (id) {
			handleSelectFile(id);
		}
	};
	var configure_main = function () {
		wizardEl = KTUtil.get('kt_wizard_v1');
		formEl = $('#kt_form');
		btnSave = formEl.find('[data-ktwizard-type="action-submit"]');
		$("#DocumentType").on("change", onChangeDocType);

		if (parseInt(electronicSign.SupElectSign) === 1) {
			$("[type='email']").typeahead();
		}		

		//$("#Identification").on("change", function () {
		//	var $identity = $(this).val();

		//	if ($identity && $identity.length == 10) {
		//		$("#DocumentType").val(1); // es Cedula
		//	} else if ($identity && $identity.length == 13) {
		//		$("#DocumentType").val(4); // es RUC
		//	} else {
		//		$("#DocumentType").val(6); // es Pasaporte
		//	}

		//	onChangeDocType();
		//});

		$("#RUC").on("change", function () {
			var $ruc = $(this).val();
			if (validarRUCNatural($ruc)) {
				$("#SignType").val(1);
			} else {
				$("#SignType").val(2);
			}
			onChangeTipoPersona();
		});
		// Convierte en mayuscula las letras del texto en los controles
		$(".form-control").on("change", function () {
			var value = $(this).val();
			var obj = $(this);
			if (value && value.includes("@")) {
				$(this).val(value.toLowerCase());
			}
			else if (obj[0].className.includes("DiscountCode")){
				$(this).val(value);
			}
			else {
				$(this).val(value.toUpperCase());
			}
		});
		// evita que al momento de pulsar enter dentro de un control se desactive  wizard 
		$(".form-control").keypress(function (e) {			
			var code = (e.keyCode ? e.keyCode : e.which);
			if (code == 13) {				
				return false;
			}
		});
		$('.bootstrap-tagsinput').on('keypress', function (e) {
			if (e.keyCode == 13) {e.preventDefault();};
		});
		$("#SignType").on("change", onChangeTipoPersona);
		$("#CedulaFrontFile").on("change", function () { onSelectFile('CedulaFrontFile', 'size_file_cedfront', 2); });
		$("#CedulaBackFile").on("change", function () { onSelectFile('CedulaBackFile', 'size_file_cedBack', 2); });
		$("#SelfieFile").on("change", function () { onSelectFile('SelfieFile', 'size_file_selfie', 2); });
		$("#RucFile").on("change", function () { onSelectFile('RucFile', 'size_file_ruc', 2); });		
		$("#DesignationFile").on("change", function () { onSelectFile('DesignationFile', 'size_file_nombramiento', 5); });
		$("#ConstitutionFile").on("change", function () { onSelectFile('ConstitutionFile', 'size_file_constcomp', 5); });
		$("#AuthorizationFile").on("change", function () { onSelectFile('AuthorizationFile', 'size_file_authorization', 2); });
		$("#AuthorizationAgeFile").on("change", function () { onSelectFile('AuthorizationAgeFile', 'size_file_edadAut', 100); });
		$(".btn_select").on("click", clickSelectFile);		
		$(".btnAplicar").on("click", set_codeProm);
		$(".select-plan").on("click", select_plan);
		$("#BirthDate").on("change click", onChangeFechaNacimiento);

		$("#ConfirmEmail").on("blur", set_requestEmail);
		$("#Email").on("blur", set_requestEmail);

		$('#Phone').bind('cut copy paste', function (e) {
			Swal.fire("", "Función no permitida!", "warning");
			e.preventDefault();
		});

		$('#ConfirmPhone').bind('cut copy paste', function (e) {
			Swal.fire("", "Función no permitida!", "warning");
			e.preventDefault();
		});
				
		$('#Email').bind('cut copy paste', function (e) {
			Swal.fire("", "Función no permitida!", "warning");			
			e.preventDefault();	
		});

		$('#ConfirmEmail').bind('cut copy paste', function (e) {
			Swal.fire("", "Función no permitida!", "warning");			
			e.preventDefault();
		});

		$('#Email2').bind('cut copy paste', function (e) {
			Swal.fire("", "Función no permitida!", "warning");
			e.preventDefault();
		});

		$('#ConfirmEmail2').bind('cut copy paste', function (e) {
			Swal.fire("", "Función no permitida!", "warning");
			e.preventDefault();
		});

		$('#Phone2').bind('cut copy paste', function (e) {
			Swal.fire("", "Función no permitida!", "warning");
			e.preventDefault();
		});

		$('#ConfirmPhone2').bind('cut copy paste', function (e) {
			Swal.fire("", "Función no permitida!", "warning");
			e.preventDefault();
		});


		onChangeTipoPersona();
		onChangeDocType();
	};
	var select_plan = function () {
		$(".select-plan").html("Seleccionar");
		var $obj = $(this);
		var id = $obj[0].value;
		var plan = electronicSign.Plans.find(function (i) { return i.Id == id; });
		var name = "Plan seleccionado " + plan.Name + "(" + (plan.AmountDocument == "000000" ? "Emisión ilimitada":plan.AmountDocument) + " documentos)"
		$("#LicenceTypeId").val(id);
		$obj[0].innerHTML = "Seleccionado &nbsp;<i class='fa fa-check-circle'></i>";
		$("#LicenceTypeId").val(id);
		$("#plan_name").val(name);		
		cargar_valores_plan(plan);
		$("#plan_name").focus();
	}
	var validateSelectPlan = function () {
		var $obj = $("#LicenceTypeId");
		if ($obj.length) {
			if ($obj[0].value == "") {
				Swal.fire("Plans", "Debe seleccionar una plan para seguir con la solcictud!", "warning");
				return false;
			}
		}
		return true;
	}
	var cargar_valores_plan = function (plan) {
		if (plan) {			
			var objproduct = $(".name-product");
			var objprice = $(".price-product");
			var objdiscount = $(".totalDiscount");
			var objsubtotal = $(".subtotal");
			var objtotalIva = $(".totalIva");
			var objtotal = $(".total");
			var _objtotal = $("._total");
			var namePlane = "Plan " + plan.Name + "(" + (plan.AmountDocument == "000000" ? "Emisión ilimitada" : plan.AmountDocument) + " documentos), Incluye firma electrónica"

			var price = getValueSafe(plan.Price);
			var ivaRate = getValueSafe(plan.TaxBase);
			var descuentoRate = getValueSafe(plan.Discount);
			var valdesc = getValueSafe(price * (descuentoRate / 100), 2);
			var subtotal = getValueSafe(price - valdesc, 6);
			var iva = getValueSafe(subtotal * (ivaRate / 100), 4);
			var total = getValueSafe(subtotal + iva, 2);			
			objproduct[0].innerHTML = "Plan " + plan.Name;
			objprice[0].innerHTML = price.toFixed(2);
			objdiscount[0].innerHTML = valdesc.toFixed(2);
			objsubtotal[0].innerHTML = subtotal.toFixed(2);
			objtotalIva[0].innerHTML = iva.toFixed(2);
			objtotal[0].innerHTML = total.toFixed(2);
			_objtotal[0].innerHTML = total.toFixed(2);
			$(".Price").val(plan.Price);
			$(".IvaRate").val(plan.TaxBase);
			$(".Product-Name").val(namePlane);		
			
		}		
    }
	var set_codeProm = function () {
		var objDisCode = $(".DiscountCode");
		var $code = objDisCode[0].value;
		var _mensajeCode = $("#msg_code");
		var _imgCode = $("#imgCode");
		var btn = $(".btnAplicar");
		var $processing = false;		
		var mensaje = "";
		_mensajeCode[0].innerHTML = "";
		_imgCode.removeClass();

		if ($code) {
			var styleOK = "fa fa-check text-success kt-icon-lg";
			var styleError = "fa fa-times text-danger kt-icon-lg";						
			var req = { code: $code };
			btn.attr('disabled', 'disabled');
			KTApp.progress(btn);
			KTApp.block(_imgCode);

			$.post(electronicSign.CodePromUrl, req, function (data) {
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
					} else {
						_mensajeCode.attr('style', 'color: #FC5555');
						_imgCode.css("border-color", "#FC5555");
						_imgCode.removeClass(styleOK);
						_imgCode.addClass(styleError);						
						mensaje = data.UserMessage;
						btn.removeAttr("disabled");
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
	var aplicarPromocion = function (data) {

		price = 0.00;
		subtotal = 0.00;
		total_descuento = 0.00;
		total = 0.00;
		iva = 0.00;
		descuentoRate = 0.00;
		ivaRate = 0.00;
		valdesc = 0.00;

		var objprice = $(".Price");
		var objivaRate = $(".IvaRate");

		if (objprice) {

			price = getValueSafe(objprice[0].value);
			ivaRate = getValueSafe(objivaRate[0].value);
			descuentoRate = getValueSafe(data.DiscountRate);
			valdesc = getValueSafe(price * (descuentoRate / 100), 2);
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
	var set_requestEmail = function ()
	{
		$("#AuthorizationFile").removeAttr("required");		
		var $email = $("#Email").val();
		var $caract = new RegExp(/^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/);
		if ($caract.test($email)) {
			var $url = `${electronicSign.EmailUrl}?email=${$email}`;
			$.get($url, {}, function (data) {
				if (data) {
					if (data.id > 3) {
						Swal.fire("Oops!", data.mensaje, "warning");
						$("#AuthorizationFile").prop("required", "required");
						$("#Email2").prop("required", "required");
						$("#Phone2").prop("required", "required");
						$(".email-soliictud").show();
					}
					else {
						$("#AuthorizationFile").removeAttr("required");
						$("#AuthorizationFile-error").hide();
						$("#Email2").removeAttr("required");
						$("#Email2-error").hide();
						$("#Email2").removeClass('is-invalid')
						$("#Phone2").removeAttr("required");
						$("#Phone2-error").hide();
						$("#Phone2").removeClass('is-invalid');
						$(".email-soliictud").hide();
                    }
				}
			}).always(function () {
				$(".loading-identity").hide();
			});

		}
	};
	
	$(".btn-modCamara").on("click", electronicSignCamara);	
	
	return {
		PaymentUrl: "",
		EmailUrl: "",
		HomeUrl: "",
		VerUrl: "",	
		SupElectSign: 0,
		SelfieFiles: [],
		SelfieTomar: false,
		SelfieTomaSize: 0,
		CodePromUrl: "",
		Plans: [],
		// public functions
		Init: function () {
			configure_main();
			configure_adress();
			initWizard();
			initValidation();
			initSubmit(); 
			
		},
		SelfieStart: function () { onChangeSelfieTomar(); }

	};
}();

function electronicSignCamara() {
	showLoader();
	const video = document.getElementById('video');
	const canvas = document.getElementById('canvas');	
	///*$("#SelfieFile").prop("required", "required");*/

	var stream;
	const constraints = {
		audio: false,
		video: {
			width: 300, height: 300
		}
	};

	// Access webcam
	async function init() {
		try {
		    stream = await navigator.mediaDevices.getUserMedia(constraints);
			handleSuccess(stream);
		} catch (e) {
			toastr.error(`Tu navegador no soporta esta funcionalidad:${e.toString()}`); 
			$("#modCamara").modal('hide')
			hideLoader();
		}
	}

	// Success
	function handleSuccess(stream) {
		window.stream = stream;
		video.srcObject = stream;
		hideLoader();
		$('.btn_Grabar').show();		
	}

	// Load init
	init();

	// Draw image
	var context = canvas.getContext('2d');	
	$("#btnGrabar").on("click", function () {
		$('.btn_Guardar').show();
		canvas.width = video.videoWidth;
		canvas.height = video.videoHeight;
		context.drawImage(video, 0, 0, canvas.width, canvas.height);
		var data = canvas.toDataURL();
		if (data) {
			SelfieFiles(data);
		}		
		
	});

	$("#btnGuardar").on("click", function () {
		vidOff();
	});

	$(".modal_Close").on("click", function () {
		vidOff();
	});

	var vidOff = function () {
		stream.getTracks()[0].stop();
		video.pause();
		video.src = "";
		$("#modCamara").modal('hide');
		$('.btn_Guardar').hide();
    }

	var SelfieFiles = function (data) {
		if (data) {	
			fetch(data)
				.then(r => r.blob())
				.then(b => {
					electronicSign.SelfieFiles.push(new File([b], 'image.png', { type: b.type }));
					electronicSign.SelfieTomaSize = b.size;
				});					
			electronicSign.SelfieTomar = true;	
			electronicSign.SelfieStart();
		}
	}

};