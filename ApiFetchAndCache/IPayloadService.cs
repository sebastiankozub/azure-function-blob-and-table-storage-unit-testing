using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchPublicApiFunction
{
    internal interface IPayloadService
    {
        Task<bool> SavePayload(string guid, string blobValue);
        Task<string> GetPayload(string guid);
    }
}
