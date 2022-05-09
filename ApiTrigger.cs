using Microsoft.Xrm.Sdk;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;

namespace ClassLibrary1
{
    public class APIData
    {
        public int name { get; set; }
        public string id { get; set; }
        public bool phone { get; set; }
        public bool mail { get; set; }
    }
    public class ApiTrigger : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)context.InputParameters["Target"];
                    if (entity != null && entity.Id != Guid.Empty)
                    {
                        string firstname = string.Empty;
                        string lastname = string.Empty;
                        string fullname = string.Empty;
                        string telephone = string.Empty;
                        string mailid = string.Empty;
                        string endpointURL = "https://us1-connect-endpoint-azure.scribesoft.com/v1/orgs/1001729/requests/444?accesstoken=99900f66-ccb7-4adc-bf29-85179b4986e3";

                        firstname = (entity.Attributes.Contains("firstname") && entity.GetAttributeValue<string>("firstname") != string.Empty) ? entity.GetAttributeValue<string>("firstname") : "";
                        lastname = (entity.Attributes.Contains("lastname") && entity.GetAttributeValue<string>("lastname") != string.Empty) ? entity.GetAttributeValue<string>("lastname") : "";
                        mailid = (entity.Attributes.Contains("emailaddress1") && entity.GetAttributeValue<string>("emailaddress1") != string.Empty) ? entity.GetAttributeValue<string>("emailaddress1") : "";
                     //   telephone = entity.Attributes.Contains("telephone1") ? entity.GetAttributeValue<int>("telephone1").ToString() : "9999999";
                        fullname = string.Concat(firstname, lastname);
                        var inputJson = new
                        {
                            name = fullname,
                            id = entity.Id.ToString(),
                            phone = telephone,
                            mail = mailid
                        };
                        ASCIIEncoding encoder = new ASCIIEncoding();
                        var serializer = new JavaScriptSerializer();
                        var serializedResult = serializer.Serialize(inputJson);

                        byte[] data = encoder.GetBytes(serializedResult); 

                        HttpWebRequest request = WebRequest.Create(endpointURL) as HttpWebRequest;
                        request.Method = "POST";
                        request.ContentType = "application/json";
                        request.ContentLength = data.Length;
                        request.Expect = "application/json";

                        request.GetRequestStream().Write(data, 0, data.Length);

                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string Serialize<T>(T item)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new DataContractJsonSerializer(typeof(T)).WriteObject(ms, item);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }
    }
}
