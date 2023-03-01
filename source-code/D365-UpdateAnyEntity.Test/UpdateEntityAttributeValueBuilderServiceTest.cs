using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace UpdateAnyEntity.Test
{
    [TestClass]
    public class UpdateEntityAttributeValueBuilderServiceTest
    {
        #region Test
        [TestMethod]
        [DynamicData(nameof(DateTimeTestData))]
        public void Should_Build_Correct_DateTime(Entity userSettings, AttributeMetadata attributeMetadata, string attributeValue, DateTime expectedDateTime)
        {
            var datetimeVal = UpdateEntityAttributeValueBuilderService.BuildDateTime(userSettings, attributeMetadata, attributeValue);
            Assert.AreEqual(expectedDateTime, datetimeVal);
        }

        [TestMethod]
        [DynamicData(nameof(IntTestData))]
        public void Should_Build_Correct_Int(Entity userSettings, string attributeValue, int expectedInt)
        {
            var intVal = UpdateEntityAttributeValueBuilderService.BuildInt(userSettings, attributeValue);
            Assert.AreEqual(expectedInt, intVal);
        }

        [TestMethod]
        [DynamicData(nameof(DecimalTestData))]
        public void Should_Build_Correct_Decimal(Entity userSettings, string attributeValue, decimal expectedDec)
        {
            var decVal = UpdateEntityAttributeValueBuilderService.BuildDecimal(userSettings, attributeValue);
            Assert.AreEqual(expectedDec, decVal);
        }

        [TestMethod]
        [DynamicData(nameof(BoolTestData))]
        public void Should_Build_Correct_Bool(AttributeMetadata attributeMetadata, string attributeValue, bool expectedBool)
        {
            var boolVal = UpdateEntityAttributeValueBuilderService.BuildBool(attributeMetadata, attributeValue);
            Assert.AreEqual(expectedBool, boolVal);
        }

        [TestMethod]
        [DynamicData(nameof(OptionSetTestData))]
        public void Should_Set_Correct_OptionSet(AttributeMetadata attributeMetadata, string attributeValue, OptionSetValue expectedOption)
        {
            var optionVal = UpdateEntityAttributeValueBuilderService.BuildOptionSet(attributeMetadata, attributeValue);
            Assert.AreEqual(expectedOption.Value, optionVal.Value);
        }

        [TestMethod]
        [DynamicData(nameof(EntityReferenceTestData))]
        public void Should_Build_Correct_EntityReference(AttributeMetadata attributeMetadata, string attributeValue, EntityReference expectedEntityReference)
        {
            var entityRefVal = UpdateEntityAttributeValueBuilderService.BuildEntityReference(attributeMetadata, attributeValue);
            Assert.AreEqual(expectedEntityReference.LogicalName, entityRefVal.LogicalName);
            Assert.AreEqual(expectedEntityReference.Id, entityRefVal.Id);
        }

        [TestMethod]
        [DynamicData(nameof(MoneyTestData))]
        public void Should_Build_Correct_Money(Entity userSettings, string attributeValue, Money expectedMoney)
        {
            var moneyVal = UpdateEntityAttributeValueBuilderService.BuildMoney(userSettings, attributeValue);
            Assert.AreEqual(expectedMoney.Value, moneyVal.Value);
        }
        #endregion Test

        #region Test Data
        public static IEnumerable<object[]> DateTimeTestData
        {
            get
            {
                return new[] {
                    new object[] { new Entity("usersettings") { ["dateformatstring"] = "dd/MM/yyyy", ["dateseparator"] = ".", ["timeformatstring"] = "HH:mm", ["timeseparator"] = ":" },
                        new DateTimeAttributeMetadata { Format = DateTimeFormat.DateAndTime }, "12.06.2022 23:17", new DateTime(2022, 6, 12, 23, 17, 0) },
                    new object[] { new Entity("usersettings") { ["dateformatstring"] = "dd/MM/yyyy", ["dateseparator"] = "/", ["timeformatstring"] = "HH:mm", ["timeseparator"] = ":" },
                        new DateTimeAttributeMetadata { Format = DateTimeFormat.DateAndTime }, "12/06/2022 23:17", new DateTime(2022, 6, 12, 23, 17, 0) },
                    new object[] { new Entity("usersettings") { ["dateformatstring"] = "dd/MM/yyyy", ["dateseparator"] = "-", ["timeformatstring"] = "HH:mm", ["timeseparator"] = ":" },
                        new DateTimeAttributeMetadata { Format = DateTimeFormat.DateAndTime }, "12-06-2022 23:17", new DateTime(2022, 6, 12, 23, 17, 0) },
                    new object[] { new Entity("usersettings") { ["dateformatstring"] = "dd/MM/yyyy", ["dateseparator"] = ".", ["timeformatstring"] = "hh:mm tt", ["timeseparator"] = ":" },
                        new DateTimeAttributeMetadata { Format = DateTimeFormat.DateAndTime }, "12.06.2022 11:17 AM", new DateTime(2022, 6, 12, 11, 17, 0) },
                    new object[] { new Entity("usersettings") { ["dateformatstring"] = "dd/MM/yyyy", ["dateseparator"] = "/", ["timeformatstring"] = "hh:mm tt", ["timeseparator"] = ":" },
                        new DateTimeAttributeMetadata { Format = DateTimeFormat.DateAndTime }, "12/06/2022 11:17 PM", new DateTime(2022, 6, 12, 23, 17, 0) },
                    new object[] { new Entity("usersettings") { ["dateformatstring"] = "dd/MM/yyyy", ["dateseparator"] = "-", ["timeformatstring"] = "h:m tt", ["timeseparator"] = ":" },
                        new DateTimeAttributeMetadata { Format = DateTimeFormat.DateAndTime }, "12-06-2022 3:8 PM", new DateTime(2022, 6, 12, 15, 8, 0) },
                    new object[] { new Entity("usersettings") { ["dateformatstring"] = "dd/MM/yyyy", ["dateseparator"] = "-", ["timeformatstring"] = "h:m tt", ["timeseparator"] = ":" },
                        new DateTimeAttributeMetadata { Format = DateTimeFormat.DateAndTime }, "12-06-2022 3:8 AM", new DateTime(2022, 6, 12, 3, 8, 0) },
                };
            }
        }

        public static IEnumerable<object[]> IntTestData
        {
            get
            {
                return new[] {
                    new object[] { new Entity("usersettings") { ["numberseparator"] = "." }, "1500", 1500 },
                    new object[] { new Entity("usersettings") { ["numberseparator"] = "." }, "1.500", 1500 },
                    new object[] { new Entity("usersettings") { ["numberseparator"] = "." }, "1.500.000", 1500000 },
                    new object[] { new Entity("usersettings") { ["numberseparator"] = "," }, "1,500", 1500 },
                    new object[] { new Entity("usersettings") { ["numberseparator"] = "," }, "1,500,000", 1500000 },
                    new object[] { new Entity("usersettings") { ["numberseparator"] = " " }, "1 500", 1500 },
                    new object[] { new Entity("usersettings") { ["numberseparator"] = " " }, "1 500 000", 1500000 },
                    new object[] { new Entity("usersettings") { ["numberseparator"] = "." }, "-1500", -1500 },
                    new object[] { new Entity("usersettings") { ["numberseparator"] = "." }, "(1500)", -1500 },
                    new object[] { new Entity("usersettings") { ["numberseparator"] = "," }, "- 1500", -1500 },
                    new object[] { new Entity("usersettings") { ["numberseparator"] = "," }, "1500-", -1500 },
                    new object[] { new Entity("usersettings") { ["numberseparator"] = "," }, "1500 -", -1500 },
                };
            }
        }

        public static IEnumerable<object[]> DecimalTestData
        {
            get
            {
                return new[] {
                    new object[] { new Entity("usersettings") { ["decimalsymbol"] = ",", ["numberseparator"] = "." }, "1,5238", (decimal)1.5238 },
                    new object[] { new Entity("usersettings") { ["decimalsymbol"] = ".", ["numberseparator"] = "," }, "1.25670", (decimal)1.2567 },
                    new object[] { new Entity("usersettings") { ["decimalsymbol"] = ",", ["numberseparator"] = "." }, "1.376,5238", (decimal)1376.5238 },
                    new object[] { new Entity("usersettings") { ["decimalsymbol"] = ".", ["numberseparator"] = "," }, "1,376.25670", (decimal)1376.2567 },
                    new object[] { new Entity("usersettings") { ["decimalsymbol"] = ".", ["numberseparator"] = "," }, "1,376,421.25670", (decimal)1376421.2567 },
                    new object[] { new Entity("usersettings") { ["decimalsymbol"] = ",", ["numberseparator"] = " " }, "1 376,5238", (decimal)1376.5238 },
                    new object[] { new Entity("usersettings") { ["decimalsymbol"] = ",", ["numberseparator"] = "." }, "-1,5238", (decimal)-1.5238 },
                    new object[] { new Entity("usersettings") { ["decimalsymbol"] = ".", ["numberseparator"] = "," }, "1.25670-", (decimal)-1.2567 },
                    new object[] { new Entity("usersettings") { ["decimalsymbol"] = ",", ["numberseparator"] = "." }, "- 1,5238", (decimal)-1.5238 },
                    new object[] { new Entity("usersettings") { ["decimalsymbol"] = ".", ["numberseparator"] = "," }, "1.25670 -", (decimal)-1.2567 },
                    new object[] { new Entity("usersettings") { ["decimalsymbol"] = ".", ["numberseparator"] = "," }, "(1.25670)", (decimal)-1.2567 },
                };
            }
        }

        public static IEnumerable<object[]> BoolTestData
        {
            get
            {
                return new[] {
                    new object[] {
                        new BooleanAttributeMetadata()
                        {
                            OptionSet = new BooleanOptionSetMetadata
                            {
                                TrueOption = new OptionMetadata
                                {
                                    Label = new Label(new LocalizedLabel { Label = "My TwoOptions"}, new LocalizedLabel[]
                                                { new LocalizedLabel("OptionTrue", 1031), new LocalizedLabel("OptionWahr", 1033) })
                                }
                            }
                        }, "true", true
                    },
                    new object[] {
                        new BooleanAttributeMetadata()
                        {
                            OptionSet = new BooleanOptionSetMetadata
                            {
                                TrueOption = new OptionMetadata
                                {
                                    Label = new Label(new LocalizedLabel { Label = "My TwoOptions"}, new LocalizedLabel[]
                                                { new LocalizedLabel("OptionTrue", 1031), new LocalizedLabel("OptionWahr", 1033) })
                                }
                            }
                        }, "OptionTrue", true
                    },
                    new object[] {
                        new BooleanAttributeMetadata()
                        {
                            OptionSet = new BooleanOptionSetMetadata
                            {
                                FalseOption = new OptionMetadata
                                {
                                    Label = new Label(new LocalizedLabel { Label = "My TwoOptions"}, new LocalizedLabel[]
                                                { new LocalizedLabel("OptionFalse", 1031), new LocalizedLabel("OptionFalsch", 1033) })
                                }
                            }
                        }, "false", false
                    },
                    new object[] {
                        new BooleanAttributeMetadata()
                        {
                            OptionSet = new BooleanOptionSetMetadata
                            {
                                FalseOption = new OptionMetadata
                                {
                                    Label = new Label(new LocalizedLabel { Label = "My TwoOptions"}, new LocalizedLabel[]
                                                { new LocalizedLabel("OptionFalse", 1031), new LocalizedLabel("OptionFalsch", 1033) })
                                },
                                TrueOption = new OptionMetadata
                                {
                                    Label = new Label(new LocalizedLabel { Label = "My TwoOptions"}, new LocalizedLabel[]
                                                { new LocalizedLabel("OptionTrue", 1031), new LocalizedLabel("OptionWahr", 1033) })
                                }
                            }
                        }, "OptionFalsch", false
                    },
                };
            }
        }

        public static IEnumerable<object[]> OptionSetTestData
        {
            get
            {
                return new[] {
                    new object[] {
                        new PicklistAttributeMetadata
                        {
                            OptionSet = new OptionSetMetadata(new OptionMetadataCollection(new List<OptionMetadata>()
                            {
                                new OptionMetadata { Label = new Label(new LocalizedLabel { Label = "Option One"}, new LocalizedLabel[]
                                                { new LocalizedLabel("Option One", 1031), new LocalizedLabel("Option Eins", 1033) }), Value = 1 },
                                new OptionMetadata { Label = new Label(new LocalizedLabel { Label = "Option Two"}, new LocalizedLabel[]
                                                { new LocalizedLabel("Option Two", 1031), new LocalizedLabel("Option Zwei", 1033) }), Value = 2 },
                            }))

                        }, "Option One", new OptionSetValue(1)
                    },
                    new object[] {
                        new PicklistAttributeMetadata
                        {
                            OptionSet = new OptionSetMetadata(new OptionMetadataCollection(new List<OptionMetadata>()
                            {
                                new OptionMetadata { Label = new Label(new LocalizedLabel { Label = "Option One"}, new LocalizedLabel[]
                                                { new LocalizedLabel("Option One", 1031), new LocalizedLabel("Option Eins", 1033) }), Value = 1 },
                                new OptionMetadata { Label = new Label(new LocalizedLabel { Label = "Option Two"}, new LocalizedLabel[]
                                                { new LocalizedLabel("Option Two", 1031), new LocalizedLabel("Option Zwei", 1033) }), Value = 2 },
                            }))

                        }, "1", new OptionSetValue(1)
                    },
                    new object[] {
                        new PicklistAttributeMetadata
                        {
                            OptionSet = new OptionSetMetadata(new OptionMetadataCollection(new List<OptionMetadata>()
                            {
                                new OptionMetadata { Label = new Label(new LocalizedLabel { Label = "Option One"}, new LocalizedLabel[]
                                                { new LocalizedLabel("Option One", 1031), new LocalizedLabel("Option Eins", 1033) }), Value = 1 },
                                new OptionMetadata { Label = new Label(new LocalizedLabel { Label = "Option Two"}, new LocalizedLabel[]
                                                { new LocalizedLabel("Option Two", 1031), new LocalizedLabel("Option Zwei", 1033) }), Value = 2 },
                            }))

                        }, "Option Eins", new OptionSetValue(1)
                    },
                    new object[] {
                        new PicklistAttributeMetadata
                        {
                            OptionSet = new OptionSetMetadata(new OptionMetadataCollection(new List<OptionMetadata>()
                            {
                                new OptionMetadata { Label = new Label(new LocalizedLabel { Label = "Option One"}, new LocalizedLabel[]
                                                { new LocalizedLabel("Option One", 1031), new LocalizedLabel("Option Eins", 1033) }), Value = 1 },
                                new OptionMetadata { Label = new Label(new LocalizedLabel { Label = "Option Two"}, new LocalizedLabel[]
                                                { new LocalizedLabel("Option Two", 1031), new LocalizedLabel("Option Zwei", 1033) }), Value = 2 },
                            }))

                        }, "1", new OptionSetValue(1)
                    },
                    new object[] {
                        new PicklistAttributeMetadata
                        {
                            OptionSet = new OptionSetMetadata(new OptionMetadataCollection(new List<OptionMetadata>()
                            {
                                new OptionMetadata { Label = new Label(new LocalizedLabel { Label = "Option One"}, new LocalizedLabel[]
                                                { new LocalizedLabel("Option One", 1031), new LocalizedLabel("Option Eins", 1033) }), Value = 1 },
                                new OptionMetadata { Label = new Label(new LocalizedLabel { Label = "Option Two"}, new LocalizedLabel[]
                                                { new LocalizedLabel("Option Two", 1031), new LocalizedLabel("Option Zwei", 1033) }), Value = 2 },
                            }))

                        }, "Option Two", new OptionSetValue(2)
                    },
                    new object[] {
                        new PicklistAttributeMetadata
                        {
                            OptionSet = new OptionSetMetadata(new OptionMetadataCollection(new List<OptionMetadata>()
                            {
                                new OptionMetadata { Label = new Label(new LocalizedLabel { Label = "Option One"}, new LocalizedLabel[]
                                                { new LocalizedLabel("Option One", 1031), new LocalizedLabel("Option Eins", 1033) }), Value = 1 },
                                new OptionMetadata { Label = new Label(new LocalizedLabel { Label = "Option Two"}, new LocalizedLabel[]
                                                { new LocalizedLabel("Option Two", 1031), new LocalizedLabel("Option Zwei", 1033) }), Value = 2 },
                            }))

                        }, "Option Zwei", new OptionSetValue(2)
                    },
                };
            }
        }

        public static IEnumerable<object[]> EntityReferenceTestData
        {
            get
            {
                return new[] {
                    new object[]
                    {
                        new LookupAttributeMetadata()
                        {
                            Targets = new[] { Account.EntityLogicalName }
                        },
                        "a5cc3fe9-bf78-ea11-a2d9-005056ba5040",
                        new EntityReference(Account.EntityLogicalName, new Guid("a5cc3fe9-bf78-ea11-a2d9-005056ba5040"))
                    },
                    new object[]
                    {
                        new LookupAttributeMetadata()
                        {
                            Targets = new[] { Account.EntityLogicalName }
                        },
                        "https://<org>/main.aspx?pagetype=entityrecord&etn=account&id=a5cc3fe9-bf78-ea11-a2d9-005056ba5040",
                        new EntityReference(Account.EntityLogicalName, new Guid("a5cc3fe9-bf78-ea11-a2d9-005056ba5040"))
                    }
                };
            }
        }

        public static IEnumerable<object[]> MoneyTestData
        {
            get
            {
                return new[] {
                    new object[]
                    {
                        new Entity("usersettings") { ["currencysymbol"] = "€", ["decimalsymbol"] = ",", ["numberseparator"] = "."  }, "125,58 €", new Money((decimal)125.58)
                    },
                    new object[]
                    {
                        new Entity("usersettings") { ["currencysymbol"] = "€", ["decimalsymbol"] = ",", ["numberseparator"] = "." }, "125,58€", new Money((decimal)125.58)
                    },
                    new object[]
                    {
                        new Entity("usersettings") { ["currencysymbol"] = "€", ["decimalsymbol"] = ",", ["numberseparator"] = "." }, "125.211,58€", new Money((decimal)125211.58)
                    },
                    new object[]
                    {
                        new Entity("usersettings") { ["currencysymbol"] = "$", ["decimalsymbol"] = ".", ["numberseparator"] = "," }, "$ 125.58", new Money((decimal)125.58)
                    },
                    new object[]
                    {
                        new Entity("usersettings") { ["currencysymbol"] = "$", ["decimalsymbol"] = ".", ["numberseparator"] = "," }, "$ 125,211.58", new Money((decimal)125211.58)
                    },
                    new object[]
                    {
                        new Entity("usersettings") { ["currencysymbol"] = "$", ["decimalsymbol"] = ".", ["numberseparator"] = "," }, "$ -125,211.58", new Money((decimal)-125211.58)
                    },
                    new object[]
                    {
                        new Entity("usersettings") { ["currencysymbol"] = "$", ["decimalsymbol"] = ".", ["numberseparator"] = "," }, "-125,211.58 $", new Money((decimal)-125211.58)
                    },
                    new object[]
                    {
                        new Entity("usersettings") { ["currencysymbol"] = "$", ["decimalsymbol"] = ".", ["numberseparator"] = "," }, "$ - 125,211.58", new Money((decimal)-125211.58)
                    },
                };
            }
        }
        #endregion Test Data
    }
}
