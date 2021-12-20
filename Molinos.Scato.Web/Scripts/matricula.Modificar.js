$(document).ready(function () {
    $('#dialogo-editar-guardar').unbind("click");
    $('#dialogo-editar-guardar').click(function (e) {
        var form = document.getElementById("formMatricula");
        if (form.getAttribute("enctype") === "multipart/form-data") {
            if (form.dataset != undefined && form.dataset.ajax) {
                e.preventDefault();
                e.stopImmediatePropagation();
                var xhr = new XMLHttpRequest();
                xhr.open(form.method, form.action);
                xhr.onreadystatechange = function () {
                    if (xhr.readyState == 4 && xhr.status == 200) {
                        if (xhr.responseText == window.ajaxEditSuccess) {
                            $('#dialogo-editar').modal('hide');
                            var container = $('#gridContainer');
                            if (container.length > 0 && container.is("[data-grid-url]")) {
                                $.get(container.data().gridUrl, function (data) {
                                    container.html(data);
                                });
                            } else {
                                $('#search-form').submit();
                            }
                            MostrarAlertaExitosa();
                        } else {
                            $('#dialogo-editar-body').html(xhr.responseText);
                            $("#dialogo-editar-guardar").attr("disabled", false);
                        }
                    }
                };
                xhr.send(new FormData(form));
                return false;
            }}
        
    });


    //window.addEventListener("submit", function (e) {
    //    var form = e.target;
    //    if (form.getAttribute("enctype") === "multipart/form-data") {
    //        if (form.dataset.ajax) {
    //            e.preventDefault();
    //            e.stopImmediatePropagation();
    //            var xhr = new XMLHttpRequest();
    //            xhr.open(form.method, form.action);
    //            xhr.onreadystatechange = function () {
    //                if (xhr.readyState == 4 && xhr.status == 200) {
    //                    if (form.dataset.ajaxUpdate) {
    //                        var updateTarget = document.querySelector(form.dataset.ajaxUpdate);
    //                        if (updateTarget) {
    //                            updateTarget.innerHTML = xhr.responseText;
    //                        }
    //                    }
    //                }
    //            };
    //            xhr.send(new FormData(form));
    //        }
    //    }
    //}, true);
    //$("#firmaBoton").filestyle({ buttonName: "btn-primary sinMargin", buttonText: "Cargar", size: "medium", icon: false });
   
});
