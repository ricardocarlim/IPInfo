using api.Domain.Models;
using api.Domain.Models.Queries;
using api.Domain.Services;
using api.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [DisableRequestSizeLimit]
    [Route("api/[controller]")]
    [ApiController]    
    public class IPInfoController : ControllerBase
    {        
        private readonly IMapper _mapper;        
        private readonly IIPAddressService _ipAddressService;
        private readonly IReportService _reportService;
        public IPInfoController( IMapper mapper, IIPAddressService ipAddressService, IReportService reportService)
        {                        
            _ipAddressService = ipAddressService;
            _mapper = mapper;
            _reportService = reportService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CountryResource), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] IPAddressQueryResource query)
        {
            try
            {
                var ipAddressQuery = _mapper.Map<IPAddressQuery>(query);
                var queryResult = await _ipAddressService.FindByIPAsync(ipAddressQuery);

                if (queryResult?.Country == null)
                {
                    return NotFound("Country information not found for the specified IP.");
                }
                
                var countryResource = _mapper.Map<Country, CountryResource>(queryResult.Country);
                return Ok(countryResource);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest($"Invalid input: {ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest($"{ex.Message}");
            }
            catch (Exception ex)
            {                
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // Novo método para gerar o relatório de países
        [HttpGet("report")]
        [ProducesResponseType(typeof(IEnumerable<CountryReport>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountryReport()
        {
            try
            {
                var report = await _reportService.GetCountryReportAsync();

                if (report == null || !report.Any())
                {
                    return NotFound("No data found for the country report.");
                }

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the report request.");
            }
        }
    }
}
