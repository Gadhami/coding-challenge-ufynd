using AutoMapper;

using FluentAssertions;

using Hotels.Domain.Entities;
using Hotels.Domain.Interfaces;
using Hotels.WebAPI.Common.Mappings;
using Hotels.WebAPI.Controllers;
using Hotels.WebAPI.Entities.DTO;
using Hotels.WebAPI.IntegrationTests.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Hotels.WebAPI.IntegrationTests.Controllers;

public class HotelsControllerTests
{
    private HotelsController _controller;
    private IRepository<Hotel> _repository;
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _host;

    public HotelsControllerTests()
    {
        _configuration = new MapperConfiguration(config =>
            config.AddProfile<MappingProfile>());

        _mapper = _configuration.CreateMapper();

        var mockEnvironment = new Mock<IWebHostEnvironment>();
        var expectedPath    = "/path/to/root";
        mockEnvironment.Setup(x => x.ContentRootPath).Returns(expectedPath);
        _host   = mockEnvironment.Object;
    }

    [SetUp]
    public void Setup()
    {
        // Mock the repository
        _repository = new InMemoryRepository<Hotel>();
        _repository.CreateAsync(new Hotel { Id = "1", Name = "Hotel 1" }).Wait();
        _repository.CreateAsync(new Hotel { Id = "2", Name = "Hotel 2" }).Wait();
        _repository.CreateAsync(new Hotel { Id = "3", Name = "Hotel 3" }).Wait();

        _controller = new HotelsController(_repository, _mapper, _host);
    }

    [Test]
    public async Task GetAll_ReturnsAllHotels()
    {
        // Arrange

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new List<Hotel>
            {
                new Hotel { Id = "1", Name = "Hotel 1" },
                new Hotel { Id = "2", Name = "Hotel 2" },
                new Hotel { Id = "3", Name = "Hotel 3" }
            });
    }

    [Test]
    public async Task GetById_WithValidId_ReturnsHotel()
    {
        // Arrange
        var id     = "2";

        // Act
        var result = await _controller.GetById(id);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
              .Which.Value.Should().BeEquivalentTo(new Hotel { Id = "2", Name = "Hotel 2" });
    }

    [Test]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var id     = "99";

        // Act
        var result = await _controller.GetById(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task Create_WithValidHotel_ReturnsCreatedHotel()
    {
        // Arrange
        var newHotel = new HotelDTO { Name = "Hotel 4" };

        // Act
        var result   = await _controller.Create(newHotel);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var outcome  = result as CreatedAtActionResult;
        outcome.Should().NotBeNull();
        (outcome.Value as Hotel).Name.Should().BeEquivalentTo(newHotel.Name);

        //(await _repository.GetByIdAsync(newHotel.Id)).Should().BeEquivalentTo(newHotel);
    }

    [Test]
    public async Task Update_WithValidIdAndHotel_ReturnsNoContent()
    {
        // Arrange
        var id = "2";
        var updatedHotel = new Hotel { Id = "2", Name = "Updated Hotel 2" };

        // Act
        var result = await _controller.Update(id, updatedHotel);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        (await _repository.GetByIdAsync(id)).Should().BeEquivalentTo(updatedHotel);
    }

    [Test]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var mockRepository = new Mock<IRepository<Hotel>>();
        var controller     = new HotelsController(mockRepository.Object, _mapper, _host);
        var invalidId      = "999";
        var hotelToUpdate  = new Hotel
        {
            Id          = invalidId,
            Name        = "Updated hotel",
            ReviewScore = 9.99M
        };

        mockRepository.Setup(x => x.GetByIdAsync(invalidId))
                      .ReturnsAsync(null as Hotel);

        // Act
        var result = await controller.Update(invalidId, hotelToUpdate);

        // Assert
        result.Should()
              .BeOfType<NotFoundResult>();
        mockRepository.Verify(x => x.GetByIdAsync(invalidId), Times.Once);
        mockRepository.Verify(x => x.UpdateAsync(invalidId, hotelToUpdate), Times.Never);
    }

    [Test]
    public async Task Delete_WithValidId_ReturnsSuccess()
    {
        // Arrange
        var additionalHotel = new Hotel
        {
            Id          = "5",
            Name        = "Test Additional Hotel",
            ReviewScore = 4
        };
        await _repository.CreateAsync(additionalHotel);

        // Act
        var result = await _controller.Delete(additionalHotel.Id);

        // Assert
        result.Should()
              .BeOfType<NoContentResult>();

        var hotels = await _repository.GetAllAsync();
        hotels.Should()
              .HaveCount(3);
    }

    [Test]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var additionalHotel = new Hotel
        {
            Id          = "1",
            Name        = "Test Additional Hotel",
            ReviewScore = 4
        };
        await _repository.CreateAsync(additionalHotel);

        // Act
        var result = await _controller.Delete("999");

        // Assert
        result.Should()
              .BeOfType<NotFoundResult>();

        var hotels = await _repository.GetAllAsync();

        hotels.Should()
              .Contain(additionalHotel);
    }
}