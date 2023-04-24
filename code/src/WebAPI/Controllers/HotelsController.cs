using AutoMapper;

using Hotels.Domain.Entities;
using Hotels.Domain.Interfaces;
using Hotels.WebAPI.Common.Filters;
using Hotels.WebAPI.Entities;
using Hotels.WebAPI.Entities.DTO;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Hotels.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IRepository<Hotel> _repository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _host;

    public HotelsController(IRepository<Hotel> repository, IMapper mapper, IWebHostEnvironment host)
    {
        _repository = repository;
        _mapper     = mapper;
        _host       = host;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var hotels = await _repository.GetAllAsync();
        return Ok(hotels);
    }

    [HttpGet("{hotelId}")]
    public async Task<IActionResult> GetById([FromRoute] string hotelId)
    {
        if (string.IsNullOrWhiteSpace(hotelId))
        {
            return BadRequest();
        }

        var hotel = await _repository.GetByIdAsync(hotelId);
        if (hotel == null)
        {
            return NotFound();
        }

        return Ok(hotel);
    }

    [HttpPost("upload")]
    [ValidateModel]
    public async Task<IActionResult> AddHotels()
    {
        var files = HttpContext.Request.Form?.Files;
        if (files?.Count == 0)
        {
            return BadRequest("No file sent");
        }

        var result = await UploadFiles(files!.ToList());
        if (result != null)
        {
            return result!;
        }

        return Ok();
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Create(HotelDTO hotelDto)
    {
        var hotel = _mapper.Map<Hotel>(hotelDto);
        await _repository.CreateAsync(hotel);
        return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
    }

    [HttpPut("{hotelId}")]
    public async Task<IActionResult> Update(string hotelId, Hotel updatedHotel)
    {
        var existingHotel = await _repository.GetByIdAsync(hotelId);
        if (existingHotel == null)
        {
            return NotFound();
        }

        await _repository.UpdateAsync(hotelId, updatedHotel);

        return NoContent();
    }

    [HttpDelete("{hotelId}")]
    public async Task<IActionResult> Delete(string hotelId)
    {
        var existingHotel = await _repository.GetByIdAsync(hotelId);
        if (existingHotel == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(hotelId);

        return NoContent();
    }

    private async Task<IActionResult?> UploadFiles(List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest("File not selected.");
        }

        foreach (var file in files)
        {
            // Check if the file exists
            if (file == null || file.Length == 0)
            {
                return BadRequest("File not selected.");
            }

            var fileName = await UploadOneFile(file);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest("File upload failed.");
            }

            var hotels = ParseJsonFile<List<HotelDataWrapper>>(fileName);

            foreach(var hotel in hotels)
            {
                var hotelObj   = _mapper.Map<Hotel>(hotel.Hotel);
                var rates      = _mapper.Map<List<HotelRate>>(hotel.HotelRates);
                hotelObj.Rates = rates;

                await _repository.CreateAsync(hotelObj);
            }
        }

        return null;
    }

    private async Task<string> UploadOneFile(IFormFile file)
    {
        // Create a unique filename
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        // Create a directory to save the file
        var uploadPath = Path.Combine(_host.ContentRootPath, "uploads");
        Directory.CreateDirectory(uploadPath);

        // Save the file to the directory
        var filePath = Path.Combine(uploadPath, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return the file path
        return filePath;
    }

    private static T ParseJsonFile<T>(string filePath)
    {
        /* TODO:
         * 1. Parse json file
         * 2. Get hotels + rates info
         * 3. Insert data into DB
         */

        var jsonString = System.IO.File.ReadAllText(filePath);
        T jsonObject   = JsonConvert.DeserializeObject<T>(jsonString);

        return jsonObject;
    }
}