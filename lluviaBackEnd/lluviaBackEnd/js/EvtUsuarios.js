var table;
var iframe;


function onBeginSubmitGuardarUsuario() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitGuardarUsuario() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultGuardarUsuario(data) {
    console.log("onSuccessResult___");

    if (data.status == 200) {

        mensajeOK(data.mensaje);
        PintarTabla();

    } else {
        mensajeERROR(data.mensaje);
    }

    $('#EditarUsuarioModal').modal('hide');

}
function onFailureResultGuardarUsuario() {
    console.log("onFailureResult___");
}

function PintarTabla() {
    $.ajax({
        url: "/Usuarios/_ObtenerUsuarios",
        data: { idUsuario: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            table.destroy();
            $('#rowTblUsuario').html(data);
            InitDataTable();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}

//function initForm() {
//    $("#frmSocio").validate({
//        rules: {
//            nombre: "required",
//            apellidos: "required",
//            telefono: {
//                required: true,
//                minlength: 10,
//                maxlength: 10,
//                digits: true
//            },
//            mail: {
//                required: true,
//                email: true
//            },

//        },
//        messages: {
//            nombre: "Este campo no puede estar vacio",
//            apellidos: "Este campo no puede estar vacio",
//            telefono: {
//                required: "Este campo no puede estar vacio",
//                minlength: "El minimo para el telefono son 10 digitos",
//                maxlength: "El maximo para el telefono son 10 digitos"
//            },
//            mail: {
//                required: "Este campo no puede estar vacio",
//                email: "Correo invalido ejem :var901106@gmail.com"
//            }
//        },
//        errorElement: "em",
//        errorPlacement: function (error, element) {
//            // Add the `help-block` class to the error element
//            error.addClass("help-block");
//            error.css("color", "#e3324c");

//            if (element.prop("type") === "checkbox") {
//                error.insertAfter(element.parent("label"));
//            } else {
//                error.insertAfter(element);
//            }
//        },
//        highlight: function (element, errorClass, validClass) {
//            $(element).parents(".col-sm-5").addClass("has-error").removeClass("has-success");
//        },
//        unhighlight: function (element, errorClass, validClass) {
//            $(element).parents(".col-sm-5").addClass("has-success").removeClass("has-error");
//        }
//    });
//}

function InitDataTable() {

    table = $('#tablaUsuarios').DataTable({
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
        "dom": 'Bfrtip',
        buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Usuarios",
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
            },


        ],

        "bDestroy": true, // es necesario para poder ejecutar la funcion LimpiaTabla()
    });

    $('#tablaUsuarios_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnAgregarUsuario" data-toggle="tooltip" title="Agregar Usuario"><i class="fas fa-user-plus"></i></a>');
    InitBtnAgregar();
}


//function PintaIconoPreview(file) {
//    var img = file.previewElement.querySelector("img");
//    var ext = (file.name).split('.')[1]
//    if (ext === 'pdf') {
//        $(img).attr("src", rootUrl("/assets/img/file-icon/pdf.png"));
//    }
//    $(img).css('width', '120');
//}

//function InitDrop() {

//    $("#dropZoneExpediente").append("<form id='FrmdropZoneExpediente' class='dropzone borde-dropzone' style='cursor: pointer;'></form>");
//    DropzoneOptions = {
//        url: rootUrl("/Socio/AgregarSocio"),
//        addRemoveLinks: true,
//        paramName: "archivo",
//        maxFilesize: 4, // MB
//        dictRemoveFile: "Eliminar",
//        acceptedFiles: ".pdf,.jpg,.png",
//        //maxFiles: 50,
//        parallelUploads: 20,
//        uploadMultiple: true,
//        autoProcessQueue: false, // true para envíar en automatico
//        init: function () {
//            this.on("maxfilesexceeded", function (file) {
//                this.removeFile(file);
//                swal("Error", "No se puede subir mas de un archivo", "error");
//            });
//            this.on("addedfile", function (file) {
//                PintaIconoPreview(file);
//            });
//            this.on("removedfile", function (file) {
//                if ($('#idSocio').val() !== '0') {
//                    EliminarExpediente($('#idSocio').val(), file);
//                }
//            });
//            this.on("complete", function (file) {
//                PintaIconoPreview(file);
//            });

//        },
//        /*
//         * ESTA FUNCION SE UTILIZA PARA CUANDO SOLO SE ENVIA UN SOLO ARCHIVO
//         * sending: function (file, xhr, formData) {
//            console.log('sendingmultiple');
//        },
//        success: function (file, data) {
//            file.previewElement.classList.add("dz-success");
//            if (data.Estatus == 200)
//                swal("Notificación", data.Mensaje, data.TipoAlerta);
//            $('#verticalCenter').modal('hide');
//            ObtenerSocio(0);
//        },
//        */
//        sendingmultiple: function (file, xhr, formData) {
//            var formSocio = $("#frmSocio").serializeArray();
//            var socio = castFormToJson(formSocio);
//            for (var key in socio) {
//                formData.append(key, socio[key]);
//            }
//            console.log('sendingmultiple');
//        },
//        successmultiple: function (file, data) {
//            console.log(data);
//            if (data.Estatus == 200) {
//                swal("Notificación", data.Mensaje, data.TipoAlerta);
//                PintarTabla();
//            }
//            $('#verticalCenter').modal('hide');
//        },
//        processingmultiple: function (file, data) {
//            console.log(file);
//        },
//        error: function (file, response) {
//            file.previewElement.classList.add("dz-error");
//            $('#verticalCenter').modal('hide');
//            console.log(response);

//        }
//    } // FIN myAwesomeDropzone
//    satDropzone = new Dropzone("#FrmdropZoneExpediente", DropzoneOptions);


//}

//function initFrame(url) {

//    iframe = $('<embed src="" style="width: 90%;margin-left:5%" height="600" alt="pdf" pluginspage="http://www.adobe.com/products/acrobat/readstep2.html">');
//    iframe.innerHTML = "";
//    iframe.attr('src', rootUrl(url));
//    $('#rowPDFView').html('');
//    $('#rowPDFView').append(iframe);
//    var content = iframe.innerHTML;
//    iframe.innerHTML = content;
//}

//function castFormToJson(formArray) {
//    var obj = {};
//    $.each(formArray, function (i, pair) {
//        var cObj = obj, pObj = {}, cpName;
//        $.each(pair.name.split("."), function (i, pName) {
//            pObj = cObj;
//            cpName = pName;
//            cObj = cObj[pName] ? cObj[pName] : (cObj[pName] = {});
//        });
//        pObj[cpName] = pair.value;
//    });
//    //obj["idRow"] = 0;
//    return obj;
//}

//function EliminarExpediente(idSocio, file) {
//    if (typeof file.id !== 'undefined') {
//        var mockFile = { nombreDoc: file.name, id: file.id, pathExpediente: file.pathExpediente };
//        $.ajax({
//            url: rootUrl("/Socio/EliminarExpediente"),
//            data: { idSocio: idSocio, exp: mockFile },
//            method: 'post',
//            dataType: 'json',
//            async: false,
//            beforeSend: function (xhr) {
//            },
//            success: function (data) {
//                swal("Notificación", data.Mensaje, data.TipoAlerta);
//            },
//            error: function (xhr, status) {
//                console.log('Disculpe, existió un problema');
//                console.log(xhr);
//                console.log(status);
//            }
//        });
//    }

//}


function ObtenerUsuario(idUsuario) {

    var result = '';
    $.ajax({
        url: "/Usuarios/ObtenerUsuario",
        data: { idUsuario: idUsuario },
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

function mensajeOK(mensaje) {
    swal(mensaje, 'Presione OK para continuar.', 'success');
}

function mensajeERROR(mensaje) {
    swal(mensaje, 'Presione OK para continuar.', 'error');
}

function VerUsuario(idUsuario) {

    $('#btnGuardarUsuario').prop('disabled', true);

    var data = ObtenerUsuario(idUsuario);

    $('#idUsuario').val(idUsuario);
    $('#idRol').val(data.idRol);
    $('#idAlmacen').val(data.idAlmacen);
    $('#idSucursal').val(data.idSucursal);
    $('#activo').val(data.activo);

    $('#usuario').val(data.usuario).prop('disabled', true);
    $('#contrasena').val(data.contrasena).prop('disabled', true);
    $('#nombre').val(data.nombre).prop('disabled', true);
    $('#apellidoPaterno').val(data.apellidoPaterno).prop('disabled', true);
    $('#apellidoMaterno').val(data.apellidoMaterno).prop('disabled', true);
    $('#telefono').val(data.telefono).prop('disabled', true);
    $('#idRolGuardar').val(data.idRol).change().prop('disabled', true);
    $('#idAlmacenGuardar').val(data.idAlmacen).change().prop('disabled', true);
    $('#idSucursalGuardar').val(data.idSucursal).change().prop('disabled', true);

    //$('#activo').trigger('click');
    //para abrir el modal
    $('#EditarUsuarioModal').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#TituloModalUsuario').html("Información del Usuario");

}

function EditarUsuario(idUsuario) {

    $('#btnGuardarUsuario').prop('disabled', false);

    var data = ObtenerUsuario(idUsuario);
    
    $('#idUsuario').val(idUsuario);
    $('#idRol').val(data.idRol);
    $('#idAlmacen').val(data.idAlmacen);
    $('#idSucursal').val(data.idSucursal);
    $('#activo').val(data.activo);

    $('#usuario').val(data.usuario).prop('disabled', false);
    $('#contrasena').val(data.contrasena).prop('disabled', false);
    $('#nombre').val(data.nombre).prop('disabled', false);
    $('#apellidoPaterno').val(data.apellidoPaterno).prop('disabled', false);
    $('#apellidoMaterno').val(data.apellidoMaterno).prop('disabled', false);
    $('#telefono').val(data.telefono).prop('disabled', false);
    $('#idRolGuardar').val(data.idRol).change().prop('disabled', false);
    $('#idAlmacenGuardar').val(data.idAlmacen).change().prop('disabled', false);
    $('#idSucursalGuardar').val(data.idSucursal).change().prop('disabled', false);

    //$('#activo').trigger('click');
    //para abrir el modal
    $('#EditarUsuarioModal').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#TituloModalUsuario').html("Editar Usuario");

}


function InitBtnAgregar() {
    $('#btnAgregarUsuario').click(function (e) {

        $('#btnGuardarUsuario').prop('disabled', false);

        $('#idUsuario').val(0);
        $('#idRol').val(0);
        $('#idAlmacen').val(0);
        $('#idSucursal').val(0);
        $('#activo').val(0);

        $('#usuario').val('').prop('disabled', false);
        $('#contrasena').val('').prop('disabled', false);
        $('#nombre').val('').prop('disabled', false);
        $('#apellidoPaterno').val('').prop('disabled', false);
        $('#apellidoMaterno').val('').prop('disabled', false);
        $('#telefono').val('').prop('disabled', false);
        $('#idRolGuardar').val('').change().prop('disabled', false);
        $('#idAlmacenGuardar').val('').change().prop('disabled', false);
        $('#idSucursalGuardar').val('').change().prop('disabled', false);

        //$('#activo').trigger('click');
        //para abrir el modal
        $('#EditarUsuarioModal').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalUsuario').html("Agregar Usuario");

    });
}

function EliminarUsuario(idUsuario) {

    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas eliminar a este usuario?',
        icon: 'warning',
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: "/Usuarios/ActualizarEstatusUsuario",
                    data: { idUsuario: idUsuario, activo: false },
                    method: 'post',
                    dataType: 'json',
                    async: false,
                    beforeSend: function (xhr) {
                        console.log("Antes ")
                    },
                    success: function (data) {
                        mensajeOK(data.mensaje);
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



//function PDFView(url, indice) {

//    $('#idImagen').val(indice);
//    initFrame(url);
//    $('#modalPDFView').modal({ backdrop: 'static', keyboard: false, show: true });

//}

$(document).ready(function () {
    //InitDrop();
    //initForm();
    InitDataTable();


    //$('#swallll').click(function (e) {

    //    mensajeOK("correcto");


    //});

    
    //$('#btnGuardarUsuario').click(function (e) {
    //    //if ($("#frmUsuario").valid()) {
    //        console.log('btnGuardarUsuario');
    //        GuardarUsuario(dataUsr);

    //    //} else
    //    //    console.log("ocurrio un error")
    //});

    //$('#btnPdfViewSieguinte').click(function (e) {

    //    var indice = Number($('#idImagen').val());
    //    if (indice === Number(satDropzone.files.length - 1))
    //        indice = 0;
    //    else
    //        indice = indice + 1;

    //    $('#idImagen').val(indice);
    //    initFrame(satDropzone.files[indice].pathExpediente);
    //});

    //$('#btnPdfViewAtras').click(function (e) {
    //    var indice = Number($('#idImagen').val());
    //    if (indice <= 0)
    //        indice = 0;
    //    else
    //        indice = indice - 1;

    //    $('#idImagen').val(indice);
    //    initFrame(satDropzone.files[indice].pathExpediente);
    //});
});