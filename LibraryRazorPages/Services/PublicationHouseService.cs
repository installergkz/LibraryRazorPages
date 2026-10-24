using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.Models;
using LibraryRazorPages.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryRazorPages.Services
{
    public class PublicationHouseService : IPublicationHouseService
    {
        private readonly IEntityRepository<PublicationHouse> _pubHouses;

        public PublicationHouseService(IEntityRepository<PublicationHouse> repo)
        {
            _pubHouses = repo;
        }

        public async Task<List<SelectListItem>> LoadPublicationHousesAsync(CancellationToken ct = default)
        {
            var pubHouses = await _pubHouses.GetAllAsync();
            return pubHouses.Select(h => new SelectListItem { Value = h.Id.ToString(), Text = h.Name }).ToList();
        }
    }
}
