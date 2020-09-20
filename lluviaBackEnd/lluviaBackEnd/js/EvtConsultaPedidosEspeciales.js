var tablaPedidosEspeciales;

$(document).ready(function () {

    if ($("#tblPedidosEspeciales ").length > 0) {
        InitTablePedidosEspeciales();
    }
    InitRangePicker('rangePedidosEspeciales', 'fechaIni', 'fechaFin');

    $('#rangePedidosEspeciales').val('');
    $('.select-multiple').select2({
        width: "100%",
        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        },

    });

    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarPedidosEspeciales").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarPedidosEspeciales .select-multiple").trigger("change");
    });  
});

function InitTablePedidosEspeciales() {
    var NombreTabla = "tblPedidosEspeciales";
    tablaPedidosEspeciales = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tablaPedidosEspeciales, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Pedidos Especiales",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['5%', '15%', '15%', '15%', '20%', '10%', '10%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Bitàcoras");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6,7,8]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6,7,8]
                },
            },
        ],
    });

    tablaPedidosEspeciales.buttons(0, null).container().prependTo(
        tablaPedidosEspeciales.table().container()
    );

    $('#tblPedidosEspeciales tbody').on('click', 'td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = tablaPedidosEspeciales.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        } else {
            // Open this row
            //  console.log(obtenerDetalleBitacora($(this).attr("idPedido")));
            $.ajax({
                url: rootUrl("/Bitacora/_DetalleBitacora"),
                data: { idPedidoInterno: $(this).attr("idPedido") },
                method: 'post',
                dataType: 'html',
                async: true,
                beforeSend: function (xhr) {
                    ShowLoader();
                },
                success: function (view) {
                    OcultarLoader();
                    row.child(view).show();
                    tr.addClass('shown');
                },
                error: function (xhr, status) {
                    OcultarLoader();
                }
            });

        }
    });

    }

function onBeginSubmitObtenerPedidosEspeciales() {
    ShowLoader("Buscando...");
}
function onCompleteObtenerPedidosEspeciales() {
    //OcultarLoader();
}
function onSuccessResultObtenerPedidosEspeciales(data) {
    $("#DivtblPedidosEspeciales").html(data);
    if ($("#tblPedidosEspeciales ").length > 0) {
        tablaPedidosEspeciales.destroy();
        InitTablePedidosEspeciales();
    }

    OcultarLoader();
}
function onFailureResultObtenerPedidosEspeciales() {
    OcultarLoader();
}

//function obtenerDetalleBitacora(idPedidoInterno) {
//    $.ajax({
//        url: rootUrl("/Bitacora/_DetalleBitacora"),
//        data: { idPedidoInterno: idPedidoInterno },
//        method: 'post',
//        dataType: 'html',
//        async: true,
//        beforeSend: function (xhr) {
//            ShowLoader();
//        },
//        success: function (view) {           
//            OcultarLoader();           
//            html=view;
//        },
//        error: function (xhr, status) {            
//            OcultarLoader();
//        }
//    });

//}



//function preguntaAceptarPedido(idPedidoEspecial) {

//    swal({
//        title: 'Mensaje',
//        text: '¿Esta seguro que quiere aceptar el pedido ' + idPedidoEspecial + '?',
//        icon: 'info',
//        buttons: ["No", "Sí"],
//        dangerMode: true,
//    })
//        .then((willDelete) => {
//            if (willDelete) {

//                const pedido = {};
//                pedido.idPedidoEspecial = idPedidoEspecial;

//                $.ajax({
//                    url: rootUrl("/PedidosEspeciales/AceptarPedido"),
//                    data: JSON.stringify(pedido),
//                    method: 'post',
//                    dataType: 'json',
//                    contentType: "application/json; charset=utf-8",
//                    async: true,
//                    beforeSend: function (xhr) {
//                        ShowLoader();
//                    },
//                    success: function (data) {
//                        OcultarLoader();
//                        MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
                        
                        
//                    },
//                    error: function (xhr, status) {
//                        OcultarLoader();
//                        console.log('Hubo un problema al aceptar el pedido especial, contactese con el administrador del sistema');
//                        console.log(xhr);
//                        console.log(status);
//                    }
//                });

//            } else {
//                console.log(willDelete);
//            }
//        });
//}





function FinalizarPedido(idPedidoEspecial) {

    $('#idPedidoEspecial').val(idPedidoEspecial);

    $.ajax({
        url: rootUrl("/PedidosEspeciales/_FinalizaPedido"),
        data: { idPedidoEspecial: idPedidoEspecial },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            $("#divFinalizarPedido").html('');
            $("#divFinalizarPedido").html(data);
            initInputsTablaPedidos();
            $('#modalFinalizarPedido').modal({ backdrop: 'static', keyboard: false, show: true });
        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al intentar finalizar el pedido especial, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
}


$('#btnRechazarPedido').click(function (e) {

    $('#motivoRechazo').val('');

    swal({
        title: 'Mensaje',
        text: '¿Esta seguro que quiere rechazar el todo el pedido?',
        icon: 'info',
        buttons: ["No", "Sí"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {

                $('#modalObservacionesRechazaPedido').modal({ backdrop: 'static', keyboard: false, show: true });

            } else {
                console.log("cancelar");
            }
        });
});


$('#btnAceptarRechazarPedido').click(function (e) {

    AceptarPedido(true);
    $('#modalObservacionesRechazaPedido').modal('hide');

});


$('#btnAceptarPedido').click(function (e) {

    AceptarPedido(false);

});

function AceptarPedido(esCancelacion) {

    //if (esCancelacion == false) {
    //    console.log('no es cancelacion');
    //}
    //if (esCancelacion == true) {
    //    console.log('si es cancelacion');
    //}


    var productos = [];
    //var idCliente = $('#idCliente').val();
    var idPedidoEspecial = $('#idPedidoEspecial').val();
    //console.log(idPedidoEspecial);
    //var descripcion = $('#descripcionPedidoInterno').val();
    var tblPedidos = document.getElementById('tablaPedidosEspecialesDetalle');
    var rCount = tblPedidos.rows.length;

    if (esCancelacion == false) {
        // validaciones
        // revisamos que todos los campos de cantidad aceptada esten llenos 
        if (rCount >= 2) {
            for (var i = 1; i < rCount; i++) {
                if  (
                        ((parseInt(tblPedidos.rows[i].cells[5].children[0].value)) === 0) &&
                        (((tblPedidos.rows[i].cells[6].children[0].value) == '') || ((tblPedidos.rows[i].cells[6].children[0].value) == null))
                    ) 

                {
                    MuestraToast('warning', "Debe especificar los productos que esta aceptando del Pedido Interno");
                    return;
                }
            }
        }

        // si no esta aceptando todos los atendidos debe especificar el motivo (que serian los rechazados)
        if (rCount >= 2) {
            for (var i = 1; i < rCount; i++) {
                if ((parseInt(tblPedidos.rows[i].cells[4].innerHTML)) != (parseInt(tblPedidos.rows[i].cells[5].children[0].value))) {

                    if (((tblPedidos.rows[i].cells[6].children[0].value) == '') || ((tblPedidos.rows[i].cells[6].children[0].value) == null)) {

                        MuestraToast('warning', "Debe especificar el motivo por el cual no esta aceptando todos los productos atendidos en el campo observaciones.");
                        return;

                    }

                }
            }
        }
    }

    //if (descripcion == "") {
    //    MuestraToast('warning', "Debe escribir una descripcion para el Pedido Interno");
    //    return;
    //}

    // si todo bien
    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            var observacion_ = "";
            var cantidadRechazada_ = parseInt(0);
            var cantidadAceptada_ = parseInt(0);

            if ((esCancelacion) == true) {
                observacion_ = $('#motivoRechazo').val();
                cantidadRechazada_ = parseInt(tblPedidos.rows[i].cells[4].innerHTML);
                cantidadAceptada_ = parseInt(0);
            }
            else {

                if (((tblPedidos.rows[i].cells[6].children[0].value) == "") || ((tblPedidos.rows[i].cells[6].children[0].value) == null)) {
                    observacion_ = "0";
                }
                else {
                    observacion_ = tblPedidos.rows[i].cells[6].children[0].value;
                }

                console.log(observacion_);
                cantidadAceptada_ = parseInt(tblPedidos.rows[i].cells[5].children[0].value);
                cantidadRechazada_ = (parseInt(tblPedidos.rows[i].cells[4].innerHTML)) - (parseInt(tblPedidos.rows[i].cells[5].children[0].value)) ;
            }

            var row_ = {
                idProducto: parseInt(tblPedidos.rows[i].cells[1].innerHTML),
                cantidadAtendida: parseInt(tblPedidos.rows[i].cells[4].innerHTML), // solicitada
                //cantidadAtendida: parseInt(tblPedidos.rows[i].cells[4].children[0].value),
                cantidadAceptada: cantidadAceptada_, //parseInt(tblPedidos.rows[i].cells[5].children[0].value),
                observacion: observacion_,
                cantidadRechazada: cantidadRechazada_,
                idPedidoInternoDetalle: parseInt(tblPedidos.rows[i].cells[8].innerHTML), // idPedidoInternoDetalle
            };
            productos.push(row_);
        }
    }

    const pedido = {};
    //pedido.idCliente = idCliente;
    //pedido.descripcion = descripcion;
    pedido.idPedidoEspecial = idPedidoEspecial;
    pedido.lstPedidosInternosDetalle = productos;

    //console.log(pedido);
    //return;

    $.ajax({
        url: rootUrl("/PedidosEspeciales/AceptarRechazarPedidoEspecial"),
        data: JSON.stringify(pedido),
        method: 'post',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            $('#modalFinalizarPedido').modal('hide');

            if (data.Estatus == 200) {

                //if ((esPedidoNormal == "true") || (esPedidoNormal == "True")) {
                //    ImprimeTicket(data.Modelo.idPedidoEspecial);
                //}

                //InitSelect2Productos();
                //limpiarTicket();
            }

        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al actualizar el pedido especial, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

}


function initInputsTablaPedidos() {
    
    $('#tablaPedidosEspecialesDetalle input').on('change', function () {

        var thisInput = $(this);
        var mensaje = "Debe escribir la cantidad de productos.";
        var cell = $(this).closest('td');
        var columna = $(this).closest("td").index();   
        var row = cell.closest('tr');
        var rowIndex = row[0].rowIndex;
        var tblPedido = document.getElementById('tablaPedidosEspecialesDetalle');


        // solo si es la columna de cantidad aceptada
        if (parseInt(columna) === 5) {

            if ((thisInput.val() == "") || (thisInput.val() == "0")) {
                MuestraToast('warning', mensaje);
                document.execCommand('undo');
            }


            if ((parseInt(thisInput.val())) > (parseInt(tblPedido.rows[rowIndex].cells[4].innerHTML))) {
                MuestraToast('warning', "No puede aceptar mas de lo que atendio.");
                document.execCommand('undo');
                return;
            }

        }

    });
}

