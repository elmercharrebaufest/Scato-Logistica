namespace Molinos.Scato.Dominio.Enums
{

    public enum TipoErrorComplianceWebService : int
    {
        Ok = 0,
        NoExisteVehiculoT = 1,
        NoExisteVehiculoA = 2,
        VehiculoTNoPertenece = 3,
        VehiculoANoPertenece = 4,
        VehiculoTesA = 5,
        VehiculoAesT = 6,
        ChoferNoExiste = 7,
        ChoferNoPertenece = 8,
        EmpresaNoExiste = 9,
        ChoferBloqueadoPorAvl = 13,
        TractorBloqueadoPorAvl = 14,
        AcompladoBloqueadoPorAvl = 15,
        ChoferBloqueadoPorSubcontratista = 16,
        TractorBloqueadoPorSubcontratista = 17,
        AcopladoBloqueadoPorSubcontratista = 18,
        ErrorDeConexion = 20
    }
}

