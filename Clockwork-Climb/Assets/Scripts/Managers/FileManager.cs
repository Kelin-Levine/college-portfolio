using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileManager
{
    public const string savePath = "save.dat";


    public static bool WriteToFile(string fileName, object data)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        try
        {
            File.WriteAllText(fullPath, JsonUtility.ToJson(data));
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data to {fullPath}: {e}");
        }
        return false;
    }

    public static bool LoadFromFile<T>(string fileName, out T target)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        try
        {
            target = JsonUtility.FromJson<T>(File.ReadAllText(fullPath));
            return true;
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning($"Failed to load data from {fullPath}: file not found.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data from {fullPath}: {e}");
        }
        target = default;
        return false;
    }

    public static bool DeleteFile(string fileName)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        try
        {
            File.Delete(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete {fullPath}: {e}");
        }
        return false;
    }


    [Serializable]
    public class SaveData : ISerializationCallbackReceiver
    {
        public Dictionary<string, LevelManager.LevelStats> levelStats;
        public int levelsUnlocked;
        public int[] bonusHighScores;

        public LevelManager.LevelStats[] _levelStatsArray;

        public SaveData(Dictionary<string, LevelManager.LevelStats> levelStats, int levelsUnlocked, int[] bonusHighScores)
        {
            this.levelsUnlocked = levelsUnlocked;
            this.bonusHighScores = bonusHighScores;
            this.levelStats = levelStats.Where(pair => pair.Value.levelWon).ToDictionary(i => i.Key, i => i.Value);
        }

        public void OnBeforeSerialize()
        {
            // Convert levelStats to an array because dictionaries aren't serializable
            _levelStatsArray = new LevelManager.LevelStats[levelStats.Count];
            levelStats.Values.CopyTo(_levelStatsArray, 0);
        }

        public void OnAfterDeserialize()
        {
            // Convert levelStats from an array because dictionaries aren't serializable
            levelStats = new();
            foreach (LevelManager.LevelStats levelStat in _levelStatsArray) levelStats.Add(levelStat.levelName, levelStat);
        }
    }
}
