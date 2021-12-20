using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject;
using Ninject.Extensions.Logging;
using PdfiumViewer;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarCPEDummy : ProcesadorComando<ConsultarCPEDummy>
    {
        const int cLongitudMaximaCupo = 16; //Se establece el limite maximo de caracteres para el campo cupo de la consulta de CTG.
        private readonly CpePortType serviceAfipCPDigital;
        private readonly IAccesoWsCtg accesoWsCtg;
        private IKernel kernel;

        public ProcesadorConsultarCPEDummy(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCPDigital, IAccesoWsCtg accesoWsCtg, IKernel kernel)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.kernel = kernel;
            this.serviceAfipCPDigital = serviceAfipCPDigital;
        }

        public override Resultado Ejecutar(ConsultarCPEDummy comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////

            var resultado = new Resultado();
            var response = serviceAfipCPDigital.dummy(new dummyRequest());
            if (response.respuesta == null)
            {
                resultado.Error("", "Sin Respuesta Afip");
            }
            if (response.respuesta.appserver != "Ok")
            {
                resultado.Error("", "Error Appserver Afip");
            }
            if (response.respuesta.authserver != "Ok")
            {
                resultado.Error("", "Error Authserver Afip");
            }
            if (response.respuesta.dbserver != "Ok")
            {
                resultado.Error("", "Error DbServer Afip");
            }
            return resultado;
        }
    }
}
