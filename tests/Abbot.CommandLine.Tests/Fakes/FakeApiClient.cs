using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;
using Serious.Abbot.Messages;
using Serious.IO.CommandLine;

namespace UnitTests.Fakes
{
    public class FakeApiClient : IAbbotApi
    {
        readonly Dictionary<Type, Dictionary<string, IApiResponse>> _skillApiResponses = new()
        {
            {typeof(SkillGetResponse), new Dictionary<string, IApiResponse>()},
            {typeof(SkillRunResponse), new Dictionary<string, IApiResponse>()},
            {typeof(SkillUpdateResponse), new Dictionary<string, IApiResponse>()}
        };

        readonly Dictionary<Type, IApiResponse> _apiResponses = new();


        public void AddResponse<T>(IApiResponse<T> response)
        {
            _apiResponses.Add(typeof(T), response);
        }

        public void AddResponse<T>(T responseBody)
        {
            IApiResponse<T> response = new ApiResponse<T>(
                new HttpResponseMessage(HttpStatusCode.OK),
                responseBody,
                new RefitSettings());
            _apiResponses.Add(typeof(T), response);
        }

        public void AddResponse<T>(string skill, IApiResponse<T> response)
        {
            _skillApiResponses[typeof(T)].Add(skill, response);
        }

        public void AddResponse<T>(string skill, T responseBody)
        {
            IApiResponse<T> response = new ApiResponse<T>(
                new HttpResponseMessage(HttpStatusCode.OK),
                responseBody,
                new RefitSettings());
            AddResponse(skill, response);
        }

        public Task<ApiResponse<SkillGetResponse>> GetSkillAsync(string skill)
        {
            return GetResponse<SkillGetResponse>(skill);
        }

        public Task<ApiResponse<StatusGetResponse>> GetStatusAsync()
        {
            return GetResponse<StatusGetResponse>();
        }

        public Task<ApiResponse<SkillUpdateResponse>> DeploySkillAsync(string skill, SkillUpdateRequest updateRequest)
        {
            return GetResponse<SkillUpdateResponse>(skill);
        }

        public Task<ApiResponse<SkillRunResponse>> RunSkillAsync(string skill, SkillRunRequest runRequest)
        {
            return GetResponse<SkillRunResponse>(skill);
        }

        public Task<ApiResponse<SkillRunResponse>> RunDeployedSkillAsync(string skill, SkillRunRequest runRequest)
        {
            return GetResponse<SkillRunResponse>(skill);
        }

        public Task<ApiResponse<SkillListResponse>> ListSkillsAsync(SkillOrderBy orderBy, OrderDirection direction, bool includeDisable = false)
        {
            return GetResponse<SkillListResponse>();
        }

        public Task<ApiResponse<object?>> GetInsightsExportAsync()
        {
            return Task.FromResult(new ApiResponse<object?>(
                new HttpResponseMessage(HttpStatusCode.NotFound),
                default,
                new RefitSettings()));
        }

        Task<ApiResponse<T>> GetResponse<T>(string skill)
        {
            var responses = _skillApiResponses[typeof(T)];

            var response = responses.TryGetValue(skill, out var found)
                ? found
                : new ApiResponse<T>(new HttpResponseMessage(HttpStatusCode.NotFound), default(T), new RefitSettings());
            return Task.FromResult((ApiResponse<T>)response);
        }

        Task<ApiResponse<T>> GetResponse<T>()
        {
            var response = _apiResponses.TryGetValue(typeof(T), out var found)
                ? found
                : new ApiResponse<T>(new HttpResponseMessage(HttpStatusCode.NotFound), default(T), new RefitSettings());
            return Task.FromResult((ApiResponse<T>)response);
        }
    }
}
