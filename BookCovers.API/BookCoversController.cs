﻿using Microsoft.AspNetCore.Mvc;

namespace BookCovers.API
{
    public class BookCoversController : ControllerBase
    {

        public async Task<IActionResult> GetBookCover(
            string name, bool returnFault = false)
        {
            // if returnFault is true, wait 100ms and
            // return an Internal Server Error

            if (returnFault)
            {
                await Task.Delay(100);
                return new StatusCodeResult(500);
            }

            // generate a "book cover" (byte array)  between 2 and 10MB
            var random = new Random();
            var fakeCoverBytes = random.Next(2097152, 10385760);
            var fakeCover = new byte[fakeCoverBytes];
            random.NextBytes(fakeCover);

            return Ok(new
            {
                Name = name,
                Content = fakeCover
            });
        }

    }
}
