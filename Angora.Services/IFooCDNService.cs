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
        void postToBlob(string blobID, string filename);

        [OperationContract]
        void deleteBlob(string blobID);

        [OperationContract]
        string getBlobInfo(string blobID);

        [OperationContract]
        void getBlob(string blobID, string fileName);

        [OperationContract]
        string getBlobURL(string blobID);

        [OperationContract]
        void putBlob(string blobID, string destination);

        [OperationContract]
        string createNewBlob(string mimeType);
    }
}
