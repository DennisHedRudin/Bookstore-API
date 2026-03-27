using System.Xml.Serialization;

namespace Bookstore_API.Models
{
    public class SuggestionModel
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Author")]
        public string Author { get; set; }

        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }

    }
}