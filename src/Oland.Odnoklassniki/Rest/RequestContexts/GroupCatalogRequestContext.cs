using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.Rest.RequestContexts;

public record GroupCatalogsRequestContext : GroupRequestContext
{
    public ICollection<CatalogId> CatalogIds { get; }

    public GroupCatalogsRequestContext(GroupId groupId, CatalogId catalogId) : base(groupId)
    {
        CatalogIds = [catalogId];
    }

    public GroupCatalogsRequestContext(AccessPair accessPair, GroupId groupId, CatalogId catalogId) : base(accessPair, groupId)
    {
        CatalogIds = [catalogId];
    }
    
    public GroupCatalogsRequestContext(GroupId groupId, params CatalogId[] catalogIds) : base(groupId)
    {
        CatalogIds = catalogIds;
    }

    public GroupCatalogsRequestContext(AccessPair accessPair, GroupId groupId, params CatalogId[] catalogIds) : base(accessPair, groupId)
    {
        CatalogIds = catalogIds; 
    }
    
    public override RestParameters Apply(RestParameters parameters)
    {
        parameters.InsertCatalogId(CatalogIds.First().Value); 
        parameters.InsertCatalogIds(CatalogIds.Select(id => id.Value));

        return base.Apply(parameters);
    }
}