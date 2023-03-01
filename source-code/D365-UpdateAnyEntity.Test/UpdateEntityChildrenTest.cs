using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace UpdateAnyEntity.Test
{
    [TestClass]
    public class UpdateEntityChildrenTest
    {
        private readonly XrmRealContext realContext;
        private readonly CrmServiceClient client;

        public UpdateEntityChildrenTest()
        {
            client = new CrmServiceClient(ConfigurationManager.ConnectionStrings[Constants.ConnectionStingNames.XRM].ConnectionString);
            realContext = new XrmRealContext(client);
        }

        [TestMethod]
        public void Should_Update_Account_Contacts_From_Attributes_NameValue_String()
        {
            using (var _context = new XrmServiceContext(client))
            {
                Random randNum = new Random();

                Guid account1Id = this.client.Create(new Account
                {
                    Id = Guid.NewGuid(),
                    Name = "DDD - " + (randNum.Next(1, 999)).ToString().PadLeft(2, '0')
                });

                Guid account2Id = this.client.Create(new Account
                {
                    Id = Guid.NewGuid(),
                    Name = "DDD - " + (randNum.Next(1, 999)).ToString().PadLeft(2, '0')
                });

                var contact1id = this.client.Create(new Contact
                {
                    Id = Guid.NewGuid(),
                    ParentCustomerId = new EntityReference(Account.EntityLogicalName, account1Id),
                    PreferredContactMethodCode = (contact_preferredcontactmethodcode)randNum.Next(1, 5),
                    MobilePhone = "+" + randNum.Next(10, 50) + " " + randNum.Next(10000000, 99999999).ToString(),
                    CreditOnHold = randNum.Next(100) <= 50 ? true : false
                });
                var contact2id = this.client.Create(new Contact
                {
                    Id = Guid.NewGuid(),
                    ParentCustomerId = new EntityReference(Account.EntityLogicalName, account1Id),
                    PreferredContactMethodCode = (contact_preferredcontactmethodcode)randNum.Next(1, 5),
                    MobilePhone = "+" + randNum.Next(10, 50) + " " + randNum.Next(10000000, 99999999).ToString(),
                    CreditOnHold = randNum.Next(100) <= 50 ? true : false
                });
                var contact3id = this.client.Create(new Contact
                {
                    Id = Guid.NewGuid(),
                    ParentCustomerId = new EntityReference(Account.EntityLogicalName, account1Id),
                    PreferredContactMethodCode = (contact_preferredcontactmethodcode)randNum.Next(1, 5),
                    MobilePhone = "+" + randNum.Next(10, 50) + " " + randNum.Next(10000000, 99999999).ToString(),
                    CreditOnHold = randNum.Next(100) <= 50 ? true : false
                });

                var newAccountId = account2Id;
                var newMobilePhone = "+" + randNum.Next(10, 50) + " " + randNum.Next(10000000, 99999999).ToString();
                var newPreferredContactMethod = randNum.Next(1, 5);
                var newCreditOnHold = randNum.Next(100) <= 50 ? true : false;

                string input = $"mobilephone={newMobilePhone}<>preferredcontactmethodcode={newPreferredContactMethod}<>creditonhold={newCreditOnHold}<>parentcustomerid={newAccountId}";

                var inputs = new Dictionary<string, object>() {
                    { "RecordInfo", $"{Constants.ORG_URL}/main.aspx?app=d365default&forceUCI=1&newWindow=true&pagetype=entityrecord&etn=account&id={account1Id}" },
                    { "RelationshipName", "contact_customer_accounts" },
                    { "UpdateInactive", true },
                    { "AttributesNameValue", input }
                };
                realContext.ExecuteCodeActivity<UpdateEntityChildrenActivity>(inputs);

                var contacts = from c in _context.ContactSet
                               where c.ParentCustomerId.Id == account1Id
                               select c;

                var contactsFilteredInput = from c in _context.ContactSet
                                            where c.ParentCustomerId.Id == account1Id &&
                                            c.MobilePhone == newMobilePhone && (int)c.PreferredContactMethodCode.Value == newPreferredContactMethod &&
                                            c.TransactionCurrencyId.Id == newAccountId && c.CreditOnHold == newCreditOnHold
                                            select c;


                Assert.IsTrue(contacts.ToList().Count == contactsFilteredInput.ToList().Count());

                this.client.Delete(Contact.EntityLogicalName, contact1id);
                this.client.Delete(Contact.EntityLogicalName, contact2id);
                this.client.Delete(Contact.EntityLogicalName, contact3id);
                this.client.Delete(Account.EntityLogicalName, account1Id);
                this.client.Delete(Account.EntityLogicalName, account2Id);
            }
        }
    }
}
