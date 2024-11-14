using Moq;
using api.Domain.Models;
using api.Domain.Repositories;
using api.Services;
using Xunit;

namespace Tests.Services
{
    public class CountryServiceTests
    {
        private readonly Mock<ICountryRepository> _mockCountryRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CountryService _service;

        public CountryServiceTests()
        {
            _mockCountryRepository = new Mock<ICountryRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _service = new CountryService(_mockCountryRepository.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task SaveAsync_ShouldSaveCountry_WhenValidData()
        {            
            var country = new Country
            {
                Name = "Brazil",
                ThreeLetterCode = "BRA",
                TwoLetterCode = "BR"
            };

            _mockCountryRepository.Setup(repo => repo.SaveAsync(It.IsAny<Country>())).ReturnsAsync(country);
            _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);
         
            var result = await _service.SaveAsync(country);

            Assert.NotNull(result);
            Assert.Equal("Brazil", result.Name);
            _mockCountryRepository.Verify(repo => repo.SaveAsync(It.IsAny<Country>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }
    }
}
