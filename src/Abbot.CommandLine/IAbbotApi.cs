using System.Threading.Tasks;
using Refit;
using Serious.Abbot.Messages;

namespace Serious.IO.CommandLine
{
    /// <summary>
    /// Client to the Abbot API. Instances of this interface are created using Refit.
    /// </summary>
    [Headers("Authorization: Bearer ")]
    public interface IAbbotApi
    {
        /// <summary>
        /// Retrieve information about a skill hosted on https://ab.bot/
        /// </summary>
        /// <param name="skill">The name of the skill to retrieve.</param>
        [Get("/api/cli/{skill}")]
        Task<ApiResponse<SkillGetResponse>> GetSkillAsync(string skill);

        /// <summary>
        /// Get the status of an Abbot Workspace such as whether it's authenticated, and to which org and user
        /// it's authenticated.
        /// </summary>
        [Get("/api/cli/status")]
        Task<ApiResponse<StatusGetResponse>> GetStatusAsync();

        /// <summary>
        /// Deploys local changes to a skill to https://ab.bot/
        /// </summary>
        /// <param name="skill">The name of the skill to deploy.</param>
        /// <param name="updateRequest">Information about the code to deploy.</param>
        [Put("/api/cli/{skill}")]
        Task<ApiResponse<SkillUpdateResponse>> DeploySkillAsync(string skill, SkillUpdateRequest updateRequest);

        /// <summary>
        /// Runs the local version of a skill in the Abbot Skill Runner.
        /// </summary>
        /// <param name="skill">The name of the skill to run.</param>
        /// <param name="runRequest">Information about the skill to run.</param>
        [Post("/api/cli/{skill}/run")]
        Task<ApiResponse<SkillRunResponse>> RunSkillAsync(string skill, SkillRunRequest runRequest);

        /// <summary>
        /// Runs the deployed version of a skill in the Abbot Skill Runner, regardless of what is stored locally
        /// for that skill.
        /// </summary>
        /// <param name="skill">The name of the skill to run.</param>
        /// <param name="runRequest">Information about the skill to run.</param>
        [Post("/api/cli/{skill}/deployed/run")]
        Task<ApiResponse<SkillRunResponse>> RunDeployedSkillAsync(string skill, SkillRunRequest runRequest);

        /// <summary>
        /// List all the skills on https://ab.bot/ for the organization.
        /// </summary>
        /// <param name="orderBy">How to order the skills.</param>
        /// <param name="direction">Which direction to order the skills.</param>
        /// <param name="includeDisable">Whether to include disabled skills or not.</param>
        [Get("/api/cli/list")]
        Task<ApiResponse<SkillListResponse>> ListSkillsAsync(
            SkillOrderBy orderBy,
            OrderDirection direction,
            bool includeDisable = false);

        /// <summary>
        /// Exports all of the Insights conversation data.
        /// </summary>
        [Get("/api/export/insights")]
        Task<ApiResponse<object>> GetInsightsExportAsync();
    }
}
