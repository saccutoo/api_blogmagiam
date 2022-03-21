using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using Utils;

namespace Templates.API
{
	public static class ECM
	{
        public static async Task<ResponseECM> UploadFileToECM (IFormFile file, string contentRootPath)
		{
			ResponseECM responseUploadECM = new ResponseECM();

			try
            {
				//// Load the xml file into XmlDocument object.
				//XmlDocument xmlDoc = new XmlDocument();
				//try
				//{
				//	xmlDoc.Load(contentRootPath + "\\Helpers\\ECM\\UploadFile.xml");
				//}
				//catch (XmlException e)
				//{
				//	Console.WriteLine(e.Message);
				//}
				//// Now create StringWriter object to get data from xml document.
				//StringWriter sw = new StringWriter();
				//XmlTextWriter xw = new XmlTextWriter(sw);
				//xmlDoc.WriteTo(xw);

				var upload = @"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:impl='http://impl.shb.vn.com' xmlns:dto='http://dto.shb.vn.com'>
								<soapenv:Body>
									<impl:uploadFile>
										<impl:request>
											<dto:cifNo></dto:cifNo>
											<dto:contractsNumber></dto:contractsNumber>
											<dto:docContent>{{Base64}}</dto:docContent>
											<dto:docName>{{FileName}}</dto:docName>
											<dto:docProcess></dto:docProcess>
											<dto:docType>{{DocType}}</dto:docType>
											<dto:mimeType>{{MineType}}</dto:mimeType>
											<dto:path></dto:path>
											<dto:posCode></dto:posCode>
											<dto:properties>{{Properties}}</dto:properties>
											<dto:userName>{{UserName}}</dto:userName>
										</impl:request>
									</impl:uploadFile>

								</soapenv:Body>
							</soapenv:Envelope>";

				using (var ms = new MemoryStream())
				{
					file.CopyTo(ms);
					var fileBytes = ms.ToArray();
					string base64 = Convert.ToBase64String(fileBytes);
					var client = new RestClient(Helpers.GetConfig("ECM:SOAPConnectionString"));
					client.Timeout = -1;
					var request = new RestRequest(Method.POST);
					request.AddHeader("Content-Type", "text/xml");
					request.AddHeader("SOAPAction", Helpers.GetConfig("ECM:SOAPAction"));

					//XML Soap
					upload = upload.Replace("{{Base64}}", base64);
					upload = upload.Replace("{{FileName}}", file.FileName);
					upload = upload.Replace("{{DocType}}", file.ContentType);
					upload = upload.Replace("{{MineType}}", file.ContentType);
					upload = upload.Replace("{{Properties}}", Helpers.GetConfig("ECM:Properties"));
					upload = upload.Replace("{{UserName}}", Helpers.GetConfig("ECM:UserName"));

					//Mở SSL
					ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
					  X509Certificate certificate,
					  X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
					{ return true; };
					request.AddParameter("text/xml", upload, ParameterType.RequestBody);

					IRestResponse response = client.Execute(request);
					//string str = response.Content.Replace("&quot;", "\"");
					XmlDocument xmlDocupload = new XmlDocument();
					xmlDocupload.LoadXml(response.Content);
					string resupload = xmlDocupload.GetElementsByTagName("properties")[0].InnerText;
					responseUploadECM =  JsonConvert.DeserializeObject<ResponseECM>(resupload);
                    if (responseUploadECM==null)
                    {
						responseUploadECM = new ResponseECM();
						responseUploadECM.StatusCode = StatusCode.Fail;
						if (!string.IsNullOrEmpty(xmlDocupload.GetElementsByTagName("code")[0].InnerText))
						{
							responseUploadECM.Message = "code ecm : " + xmlDocupload.GetElementsByTagName("code")[0].InnerText + " - message ECM : " + xmlDocupload.GetElementsByTagName("message")[0].InnerText;
						}
						else
						{
							responseUploadECM.Message = "Server ECM lỗi";
						}
					}
				}
			}
            catch (Exception ex)
            {
				responseUploadECM.StatusCode = StatusCode.Fail;
				responseUploadECM.Message = ex.Message ;
			}
			return responseUploadECM;
		}

		public static async Task<ResponseECM> DownloadFileFromECM(string docId,string contentRootPath)
        {
			ResponseECM responseECM = new ResponseECM();
			try
            {
				var body = @"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:impl='http://impl.shb.vn.com' xmlns:dto='http://dto.shb.vn.com'>
								<soapenv:Body>
									<impl:downloadFile>
										<impl:dto>
											<dto:cifNo></dto:cifNo>
											<dto:contractsNumber></dto:contractsNumber>
											<dto:docContent></dto:docContent>
											<dto:docName></dto:docName>
											<dto:docProcess></dto:docProcess>
											<dto:docType>{{DocId}}</dto:docType>
											<dto:mimeType></dto:mimeType>
											<dto:path></dto:path>
											<dto:posCode></dto:posCode>
											<dto:properties>{{Properties}}</dto:properties>
											<dto:userName>{{UserName}}</dto:userName>
										</impl:dto>
									</impl:downloadFile>
								</soapenv:Body>
							</soapenv:Envelope>";

				var client = new RestClient(Helpers.GetConfig("ECM:SOAPConnectionString"));
				client.Timeout = -1;
				var request = new RestRequest(Method.POST);
				request.AddHeader("Content-Type", "text/xml");
				request.AddHeader("SOAPAction", Helpers.GetConfig("ECM:SOAPAction"));
				body = body.Replace("{{DocId}}", docId );
				body = body.Replace("{{Properties}}", Helpers.GetConfig("ECM:Properties"));
				body = body.Replace("{{UserName}}", Helpers.GetConfig("ECM:UserName"));
				ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
				  X509Certificate certificate,
				  X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };
				request.AddParameter("text/xml", body, ParameterType.RequestBody);

				IRestResponse response = client.Execute(request);
				XmlDocument xmlDocupload = new XmlDocument();
				xmlDocupload.LoadXml(response.Content);
				string resDownLoad = xmlDocupload.GetElementsByTagName("properties")[0].InnerText;
				responseECM = JsonConvert.DeserializeObject<ResponseECM>(resDownLoad);
				if (responseECM == null)
				{
					responseECM = new ResponseECM();
					responseECM.StatusCode = StatusCode.Fail;
                    if (!string.IsNullOrEmpty(xmlDocupload.GetElementsByTagName("code")[0].InnerText))
                    {
						responseECM.Message = "code ecm : " + xmlDocupload.GetElementsByTagName("code")[0].InnerText  + " - message ECM : " + xmlDocupload.GetElementsByTagName("message")[0].InnerText;
					}
                    else
                    {
						responseECM.Message = "Server ECM lỗi";
					}
					
				}
			}
			catch (Exception ex)
			{
				responseECM.StatusCode = StatusCode.Fail;
				responseECM.Message = ex.Message;
			}
			return responseECM;

		}

		public static async Task<ResponseECM> UploadFileByBase64ToECM(string base64,string fileName,string contentType , string contentRootPath)
		{
			ResponseECM responseUploadECM = new ResponseECM();

			try
			{
				var upload = @"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:impl='http://impl.shb.vn.com' xmlns:dto='http://dto.shb.vn.com'>
								<soapenv:Body>
									<impl:uploadFile>
										<impl:request>
											<dto:cifNo></dto:cifNo>
											<dto:contractsNumber></dto:contractsNumber>
											<dto:docContent>{{Base64}}</dto:docContent>
											<dto:docName>{{FileName}}</dto:docName>
											<dto:docProcess></dto:docProcess>
											<dto:docType>{{DocType}}</dto:docType>
											<dto:mimeType>{{MineType}}</dto:mimeType>
											<dto:path></dto:path>
											<dto:posCode></dto:posCode>
											<dto:properties>{{Properties}}</dto:properties>
											<dto:userName>{{UserName}}</dto:userName>
										</impl:request>
									</impl:uploadFile>
								</soapenv:Body>
							</soapenv:Envelope>";

				var client = new RestClient(Helpers.GetConfig("ECM:SOAPConnectionString"));
				client.Timeout = -1;
				var request = new RestRequest(Method.POST);
				request.AddHeader("Content-Type", "text/xml");
				request.AddHeader("SOAPAction", Helpers.GetConfig("ECM:SOAPAction"));

				//XML Soap
				upload = upload.Replace("{{Base64}}", base64);
				upload = upload.Replace("{{FileName}}", fileName);
				upload = upload.Replace("{{DocType}}", contentType);
				upload = upload.Replace("{{MineType}}", contentType);

				//Mở SSL
				ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
				  X509Certificate certificate,
				  X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
				{ return true; };
				request.AddParameter("text/xml", upload, ParameterType.RequestBody);

				IRestResponse response = client.Execute(request);
				//string str = response.Content.Replace("&quot;", "\"");
				XmlDocument xmlDocupload = new XmlDocument();
				xmlDocupload.LoadXml(response.Content);
				string resupload = xmlDocupload.GetElementsByTagName("properties")[0].InnerText;
				responseUploadECM = JsonConvert.DeserializeObject<ResponseECM>(resupload);
				if (responseUploadECM == null)
				{
					responseUploadECM = new ResponseECM();
					responseUploadECM.StatusCode = StatusCode.Fail;
					if (!string.IsNullOrEmpty(xmlDocupload.GetElementsByTagName("code")[0].InnerText))
					{
						responseUploadECM.Message = "code ecm : " + xmlDocupload.GetElementsByTagName("code")[0].InnerText + " - message ECM : " + xmlDocupload.GetElementsByTagName("message")[0].InnerText;
					}
					else
					{
						responseUploadECM.Message = "Server ECM lỗi";
					}
				}
			}
			catch (Exception ex)
			{
				responseUploadECM.StatusCode = StatusCode.Fail;
				responseUploadECM.Message = ex.Message;
			}
			return responseUploadECM;
		}
	}

}
