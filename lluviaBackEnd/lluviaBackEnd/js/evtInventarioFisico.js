﻿var tblInventarioFisico;
var tblAjusteInventarioFisico;
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
                title: "Inventario Fisico",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '20%', '15%', '15%', '15%', '15%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Inventario Fisico");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6,7]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6,7]
                },
            },
        ],

    });

    tblInventarioFisico.buttons(0, null).container().prependTo(
        tblInventarioFisico.table().container()
    );
}

function InitDataTableAjusteInventarioFisico(NameFile) {
    var NombreTabla = "tblAjusteInventarioFisico";
    tblAjusteInventarioFisico = initDataTable(NombreTabla);
    
    new $.fn.dataTable.Buttons(tblAjusteInventarioFisico, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: NameFile,
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    //doc.content[1].table.widths = ['10%', '20%', '15%', '15%', '15%', '15%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF(NameFile);
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6,7]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6,7]
                },
            },
        ],

    });

    tblAjusteInventarioFisico.buttons(0, null).container().prependTo(
        tblAjusteInventarioFisico.table().container()
    );
}


function actualizarInventarioFisico(idInventarioFisico, ValorAnt, ValorNew) {

    if ($.trim(ValorAnt) == $.trim(ValorNew)) {
        return;
    }

    if ($.trim(ValorNew) == "") {
        $("#txtNombre_" + idInventarioFisico).focus();
        MuestraToast("error", "El nombre que desea actualizar no puede ir vacio");
        return;
    }



    swal({
        title: '',
        text: 'Estas seguro que deseas actualizar el nombre?',
        icon: '',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {              

               
                $.ajax({
                    url: rootUrl("/InventarioFisico/GuardarInventarioFisico"),
                    data: { idInventarioFisico: idInventarioFisico, Nombre: $.trim(ValorNew) },
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
                           
                        } else {
                            MuestraToast("error", data.Mensaje);
                        }

                        PintarTabla();

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

function VerDetalleInventarioFisico(idInventarioFisico,nameInventarioFisico) {

    $.ajax({
        url: rootUrl("/InventarioFisico/_InventarioFisico"),
        data: { idInventarioFisico: idInventarioFisico},
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            if (tblAjusteInventarioFisico != null)
                tblAjusteInventarioFisico.destroy(); 
            $("#_ViewInventarioFisico").html(data);
            if ($("#tblAjusteInventarioFisico").length > 0)
                InitDataTableAjusteInventarioFisico('Ajuste de Inventario Fisico "' + nameInventarioFisico + '"');
               
            $('#modalAjusteInventarioFisico').modal({ backdrop: 'static', keyboard: false, show: true });

        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al intentar mostrar el detalle del inventario fisico, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
}

function ActualizarEstatusInventarioFisico(idInventarioFisico,idStatusInventarioFisico,Observaciones,descEstatus) {
    
    swal({
        title: '',
        text: 'Estas seguro que deseas ' + descEstatus+' el ajuste de inventario fisico?',
        icon: '',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {

                var EstatusInventario = new Object();
                EstatusInventario.idStatus = idStatusInventarioFisico;

                var inventarioFisico = new Object();
                inventarioFisico.idInventarioFisico = idInventarioFisico
                inventarioFisico.EstatusInventarioFisico = EstatusInventario;
                inventarioFisico.Observaciones = Observaciones;

                var dataToPost = JSON.stringify({ inventarioFisico: inventarioFisico });

                $.ajax({
                    url: rootUrl("/InventarioFisico/ActualizaEstatusInventarioFisico"),
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
                            $('#modalAjusteInventarioFisico').modal('hide');

                        } else {
                            MuestraToast("error", data.Mensaje);
                        }

                        PintarTabla();

                        OcultarLoader();


                    },
                    error: function (xhr, status) {
                        OcultarLoader();
                        console.log('Hubo un problema al actualizar el estatus del inventario fisico, contactese con el administrador del sistema');
                        console.log(xhr);
                        console.log(status);
                    }
                });
            } else {
                console.log("cancelar");
            }
        });
}