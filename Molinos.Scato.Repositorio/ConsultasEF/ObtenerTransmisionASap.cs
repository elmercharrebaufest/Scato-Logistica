using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerTransmisionASap : IConsultaEscalar<TransmisionASap>
    {
        private readonly int id;

        public ObtenerTransmisionASap(int id)
        {
            this.id = id;
        }

        private Dictionary<FuncionSAP, string> TablaPorFuncion { get; } = new Dictionary<FuncionSAP, string>
        {
            { FuncionSAP.IngresosPorCompraDeGranos , "IngresosPorCompraDeGranosTransmisionASap" },
            { FuncionSAP.SalidaDeOrigenEnRedespachos , "SalidaDeOrigenEnRedespachosTransmisionASap" },
            { FuncionSAP.LlegadaADestinosEnRedespachos , "LlegadaAdestinosEnRedespachosTransmisionASap" },
            { FuncionSAP.AjusteDeDiferencias , "AjusteDeDiferenciasEnRedespachosTransmisionASap" },
            { FuncionSAP.EgresosMaterialNoProductivo , "EgresosNoProductivosTransmisionASap" },
            { FuncionSAP.IngresosEgresosFazones , "IngresosEgresosFazonesTransmisionASap" },
            { FuncionSAP.PesaNeto , "PesaNetoTransmisionASap" },
            { FuncionSAP.EgresoSinFleteFazones , "EgresoSinFleteFasonesTransmisionASap" },
            { FuncionSAP.CartaPorteTransporteAutomotorRegistro , "CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto" },
            { FuncionSAP.CartaPorteVagonFerroviarioRegistro , "CartaPorteVagonFerroviarioRegistroTransmisionAMonsanto" },
            { FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro , "MuestreoPesajeTransporteAutomotorTransmisionAMonsanto" },
            { FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro  , "MuestreoPesajeVagonFerroviarioTransmisionAMonsanto" },
            { FuncionSAP.InformarCupo  , "InformarCupoTransmisionASap" },
            { FuncionSAP.IngresosBodega , "IngresosBodegaTransmisionASap" },
            { FuncionSAP.ZE7550 , "ZE7550TransmisionASap" },
            { FuncionSAP.FletesDobleTramo, "ZE7550TransmisionASap" }
        };

        public virtual TransmisionASap Ejecutar(DbContext contexto)
        {

            var impresion = contexto.Database.SqlQuery<FuncionSAP?>(
                 "Select FuncionSAP from TransmisionASap where TransmisionASap.Id = @id"
                 , new SqlParameter("@id", id)).FirstOrDefault();

            if (impresion == null) return null;

            var type = Type.GetType(typeof(TransmisionASap).AssemblyQualifiedName.Replace("TransmisionASap", TablaPorFuncion[impresion.Value]));


            return typeof(ObtenerTransmisionASap).GetMethod("Obtener")
                .MakeGenericMethod(type).Invoke(null, new object[] { contexto, id, TablaPorFuncion[impresion.Value] }) as TransmisionASap;
        }

        public static TransmisionASap Obtener<T>(DbContext contexto, int id, string tabla) where T : class
        {
            return contexto.Set<T>().SqlQuery(
                             $"SELECT * FROM TransmisionASap R INNER JOIN { tabla} imp on imp.id = R.id where R.id = @idd "
                             , new SqlParameter("@idd", id)).First() as TransmisionASap;
        }
    }
}
