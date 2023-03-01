using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace UpdateAnyEntity
{
    public interface IUpdateEntityService
    {
        void AddAttributeValueOnEntity(ref Entity entity, string attributeName, string attributeValue);
    }
}
