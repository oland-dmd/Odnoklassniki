using Microsoft.Extensions.Options;
using Oland.Odnoklassniki.Rest.ApiClientCore;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.Rest.RequestContexts;

public record MainGroupRequestContext : MainAccountRequestContext
{
    public GroupId GroupId { get; } 
    
    public MainGroupRequestContext(IOptions<ApplicationOptions> Options) : base(Options)
    {
        GroupId = new GroupId(Options.Value.GroupId);
    }
}