using HtmlAgilityPack;
using log4net;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Molinos.Scato.ModuloImpresor.Zebra
{
    public class ZebraPrinter
    {
        public static Image ObtenerImagen(string zpl, string printerIpAddress, ILogger log)
        {
            var imageName = PostZplAndReturnImageName(zpl, printerIpAddress, log);

            // Get the image from the printer
            return LoadImageFromPrinter(imageName, printerIpAddress, log);
        }
        
        private static string PostZplAndReturnImageName(string zpl, string printerIpAddress, ILogger log)
        {
            var encabezado = zpl.IndexOf("^XZ\r\n^XA");
            log.Debug($"Imagen Generada en Zebra sin cortar : {zpl}");
            if (encabezado > -1)
            {
                zpl = zpl.Remove(0, encabezado + 5);
            }
            log.Debug($"Imagen Generada en Zebra cortada : {zpl}");


            string response = null;
            // Setup the post parameters.
            string parameters = "data=" + zpl;
            parameters = parameters + "&" + "dev=R";
            parameters = parameters + "&" + "oname=UNKNOWN";
            parameters = parameters + "&" + "otype=ZPL";
            parameters = parameters + "&" + "prev=Preview Label";
            parameters = parameters + "&" + "pw=";

            // Post to the printer
            response = HttpPost("http://" + printerIpAddress + "/zpl", parameters);

            // Parse the response to get the image name.  This image name is stored for one retrieval only.
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);
            var imageNameXPath = "/html[1]/body[1]/div[1]/img[1]/@alt[1]";
            var imageAttributeValue = doc.DocumentNode.SelectSingleNode(imageNameXPath).GetAttributeValue("alt", "");
            // Take off the R: from the front and the .PNG from the back.
            var imageName = imageAttributeValue.Substring(2);
            log.Debug($"Imagen Generada en Zebra : {imageName} 1");
            imageName = imageName.Substring(0, imageName.Length - 4);
            log.Debug($"Imagen Generada en Zebra : {imageName} 2");
            // Return the image name.
            return imageName;
        }
        
        private static string HttpPost(string URI, string Parameters)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            req.Proxy = new System.Net.WebProxy();

            //Add these, as we're doing a POST
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";

            //We need to count how many bytes we're sending. 
            //Post'ed Faked Forms should be name=value&
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Parameters);
            req.ContentLength = bytes.Length;

            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();

            System.Net.WebResponse resp = req.GetResponse();

            if (resp == null) return null;
            System.IO.StreamReader sr =
                  new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        private static Image LoadImageFromPrinter(string imageName, string printerIpAddress, ILogger log)
        {
            string url = "http://" + printerIpAddress + "/png?prev=Y&dev=R&oname=" + imageName + "&otype=PNG";
            log.Debug($"Intentamos descargar la imagen de Zebra : {url}");

            var response = Http.Get(url);

            log.Debug($"Ok descargar la imagen de Zebra : {url}");

            Image retorno = null;

            using (var ms = new MemoryStream(response))
            {
                retorno = Image.FromStream(ms);
            }

            return retorno;
        }

        private static class Http
        {
            public static byte[] Get(string uri)
            {
                byte[] response = null;
                using (WebClient client = new WebClient())
                {
                    response = client.DownloadData(uri);
                }
                return response;
            }
        }
    }
}