
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdateAnyEntity
{
    public class UpdateEntityService : IUpdateEntityService
    {
        private AttributeMetadata[] attributeMetadatas;
        IOrganizationService organizationService;
        private Entity userSettings;
        Guid userId;

        public UpdateEntityService(IOrganizationService organizationService, string entityName, Guid userId)
        {
            this.userId= userId;
            this.organizationService = organizationService;
            this.userSettings = this.GetUserSettings();
            var metaDataService = new MetaDataService(organizationService);
            this.attributeMetadatas = metaDataService.GetAttributeMetadata(entityName);
        }

        private Entity GetUserSettings()
        {
            ConditionExpression condition = new ConditionExpression();
            condition.AttributeName = "systemuserid";
            condition.Operator = ConditionOperator.Equal;
            condition.Values.Add(this.userId);

            FilterExpression filter = new FilterExpression();
            filter.Conditions.Add(condition);

            QueryExpression query = new QueryExpression("usersettings");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddFilter(filter);

            EntityCollection result = this.organizationService.RetrieveMultiple(query);

            return result.Entities.FirstOrDefault();
        }
        /// <summary>
        /// Update each single attribute on entity from the list of AttributeValues
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        /// <param name="attributesValues">List KeyValuePair with attribute name and attribute value</param>
        public void UpdateEntityAttributes(ref Entity entity, IEnumerable<KeyValuePair<string, string>> attributesValues)
        {
            foreach (var a in attributesValues)
            {
                this.AddAttributeValueOnEntity(ref entity, a.Key, a.Value);
            }
        }

        /// <summary>
        /// Add/Update the attribute with the proper type
        /// </summary>
        /// <param name="entity">Entity to be updatet</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="attributeValue">Attribute value</param>
        public void AddAttributeValueOnEntity(ref Entity entity, string attributeName, string attributeValue)
        {
            if (entity == null || attributeName == null || attributeValue == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(attributeValue))
            {
                entity.Attributes.Add(attributeName, null);
                return;
            }

            var attributeMetadata = this.attributeMetadatas.Where(a => a.LogicalName.Equals(attributeName)).FirstOrDefault();

            if (attributeMetadata == null || attributeMetadata.IsValidForUpdate == false || attributeMetadata.IsPrimaryId == true)
            {
                return;
            }

            var attributeType = attributeMetadata.AttributeType;

            if (attributeType == AttributeTypeCode.Boolean)
            {
                var boolVal = UpdateEntityAttributeValueBuilderService.BuildBool(attributeMetadata, attributeValue);

                entity.Attributes.Add(attributeName, boolVal);
            }
            else if (attributeType == AttributeTypeCode.Picklist)
            {
                var optionsetVal = UpdateEntityAttributeValueBuilderService.BuildOptionSet(attributeMetadata, attributeValue);

                entity.Attributes.Add(attributeName, optionsetVal);
            }
            else if (attributeType == AttributeTypeCode.Decimal)
            {
                var decVal = UpdateEntityAttributeValueBuilderService.BuildDecimal(this.userSettings, attributeValue);
                entity.Attributes.Add(attributeName, decVal);
            }
            else if (attributeType == AttributeTypeCode.Integer)
            {
                var intVal = UpdateEntityAttributeValueBuilderService.BuildInt(this.userSettings, attributeValue);

                entity.Attributes.Add(attributeName, intVal);
            }
            else if (attributeType == AttributeTypeCode.DateTime)
            {
                var dateTimeVal = UpdateEntityAttributeValueBuilderService.BuildDateTime(this.userSettings, attributeMetadata, attributeValue);

                entity.Attributes.Add(attributeName, dateTimeVal);
            }
            else if (attributeType == AttributeTypeCode.Lookup || attributeType == AttributeTypeCode.Customer || attributeType == AttributeTypeCode.Owner)
            {
                var entityRefVal = UpdateEntityAttributeValueBuilderService.BuildEntityReference(attributeMetadata, attributeValue);

                entity.Attributes.Add(attributeName, entityRefVal);
            }
            else if (attributeType == AttributeTypeCode.Money)
            {
                var moneyVal = UpdateEntityAttributeValueBuilderService.BuildMoney(this.userSettings, attributeValue);

                entity.Attributes.Add(attributeName, moneyVal);
            }
            else if (attributeType == AttributeTypeCode.String || attributeType == AttributeTypeCode.Memo)
            {
                entity.Attributes.Add(attributeName, attributeValue);
            }
        }
    }
}
