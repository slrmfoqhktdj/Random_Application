using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

using System;
using System.IO;
using UnityEngine.Networking;

public class SQLiteCon : MonoBehaviour
{
    private static SQLiteCon instance;
    public static SQLiteCon Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SQLiteCon");
                instance = go.AddComponent<SQLiteCon>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private string dbFilePath;
    public bool isInitialized = false;

    void Awake()
    {
        // 중복 인스턴스 방지
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // DB 초기화
        StartCoroutine(InitializeDatabase());
    }

    // 데이터베이스 초기화 (비동기)
    private IEnumerator InitializeDatabase()
    {
        yield return StartCoroutine(DBCreator());
        DBConnectionCheck();
        isInitialized = true;
    }

    // 초기화 완료 여부 확인
    public bool IsReady() => isInitialized;

    // 시작시 db파일 플랫폼에 맞게 쓰기가능경로로 복사
    private IEnumerator DBCreator()
    {
        
        //Debug.Log("DBCreator");

        if (Application.platform == RuntimePlatform.Android)
        {
            dbFilePath = Application.persistentDataPath + "/NumList.db";
            if (!File.Exists(dbFilePath))
            {
                UnityWebRequest unityWebRequest = UnityWebRequest.Get("jar:file://" + Application.dataPath + "!/assets/NumList.db");
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllBytes(dbFilePath, unityWebRequest.downloadHandler.data);
                    Debug.Log("SQLiteCon: Database file copied to Android persistent path");
                }
                else
                {
                    Debug.LogError("SQLiteCon: Failed to copy database file - " + unityWebRequest.error);
                }
            }
        }
        else
        {
            dbFilePath = Application.dataPath + "/NumList.db";
            if (!File.Exists(dbFilePath))
            {
                string sourcePath = Application.streamingAssetsPath + "/NumList.db";
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, dbFilePath);
                    Debug.Log("SQLiteCon: Database file copied to data path");
                }
                else
                {
                    Debug.LogError("SQLiteCon: Source database file not found at " + sourcePath);
                }
            }
        }
    }

    // 생성된 db 파일 경로 리턴
    private string GetDBFilePath()
    {
        return "URI=file:" + dbFilePath;
    }

    // db연결상태 확인
    public void DBConnectionCheck()
    {
        try
        {
            using (IDbConnection dbConnection = new SqliteConnection(GetDBFilePath()))
            {
                dbConnection.Open();
                if (dbConnection.State == ConnectionState.Open)
                {
                    Debug.Log("SQLiteCon: Database connection successful");
                }
                else
                {
                    Debug.LogError("SQLiteCon: Database connection failed");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SQLiteCon: Database connection error - " + e.Message);
        }
    }

    // db읽어오기 (디버그용)
    public void DBRead(string query)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("SQLiteCon: Database not initialized yet");
            return;
        }

        try
        {
            using (IDbConnection dbConnection = new SqliteConnection(GetDBFilePath()))
            {
                dbConnection.Open();
                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    dbCommand.CommandText = query;
                    using (IDataReader dataReader = dbCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Debug.Log("SQLiteCon: DBRead() = " + 
                                dataReader.GetInt32(0) + ", " + 
                                dataReader.GetInt32(1) + ", " + 
                                dataReader.GetInt32(2) + ", " + 
                                dataReader.GetInt32(3) + ", " + 
                                dataReader.GetInt32(4) + ", " + 
                                dataReader.GetInt32(5) + ", " + 
                                dataReader.GetInt32(6));
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SQLiteCon: DBRead error - " + e.Message);
        }
    }

    // db 초기에 저장된 리스트 전부 읽어오기
    public List<List<int>> DBReadAll()
    {
        List<List<int>> dataList = new List<List<int>>();

        if (!isInitialized)
        {
            Debug.LogWarning("SQLiteCon: Database not initialized yet");
            return dataList;
        }

        try
        {
            using (IDbConnection dbConnection = new SqliteConnection(GetDBFilePath()))
            {
                dbConnection.Open();
                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    dbCommand.CommandText = "Select N1,N2,N3,N4,N5,N6,N7 From NumList";
                    using (IDataReader dataReader = dbCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            dataList.Insert(0,new List<int>()
                            {
                                dataReader.GetInt32(0),
                                dataReader.GetInt32(1),
                                dataReader.GetInt32(2),
                                dataReader.GetInt32(3),
                                dataReader.GetInt32(4),
                                dataReader.GetInt32(5),
                                dataReader.GetInt32(6)
                            });
                        
                        }
                    }
                }
            }
            Debug.Log($"SQLiteCon: Loaded {dataList.Count} records from database");
        }
        catch (Exception e)
        {
            Debug.LogError("SQLiteCon: DBReadAll error - " + e.Message);
        }

        return dataList;
    }

    // db 리스트 추가하기
    public bool DBAddList(List<int> nlist)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("SQLiteCon: Database not initialized yet");
            return false;
        }

        if (nlist == null || nlist.Count != 7)
        {
            Debug.LogError("SQLiteCon: Invalid list format - must contain exactly 7 integers");
            return false;
        }

        try
        {
            using (var dbConnection = new SqliteConnection(GetDBFilePath()))
            {
                dbConnection.Open();
                string sql = "Insert Into NumList (N1,N2,N3,N4,N5,N6,N7) Values (@N1,@N2,@N3,@N4,@N5,@N6,@N7)";

                using (var command = new SqliteCommand(sql, dbConnection))
                {
                    command.Parameters.AddWithValue("@N1", nlist[0]);
                    command.Parameters.AddWithValue("@N2", nlist[1]);
                    command.Parameters.AddWithValue("@N3", nlist[2]);
                    command.Parameters.AddWithValue("@N4", nlist[3]);
                    command.Parameters.AddWithValue("@N5", nlist[4]);
                    command.Parameters.AddWithValue("@N6", nlist[5]);
                    command.Parameters.AddWithValue("@N7", nlist[6]);

                    int rowsAffected = command.ExecuteNonQuery();
                    Debug.Log($"SQLiteCon: Successfully added list to database");
                    return rowsAffected > 0;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SQLiteCon: DBAddList error - " + e.Message);
            return false;
        }
    }

    // db 아이템 삭제하기
    public bool DBDeleteItem(List<int> nlist)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("SQLiteCon: Database not initialized yet");
            return false;
        }

        if (nlist == null || nlist.Count != 7)
        {
            Debug.LogError("SQLiteCon: Invalid list format - must contain exactly 7 integers");
            return false;
        }

        try
        {
            using (var dbConnection = new SqliteConnection(GetDBFilePath()))
            {
                dbConnection.Open();
                string sql = "Delete From NumList Where N1 = @N1 And N2 = @N2 And N3 = @N3 And N4 = @N4 And N5 = @N5 And N6 = @N6 And N7 = @N7";

                using (var command = new SqliteCommand(sql, dbConnection))
                {
                    command.Parameters.AddWithValue("@N1", nlist[0]);
                    command.Parameters.AddWithValue("@N2", nlist[1]);
                    command.Parameters.AddWithValue("@N3", nlist[2]);
                    command.Parameters.AddWithValue("@N4", nlist[3]);
                    command.Parameters.AddWithValue("@N5", nlist[4]);
                    command.Parameters.AddWithValue("@N6", nlist[5]);
                    command.Parameters.AddWithValue("@N7", nlist[6]);

                    int rowsAffected = command.ExecuteNonQuery();
                    Debug.Log($"SQLiteCon: Deleted {rowsAffected} record(s) from database");
                    return rowsAffected > 0;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SQLiteCon: DBDeleteItem error - " + e.Message);
            return false;
        }
    }

    // 레코드 개수 조회
    public int GetRecordCount()
    {
        if (!isInitialized) return 0;

        try
        {
            using (var dbConnection = new SqliteConnection(GetDBFilePath()))
            {
                dbConnection.Open();
                using (var command = new SqliteCommand("Select Count(*) From NumList", dbConnection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SQLiteCon: GetRecordCount error - " + e.Message);
            return 0;
        }
    }

    // 모든 레코드 삭제
    public bool ClearAllData()
    {
        if (!isInitialized) return false;

        try
        {
            using (var dbConnection = new SqliteConnection(GetDBFilePath()))
            {
                dbConnection.Open();
                using (var command = new SqliteCommand("Delete From NumList", dbConnection))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    Debug.Log($"SQLiteCon: Cleared {rowsAffected} record(s) from database");
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SQLiteCon: ClearAllData error - " + e.Message);
            return false;
        }
    }
}
