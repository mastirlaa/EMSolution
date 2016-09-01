using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMLibrary
{
    public class NameExtractor
    {
        private string m_source;
        public NameExtractor(INameProvider provider)
        {
            m_source = provider.Name;
        }

        public NameExtractionResult Extract()
        {
            string[] parts = m_source.Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            if(parts.Length > 3)
            {
                throw new FormatException();
            }
            if(parts.Length == 1)
            {
                return new NameExtractionResult()
                {
                    LastName = parts[0]
                };
            }
            if (parts.Length == 3)
            {
                return new NameExtractionResult()
                {
                    Title = parts[0],
                    FirstName = parts[1],
                    LastName = parts[2]
                };
            }
            if (parts.Length == 2)
            {
                string[] validTitles = { "Mr", "Miss", "Ms", "Mrs", "Mx", "Sir", "Madam", "Dame", "Lord", "Lady", "Adv", "Dr", "Prof", "Fr", "Pr", "Br", "Sr", "Elder", "Rabbi" };

                if(validTitles.Contains(parts[0]))
                {
                    return new NameExtractionResult()
                    {
                        Title = parts[0],
                        LastName = parts[1]
                    };
                }
                else
                {
                    return new NameExtractionResult()
                    {
                        FirstName = parts[0],
                        LastName = parts[1]
                    };

                }
            }
            // Real Implementation should be here
            return new NameExtractionResult();
        }
    }
}
