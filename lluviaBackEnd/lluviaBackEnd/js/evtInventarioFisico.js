var tblInventarioFisico;
$(document).ready(function () {
    if ($("#tblInventarioFisico").length > 0)
        InitDataTableInventarioFisico();
});

//busqueda
function onBeginSubmitInventarioFisico() {
    ShowLoader("Guardando...");
}
function onSuccessResultInventarioFisico(data) {
    if (data.Estatus == 200) {
        MuestraToast('success', data.Mensaje);
        $("nombreInventarioFisico").val("");
        PintarTabla();
    } else {
        MuestraToast("error", data.Mensaje);
    }

    OcultarLoader();
}
function onFailureResultInventarioFisico() {
    OcultarLoader();
}

function PintarTabla() {
    $.ajax({
        url: rootUrl("/InventarioFisico/_ObtenerInventarioFisico"),
        data: { idUsuario: 0 },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Cargando...");
        },
        success: function (data) {
            if (tblInventarioFisico != null)
                tblInventarioFisico.destroy();
            $('#ViewInventarioFisico').html(data);
            if ($("#tblInventarioFisico").length > 0)
                InitDataTableInventarioFisico();
            OcultarLoader();
        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un error al pintar la tabla de inventario fisico, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}


function InitDataTableInventarioFisico() {
    var NombreTabla = "tblInventarioFisico";
    tblInventarioFisico = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblInventarioFisico, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Nivel de Servicio Proveedor",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    // doc.content[1].table.widths = ['10%', '25%', '15%', '15%', '20%', '15%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Nivel de Servicio Proveedor");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                },
            },
        ],

    });

    tblInventarioFisico.buttons(0, null).container().prependTo(
        tblInventarioFisico.table().container()
    );
}

function actualizarInventarioFisico(idTipoActualizacion, idInventarioFisico, ValorAnt, ValorNew) {

    if ($.trim(ValorAnt) == $.trim(ValorNew)) {
        return;
    }

    if (idTipoActualizacion == 1 && $.trim(ValorNew) == "") {
        $("#txtNombre_" + idInventarioFisico).focus();
        MuestraToast("error", "El nombre que desea actualizar no puede ir vacio");
        return;
    }



    swal({
        title: '',
        text: idTipoActualizacion == 1 ? 'Estas seguro que deseas actualizar el nombre?' : (ValorNew == true ? 'Estas seguro que deseas activar este inventario fisico' : 'Estas seguro que deseas desactivar este inventario fisico?'),
        icon: '',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {                

                var dataToPost;

                if (idTipoActualizacion == 1)
                    dataToPost = JSON.stringify({ idInventarioFisico: idInventarioFisico, Nombre: $.trim(ValorNew) });
                else
                    dataToPost = JSON.stringify({ idInventarioFisico: idInventarioFisico, Activo: ValorNew });

                $.ajax({
                    url: rootUrl("/InventarioFisico/GuardarInventarioFisico"),
                    data: dataToPost,
                    method: 'POST',
                    dataType: 'JSON',
                    contentType: "application/json; charset=utf-8",
                    async: true,
                    beforeSend: function (xhr) {
                        ShowLoader("Actualizando.");
                    },
                    success: function (data) {
                        if (data.Estatus == 200) {
                            MuestraToast('success', data.Mensaje);                           
                            PintarTabla();
                        } else {
                            MuestraToast("error", data.Mensaje);
                        }

                        OcultarLoader();


                    },
                    error: function (xhr, status) {
                        OcultarLoader();
                        console.log('Hubo un problema al actualizar el inventario fisico, contactese con el administrador del sistema');
                        console.log(xhr);
                        console.log(status);
                    }
                });
            } else {
                console.log("cancelar");
            }
        });

    console.log(idTipoActualizacion);
    console.log(ValorAnt);
    console.log(ValorNew);

}