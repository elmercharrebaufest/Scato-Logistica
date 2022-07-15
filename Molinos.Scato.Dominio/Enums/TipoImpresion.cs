namespace Molinos.Scato.Dominio.Enums
{
    //Al agregar una impresion nueva al enum se debe insertar en la tabla TipoImpresion y en Datos base.sql
    public enum TipoImpresion : int
    {
        AsignacionDeRuta = 0,
        CertificadoDeAnalisis = 1,
        CertificadoDeCartaPorte = 2,
        ConstanciaDeEntregaLaser = 3,
        DeclaracionFosfina = 4,
        DocumentoDeEntrada = 5,
        Formulario239 = 6,
        IdentificacionEnvioLoteACamara = 7, 
        IdentificacionMicromuestra = 8,
        IdentificacionMuestraCalado = 9,
        TicketPesada = 10,
        SolicitudDeAnalisis = 11,
        ImpresionGenerica = 12,
        ResumenDeRecepcion = 13,
        InformeDeRecepcion = 14,
        ReciboMunicipal = 15,
        ControlDeCarga = 16,
        IdentificacionMuestraAuditoria = 17,
        EtiquetaAuditoria = 18,
        EtiquetaIntacta = 19,
        TicketPesadaBodega = 20,
        TicketPesadaAduana = 21,
        EtiquetaRubrosAnalizar = 22,
        ReciboMunicipalImportacion = 23,
        AsigRecorrCtrolCalid = 24,
        CartaPorteUrenport = 25,
        GaritaSalida = 26,
        ResumenHojaDeRuta = 27,
        EtiquetaAuditoriaCamara = 28,
        CartaDePorteElectronica = 29,
        EtiquetaMuestraInase = 30
    }
}
