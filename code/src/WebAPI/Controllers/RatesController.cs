using AutoMapper;

using Hotels.Domain.Entities;
using Hotels.Domain.Interfaces;
using Hotels.Infrastructure.Persistence.Interfaces;
using Hotels.WebAPI.Common.Filters;
using Hotels.WebAPI.Entities.DTO;

using Microsoft.AspNetCore.Mvc;

using MongoDB.Driver;

namespace Hotels.WebAPI.Controllers;

[ApiController]
[Route("hotels/{hotelId}/[controller]")]
public class RatesController : ControllerBase
{
    private readonly IUnitOfWork _work;
    private readonly IMapper _mapper;

    public RatesController(IUnitOfWork work, IMapper mapper)
    {
        _work   = work;
        _mapper = mapper;
    }

    [HttpGet("arrival/{arrivalDate:datetime}")]
    public async Task<IActionResult> GetAllHotelRatesByArrival([FromRoute] string hotelId, [FromRoute] DateTime arrivalDate)
    {
        if (string.IsNullOrWhiteSpace(hotelId))
        {
            return BadRequest("Incorrect Hotel ID!");
        }

        var hotel = await _work.Hotels.GetByIdAsync(hotelId);
        if (hotel == null)
        {
            return NotFound("Hotel not found!");
        }

        var rates = hotel.Rates.Where(rate => rate.TargetDay.Date == arrivalDate.Date)
                               .ToList();

        return Ok(rates);
    }

    [HttpGet("/[controller]/arrival/{arrivalDate:datetime}")]
    public async Task<IActionResult> GetAllRatesByArrival([FromRoute] DateTime arrivalDate)
    {
        var startOfDay = arrivalDate.Date;
        var endOfDay   = startOfDay.AddDays(1);

        var filter = Builders<HotelRate>.Filter.Gte(rate => rate.TargetDay, startOfDay) &
                     Builders<HotelRate>.Filter.Lt(rate => rate.TargetDay, endOfDay);
        var rates  = await (_work.Rates as IMongoDbRepository<HotelRate>)!.GetAllWithFilterAsync(filter);

        return Ok(rates.ToList());
    }


    [HttpGet("{rateId}")]
    public async Task<IActionResult> GetById([FromRoute] string hotelId, [FromRoute] string rateId)
    {
        if (string.IsNullOrWhiteSpace(rateId) || string.IsNullOrWhiteSpace(hotelId))
        {
            return BadRequest("Incorrect ID");
        }

        var hotel = await _work.Hotels.GetByIdAsync(hotelId);
        if (hotel == null)
        {
            return NotFound();
        }

        var rate = hotel.Rates.FirstOrDefault(b => b.Id == rateId);
        if (rate == null)
        {
            return NotFound();
        }

        return Ok(rate);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Create([FromRoute] string hotelId, HotelRateDTO rate)
    {
        if (string.IsNullOrWhiteSpace(hotelId))
        {
            return BadRequest("Incorrect ID");
        }

        var hotel = await _work.Hotels.GetByIdAsync(hotelId);
        if (hotel == null)
        {
            return NotFound();
        }

        var newRate = _mapper.Map<HotelRate>(rate);

        if (!CanBookRoom(hotel, newRate))
        {
            return BadRequest("Reservation is unavailable during this time");
        }

        hotel.Rates.Add(newRate);
        await _work.Hotels.UpdateAsync(hotelId, hotel);

        // Retrieve the newly created rate from the repository to get its _id property
        var createdRate = hotel.Rates.Last();

        return CreatedAtAction(nameof(GetById), new { hotelId = hotelId, rateId = createdRate.Id }, createdRate);
    }

    [HttpPut("{rateId}")]
    public async Task<IActionResult> Update([FromRoute] string hotelId, [FromRoute] string rateId, HotelRate rate)
    {
        if (string.IsNullOrWhiteSpace(hotelId))
        {
            return BadRequest("Incorrect Hotel ID");
        }

        if (string.IsNullOrWhiteSpace(rateId) || rate.Id != rateId)
        {
            return BadRequest("Incorrect ID");
        }

        var existingHotel = await _work.Hotels.GetByIdAsync(hotelId);
        if (existingHotel == null)
        {
            return NotFound();
        }

        var existingRate = existingHotel.Rates.FirstOrDefault(b => b.Id == rateId);
        if (existingRate == null)
        {
            return NotFound();
        }

        //existingRate.Adults = rate.Adults;
        existingRate = (HotelRate)rate.ShallowCopy();

        await _work.Hotels.UpdateAsync(existingHotel.Id, existingHotel);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] string hotelId, [FromRoute] string rateId)
    {
        if (string.IsNullOrWhiteSpace(hotelId) || string.IsNullOrWhiteSpace(rateId))
        {
            return BadRequest("Incorrect ID");
        }

        var hotel = await _work.Hotels.GetByIdAsync(hotelId);
        if (hotel == null)
        {
            return NotFound();
        }

        var rate = hotel.Rates.FirstOrDefault(b => b.Id == rateId);
        if (rate == null)
        {
            return NotFound();
        }

        hotel.Rates.Remove(rate);
        await _work.Hotels.UpdateAsync(hotel.Id, hotel);

        return NoContent();
    }

    bool CanBookRoom(Hotel hotel, HotelRate rate)
    {
        // TODO: Make sure booking *is* available given the arrival time!
        return true;
    }
}