using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;
using System;
using System.Globalization;
using System.Linq;

namespace UpdateAnyEntity
{
    /// <summary>
    /// Class for building attribute value starting from the attribute metadata and the user settings
    /// </summary>
    public static class UpdateEntityAttributeValueBuilderService
    {
        /// <summary>
        /// Build a DateTime value from user settings and attribute metadata
        /// </summary>
        /// <param name="userSettings"></param>
        /// <param name="attributeMetadata"></param>
        /// <param name="attributeValue"></param>
        /// <returns>DateTime value</returns>
        public static DateTime BuildDateTime(Entity userSettings, AttributeMetadata attributeMetadata, string attributeValue)
        {
            var datetimeMetadata = (DateTimeAttributeMetadata)attributeMetadata;
            var dateTimeFormatString = userSettings.GetAttributeValue<string>("dateformatstring");

            string dateFormat = dateTimeFormatString.Replace("/", userSettings.GetAttributeValue<string>("dateseparator"));

            if (datetimeMetadata.Format == DateTimeFormat.DateAndTime)
            {
                dateFormat += $" { userSettings.GetAttributeValue<string>("timeformatstring").Replace(":", userSettings.GetAttributeValue<string>("timeseparator")) }";

                dateFormat += dateTimeFormatString.Contains("hh") ? dateFormat += $" tt" :
                    dateTimeFormatString.Contains("h") ? $"t" : string.Empty;
            }

            var dateTime = DateTime.ParseExact(attributeValue, dateFormat, CultureInfo.InvariantCulture);

            return dateTime;
        }

        /// <summary>
        /// Build an int value 
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns>int value</returns>
        public static int BuildInt(Entity userSettings, string attributeValue)
        {
            attributeValue = attributeValue.Trim().Replace("- ", "-").Replace(" -", "-");
            var numberStyles = NumberStyles.Number | NumberStyles.AllowThousands | NumberStyles.Integer | NumberStyles.AllowParentheses;

            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberGroupSeparator = userSettings.GetAttributeValue<string>("numberseparator");
            numberFormatInfo.NumberDecimalSeparator = userSettings.GetAttributeValue<string>("numberseparator") == "," ? "." : ",";

            return int.Parse(attributeValue, numberStyles, numberFormatInfo);
        }

        /// <summary>
        /// Build a decimal value from user settings
        /// </summary>
        /// <param name="userSettings"></param>
        /// <param name="attributeValue"></param>
        /// <returns>decimal value</returns>
        public static decimal BuildDecimal(Entity userSettings, string attributeValue)
        {
            attributeValue = attributeValue.Trim().Replace("- ", "-").Replace(" -", "-");
            var numberStyles = NumberStyles.Number | 
                                NumberStyles.AllowThousands | 
                                NumberStyles.Float | 
                                NumberStyles.AllowParentheses;
            
            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = userSettings.GetAttributeValue<string>("decimalsymbol");
            numberFormatInfo.NumberGroupSeparator = userSettings.GetAttributeValue<string>("numberseparator");

            return decimal.Parse(attributeValue, numberStyles, numberFormatInfo);
        }

        /// <summary>
        /// Build a boolean value from attribute metadata
        /// </summary>
        /// <param name="attributeMetadata"></param>
        /// <param name="attributeValue"></param>
        /// <returns>bool value</returns>
        public static bool BuildBool(AttributeMetadata attributeMetadata, string attributeValue)
        {
            bool boolVal;
            if (!bool.TryParse(attributeValue, out boolVal))
            {
                var optionMetadata = ((BooleanAttributeMetadata)attributeMetadata).OptionSet;
                var optTrue = optionMetadata.TrueOption.Label.LocalizedLabels.ToList().Where(a => a.Label.Equals(attributeValue, StringComparison.OrdinalIgnoreCase));

                boolVal = optTrue.Any();
            }

            return boolVal;
        }

        /// <summary>
        /// Build an optionset value from attribute metadata
        /// </summary>
        /// <param name="attributeMetadata"></param>
        /// <param name="attributeValue"></param>
        /// <returns>option set value</returns>
        public static OptionSetValue BuildOptionSet(AttributeMetadata attributeMetadata, string attributeValue)
        {
            int optionsetVal;
            if (!int.TryParse(attributeValue, out optionsetVal))
            {
                var optionsMetadata = ((PicklistAttributeMetadata)attributeMetadata).OptionSet.Options.ToList();

                var opt = optionsMetadata.Where(a => a.Label.LocalizedLabels.Where(l => l.Label.Equals(attributeValue, StringComparison.OrdinalIgnoreCase)).Any());

                optionsetVal = (opt.Count() > 0 && opt.First().Value.HasValue) ? opt.First().Value.Value : 0;
            }

            return new OptionSetValue(optionsetVal);
        }

        /// <summary>
        /// Build an EntityReference value from attribute metadata
        /// </summary>
        /// <param name="attributeMetadata"></param>
        /// <param name="attributeValue"></param>
        /// <returns>entity reference value</returns>
        public static EntityReference BuildEntityReference(AttributeMetadata attributeMetadata, string attributeValue)
        {
            Guid entityId;
            if (!Guid.TryParse(attributeValue, out entityId))
            {
                entityId = Guid.Parse(DynamicsUrlService.GetRecordIdFromUrl(attributeValue));
            }
            var entityType = ((LookupAttributeMetadata)attributeMetadata).Targets[0];

            return new EntityReference(entityType, entityId);
        }

        /// <summary>
        /// Build a money value from user settings
        /// </summary>
        /// <param name="userSettings"></param>
        /// <param name="attributeValue"></param>
        /// <returns>money value</returns>
        public static Money BuildMoney(Entity userSettings, string attributeValue)
        {
            attributeValue = attributeValue.Trim().Replace("- ", "-").Replace(" -", "-");
            
            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = userSettings.GetAttributeValue<string>("decimalsymbol");
            numberFormatInfo.CurrencySymbol = userSettings.GetAttributeValue<string>("currencysymbol");
            numberFormatInfo.NumberGroupSeparator = userSettings.GetAttributeValue<string>("numberseparator");
            numberFormatInfo.CurrencyDecimalSeparator = userSettings.GetAttributeValue<string>("decimalsymbol");
            numberFormatInfo.CurrencyGroupSeparator = userSettings.GetAttributeValue<string>("numberseparator");
            numberFormatInfo.CurrencyGroupSizes = new int[] { 3 };

            var numberStyles = NumberStyles.Currency | 
                                NumberStyles.AllowCurrencySymbol | 
                                NumberStyles.AllowThousands | 
                                NumberStyles.AllowDecimalPoint | 
                                NumberStyles.AllowParentheses;

            return new Money(decimal.Parse(attributeValue, numberStyles, numberFormatInfo));
        }
    }
}
