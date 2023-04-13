
//Funcion encargada de realizar el ajaxs al ordenar SortOrder de un webgrid, es importante
//que el div como figura en la funcion que contiene a la tabla tenga ID =gridContainer
$(document).ready(function () {
    $(document).on('click', '#grid thead th a, #grid tfoot td a', function (evt) {
        var container = $(this).parents('#gridContainer');
        if (container.attr('data-grid-url')) {
            container.data().gridUrl = this.href;
        }
        $.get(this.href, function (data) {
            container.html(data);
        });
        return false;
    });	
});


