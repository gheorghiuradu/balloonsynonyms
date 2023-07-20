using Assets.Scripts.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SceneHelper
{
    public static GameModeSettings GameModeSettings { get; private set; }

    public static int CurrentLevel { get; set; }

    public static string GameModeFileName { get; set; }

    public static string DictionaryFileName { get; set; }

    public static List<Word> Dictionary { get; set; }

    public static string IntroConfigFileName { get; set; }

    public static event System.Action<IntroSettings> IntroSettingsLoaded;

    public static event System.Action GameModeSettingsLoaded;

    public static IEnumerator LoadLevelConfigs()
    {
        var levelFilePath = Path.Combine(Application.streamingAssetsPath, GameModeFileName);
        string json;
#if UNITY_ANDROID
        using (var request = UnityWebRequest.Get(levelFilePath))
        {
            yield return request.SendWebRequest();
            json = request.downloadHandler.text;
        }
#else
        json = File.ReadAllText(levelFilePath);
        yield return null;
#endif

        GameModeSettings = JsonUtility.FromJson<GameModeSettings>(json);

        levelFilePath = Path.Combine(Application.streamingAssetsPath, DictionaryFileName);
#if UNITY_ANDROID
        using (var request = UnityWebRequest.Get(levelFilePath))
        {
            yield return request.SendWebRequest();
            json = request.downloadHandler.text;
        }
#else
        json = File.ReadAllText(levelFilePath);
        yield return null;
#endif
        Dictionary = JsonUtility.FromJson<Dictionary>(json).Words.ToList();
        for (int i = 0; i < Dictionary.Count; i++)
        {
            Dictionary[i].Id = i;
        }

        GameModeSettingsLoaded?.Invoke();
    }

    public static List<Word> PickWords(WordPick[] wordPicks)
    {
        Dictionary.Shuffle();

        var words = new List<Word>();
        foreach (var wordPick in wordPicks)
        {
            var relevantWords = Dictionary
                            .Where(w => w.Difficutly == wordPick.Difficutly && !words.Contains(w))
                            .Take(wordPick.NumberOfWords);
            words.AddRange(relevantWords);
        }

        //TODO: We don't have enough words for this, but wen we do, we can enable it
        //foreach (var word in words)
        //{
        //    Dictionary.Remove(word);
        //}

        return words;
    }

    public static IEnumerator LoadIntroConfig()
    {
        var filePath = Path.Combine(Application.streamingAssetsPath, IntroConfigFileName);
        string json;
#if UNITY_ANDROID
        using (var request = UnityWebRequest.Get(filePath))
        {
            yield return request.SendWebRequest();
            json = request.downloadHandler.text;
        }
#else
        json = File.ReadAllText(filePath);
        yield return null;

#endif

        IntroSettingsLoaded?.Invoke(JsonUtility.FromJson<IntroSettings>(json));
    }
}