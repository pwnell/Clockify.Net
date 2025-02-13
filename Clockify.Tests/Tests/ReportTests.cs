using System;
using System.Threading.Tasks;
using Clockify.Net;
using Clockify.Net.Models.Clients;
using Clockify.Net.Models.Projects;
using Clockify.Net.Models.Reports;
using Clockify.Net.Models.TimeEntries;
using Clockify.Tests.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TimeZoneConverter;

namespace Clockify.Tests.Tests
{
	public class ReportTests
    {
        private readonly ClockifyClient _client;
        private string _workspaceId;

        public ReportTests()
        {
            _client = new ClockifyClient();
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _workspaceId = await SetupHelper.CreateOrFindWorkspaceAsync(_client, "Clockify.NetTestWorkspace");
        }

        [Test]
        public async Task GetDetailedReportAsync_ShouldReturnDetailedReportDto()
        {
            const int hourlyRateAmount = 1234;

            var guid = Guid.NewGuid().ToString();
            var now = DateTimeOffset.UtcNow;

            var clientRequest = new ClientRequest
            {
                Name = "GetDetailedReportAsync " + guid
            };

            var createClientResponse = await _client.CreateClientAsync(_workspaceId, clientRequest);
            createClientResponse.IsSuccessful.Should().BeTrue();
            createClientResponse.Data.Should().NotBeNull();
            createClientResponse.Data.Name.Should().Equals(clientRequest.Name);

            ClientDto client = createClientResponse.Data;

            var projectRequest = new ProjectRequest
            {
                Name = "GetDetailedReportAsync " + guid,
                Color = "#FF00FF",
                HourlyRate = new Net.Models.HourlyRates.HourlyRateRequest { Amount = hourlyRateAmount },
                ClientId = createClientResponse.Data.Id
            };

            var createProject = await _client.CreateProjectAsync(_workspaceId, projectRequest);
            createProject.IsSuccessful.Should().BeTrue();
            createProject.Data.Should().NotBeNull();

            ProjectDtoImpl project = createProject.Data;

            var timeEntryRequest = new TimeEntryRequest
            {
                Start = now,
                End = now.AddSeconds(30),
                ProjectId = project.Id
            };

            var createResult = await _client.CreateTimeEntryAsync(_workspaceId, timeEntryRequest);
            createResult.IsSuccessful.Should().BeTrue();

            var userResponse = await _client.GetCurrentUserAsync();
            userResponse.IsSuccessful.Should().BeTrue();

            // First, obtain the OS version of time zone based on the Clockify users' settings.
            var tzi = TZConvert.GetTimeZoneInfo(userResponse.Data.Settings.TimeZone);

            // Second, translate current time into the Clockify users' time zone.
            var nowTz = now.ToOffset(tzi.BaseUtcOffset).DateTime;

            // Third, just to be safe we need to translate again to make sure Daylight Savings time is accounted for.
            var nowTz2 = now.ToOffset(tzi.GetUtcOffset(nowTz)).DateTime;

            var detailedReportRequest = new DetailedReportRequest
            {
                ExportType = ExportType.JSON,
                DateRangeStart = nowTz2.AddMinutes(-2),
                DateRangeEnd = nowTz2.AddMinutes(2),
                SortOrder = SortOrderType.DESCENDING,
                Description = String.Empty,
                Rounding = false,
                WithoutDescription = false,
                AmountShown = AmountShownType.EARNED,
                Clients = new ClientsFilterDto
                {
                    Contains = ContainsType.CONTAINS,
                    Ids = new System.Collections.Generic.List<string> { client.Id },
                    Status = StatusType.ACTIVE
                },
                DetailedFilter = new DetailedFilterDto
                {
                    SortColumn = SortColumnType.DATE,
                    Page = 1,
                    PageSize = 50
                },
                TimeZone = userResponse.Data.Settings.TimeZone
            };

            var getDetailedReportResult = await _client.GetDetailedReportAsync(_workspaceId, detailedReportRequest);
            getDetailedReportResult.IsSuccessful.Should().BeTrue();
            getDetailedReportResult.Data.Should().NotBeNull();
            getDetailedReportResult.Data.TimeEntries.Should().HaveCountGreaterOrEqualTo(1);
        }
    }
}