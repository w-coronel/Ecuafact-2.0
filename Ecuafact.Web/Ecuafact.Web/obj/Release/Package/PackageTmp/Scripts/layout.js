////////
////////    
////////    Scripts Basicos de Forma
////////
/**
 * Algoritmo para validar cedulas de Ecuador
 * @param {string} cedula
 */
var validarCedula = function (cedula) {

    if (!cedula) {
        return false;
    }

    var cedula = cedula.trim();
    /**
     * Algoritmo para validar cedulas de Ecuador
     * @Author : Victor Diaz De La Gasca.
     * @Fecha  : Quito, 15 de Marzo del 2013
     * @Email  : vicmandlagasca@gmail.com
     * @Pasos  del algoritmo
     * 1.- Se debe validar que tenga 10 numeros
     * 2.- Se extrae los dos primero digitos de la izquierda y compruebo que existan las regiones
     * 3.- Extraigo el ultimo digito de la cedula
     * 4.- Extraigo Todos los pares y los sumo
     * 5.- Extraigo Los impares los multiplico x 2 si el numero resultante es mayor a 9 le restamos 9 al resultante
     * 6.- Extraigo el primer Digito de la suma (sumaPares + sumaImpares)
     * 7.- Conseguimos la decena inmediata del digito extraido del paso 6 (digito + 1) * 10
     * 8.- restamos la decena inmediata - suma / si la suma nos resulta 10, el decimo digito es cero
     * 9.- Paso 9 Comparamos el digito resultante con el ultimo digito de la cedula si son iguales todo OK sino existe error.
     */


    //Preguntamos si la cedula consta de 10 digitos
    if (cedula.length == 10) {

        //Obtenemos el digito de la region que sonlos dos primeros digitos
        var digito_region = cedula.substring(0, 2);

        //Pregunto si la region existe ecuador se divide en 24 regiones
        if (digito_region >= 1 && digito_region <= 24) {

            // Extraigo el ultimo digito
            var ultimo_digito = cedula.substring(9, 10);

            //Agrupo todos los pares y los sumo
            var pares = parseInt(cedula.substring(1, 2)) + parseInt(cedula.substring(3, 4)) + parseInt(cedula.substring(5, 6)) + parseInt(cedula.substring(7, 8));

            //Agrupo los impares, los multiplico por un factor de 2, si la resultante es > que 9 le restamos el 9 a la resultante
            var numero1 = cedula.substring(0, 1);
            var numero1 = (numero1 * 2);
            if (numero1 > 9) {
                var numero1 = (numero1 - 9);
            }

            var numero3 = cedula.substring(2, 3);
            var numero3 = (numero3 * 2);
            if (numero3 > 9) {
                var numero3 = (numero3 - 9);
            }

            var numero5 = cedula.substring(4, 5);
            var numero5 = (numero5 * 2);
            if (numero5 > 9) {
                var numero5 = (numero5 - 9);
            }

            var numero7 = cedula.substring(6, 7);
            var numero7 = (numero7 * 2);
            if (numero7 > 9) {
                var numero7 = (numero7 - 9);
            }

            var numero9 = cedula.substring(8, 9);
            var numero9 = (numero9 * 2);
            if (numero9 > 9) {
                var numero9 = (numero9 - 9);
            }

            var impares = numero1 + numero3 + numero5 + numero7 + numero9;

            //Suma total
            var suma_total = (pares + impares);

            //extraemos el primero digito
            var primer_digito_suma = String(suma_total).substring(0, 1);

            //Obtenemos la decena inmediata
            var decena = (parseInt(primer_digito_suma) + 1) * 10;

            //Obtenemos la resta de la decena inmediata - la suma_total esto nos da el digito validador
            var digito_validador = decena - suma_total;

            //Si el digito validador es = a 10 toma el valor de 0
            if (digito_validador == 10)
                var digito_validador = 0;

            //Validamos que el digito validador sea igual al de la cedula
            if (digito_validador == ultimo_digito) {
                // console.log('la cedula:' + cedula + ' es correcta');
                return true;
            } else {
                // console.log('la cedula:' + cedula + ' es incorrecta');
                return false;
            }

        } else {
            // imprimimos en consola si la region no pertenece
            // console.log('Esta cedula no pertenece a ninguna region');
            return false;
        }
    } else {
        //imprimimos en consola si la cedula tiene mas o menos de 10 digitos
        // console.log('Esta cedula tiene menos de 10 Digitos');
        return false;
    }


}


var validarRUCNatural = function (ruc) {
    if (!ruc || ruc.length != 13) {
        return false;
    }

    var cedula = ruc.substring(0, 10);

    return validarCedula(cedula);
}

/**
 * Realiza la validacion de correos electronicos
 * @param {string} email
 */
var validarEmail = function (email) {
    if (/^((([a-zA-Z\-0-9\._]+@)([a-zA-Z\-0-9\.]+)[;, ]*)+)$/.test(email)) {
        return true;
    } else {
        return false;
    }
}

/**
 * Realiza la validacion de correos electronicos
 * @param {string} phone
 */
var validarTelefono = function (phone) {
    if (/^\d*(?:(,?;? ?\/?\|?-?)\d?)*$/.test(phone)) {
        return true;
    } else {
        return false;
    }
}


var isTrue = function (x) {
    return typeof x == 'boolean' ? x : x == 'true' || x == 1;
}

if ($.validator) {



    $.validator.addMethod("multiplePhone", function (value, element) {
        if (value) {
            return validarTelefono(value.trim());
        }
        return true;
    });

    $.validator.addMethod("multipleEmail", function (value, element) {
        if (value) {
            return validarEmail(value.trim());
        }
        return true;
    });


    $.validator.addMethod("onlyNumbers", function (value, element) {
        if (value) {
            return /^[0-9\s]+$/.test(value);
        }
        return true;
    });


    $.validator.addMethod("onlyLetters", function (value, element) {
        if (value) {
            return /^[A-Za-zÑñÜü\s]+$/.test(value);
        }
        return true;
    });


    $.validator.addMethod("onlyWords", function (value, element) {
        if (value) {
            valueSplit = value.split(" ");
            return /^[A-Za-z0-9ÑñÜü(),.\s]+$/.test(value) && (valueSplit.length > 1);
        }
        return true;
    });


    $.validator.addMethod('fileSize', function (value, element, param) {
        if (this.optional(element)) {
            return true;
        }

        if (element && element.files && element.files.length > 0) {
            var iSize = (element.files[0].size / 1024);
            if (iSize / 1024 > 1) {
                iSize = (Math.round((iSize / 1024) * 100) / 100);

                if (iSize > param) {
                    return false;
                }
            }
        } else {
            return false;
        }

        return true;
    }, 'Disminuya el tamaño del archivo (máx {0} MB)');



    $.extend($.validator.messages, {

        required: "Este campo es obligatorio.",
        remote: "Por favor, rellena este campo.",
        email: "Por favor, escribe una dirección de correo válida",
        url: "Por favor, escribe una URL válida.",
        date: "Por favor, escribe una fecha válida.",
        dateISO: "Por favor, escribe una fecha (ISO) válida.",
        number: "Por favor, escribe un número entero válido.",
        digits: "Por favor, escribe sólo dígitos.",
        creditcard: "Por favor, escribe un número de tarjeta válido.",
        equalTo: "Por favor, escribe el mismo valor de nuevo.",
        accept: "Por favor, seleccione un archivo del tipo, formato o extensión permitida.",
        maxlength: $.validator.format("Por favor, no escribas más de {0} caracteres."),
        minlength: $.validator.format("Por favor, no escribas menos de {0} caracteres."),
        rangelength: $.validator.format("Por favor, escribe un valor entre {0} y {1} caracteres."),
        range: $.validator.format("Por favor, escribe un valor entre {0} y {1}."),
        max: $.validator.format("Por favor, escribe un valor menor o igual a {0}."),
        min: $.validator.format("Por favor, escribe un valor mayor o igual a {0}."),
        step: $.validator.format("Por favor, escribe un multiplo de {0}."),
        tel: "Por favor, escribe un número de teléfono válido.",
        multiplePhone: "Por favor, escribe un número de teléfono válido.",
        multipleEmail: "Por favor, escribe una dirección de correo válido.",
        onlyNumbers: "Por favor, escribe sólo números.",
        onlyLetters: "Por favor, escribe sólo letras.",

    });

}

// HELPER FUNCTIONS:



if (moment && moment().locale) {
    moment().locale("es");
}

var showDisabledMsg = function () {
    var disabledText = "Esta opcion no esta disponible.\n" +
        "Se requiere que usted configure su cuenta para la Emisión de Documentos Electrónicos.\n" +
        "[ Sistema > Configuracion > Emisión ]";

    Swal.fire("No Disponible", disabledText, "warning");
};

var refreshMethod = function () { };

var searchMethod = function () { }

/**
 * Convertir Json String Date a Fecha
 * @param {string} text
 */
var dateFromJson = function (text) {
    if (text) {
        if (typeof text == 'string') {
            var dt = text.replace("/Date(", "");
            dt = dt.replace(")/", "");
            dt = parseFloat(dt);
            return new Date(dt)
        }
        else if (typeof text == "number") {
            return new Date(text);
        }
    }
    return text;
}

if ($.fn.datepicker) {
    $.fn.datepicker.dates['es'] = {
        days: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"],
        daysShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
        daysMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
        months: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
        monthsShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
        today: "Hoy",
        clear: "Limpiar",
        format: "dd/mm/yyyy",
        titleFormat: "MM yyyy", /* Leverages same syntax as 'format' */
        weekStart: 0
    };

}

////////////////////////////////////////////////////////////////////////
//FUNCIONES BASICAS
var formatNumDoc = function (text) {
    if (text !== null) {
        text = text.replace(/\D/g, '');
    }
    else {
        text = "";
    }
    console.log(text);
    var mth = text.match(/^(\d{3})(\d{3})(\d{0,15})$/);

    if (mth) {
        var numDoc = mth[3];
        if (numDoc !== null) {
            if ((numDoc * 1) == 0) {
                numDoc = '1';
            } 
            numDoc = numDoc.padStart(9, "0");
        }

        if (mth[1] == '000') {
            mth[1] = '001'
        }

        if (mth[2] == '000') {
            mth[2] = '001'
        }

        text = mth[1] + "-" + mth[2] + "-" + numDoc;
    }
    else {
        if (text && (text*1) > 0) {
            text = "001-001-" + text.padStart(9, "0");    
        }
        else {
            text = '';
        }
    }

    return text;
}


function formatNumDAU(text) {
    if (text && text !== null) {
        text = text.replace(/\D/g, '');
    }
    else {
        text = "0";
    }

    var mth = text.match(/^(\d{0,3})(\d{0,4})(\d{0,2})(\d{0,8})$/);

    if (mth) {
        var num1 = mth[1];
        if (!num1 || num1 === null) {
            num1 = "0";
        }

        var num2 = mth[2];
        if (!num2 || num2 === null) {
            num2 = "0";
        }

        var num3 = mth[3];
        if (!num3 || num3 === null) {
            num3 = "0";
        }

        var num4 = mth[4];
        if (!num4 || num4 === null) {
            num4 = "0";
        }

        num1 = num1.padStart(3, "0");
        num2 = num2.padStart(4, "0");
        num3 = num3.padStart(2, "0");
        num4 = num4.padStart(8, "0");

        text = num1 + "-" + num2 + "-" + num3 + "-" + num4;
    }

    return text;

}

function formatPlate(text) {
    if (text && text !== null) {
        text = text.replace(/\W/g, '');
        text = text.toUpperCase();
    }
    else {
        text = "";
    }

    var mth = text.match(/^(\w{0,3})(\d{0,4})$/);

    if (mth) {
        var infDoc = mth[1];
        if (infDoc && infDoc !== null) {
            infDoc = infDoc.padEnd(3, "X");
        }

        var numDoc = mth[2];
        if (numDoc && numDoc !== null) {
            numDoc = numDoc.padStart(4, "0");
        }

        text = infDoc + "-" + numDoc;
    }

    return text;

}

function formatDateColumn(data, type, row) {
    if (data && typeof data != 'undefined' && (type === "display" || type === "filter")) {

        return data.toLocaleDateString();
    }

    return data;
}

function formatDateColumnII(data, type, row) {
    if (data === null) return "";
    var pattern = /Date\(([^)]+)\)/; //date format from server side
    var results = pattern.exec(data);
    var dt = new Date(parseFloat(results[1]));
    if (dt.getFullYear() === 9999) return ""; //Control para MaxValue
    return dt.getDate() + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
}

function formatPercentageColumn(data, type, row) {
    if (data && typeof data != 'undefined' && (type === "display" || type === "filter")) {

        return data + " %";
    }

    return data;
}

var formatMoneyColumn = function (data, type, row) {
    if (data && typeof data != 'undefined' && (type === "display" || type === "filter")) {

        return "$ " + (data * 1).toFixed(2);
    }

    return "$ 0.00";
}



var rangePickerOptions = {
    locale: {
        format: 'DD/MM/YYYY',
        separator: ' - ',
        applyLabel: 'OK',
        cancelLabel: 'Cancelar',
        fromLabel: 'Desde',
        toLabel: 'Hasta',
        customRangeLabel: 'Personalizar',
        weekLabel: 'S',
        daysOfWeek: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa'],
        monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        firstDay: 1
    },
    opens: 'right',
    format: 'DD/MM/YYYY',
    separator: ' hasta ',
    startDate: moment().subtract(29, 'days'),
    endDate: moment(),
    ranges: {
        'Hoy': [moment(), moment()],
        'Últimos 7 días': [moment().subtract(6, 'days'), moment()],
        'Últimos 30 días': [moment().subtract(29, 'days'), moment()],
        'Este Mes': [moment().startOf('month'), moment().endOf('month')],
        'Mes Anterior': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
        'Este Año': [moment().startOf('year'), moment().endOf('year')],
        'Año Anterior': [moment().subtract(1, 'year').startOf('year'), moment().subtract(1, 'year').endOf('year')]
    },
    minDate: '01/01/2000',
    maxDate: '31/12/2200'
};

var createRangePicker = function (rangeName, rangeMethod, startDate, endDate) {
    var options = Object.assign({}, rangePickerOptions);
    if (startDate) {
        options.startDate = startDate;
    }
    if (endDate) {
        options.endDate = endDate;
    }
    $(rangeName).daterangepicker(options,
        function (start, end) {
            rangeMethod(start, end);
        }
    );
};


/**
 * Convierte un valor tipo texto en el objeto Clase Moment.
 * @param {string} text
 */
var convertToMoment = function (text) {

    if (typeof text == 'string') {
        if (text.includes("/")) {
            var aDate = text.split("/");
            var day = aDate[0];
            var month = aDate[1];
            var year = aDate[2];
            return moment(year + '-' + month + '-' + day).locale("es");
        }
    }

    return moment(text).locale("es");
};


/**
 * Convierte los valor tipo texto a un objeto Date (fecha)
 * @param {string} text
 */
var convertToDate = function (text) {
    if (typeof text == 'string') {
        var aDate = text.split("/");
        var day = aDate[0];
        var month = aDate[1];
        var year = aDate[2];

        return new Date(Date.parse(year + '-' + month + '-' + day) * 1.000011544);
    }
    else if (typeof text == 'number') {
        return new Date(parseInt(text));
    }
    else {
        return new Date(text);
    }
};


var DTSpanish = {
    "sProcessing": "Procesando...",
    "sLengthMenu": "Mostrar _MENU_ registros",
    "sZeroRecords": "No se encontraron resultados",
    "sEmptyTable": "No existen registros disponibles en esta tabla",
    "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
    "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
    "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
    "sInfoPostFix": "",
    "sSearch": "Filtrar:",
    "sUrl": "",
    "sInfoThousands": ",",
    "sLoadingRecords": "Cargando...",
    "oPaginate": {
        "sFirst": "Primero",
        "sLast": "Último",
        "sNext": "Siguiente",
        "sPrevious": "Anterior"
    },
    "oAria": {
        "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
        "sSortDescending": ": Activar para ordenar la columna de manera descendente"
    }
};


$.fn.serializeJSON = function () {

    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });

    return o;
};

$.fn.serializeJSONString = function () {

    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return JSON.stringify(o);
};


var datePickerOptions = {
    format: "dd/mm/yyyy",
    language: "es",
    autoclose: true,
    orientation: "bottom"
};



var showLoader = function (html) {
    if (typeof html === 'string') {
        $("#loaderV1Text").html(html);
    }
    else {
        $("#loaderV1Text").html("");
    }

    $(".loaderV1").fadeIn();
}

var hideLoader = function () {
    $(".loaderV1").fadeOut();
}


/**
 * Mostrar una ventana modal
 * @param {any} url: URL
 * @param {any} args: Argumentos
 * @param {any} div: Elemento
 */
window.showModal = function (url, args, div) {
    if (!args) {
        args = {};
    }

    if (!div) {
        div = "#myModal";
    }

    $div = $(div);

    showLoader();
    $.get(url, args, function (data) {
        if (!data.includes("kt_login")) {
            // ahora valido si incluye la estructura basica de un modal:
            if (!data.includes("modal-dialog")) {
                data = '<div class="modal-dialog" style="max-width:90%" role="document"><div class="modal-content"><div class="modal-header">' +
                    '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                    '</div> <div class="modal-body">' + data + '</div></div></div>';
            }

            $div.html(data);
            $div.fadeIn();
            $div.modal("show");
        }
        else {
            window.open(location.origin + "/auth", "_login");
        }

        hideLoader();
    }).always(function () {
        hideLoader();
    });
}


var hideModal = function (div) {
    if (!div) {
        div = "#myModal";
    }

    $div = $(div);
    $div.fadeOut();
    $div.empty();
    $div.modal("hide");
}


var getValueSafe = function (text, dec) {
    dec = dec * 1;
    if (isNaN(dec) || dec < 0 || dec > 6) {
        dec = 6;
    }

    var value = 0.000000;

    if (typeof text == 'object')
        if (typeof text.value == 'string' || typeof text.value == 'number') {
            text = text.value;
        }
        else if (typeof text.val !== 'undefined') {
            text = text.val();
        }


    if (typeof text == 'number') {
        value = text;
    }
    else if (typeof text !== 'undefined') {
        text = text.toString();

        if (typeof text == 'string') {

            // Analizo si es un numero en especial:
            if (text.lastIndexOf(".") > 0) {

                while (text.lastIndexOf(",") > 0) {
                    text = text.replace(",", "");
                }
            }

            if (text.split(",").length > 2) {
                text = text.replace(",", "");
            }
            else {
                text = text.replace(",", ".");
            }


            // Elimino del String los caracteres especiales que contenga
            var mth = text.match(/[-?\d\.]+/);
            value = parseFloat(mth);
        }
    }

    if (isNaN(value)) {
        value = 0.000000;
    }

    value = value.toFixed(dec);

    return parseFloat(value);
}


var fixUp = function (value) {

    var result = value;

    if (result) {

        if (typeof result == 'number') {
            result = result.toFixed(6);
        }

        if (result.endsWith(".000000") || result.endsWith(",000000")) {
            result = result.substring(0, result.length - 7);
        }
        else if (result.endsWith("0000")) {
            var idx = result.lastIndexOf("0000");
            result = result.substring(0, idx);
        }
        else if (result.endsWith("00")) {
            var idx = result.lastIndexOf("00");
            result = result.substring(0, idx);
        }
    }

    return result;
};


var setValueSafe = function (obj, value, format) {

    if (typeof obj !== 'undefined') {

        if (typeof obj[0] !== 'undefined' && obj[0] !== null) {
            obj = obj[0];
        }

        // Si es numero lo formatea para que sea un numero
        if (typeof value == 'number') {
            if (format) {

                value = value.toLocaleString('en-US', {
                    style: 'currency',
                    currency: 'USD'
                });

            }
            else {

                value = value
                    .toFixed(2)
                    .toLocaleString('en-US');

            }
        }

        obj.value = value;

        $(obj).trigger('change');
    }
}


$.fn.upperText = function () {

    $(this).on("change keyup keydown", function () {
        var $me = $(this);

        if ($me.prop("type") == "hidden") {
            return;
        }

        var value = $me.val();

        if (value) {
            if (value.includes("@")) {
                $me.val(value.toLowerCase());
            } else {
                $me.val(value.toUpperCase());
            }
        }
    });

};


$.fn.totalText = function () {

    $(this).on("change keyup keydown", function () {

        var $sub = $(this);
        var $value = $sub.val();
        var $field = $sub.attr("name");

        $("[data-field='" + $field + "']").text($value);

    });

};

function initControls() {
   window.location.hash = "no-back-button";
    window.location.hash = "Again-No-back-button";//esta linea es necesaria para chrome
    window.onhashchange = function () { window.location.hash = ""; }
}

$(document).ready(function () {

    /*javascript: window.history.forward(1);*/
    //initControls(); 
    history.forward();

    // Convierte en mayuscula las letras del texto en los controles
    $(".upper-text").upperText();

    if (typeof $.fn.selectpicker != 'undefined') {
        $('.bs-select').selectpicker({
            iconBase: 'fa',
            tickIcon: 'fa-check'
        });
    }

    if (typeof $.fn.datepicker != 'undefined') {
        $('.date-picker').datepicker({
            format: "dd/mm/yyyy",
            language: "es",
            autoclose: true
        });

        $('.datepicker').datepicker(datePickerOptions);
        $('.date-picker').datepicker(datePickerOptions);
    }


    $(".sidebar-search input").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $(".nav-item li").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });

    var refreshButton = $(".refresh");
    if (refreshButton) {
        refreshButton.on("click", function () {
            refreshMethod();
        });
    }

    if (typeof $.fn.daterangepicker != 'undefined') {
        $('.range-date-picker').daterangepicker(rangePickerOptions,
            function (start, end) {
                $('.range-date-picker span').html(start.format('DD/MMMM/YYYY') + ' - ' + end.format('DD/MMMM/YYYY'));
                if (rangefunctions) {
                    for (var i = 0; i < rangefunctions.length; i++) {
                        rangefunctions[i](start, end);
                    }
                }
            }
        );
    }

    if ($.fn.counterUp) {

        $('.counter').counterUp({
            delay: 10,
            time: 1000
        });

    }

    if (toastr) {
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "newestOnTop": true,
            "progressBar": false,
            "positionClass": "toast-top-right",
            "preventDuplicates": true,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
    }

    if (searchMethod) {
        if ($(".search-form a").length > 0) {
            $(".search-form a").onclick = function () {
                var searchTerm = $(".search-form input").val();
                searchMethod(searchTerm);
            };
        }

        if ($("form.search-form").length > 0) {
            $("form.search-form").on("submit", function (e) {
                var searchTerm = $(".search-form input").val();

                e.preventDefault();
                searchMethod(searchTerm);
            });
        }
    }


    $('[type="checkbox"]').on("click", function (e) {
        var $me = $(this);
        var isChecked = $me.is(":checked");
        var $name = $me.attr("name");
        $("input[name='" + $name + "']").val(isChecked);
    });



    $(".sidebar-search input").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $(".nav-item li").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });


    $("input[type='number']").each(function (index, element) {
        var $me = $(element);
        if ($me.prop("maxlength") > 0) {

            element.onkeypress = function () {
                var maxlength = $(this).prop("maxlength");
                if (maxlength && this.value.length >= maxlength) {
                    return false;
                }
                
            };
        }
    });


    $(".kt-grid-nav__item").tooltip();

    $(".invoice-subtotal").totalText();

});