using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorSincronizarClientes : ProcesadorSincronizar<SincronizarClientes>
    {
        public ProcesadorSincronizarClientes(IRepositorio repositorio, IConversor conversor, ZSDWS_SCATO servicioSap, ILogger log)
            : base(repositorio, conversor, servicioSap, log)
        {
        }

        protected override int ProcesarSincronizacion(SincronizarClientes comando, DateTime fechaEjecucion, out Resultado resultado)
        {
            resultado = new ResultadoSincronizarClientes();
            //Carga Masiva determina si se busca un registro o todos. 
            //En caso de ser masiva se debe pasar la fecha como parámetro.

            var cuit = comando.Cuit != null ? comando.Cuit.Replace("-", string.Empty) : string.Empty;
            var fecha = comando.CargaMasiva ? fechaEjecucion.ToString("yyyy-MM-dd") : string.Empty;

            var datosSap = ServicioSap.DatosCliente(
                new DatosClienteRequest(new DatosCliente
                    {
                        CUIT = cuit,
                        Fecha = fecha,
                        IdSAP = string.Empty
                    }));
            var clientesDescargados = datosSap.DatosClienteResponse.Clientes.Length;
            Log.Debug("Total clientes descargados: {0}", clientesDescargados);
            Log.Debug("Comenzando sincronización...");
            var cantidad = 0;
            foreach (var clienteSap in datosSap.DatosClienteResponse.Clientes)
            {
                var cliente = Repositorio.Obtener<Cliente>(x => x.CodigoSap == clienteSap.ID_SAP);
                if (cliente == null)
                {
                    Log.Debug("Sincronización - Insertando nuevo cliente: {0} - {1}.", clienteSap.ZCUIT, clienteSap.DESCRIPCION);
                    cliente = new Cliente();
                    ActualizarCliente(cliente, clienteSap);
                    if (cliente.Activo)
                    {
                        cliente = Repositorio.Agregar(cliente);
                    }
                }
                else
                {
                    Log.Debug("Sincronización - Actualizando cliente: {0} - {1}.", clienteSap.ZCUIT, clienteSap.DESCRIPCION);
                    ActualizarCliente(cliente, clienteSap);
                }
                cantidad++;
                if (comando.RetornarResultado)
                {
                    ((ResultadoSincronizarClientes)resultado).AgregarResultado(Conversor.Convertir<Cliente, ClienteDto>(cliente));
                }
                //if (cantidad % 1000 == 0)
                //{
                //    Log.Debug("Procesados {0}/{1}. Guardando cambios...", cantidad, clientesDescargados);
                //    Repositorio.GuardarCambios();
                //}
            }
            ((ResultadoSincronizarClientes)resultado).Cantidad = cantidad;

            return cantidad;
        }

        protected override string NombreInterface()
        {
            return "Clientes";
        }

        private static void ActualizarCliente(Cliente cliente, ZSDES5200 clienteSap)
        {
            cliente.CodigoSap = clienteSap.ID_SAP;
            cliente.Cuit = MascaraCuit(clienteSap.ZCUIT);
            cliente.Descripcion = clienteSap.DESCRIPCION;
            cliente.Activo = clienteSap.ACTIVO == "X";
            cliente.Direccion = clienteSap.ZCALLE + " " + clienteSap.ZNUMERO;
            cliente.Localidad = clienteSap.ZLOCALIDAD;
            cliente.Provincia = clienteSap.ZPROVINCIA;
        }
    }
}
