using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisLink.Data;
using RedisLink.Models;
using RedisLink.Services;

namespace RedisLink.Controllers;

[ApiController]
[Route("[controller]")]
public class DriversController : ControllerBase
{

    private readonly ILogger<DriversController> _logger;
    private readonly ICacheService _cacheService;
    private readonly AppDbContext _context;

    public DriversController(ILogger<DriversController> logger,
                             ICacheService cacheService,
                             AppDbContext context)
    {
        _logger = logger;
        _cacheService = cacheService;
        _context = context;
    }

    [HttpGet("drivers")]
    public async Task<IActionResult> Get()
    {
        //check cache data
        var cacheData = _cacheService.GetData<IEnumerable<Driver>>("drivers");

        if (cacheData != null && cacheData.Count() > 0)
        {
            Console.WriteLine("Cached data!");
            return Ok(cacheData);
        }
            

        cacheData = await _context.Drivers.ToListAsync();
        
        //Set expiry time
        var expireTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData($"drivers", cacheData, expireTime);

        return Ok(cacheData);
    }

    [HttpPost("drivers/add")]
    public async Task<IActionResult> Post(Driver value)
    {
        var addedObj = await _context.Drivers.AddAsync(value);
        var expireTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData($"driver{value.Id}", addedObj.Entity, expireTime);

        await _context.SaveChangesAsync();
        
        return Ok(addedObj.Entity);
    }
    
    [HttpDelete("drivers/delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var exist = await _context.Drivers.FirstOrDefaultAsync(x => x.Id == id);

        if (exist != null)
        {
            _context.Remove(exist);
            _cacheService.RemoveData($"driver{id}");
            Console.WriteLine($"Removed cached data for {id}!");
            await _context.SaveChangesAsync();

            return NoContent();
        }

        return NotFound();
    }

    
}