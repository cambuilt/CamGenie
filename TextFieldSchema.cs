using System;
using System.IO;
using System.Xml;

namespace CamGenie
{
	internal class TextFieldSchema
	{
		private string m_FilePath = "";
		private string m_TableName = "TEXTDATA";
		private char m_FieldDelimiter = ',';
		private char m_QuoteDelimiter = '\"';
		private FileFormat m_FileFormat = FileFormat.Delimited;
		private TextFieldCollection m_TextFields = null;

		public TextFieldSchema()
		{
		
		}

		public TextFieldSchema(string filePath)
		{
			m_FilePath = filePath;
			ParseSchema();
		}

		private void ParseSchema()
		{
			m_TextFields = new TextFieldCollection();

			XmlDocument doc = new XmlDocument();
			doc.Load(m_FilePath);
			
			XmlNodeList lst = doc.GetElementsByTagName("TABLE");

			if(lst.Count == 0)
				throw new XmlException("Could not locate the 'TABLE' node." + Environment.NewLine + 
					"The Schema is case sensitive.");

			if(lst.Count > 1)
				throw new XmlException("There are multiple 'TABLE' nodes." + Environment.NewLine + 
					"There can be only one 'TABLE' node.");

			XmlNode tableNode = lst[0];

			foreach(XmlAttribute attribute in tableNode.Attributes)
			{
				switch(attribute.Name.ToLower())
				{
					case "name":
						m_TableName = attribute.Value;
						break;
					case "fileformat":
						m_FileFormat = (FileFormat)Enum.Parse(typeof(FileFormat), attribute.Value);
						break;
					case "delimiter":
						m_FieldDelimiter = attribute.Value[0];
						break;
					case "quotecharacter":
						m_QuoteDelimiter = attribute.Value[0];
						break;
					default:
						throw new NotSupportedException("The attribute '" + attribute.Name + "' is not supported.");
				}
			}

			lst = doc.GetElementsByTagName("FIELD");
			TextField field;
			string name = "";
			TypeCode datatype;
			bool quoted = false;
			int length = 0;
			foreach(XmlNode node in lst)
			{
				name = "";
				datatype = TypeCode.String;
				quoted = false;
				length = 0;
				
				foreach(XmlAttribute fattribute in node.Attributes)
				{
					switch(fattribute.Name.ToLower())
					{
						case "name":
							name = fattribute.Value;
							break;
						case "datatype":
							datatype = (TypeCode)Enum.Parse(typeof(TypeCode), fattribute.Value);
							break;
						case "quoted":
							quoted = Boolean.Parse(fattribute.Value);
							break;
						case "length":
							length = Int32.Parse(fattribute.Value);
							break;
						default:
							throw new NotSupportedException("The attribute '" + fattribute.Name + "' is not supported.");
					}
				}

				if(name.Trim().Length == 0)
					throw new ArgumentException("The attribute 'Name' cannot be blank");
				
				if(m_FileFormat == FileFormat.FixedWidth && length <= 0)
					throw new ArgumentOutOfRangeException("A 'Length' attribute > 0 must be specified for all fields in a fixed width file.");

				if(m_FileFormat == FileFormat.FixedWidth)
					field = new TextField(name, datatype, length);
				else
					field = new TextField(name, datatype, quoted);

				m_TextFields.Add(field);
			}
		}

		public string FilePath
		{
			get{return m_FilePath;}
			set
			{
				m_FilePath = value;
				ParseSchema();
			}
		}

		public string TableName
		{
			get{return m_TableName;}
			set{m_TableName = value;}
		}

		public char FieldDelimiter
		{
			get{return m_FieldDelimiter;}
			set{m_FieldDelimiter = value;}
		}

		public char QuoteDelimiter
		{
			get{return m_QuoteDelimiter;}
			set{m_QuoteDelimiter = value;}
		}

		public FileFormat FileFormat
		{
			get{return m_FileFormat;}
			set{m_FileFormat = value;}
		}

		public TextFieldCollection TextFields
		{
			get{return m_TextFields;}
			set{m_TextFields = value;}
		}
	}
}
