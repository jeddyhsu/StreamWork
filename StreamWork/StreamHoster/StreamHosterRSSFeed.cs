using System.Xml.Serialization;
using System.Collections.Generic;

namespace StreamHoster
{
	[XmlRoot(ElementName = "link", Namespace = "http://www.w3.org/2005/Atom")]
	public class Link
	{
		[XmlAttribute(AttributeName = "href")]
		public string Href { get; set; }
		[XmlAttribute(AttributeName = "rel")]
		public string Rel { get; set; }
		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }
	}

	[XmlRoot(ElementName = "thumbnail", Namespace = "http://search.yahoo.com/mrss/")]
	public class Thumbnail
	{
		[XmlAttribute(AttributeName = "url")]
		public string Url { get; set; }
	}

	[XmlRoot(ElementName = "content", Namespace = "http://search.yahoo.com/mrss/")]
	public class Content
	{
		[XmlAttribute(AttributeName = "url")]
		public string Url { get; set; }
		[XmlAttribute(AttributeName = "duration")]
		public string Duration { get; set; }
		[XmlAttribute(AttributeName = "lang")]
		public string Lang { get; set; }
		[XmlAttribute(AttributeName = "medium")]
		public string Medium { get; set; }
	}

	[XmlRoot(ElementName = "item")]
	public class Item
	{
		[XmlElement(ElementName = "title")]
		public List<string> Title { get; set; }
		[XmlElement(ElementName = "pubDate")]
		public string PubDate { get; set; }
		[XmlElement(ElementName = "description")]
		public List<string> Description { get; set; }
		[XmlElement(ElementName = "guid")]
		public string Guid { get; set; }
		[XmlElement(ElementName = "mediaid", Namespace = "http://static.streamhoster.com/feed/sh.dtd")]
		public string Mediaid { get; set; }
		[XmlElement(ElementName = "category", Namespace = "http://search.yahoo.com/mrss/")]
		public string Category { get; set; }
		[XmlElement(ElementName = "thumbnail", Namespace = "http://search.yahoo.com/mrss/")]
		public Thumbnail Thumbnail { get; set; }
		[XmlElement(ElementName = "content", Namespace = "http://search.yahoo.com/mrss/")]
		public Content Content { get; set; }
	}

	[XmlRoot(ElementName = "channel")]
	public class Channel
	{
		[XmlElement(ElementName = "title")]
		public string Title { get; set; }
		[XmlElement(ElementName = "description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "language")]
		public string Language { get; set; }
		[XmlElement(ElementName = "lastBuildDate")]
		public string LastBuildDate { get; set; }
		[XmlElement(ElementName = "link")]
		public List<string> Link { get; set; }
		[XmlElement(ElementName = "pubDate")]
		public string PubDate { get; set; }
		[XmlElement(ElementName = "item")]
		public Item Item { get; set; }
	}

	[XmlRoot(ElementName = "rss")]
	public class StreamHosterRSSFeed
	{
		[XmlElement(ElementName = "channel")]
		public Channel Channel { get; set; }
		[XmlAttribute(AttributeName = "version")]
		public string Version { get; set; }
		[XmlAttribute(AttributeName = "sh", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Sh { get; set; }
		[XmlAttribute(AttributeName = "media", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Media { get; set; }
		[XmlAttribute(AttributeName = "atom", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Atom { get; set; }
		[XmlAttribute(AttributeName = "itunes", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Itunes { get; set; }
	}

}
