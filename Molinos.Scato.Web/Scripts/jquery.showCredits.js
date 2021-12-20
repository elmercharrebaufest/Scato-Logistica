//ShowDialog jQuery Plugin
//Requires jQuery and jQuery UI
; (function ($) {
    var defaults = {
        message: '',
        title: '',
        iconClass: '',
        onClick: function () {
            $('#dialog-credits').modal('hide');
        }
    };
    
    $.showCredits = function (params) {
        var params = $.extend({}, defaults, params);

        $('#dialog-credits').modal({
            backdrop: 'static', keyboard: true
        }).css({
            width: '50%'
            , 'margin-left': function () {
                return -($(this).width() / 2);
            },
            'top': '40%',
            'margin-top': function () {
                return -($(this).height() / 2);
            }
        });

        var title = '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button><div>' +
                        '<div> ' + params.title + '</div>' +
                        '<div class="ui-dialog-subtitle"><span style="font-size: 10px;">Versión: ' + params.version + ' - Fecha: ' + params.fecha + '</span></div>' +
                    '</div>';

        $("#header-credits").html(title);
        
        var baufest = '<span style="float:left; margin-top: 30px; padding-left:20px; padding-right:30px; width: 50%; border-right: 1px dashed; border-color: #D3D3D3;">' +
                        (params.sdl ? '<strong>Líder de Proyecto:</strong> ' + params.sdl + '<br />' : '') +
                        (params.analista ? '<strong>Analista Funcional:</strong> ' + params.analista + '<br />' : '') +
                        (params.tel ? '<strong>Líder Técnico:</strong> ' + params.tel + '<br />' : '') +
                        (params.desarrolladores ? '<strong>Desarrolladores:</strong> ' + params.desarrolladores + ' <br />' : '') +
                        (params.testers ? '<strong>Tester:</strong> ' + params.testers : '') +
                      '</span>';

        var cliente = '<span style="position:absolute; margin-top: 30px; padding-left: 30px; padding-right: 30px;">' +
                        (params.liderCliente ? '<strong>Project Manager:</strong> ' + params.liderCliente + '<br />' : '') +
                        (params.analistaCliente ? '<strong>Analistas Funcionales:</strong> ' + params.analistaCliente + '<br />' : '') +
                      '</span>';

        var logos = '<div style="float:left; width:100%">' +
                       '<span style="float:left; padding-left:20px; padding-right:30px; padding-top: 20px; width: 50%; border-right: 1px dashed; border-color: #D3D3D3;"><a href="http://www.baufest.com" target="_blank" ><img src="' + params.logoBaufest + '" style="height: 30px; float:right;" /></a></span>' +
                       '<span style="position:absolute; width: 30%; padding-left: 30px; padding-right: 30px;"><img src="' + params.logoCliente + '" style="float:right;" /></span>' +
                     '</div>';
        
        $("#body-credits").html('<div class="row">' + baufest + cliente + logos + '</div>');
        
        var aceptar = '<input id="aceptar-credits" type="button" class="btn btn-primary" value="Aceptar"/>';

        $("#footer-credits").html(aceptar);

        $("#aceptar-credits").click(params.onClick);
        
    };

    $.showVersionsHistory = function (params) {
        var params = $.extend({}, defaults, params);

        $('#dialog-historial-credits').modal({
            backdrop: 'static', keyboard: true
        });

        var title = '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button><div>' +
                    '<div> ' + params.title + '</div>' +
                    '<div class="ui-dialog-subtitle"><span style="font-size: 10px;">' + params.subtitle + '</span></div>' +
                    '</div>';
        
        $("#header-historial-credits").html(title);
        
        var logos = '<div style="float:left; width:100%">' +
                       '<span style="float:left; padding-left:20px; padding-right:30px; width: 50%; border-right: 0px dashed; border-color: #D3D3D3;"><a href="http://www.baufest.com" target="_blank" ><img src="' + params.logoBaufest + '" style="height: 30px; float:right;" /></a></span>' +
                       '<span style="position:absolute; width: 50%; padding-left: 30px; padding-right: 30px;"><img src="' + params.logoCliente + '" style="height: 50px; float:right;" /></span>' +
                     '</div>';

        var versiones = '';

        if (params.versiones) {
            $.each(params.versiones, function (index, element) {
                versiones += '<div style="width:100%; float:left;">' +
                                '<div style="width:100%; margin-top:20px;"><strong>Versión ' + element.Numero + '</strong></div>' +
                                '<div class="data-container">' +
                                    '<span style="float:left; padding-left:20px; padding-right:30px; width: 50%; border-right: 1px dashed; border-color: #D3D3D3;">' +
                                        (element.SDL ? '<strong>Líder de Proyecto:</strong> ' + element.SDL + '<br />' : '') +
                                        (element.Analista ? '<strong>Analista Funcional:</strong> ' + element.Analista + '<br />' : '') +
                                        (element.TEL ? '<strong>Líder Técnico:</strong> ' + element.TEL + '<br />' : '') +
                                        (element.Desarrolladores ? '<strong>Desarrolladores:</strong> ' + element.Desarrolladores + ' <br />' : '') +
                                        (element.Testers ? '<strong>Tester:</strong> ' + element.Testers : '') +
                                    '</span>';

                versiones += '<span style="position:absolute; padding-left: 30px; padding-right: 30px;">' +
                                        (element.LiderCliente ? '<strong>Project Manager:</strong> ' + element.LiderCliente + '<br />' : '') +
                                        (element.AnalistaCliente ? '<strong>Analistas Funcionales:</strong> ' + element.AnalistaCliente + '<br />' : '') +
                                    '</span>' +
                                '</div>' +
                             '</div>';
            });
        }
        
        $("#body-historial-credits").html('<div class="row">' + versiones + logos + '</div>');

        var ocultarHistorialVersiones = '<input id="ocultar-historial-credits" type="button" class="btn btn-primary" value="Ocultar Historial de Versiones" />';
        var aceptar = '<input id="aceptar-historial-credits" type="button" class="btn btn-primary" value="Aceptar"/>';

        $("#footer-historial-credits").html(ocultarHistorialVersiones + aceptar);

        $("#aceptar-historial-credits").click(function() {
            params.onClick();
            $('#dialog-historial-credits').modal('hide');
        });

        $("#ocultar-historial-credits").click(function() {
            $('#dialog-historial-credits').modal('hide');
        });
    };
}(jQuery));