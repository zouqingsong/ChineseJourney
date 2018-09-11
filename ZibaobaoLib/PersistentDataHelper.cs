using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using ZibaobaoLib.Helpers;

namespace ZibaobaoLib
{
    public class SecureSqLiteConnectionWithLock : SQLiteConnectionWithLock
    {
        public SecureSqLiteConnectionWithLock(SQLiteConnectionString connectionString, SQLiteOpenFlags openFlags) 
            : base(connectionString, openFlags)
        {
        }
    }
    public class PersistentDataHelper
    {
        static PersistentDataHelper _instance;

        public const string MemoryDB = ":memory:";

        public const string DataDB = "ChineseJourneyQuestions.db3";

        Dictionary<string, SQLiteConnectionWithLock> _connections = new Dictionary<string, SQLiteConnectionWithLock>();
        object _connectionLock = new object();
        protected PersistentDataHelper()
        {
        }
        public static PersistentDataHelper Instance => _instance ?? (_instance = new PersistentDataHelper());

        public SQLiteConnectionWithLock GetConnection(bool isGlobal, string dbName)
        {
            lock (_connectionLock)
            {
                try
                {
                    string connectionKey = GetDataBasePath(isGlobal, dbName);
                    SQLiteConnectionWithLock connection;
                    if (!_connections.TryGetValue(connectionKey, out connection))
                    {
                        connection = new SecureSqLiteConnectionWithLock(
                            new SQLiteConnectionString(connectionKey, true), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
                        _connections[connectionKey] = connection;
                    }
                    return connection;
                }
                catch (TypeInitializationException e)
                {
                    X1LogHelper.Exception(e);
                    X1LogHelper.Error("Please try to rebuild entire service package when this happens");
                    throw;
                }
            }
        }

        public void RemoveConnection(bool isGlobal, string dbName)
        {
            lock (_connectionLock)
            {
                try
                {
                    string connectionKey = GetDataBasePath(isGlobal, dbName);
                    if (_connections.ContainsKey(connectionKey))
                    {
                        _connections.Remove(connectionKey);
                    }
                }
                catch (Exception e)
                {
                    X1LogHelper.Exception(e);
                    throw;
                }
            }
        }
        public string DataPath => Storage.DataPath;

        public string ServerID { get; set; }

        public IPersistentStorage Storage => ZibaobaoLibContext.Instance.PersistentStorage;

        public string GetDataBasePath(bool isGlobal,string dbName)
        {
            if (dbName == MemoryDB)
            {
                return dbName;
            }
            string dbPath = dbName;
            if (!Path.IsPathRooted(dbName))
            {
                dbPath = Path.Combine(isGlobal || string.IsNullOrEmpty(ServerID) ? DataPath : Path.Combine(DataPath, ServerID), dbName);
            }
            if (!Storage.Exists(dbPath))
            {
                ZibaobaoLibContext.Instance.IsFirstTimeStart = true;
                Storage.Create(dbPath);
            }
            return dbPath;
        }

        public string EntityPath
        {
            get
            {
                string path = Path.Combine(DataPath, "entity");
                Storage.CreateDirectory(path);
                return path;
            }
        }

        public string CachePath
        {
            get
            {
                string path = Path.Combine(DataPath, "cache");
                Storage.CreateDirectory(path);
                return path;
            }
        }
    }

    public interface IEntityManager<T> where T : DataEntity, new()
    {
        EntityDB<T> Data { get;}
    }
    public class RecordItem
    {
        public int Num { get; set; }
    }

    public interface IEntityDB
    {
        void InitDB();
        string DbPath { get; }

        bool Exist(string key);
        SQLiteConnection Connection { get; }
        string TableName { get; }
        string PrimaryKey { get; }
        List<T> Query<T>(string sql) where T: new();
        TableQuery<T> Table<T>() where T : new();
        List<T> ItemsLike<T>(string likeClause, string field = "", string order = "") where T: new();
        List<T> ItemsLikeAny<T>(string id, string field="", string order = "") where T:new();
        void ExecuteRepeat(string query, params object[] args);
        List<object[]> Query(string sql, bool includeColumnNamesAsFirstRow=false);
        int QueryStatistics(string sql, string itemToQuery);
        int Count(string sql = "");
        long GetTimeStampFromRecord(string id);
    }
    public class EntityDB<T> : IEntityDB, IDisposable where T: DataEntity, new()
    {
        string _dbName;
        bool _isGlobal;
        object _dbLock = new object();
        public const int OneUpdateRecordNum = 300;
        public EntityDB(bool autoInit = true, bool isGlobal=false, string dbName= PersistentDataHelper.DataDB)
        {
            _dbName = dbName;
            _isGlobal = isGlobal;
            if (autoInit)
            {
                InitDB();
            }
        }

        public int QueryStatistics(string sql, string itemToQuery)
        {
            string countSql = $"select {itemToQuery} as Num from (" + sql + ")";
            var result = Query<RecordItem>(countSql);
            if (result != null && result.Count >= 1)
            {
                return result[0].Num;
            }
            return 0;
        }

        public int Count(long timeStamp)
        {
            return Count(ConstructSqlAllBefore(timeStamp));
        }
        public int Count(string sql = "")
        {
            if (string.IsNullOrEmpty(sql))
            {
                sql = TableName;
            }
            int count = QueryStatistics(sql, "count(1)");
            X1LogHelper.Verbose($"Count {sql} [{count}]");
            return count;
        }

        public void InitDB()
        {
            lock (_dbLock)
            {
                int reTry = 3;
                X1LogHelper.Log($"InitDB [{_dbName}] [{typeof(T).FullName}]");
                Exception ex = null;
                while (reTry-- > 0)
                {
                    try
                    {
                        Connection = PersistentDataHelper.Instance.GetConnection(_isGlobal, _dbName);
                        Connection?.CreateTable<T>();
                        return;
                    }
                    catch (Exception e)
                    {
                        ex = e;
                    }
                }
                X1LogHelper.Error("InitDB failed");
                if (ex != null)
                {
                    X1LogHelper.Exception(ex);
                }
            }
        }
        public void CopyFrom(EntityDB<T> sourceDb)
        {
            Connection.InsertAll(sourceDb.Data);
        }
        public bool UseRaw { get; set; }
        public string DbPath => PersistentDataHelper.Instance.GetDataBasePath(_isGlobal, _dbName);
        public string DbFolder => PersistentDataHelper.Instance.DataPath;
        public SQLiteConnection Connection { get; private set;}
        public TableQuery<T> Data => Table<T>();
        public string TableName => Connection?.GetMapping<T>()?.TableName;
        public List<T> ItemsLikeAny<T>(string id, string field="", string order = "") where T:new()
        {
            return ItemsLike<T>("%" + id + "%", field, order);
        }

        public TableQuery<TT> Table<TT>() where TT : new()
        {
            return Connection?.Table<TT>();
        }

        public List<TT> Query<TT>(string sql) where TT: new()
        {
            try{
                return Connection?.Query<TT>(sql);
            }
            catch{
                return new List<TT>();
            }

        }
        public List<T> ItemsLike<T>(string likeClause, string field, string order="") where T: new()
        {
            if (string.IsNullOrEmpty(field))
            {
                field = PrimaryKey;
            }
            return Data.Connection.Query<T>($"select * from {TableName} where {field} like '{likeClause}' " + (!string.IsNullOrEmpty(order)?order:""));
        }
        public IReadOnlyCollection<T> All => Data?.ToList();

        public long GetTimeStampFromRecord(string id)
        {
            return (!string.IsNullOrEmpty(id) ? Get(id) : default(T))?.TimeStamp.Ticks ?? 0;
        }

        public IReadOnlyCollection<T> QueryWithLimit(string sql, int limit = -1)
        {
            if (limit > 0)
            {
                sql += " LIMIT " + limit;
            }
            return Data.Connection.Query<T>(sql);
        }

        public string ConstructSqlAllAfter(long timeStamp, bool reverseOrder=false)
        {
            string sql = $"select * from {TableName}";
            if (timeStamp > 0)
            {
                sql += $" where TimeStamp > {timeStamp}";
            }

            sql += " ORDER BY TimeStamp";
            if (reverseOrder)
            {
                sql += " DESC";
            }
            return sql;
        }

        public string ConstructSqlAllBefore(long timeStamp, bool reverseOrder = false)
        {
            string sql = $"select * from {TableName}";
            if (timeStamp > 0)
            {
                sql += $" where TimeStamp <= {timeStamp}";
            }

            sql += " ORDER BY TimeStamp";
            if (reverseOrder)
            {
                sql += " DESC";
            }
            return sql;
        }

        public void ExecuteRepeat(string query, params object[] args)
        {
            int repeats = 3;
            while (repeats-- > 0)
            {
                try
                {
                    Connection.Execute(query, args);
                    break;
                }
                catch (Exception e)
                {
                    Task.Delay(100);
                }
            }
        }
        public List<object[]> Query(string sql, bool includeColumnNamesAsFirstRow=false)
        {
            var lstRes = new List<object[]>();
            SQLitePCL.sqlite3_stmt stQuery = null;
            try
            {
                stQuery = SQLite3.Prepare2(Connection?.Handle, sql);
                var colLenght = SQLite3.ColumnCount(stQuery);

                if (includeColumnNamesAsFirstRow)
                {
                    var obj = new object[colLenght];
                    lstRes.Add(obj);
                    for (int i = 0; i < colLenght; i++)
                    {
                        obj[i] = SQLite3.ColumnName(stQuery, i);
                    }
                }

                while (SQLite3.Step(stQuery) == SQLite3.Result.Row)
                {
                    var obj = new object[colLenght];
                    lstRes.Add(obj);
                    for (int i = 0; i < colLenght; i++)
                    {
                        var colType = SQLite3.ColumnType(stQuery, i);
                        switch (colType)
                        {
                            case SQLite3.ColType.Blob:
                                obj[i] = SQLite3.ColumnBlob(stQuery, i);
                                break;
                            case SQLite3.ColType.Float:
                                obj[i] = SQLite3.ColumnDouble(stQuery, i);
                                break;
                            case SQLite3.ColType.Integer:
                                obj[i] = SQLite3.ColumnInt(stQuery, i);
                                break;
                            case SQLite3.ColType.Null:
                                obj[i] = null;
                                break;
                            case SQLite3.ColType.Text:
                                obj[i] = SQLite3.ColumnString(stQuery, i);
                                break;
                        }
                    }
                }
                return lstRes;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (stQuery != null)
                {
                    SQLite3.Finalize(stQuery);
                }
            }
        }
        public IReadOnlyCollection<string> AllIds
        {
            get
            {
                List<string> ids = new List<string>();
                if (Data != null)
                {
                    var mapping = Connection.GetMapping<T>();
                    foreach (var item in Data)
                    {
                        ids.Add(mapping.PK.GetValue(item).ToString());
                    }
                }
                return ids;
            }
        }
        public string GetPKValue(T item)
        {
            try
            {
                var mapping = Connection.GetMapping<T>();
                var PK = mapping.PK.GetValue(item);
                if (PK != null)
                {
                    return PK.ToString();
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        public string PrimaryKey
        {
            get
            {
                var mapping = Connection.GetMapping<T>();
                return mapping.PK.Name;
            }
        }
        Dictionary<string, int> _autoNumbers = new Dictionary<string, int>();
        public string GetNextAutoKey(string prefix, string fieldName)
        {
            int num = 0;
            string key = prefix + fieldName;
            if (_autoNumbers.ContainsKey(key))
            {
                num = _autoNumbers[key];
                num++;
            }
            else
            {
                var existingIDs = Query<T>($"select {fieldName} as ID from {TableName} where {fieldName} like '{prefix}%' ORDER BY {fieldName} DESC Limit 1");
                if (existingIDs != null && existingIDs.Count > 0)
                {
                    var id = existingIDs[0].ID;
                    int index = id.IndexOf(prefix, StringComparison.Ordinal);
                    if (index >= 0)
                    {
                        id = id.Substring(index + prefix.Length);
                        id = id.Trim(' ', '_', ' ');
                        if (!string.IsNullOrEmpty(id))
                        {
                            if (int.TryParse(id, out num))
                            {
                                num++;
                            }
                            else
                            {
                                num = 0;
                            }
                        }
                    }
                }
            }

            if (num < 0)
            {
                num = 0;
            }
            string name;
            while (true)
            {
                name = DataEntity.GetAutoKey(prefix, num);
                var results = Query($"select ID from {TableName} where {fieldName} = '{name}'");
                if (results == null || results.Count <= 0)
                {
                    break;
                }
                num++;
            }
            _autoNumbers[key] = num;
            return name;
        }

        void CheckDateTime(T entity)
        {
            var dataEntity = entity as DataEntity;
            if (dataEntity != null)
            {
                if (string.IsNullOrEmpty(dataEntity.ID))
                {
                    dataEntity.ID = Guid.NewGuid().ToString();
                }
                if (dataEntity.TimeStamp.Year < 1980)
                {
                    dataEntity.TimeStamp = DateTime.Now;
                }

                if (dataEntity.CreateTimeStamp.Year < 1980)
                {
                    dataEntity.CreateTimeStamp = dataEntity.TimeStamp;
                }
            }
        }
        public virtual bool Add(T entity)
        {
            try
            {
                CheckDateTime(entity);
                Connection.Insert(entity);
                return true;
            }
            catch (Exception ex)
            {
                X1LogHelper.Exception(ex);
                return false;
            }

        }

        public virtual bool Delete(string key)
        {
            var ret = Connection.Delete<T>(key);
            return ret > 0;
        }

        public virtual bool Update(T entity)
        {
            if (entity == null)
            {
                return false;
            }
            try
            {
                var existingEntity = Get(entity.ID);
                if (existingEntity != null)
                {
                    entity.CreateTimeStamp = existingEntity.CreateTimeStamp;
                }
                CheckDateTime(entity);
                Connection.Update(entity);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public virtual bool AddOrUpdate(T entity)
        {
            return Exist(GetPKValue(entity))? Update(entity) : Add(entity);
        }

        public T Get(string key)
        {
            try
            {
                return Connection.Get<T>(key);
            }
            catch (Exception)
            {
            }
            return default(T);
        }

        public bool Exist(string key)
        {
            try
            {
                var result = Connection.Query<T>("SELECT * FROM " + TableName + " WHERE " + PrimaryKey +"='" + key + "'");
                return result != null && result.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public T LastRecord
        {
            get
            {
                try
                {
                    var list = Connection.Query<T>("SELECT * FROM " + TableName + " ORDER BY TimeStamp DESC LIMIT 1");
                    var result = list?.ToList();
                    if (result?.Count > 0)
                    {
                        return result[0];
                    }
                }
                catch (Exception ex)
                {
                    
                }
                return default(T);
            }
        }

        public T FirstRecord
        {
            get
            {
                try
                {
                    var list = Connection.Query<T>("SELECT * FROM " + TableName + " ORDER BY TimeStamp LIMIT 1");
                    if(list != null)
                    {
                        var result = list.ToList();
                        if (result.Count > 0)
                        {
                            return result[0];
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                return default(T);
            }
        }

        public void Dispose()
        {
            try
            {
                Connection.Close();
            }
            catch (Exception e)
            {
                X1LogHelper.Exception(e);
            }
            PersistentDataHelper.Instance.RemoveConnection(_isGlobal, _dbName);
        }
    }
}

