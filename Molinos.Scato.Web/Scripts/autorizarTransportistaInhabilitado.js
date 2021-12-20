$(document).ready(function () {
    $(document).on('click', '.confirmar-boton', function () {
        $("#dialogo-confirmar-confirmar").val($(this).data().accion);
       
        $("#demorar").hide();
        
        if ($(this).data().accion == "Autorizar" || $(this).data().accion == "TerminarInhabilitaciones") {
            $("#terminarInhabilitacion").hide();
            $("#MotivoTerminarInhabilitacion").hide();
            $("#motivo").show();
            $("#MotivoAutorizacion").show();
            $("#dialogo-confirmar-text").text($(this).data().mensaje).hide();
            $("#dialogo-confirmar").addClass("margen-400");

        } else if ($(this).data().accion == "Demorar") {
            $("#motivoDemora").val("");
            $("#demorar").show();
            
            $("#dialogo-confirmar-confirmar").click(function (e) {
                if ($("#motivoDemora").val() == "" || $("#motivoDemora").val().length < "10") {
                    $("#requerido").hide();
                    $("#largoMensaje").hide();
                    e.preventDefault();
                    if ($("#motivoDemora").val() == "") {
                        $("#requerido").show();
                    }
                    else if ($("#motivoDemora").val().length < "10") {
                        $("#largoMensaje").show();
                    }
                }
            });
        }else {
            $("#motivo").hide();
            $("#MotivoAutorizacion").hide();
            $("#terminarInhabilitacion").hide();
            $("#MotivoTerminarInhabilitacion").hide();
            $("#dialogo-confirmar-text").text($(this).data().mensaje).show();
            $("#dialogo-confirmar").removeClass("margen-400");

            
        }
        $("#dialogo-confirmar").on('hide.bs.modal', function () {            
            $("#MotivoAutorizacion").val("");
            $("#MotivoTerminarInhabilitacion").val("");
            $("#dialogo-confirmar-confirmar").off("click");
        });
        $("#dialogo-confirmar").modal('show');
        return false;
    });
   
});