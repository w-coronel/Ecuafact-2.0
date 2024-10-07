function roundf(e, a) {
    var t = parseFloat(e),
        n = "0.00";
    return isNaN(t) || (t = Math.round(t * Math.pow(10, a)) / Math.pow(10, a), n = String(t), n += (-1 == n.indexOf(".") ? "." : "") + String(Math.pow(10, a)).substr(1), n = n.substr(0, n.indexOf(".") + a + 1)), n
}

function strPad(e, a, t) {
    return t = t || "0", e += "", e.length >= a ? e : new Array(a - e.length + 1).join(t) + e
}

function validarIdentificacion2(e, a) {
    if (error = !1, validar = !1, "05" == a ? (validar = !0, "10" != e.length && (error = !0)) : "04" == a ? (validar = !0, 13 != e.length && (error = !0)) : "00" == a && (validar = !0, 13 != e.length && 10 != e.length && (error = !0)), validar && !error) {
        if (13 == e.length) return !1;
        for (array = e.split(""), num = array.length, total = 0, digito = 1 * array[9], i = 0; i < 9; i++) mult = 0, i % 2 != 0 ? total += 1 * array[i] : (mult = 2 * array[i], mult > 9 ? total += mult - 9 : total += mult);
        decena = total / 10, decena = Math.floor(decena), decena = 10 * (decena + 1), final = decena - total, (10 == final && 0 != digito || final != digito) && (error = !0)
    }
    return error
}

function validarIdentificacion(e, a) {
    if (error = !1, validar = !1, 5 == a || 4 == a) {
        var t = e,
            n = 0,
            o = 0,
            r = !1,
            l = !1,
            u = !1,
            p = 11,
            d = 1;

        for (i = 0; i < t.length && 1 == d; i++) {
            var c = parseInt(t.charAt(i));
            isNaN(c) && (d = 0)
        }
        if (0 == d) return !0;
        if (10 != t.length && "05" == a) return !0;
        if (13 != t.length && "04" == a) return !0;
        if ("0960571719001" == t || "0960056208001" == t || "0960841922001" == t || "0962285938001" == t || "3050136161001" == t || "0962334777001" == t || "0960723955001" == t) return !1;
        if ("096" == t.substr(0, 3)) return !1;
        if (provincia = t.substr(0, 2), provincia < 1 || provincia > 24) return !0;
        if (d1 = t.substr(0, 1), d2 = t.substr(1, 1), d3 = t.substr(2, 1), d4 = t.substr(3, 1), d5 = t.substr(4, 1), d6 = t.substr(5, 1), d7 = t.substr(6, 1), d8 = t.substr(7, 1), d9 = t.substr(8, 1), d10 = t.substr(9, 1), 7 == d3 || 8 == d3) return !0;
        if (d3 < 6 ? (u = !0, p1 = 2 * d1, p1 >= 10 && (p1 -= 9), p2 = 1 * d2, p2 >= 10 && (p2 -= 9), p3 = 2 * d3, p3 >= 10 && (p3 -= 9), p4 = 1 * d4, p4 >= 10 && (p4 -= 9), p5 = 2 * d5, p5 >= 10 && (p5 -= 9), p6 = 1 * d6, p6 >= 10 && (p6 -= 9), p7 = 2 * d7, p7 >= 10 && (p7 -= 9), p8 = 1 * d8, p8 >= 10 && (p8 -= 9), p9 = 2 * d9, p9 >= 10 && (p9 -= 9), p = 10) : 6 == d3 ? (l = !0, p1 = 3 * d1, p2 = 2 * d2, p3 = 7 * d3, p4 = 6 * d4, p5 = 5 * d5, p6 = 4 * d6, p7 = 3 * d7, p8 = 2 * d8, p9 = 0) : 9 == d3 && (r = !0, p1 = 4 * d1, p2 = 3 * d2, p3 = 2 * d3, p4 = 7 * d4, p5 = 6 * d5, p6 = 5 * d6, p7 = 4 * d7, p8 = 3 * d8, p9 = 2 * d9), n = p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9, o = n % p, digitoVerificador = 0 == o ? 0 : p - o, 1 == l) {
            if (digitoVerificador != d9) return !0;
            if ("0001" != t.substr(9, 4)) return !0
        } else if (1 == r) {
            if (digitoVerificador != d10) return !0;
            if ("001" != t.substr(10, 3)) return !0
        } else if (1 == u) {
            if (digitoVerificador != d10) return !0;
            if (t.length > 10 && "001" != t.substr(10, 3)) return !0
        }
    }
    return !1
}

function esConsumidorFinal() {
    return "07" == $("select[name='cliente_tipo_identificacion']").val()
}

function generarJsonCliente() {
    var e = {};
    return e.razon_social = $('input[name="cliente_razon_social"]').val(),
        e.identificacion = $('input[name="cliente_identificacion"]').val(),
        e.tipo_identificacion = $('select[name="cliente_tipo_identificacion"]').val(),
        e.direccion = $('input[name="cliente_direccion"]').val(), e.email = $('input[name="cliente_email"]').val(),
        e.telefono = $('input[name="cliente_telefono"]').val(), e.fecha_inicio = $('input[name="start"]').val(),
        e.fecha_fin = $('input[name="end"]').val(), e.repeticiones = $('input[name="repeticiones"]').val(),
        e.periocidad = $('select[name="opciones_factura_periocidad"]').val(), e.descripcion = $('textarea[name="descripcion"]').val(),
        e
}

function sendXml(e, a, t) {
    $.ajax("/getPreviewDocument", {
        data: {
            xml: e,
            tipo_documento: a,
            establecimiento_id: t
        },
        dataType: "json",
        type: "POST",
        async: !1,
        success: function (e) {
            document.getElementById("divPreviewView").innerHTML = e.xml
        },
        error: function (e) { }
    })
}

function sendXmlPreFactura(e, a) {
    $.ajax("/getPreviewDocumentPreFactura", {
        data: {
            xml: e,
            establecimiento_id: a
        },
        dataType: "json",
        type: "POST",
        async: !1,
        success: function (e) {
            document.getElementById("divPreviewView").innerHTML = e.xml
        },
        error: function (e) { }
    })
}

function resizeIframe(e) {
    document.getElementById(e).height = "500px";
}

function onlyNumber(e) {
    var a = e.which ? e.which : e.keyCode;
    return !(a > 31 && (a < 48 || a > 57))
}

function onlyNumberAndMiddleScript(e) {
    var a = e.which ? e.which : e.keyCode;
    return !(a > 31 && (a < 48 || a > 57)) || 45 == a
}

function validarEmailCliente(e) {
    var a = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
        t = e.split(","),
        n = !1,
        i = "";
    return $.each(t, function (e, t) {
        i = t.split(";"), $.each(i, function (e, t) {
            a.test(t) || (n = !0, $(".bootstrap-tagsinput").addClass("errorTagsInput"))
        })
    }), n
}

function getIceXml() {
    var e = [];
    return $(".table-items tbody tr").each(function (a) {
        var t = [];
        if (t[0] = $(this).find('input[name="documento_ice_base_imponible[]"]').val(), t[1] = $(this).find('input[name="documento_ice_codigo[]"]').val(), t[2] = $(this).find('input[name="documento_ice_porcentaje[]"]').val(), t[3] = $(this).find('input[name="documento_ice_valor[]"]').val(), parseInt(t[1]) > 0) {
            for (is_new = !0, i = 0; i < e.length; i++) e[i][1] == t[1] && (e[i][0] = parseFloat(e[i][0]) + parseFloat(t[0]), e[i][3] = parseFloat(e[i][3]) + parseFloat(t[3]), is_new = !1);
            1 == is_new && e.push(t)
        }
    }), e
}

function calcularICE() {
    valor = $("input[name='documento_ice_porcentaje_campo']").val() * $("input[name='documento_ice_base_imponible_campo']").val() / 100, $("input[name='documento_ice_valor_campo']").val(roundf(valor, 2).toString())
}

function calcularICEReembolso() {
    valor = $("input[name='documento_ice_porcentaje_campo_reembolso']").val() * $("input[name='documento_ice_base_imponible_campo_reembolso']").val() / 100, $("input[name='documento_ice_valor_campo_reembolso']").val(roundf(valor, 2).toString())
}

function calcularIRBPNR() {
    valor = $("input[name='documento_irbpnr_porcentaje_campo']").val() * $("input[name='documento_irbpnr_base_imponible_campo']").val(), $("input[name='documento_irbpnr_valor_campo']").val(roundf(valor, 2).toString())
}

function calcularIRBPNRReembolso() {
    valor = $("input[name='documento_irbpnr_porcentaje_campo_reembolso']").val() * $("input[name='documento_irbpnr_base_imponible_campo_reembolso']").val(), $("input[name='documento_irbpnr_valor_campo_reembolso']").val(roundf(valor, 2).toString())
}

function calcularFactura(e) {
    var a = $('select[name="tipoFactura"]').val(),
        t = $('input[name="descuento-porcentaje"]').val(),
        n = 0,
        i = 0,
        o = 0,
        r = 0,
        l = 0,
        u = 0,
        p = 0,
        d = 0,
        c = 0,
        s = 0,
        m = 0,
        _ = 0,
        v = 0,
        f = 0,
        h = 0,
        b = 0,
        g = 0,
        F = 0,
        y = 0;
    $(".table-items tbody tr").each(function (e) {
        $(this).children("td").each(function (e) {
            switch (e) {
                case 0:
                case 1:
                    break;
                case 2:
                    n = $(this).find('input[name="item_cantidad[]"]').val(), $(this).find('input[name="item_cantidad[]"]').val(roundf(n, 6));
                    break;
                case 3:
                    o = $(this).find('input[name="item_precio_unitario[]"]').val(), $(this).find('input[name="item_precio_unitario[]"]').val(roundf(o, 6));
                    break;
                case 4:
                    1 == t ? (porcentajeDescuento = $(this).find('input[name="item_descuento[]"]').val(), i = parseFloat(o) * porcentajeDescuento * n / 100, $(this).find('input[name="item_descuento[]"]').text(roundf(porcentajeDescuento, 2) + "asdasdasdas")) : (i = $(this).find('input[name="item_descuento[]"]').val(), $(this).find('input[name="item_descuento[]"]').val(roundf(i, 2)));
                    break;
                case 5:
                    r = roundf(parseFloat(o) * parseFloat(n) - i, 2), $(this).find('input[name="item_precio_total[]"]').val(r);
                    break;
                case 6:
                    l = $(this).find('select[name="item_iva[]"]').val();
                    break;
                case 7:
                    _ = $(this).find('input[name="documento_ice_valor[]"]').val(), F = $(this).find('input[name="documento_irbpnr_valor[]"]').val()
            }
        }), 2 == l || 3 == l ? (p = parseFloat(p) + parseFloat(r), _ > 0 && (p += parseFloat(_), v += parseFloat(_))) : 0 == l ? (d = parseFloat(d) + parseFloat(r), _ > 0 && (d += parseFloat(_), f += parseFloat(_))) : 6 == l ? (m = parseFloat(m) + parseFloat(r), _ > 0 && (m += parseFloat(_), h += parseFloat(_))) : 7 == l && (s = parseFloat(s) + parseFloat(r), _ > 0 && (s += parseFloat(_), b += parseFloat(_))), _ > 0 && (g += parseFloat(_)), F > 0 && (y += parseFloat(F)), "0" != i && (c = parseFloat(c) + parseFloat(i))
    }), u += p * (e / 100), $("input[name='documento_subtotal_12']").val(roundf(p, 2).toString()), $("input[name='documento_subtotal_0']").val(roundf(d, 2).toString()), $("input[name='documento_subtotal_exento_iva']").val(roundf(s, 2).toString()), $("input[name='documento_subtotal_no_iva']").val(roundf(m, 2).toString()), $("input[name='documento_ice']").val(roundf(g, 2).toString()), $("input[name='documento_irbpnr']").val(roundf(y, 2).toString()), subtotal = p + d + s + m - v - f - b - h, $("input[name='documento_subtotal_sin_impuestos']").val(roundf(subtotal, 2).toString()), isPropinaChecked = $('input[name="documento_propina_check"]').is(":checked"), isPropinaChecked ? $("input[name='documento_propina']").val(roundf(10 * subtotal / 100, 2)) : $("input[name='documento_propina']").val("0.00"), propina = parseFloat($("input[name='documento_propina']").val()), isLeySolidariaChecked = $('input[name="documento_ley_solidaria_check"]').is(":checked"), isLeySolidariaChecked ? $("input[name='documento_ley_solidaria']").val(roundf(2 * p / 100, 2)) : $("input[name='documento_ley_solidaria']").val("0.00"), ley_solidaria = parseFloat($("input[name='documento_ley_solidaria']").val()), $("input[name='documento_total_descuento']").val(roundf(c, 2).toString()), $("input[name='documento_iva']").val(roundf(u.toString(), 2)), isNaN(g) && (g = 0), isNaN(y) && (y = 0);
    var S = 0;
    if ($(".table-rubros-terceros tbody tr").each(function (e) {
        var a = $(this).find('input[name="campo_rubros_terceros[]"]').val(),
            t = $(this).find('input[name="total_rubros_terceros[]"]').val();
        $(this).find('input[name="total_rubros_terceros[]"]').val(roundf(t, 2)), "" != a && "" != t && (S += parseFloat($(this).find('input[name="total_rubros_terceros[]"]').val()))
    }), valor_total = subtotal + u + propina + g + y + S - ley_solidaria, 2 == a) {
        $("input[name='fleteInternacional']").val(roundf($("input[name='fleteInternacional']").val(), 2));
        var I = parseFloat($("input[name='fleteInternacional']").val()) >= 0 ? parseFloat($("input[name='fleteInternacional']").val()) : 0;
        $("input[name='seguroInternacional']").val(roundf($("input[name='seguroInternacional']").val(), 2));
        var k = parseFloat($("input[name='seguroInternacional']").val()) >= 0 ? parseFloat($("input[name='seguroInternacional']").val()) : 0;
        $("input[name='gastosAduaneros']").val(roundf($("input[name='gastosAduaneros']").val(), 2));
        var w = parseFloat($("input[name='gastosAduaneros']").val()) >= 0 ? parseFloat($("input[name='gastosAduaneros']").val()) : 0;
        $("input[name='gastoTrasporte']").val(roundf($("input[name='gastoTrasporte']").val(), 2));
        var C = parseFloat($("input[name='gastoTrasporte']").val()) >= 0 ? parseFloat($("input[name='gastoTrasporte']").val()) : 0;
        valor_total += I + k + w + C
    }
    $("input[name='documento_valor_total']").val(roundf(valor_total, 2).toString()), $('input[name="documento_credito_check"]').is(":checked") ? 1 == $('input[name="total_forma_pago_credito[]"]').length && $('input[name="total_forma_pago_credito[]"]').val(roundf(valor_total, 2)) : 1 == $('input[name="total_forma_pago[]"]').length && $('input[name="total_forma_pago[]"]').val(roundf(valor_total, 2))
}

function limpiarRideFactura() {
    $(".ride-clean").html(""), $(".ride .ride-productos > tbody").html(""), $(".ride .ride-infoAdicional > tbody").html(""), $(".ride .ride-forma-pago > tbody").html(""), $(".ride .ride-impuesto0").html("0.00"), $(".ride .ride-impuesto12").html("0.00"), $(".ride .ride-impuesto-no-iva").html("0.00"), $(".ride .ride-impuesto-exento-iva").html("0.00")
}

function SoloComaPuntoyNumeros(e) {
    var a = e.keyCode;
    return (!e.shiftKey || 188 != a && 190 != a && 110 != a) && !(!SoloNumeros(e) && 188 != a && 190 != a && 110 != a)
}

function SoloNumeros(e) {
    var a = e.keyCode;
    return !(e.shiftKey && a >= 48 && a <= 57) && (a >= 48 && a <= 57 || a >= 35 && a <= 40 || a >= 96 && a <= 105 || 8 == a || 9 == a || 45 == a || 46 == a)
}