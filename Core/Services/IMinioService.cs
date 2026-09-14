using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
	public interface IMinioService
	{
		Task<string> UploadFileAsync(Stream fileStream, string fileName);
		Task DeleteFileAsync(string fileName);
		Task<Stream> GetFileAsync(string fileName);
	}
}
