/*
 * Erstellt mit SharpDevelop.
 * Benutzer: apophis
 * Datum: 21.03.2010
 * Zeit: 02:20
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Data.SQLite;

namespace IrcD.Database
{
	/// <summary>
	/// Description of DatabaseCommon.
	/// </summary>
	public class DatabaseCommon
	{	
		private static SQLiteConnection sqLiteConnection = new SQLiteConnection("Data Source=ircd.db;FailIfMissing=true;");
		
		private static Main db;
		
		public static Main Db {
			get {
                return db = db ?? new Main(sqLiteConnection);
			}
		}		
	}
}
