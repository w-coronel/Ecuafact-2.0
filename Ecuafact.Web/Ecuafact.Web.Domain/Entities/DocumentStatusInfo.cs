namespace Ecuafact.Web.Domain.Entities
{
    public class DocumentStatusInfo
    {
        public StatusError Result { get; set; }

        public string AccessKey { get; set; }

        public string AuthorizationDate { get; set; }

        public string AuthorizationNumber { get; set; }

        public string PDF { get; set; }

        public DocumentStatusEnum? State { get; set; }

        public string StatusMsg { get; set; }

        public string XML { get; set; }

        /*

        {
          "result": {
            "code": "1000",
            "message": "OK"
          },
          "accessKey": "2112201801099288254900120010010000000019846951115",
          "authorizationDate": "-",
          "authorizationNumber": "2112201801099288254900120010010000000019846951115",
          "pdf": null,
          "state": "10",
          "statusMsg": "VALIDACI&amp;#211;N ECUAFACT: Valor Detalle Impuesto0.000000 no coincide con el calculado 0.600000000000 Tarifa: 12.000000 Base Imponible: 5.000000. Tolerancia: 0.01",
          "xml": null
        }
        */



        public class StatusError
        {
            public int Code { get; set; }
            public string Message { get; set; }
        }
    }
}