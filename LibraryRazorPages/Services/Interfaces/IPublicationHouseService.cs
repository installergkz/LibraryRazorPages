using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryRazorPages.Services.Interfaces
{
    public interface IPublicationHouseService
    {
        Task<List<SelectListItem>> LoadPublicationHousesAsync(CancellationToken ct = default);
    }
}
