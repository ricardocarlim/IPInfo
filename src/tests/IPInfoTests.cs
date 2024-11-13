using api.Controllers;
using api.Domain.Models;
using api.Domain.Models.Queries;
using api.Domain.Repositories;
using api.Domain.Services;
using api.Resources;
using api.Services;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tests
{
    public class IPInfoTests
    {
        //private readonly Mock<IMapper> _mockMapper;
        //private readonly Mock<IIPAddressService> _mockIPAddressService;
        //private readonly IPInfoController _controller;

        //private readonly Mock<IIPAddressRepository> _mockRepository;
        //private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        //private readonly Mock<IMemoryCache> _mockCache;
        //private readonly Mock<ILogger<IPAddressService>> _mockLogger;
        //private readonly IPAddressService _service;

        //private readonly Mock<IConnectionMultiplexer> _mockIConnectionMultiplexer;
        //private readonly Mock<IDistributedCache> _mockIDistributedCache;
               

        //public IPInfoTests()
        //{
        //    _mockMapper = new Mock<IMapper>();
        //    _mockIPAddressService = new Mock<IIPAddressService>();
        //    _controller = new IPInfoController(_mockMapper.Object, _mockIPAddressService.Object);

        //    _mockRepository = new Mock<IIPAddressRepository>();
        //    _mockUnitOfWork = new Mock<IUnitOfWork>();
        //    _mockCache = new Mock<IMemoryCache>();
        //    _mockLogger = new Mock<ILogger<IPAddressService>>();

        //    _mockIConnectionMultiplexer = new Mock<IConnectionMultiplexer>(); 
        //    _mockIDistributedCache = new Mock<IDistributedCache>(); 

        //    _service = new IPAddressService(
        //        _mockRepository.Object,
        //        _mockUnitOfWork.Object,
        //        _mockCache.Object,
        //        _mockLogger.Object,
        //        _mockIConnectionMultiplexer.Object,
        //        _mockIDistributedCache.Object

        //    );
        //}

        //[Fact]
        //public async Task Get_ReturnsQueryResultResource_WithStatus200()
        //{
        //    // Arrange
        //    var queryResource = new IPAddressQueryResource();
        //    var ipAddressQuery = new IPAddressQuery("187.183.35.59", 0, 10);
        //    var queryResult = new api.Domain.Models.IPAddress
        //    {
        //        Id = 1,
        //        IP = "187.183.35.59",
        //        CountryId = 1,
        //        CreatedAt = DateTime.UtcNow,
        //        UpdatedAt = DateTime.UtcNow
        //    };

        //    var mappedQueryResultResource = new QueryResultResource<IPAddressResource>
        //    {
        //        TotalItems = 1,
        //        Items = new List<IPAddressResource> { new IPAddressResource() }
        //    };

        //    // Configurações do Mock
        //    _mockMapper.Setup(m => m.Map<IPAddressQuery>(queryResource))
        //               .Returns(ipAddressQuery);

        //    // Mock do método FindByIPAsync
        //    _mockIPAddressService.Setup(s => s.FindByIPAsync(ipAddressQuery))
        //                         .ReturnsAsync(queryResult);

        //    // Mapeamento do resultado
        //    _mockMapper.Setup(m => m.Map<QueryResultResource<IPAddressResource>>(queryResult))
        //               .Returns(mappedQueryResultResource);

        //    // Act
        //    var result = await _controller.Get(queryResource);

        //    // Assert
        //    var okResult = Assert.IsType<QueryResultResource<IPAddressResource>>(result);
        //    Assert.Equal(mappedQueryResultResource, okResult);
        //    _mockMapper.Verify(m => m.Map<IPAddressQuery>(queryResource), Times.Once);
        //    _mockIPAddressService.Verify(s => s.FindByIPAsync(ipAddressQuery), Times.Once);
        //    _mockMapper.Verify(m => m.Map<QueryResultResource<IPAddressResource>>(queryResult), Times.Once);
        //}

        //[Fact]
        //public async Task DeleteAsync_ThrowsNotImplementedException()
        //{
        //    await Assert.ThrowsAsync<NotImplementedException>(() => _service.DeleteAsync(1));
        //}

        //[Fact]
        //public async Task FindByIPAsync_ReturnsIPAddress_FromRepository()
        //{
        //    // Arrange
        //    var expectedIPAddress = new IPAddress { Id = 1, IP = "187.183.35.59" };
        //    _mockRepository.Setup(r => r.FindByIPAsync("187.183.35.59"))
        //                   .ReturnsAsync(expectedIPAddress);

        //    // Act
        //    var result = await _service.FindByIPAsync("187.183.35.59");

        //    // Assert
        //    Assert.Equal(expectedIPAddress, result);
        //    _mockRepository.Verify(r => r.FindByIPAsync("187.183.35.59"), Times.Once);
        //}

        //[Fact]
        //public async Task FindByIPAsync_WithQuery_ReturnsIPAddress_FromRepository()
        //{
        //    // Arrange
        //    var query = new IPAddressQuery("187.183.35.59", 0, 10);
        //    var expectedIPAddress = new IPAddress { Id = 1, IP = "187.183.35.59" };
        //    _mockRepository.Setup(r => r.FindByIPAsync("187.183.35.59"))
        //                   .ReturnsAsync(expectedIPAddress);

        //    // Act
        //    var result = await _service.FindByIPAsync(query);

        //    // Assert
        //    Assert.Equal(expectedIPAddress, result);
        //    _mockRepository.Verify(r => r.FindByIPAsync(query.IP), Times.Once);
        //}

        //[Fact]
        //public async Task FindInCacheByIPAsync_ThrowsNotImplementedException()
        //{
        //    await Assert.ThrowsAsync<NotImplementedException>(() => _service.FindInCacheByIPAsync("187.183.35.59"));
        //}

        //[Fact]
        //public async Task FindInDBByIPAsync_ReturnsIPAddress_FromRepository()
        //{
        //    //// Arrange
        //    //var expectedIPAddress = new IPAddress { Id = 1, IP = "187.183.35.59" };

        //    //// Mock do repositório
        //    //_mockRepository.Setup(r => r.FindByIPAsync("187.183.35.59"))
        //    //               .ReturnsAsync(expectedIPAddress);

        //    //// Mock do IUnitOfWork
        //    //_mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(true);

        //    //// Act
        //    //var result = await _service.FindInDBByIPAsync("187.183.35.59");

        //    //// Assert
        //    //Assert.Equal(expectedIPAddress, result);
        //    //_mockRepository.Verify(r => r.FindByIPAsync("187.183.35.59"), Times.Once);
        //    //_mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);  // Se o método não deve ser chamado aqui
        //}

        //[Fact]
        //public async Task FindInIP2CByIPAsync_TwoThreeLetters()
        //{
        //    // Arrange
        //    //var expectedIPAddress = new IPAddress { IP = "187.183.35.59" };
        //    //_mockRepository.Setup(r => r.FindByIPAsync("187.183.35.59"))
        //    //               .ReturnsAsync(expectedIPAddress);

        //    //expectedIPAddress.Country = new Country();

        //    //expectedIPAddress.Country.TwoLetterCode = "BR";
        //    //expectedIPAddress.Country.ThreeLetterCode = "BRA";

        //    //// Act
        //    //var result = await _service.FindInIP2CByIPAsync("187.183.35.59");

        //    //// Assert
        //    //Assert.Equal(expectedIPAddress.Country.TwoLetterCode, result.Country.TwoLetterCode);
        //    //Assert.Equal(expectedIPAddress.Country.ThreeLetterCode, result.Country.ThreeLetterCode);                       
        //}

        //[Fact]
        //public async Task SaveAsync_ThrowsNotImplementedException()
        //{
        //    var ipAddress = new IPAddress { Id = 1, IP = "187.183.35.59" };
        //    await Assert.ThrowsAsync<NotImplementedException>(() => _service.SaveAsync(ipAddress));
        //}

        //[Fact]
        //public async Task UpdateAsync_ThrowsNotImplementedException()
        //{
        //    var ipAddress = new IPAddress { Id = 1, IP = "187.183.35.59" };
        //    await Assert.ThrowsAsync<NotImplementedException>(() => _service.UpdateAsync(1, ipAddress));
        //}


    }
}
