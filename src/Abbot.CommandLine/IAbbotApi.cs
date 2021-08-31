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
        Task<ApiResponse<SkillUpdateResponse>> DeploySkillAsync(string skill, SkillUpdateRequest updateRequest);

        [Post("/api/cli/{skill}/run")]
        Task<ApiResponse<SkillRunResponse>> RunSkillAsync(string skill, SkillRunRequest runRequest);

        [Get("/api/cli/list")]
        Task<ApiResponse<SkillListResponse>> ListSkillsAsync(SkillOrderBy orderBy, OrderDirection direction, bool includeDisable = false);
    }
}