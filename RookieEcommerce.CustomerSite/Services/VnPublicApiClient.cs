
using RookieEcommerce.SharedViewModels.VnApiDtos;
using System.Net.Http;

namespace RookieEcommerce.CustomerSite.Services
{
    public class VnPublicApiClient(HttpClient httpClient)
    {
        public async Task<VnApiDto<DistrictDto>?> GetDistrictsAsync(string provinceCode)
        {
            var response = await httpClient.GetAsync($"districts/getAll?limit=-1&q={provinceCode}&cols=parent_code");
            response.EnsureSuccessStatusCode(); // Hoặc xử lý lỗi nếu cần

            var apiResponse = await response.Content.ReadFromJsonAsync<VnApiDto<DistrictDto>>();
            return apiResponse;
        }

        public async Task<VnApiDto<ProvinceDto>?> GetProvincesAsync()
        {
            var response = await httpClient.GetAsync("provinces/getAll?limit=-1");
            response.EnsureSuccessStatusCode(); // Hoặc xử lý lỗi nếu cần

            var apiResponse = await response.Content.ReadFromJsonAsync<VnApiDto<ProvinceDto>>();
            return apiResponse;
        }

        public async Task<VnApiDto<WardDto>?> GetWardsAsync(string districtCode)
        {
            var response = await httpClient.GetAsync($"wards/getAll?limit=-1&q={districtCode}&cols=parent_code");
            response.EnsureSuccessStatusCode(); // Hoặc xử lý lỗi nếu cần

            var apiResponse = await response.Content.ReadFromJsonAsync<VnApiDto<WardDto>>();
            return apiResponse;
        }
    }
}
