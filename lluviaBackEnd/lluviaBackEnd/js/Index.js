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
    $('#textLoader').html((mensaje==='undefined' || mensaje ==='' ? "Cargadno" : mensaje));
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

    return $('#' + nombreTabla).DataTable({
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
        "dom": 'frtip',


        "bDestroy": true, // es necesario para poder ejecutar la funcion LimpiaTabla()
        "ordering": false,
    });

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
        $('#' + fechaIni).val(picker.startDate.format('YYYY-MM-DD') );
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

function esPrecio(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    //console.log(charCode);
    if (charCode > 31 && (charCode < 48 || charCode > 57) && (charCode != 46) ) {
            return false;
    }
    return true;
}


$(document).ready(function () {
    console.log("index ready");

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