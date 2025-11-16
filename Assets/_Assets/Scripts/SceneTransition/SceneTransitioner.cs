using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using BeauRoutine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneTransitioner : Singleton<SceneTransitioner>
{
    [SerializeField] private Animator anim;

    public const int MAIN_MENU_INDEX = 1;
    public const int GAMEPLAY_INDEX = 2;

    public const float FADE_ANIM_DURATION = 0.5f;

    [SerializeField] private int startingBuildIndex = MAIN_MENU_INDEX;

    public static int CurrBuildIndex = 0;

    private bool didFirstLoad = false;

    private Routine loadingSceneRoutine = Routine.Null;
    private bool isLoading = false;

    private const string ANIM_PARAM_SCREENDARK = "IsDark";

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (!Application.isEditor)
            startingBuildIndex = MAIN_MENU_INDEX;

        LoadSceneWithIndex(startingBuildIndex);
    }

    public void UpdateStartingBuildIndexBasedOnOpenEditorScenes()
    {
        Scene lastLoadedScene = SceneManager.GetSceneAt(SceneManager.loadedSceneCount - 1);
        startingBuildIndex = lastLoadedScene.buildIndex;
        if (startingBuildIndex == 0)
            startingBuildIndex = MAIN_MENU_INDEX;

        Scene newScene = SceneManager.GetSceneByBuildIndex(startingBuildIndex);
        Debug.Log($"Scene to start in: \"{newScene.name}\", index: {newScene.buildIndex}");
    }

    public void LoadSceneWithIndex(int _index)
    {
        if (isLoading)
        {
            Debug.LogError("Already loading a scene!");
            return;
        }

        isLoading = true;
        loadingSceneRoutine = Routine.Start(this, WaitThenLoadScene(_index));
    }

    private IEnumerator WaitThenLoadScene(int _index)
    {
        anim.SetBool(ANIM_PARAM_SCREENDARK, true);

        if (didFirstLoad)
            yield return FADE_ANIM_DURATION;

        SceneManager.LoadScene(_index);
    }

    private void OnSceneLoaded(Scene newScene, LoadSceneMode loadSceneMode)
    {
        if(!didFirstLoad)
        {
            //Don't unhide transition UI until the next scene (usually main menu) has been loaded
            didFirstLoad = true;
            return;
        }

        anim.SetBool(ANIM_PARAM_SCREENDARK, false);

        isLoading = false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SceneTransitioner))]
public class SceneTransitioner_Editor: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Update Starting Scene"))
        {
            SceneTransitioner sceneTransitioner = (SceneTransitioner)target;
            sceneTransitioner.UpdateStartingBuildIndexBasedOnOpenEditorScenes();
        }
    }
}
#endif
