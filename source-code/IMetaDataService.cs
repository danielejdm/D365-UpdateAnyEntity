using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;

namespace UpdateAnyEntity
{
    public interface IMetaDataService
    {
        string GetEntityLogicalName(int entityTypeCode);
        AttributeMetadata[] GetAttributeMetadata(string entityName);
        IEnumerable<string> GetAttributesBlacklist(string entityName);
    }
}
