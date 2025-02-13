﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clockify.Net.Models.Projects;
using Clockify.Net.Models.TimeEntries;
using Newtonsoft.Json;
using RestSharp;

namespace Clockify.Net
{
    public partial class ClockifyClient
    {
        /// <summary>
        /// Add a new time entry to workspace. If end is not sent in request means that stopwatch mode is active, otherwise time entry is manually added.
        /// </summary>
        public Task<IRestResponse<TimeEntryDtoImpl>> CreateTimeEntryAsync(string workspaceId, TimeEntryRequest timeEntryRequest)
        {
            if (timeEntryRequest == null) { throw new ArgumentNullException(nameof(timeEntryRequest)); }
            if (timeEntryRequest.Start == null) { throw new ArgumentNullException(nameof(timeEntryRequest.Start)); }

            var request = new RestRequest($"workspaces/{workspaceId}/time-entries", Method.POST);
            request.AddJsonBody(timeEntryRequest);
            return _client.ExecutePostAsync<TimeEntryDtoImpl>(request);
        }

        /// <summary>
        /// Add a new time entry to workspace for another user. If end is not sent in request means that stopwatch mode is active, otherwise time entry is manually added.
        /// </summary>
        public Task<IRestResponse<TimeEntryDtoImpl>> CreateTimeEntryForAnotherUserAsync(string workspaceId, string userId, TimeEntryRequest timeEntryRequest)
        {
            if (timeEntryRequest == null) { throw new ArgumentNullException(nameof(timeEntryRequest)); }
            if (timeEntryRequest.Start == null) { throw new ArgumentNullException(nameof(timeEntryRequest.Start)); }
            if (string.IsNullOrEmpty(userId)) { throw new ArgumentNullException(nameof(userId)); }

            var request = new RestRequest($"workspaces/{workspaceId}/user/{userId}/time-entries", Method.POST);
            request.AddJsonBody(timeEntryRequest);
            return _client.ExecutePostAsync<TimeEntryDtoImpl>(request);
        }


        /// <summary>
        /// Get time entry from. workspace. See Clockify docs for query params explanation.
        /// </summary>
        public Task<IRestResponse<TimeEntryDtoImpl>> GetTimeEntryAsync(
            string workspaceId,
            string timeEntryId,
            bool considerDurationFormat = false,
            bool hydrated = false)
        {
            var request = new RestRequest($"workspaces/{workspaceId}/time-entries/{timeEntryId}");
            request.AddQueryParameter("consider-duration-format", considerDurationFormat.ToString());
            request.AddQueryParameter(nameof(hydrated), hydrated.ToString());
            return _client.ExecuteGetAsync<TimeEntryDtoImpl>(request);
        }

        /// <summary>
        /// Update time entry with id.
        /// </summary>
        public Task<IRestResponse<TimeEntryDtoImpl>> UpdateTimeEntryAsync(string workspaceId, string timeEntryId, UpdateTimeEntryRequest updateTimeEntryRequest)
        {
            if (updateTimeEntryRequest == null) { throw new ArgumentNullException(nameof(updateTimeEntryRequest)); }
            if (updateTimeEntryRequest.Start == null) { throw new ArgumentNullException(nameof(updateTimeEntryRequest.Start)); }
            if (updateTimeEntryRequest.Billable == null) { throw new ArgumentNullException(nameof(updateTimeEntryRequest.Billable)); }

            var request = new RestRequest($"workspaces/{workspaceId}/time-entries/{timeEntryId}", Method.PUT);
            request.AddJsonBody(updateTimeEntryRequest);
            return _client.ExecuteAsync<TimeEntryDtoImpl>(request);
        }

        /// <summary>
        /// Delete time entry with id.
        /// </summary>
        public Task<IRestResponse> DeleteTimeEntryAsync(string workspaceId, string timeEntryId)
        {
            var request = new RestRequest($"workspaces/{workspaceId}/time-entries/{timeEntryId}");
            return _client.ExecuteAsync(request, Method.DELETE);
        }

        /// <summary>
        /// Get templates for current user on specified workspace. See Clockify docs for query params explanation.
        /// </summary>
        public Task<IRestResponse<List<TimeEntryDtoImpl>>> FindAllTimeEntriesForUserAsync(
            string workspaceId,
            string userId,
            string description = null,
            DateTimeOffset? start = null,
            DateTimeOffset? end = null,
            string project = null,
            string task = null,
            bool? projectRequired = null,
            bool? taskRequired = null,
            bool? considerDurationFormat = null,
            bool? hydrated = null,
            bool? inProgress = null,
            int page = 1,
            int pageSize = 50,
            List<string> tags = null)
        {
            var request = new RestRequest($"workspaces/{workspaceId}/user/{userId}/time-entries");

            if (description != null) { request.AddQueryParameter(nameof(description), description); }
            if (start != null) { request.AddQueryParameter(nameof(start), start.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")); }
            if (end != null) { request.AddQueryParameter(nameof(end), end.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")); }
            if (project != null) { request.AddQueryParameter(nameof(project), project); }
            if (task != null) { request.AddQueryParameter(nameof(task), task); }
            if (projectRequired != null) { request.AddQueryParameter("project-required", projectRequired.ToString()); }
            if (taskRequired != null) { request.AddQueryParameter("task-required", taskRequired.ToString()); }
            if (considerDurationFormat != null) { request.AddQueryParameter("consider-duration-format", considerDurationFormat.ToString()); }
            if (hydrated != null) { request.AddQueryParameter(nameof(hydrated), hydrated.ToString()); }
            if (inProgress != null) { request.AddQueryParameter("in-progress", inProgress.ToString()); }
            if (tags != null)
            {
                foreach(string tag in tags)
                {
                    request.AddQueryParameter(nameof(tags), tag);
                }
            }

            request.AddQueryParameter(nameof(page), page.ToString());
            request.AddQueryParameter("page-size", pageSize.ToString());

            return _client.ExecuteGetAsync<List<TimeEntryDtoImpl>>(request);
        }

        /// <summary>
        /// Get hydrated time entries for current user on specified workspace. See Clockify docs for query params explanation.
        /// </summary>
        public Task<IRestResponse<List<HydratedTimeEntryDtoImpl>>> FindAllHydratedTimeEntriesForUserAsync(
            string workspaceId,
            string userId,
            string description = null,
            DateTimeOffset? start = null,
            DateTimeOffset? end = null,
            string project = null,
            string task = null,
            bool? projectRequired = null,
            bool? taskRequired = null,
            bool? considerDurationFormat = null,
            bool? inProgress = null,
            int page = 1,
            int pageSize = 50)
        {
            var request = new RestRequest($"workspaces/{workspaceId}/user/{userId}/time-entries");
            const bool hydrated = true;

            if (description != null) { request.AddQueryParameter(nameof(description), description); }
            if (start != null) { request.AddQueryParameter(nameof(start), start.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")); }
            if (end != null) { request.AddQueryParameter(nameof(end), end.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")); }
            if (project != null) { request.AddQueryParameter(nameof(project), project); }
            if (task != null) { request.AddQueryParameter(nameof(task), task); }
            if (projectRequired != null) { request.AddQueryParameter("project-required", projectRequired.ToString()); }
            if (taskRequired != null) { request.AddQueryParameter("task-required", taskRequired.ToString()); }
            if (considerDurationFormat != null) { request.AddQueryParameter("consider-duration-format", considerDurationFormat.ToString()); }
            if (inProgress != null) { request.AddQueryParameter("in-progress", inProgress.ToString()); }

            request.AddQueryParameter(nameof(hydrated), hydrated.ToString());
            request.AddQueryParameter(nameof(page), page.ToString());
            request.AddQueryParameter("page-size", pageSize.ToString());

            return _client.ExecuteGetAsync<List<HydratedTimeEntryDtoImpl>>(request);
        }

        /// <summary>
        /// Get all time entries for a project.
        /// </summary>
        public async Task<IRestResponse<IEnumerable<TimeEntryDtoImpl>>> FindAllTimeEntriesForProjectAsync(
            string workspaceId,
            string projectId,
            string description = null,
            DateTimeOffset? start = null,
            DateTimeOffset? end = null,
            string task = null,
            bool? projectRequired = null,
            bool? taskRequired = null,
            bool? considerDurationFormat = null,
            bool? hydrated = null,
            bool? inProgress = null,
            int page = 1,
            int pageSize = 50)
        {
            // Find project
            var requestProject = new RestRequest($"workspaces/{workspaceId}/projects/{projectId}");
            var projectResponse = await _experimentalClient.ExecuteAsync(requestProject, Method.GET);

            var project = JsonConvert.DeserializeObject<ProjectDtoImpl>(projectResponse.Content);

            // All the time entries in the project
            List<TimeEntryDtoImpl> timeEntriesProject = new List<TimeEntryDtoImpl>();

            foreach (var member in project.Memberships)
            {
                var responseTimeEntriesMember = await FindAllTimeEntriesForUserAsync(workspaceId, member.UserId, description, start, end, project.Id, task, projectRequired, taskRequired, considerDurationFormat, hydrated, inProgress, page, pageSize);

                if (responseTimeEntriesMember.IsSuccessful && responseTimeEntriesMember.Data != null)
                    timeEntriesProject.AddRange(responseTimeEntriesMember.Data);
            }

            IRestResponse<IEnumerable<TimeEntryDtoImpl>> response = new RestResponse<IEnumerable<TimeEntryDtoImpl>>()
            {
                Data = timeEntriesProject,
            };

            return response;
        }
    }
}
