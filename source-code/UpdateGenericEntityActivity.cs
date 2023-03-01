using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace UpdateAnyEntity
{
    public class UpdateGenericEntityActivity : CodeActivity
    {
        [Input(nameof(RecordInfo) + " (RecordUrl | String-Id)")]
        [RequiredArgument]
        public InArgument<string> RecordInfo { get; set; }

        [Input(nameof(EntityName))]
        public InArgument<string> EntityName { get; set; }

        [Input(nameof(UpdateInactive))]
        [RequiredArgument]
        [Default("False")]
        public InArgument<bool> UpdateInactive { get; set; }

        [Input(nameof(AttributesNameValue))]
        [RequiredArgument]
        public InArgument<string> AttributesNameValue { get; set; }
        protected override void Execute(CodeActivityContext activityContext)
        {
            IWorkflowContext wfContext = activityContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = activityContext.GetExtension<IOrganizationServiceFactory>();
            var orgService = serviceFactory.CreateOrganizationService(wfContext.UserId);

            string attributesNameValue = this.AttributesNameValue.Get(activityContext);
            string entityName = this.EntityName.Get(activityContext);
            string entityPrimaryIdField;

            if (string.IsNullOrEmpty(attributesNameValue))
            {
                return;
            }

            bool updateInactive = this.UpdateInactive.Get(activityContext);
            string recordInfo = this.RecordInfo.Get(activityContext);

            Guid recordId;
            if (!Guid.TryParse(recordInfo, out recordId))
            {
                recordId = Guid.Parse(DynamicsUrlService.GetRecordIdFromUrl(recordInfo));
                entityName = DynamicsUrlService.GetEntityNameFromUrl(recordInfo, orgService);
            } else if (entityName == null)
            {
                throw new InvalidPluginExecutionException($"Parameter {nameof(EntityName)} cannot be null if the parameter {nameof(RecordInfo)} is not a valid RecordUrl");
            }

            entityPrimaryIdField = entityName + "id";

            var record = orgService.Retrieve(entityName, recordId, new ColumnSet(new string[] { entityPrimaryIdField, "statecode", "statuscode" }));

            if(!updateInactive && record.GetAttributeValue<bool>("statecode") == true)
            {
                return;
            }

            var updateEntityService = new UpdateEntityService(orgService, entityName, wfContext.InitiatingUserId);

            var attributesNameValuePair = AttributesNameValueExtractor.GetAttributesNamesValuesPair(attributesNameValue);

            var updEntity = new Entity(record.LogicalName, record.Id);
            updateEntityService.UpdateEntityAttributes(ref updEntity, attributesNameValuePair);

            orgService.Update(updEntity);
        }
    }
}
