var tblRetiros;
$(document).ready(function () {
    var idAlmacenUsuario = $('#idAlmacenUsuario').val();
    
    if (idAlmacenUsuario > 0) {
        $('#idEstacion').val(idAlmacenUsuario).trigger('change');
    }
    else {
        InitSelect2Usuarios(0);
    }

    $("#frmRetiros").submit();
    //InitDataTableRetiros();
});


function InitDataTableRetiros() {
    var NombreTabla = "tblRetiros";
    tblRetiros = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblRetiros, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Retiros",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['20%', '10%', '20%', '20%', '10%', '10%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Retiros de Efectivo");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5,6]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5,6]
                },
            },
        ],

    });

    tblRetiros.buttons(0, null).container().prependTo(
        tblRetiros.table().container()
    );
}

//busqueda
function onBeginSubmitRetirosAutorizacion() {
    ShowLoader("Buscando retiros...");
}

function onSuccessResultRetirosAutorizacion(data) {
    OcultarLoader();
    if (tblRetiros!=null)
        tblRetiros.destroy();
    $('#DivRetiros').html(data);
    InitDataTableRetiros();
}
function onFailureResultRetirosAutorizacion() {
    OcultarLoader();
    MuestraToast("error","Ocurrio un error al buscar los retiros")
}

function ActualizarEstatusRetiro(idRetiro, tipoRetiro,idStatus) {

    if (idStatus == 2) {     
 
        if ($("#Monto_" + idRetiro).val() == "" || parseFloat($("#Monto_" + idRetiro).val()) <= 0) {
            MuestraToast("error", "Debe de capturar el monto por el cual va a autorizar el retiro");
            return;
        }  
        

    }

    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas ' + (idStatus == 2 ? 'autorizar' : 'cancelar') + ' el retiro ' + (tipoRetiro == 1 ? 'por exceso de efectivo' : 'al cierre del dìa') + '?',
        icon: '',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {

                var Status = {
                    idStatus: idStatus,
                    descripcion: "",
                  
                };

                var Retiro = {
                    idRetiro: idRetiro,
                    tipoRetiro: tipoRetiro,
                    estatusRetiro: Status,
                    montoAutorizado: parseFloat($("#Monto_" + idRetiro).val())
                };

                dataToPost = JSON.stringify({ retiros: Retiro });
                console.log(dataToPost);

                $.ajax({
                    url: rootUrl("/Ventas/ActualizaEstatusRetiro"),
                    data: { idRetiro: idRetiro, tipoRetiro: tipoRetiro, estatusRetiro: Status, montoAutorizado: (idStatus == 2 ? parseFloat($("#Monto_" + idRetiro).val())  : 0)},
                    method: 'post',
                    dataType: 'json',
                    async: true,
                    beforeSend: function (xhr) {
                        ShowLoader((idStatus == 2 ? 'Autorizando...' : 'Cancelando...') );
                    },
                    success: function (data) {
                        OcultarLoader();
                        if (data.Estatus == 200) {
                            MuestraToast('success', data.Mensaje);
                            $("#frmRetiros").submit();
                        }
                        else {
                            MuestraToast('error', data.Mensaje);
                        }
                        
                    },
                    error: function (xhr, status) {
                        OcultarLoader();
                        MuestraToast('error', "Ocurrio un error al cambiar de estatus el retiro ");
                    }
                });

            } 
        });

}



$("#idEstacion").on("change", function () {

    var idAlmacen = parseInt($('#idEstacion').val());
    //alert(idAlmacen);

    if (idAlmacen > 0) {
        InitSelect2Usuarios(idAlmacen);
    }
    else {
        InitSelect2Usuarios(idAlmacen);
    }


    //$('#efectivo').val('');
    //document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    //document.getElementById("chkFacturar").checked = false;
    //document.getElementById("divUsoCFDI").style.display = 'none';
    //$('#usoCFDI').val("3").trigger('change');
    //$('#formaPago').val("1").trigger('change');

    //var idCliente = parseFloat($('#idCliente').val());
    //var data = ObtenerCliente(idCliente);
    //var nombre = data.Modelo.nombres + "  " + data.Modelo.apellidoPaterno + "  " + data.Modelo.apellidoMaterno;
    //var descuento = parseFloat(0.0);

    //if (idCliente != 1) {
    //    descuento = parseFloat(data.Modelo.tipoCliente.descuento).toFixed(2);;
    //}

    //var total = parseFloat(document.getElementById("previoTotal").innerHTML.replace("<h4>$", "").replace("</h4>", "")).toFixed(2);
    //var descuentoMenudeo = parseFloat(document.getElementById("previoDescuentoMenudeo").innerHTML.replace("<h4>$", "").replace("</h4>", "")).toFixed(2);
    //var cantidadDescontada = parseFloat(0).toFixed(2);

    //if (descuento > 0.0) {
    //    cantidadDescontada = parseFloat((total - descuentoMenudeo) * (descuento / 100)).toFixed(2);
    //}

    //var subTotal = parseFloat(total - descuentoMenudeo - cantidadDescontada).toFixed(2);
    //var iva = parseFloat(0).toFixed(2);

    //// si lleva iva
    //if ($("#chkFacturar").is(":checked")) {
    //    iva = parseFloat(subTotal * 0.16).toFixed(2);
    //}

    //var final = (parseFloat(subTotal) + parseFloat(iva)).toFixed(2);

    //document.getElementById("previoDescuentoCliente").innerHTML = "<h4>$" + cantidadDescontada + "</h4>";
    //document.getElementById("previoSubTotal").innerHTML = "<h4>$" + subTotal + "</h4>";
    //document.getElementById("previoIVA").innerHTML = "<h4>$" + iva + "</h4>";
    //document.getElementById("previoFinal").innerHTML = "<h4>$" + final + "</h4>";

    //// para los datos del cliente
    //var row_ = "<address>" +
    //    "    <strong></strong><br>" +
    //    "    <br>" +
    //    "    <br>" +
    //    "    <br>" +
    //    "    <br>" +
    //    "    <br>" +
    //    "</address>";

    //if ((data.idCliente != 1) && (idCliente != 1)) {
    //    row_ = "<address>" +
    //        "    <strong>Datos del Cliente:</strong><br>" +
    //        "    Nombre: " + nombre.toUpperCase() + "<br>" +
    //        "    Telefono: " + data.Modelo.telefono + "<br>" +
    //        "    E-mail: " + data.Modelo.correo + "<br>" +
    //        "    RFC: " + data.Modelo.rfc + "<br>" +
    //        "    Tipo de Cliente: " + data.Modelo.tipoCliente.descripcion + "<br>" +
    //        "</address>";
    //}

    //// para los tipo de clietne ruta
    //if (data.Modelo.nombres.includes('RUTA')) {
    //    row_ = "<div id =\"divClientesAtendidos\">" +
    //        "     <div class=\"section-title\"><strong>  </strong></div>" +
    //        "     <div class=\"input-group mb-3\">" +
    //        "         <div class=\"input-group-prepend\">" +
    //        "             <span class=\"input-group-text\">Número de Clientes Atendidos por Ruta:</span>" +
    //        "         </div>" +
    //        "         <input id=\"numClientesAtendidos\" type=\"text\" class=\"form-control\" onkeypress=\"return esNumero(event)\">" +
    //        "     </div>" +
    //        "</div><br><br><br><br>";
    //}

    //document.getElementById("nombreCliente").innerHTML = row_;
}); 

function InitSelect2Usuarios(idAlmacen) {

    var result = '';
    $('#idUsuario').prop('disabled', false);

    $.ajax({
        url: rootUrl("/Usuarios/ObtenerUsuariosPorAlmacenyRol"),
        data: { idUsuario: 0, idRol: 3, idAlmacen : idAlmacen },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            result = data;
        },
        error: function (xhr, status) {
            console.log('hubo un problema pongase en contacto con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

    var i;
    for (i = 0; i < result.length; i++) {
        result[i].id = result[i]['idUsuario'];
        result[i].text = result[i]['nombre'] + " " + result[i]['apellidoPaterno'] + " " + result[i]['apellidoMaterno'];
    }
    //console.log(result);

    $("#idUsuario").html('').select2();
    $('#idUsuario').select2({
        width: "100%",
        placeholder: "-- TODOS --",
        data: result,

        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        }
    });

    $('#idUsuario').val("0").trigger('change');

    if (result.length <= 0) {
        $('#idUsuario').prop('disabled', true);
    }

}