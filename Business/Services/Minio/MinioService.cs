using Core.Configuration;
using Core.Services;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Minio
{
	public class MinioService : IMinioService
	{
		private readonly IMinioClient _minioClient;
		private readonly string _bucketName;

		public MinioService(IOptions<MinioSettings> minioSettings)
		{
			var settings = minioSettings.Value;
			_minioClient = new MinioClient()
				.WithEndpoint(settings.Endpoint)
				.WithCredentials(settings.AccessKey, settings.SecretKey)
				.WithSSL(settings.UseSSL)
				.Build();
			_bucketName = settings.BucketName;
		}

		public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
		{
			try
			{
				// Ensure bucket exists
				bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
				if (!found)
				{
					await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
				}

				// Set public read policy on bucket so images can be viewed publicly in browser img tags
				try
				{
					string policy = $@"{{
						""Version"": ""2012-10-17"",
						""Statement"": [
							{{
								""Effect"": ""Allow"",
								""Principal"": {{""AWS"": [""*""]}},
								""Action"": [""s3:GetObject""],
								""Resource"": [""arn:aws:s3:::{_bucketName}/*""]
							}}
						]
					}}";
					await _minioClient.SetPolicyAsync(new SetPolicyArgs().WithBucket(_bucketName).WithPolicy(policy));
				}
				catch { /* Policy set error ignored if already set */ }

				// Generate unique file name
				string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

				string contentType = "application/octet-stream";
				if (fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg"))
					contentType = "image/jpeg";
				else if (fileName.EndsWith(".png"))
					contentType = "image/png";
				else if (fileName.EndsWith(".gif"))
					contentType = "image/gif";

				if (fileStream.CanSeek)
				{
					fileStream.Position = 0;
				}

				// Upload file
				await _minioClient.PutObjectAsync(
					new PutObjectArgs()
						.WithBucket(_bucketName)
						.WithObject(uniqueFileName)
						.WithStreamData(fileStream)
						.WithObjectSize(fileStream.Length)
						.WithContentType(contentType)
				);

				// Return relative path with bucket
				return $"/{_bucketName}/{uniqueFileName}";
			}
			catch (Exception ex)
			{
				if (ex.ToString().Contains("127.0.0.1") || ex.ToString().Contains("refused") || ex.ToString().Contains("reddetti"))
				{
					throw new ApplicationException("MinIO servisine bağlanılamadı (127.0.0.1:9000). Lütfen MinIO servisinin açık olduğundan emin olun.");
				}
				throw new Exception($"Error uploading file to MinIO: {ex.Message}", ex);
			}
		}

		public async Task DeleteFileAsync(string fileName)
		{
			try
			{
				await _minioClient.RemoveObjectAsync(
					new RemoveObjectArgs()
						.WithBucket(_bucketName)
						.WithObject(fileName)
				);
			}
			catch (Exception ex)
			{
				throw new Exception($"Error deleting file from MinIO: {ex.Message}", ex);
			}
		}

		public async Task<Stream> GetFileAsync(string fileName)
		{
			try
			{
				var memoryStream = new MemoryStream();
				await _minioClient.GetObjectAsync(
					new GetObjectArgs()
						.WithBucket(_bucketName)
						.WithObject(fileName)
						.WithCallbackStream(stream => stream.CopyTo(memoryStream))
				);
				memoryStream.Position = 0;
				return memoryStream;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error getting file from MinIO: {ex.Message}", ex);
			}
		}
	}
}
