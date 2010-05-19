#region Auto-generated classes for main database on 2010-05-19 15:09:12Z

//
//  ____  _     __  __      _        _
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from main on 2010-05-19 15:09:12Z
// Please visit http://linq.to/db for more information

#endregion

using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using System.Reflection;
#if MONO_STRICT
using System.Data.Linq;
#else   // MONO_STRICT
using DbLinq.Data.Linq;
using DbLinq.Vendor;
#endif  // MONO_STRICT
using System.ComponentModel;

namespace IrcD.Database
{
	public partial class Main : DataContext
	{
		#region Extensibility Method Definitions

		partial void OnCreated();

		#endregion

		public Main(string connectionString)
			: base(connectionString)
		{
			OnCreated();
		}

		public Main(IDbConnection connection)
		#if MONO_STRICT
			: base(connection)
		#else   // MONO_STRICT
			: base(connection, new DbLinq.Sqlite.SqliteVendor())
		#endif  // MONO_STRICT
		{
			OnCreated();
		}

		public Main(string connection, MappingSource mappingSource)
			: base(connection, mappingSource)
		{
			OnCreated();
		}

		public Main(IDbConnection connection, MappingSource mappingSource)
			: base(connection, mappingSource)
		{
			OnCreated();
		}

		#if !MONO_STRICT
		public Main(IDbConnection connection, IVendor vendor)
			: base(connection, vendor)
		{
			OnCreated();
		}
		#endif  // !MONO_STRICT

		#if !MONO_STRICT
		public Main(IDbConnection connection, MappingSource mappingSource, IVendor vendor)
			: base(connection, mappingSource, vendor)
		{
			OnCreated();
		}
		#endif  // !MONO_STRICT

		public Table<Setting> Settings { get { return GetTable<Setting>(); } }

	}

	[Table(Name = "main.Setting")]
	public partial class Setting : INotifyPropertyChanging, INotifyPropertyChanged
	{
		#region INotifyPropertyChanging handling

		public event PropertyChangingEventHandler PropertyChanging;

		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");
		protected virtual void SendPropertyChanging()
		{
			if (PropertyChanging != null)
			{
				PropertyChanging(this, emptyChangingEventArgs);
			}
		}

		#endregion

		#region INotifyPropertyChanged handling

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void SendPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		#region Extensibility Method Definitions

		partial void OnCreated();
		partial void OnIDChanged();
		partial void OnIDChanging(int value);
		partial void OnKeyChanged();
		partial void OnKeyChanging(string value);
		partial void OnValueChanged();
		partial void OnValueChanging(string value);

		#endregion

		#region int ID

		private int _id;
		[DebuggerNonUserCode]
		[Column(Storage = "_id", Name = "ID", DbType = "INTEGER", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		public int ID
		{
			get
			{
				return _id;
			}
			set
			{
				if (value != _id)
				{
					OnIDChanging(value);
					SendPropertyChanging();
					_id = value;
					SendPropertyChanged("ID");
					OnIDChanged();
				}
			}
		}

		#endregion

		#region string Key

		private string _key;
		[DebuggerNonUserCode]
		[Column(Storage = "_key", Name = "Key", DbType = "VARCHAR(255)", AutoSync = AutoSync.Never, CanBeNull = false)]
		public string Key
		{
			get
			{
				return _key;
			}
			set
			{
				if (value != _key)
				{
					OnKeyChanging(value);
					SendPropertyChanging();
					_key = value;
					SendPropertyChanged("Key");
					OnKeyChanged();
				}
			}
		}

		#endregion

		#region string Value

		private string _value;
		[DebuggerNonUserCode]
		[Column(Storage = "_value", Name = "Value", DbType = "TEXT", AutoSync = AutoSync.Never)]
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				if (value != _value)
				{
					OnValueChanging(value);
					SendPropertyChanging();
					_value = value;
					SendPropertyChanged("Value");
					OnValueChanged();
				}
			}
		}

		#endregion

		#region ctor

		public Setting()
		{
			OnCreated();
		}

		#endregion

	}
}
