using System;

namespace CamGenie
{
	public class TextField
	{
		private string m_Name;
		private TypeCode m_DataType = TypeCode.String;
		private int m_Length;
		private bool m_Quoted = true;
		private object m_Value = null;

		public TextField(string Name, TypeCode DataType, bool Quoted) : this(Name, DataType, 4095, Quoted)
		{

		}

		public TextField(string Name, TypeCode DataType) : this(Name, DataType, 4095, false)
		{

		}

		public TextField(string Name, TypeCode DataType, int Length) : this(Name, DataType, Length, false)
		{

		}

		private TextField(string Name, TypeCode DataType, int Length, bool Quoted)
		{
			m_Name = Name;
			m_DataType = DataType;
			m_Length = Length;
			m_Quoted = Quoted;
			m_Value = null;
		}

		public string Name
		{
			get{return m_Name;}
			set
			{
				if(value.Length < 1 || value == null || value == String.Empty)
					throw new ApplicationException("You can not set the Name property to a blank, empty or null value.");
			
				m_Name = value;
			}
		}


		public TypeCode DataType
		{
			get{return m_DataType;}
			set{m_DataType = value;}
		}

		public int Length
		{
			get{return m_Length;}
			set
			{
				if(value < 1)
					throw new ApplicationException("You can not set the Length property to a zero or negative value.");
				m_Length = value;
			}
		}

		public bool Quoted
		{
			get{return m_Quoted;}
			set{m_Quoted = value;}
		}

		public object Value
		{
			get{return m_Value;}
			set
			{
				try
				{
					if(value.ToString().Trim().Length == 0) //Allow for null values.
					{
						m_Value = Convert.DBNull;
					}
					else if(m_DataType == TypeCode.Boolean) //special form boolean
					{
						if(value.ToString().Trim() == "1")
							m_Value = true;
						else if(value.ToString().Trim() == "0")
							m_Value = false;
						else
							m_Value = Convert.ChangeType(value, m_DataType);
					}
					else
					{
						m_Value = Convert.ChangeType(value, m_DataType);
					}
				}
				catch
				{
					throw new ArgumentException(String.Format("There was an error converting the value \"{0}\" to a {1} for the field \"{2}\".", value, m_DataType.ToString(), m_Name));
				}
			}
		}
	}

	public class TextFieldCollection : System.Collections.CollectionBase
	{
		public TextFieldCollection() : base ()
		{
		}

		public TextFieldCollection(TextFieldCollection texValue) : base()
		{
			AddRange(texValue);
		}


		public TextFieldCollection(TextField[] texValue) : base()
		{
			AddRange(texValue);
		}

		public TextField this[int index]
		{
			get{return (TextField)List[index];}
			set{List[index] = value;}
		}

		public int Add(TextField texValue)
		{
			return List.Add(texValue);
		}

		public void AddRange(TextField[] texValue)
		{
			int intCounter = 0;
			while(intCounter < texValue.Length)
			{
				Add(texValue[intCounter]);
				intCounter ++;
			}
		}

		public void AddRange(TextFieldCollection texValue)
		{
			int intCounter = 0;
			while(intCounter < texValue.Count)
			{
				Add(texValue[intCounter]);
				intCounter++;
			}
		}


		public bool Contains(TextField texValue)
		{
			return List.Contains(texValue);
		}

		public void CopyTo(TextField[] texArray, int intIndex)
		{
			List.CopyTo(texArray, intIndex);
		}

		public int IndexOf(TextField texValue)
		{
			return List.IndexOf(texValue);
		}

		public void Insert(int intIndex, TextField texValue)
		{
			List.Insert(intIndex, texValue);
		}

		public new TextFieldEnumerator GetEnumerator()
		{
			return new TextFieldEnumerator(this);
		}

		public void Remove(TextField texValue)
		{
			List.Remove(texValue);
		}

		public class TextFieldEnumerator : System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator iEnBase;
			private System.Collections.IEnumerable iEnLocal;

			public TextFieldEnumerator(TextFieldCollection texMappings) : base()
			{
				iEnLocal = (System.Collections.IEnumerable)texMappings;
				iEnBase = iEnLocal.GetEnumerator();
			}

			public TextField Current
			{
				get
				{
					return (TextField)iEnBase.Current;
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get{return iEnBase.Current;}
			}

			public bool MoveNext()
			{
				return iEnBase.MoveNext();
			}

			bool System.Collections.IEnumerator.MoveNext()
			{
				return iEnBase.MoveNext();
			}

			public void Reset()
			{
				iEnBase.Reset();
			}

			void System.Collections.IEnumerator.Reset()
			{
				iEnBase.Reset();
			}
		}
	}
}
