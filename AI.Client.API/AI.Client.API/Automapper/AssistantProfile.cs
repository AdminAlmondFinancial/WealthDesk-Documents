using AI.Client.API.Assistant;
using AI.Client.API.Assistant.Manager;
using AI.Client.API.Dto;
using AI.Client.API.UserManagerFactory;
using AutoMapper;

namespace AI.Client.API.Automapper;

public sealed class AssistantProfile : Profile
{
    public AssistantProfile()
    {
        CreateMap<CurrentUser, MeDto>();
        CreateMap<CurrentUser, LoginResultDto>()
            .ForCtorParam("AccessAreas", o => o.MapFrom(s => s.Areas));
        CreateMap<ChatGroup, HistoryGroupDto>()
            .ForCtorParam("Items", o => o.MapFrom(s => s.Items.Select(i => new[] { i.Id.ToString(), i.Title }).ToList()));
        CreateMap<IFormattedDocument, OutputFileDto>();
        CreateMap<FormatResult, FormatResultDto>()
            .ForCtorParam("Out", o => o.MapFrom(s => s.Document));
    }
}
