using System;
using System.Linq;
using System.Transactions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCaracteristicaDeCalidad : ProcesadorComando<CrearCaracteristicaDeCalidad>
    {
        public ProcesadorCrearCaracteristicaDeCalidad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearCaracteristicaDeCalidad comando)
        {
            var resultado = new ResultadoCrear();
            CaracteristicaDeCalidad caracteristicaDeCalidad = null;
            CaracteristicaDeCalidadMaestro caracteristicaDeCalidadMaestro = null;
            using (var transaction = new TransactionScope())
            {
                try
                {
                    var existeCaracteristica =
                        Repositorio.Existe<CaracteristicaDeCalidadMaestro>(c => c.Descripcion == comando.Dto.Descripcion);
                    if (!existeCaracteristica)
                    {
                        caracteristicaDeCalidadMaestro = new CaracteristicaDeCalidadMaestro
                            {
                                Descripcion = comando.Dto.Descripcion
                            };
                        Repositorio.Agregar(caracteristicaDeCalidadMaestro);
                        Repositorio.GuardarCambios();
                    }
                    else
                    {
                        caracteristicaDeCalidadMaestro =
                            Repositorio.Obtener<CaracteristicaDeCalidadMaestro>(
                                c => c.Descripcion == comando.Dto.Descripcion);
                    }

                    caracteristicaDeCalidad = CrearEntidadCaracteristicaDeCalidadDetalle(comando);
                    Validar(comando, resultado);
                    if (!resultado.HayErrores)
                    {
                        caracteristicaDeCalidad.CaracteristicaDeCalidadMaestro = caracteristicaDeCalidadMaestro;
                        Repositorio.Agregar(caracteristicaDeCalidad);
                        Repositorio.GuardarCambios();
                        
                        resultado.Id = caracteristicaDeCalidad.Id;

                        var logueaEntidad = comando.GetType().GetCustomAttributes(true).Any(s => s.GetType() == typeof(LoguearEntidad));
                        if (logueaEntidad)
                        {
                            try
                            {
                                var logAbm = new LogABM
                                {
                                    Pantalla = comando.GetType().Name,
                                    Usuario = comando.Usuario,
                                    Fecha = DateTime.Now,
                                    Evento = EventoABM.Alta,
                                    Entidad = comando.ToXml()
                                };
                                Repositorio.Agregar(logAbm);
                                Repositorio.GuardarCambios();
                            }
                            catch (Exception e)
                            {
                                Log.Warn(e, "Ocurrio un error al crear el log AMB Crear");
                            }
                        }
                        transaction.Complete();
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error al crear característica de calidad {0}", comando.Dto.Descripcion);
                    resultado.Error("", Textos.Error);
                }
            }

            return resultado;
        }

        private CaracteristicaDeCalidad CrearEntidadCaracteristicaDeCalidadDetalle(CrearCaracteristicaDeCalidad comando)
        {
            var item = Conversor.Convertir<CaracteristicaDeCalidadDto, CaracteristicaDeCalidad>(comando.Dto);
            if (comando.Dto.DescuentosDto != null)
            {
                var descuentos = (from descuento in comando.Dto.DescuentosDto
                                  where !descuento._destroy
                                  select Conversor.Convertir<DescuentoDto, Descuento>(descuento)).ToList();
                foreach (var descuento in descuentos)
                {
                    descuento.CaracteristicaDeCalidad = item;
                }
                item.Descuentos = descuentos;
            }
            item.MaterialPorCentro =
                Repositorio.Obtener<MaterialPorCentro>(
                    x => x.Material.Id == comando.Dto.MaterialId & x.Centro.Id == comando.Dto.CentroId);

            return item;
        }

        private void Validar(CrearCaracteristicaDeCalidad comando, Resultado resultado)
        {
            var cantidad = 0;
            if (comando.Dto.EsHumedad)
            {
                cantidad++;
                if (
                    Repositorio.Existe<CaracteristicaDeCalidad>(
                        e =>
                        e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId &&
                        e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsHumedad))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteHumedad);
                }
            }
            if (comando.Dto.EsCalidadUva)
            {
                cantidad++;
                if (
                    Repositorio.Existe<CaracteristicaDeCalidad>(
                        e =>
                        e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId &&
                        e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsCalidadUva))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteCalidadUva);
                }
            }
            if (comando.Dto.EsEstadoSanitario)
            {
                cantidad++;
                if (
                    Repositorio.Existe<CaracteristicaDeCalidad>(
                        e =>
                        e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId &&
                        e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsEstadoSanitario))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteEstadoSanitario);
                }
            }
            if (comando.Dto.EsTenorAzucarino)
            {
                cantidad++;
                if (
                    Repositorio.Existe<CaracteristicaDeCalidad>(
                        e =>
                        e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsTenorAzucarino))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteTenorAzucarino);
                }
            }
            if (comando.Dto.EsCuerposExtranos)
            {
                cantidad++;
                if (
                    Repositorio.Existe<CaracteristicaDeCalidad>(
                        e =>
                        e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsCuerposExtranos))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteCuerposExtraños);
                }
            }
            if (comando.Dto.EsGranosVerdes)
            {
                cantidad++;
                if (
                    Repositorio.Existe<CaracteristicaDeCalidad>(
                        e =>
                        e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsGranosVerdes))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteGranosVerdes);
                }
            }
            if (comando.Dto.EsGranosDañados)
            {
                cantidad++;
                if (Repositorio.Existe<CaracteristicaDeCalidad>(e => e.Id != comando.Dto.Id && e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsGranosDañados))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteGranosDañados);
                }
            }
            if (cantidad > 1)
            {
                resultado.Error("SoloUno", Textos.Error_SoloUno);
            }

            if (Repositorio.Existe<CaracteristicaDeCalidad>(e => e.CaracteristicaDeCalidadMaestro.Descripcion == comando.Dto.Descripcion && e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Descripcion", Textos.CaracteristicaDeCalidad_ExisteDescripcion);
            }
        }
    }
}