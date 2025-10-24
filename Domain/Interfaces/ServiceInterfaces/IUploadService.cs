using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IUploadService
    {
        Task<List<string>?> UploadFile(List<IFormFile> files, string folderName);
    }
}
