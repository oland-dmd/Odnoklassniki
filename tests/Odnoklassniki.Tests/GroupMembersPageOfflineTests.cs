using System.Text.Json;
using Microsoft.Extensions.Options;
using NSubstitute;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClientCore;
using Oland.Odnoklassniki.Rest.ApiClients.Groups;
using Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;
using Oland.Odnoklassniki.Rest.RequestContexts;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Odnoklassniki.Tests;

public sealed class GroupMembersPageOfflineTests
{
    [Fact]
    public async Task NullProtocolBodyIsNotVerifiedEmptyRoster()
    {
        var api = new GroupsApiClient(Substitute.For<IOkApiClientCore>(), new MainAccountRequestContext(Options.Create(new ApplicationOptions
            { ApplicationKey = "offline", AccessToken = "offline", SessionSecretKey = "offline", GroupId = "g" })));
        var context = new GroupRequestContext(new AccessPair(), new GroupId("g"));
        var page = await api.GetMembersPageAsync(context, new AnchorConfiguration());
        Assert.False(page.IsValid); Assert.Null(page.Members); Assert.Null(page.HasMore); Assert.Null(page.TotalCount);
        // Legacy navigator contract remains compatible and still terminates rather than emitting a fake populated page.
        var old = api.GetMembersNavigator(context, new AnchorConfiguration());
        Assert.False(await old.MoveNextAsync());
    }
    [Fact]
    public void HiddenMembershipFlagIsRetainedIndependentlyOfVisibleCount()
    {
        var wire = typeof(GroupsApiClient).Assembly.GetType("Oland.Odnoklassniki.Rest.ApiClients.Groups.Responses.GroupMembersResponse")!;
        var value = JsonSerializer.Deserialize("{\"members\":[],\"totalCount\":0,\"has_more\":false,\"membersHidden\":true}", wire)!;
        Assert.Equal(true, wire.GetProperty("MembersHidden")!.GetValue(value));
        var page = new GroupMembersPage([], null, false, 0) { MembersHidden = true };
        Assert.True(page.IsValid); Assert.True(page.MembersHidden);
    }
    [Theory]
    [InlineData("{}", false)]
    [InlineData("{\"members\":[],\"totalCount\":0}", false)]
    [InlineData("{\"members\":[],\"has_more\":false}", false)]
    [InlineData("{\"members\":[],\"totalCount\":0,\"has_more\":false}", true)]
    public void MissingWireMetadataRemainsUnknown(string json, bool valid)
    {
        var wire = typeof(GroupsApiClient).Assembly.GetType("Oland.Odnoklassniki.Rest.ApiClients.Groups.Responses.GroupMembersResponse")!;
        var value = JsonSerializer.Deserialize(json, wire)!;
        var members = (ICollection<GroupMemberDto>?)wire.GetProperty("Members")!.GetValue(value);
        var page = new GroupMembersPage(members?.ToList(), null, (bool?)wire.GetProperty("HasMore")!.GetValue(value), (int?)wire.GetProperty("TotalCount")!.GetValue(value));
        Assert.Equal(valid, page.IsValid);
    }
}
