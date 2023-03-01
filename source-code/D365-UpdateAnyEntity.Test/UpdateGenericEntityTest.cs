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
    public class UpdateGenericEntityTest
    {
        private readonly XrmRealContext realContext;
        private readonly CrmServiceClient client;


        public UpdateGenericEntityTest()
        {
            client = new CrmServiceClient(ConfigurationManager.ConnectionStrings[Constants.ConnectionStingNames.XRM].ConnectionString);
            realContext = new XrmRealContext(client);
        }

        [TestMethod]
        public void Should_Update_Account_Fields_From_Attributes_NameValue_String()
        {
            using (var _context = new XrmServiceContext(client))
            {
                Random randNum = new Random();
                var contact1 = new Contact
                {
                    Id = Guid.NewGuid(),
                    FirstName = "FN-" + randNum.Next(1, 50),
                    LastName = "LN-" + randNum.Next(1, 50)
                };
                var contact1id = this.client.Create(contact1);

                var contact2 = new Contact
                {
                    Id = Guid.NewGuid(),
                    FirstName = "FN-" + randNum.Next(1, 50),
                    LastName = "LN-" + randNum.Next(1, 50)
                };
                var contact2id = this.client.Create(contact2);

                var account = new Account
                {
                    Id = Guid.NewGuid(),
                    Name = "My Company",
                    AccountNumber = "DDD - " + (randNum.Next(1, 999)).ToString().PadLeft(2, '0'),
                    PrimaryContactId = contact1.ToEntityReference(),
                    AccountCategoryCode = account_accountcategorycode.PreferredCustomer,
                    MarketCap = new Money(100000),
                    DoNotEMail = false,
                };
                Guid accountId = this.client.Create(account);

                var newName = "My Company new name";
                var newAccountNumber = "DDD - " + (randNum.Next(1, 999)).ToString().PadLeft(2, '0');
                var newPrimaryContactUrl = $"https://crmtstb.bayernlb.sfinance.net/BayernLBCRMTSTB/main.aspx?pagetype=entityrecord&etn=contact&id={contact2id}";

                string input = $"name=>{newName}<>accountnumber=>{newAccountNumber}<>primarycontactid=>{newPrimaryContactUrl}<>" +
                    $"accountcategorycode=><>blb_kundendb_aenderung_am_date=><>marketcap=>700000<>donotemail=>";

                var inputs = new Dictionary<string, object>() {
                    { "RecordInfo", $"https://crmtstb.bayernlb.sfinance.net/BayernLBCRMTSTB/main.aspx?app=d365default&forceUCI=1&newWindow=true&pagetype=entityrecord&etn=account&id={accountId}" },
                    { "UpdateInactive", true },
                    { "AttributesNameValue", input }
                };
                realContext.ExecuteCodeActivity<UpdateGenericEntityActivity>(inputs);

                var retrievedAcc =  (from a in _context.AccountSet
                                     where a.Id == accountId
                                     select a).First();



                Assert.IsTrue(retrievedAcc.Name == newName);
                Assert.IsTrue(retrievedAcc.AccountNumber == newAccountNumber);
                Assert.IsFalse(retrievedAcc.AccountCategoryCode.HasValue);
                Assert.IsTrue(retrievedAcc.MarketCap.Value == 700000);

                this.client.Delete(Contact.EntityLogicalName, contact1id);
                this.client.Delete(Contact.EntityLogicalName, contact2id);
                this.client.Delete(Account.EntityLogicalName, accountId);
            }
        }
    }
}
