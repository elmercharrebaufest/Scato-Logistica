jQuery(document).ready(function ($) {
    $("#FechaDesde").attr('autocomplete', 'off');
    $("#FechaHasta").attr('autocomplete', 'off');

    Totalizador("#grid", [5, 6, 7, 8, 9], 10, 4);
});