using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string PlayerName;
    public List<HighScoreEntry> HighScores = new List<HighScoreEntry>();
    public int MaxHighScoreEnteries = 10;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadGameData();
    }

    [System.Serializable]
    class SaveData
    {
        public string PlayerName;
        public List<HighScoreEntry> HighScores;

        public SaveData()
        {
            HighScores = new List<HighScoreEntry>();
        }
    }

    [System.Serializable]
    public class HighScoreEntry
    {
        public string Name;
        public int Score;

        public HighScoreEntry(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }

    public void SaveGameData()
    {
        SaveData data = new SaveData();
        data.PlayerName = PlayerName;
        data.HighScores = HighScores;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadGameData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            if (data != null)
            {
                PlayerName = data.PlayerName;
                HighScores = data.HighScores ?? new List<HighScoreEntry>();
            }
        }
        else
        {
            HighScores = new List<HighScoreEntry>();
        }
    }

    public void SaveHighScore(string playerName, int score)
    {
        HighScoreEntry newEntery = new HighScoreEntry(playerName, score);
        HighScores.Add(newEntery);

        HighScores = HighScores.OrderByDescending(e => e.Score).ToList();
        if (HighScores.Count > MaxHighScoreEnteries)
        {
            HighScores.RemoveRange(MaxHighScoreEnteries, HighScores.Count - MaxHighScoreEnteries);
        }

        SaveGameData();
    }
#if UNITY_EDITOR
    [ContextMenu("Delete Save File")] // Add context menu item in editor
    public void DeleteSaveFile()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted!");
            PlayerName = ""; // Reset player name as well, optional
            HighScores.Clear(); // Clear high scores list
            SaveGameData(); // Save empty data to file
        }
        else
        {
            Debug.Log("No save file to delete.");
        }
    }
#endif

}
