using System.Net.Http;
using System.Threading.Tasks;
using Refit;
using Serious.Abbot.Messages;

namespace Serious.Abbot.CommandLine
{
    [Headers("Authorization: Bearer ")]
    public interface IAbbotApi
    {
        [Get("/api/cli/{skill}")]
        Task<ApiResponse<SkillGetResponse>> GetSkillAsync(string skill);
        
        [Get("/api/cli/status")]
        Task<ApiResponse<StatusGetResponse>> GetStatusAsync();

        [Put("/api/cli/{skill}")]
        Task<HttpResponseMessage> PublishSkillAsync(string skill, SkillUpdateRequest updateRequest);
    }
}