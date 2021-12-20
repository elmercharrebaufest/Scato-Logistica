using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorSincronizarProveedores : ProcesadorSincronizar<SincronizarProveedores>
    {
        public ProcesadorSincronizarProveedores(IRepositorio repositorio, IConversor conversor, ZSDWS_SCATO servicioSap, ILogger log)
            : base(repositorio, conversor, servicioSap, log)
        {
        }

        protected override int ProcesarSincronizacion(SincronizarProveedores comando, DateTime fechaEjecucion, out Resultado resultado)
        {
            resultado = new ResultadoSincronizarProveedores();
            //Carga Masiva determina si se busca un registro o todos. 
            //En caso de ser masiva se debe pasar la fecha como parámetro.
            var cuit = comando.Cuit != null ? comando.Cuit.Replace("-", string.Empty) : string.Empty;
            var fecha = comando.CargaMasiva ? fechaEjecucion.ToString("yyyy-MM-dd") : string.Empty;

            Log.Debug("Obteniendo Proveedores del tipo 'PR'...");
            var datosSapPr = ServicioSap.DatosProveedores(
                new DatosProveedoresRequest(new DatosProveedores
                    {
                        CUIT = cuit,
                        Fecha = fecha,
                        IdSAP = string.Empty,
                        TipoProveedor = "PR"
                    })).DatosProveedoresResponse.Proveedores.Select(p => new {Prov = p, Rol = "PR"});
            Log.Debug("Obteniendo Proveedores del tipo 'AM'...");
            var datosSapAm = ServicioSap.DatosProveedores(
                new DatosProveedoresRequest(new DatosProveedores
                {
                    CUIT = cuit,
                    Fecha = fecha,
                    IdSAP = string.Empty,
                    TipoProveedor = "AM"
                })).DatosProveedoresResponse.Proveedores.Select(p => new {Prov = p, Rol = "AM"});
            Log.Debug("Obteniendo Proveedores del tipo 'VM'...");
            var datosSapVm = ServicioSap.DatosProveedores(
                new DatosProveedoresRequest(new DatosProveedores
                    {
                        CUIT = cuit,
                        Fecha = fecha,
                        IdSAP = string.Empty,
                        TipoProveedor = "VM"
                    })).DatosProveedoresResponse.Proveedores.Select(p => new {Prov = p, Rol = "VM"});
            Log.Debug("Obteniendo Proveedores del tipo 'CM'...");
            var datosSapCm = ServicioSap.DatosProveedores(
                new DatosProveedoresRequest(new DatosProveedores
                    {
                        CUIT = cuit,
                        Fecha = fecha,
                        IdSAP = string.Empty,
                        TipoProveedor = "CM"
                    })).DatosProveedoresResponse.Proveedores.Select(p => new {Prov = p, Rol = "CM"});

            var datosSap = datosSapPr.Union(datosSapAm).Union(datosSapVm).Union(datosSapCm);

            var proveedoresSap = datosSap.GroupBy(p => p.Prov.LIFNR,
                                                  (cod, provs) =>
                                                  new {
                                                      Proveedor = provs.First().Prov, 
                                                      Roles = provs.Select(p => p.Rol).ToList()
                                                  }).ToList();
            var proveedoresDescargados = proveedoresSap.Count;
            Log.Debug("Total proveedores descargados: {0}", proveedoresDescargados);
            Log.Debug("Comenzando sincronización...");
            var cantidad = 0;
            foreach (var proveedorRoles in proveedoresSap)
            {
                var proveedorSap = proveedorRoles.Proveedor;
                var codigoSap = proveedorSap.LIFNR.TrimStart(new[] {'0'});
                var proveedor = Repositorio.Obtener<Proveedor>(x => x.CodigoSap == codigoSap);
                if (proveedor == null)
                {
                    Log.Debug("Sincronización - Insertando nuevo proveedor: {0} - {1}.", proveedorSap.LIFNR, proveedorSap.NAME1);
                    proveedor = new Proveedor {CodigoSap = codigoSap};
                    ActualizarProveedor(proveedor, proveedorSap, proveedorRoles.Roles);
                    if (proveedor.Activo)
                    {
                        proveedor = Repositorio.Agregar(proveedor);
                    }
                }
                else
                {
                    Log.Debug("Sincronización - Actualizando proveedor: {0} - {1}.", proveedorSap.LIFNR, proveedorSap.NAME1);
                    ActualizarProveedor(proveedor, proveedorSap, proveedorRoles.Roles);
                }
                cantidad++;
                if (comando.RetornarResultado)
                {
                    ((ResultadoSincronizarProveedores)resultado).AgregarResultado(Conversor.Convertir<Proveedor, ProveedorDto>(proveedor));
                }
            }
            ((ResultadoSincronizarProveedores)resultado).Cantidad = cantidad;
            return cantidad;
        }

        protected override string NombreInterface()
        {
            return "Proveedores";
        }

        private void ActualizarProveedor(Proveedor proveedor, ZSDES3133 proveedorSap, IList<string> roles)
        {
            proveedor.Cuil = MascaraCuit(proveedorSap.STCD1);
            proveedor.Descripcion = proveedorSap.NAME1;
            proveedor.RazonSocial = proveedorSap.NAME1;
            int codigoProvincia;
            if (int.TryParse(proveedorSap.PROV_ONCCA, out codigoProvincia))
            {
                proveedor.Provincia = Repositorio.Obtener<Provincia>(x => x.CodigoAfip == codigoProvincia);
            }
            proveedor.Activo = proveedorSap.ACTIVO == "X";
            proveedor.PR = roles.Contains("PR");
            proveedor.CM = roles.Contains("CM");
            proveedor.AM = roles.Contains("AM");
            proveedor.VM = roles.Contains("VM");
        }

        private class ProveedorSapEqualityComparer : IEqualityComparer<ZSDES3133>
        {
            public bool Equals(ZSDES3133 x, ZSDES3133 y)
            {
                return x.LIFNR == y.LIFNR;
            }

            public int GetHashCode(ZSDES3133 obj)
            {
                return obj.LIFNR.GetHashCode();
            }
        }
    }
}
