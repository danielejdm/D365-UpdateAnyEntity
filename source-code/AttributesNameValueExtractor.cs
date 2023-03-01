using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateAnyEntity
{
    public static class AttributesNameValueExtractor
    {
        private const string fieldsSeparator = "<>";
        private const string fieldAssigner = "=>";

        /// <summary>
        /// Extract attribute name and value from a string
        /// </summary>
        /// <param name="input">String representation of attribute name and value: attribute1Name=attribute1Val<>attribute2Name=attribute2Val...</param>
        /// <returns>List of keyvaluepair (attributeName, attributeVal))</returns>
        public static IList<KeyValuePair<string, string>> GetAttributesNamesValuesPair(string input)
        {
            if ((input = input.Trim()).EndsWith(fieldAssigner))
            {
                var lastFieldsSeparator = input.LastIndexOf(fieldsSeparator);
                input = input.Substring(0, lastFieldsSeparator);
            }

            try
            {
                return (from a in input.Split(new string[] { fieldsSeparator }, StringSplitOptions.None)
                        select new KeyValuePair<string, string>(a.Split(new string[] { fieldAssigner }, StringSplitOptions.None)[0].Trim(),
                        a.Split(new string[] { fieldAssigner }, StringSplitOptions.None)[1].Trim())).ToList();
            }
            catch
            {
                throw new ArgumentException($"Input string not in a valid format. Input was: '${input}'.");
            }
        }
    }
}
