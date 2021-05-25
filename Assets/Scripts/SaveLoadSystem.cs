using System;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public static class SaveLoadSystem 
{
    private static string _pathMain =  $"{Application.streamingAssetsPath}\\Levels";
    private static string _pathCustom = $"{Application.streamingAssetsPath}\\Levels\\Custom";

    public static List<LevelData> LoadMain() => Load(_pathMain);
    public static List<LevelData> LoadCustom() => Load(_pathCustom);

    public static int GetMainLevelsCount => Directory.GetFiles(_pathMain, "*.json").Length;
    public static int GetCustomLevelsCount => Directory.GetFiles(_pathCustom, "*.json").Length;

    public static void DeleteLevel(string name)
    {
        List<string> allLevels = Directory.GetFiles(_pathCustom, "*.json").ToList();
        string path = allLevels.First(x => x.Contains(name));
        allLevels.Remove(path);
        File.Delete(path);
        
        for(int i = 0; i < allLevels.Count; i++)
        {
            try
            {
                string fileString;
                
                using (FileStream stream = new FileStream(allLevels[i], FileMode.Open))
                {
                    StreamReader reader = new StreamReader(stream);
                    fileString = reader.ReadToEnd();
                    reader.Close();
                }
                
                LevelData data = JsonUtility.FromJson<LevelData>(fileString);
                data.levelNumber = i + 1;
                
                using (FileStream stream = new FileStream(allLevels[i], FileMode.Open))
                {
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(JsonUtility.ToJson(data));
                    writer.Close();
                }
                
                File.Move(allLevels[i], Path.Combine(_pathCustom, data.levelNumber + data.levelName + ".json"));
            }
            catch(Exception e )
            {
                Debug.LogError($"An error occured on deleting files.\n{e}");
            }
        }

        #if UNITY_EDITOR
            AssetDatabase.Refresh();
        #endif
    }
    
    private static List<LevelData> Load(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);   
        }

        string[] levels = Directory.GetFiles(path, "*.json");
        List<LevelData> data = new List<LevelData>();

        if (levels.Length == 0)
        {
            return data;
        }
        
        foreach (var level in levels)
        {
            try
            {
                string jsonString;
                using (FileStream stream = new FileStream(level, FileMode.Open))
                {
                    StreamReader reader = new StreamReader(stream);
                    jsonString = reader.ReadToEnd();
                    reader.Close();
                }
                data.Add(JsonUtility.FromJson<LevelData>(jsonString));
            }
            catch(Exception e )
            {
                Debug.LogError($"An error occured on loading files.\n{e}");
            }
        }
       
        return data;
    }

    public static void Save(LevelData data)
    {
        try
        {
            string jsonString = JsonUtility.ToJson(data);
            string file = $"{_pathCustom}\\{data.levelNumber}{data.levelName}.json";
            using (FileStream stream = new FileStream(file, FileMode.Create))
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(jsonString);
                writer.Close();
            }
        }
        catch(Exception e )
        {
            Debug.LogError($"An error occured on saving files.\n{e}");
        }
       
        #if UNITY_EDITOR
            AssetDatabase.Refresh();
        #endif
    }
}

[Serializable]
public class LevelData
{
    public int levelNumber;
    public string levelName;
    public Vector2Int gridSize;
    public int[] gridArray;
    public Vector2Int startPlayerPosition;
    public int totalEnemies;
    public int enemiesPortionCount;
    public uint enemyHp;
    public uint playerHp;
    public uint bigBullets;
}