#region Auto-generated classes for main database on 2010-05-19 16:31:56Z

//
//  ____  _     __  __      _        _
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from main on 2010-05-19 16:31:56Z
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

		public Table<Channel> Channels { get { return GetTable<Channel>(); } }
		public Table<Log> Logs { get { return GetTable<Log>(); } }
		public Table<Setting> Settings { get { return GetTable<Setting>(); } }
		public Table<User> Users { get { return GetTable<User>(); } }

	}

	[Table(Name = "main.Channel")]
	public partial class Channel : INotifyPropertyChanging, INotifyPropertyChanged
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
		partial void OnIDChanging(int? value);
		partial void OnNameChanged();
		partial void OnNameChanging(string value);

		#endregion

		#region int? ID

		private int? _id;
		[DebuggerNonUserCode]
		[Column(Storage = "_id", Name = "ID", DbType = "INTEGER", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.Never)]
		public int? ID
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

		#region string Name

		private string _name;
		[DebuggerNonUserCode]
		[Column(Storage = "_name", Name = "Name", DbType = "VARCHAR(255)", AutoSync = AutoSync.Never, CanBeNull = false)]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (value != _name)
				{
					OnNameChanging(value);
					SendPropertyChanging();
					_name = value;
					SendPropertyChanged("Name");
					OnNameChanged();
				}
			}
		}

		#endregion

		#region ctor

		public Channel()
		{
			OnCreated();
		}

		#endregion

	}

	[Table(Name = "main.Log")]
	public partial class Log : INotifyPropertyChanging, INotifyPropertyChanged
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
		partial void OnIDChanging(int? value);
		partial void OnLevelChanged();
		partial void OnLevelChanging(int? value);
		partial void OnLocationChanged();
		partial void OnLocationChanging(string value);
		partial void OnMessageChanged();
		partial void OnMessageChanging(string value);
		partial void OnTimeChanged();
		partial void OnTimeChanging(DateTime value);

		#endregion

		#region int? ID

		private int? _id;
		[DebuggerNonUserCode]
		[Column(Storage = "_id", Name = "ID", DbType = "INTEGER", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.Never)]
		public int? ID
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

		#region int? Level

		private int? _level;
		[DebuggerNonUserCode]
		[Column(Storage = "_level", Name = "Level", DbType = "INTEGER", AutoSync = AutoSync.Never)]
		public int? Level
		{
			get
			{
				return _level;
			}
			set
			{
				if (value != _level)
				{
					OnLevelChanging(value);
					SendPropertyChanging();
					_level = value;
					SendPropertyChanged("Level");
					OnLevelChanged();
				}
			}
		}

		#endregion

		#region string Location

		private string _location;
		[DebuggerNonUserCode]
		[Column(Storage = "_location", Name = "Location", DbType = "VARCHAR(255)", AutoSync = AutoSync.Never)]
		public string Location
		{
			get
			{
				return _location;
			}
			set
			{
				if (value != _location)
				{
					OnLocationChanging(value);
					SendPropertyChanging();
					_location = value;
					SendPropertyChanged("Location");
					OnLocationChanged();
				}
			}
		}

		#endregion

		#region string Message

		private string _message;
		[DebuggerNonUserCode]
		[Column(Storage = "_message", Name = "Message", DbType = "TEXT", AutoSync = AutoSync.Never)]
		public string Message
		{
			get
			{
				return _message;
			}
			set
			{
				if (value != _message)
				{
					OnMessageChanging(value);
					SendPropertyChanging();
					_message = value;
					SendPropertyChanged("Message");
					OnMessageChanged();
				}
			}
		}

		#endregion

		#region DateTime Time

		private DateTime _time;
		[DebuggerNonUserCode]
		[Column(Storage = "_time", Name = "Time", DbType = "TIMESTAMP", AutoSync = AutoSync.Never, CanBeNull = false)]
		public DateTime Time
		{
			get
			{
				return _time;
			}
			set
			{
				if (value != _time)
				{
					OnTimeChanging(value);
					SendPropertyChanging();
					_time = value;
					SendPropertyChanged("Time");
					OnTimeChanged();
				}
			}
		}

		#endregion

		#region ctor

		public Log()
		{
			OnCreated();
		}

		#endregion

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

	[Table(Name = "main.User")]
	public partial class User : INotifyPropertyChanging, INotifyPropertyChanged
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

		#region ctor

		public User()
		{
			OnCreated();
		}

		#endregion

	}
}
