// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from main on 2010-05-30 20:37:40Z.
// Please visit http://code.google.com/p/dblinq2007/ for more information.
//
namespace IrcD.Database
{
	using System;
	using System.ComponentModel;
	using System.Data;
#if MONO_STRICT
	using System.Data.Linq;
#else   // MONO_STRICT
	using DbLinq.Data.Linq;
	using DbLinq.Vendor;
#endif  // MONO_STRICT
	using System.Data.Linq.Mapping;
	using System.Diagnostics;
	
	
	public partial class Main : DataContext
	{
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		#endregion
		
		
		public Main(string connectionString) : 
				base(connectionString)
		{
			this.OnCreated();
		}
		
		public Main(string connection, MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			this.OnCreated();
		}
		
		public Main(IDbConnection connection, MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			this.OnCreated();
		}
		
		public Table<Channel> Channels
		{
			get
			{
				return this.GetTable<Channel>();
			}
		}
		
		public Table<Log> Logs
		{
			get
			{
				return this.GetTable<Log>();
			}
		}
		
		public Table<Setting> Settings
		{
			get
			{
				return this.GetTable<Setting>();
			}
		}
		
		public Table<WhoWa> WhoWas
		{
			get
			{
				return this.GetTable<WhoWa>();
			}
		}
	}
	
	#region Start MONO_STRICT
#if MONO_STRICT

	public partial class Main
	{
		
		public Main(IDbConnection connection) : 
				base(connection)
		{
			this.OnCreated();
		}
	}
	#region End MONO_STRICT
	#endregion
#else     // MONO_STRICT
	
	public partial class Main
	{
		
		public Main(IDbConnection connection) : 
				base(connection, new DbLinq.Sqlite.SqliteVendor())
		{
			this.OnCreated();
		}
		
		public Main(IDbConnection connection, IVendor sqlDialect) : 
				base(connection, sqlDialect)
		{
			this.OnCreated();
		}
		
		public Main(IDbConnection connection, MappingSource mappingSource, IVendor sqlDialect) : 
				base(connection, mappingSource, sqlDialect)
		{
			this.OnCreated();
		}
	}
	#region End Not MONO_STRICT
	#endregion
#endif     // MONO_STRICT
	#endregion
	
	[Table(Name="main.Channel")]
	public partial class Channel : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private System.Nullable<System.DateTime> _created;
		
		private System.Nullable<int> _id;
		
		private string _name;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnCreatedChanged();
		
		partial void OnCreatedChanging(System.Nullable<System.DateTime> value);
		
		partial void OnIDChanged();
		
		partial void OnIDChanging(System.Nullable<int> value);
		
		partial void OnNameChanged();
		
		partial void OnNameChanging(string value);
		#endregion
		
		
		public Channel()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_created", Name="Created", DbType="TIMESTAMP", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<System.DateTime> Created
		{
			get
			{
				return this._created;
			}
			set
			{
				if ((_created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		[Column(Storage="_id", Name="ID", DbType="INTEGER", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> ID
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((_id != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[Column(Storage="_name", Name="Name", DbType="VARCHAR(255)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (((_name == value) 
							== false))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="main.Log")]
	public partial class Log : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private System.Nullable<int> _id;
		
		private System.Nullable<int> _level;
		
		private string _location;
		
		private string _message;
		
		private System.DateTime _time;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDChanged();
		
		partial void OnIDChanging(System.Nullable<int> value);
		
		partial void OnLevelChanged();
		
		partial void OnLevelChanging(System.Nullable<int> value);
		
		partial void OnLocationChanged();
		
		partial void OnLocationChanging(string value);
		
		partial void OnMessageChanged();
		
		partial void OnMessageChanging(string value);
		
		partial void OnTimeChanged();
		
		partial void OnTimeChanging(System.DateTime value);
		#endregion
		
		
		public Log()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_id", Name="ID", DbType="INTEGER", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> ID
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((_id != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[Column(Storage="_level", Name="Level", DbType="INTEGER", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if ((_level != value))
				{
					this.OnLevelChanging(value);
					this.SendPropertyChanging();
					this._level = value;
					this.SendPropertyChanged("Level");
					this.OnLevelChanged();
				}
			}
		}
		
		[Column(Storage="_location", Name="Location", DbType="VARCHAR(255)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Location
		{
			get
			{
				return this._location;
			}
			set
			{
				if (((_location == value) 
							== false))
				{
					this.OnLocationChanging(value);
					this.SendPropertyChanging();
					this._location = value;
					this.SendPropertyChanged("Location");
					this.OnLocationChanged();
				}
			}
		}
		
		[Column(Storage="_message", Name="Message", DbType="TEXT", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Message
		{
			get
			{
				return this._message;
			}
			set
			{
				if (((_message == value) 
							== false))
				{
					this.OnMessageChanging(value);
					this.SendPropertyChanging();
					this._message = value;
					this.SendPropertyChanged("Message");
					this.OnMessageChanged();
				}
			}
		}
		
		[Column(Storage="_time", Name="Time", DbType="TIMESTAMP", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public System.DateTime Time
		{
			get
			{
				return this._time;
			}
			set
			{
				if ((_time != value))
				{
					this.OnTimeChanging(value);
					this.SendPropertyChanging();
					this._time = value;
					this.SendPropertyChanged("Time");
					this.OnTimeChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="main.Setting")]
	public partial class Setting : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private int _id;
		
		private string _key;
		
		private string _value;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDChanged();
		
		partial void OnIDChanging(int value);
		
		partial void OnKeyChanged();
		
		partial void OnKeyChanging(string value);
		
		partial void OnValueChanged();
		
		partial void OnValueChanging(string value);
		#endregion
		
		
		public Setting()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_id", Name="ID", DbType="INTEGER", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int ID
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((_id != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[Column(Storage="_key", Name="Key", DbType="VARCHAR(255)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string Key
		{
			get
			{
				return this._key;
			}
			set
			{
				if (((_key == value) 
							== false))
				{
					this.OnKeyChanging(value);
					this.SendPropertyChanging();
					this._key = value;
					this.SendPropertyChanged("Key");
					this.OnKeyChanged();
				}
			}
		}
		
		[Column(Storage="_value", Name="Value", DbType="TEXT", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (((_value == value) 
							== false))
				{
					this.OnValueChanging(value);
					this.SendPropertyChanging();
					this._value = value;
					this.SendPropertyChanged("Value");
					this.OnValueChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="main.WhoWas")]
	public partial class WhoWa : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private System.Nullable<System.DateTime> _created;
		
		private string _host;
		
		private System.Nullable<int> _id;
		
		private string _nick;
		
		private string _realName;
		
		private string _user;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnCreatedChanged();
		
		partial void OnCreatedChanging(System.Nullable<System.DateTime> value);
		
		partial void OnHostChanged();
		
		partial void OnHostChanging(string value);
		
		partial void OnIDChanged();
		
		partial void OnIDChanging(System.Nullable<int> value);
		
		partial void OnNickChanged();
		
		partial void OnNickChanging(string value);
		
		partial void OnRealNameChanged();
		
		partial void OnRealNameChanging(string value);
		
		partial void OnUserChanged();
		
		partial void OnUserChanging(string value);
		#endregion
		
		
		public WhoWa()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_created", Name="Created", DbType="TIMESTAMP", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<System.DateTime> Created
		{
			get
			{
				return this._created;
			}
			set
			{
				if ((_created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		[Column(Storage="_host", Name="Host", DbType="VARCHAR(255)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Host
		{
			get
			{
				return this._host;
			}
			set
			{
				if (((_host == value) 
							== false))
				{
					this.OnHostChanging(value);
					this.SendPropertyChanging();
					this._host = value;
					this.SendPropertyChanged("Host");
					this.OnHostChanged();
				}
			}
		}
		
		[Column(Storage="_id", Name="ID", DbType="INTEGER", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> ID
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((_id != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[Column(Storage="_nick", Name="Nick", DbType="VARCHAR(255)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Nick
		{
			get
			{
				return this._nick;
			}
			set
			{
				if (((_nick == value) 
							== false))
				{
					this.OnNickChanging(value);
					this.SendPropertyChanging();
					this._nick = value;
					this.SendPropertyChanged("Nick");
					this.OnNickChanged();
				}
			}
		}
		
		[Column(Storage="_realName", Name="RealName", DbType="VARCHAR(255)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string RealName
		{
			get
			{
				return this._realName;
			}
			set
			{
				if (((_realName == value) 
							== false))
				{
					this.OnRealNameChanging(value);
					this.SendPropertyChanging();
					this._realName = value;
					this.SendPropertyChanged("RealName");
					this.OnRealNameChanged();
				}
			}
		}
		
		[Column(Storage="_user", Name="User", DbType="VARCHAR(255)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string User
		{
			get
			{
				return this._user;
			}
			set
			{
				if (((_user == value) 
							== false))
				{
					this.OnUserChanging(value);
					this.SendPropertyChanging();
					this._user = value;
					this.SendPropertyChanged("User");
					this.OnUserChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
