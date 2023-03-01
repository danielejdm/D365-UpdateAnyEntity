using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Linq;

namespace UpdateAnyEntity
{
    public class UpdateEntityChildrenActivity : CodeActivity
    {
        [Input(nameof(RecordInfo) + " (RecordUrl | String-Id)")]
        [RequiredArgument]
        public InArgument<string> RecordInfo { get; set; }

        [Input(nameof(RelationshipName))]
        [RequiredArgument]
        public InArgument<string> RelationshipName { get; set; }

        [Input(nameof(UpdateInactive))]
        [RequiredArgument]
        [Default("False")]
        public InArgument<bool> UpdateInactive { get; set; }

        [Input("AttributesValuesString")]
        [RequiredArgument]
        public InArgument<string> AttributesNameValue { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            IWorkflowContext wfContext = activityContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = activityContext.GetExtension<IOrganizationServiceFactory>();
            var orgService = serviceFactory.CreateOrganizationService(wfContext.UserId);
            

            string attributesNameValue = this.AttributesNameValue.Get(activityContext);

            if (string.IsNullOrEmpty(attributesNameValue))
            {
                return;
            }

            string recordInfo = this.RecordInfo.Get(activityContext);
            string relationshipName = this.RelationshipName.Get(activityContext);
            bool updateInactive = this.UpdateInactive.Get(activityContext);

            Guid recordId;
            if(!Guid.TryParse(recordInfo, out recordId)) 
            {
                recordId = Guid.Parse(DynamicsUrlService.GetRecordIdFromUrl(recordInfo));
            }

            RetrieveRelationshipRequest req = new RetrieveRelationshipRequest()
            {
                Name = relationshipName
            };

            // Get child entity type and lookup field name
            RetrieveRelationshipResponse res = (RetrieveRelationshipResponse)orgService.Execute(req);
            OneToManyRelationshipMetadata rel = (OneToManyRelationshipMetadata)res.RelationshipMetadata;
            string childEntityType = rel.ReferencingEntity.ToLower();
            string childEntityFieldName = rel.ReferencingAttribute.ToLower();

            var updateEntityService = new UpdateEntityService(orgService, childEntityType, wfContext.InitiatingUserId);

            // Retrieve all child records
            var queryExpression = new QueryExpression(childEntityType);
            queryExpression.ColumnSet.AddColumn(childEntityType + "id");
            queryExpression.Criteria.AddCondition(childEntityFieldName, ConditionOperator.Equal, recordId);

            if(updateInactive == true)
            {
                queryExpression.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            }

            var retrieved = orgService.RetrieveMultiple(queryExpression).Entities.ToList();

            foreach(var entity in retrieved)
            {
                var attributesNameValuePair = AttributesNameValueExtractor.GetAttributesNamesValuesPair(attributesNameValue);

                var updEntity = new Entity(entity.LogicalName, entity.Id);

                updateEntityService.UpdateEntityAttributes(ref updEntity, attributesNameValuePair);

                orgService.Update(updEntity);
            }
        }
    }
}
