using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.WebMobile.Controllers;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.ControllersMobile
{
    [TestFixture]
    public class EstadoDePlantaControllerTest
    {
        private IndexController target;
        private Mock<IConfiguracionProvider> configuracionProvider;
        private Mock<IFirmaProvider> firmaProvider;
        private Mock<IServicioRepositorio> servicio;
        private Mock<IServicioComandos> comandos;
        private Mock<IListaDeWorkflows> listaDeWorkflows;

        private NullLogger log;

    }
}
