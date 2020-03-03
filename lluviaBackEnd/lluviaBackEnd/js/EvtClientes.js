var tablaClientes;

function onBeginSubmitGuardarCliente() {
}

function onCompleteSubmitGuardarCliente() {
}

function onFailureResultGuardarCliente() {
}

function onSuccessResultGuardarCliente(data) {
    console.log(data);
    if (data.Estatus === 200) {
        MuestraToast("success", data.Mensaje)

    } else {
        MuestraToast("error", data.Mensaje)
    }

}
function ObtenerClientes() {
    $.ajax({
        url: rootUrl("/Clientes/_ObtenerClientes"),
        data: { idCliente: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaClientes.destroy();
            $('#rowTblClientes').html(data);
            InitDataTable();
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
        }
    });
}

function EliminarCliente(idCliente) {

    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas eliminar este cliente?',
        icon: 'warning',
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: rootUrl("/Clientes/EliminarCliente"),
                    data: { idCliente: idCliente },
                    method: 'post',
                    dataType: 'json',
                    async: true,
                    beforeSend: function (xhr) {
                    },
                    success: function (data) {
                        console.log(data);
                        MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
                        ObtenerClientes();
                    },
                    error: function (xhr, status) {
                        console.log('Disculpe, existió un problema');
                        console.log(xhr);
                        console.log(status);
                    }
                });

            } else {
                console.log("cancelar");
            }
        });



}

function InitTableClientes() {
    var NombreTabla = "tblClientes";
    tablaClientes = initDataTable(NombreTabla)

    new $.fn.dataTable.Buttons(tablaClientes, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Proveedores",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8; //2, 3, 4,etc
                    doc.styles.tableHeader.fontSize = 10; //2, 3, 4, etc
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '20%', '20%', '20%', '20%', '10%'];
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                },
            },
            {

                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                },

            }
        ]
    });

    tablaClientes.buttons(0, null).container().prependTo(
        tablaClientes.table().container()
    );

    $('#' + NombreTabla + '_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnAgregarCliente" data-toggle="tooltip" title="Agregar Cliente"><i class="fas fa-user-plus"></i></a>');
    InitBtnAgregar();
}

function InitBtnAgregar() {
    $('#btnAgregarCliente').click(function (e) {

        $('#btnGuardarProveedor').prop('disabled', false);
        //para abrir el modal
        $('#mdlAgregarCliente').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalCliente').html("Agregar Cliente");

    });
}

$(document).ready(function () {
    InitTableClientes();

});