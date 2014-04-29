using System.Threading.Tasks;

namespace Angora.Services
{
    public interface IFooCDNService
    {
        void PostToBlob(string blobID, string filename);
        Task PostToBlob(string blobID, byte[] data, string filename);
        void DeleteBlob(string blobID);
        string GetBlobInfo(string blobID);
        void GetBlob(string blobID, string fileName);
        string GetBlobURL(string blobID);
        void PutBlob(string blobID, string destination);
        string CreateNewBlob(string mimeType);
    }
}
