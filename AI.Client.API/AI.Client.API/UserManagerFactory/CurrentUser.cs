using AI.Client.API.Validation;

namespace AI.Client.API.UserManagerFactory;

public sealed class CurrentUser
{
    public CurrentUser(string id, string name, string role, IReadOnlyList<string> areas)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ValidationException("User id is required.");
        Id = id;
        Name = string.IsNullOrWhiteSpace(name) ? "Unknown user" : name;
        Role = string.IsNullOrWhiteSpace(role) ? "Paraplanner" : role;
        Areas = areas;
    }

    public string Id { get; }
    public string Name { get; }
    public string Role { get; }
    public IReadOnlyList<string> Areas { get; }

    public void Require(string area)
    {
        if (!Areas.Contains(area)) throw new ForbiddenException();
    }
}
