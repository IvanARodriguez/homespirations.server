using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using NUlid;

namespace Homespirations.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public AppUser()
    {
        Id = Ulid.NewUlid().ToString();
    }

    [JsonIgnore]
    [NotMapped]
    public Ulid UlidId
    {
        get => Ulid.Parse(Id);
        set => Id = value.ToString();
    }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
