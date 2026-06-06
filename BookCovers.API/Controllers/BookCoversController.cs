using Microsoft.AspNetCore.Mvc;

namespace BookCovers.API.Controllers;

[Route("api/bookcovers")]
[ApiController]
public class BookCoversController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookCover(string id,
        bool returnFault = false)
    {
        // if returnFault is true, wait 100ms and
        // return an Internal Server Error
        if (returnFault)
        {
            await Task.Delay(100);
            return new StatusCodeResult(500);
        }

		// simulate latency
		await Task.Delay(5000);

		// generate a "book cover" (byte array) between 5 and 10MB
		//var random = new Random();
		//int fakeCoverBytes = random.Next(5097152, 10485760);
		//byte[] fakeCover = new byte[fakeCoverBytes];
		//random.NextBytes(fakeCover);
		var random = new Random();
		byte[] fakeCover = new byte[5097152];
		random.NextBytes(fakeCover);


		return Ok(new
        {
            Id = id,
            Content = fakeCover
        });
    }
}
