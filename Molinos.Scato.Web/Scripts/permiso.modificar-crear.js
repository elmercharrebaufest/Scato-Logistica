$(document).ready(function () {
    //verifico si el permiso es actividad y si eligió actividad
    $.validator.addMethod("actividadObligatoria", function (value, element) {
        if ($('#TipoPermiso').val() == $('#PermisoActividad').val() && $('#actividades').val() == $("#actividades option:first").val()) {
            return false;
        } else {
            return true;
        }
    }, $('#ErrorActividadObligatoria').val());
});

