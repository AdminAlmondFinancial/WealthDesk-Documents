namespace AI.Client.API.Dto;

public sealed record MeDto(string Name, string Role);

public sealed record LoginResultDto(string Name, string Role, IReadOnlyList<string> AccessAreas);

/// <summary>Items are [id, title] pairs, as the frontend expects.</summary>
public sealed record HistoryGroupDto(string Label, IReadOnlyList<string[]> Items);

public sealed record OutputFileDto(Guid Id, string Name, string Meta);

/// <summary>Warnings are for a person to check. They are never applied automatically.</summary>
public sealed record FormatResultDto(IReadOnlyList<string> Changes, OutputFileDto Out, IReadOnlyList<string> Warnings);

public sealed record ChatRequestDto(string? Text, string? Style, string? Model);

public sealed record ChatResultDto(string Text);
