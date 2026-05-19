using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
	[Route("api/files")]
	[ApiController]
	public class FileController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider) : ControllerBase
	{
		[HttpGet("{fileId}")]
		public async Task<ActionResult> GetFile(string fileId)
		{
			// look up the actual file, depending on the fileId
			// demo code
			var pathToFile = "sample-file.pdf";

			// check whether the file exists
			if (!System.IO.File.Exists(pathToFile))
			{
				return NotFound();
			}

			if (!fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType))
			{
				contentType = "application/octet-stream";
			}

			var bytes = await System.IO.File.ReadAllBytesAsync(pathToFile);
			return File(bytes, contentType, Path.GetFileName(pathToFile));
		}

		[HttpPost]
		public async Task<ActionResult> CreateFile(IFormFile file)
		{
			// Validate the input. Put a limit on filesize to avoid large uploads attacks.
			// Only accept .pdf files (check content-type)
			if (file.Length is 0
				|| file.Length > 20 * 1024 * 1024 // 20 MB limit
				|| file.ContentType != "application/pdf"
				) 
			{
				return BadRequest("No file or an invalid one has been inputted.");
			}

			// Create a tile path. Avoid using file.FileName, as an attack can provide a 
			// malicious file name with path traversal characters (e.g. ../../evil-file.pdf) to overwrite important files on the server.
			// including full paths or relative paths.

			var path = Path.Combine(Directory.GetCurrentDirectory(), $"uploaded_file_{Guid.NewGuid()}.pdf");

			await using var stream = new FileStream(path, FileMode.Create);
			await file.CopyToAsync(stream);

			return Ok("Your file has been uploaded successfully.");
		}
	}
}
