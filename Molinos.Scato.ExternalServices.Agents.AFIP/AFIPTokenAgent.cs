using AFIPTokenService;
using Molinos.Scato.ExternalServices.Contracts.AFIP.Contracts;
using Molinos.Scato.ExternalServices.Contracts.AFIP.IServices;
using Molinos.Scato.ExternalServices.DTO;
using Molinos.Scato.ExternalServices.IoC;
using Molinos.Scato.ExternalServices.Repository.Interfaces;
using Molinos.Scato.ExternalServices.Repository.Interfaces.Core;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace Molinos.Scato.ExternalServices.Agents.AFIP
{
    public class AFIPTokenAgent : IAFIPTokenService
    {
        private readonly LoginCMSClient _serviceLoginCMS;
        private readonly Lazy<IUnitOfWork> _unitOfWork;

        public AFIPTokenAgent()
        {
            _serviceLoginCMS = new LoginCMSClient();
            _unitOfWork = new Lazy<IUnitOfWork>(() => IoCContainer.Current.Resolve<IUnitOfWork>());
        }

        private IUnitOfWork UnitOfWork => _unitOfWork.Value;
        private ITicketAccesoAfipRepository TicketAccesoAfipRepository => UnitOfWork.Repository<ITicketAccesoAfipRepository>();

        public async Task<ResponseDTO> ObtenerTicketDeAccesoAFIP(TokenRequest request)
        {
            var servicio = "wscpe";
            var xmlLoginTicketRequest = GenerarXmlLoginTicketRequest(servicio);
            var cmsFirmadoBase64 = EncriptarXmlLoginTicketRequest(xmlLoginTicketRequest, "Certificados\\alias.p12");
            var ticketDeAcceso = await _serviceLoginCMS.loginCmsAsync(cmsFirmadoBase64);

            var xmlTicketDeAcceso = new XmlDocument();
            xmlTicketDeAcceso.LoadXml(ticketDeAcceso.loginCmsReturn);
            var nodoHeader = xmlTicketDeAcceso.SelectSingleNode("//header");
            var nodoCredentials = xmlTicketDeAcceso.SelectSingleNode("//credentials");
            var sign = nodoCredentials.SelectSingleNode("//sign") == null ? "" : nodoCredentials.SelectSingleNode("//sign").InnerText;
            var token = nodoCredentials.SelectSingleNode("//token") == null ? "" : nodoCredentials.SelectSingleNode("//token").InnerText;

            return new ResponseDTO();
        }

        private string EncriptarXmlLoginTicketRequest(XmlDocument xmlLoginTicketRequest, string pathCertificado)
        {

            var path = AppDomain.CurrentDomain.BaseDirectory + pathCertificado;
            var certFirmante = new X509Certificate2(path, "", X509KeyStorageFlags.MachineKeySet);

            Encoding encodedMsg = Encoding.UTF8;
            byte[] msgBytes = encodedMsg.GetBytes(xmlLoginTicketRequest.OuterXml);

            var infoContenido = new System.Security.Cryptography.Pkcs.ContentInfo(msgBytes);
            var cmsFirmado = new System.Security.Cryptography.Pkcs.SignedCms(infoContenido);

            var cmsFirmante = new System.Security.Cryptography.Pkcs.CmsSigner(certFirmante);
            cmsFirmante.IncludeOption = X509IncludeOption.EndCertOnly;

            // Firmo el mensaje PKCS #7
            cmsFirmado.ComputeSignature(cmsFirmante);

            // Encodeo el mensaje PKCS #7.
            byte[] encodedSignedCms = cmsFirmado.Encode();

            var cmsFirmadoBase64 = Convert.ToBase64String(encodedSignedCms);
            return cmsFirmadoBase64;
        }

        private static XmlDocument GenerarXmlLoginTicketRequest(string servicio)
        {
            const string xmlStrLoginTicketRequestTemplate =
                "<loginTicketRequest><header><uniqueId></uniqueId><generationTime></generationTime><expirationTime></expirationTime></header><service></service></loginTicketRequest>";

            var xmlLoginTicketRequest = new XmlDocument();
            xmlLoginTicketRequest.LoadXml(xmlStrLoginTicketRequestTemplate);

            var xmlNodoUniqueId = xmlLoginTicketRequest.SelectSingleNode("//uniqueId");
            var xmlNodoGenerationTime = xmlLoginTicketRequest.SelectSingleNode("//generationTime");
            var xmlNodoExpirationTime = xmlLoginTicketRequest.SelectSingleNode("//expirationTime");
            var xmlNodoService = xmlLoginTicketRequest.SelectSingleNode("//service");

            // Las horas son UTC formato yyyy-MM-ddTHH:mm:ssZ
            xmlNodoGenerationTime.InnerText = DateTime.UtcNow.AddMinutes(-10).ToString("s") + "Z";
            xmlNodoExpirationTime.InnerText = DateTime.UtcNow.AddMinutes(+10).ToString("s") + "Z";
            xmlNodoUniqueId.InnerText = Convert.ToString(1);
            xmlNodoService.InnerText = servicio;
            return xmlLoginTicketRequest;
        }
    }
}