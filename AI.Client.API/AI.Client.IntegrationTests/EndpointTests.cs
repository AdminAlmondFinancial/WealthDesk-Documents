using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AI.Client.API.Assistant;
using AI.Client.API.Assistant.DataStores;
using AI.Client.API.Dto;

namespace AI.Client.IntegrationTests;

public class EndpointTests
{
    private static MultipartFormDataContent Form(byte[] file, string fileName, string style = "File note")
    {
        var form = new MultipartFormDataContent
        {
            { new ByteArrayContent(file), "file", fileName },
            { new StringContent(style), "style" },
            { new StringContent(""), "instructions" },
        };
        return form;
    }

    private static byte[] SampleDocx() => new DocumentConverter().BuildDocx("Dear client\nYour review");

    public class GetMe : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory ApiFactory;
        public GetMe(ApiFactory factory) => ApiFactory = factory;

        [Fact]
        public async Task NoToken_Get_Returns401() =>
            Assert.Equal(HttpStatusCode.Unauthorized, (await ApiFactory.CreateClient().GetAsync("/api/me")).StatusCode);

        [Fact]
        public async Task SignedIn_Get_ReturnsNameAndRole()
        {
            var me = await ApiFactory.CreateUserClient().GetFromJsonAsync<MeDto>("/api/me");
            Assert.Equal(new MeDto("Sarah Thompson", "Paraplanner"), me);
        }
    }

    public class PostLogin : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory ApiFactory;
        public PostLogin(ApiFactory factory) => ApiFactory = factory;

        [Fact]
        public async Task SignedIn_Post_GrantsAllAccessAreas()
        {
            var response = await ApiFactory.CreateUserClient().PostAsync("/api/auth/login", null);
            var login = await response.Content.ReadFromJsonAsync<LoginResultDto>();
            Assert.Equal(AI.Client.API.AccountUsers.AccessAreas.All, login!.AccessAreas);
        }
    }

    public class GetChats : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory ApiFactory;
        public GetChats(ApiFactory factory) => ApiFactory = factory;

        [Fact]
        public async Task SavedChat_Get_ReturnsItUnderToday()
        {
            await ApiFactory.Chats.AddAsync(new ChatSession("user-chats", "Formatted letter", DateTime.UtcNow), default);
            var groups = await ApiFactory.CreateUserClient("user-chats").GetFromJsonAsync<List<HistoryGroupDto>>("/api/chats");
            var group = Assert.Single(groups!);
            Assert.Equal("Today", group.Label);
            Assert.Equal("Formatted letter", group.Items[0][1]);
        }

        [Fact]
        public async Task NoChats_Get_ReturnsEmptyList()
        {
            var json = await ApiFactory.CreateUserClient("user-empty").GetStringAsync("/api/chats");
            Assert.Equal("[]", json);
        }
    }

    public class PostFormat : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory ApiFactory;
        public PostFormat(ApiFactory factory) => ApiFactory = factory;

        [Fact]
        public async Task ValidDocx_Post_ReturnsOutputWarningsAndDownloadWorks()
        {
            var client = ApiFactory.CreateUserClient("user-format");
            var response = await client.PostAsync("/api/documents/format", Form(SampleDocx(), "Letter.docx"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<FormatResultDto>();
            Assert.Equal("Letter (formatted).docx", body!.Out.Name);
            Assert.Equal(["Check the totals"], body.Warnings);

            var download = await client.GetAsync($"/api/documents/{body.Out.Id}/download");
            Assert.Equal(HttpStatusCode.OK, download.StatusCode);
            Assert.NotEmpty(await download.Content.ReadAsByteArrayAsync());
        }

        [Fact]
        public async Task DocumentOverTwoMegabytes_Post_Returns413WithoutCallingOrchestrator()
        {
            var before = ApiFactory.Orchestrator.Calls;
            var response = await ApiFactory.CreateUserClient().PostAsync(
                "/api/documents/format", Form(new byte[2 * 1024 * 1024 + 1], "Big.docx"));
            Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);
            Assert.Equal(before, ApiFactory.Orchestrator.Calls);
        }

        [Fact]
        public async Task OrchestratorRejects_Post_Returns422()
        {
            var factory = new ApiFactory(); factory.Orchestrator.Throw = new OrchestratorRejectedException();
            var response = await factory.CreateUserClient().PostAsync("/api/documents/format", Form(SampleDocx(), "A.docx"));
            Assert.Equal((HttpStatusCode)422, response.StatusCode);
        }

        [Fact]
        public async Task OrchestratorDown_Post_Returns502()
        {
            var factory = new ApiFactory(); factory.Orchestrator.Throw = new OrchestratorUnavailableException("down");
            var response = await factory.CreateUserClient().PostAsync("/api/documents/format", Form(SampleDocx(), "A.docx"));
            Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
        }

        [Fact]
        public async Task NoToken_Post_Returns401() =>
            Assert.Equal(HttpStatusCode.Unauthorized,
                (await ApiFactory.CreateClient().PostAsync("/api/documents/format", Form(SampleDocx(), "A.docx"))).StatusCode);
    }

    public class GetDownload : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory ApiFactory;
        public GetDownload(ApiFactory factory) => ApiFactory = factory;

        [Fact]
        public async Task OtherUsersDocument_Get_Returns404()
        {
            var owner = ApiFactory.CreateUserClient("owner");
            var created = await (await owner.PostAsync("/api/documents/format", Form(SampleDocx(), "A.docx")))
                .Content.ReadFromJsonAsync<FormatResultDto>();
            var response = await ApiFactory.CreateUserClient("intruder").GetAsync($"/api/documents/{created!.Out.Id}/download");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    public class PostChat : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory ApiFactory;
        public PostChat(ApiFactory factory) => ApiFactory = factory;

        [Fact]
        public async Task ValidMessage_Post_Returns501UntilOrchestratorSupportsChat()
        {
            var response = await ApiFactory.CreateUserClient().PostAsJsonAsync(
                "/api/chat", new ChatRequestDto("hello", "File note", "Phi-4"));
            Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
        }
    }

    public class Auditing : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory ApiFactory;
        public Auditing(ApiFactory factory) => ApiFactory = factory;

        [Fact]
        public async Task AnyRequest_Get_AuditsWhoAndStatusOnly()
        {
            await ApiFactory.CreateUserClient("audited").GetAsync("/api/me");
            Assert.Contains(ApiFactory.Audit.Entries, e => e.UserId == "audited" && e.Action == "GET /api/me" && e.Status == 200);
        }
    }
}
