using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class AutoOpenMainScene
{
    private const string MainScenePath =
        "Assets/Scenes/UkraineVsZombies.unity";

    private const string SessionKey =
        "UkraineVsZombies.MainSceneOpened";

    static AutoOpenMainScene()
    {
        EditorApplication.delayCall += TryOpenMainScene;
    }

    private static void TryOpenMainScene()
    {
        if (SessionState.GetBool(SessionKey, false))
            return;

        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += TryOpenMainScene;
            return;
        }

        SceneAsset mainScene =
            AssetDatabase.LoadAssetAtPath<SceneAsset>(MainScenePath);

        if (mainScene == null)
        {
            Debug.LogError(
                $"Не найдена главная сцена: {MainScenePath}"
            );
            return;
        }

        EditorSceneManager.playModeStartScene = mainScene;

        Scene activeScene = SceneManager.GetActiveScene();

        if (activeScene.path == MainScenePath)
        {
            SessionState.SetBool(SessionKey, true);
            return;
        }

        if (string.IsNullOrEmpty(activeScene.path))
        {
            EditorSceneManager.OpenScene(
                MainScenePath,
                OpenSceneMode.Single
            );
        }
        else
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            EditorSceneManager.OpenScene(
                MainScenePath,
                OpenSceneMode.Single
            );
        }

        SessionState.SetBool(SessionKey, true);
    }
}