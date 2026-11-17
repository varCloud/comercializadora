// Módulo Configuración (solo Administrador). Una función init por sección, llamadas desde el ready.

/* ---------- NIP de fórmula de producción ---------- */

var LONGITUD_NIP = 4;

function initNipFormula() {
    $('#frmNipFormula .solo-digitos').on('input', function () {
        this.value = this.value.replace(/\D/g, '').substring(0, LONGITUD_NIP);
    });

    // Ojito: alterna entre ocultar y mostrar lo que se escribe en el campo asociado.
    $('#frmNipFormula .btn-ver-nip').on('click', function () {
        var $boton = $(this);
        var $campo = $($boton.data('target'));
        var mostrar = $campo.attr('type') === 'password';
        $campo.attr('type', mostrar ? 'text' : 'password');
        $boton.attr('title', mostrar ? 'Ocultar NIP' : 'Mostrar NIP');
        $boton.find('i').toggleClass('fa-eye', !mostrar).toggleClass('fa-eye-slash', mostrar);
    });

    $('#frmNipFormula').on('submit', function (e) {
        e.preventDefault();
        var nip = $('#txtNipNuevo').val();
        var confirmacion = $('#txtNipConfirmacion').val();
        var reNip = new RegExp('^\\d{' + LONGITUD_NIP + '}$');

        if (!reNip.test(nip)) {
            MuestraToast("error", "El NIP debe tener exactamente 4 dígitos numéricos");
            return;
        }
        if (nip !== confirmacion) {
            MuestraToast("error", "La confirmación del NIP no coincide");
            return;
        }

        $.ajax({
            url: rootUrl("/Configuracion/CambiarNipFormula"),
            data: { nipNuevo: nip, confirmacionNip: confirmacion },
            method: 'post',
            dataType: 'json',
            async: true,
            beforeSend: function () {
                ShowLoader("Guardando NIP...");
            },
            success: function (data) {
                OcultarLoader();
                if (data.Estatus === 200) {
                    MuestraToast("success", data.Mensaje);
                    $('#frmNipFormula')[0].reset();
                    // Tras guardar, los campos vuelven a quedar ocultos.
                    $('#frmNipFormula .solo-digitos').attr('type', 'password');
                    $('#frmNipFormula .btn-ver-nip').attr('title', 'Mostrar NIP').find('i').removeClass('fa-eye-slash').addClass('fa-eye');
                } else {
                    MuestraToast("error", data.Mensaje);
                }
            },
            error: function () {
                OcultarLoader();
                MuestraToast("error", "Ocurrió un error al cambiar el NIP");
            }
        });
    });
}

$(document).ready(function () {
    initNipFormula();
});
