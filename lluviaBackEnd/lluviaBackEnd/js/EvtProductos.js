var table;
var iframe;
var tablaProductos;  

//busqueda
function onBeginSubmitProductos() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitProductos() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultProductos(data) {
    console.log("onSuccessResult", JSON.stringify(data) );
    
    //if (notificacion.Estatus == 200) {
    //    MuestraToast('success', notificacion.Mensaje);

        tablaProductos.destroy();
        $('#rowProductos').html(data);
        InitTableProductos();

    //} else {
    //    MuestraToast('error', "error");
    //}
}
function onFailureResultProductos() {
    console.log("onFailureResult___");
}



// guardar-modificar
function onBeginSubmitGuardarProducto() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitGuardarProducto() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultGuardarProducto(data) {
    console.log("onSuccessResult", JSON.stringify(data));   
    if (data.Estatus == 200) {
        MuestraToast('success', data.Mensaje);
        PintarTabla();
    } else {
        //MuestraToast("error", data.Mensaje);
    }
    $('#EditarProductoModal').modal('hide');
}
function onFailureResultGuardarProducto() {
    console.log("onFailureResult___");
}


function PintarTabla() {
    $.ajax({
        url: "/Productos/_ObtenerProductos",
        data: { idUsuario: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaProductos.destroy();
            $('#rowProductos').html(data);
            InitTableProductos();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}

function BuscarProductos(data) {
    $.ajax({
        url: "/Productos/BuscarProductos",
        data: { idUsuario: data.idProducto },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            table.destroy();
            $('#rowProductos').html(data);
            InitDataTable();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}

function InitTableProductos() {
    var NombreTabla = "tablaProductos";
    tablaProductos = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tablaProductos, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Productos",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['25%', '25%',  '25%', '25%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Productos");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3]
                },
            },
        ],
    });

    tablaProductos.buttons(0, null).container().prependTo(
        tablaProductos.table().container()
    );

  

    $('#' + NombreTabla+'_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnAgregarProducto" data-toggle="tooltip" title="Agregar Producto"><i class="fas fa-user-plus"></i></a>');
    InitBtnAgregar();
}


function ObtenerProducto(idProducto) {

    var result = '';
    $.ajax({
        url: "/Productos/ObtenerProductos",
        data: { idProducto: idProducto },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            console.log("Antes")
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

    return result;
}

function VerProducto(idProducto) {

    $('#btnGuardarProducto').prop('disabled', true);

    var data = ObtenerProducto(idProducto);

    $('#idProducto').val(idProducto);
    $('#activo').val(data.activo);
    $('#descripcion').val(data.descripcion).prop('disabled', true);
    $('#idUnidadMedida').val(data.idUnidadMedida).prop('disabled', true);
    $('#idLineaProducto').val(data.idLineaProducto).prop('disabled', true);
    $('#cantidadUnidadMedida').val(data.cantidadUnidadMedida).prop('disabled', true);
    $('#articulo').val(data.articulo).prop('disabled', true);
    obtenerCodigos();
    //para abrir el modal
    $('#EditarProductoModal').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#TituloModalProducto').html("Información del Producto");
}

function EditarProducto(idProducto) {

    $('#btnGuardarProducto').prop('disabled', false);

    var data = ObtenerProducto(idProducto);

    $('#idProducto').val(idProducto);
    $('#activo').val(data.activo);
    $('#descripcion').val(data.descripcion).prop('disabled', false);
    $('#idUnidadMedida').val(data.idUnidadMedida).prop('disabled', false);
    $('#idLineaProducto').val(data.idLineaProducto).prop('disabled', false);
    $('#cantidadUnidadMedida').val(data.cantidadUnidadMedida).prop('disabled', false);
    $('#articulo').val(data.articulo).prop('disabled', false);
    obtenerCodigos();
    //para abrir el modal
    $('#EditarProductoModal').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#TituloModalUsuario').html("Editar Producto");
}


function InitBtnAgregar() {
    $('#btnAgregarProducto').click(function (e) {

        $('#btnGuardarProducto').prop('disabled', false);

        $('#idProducto').val(0);
        $('#descripcion').val('').prop('disabled', false);
        $('#idUnidadMedida').val('').prop('disabled', false);
        $('#idLineaProducto').val('').prop('disabled', false);
        $('#cantidadUnidadMedida').val('').prop('disabled', false);
        $('#articulo').val('').prop('disabled', false);
        //para abrir el modal
        $('#EditarProductoModal').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalProducto').html("Agregar Producto");
    });
}

function EliminarProducto(idProducto) {

    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas eliminar a este Producto?',
        icon: 'warning',
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: "/Productos/ActualizarEstatusProducto",
                    data: { idProducto: idProducto, activo: false },
                    method: 'post',
                    dataType: 'json',
                    async: false,
                    beforeSend: function (xhr) {
                        console.log("Antes ")
                    },
                    success: function (data) {
                        MuestraToast('success', data.Mensaje);
                        PintarTabla();
                    },
                    error: function (xhr, status) {
                        console.log('Hubo un problema al intentar eliminar al usuario, contactese con el administrador del sistema');
                        console.log(xhr);
                        console.log(status);
                    }
                });

            } else {
                console.log("cancelar");
            }
        });
}

function InitRangePicker() {

    $('.daterange-cus').daterangepicker({
        locale: { format: 'YYYY-MM-DD' },
        drops: 'down',
        opens: 'right'
    });

}

function obtenerCodigos() {
    console.log($('#articulo').val());
    if ($('#articulo').val() !== '') {
        $.ajax({
            url: "/Productos/ObtenerCodigos",
            data: { cadena: $('#articulo').val() },
            method: 'post',
            dataType: 'json',
            async: true,
            beforeSend: function (xhr) {
                console.log("Antes ")
            },
            success: function (data) {
                console.log(data);
                $("#barra").attr('src', 'data:image/png;base64,' + data.barra);
                $("#qr").attr('src', 'data:image/png;base64,' + data.qr);
            },
            error: function (xhr, status) {
                console.log('Hubo un problema al intentar eliminar al usuario, contactese con el administrador del sistema');
                console.log(xhr);
                console.log(status);
            }
        });
    }
}

$(document).ready(function () {

    InitTableProductos();
    InitRangePicker();
    $('#idLineaProductoBusqueda').val('');

    document.getElementById('articulo').onchange = function () {
        obtenerCodigos();
    };

    $('#articulo').keyup(function () {
        obtenerCodigos();
    });

    $('#descripcion').keyup(function () {
        if ( ($('#idLineaProducto').val()) !== null )
        {
            $('#articulo').val($("#idLineaProducto option:selected").text().replace('Linea ', '').substring(0, 2).concat('-').concat($("#descripcion").val().substring(0, 3)).toUpperCase());
            obtenerCodigos();
        }
    });

    $('#idLineaProducto').change(function () {
        if (($('#descripcion').val()) !== '') {
            $('#articulo').val($("#idLineaProducto option:selected").text().replace('Linea ', '').substring(0, 2).concat('-').concat($("#descripcion").val().substring(0, 3)).toUpperCase());
            obtenerCodigos();
        }
    });


});