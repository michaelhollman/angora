using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Angora.Services
{
    [ServiceContract]
    public interface IFooCDNService
    {
        [OperationContract]
        void PostToBlob(string blobID, string filename);

        [OperationContract]
        void DeleteBlob(string blobID);

        [OperationContract]
        string GetBlobInfo(string blobID);

        [OperationContract]
        void GetBlob(string blobID, string fileName);

        [OperationContract]
        string GetBlobURL(string blobID);

        [OperationContract]
        void PutBlob(string blobID, string destination);

        [OperationContract]
        string CreateNewBlob(string mimeType);
    }
}
