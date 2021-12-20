using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class EnviarMailTest
    {
        private EnviarMail target;
        private WorkflowInvokerTest host;
        private Mock<IFirmaProvider> firmaProviderMock;
        private Mock<IServicioComandos> servComandoMock;

        [SetUp]
        public void SetUp()
        {
            target = new EnviarMail();
            firmaProviderMock = new Mock<IFirmaProvider>();
            servComandoMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(firmaProviderMock.Object);
            host.Extensions.Add(servComandoMock.Object);
            host.InArguments.To = "Camara@gmail.com";
            host.InArguments.From = "Baufest@gmail.com";
            host.InArguments.Subject = "Subject1";
            host.InArguments.CC = "Bf@gmail.com";
            host.InArguments.Body = "Mail de ejemplo enviado";
            host.InArguments.Attachments = new Collection<Attachment>();
            host.InArguments.Tokens = new Dictionary<string, string> {{"firma", "Baufest"}};

        }

        [Test]
        public void EnviarMailFallido()
        {
            firmaProviderMock.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto());
            servComandoMock.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());

            var resultado = host.TestActivity();
            var res = (bool)resultado.First(f => f.Key == "Result").Value;
            var error = (string)resultado.First(f => f.Key == "Error").Value;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(res, Is.False);
            Assert.That(error, Is.Not.Empty);
            firmaProviderMock.Verify(s => s.ObtenerFirmaSinLogo(),Times.Exactly(1));
        }
    }
}
