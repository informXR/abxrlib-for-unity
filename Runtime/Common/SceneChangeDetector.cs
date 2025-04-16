﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeDetector : MonoBehaviour
{
    public static string CurrentSceneName;
    
    private void Start()
    {
        CurrentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks or unwanted calls
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }
    
    private static void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        CurrentSceneName = newScene.name;
        Debug.Log("AbxrLib - Active scene changed to " + newScene.name);
        Abxr.Event("Scene Changed", new Dictionary<string, string> { ["Scene Name"] = newScene.name });
    }
}