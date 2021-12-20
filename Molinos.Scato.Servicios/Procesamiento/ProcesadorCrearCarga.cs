using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCarga : ProcesadorCrear<CrearCarga, Carga>
    {
        public ProcesadorCrearCarga(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Carga CrearEntidad(CrearCarga comando)
        {
            var offsetBalanza = Repositorio.Obtener<BalanzaPuerto>(x => x.CodigoBalanza == comando.Dto.NumeroBalanza).OffSetPlc;
            var fechaInicio = comando.Dto.FechaInicio;
            Carga cargaOpuesta = null;
            if (comando.Dto.CargaOpuesta_Id != 0 && comando.Dto.CargaOpuesta_Id != null)
            {
                cargaOpuesta = Repositorio.Obtener<Carga>(x => x.Id == comando.Dto.CargaOpuesta_Id && x.NumeroBalanza == comando.Dto.NumeroBalanza);
            }
            var carga = new Carga
            {
                Id = comando.Dto.Id + offsetBalanza,
                NumeroBalanza = comando.Dto.NumeroBalanza,
                Tipo = comando.Dto.Tipo,
                Fecha = comando.Dto.Fecha.Value,
                FechaInicio = fechaInicio,
                Vapor = ExisteVapor(comando.Dto.Vapor),
                Bodega = ExisteBodega(comando.Dto.Bodega),
                Destino = ExisteDestino(comando.Dto.Destino),
                Exportador = ExisteExportador(comando.Dto.Exportador),
                Material = ExisteMaterial(comando.Dto.Material),
                PesoProgramado = comando.Dto.PesoProgramado,
                CargaOpuesta = (cargaOpuesta != null) ? cargaOpuesta : null,
                ToneladasAW = comando.Dto.ToneladasAW
            };
            if (cargaOpuesta != null)
            {
                cargaOpuesta.CargaOpuesta = carga;
            }
            return carga;
        }

        protected override void Validar(CrearCarga comando, Resultado resultado)
        {
            var offsetBalanza = Repositorio.Obtener<BalanzaPuerto>(x => x.CodigoBalanza == comando.Dto.NumeroBalanza).OffSetPlc;
            ValidarFechaInicio(comando, resultado);
            ValidarCargaExistente(comando, resultado, offsetBalanza);
            ValidarExisteBalanza(comando, resultado);
            ValidarCargaOpuesta(comando, resultado, offsetBalanza);
            if(ExisteMaterial(comando.Dto.Material) == null)
            {
                resultado.Error("Material", string.Format(Textos.Error_Requerido, "Material"));
            }
        }

        private void ValidarCargaExistente(CrearCarga comando, Resultado resultado, int offSet)
        {
            if (Repositorio.Existe<RegistroBalanzaPuerto>(e => e.Id == comando.Dto.Id + offSet && e.NumeroBalanza == comando.Dto.NumeroBalanza))
            {
                resultado.Error("CargaRepetidaBalanzaPuerto", string.Format(Textos.Error_CrearCarga_IdRepetido));
            }
        }

        private void ValidarCargaOpuesta(CrearCarga comando, Resultado resultado, int offSet)
        {
            if (comando.Dto.CargaOpuesta_Id != 0 && comando.Dto.CargaOpuesta_Id != null)
            {
                if (comando.Dto.Tipo == "fin")
                {
                    if (comando.Dto.Id + offSet <= comando.Dto.CargaOpuesta_Id)
                    {
                        resultado.Error("CrearBalanzaPuerto", string.Format(Textos.Error_CrearCarga_ComplementariaFinMayorInicio));
                    }
                }
                else
                {
                    if (comando.Dto.Id + offSet >= comando.Dto.CargaOpuesta_Id)
                    {
                        resultado.Error("CrearBalanzaPuerto", string.Format(Textos.Error_CrearCarga_ComplementariaInicioMayorFin));
                    }
                }
                if (!Repositorio.Existe<Carga>(e => e.Id == comando.Dto.CargaOpuesta_Id && e.NumeroBalanza == comando.Dto.NumeroBalanza && e.Tipo == (comando.Dto.Tipo == "inicio" ? "fin" : "inicio")))
                {
                    resultado.Error("CrearBalanzaPuerto1", string.Format(Textos.Error_CrearCarga_Complementaria));
                }
                else
                {
                    ValidarDatosIgualesDeInicioFin(comando, resultado);
                }
                ValidarNoExisteCargaEnElmedio(comando.Dto.Id + offSet, comando.Dto.CargaOpuesta_Id.Value, comando.Dto.NumeroBalanza, comando.Dto.Tipo, resultado);
            }
        }
        private void ValidarExisteBalanza(CrearCarga comando, Resultado resultado)
        {
            if (!Repositorio.Existe<BalanzaPuerto>(e => e.CodigoBalanza == comando.Dto.NumeroBalanza))
            {
                resultado.Error("NoExisteBalanzaBalanzaPuerto", string.Format(Textos.Error_CrearCarga_NoExisteBalanza));
            }
        }

        private void ValidarNoExisteCargaEnElmedio(int cargaId, int CargaOpuestaId, string numeroBalanza, string tipo, Resultado resultado)
        {
            if (Repositorio.Existe<Carga>(x => x.NumeroBalanza == numeroBalanza && (tipo == "inicio") ? x.Id > cargaId && x.Id < CargaOpuestaId : x.Id < cargaId && x.Id > CargaOpuestaId))
            {
                resultado.Error("CrearBalanzaPuertoCargaIntermedio", string.Format(Textos.Error_CrearCarga_CargaIntermedio));
            }
        }

        private void ValidarDatosIgualesDeInicioFin(CrearCarga comando, Resultado resultado)
        {
            var opuesta = Repositorio.Obtener<Carga>(x => x.Id == comando.Dto.CargaOpuesta_Id && comando.Dto.NumeroBalanza == x.NumeroBalanza);
            if (opuesta.Bodega.Id != comando.Dto.BodegaId)
            {
                resultado.Error("BodegaDistintoInicioFin", string.Format(Textos.Error_CrearCarga_BodegaDistintaInicioFin) + " (" + opuesta.Bodega.Nombre + ") ");
            }
            if (opuesta.Destino.Id != comando.Dto.DestinoId)
            {
                resultado.Error("DestinoDistintoInicioFin", string.Format(Textos.Error_CrearCarga_DestinoDistintaInicioFin) + " (" + opuesta.Destino.Nombre + ") ");
            }
            if (opuesta.Exportador.Id != comando.Dto.ExportadorId)
            {
                resultado.Error("ExportadorDistintoInicioFin", string.Format(Textos.Error_CrearCarga_ExportadorDistintaInicioFin) + " (" + opuesta.Exportador.Nombre + ") ");
            }
            if (opuesta.Material.Id != comando.Dto.MaterialId)
            {
                resultado.Error("MaterialDistintoInicioFin", string.Format(Textos.Error_CrearCarga_MaterialDistintaInicioFin) + " (" + opuesta.Material.Descripcion + ") ");
            }
            if (opuesta.Vapor.Id != comando.Dto.VaporId)
            {
                resultado.Error("VaporDistintoInicioFin", string.Format(Textos.Error_CrearCarga_VaporDistintaInicioFin) + " (" + opuesta.Vapor.Nombre + ") ");
            }
        }

        private void ValidarFechaInicio(CrearCarga comando, Resultado resultado)
        {
            if (comando.Dto.FechaInicio != null)
            {
                var inicio = Repositorio.Obtener<Carga>(x => x.Id == comando.Dto.CargaOpuesta_Id && x.NumeroBalanza == comando.Dto.NumeroBalanza);
                if (inicio.Fecha >= comando.Dto.Fecha)
                {
                    resultado.Error("FechaInvalida", string.Format(Textos.Error_CrearCarga_FechaInicioMenorAFecha));
                }
                if (comando.Dto.FechaInicio >= comando.Dto.Fecha)
                {
                    resultado.Error("FechaInvalida", string.Format(Textos.Error_CrearCarga_FechaInicioMenorAFecha));
                }

            }
        }

        private Vapor ExisteVapor(string vapor)
        {
            if (!Repositorio.Existe<Vapor>(e => e.Nombre == vapor))
            {
                var nuevoVapor = new Vapor
                {
                    Nombre = vapor
                };
                Repositorio.Agregar(nuevoVapor);
                Repositorio.GuardarCambios();
            }
            return Repositorio.Obtener<Vapor>(e => e.Nombre == vapor);
        }

        private Bodega ExisteBodega(string bodega)
        {
            if (!Repositorio.Existe<Bodega>(e => e.Nombre == bodega))
            {
                var nuevaBodega = new Bodega
                {
                    Nombre = bodega
                };
                Repositorio.Agregar(nuevaBodega);
                Repositorio.GuardarCambios();
            }
            return Repositorio.Obtener<Bodega>(e => e.Nombre == bodega);
        }

        private Destino ExisteDestino(string destino)
        {
            if (!Repositorio.Existe<Destino>(e => e.Nombre == destino))
            {
                var nuevoDestino = new Destino
                {
                    Nombre = destino
                };
                Repositorio.Agregar(nuevoDestino);
                Repositorio.GuardarCambios();
            }
            return Repositorio.Obtener<Destino>(e => e.Nombre == destino);
        }

        private Exportador ExisteExportador(string exportador)
        {
            if (!Repositorio.Existe<Exportador>(e => e.Nombre == exportador))
            {
                var nuevoExportador = new Exportador
                {
                    Nombre = exportador
                };
                Repositorio.Agregar(nuevoExportador);
                Repositorio.GuardarCambios();
            }
            return Repositorio.Obtener<Exportador>(e => e.Nombre == exportador);
        }

        private MaterialPuerto ExisteMaterial(string material)
        {
            return Repositorio.Obtener<MaterialPuerto>(e => e.Descripcion == material);
        }
    }
}
