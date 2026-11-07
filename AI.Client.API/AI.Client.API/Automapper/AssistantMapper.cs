using AI.Client.API.Assistant;
using AI.Client.API.Assistant.Manager;
using AI.Client.API.Dto;
using AI.Client.API.UserManagerFactory;
using Riok.Mapperly.Abstractions;

namespace AI.Client.API.Automapper;

/// <summary>Mapping code is generated at build time by Mapperly, so there is no licence key.</summary>
[Mapper]
public sealed partial class AssistantMapper
{
    [MapperIgnoreSource(nameof(CurrentUser.Id))]
    [MapperIgnoreSource(nameof(CurrentUser.Areas))]
    public partial MeDto ToMeDto(CurrentUser user);

    [MapperIgnoreSource(nameof(IFormattedDocument.UserId))]
    [MapperIgnoreSource(nameof(IFormattedDocument.Content))]
    [MapperIgnoreSource(nameof(IFormattedDocument.CreatedUtc))]
    public partial OutputFileDto ToOutputFileDto(IFormattedDocument document);

    [MapProperty(nameof(FormatResult.Document), nameof(FormatResultDto.Out))]
    public partial FormatResultDto ToFormatResultDto(FormatResult result);

    // Items are [id, title] pairs, as the frontend expects.
    public HistoryGroupDto ToHistoryGroupDto(ChatGroup group) =>
        new(group.Label, group.Items.Select(i => new[] { i.Id.ToString(), i.Title }).ToList());
}
