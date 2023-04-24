using AutoMapper;

using FluentAssertions;

using Hotels.Domain.Entities;
using Hotels.Domain.Interfaces;
using Hotels.WebAPI.Common.Mappings;
using Hotels.WebAPI.Controllers;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Hotels.WebAPI.IntegrationTests.Controllers;

[TestFixture]
public class RatesControllerTests
{
    private RatesController _controller;
    private Mock<IUnitOfWork> _work;
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public RatesControllerTests()
    {
        _configuration = new MapperConfiguration(config =>
            config.AddProfile<MappingProfile>());

        _mapper = _configuration.CreateMapper();
    }

    [SetUp]
    public void SetUp()
    {
        _work       = new Mock<IUnitOfWork>();
        _controller = CreateRatesController();

        var hotels  = new List<Hotel>
        {
            new Hotel
            {
                Id    = "1",
                Name  = "Test Hotel",
                Rates = new List<HotelRate>
                {
                    new HotelRate { Price = new Price() { Currency = "EUR", NumericFloat = 100 }, TargetDay = DateTime.Today },
                    new HotelRate { Price = new Price() { Currency = "EUR", NumericFloat = 200 }, TargetDay = DateTime.Today.AddDays(1) },
                    new HotelRate { Price = new Price() { Currency = "EUR", NumericFloat = 300 }, TargetDay = DateTime.Today.AddDays(2) }
                }
            }
        };

        var hotelRepository = new Mock<IRepository<Hotel>>();
        hotelRepository.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                       .ReturnsAsync((string id) => hotels.FirstOrDefault(h => h.Id == id));

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(u => u.Hotels)
                  .Returns(hotelRepository.Object);
    }

    private RatesController CreateRatesController()
    {
        return new RatesController(_work.Object, _mapper);
    }

    [Test]
    public async Task GetAllHotelRatesByArrival_ShouldReturnBadRequest_WhenHotelIdIsNullOrWhitespace()
    {
        // Arrange
        var hotelId     = "";
        var arrivalDate = DateTime.Now;

        // Act
        var result = await _controller.GetAllHotelRatesByArrival(hotelId, arrivalDate);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test]
    public async Task GetAllHotelRatesByArrival_ShouldReturnNotFound_WhenHotelIsNotFound()
    {
        // Arrange
        var hotelId     = "non-existent-hotel-id";
        var arrivalDate = DateTime.UtcNow;
        _work.Setup(u => u.Hotels.GetByIdAsync(hotelId)).ReturnsAsync((Hotel)null);

        // Act
        var result = await _controller.GetAllHotelRatesByArrival(hotelId, arrivalDate);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task GetAllHotelRatesByArrival_ShouldReturnOk_WhenRatesAreFound()
    {
        // Arrange
        var hotelId     = "1";
        var arrivalDate = DateTime.UtcNow;
        var hotel       = new Hotel
        {
            Id    = hotelId,
            Name  = "Test Hotel",
            Rates = new List<HotelRate>
            {
                new HotelRate { TargetDay = arrivalDate }
            }
        };
        _work.Setup(u => u.Hotels.GetByIdAsync(hotelId)).ReturnsAsync(hotel);

        // Act
        var result = await _controller.GetAllHotelRatesByArrival(hotelId, arrivalDate);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeOfType<List<HotelRate>>();

        var rates    = (List<HotelRate>)okResult.Value;
        rates.Should().HaveCount(1);
    }

    [Test]
    public async Task GetAllHotelRatesByArrival_ShouldReturnEmptyList_WhenArrivalDateAreNotFound()
    {
        // Arrange
        var hotelId     = "1";
        var arrivalDate = DateTime.UtcNow.AddDays(-1);
        var hotel       = new Hotel
        {
            Id    = hotelId,
            Name  = "Test Hotel",
            Rates = new List<HotelRate> { }
        };
        _work.Setup(u => u.Hotels.GetByIdAsync(hotelId)).ReturnsAsync(hotel);

        // Act
        var result = await _controller.GetAllHotelRatesByArrival(hotelId, arrivalDate);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeOfType<List<HotelRate>>();

        var rates    = (List<HotelRate>)okResult.Value;
        rates.Should().HaveCount(0);
    }
}