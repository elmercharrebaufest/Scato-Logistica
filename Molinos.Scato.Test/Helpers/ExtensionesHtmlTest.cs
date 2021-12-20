using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Web.Helpers;
using NUnit.Framework;

namespace Molinos.Scato.Test.Helpers
{
    [TestFixture]
    public class ExtensionesHtmlTest
    {
        private HtmlHelper htmlHelp;
        private HtmlHelper<ControlDeBalanzaDto> htmlHelpTipo;

        [SetUp]
        public void SetUp()
        {
            htmlHelp = new HtmlHelper(new ViewContext(), new ViewPage());
            htmlHelpTipo = new HtmlHelper<ControlDeBalanzaDto>(new ViewContext(), new ViewPage());
        }

        [Test]
        public void BotonLink()
        {

            var htmlHelpit = htmlHelp.BotonLink("Boton1", "action1", "controller1", new object());      

            Assert.That(htmlHelpit, Is.Not.Null);
        }

        [Test]
        public void CheckBoxLink()
        {

            var htmlHelpit = htmlHelp.CheckBoxLink("Boton1",true, "action1", "controller1", new object());      

            Assert.That(htmlHelpit, Is.Not.Null);
        }
        
        [Test]
        public void BotonLinkDesactivable()
        {

            var htmlHelpit = htmlHelp.BotonLinkDesactivable("Boton1", "action1", "controller1", new object());      

            Assert.That(htmlHelpit, Is.Not.Null);
        }

        [Test]
        public void BotonId()
        {

            var htmlHelpit = htmlHelp.BotonId("Boton1", 1);

            Assert.That(htmlHelpit, Is.Not.Null);
        }

        [Test]
        public void RadioBoton()
        {

            var htmlHelpit = htmlHelp.RadioBoton("label","name","value");

            Assert.That(htmlHelpit, Is.Not.Null);
        }

        [Test]
        public void IconoColor()
        {

            var htmlHelpit = htmlHelp.IconoColor("color");

            Assert.That(htmlHelpit, Is.Not.Null);
        }

        [Test]
        public void Breadcrumb()
        {

            var htmlHelpit = htmlHelp.Breadcrumb("label", "name");

            Assert.That(htmlHelpit, Is.Not.Null);
        }

        [Test]
        public void EnumDisplayTextFor()
        {

            var htmlHelpit = htmlHelpTipo.EnumDisplayTextFor(s => s.TipoPesada);

            Assert.That(htmlHelpit, Is.Not.Null);
        }

        [Test]
        public void DisplayText()
        {
            var control = new ControlDeBalanzaDto();
            var htmlHelpit =  control.TipoPesada.DisplayText();

            Assert.That(htmlHelpit, Is.Not.Null);
        }
        
     
    }
}