using BerAuto.Lib.ManagerServices;
using Microsoft.AspNetCore.Mvc;

namespace BerAuto_API.Controllers
{

    [Route("/api/[controller]")]
    [ApiController]
    public class RentalController : Controller
    {
        RentalManagerService rentalManager;


        public RentalController(API_DbContext dbContext, IDistributedCache cache)
        {
            rentalManager = new RentalManagerService(dbContext, cache);
        }

        [HttpGet]
        public async Task<IActionResult> ListCars()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.Data = await rentalManager.ListRentals();
                return Ok(response);
            }
            catch (Exception e)
            {
                response.StatusCode = 400;
                response.Message = e.Message;
            }

            return BadRequest(response);
        }

    }
}