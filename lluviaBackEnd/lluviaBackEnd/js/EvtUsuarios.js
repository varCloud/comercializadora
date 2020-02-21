

var table;
var iframe;

function PintarTabla() {
    $.ajax({
        url: rootUrl("/Socio/_ObtenerSocio"),
        data: { idSocio: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            table.destroy();
            $('#rowTblSocio').html(data);
            InitDataTable();
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
        }
    });
}

function initForm() {
    $("#frmSocio").validate({
        rules: {
            nombre: "required",
            apellidos: "required",
            telefono: {
                required: true,
                minlength: 10,
                maxlength: 10,
                digits: true
            },
            mail: {
                required: true,
                email: true
            },

        },
        messages: {
            nombre: "Este campo no puede estar vacio",
            apellidos: "Este campo no puede estar vacio",
            telefono: {
                required: "Este campo no puede estar vacio",
                minlength: "El minimo para el telefono son 10 digitos",
                maxlength: "El maximo para el telefono son 10 digitos"
            },
            mail: {
                required: "Este campo no puede estar vacio",
                email: "Correo invalido ejem :var901106@gmail.com"
            }
        },
        errorElement: "em",
        errorPlacement: function (error, element) {
            // Add the `help-block` class to the error element
            error.addClass("help-block");
            error.css("color", "#e3324c");

            if (element.prop("type") === "checkbox") {
                error.insertAfter(element.parent("label"));
            } else {
                error.insertAfter(element);
            }
        },
        highlight: function (element, errorClass, validClass) {
            $(element).parents(".col-sm-5").addClass("has-error").removeClass("has-success");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).parents(".col-sm-5").addClass("has-success").removeClass("has-error");
        }
    });
}

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
                "first": "First",
                "last": "Last",
                "next": "Siguiente",
                "previous": "Anterior"
            },
        },
        "dom": 'Bfrtip',
        buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
        buttons: [
            {
                extend: 'copy',
                text: '<i class="fa fa-copy" style="font-size:20px"></i>',
                className: 'btn btn-icon btn-round btn-outline-warning',
                titleAttr: 'Copiar',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                },
            },
            {
                extend: 'pdfHtml5',
                text: '<i class="fa fa-file-pdf-o"  style="font-size:20px"></i>',
                className: 'btn btn-icon btn-round btn-outline-info',
                titleAttr: 'Exportar a PDF',
                title: "Socios Cedicoop",
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
                text: '<i class="fa fa-file-excel-o"  style="font-size:20px"></i>',
                className: 'btn btn-icon btn-round btn-outline-success',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                },
            },

        ],

        "bDestroy": true, // es necesario para poder ejecutar la funcion LimpiaTabla()
    });

    //$('#tablaUsuarios').addClass('form-control');
   // $('#tablaUsuarios').append('&nbsp;&nbsp;<button value=""  title="Agregar" data-toggle="tooltip"  class="tooltip-wrapper btn btn-icon btn-round btn-outline-dark" name="" id="btnAgregarSocio"><i class="fa fa-plus"></i></button>');
    //InitBtnAgregar();
}

function InitBtnAgregar() {
    $('#btnAgregarSocio').click(function (e) {
        $('#idSocio').val(0)
        satDropzone.removeAllFiles();
        $('#dropZoneExpediente').css('display', '');
        $('#rowExpedientes').css('display', 'none');
        $('#Nombre').prop('disabled', false);
        $('#Apellidos').prop('disabled', false);
        $('#Telefono').prop('disabled', false);
        $('#Mail').prop('disabled', false);
        $('#NumeroSocioCMV').prop('disabled', false);
        $('#idSocio').val('');
        $('#btnReseFrm').trigger('click');
        $('#verticalCenter').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#verticalCenterTitle').html("Agregar Usuario")
    });
}

function PintaIconoPreview(file) {
    var img = file.previewElement.querySelector("img");
    var ext = (file.name).split('.')[1]
    if (ext === 'pdf') {
        $(img).attr("src", rootUrl("/assets/img/file-icon/pdf.png"));
    }
    $(img).css('width', '120');
}

function InitDrop() {

    $("#dropZoneExpediente").append("<form id='FrmdropZoneExpediente' class='dropzone borde-dropzone' style='cursor: pointer;'></form>");
    DropzoneOptions = {
        url: rootUrl("/Socio/AgregarSocio"),
        addRemoveLinks: true,
        paramName: "archivo",
        maxFilesize: 4, // MB
        dictRemoveFile: "Eliminar",
        acceptedFiles: ".pdf,.jpg,.png",
        //maxFiles: 50,
        parallelUploads: 20,
        uploadMultiple: true,
        autoProcessQueue: false, // true para envíar en automatico
        init: function () {
            this.on("maxfilesexceeded", function (file) {
                this.removeFile(file);
                swal("Error", "No se puede subir mas de un archivo", "error");
            });
            this.on("addedfile", function (file) {
                PintaIconoPreview(file);
            });
            this.on("removedfile", function (file) {
                if ($('#idSocio').val() !== '0') {
                    EliminarExpediente($('#idSocio').val(), file);
                }
            });
            this.on("complete", function (file) {
                PintaIconoPreview(file);
            });

        },
        /*
         * ESTA FUNCION SE UTILIZA PARA CUANDO SOLO SE ENVIA UN SOLO ARCHIVO
         * sending: function (file, xhr, formData) {
            console.log('sendingmultiple');
        },
        success: function (file, data) {
            file.previewElement.classList.add("dz-success");
            if (data.Estatus == 200)
                swal("Notificación", data.Mensaje, data.TipoAlerta);
            $('#verticalCenter').modal('hide');
            ObtenerSocio(0);
        },
        */
        sendingmultiple: function (file, xhr, formData) {
            var formSocio = $("#frmSocio").serializeArray();
            var socio = castFormToJson(formSocio);
            for (var key in socio) {
                formData.append(key, socio[key]);
            }
            console.log('sendingmultiple');
        },
        successmultiple: function (file, data) {
            console.log(data);
            if (data.Estatus == 200) {
                swal("Notificación", data.Mensaje, data.TipoAlerta);
                PintarTabla();
            }
            $('#verticalCenter').modal('hide');
        },
        processingmultiple: function (file, data) {
            console.log(file);
        },
        error: function (file, response) {
            file.previewElement.classList.add("dz-error");
            $('#verticalCenter').modal('hide');
            console.log(response);

        }
    } // FIN myAwesomeDropzone
    satDropzone = new Dropzone("#FrmdropZoneExpediente", DropzoneOptions);


}

function initFrame(url) {

    iframe = $('<embed src="" style="width: 90%;margin-left:5%" height="600" alt="pdf" pluginspage="http://www.adobe.com/products/acrobat/readstep2.html">');
    iframe.innerHTML = "";
    iframe.attr('src', rootUrl(url));
    $('#rowPDFView').html('');
    $('#rowPDFView').append(iframe);
    var content = iframe.innerHTML;
    iframe.innerHTML = content;
}

function castFormToJson(formArray) {
    var obj = {};
    $.each(formArray, function (i, pair) {
        var cObj = obj, pObj = {}, cpName;
        $.each(pair.name.split("."), function (i, pName) {
            pObj = cObj;
            cpName = pName;
            cObj = cObj[pName] ? cObj[pName] : (cObj[pName] = {});
        });
        pObj[cpName] = pair.value;
    });
    //obj["idRow"] = 0;
    return obj;
}

function EliminarExpediente(idSocio, file) {
    if (typeof file.id !== 'undefined') {
        var mockFile = { nombreDoc: file.name, id: file.id, pathExpediente: file.pathExpediente };
        $.ajax({
            url: rootUrl("/Socio/EliminarExpediente"),
            data: { idSocio: idSocio, exp: mockFile },
            method: 'post',
            dataType: 'json',
            async: false,
            beforeSend: function (xhr) {
            },
            success: function (data) {
                swal("Notificación", data.Mensaje, data.TipoAlerta);
            },
            error: function (xhr, status) {
                console.log('Disculpe, existió un problema');
                console.log(xhr);
                console.log(status);
            }
        });
    }

}

function ObtenerSocio(idSocio) {

    var result = '';
    $.ajax({
        url: rootUrl("/Socio/ObtenerSocio"),
        data: { idSocio: idSocio },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            console.log("Antes de enviar")
        },
        success: function (data) {

            result = data;
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
        }
    });

    return result;
}

function EditarSocio(idSocio) {

    var data = ObtenerSocio(idSocio);
    $('#idSocio').val(0)
    $('#btnReseFrm').trigger('click');
    $('#Nombre').val(data.Nombre).prop('disabled', false);
    $('#Apellidos').val(data.Apellidos).prop('disabled', false);
    $('#Telefono').val(data.Telefono).prop('disabled', false);
    $('#Mail').val(data.Mail).prop('disabled', false);
    $('#NumeroSocioCMV').val(data.NumeroSocioCMV).prop('disabled', false);
    satDropzone.removeAllFiles();
    $('#idSocio').val(data.IdSocio).prop('disabled', false);
    $.each(data.Expedientes, function (index, value) {

        var URLdomainImage = "http://" + window.location.host + value.pathExpediente;
        var mockFile = { name: value.nombreDoc, size: value.pesoByte, id: value.id, pathExpediente: value.pathExpediente };
        satDropzone.emit("addedfile", mockFile);
        satDropzone.files.push(mockFile);
        satDropzone.options.thumbnail.call(satDropzone, mockFile, URLdomainImage);
        satDropzone.emit("complete", mockFile);
    });
    $('#dropZoneExpediente').css('display', '');
    $('#rowExpedientes').css('display', 'none');
    // para abrir el modal 
    $('#verticalCenter').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#verticalCenterTitle').html("Editar Socio");

}

function EliminarSocio(idSocio) {

    swal({
        title: "Mensaje",
        text: "Estas seguro que deseas eliminar al socio?",
        type: 'question',
        showCancelButton: true,
        cancelButtonText: "Cancelar",
        confirmButtonText: 'Si, Eliminar!',
    }).then((result) => {
        if (result.value) {
            $.ajax({
                url: rootUrl("/Socio/ActualizarEstatusSocio"),
                data: { idSocio: idSocio, estatus: false },
                method: 'post',
                dataType: 'json',
                beforeSend: function (xhr) {
                    console.log("Antes de enviar")
                },
                success: function (data) {
                    swal({
                        type: data.TipoAlerta,
                        title: 'Notificación',
                        text: data.mensaje,
                    })
                    PintarTabla();
                },
                error: function (xhr, status) {
                    console.log('Disculpe, existió un problema');
                    console.log(xhr);
                    console.log(status);
                }
            });

        } else {
            console.log("cancelar" + result);
        }
    });

}

function VerExpediente(idSocio) {
    var data = ObtenerSocio(idSocio);
    var cuadro = '';
    console.log(data);

    $('#Nombre').val(data.Nombre).prop('disabled', true);
    $('#Apellidos').val(data.Apellidos).prop('disabled', true);
    $('#Telefono').val(data.Telefono).prop('disabled', true);
    $('#Mail').val(data.Mail).prop('disabled', true);
    $('#NumeroSocioCMV').val(data.NumeroSocioCMV).prop('disabled', true);
    $.each(data.Expedientes, function (index, value) {
        var mockFile = { nombreDoc: value.nombreDoc, id: value.id, indice: index, pathExpediente: value.pathExpediente };
        satDropzone.files.push(mockFile);
        var ext = (value.nombreDoc).split('.')[1];
        var img = '';
        if (ext === 'pdf')
            img = "/assets/img/file-icon/pdf.png";
        else
            img = value.pathExpediente;


        cuadro += '<div class="col-xl-3 col-sm-6">';
        cuadro += '<div class="card card-statistics" >';
        cuadro += '<div class="card-body">';
        cuadro += '<div class="text-center p-2">';
        cuadro += '<div class="mb-2">';

        //cuadro += '<img src="/assets/img/file-icon/' + img + '" alt="png-img"></div>';
        cuadro += '<img src="' + img + '" alt="png-img" style="width:120px"></div>';

        cuadro += '<h4 class="mb-0" style="font-size:12px">' + value.nombreDoc + '</h4>';
        //cuadro += '<p class="mb-2">28.8 kb</p>';
        cuadro += "<button onclick='PDFView(\"" + value.pathExpediente + "\" , " + index + ")' class='btn btn-light'>Ver</button>";
        cuadro += '</div>';
        cuadro += '</div>';
        cuadro += '</div>';
        cuadro += ' </div >';
    });
    $('#dropZoneExpediente').css('display', 'none');
    $('#rowExpedientes').css('display', '');
    $('#rowExpedientes').html(cuadro);
    $('#verticalCenter').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#verticalCenterTitle').html("Información del socio");
}

function PDFView(url, indice) {

    $('#idImagen').val(indice);
    initFrame(url);
    $('#modalPDFView').modal({ backdrop: 'static', keyboard: false, show: true });

}

$(document).ready(function () {
    //InitDrop();
    //initForm();
    InitDataTable();


    //$('#btnGuardarSocio').click(function (e) {
    //    if ($("#frmSocio").valid()) {
    //        console.log('btnGuardarSocio');
    //        satDropzone.processQueue();
    //    } else
    //        console.log("IN-valido")
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