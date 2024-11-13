using api.Domain.Models;
using api.Domain.Models.Queries;
using api.Domain.Services.Communication;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace api.Domain.Repositories
{
    public interface ICountryRepository
    {
        Task<Country> ListByNameAsync(string name);
        Task<Country> SaveAsync(Country country);
    }
}
