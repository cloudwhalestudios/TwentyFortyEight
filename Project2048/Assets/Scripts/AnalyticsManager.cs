using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;
    public enum LevelPlayState { InProgress, Won, Lost, Skip, Quit }

    private Scene thisScene;
    private LevelPlayState state = LevelPlayState.InProgress;
    private int score = 0;
    private float secondsSinceStart = 0;
    private int restarts = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
        }
        else
        {
            AnalyticsEvent.GameStart();
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void OnEnable()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        throw new System.NotImplementedException();
    }

    public void SetLevelPlayState(LevelPlayState newState)
    {
        this.state = newState;
    }

    public void IncreaseScore(int points)
    {
        score += points;
    }

    public void IncrementDeaths()
    {
        restarts++;
    }

    void Update()
    {
        secondsSinceStart += Time.deltaTime;
    }

    void OnDestroy()
    {
        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("seconds_since_start", secondsSinceStart);
        customParams.Add("points", score);
        customParams.Add("restarts", restarts);

        switch (this.state)
        {
            case LevelPlayState.Won:
                AnalyticsEvent.LevelComplete(thisScene.name,
                                                 thisScene.buildIndex,
                                                 customParams);
                break;
            case LevelPlayState.Lost:
                AnalyticsEvent.LevelFail(thisScene.name,
                                             thisScene.buildIndex,
                                             customParams);
                break;
            case LevelPlayState.Skip:
                AnalyticsEvent.LevelSkip(thisScene.name,
                                             thisScene.buildIndex,
                                             customParams);
                break;
            case LevelPlayState.InProgress:
            case LevelPlayState.Quit:
            default:
                AnalyticsEvent.LevelQuit(thisScene.name,
                                             thisScene.buildIndex,
                                             customParams);
                break;
        }
    }
}
