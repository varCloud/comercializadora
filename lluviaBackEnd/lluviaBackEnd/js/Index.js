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
    });

}
/*
function rootUrl(url) {
    var _rootUrl = '@Url.Content("~")';
    var x = url;
    if (url.indexOf(_rootUrl) != 0) {
        x = _rootUrl + "/" + url;
        x = x.replace(/\/\//g, "/").replace(/\/\//g, "/");
    }
    return x;
};
*/

$(document).ready(function () {
    console.log("index ready");

    toDataURL(
        'http://localhost:56196/assets/img/logo_lluvia.png',
        function (dataUrl) {
            //console.log('base64:', dataUrl)
            logoBase64 = dataUrl;
        }
    )
});