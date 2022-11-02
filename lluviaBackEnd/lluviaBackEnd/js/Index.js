var logoBase64;
//convierte una url de una imagen en base64
function toDataURL(src, callback, outputFormat) {
    var img = new Image();
    img.crossOrigin = 'Anonymous';
    img.onload = function () {
        var canvas = document.createElement('CANVAS');
        var ctx = canvas.getContext('2d');
        var dataURL;
        canvas.height = this.naturalHeight;
        canvas.width = this.naturalWidth;
        ctx.drawImage(this, 0, 0);
        dataURL = canvas.toDataURL(outputFormat);
        callback(dataURL);
    };
    img.src = src;
}

function ShowLoader(mensaje) {
    $('#textLoader').html((mensaje === 'undefined' || mensaje === '' ? "Cargando" : mensaje));
    $('#loader-lluvia').css('display', 'flex');
}


function OcultarLoader() {
    $('#loader-lluvia').css('display', 'none');
}

//ESTA FUNCION ES PARA NOTIFICACIONES 
function MuestraToast(tipoNotificacion, mensaje) {

    switch (tipoNotificacion) {

        case "success":
            iziToast.success({
                title: mensaje,
                message: '',
                position: 'topRight'
            });
            break;
        case "info":
            iziToast.info({
                title: mensaje,
                message: '',
                position: 'topRight'
            });
            break;
        case "warning":
            iziToast.warning({
                title: mensaje,
                message: '',
                position: 'topRight'
            });
            break;
        case "error":
            iziToast.error({
                title: mensaje,
                message: '',
                position: 'topRight'
            });
            break;

    }

}
function setFooterPDF(page, pages) {
    return {
        columns: [
            {
                alignment: 'right',
                text: ['pagina ', { text: page.toString() }, ' de ', { text: pages.toString() }]
            }
        ],
        margin: [0, 0, 30]
    }

}


function SetHeaderPDF(titulo) {

    return {
        columns: [
            {
                image: logoBase64,
                width: 64,
                margin: [0, 20, -20, 0]
            },
            /*{
                alignment: 'left',
                italics: true,
                text: 'dataTables',
                fontSize: 18,
                margin: [10,0]
            },*/
            {
                alignment: 'center',
                fontSize: 14,
                text: titulo,
                margin: [0, 40, 80]
            }
        ],
        margin: [10, 0]
    }


}

function initDataTable(nombreTabla) {

    let dataTable = $('#' + nombreTabla).DataTable({

        "language": {
            "lengthMenu": "Muestra _MENU_ registros por pagina",
            "zeroRecords": "No existen registros",
            "info": "Pagina _PAGE_ de _PAGES_",
            "infoEmpty": "No existe informacion para mostrar",
            "infoFiltered": "(filtered from _MAX_ total records)",
            "search": "Buscar:",
            "paginate": {
                "first": "Primero",
                "last": "Ultimo",
                "next": "Siguiente",
                "previous": "Anterior"
            },
        },
        "dom": 'lfrtip',


        "bDestroy": true, // es necesario para poder ejecutar la funcion LimpiaTabla()
        "ordering": false,
        "pageLength": 25,
    });


    $('#' + nombreTabla + '_length').css("margin-top", "20px");
    return dataTable;

}


function InitRangePicker(nombrePicker, fechaIni, fechaFin) {

    $('#' + nombrePicker).daterangepicker({
        singleDatePicker: false,
        showDropdowns: true,
        locale: {
            "format": "YYYY-MM-DD",
            "separator": " - ",
            "applyLabel": "Aceptar",
            "cancelLabel": "Cancelar",
            "fromLabel": "De",
            "toLabel": "Hasta",
            "customRangeLabel": "Custom",
            "daysOfWeek": [
                "Dom",
                "Lun",
                "Mar",
                "Mié",
                "Juv",
                "Vie",
                "Sáb"
            ],
            "monthNames": [
                "Enero",
                "Febrero",
                "Marzo",
                "Abril",
                "Mayo",
                "Junio",
                "Julio",
                "Augosto",
                "Septiembre",
                "Octubre",
                "Noviembre",
                "Diciembre"
            ],
            "firstDay": 1
        }
    }, function (start, end, label) {
        $('#' + fechaIni).val(start.format('YYYY-MM-DD'));
        $('#' + fechaFin).val(end.format('YYYY-MM-DD'));
    });

    $('#' + nombrePicker).on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('YYYY-MM-DD') + ' - ' + picker.endDate.format('YYYY-MM-DD'));
        $('#' + fechaIni).val(picker.startDate.format('YYYY-MM-DD'));
        $('#' + fechaFin).val(picker.endDate.format('YYYY-MM-DD'));

    });

    $('#' + nombrePicker).on('cancel.daterangepicker', function (ev, picker) {
        $('#' + nombrePicker).val('');
        $('#' + fechaIni).val('');
        $('#' + fechaFin).val('');

    });

}


function esNumero(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

//function esDecimal(txt, evt) {
//    var charCode = (evt.which) ? evt.which : evt.keyCode;
//    if (charCode == 46) {
//        if (txt.value.indexOf('.') === -1) {
//            return true;
//        } else {
//            return false;
//        }
//    } else {
//        if (charCode > 31 &&
//            (charCode < 48 || charCode > 57))
//            return false;
//    }
//    return true;
//}

function esDecimal(input, evt) {
    // Backspace = 8, Enter = 13, ‘0′ = 48, ‘9′ = 57, ‘.’ = 46, ‘-’ = 43
    var key = window.Event ? evt.which : evt.keyCode;
    var chark = String.fromCharCode(key);
    var tempValue = input.value + chark;
    if (key >= 48 && key <= 57) {
        if (filter(tempValue) === false) {
            return false;
        } else {
            return true;
        }
    } else {
        if (key == 8 || key == 13 || key == 0) {
            return true;
        } else if (key == 46) {
            if (filter(tempValue) === false) {
                return false;
            } else {
                return true;
            }
        } else {
            return false;
        }
    }
}

function _esDecimal(input, evt) {
    // Backspace = 8, Enter = 13, ‘0′ = 48, ‘9′ = 57, ‘.’ = 46, ‘-’ = 43
    var key = window.Event ? evt.which : evt.keyCode;
    var chark = String.fromCharCode(key);
    var tempValue = input.value + chark;
    if (key >= 48 && key <= 57) {
        if (filter(tempValue) === false) {
            return false;
        } else {
            return true;
        }
    } else {
        if (key == 8 || key == 13 || key == 0) {
            return true;
        } else if (key == 46) {
            if (filter(tempValue) === false) {
                return false;
            } else {
                return true;
            }
        } else {
            return false;
        }
    }
}
function filter(__val__) {
    var preg = /^([0-9]+\.?[0-9]{0,2})$/;
    if (preg.test(__val__) === true) {
        return true;
    } else {
        return false;
    }

}


$(document).ready(function () {


    toDataURL(
        pathServer,
        //'http://localhost:56196/assets/img/logo_lluvia.png',
        function (dataUrl) {
            //console.log('base64:', dataUrl)
            logoBase64 = dataUrl;
        }
    )

    $('#facturar').click(function (e) {
        $.ajax({
            url: rootUrl("/Factura/CancelarFactura"),
            data: { idVenta: 1, idFactura: 1 },
            method: 'post',
            dataType: 'json',
            async: false,
            beforeSend: function (xhr) {
                console.log("Antes")
            },
            success: function (data) {
                console.log(JSON.stringify(data));
                MuestraToast("info", JSON.stringify(data));
            },
            error: function (xhr, status) {
                console.log('hubo un problema pongase en contacto con el administrador del sistema');
                console.log(xhr);
                console.log(status);
            }
        });
    });
});

function roundToTwo(valor) {
    return +(Math.round(valor + "e+2") + "e-2");
}

function InitSelect2() {
    $('.select-multiple').select2({
        width: "100%",
        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        }
    });

    // $('#' + item).val("0").trigger('change');
}

function ConsultExcesoEfectivo() {
    $.ajax({
        url: rootUrl("/Ventas/_ExcesoEfectivo"),
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            $('#NotificacionesExcesoEfectivo').html(data);
            feather.replace();            
            //iziToast.warning({
            //    title: 'Exceso de efectivo en caja',
            //    message: '',
            //    position: 'bottomRight'
            //});

        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}

function validarEmail(valor) {
    if (/^(([^<>()[\]\.,;:\s@\"]+(\.[^<>()[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i.test(valor)) {
        return true;
    } else {
        return false;
    }
}

function validarRFC(valor) {
    if (/^(([ÑA-Z|ña-z|&amp;]{3}|[A-Z|a-z]{4})\d{2}((0[1-9]|1[012])(0[1-9]|1\d|2[0-8])|(0[13456789]|1[012])(29|30)|(0[13578]|1[02])31)(\w{2})([A|a|0-9]{1}))$|^(([ÑA-Z|ña-z|&amp;]{3}|[A-Z|a-z]{4})([02468][048]|[13579][26])0229)(\w{2})([A|a|0-9]{1})$/i.test(valor)) {
        return true;
    } else {
        return false;
    }
}

function formatoMoneda(valor) {    
    const exp = /(\d)(?=(\d{3})+(?!\d))/g;
    const rep = '$1,';
    let arr = valor.toString().split('.');
    arr[0] = arr[0].replace(exp, rep);
    return '$' + (arr[1] ? arr.join('.') : arr[0]);
}


function lluviaRedirect(vista="login",controlador="login") {
    window.location.href = rootUrl(vista+"/"+controlador);
}

function clearQueryParams() {
    var uri = window.location.toString();
    if (uri.indexOf("?") > 0) {
        var clean_uri = uri.substring(0, uri.indexOf("?"));
        window.history.replaceState({}, document.title, clean_uri);
    }
}
