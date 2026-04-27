using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarPDFCpe : ProcesadorComando<ConsultarPDFCpe>
    {
        public ProcesadorConsultarPDFCpe(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ConsultarPDFCpe comando)
        {
            var resultado = new ResultadoConsultarPDFCpe();

            try
            {
                if (comando.NroCtg <= 0)
                {
                    resultado.Errores.Add("2", "El número de CTG es inválido");
                    return resultado;
                }
                var ctg = comando.NroCtg.ToString();
                var recorrido = Repositorio.Listar<Recorrido>(x => x.NumeroDocumentoIngreso == ctg).FirstOrDefault();
                if (recorrido == null)
                {
                    resultado.Errores.Add("1", $"No se encontró recorrido asociado al CTG {comando.NroCtg}");
                    return resultado;
                }

                var documento = Repositorio.ObtenerConsultaEscalar(new ConsultarDocumento(recorrido.InstanciaWorkflow, TipoImpresion.CartaDePorteElectronica, "pdf"));

                if (documento != null)
                {
                    if (documento.Path != null)
                    {
                        resultado.Pdf = System.IO.File.ReadAllBytes(documento.Path);
                    }
                    else
                    {
                        resultado.Errores.Add("3", "Error no se pudo obtener la imagen de la CP desde SCATO");
                    }
                } 
                else
                {
                    resultado.Errores.Add("4", Textos.Error_Generico);
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("5", Textos.Error_Generico);
                Log.Error(e, $"Error al obtener pdf de ctg {comando.NroCtg} en ProcesadorConsultarPDFCpe");
            }

            return resultado;
        }
    }
}
