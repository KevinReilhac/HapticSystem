using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HapticSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class HapticClipInstanceList : EditorWindow
{
    [MenuItem("Tools/Haptic/HapticClipInstanceList")]
    public static void ShowWindow()
    {
        GetWindow<HapticClipInstanceList>("HapticClipInstanceList");
    }

    private Vector2 scrollPosition;
    private bool autoRefresh = true;
    private float lastRefreshTime;
    private const float REFRESH_INTERVAL = 0.1f; // Refresh every 100ms
    
    // Column widths
    private const float CLIP_NAME_WIDTH = 150f;
    private const float GAMEPAD_INDEX_WIDTH = 80f;
    private const float IS_PLAYING_WIDTH = 70f;
    private const float PROGRESS_WIDTH = 100f;
    private const float STRENGTH_WIDTH = 80f;
    private const float LOW_FREQ_WIDTH = 80f;
    private const float HIGH_FREQ_WIDTH = 80f;
    private const float SPEED_WIDTH = 80f;
    private const float ACTIONS_WIDTH = 100f;

    private void OnEnable()
    {
        // Enable auto-refresh when window opens
        EditorApplication.update += OnEditorUpdate;
        
        // Force updates even when window is not focused
        wantsMouseMove = true;
        autoRepaintOnSceneChange = true;
        wantsLessLayoutEvents = false;
    }

    private void OnDisable()
    {
        // Disable auto-refresh when window closes
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        if (autoRefresh && Time.realtimeSinceStartup - lastRefreshTime > REFRESH_INTERVAL)
        {
            lastRefreshTime = Time.realtimeSinceStartup;
            // Force repaint even if window is not focused
            Repaint();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        
        // Header controls
        DrawHeader();
        
        // Column headers
        DrawColumnHeaders();
        
        // Clip instances list
        DrawClipInstancesList();
        
        EditorGUILayout.EndVertical();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        
        GUILayout.Label($"Active Haptic Clip Instances: {GetTotalInstanceCount()}", EditorStyles.boldLabel);
        
        GUILayout.FlexibleSpace();
        
        // Auto-refresh toggle
        autoRefresh = GUILayout.Toggle(autoRefresh, "Auto Refresh", EditorStyles.toolbarButton);
        
        // Manual refresh button
        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
        {
            Repaint();
        }
        
        // Stop all button
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Stop All", EditorStyles.toolbarButton))
        {
            HapticManager.StopAllClipInstances();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawColumnHeaders()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        
        GUILayout.Label("Clip Name", EditorStyles.toolbarButton, GUILayout.Width(CLIP_NAME_WIDTH));
        GUILayout.Label("Gamepad", EditorStyles.toolbarButton, GUILayout.Width(GAMEPAD_INDEX_WIDTH));
        GUILayout.Label("Playing", EditorStyles.toolbarButton, GUILayout.Width(IS_PLAYING_WIDTH));
        GUILayout.Label("Progress", EditorStyles.toolbarButton, GUILayout.Width(PROGRESS_WIDTH));
        GUILayout.Label("Strength", EditorStyles.toolbarButton, GUILayout.Width(STRENGTH_WIDTH));
        GUILayout.Label("Low Freq", EditorStyles.toolbarButton, GUILayout.Width(LOW_FREQ_WIDTH));
        GUILayout.Label("High Freq", EditorStyles.toolbarButton, GUILayout.Width(HIGH_FREQ_WIDTH));
        GUILayout.Label("Speed", EditorStyles.toolbarButton, GUILayout.Width(SPEED_WIDTH));
        GUILayout.Label("Actions", EditorStyles.toolbarButton, GUILayout.Width(ACTIONS_WIDTH));
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawClipInstancesList()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        var playingClips = GetAllPlayingClips();
        
        if (playingClips.Count == 0)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("No active haptic clip instances", EditorStyles.centeredGreyMiniLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            foreach (var clipInstance in playingClips)
            {
                DrawClipInstanceRow(clipInstance);
            }
        }
        
        EditorGUILayout.EndScrollView();
    }

    private void DrawClipInstanceRow(HapticClipInstance clipInstance)
    {
        EditorGUILayout.BeginHorizontal();
        
        // Clip Name
        string clipName = clipInstance.clip != null ? clipInstance.clip.name : "Unknown";
        GUILayout.Label(clipName, GUILayout.Width(CLIP_NAME_WIDTH));
        
        // Gamepad Index
        string gamepadText = clipInstance.targetGamepadIndex == -1 ? "All" : clipInstance.targetGamepadIndex.ToString();
        GUILayout.Label(gamepadText, GUILayout.Width(GAMEPAD_INDEX_WIDTH));
        
        // Is Playing
        Color originalColor = GUI.color;
        GUI.color = clipInstance.isPlaying ? Color.green : Color.red;
        GUILayout.Label(clipInstance.isPlaying ? "Playing" : "Stopped", GUILayout.Width(IS_PLAYING_WIDTH));
        GUI.color = originalColor;
        
        // Progress
        EditorGUI.ProgressBar(
            GUILayoutUtility.GetRect(PROGRESS_WIDTH, EditorGUIUtility.singleLineHeight), 
            clipInstance.progress, 
            $"{(clipInstance.progress * 100f):F1}%"
        );

        clipInstance.EvaluateStrenghts(out float lowFrequency, out float highFrequency);
        
        // Strength Multiplier
        EditorGUI.ProgressBar(
            GUILayoutUtility.GetRect(STRENGTH_WIDTH, EditorGUIUtility.singleLineHeight), 
            clipInstance.strenghtMultiplier, 
            $"{(clipInstance.strenghtMultiplier * 100f):F1}%"
        );
        
        // Low Frequency Multiplier
        EditorGUI.ProgressBar(
            GUILayoutUtility.GetRect(LOW_FREQ_WIDTH, EditorGUIUtility.singleLineHeight), 
            lowFrequency, 
            $"{(lowFrequency * 100f):F1}%"
        );
        
        // High Frequency Multiplier
        EditorGUI.ProgressBar(
            GUILayoutUtility.GetRect(HIGH_FREQ_WIDTH, EditorGUIUtility.singleLineHeight), 
            highFrequency, 
            $"{(highFrequency * 100f):F1}%"
        );
        
        // Speed Multiplier
        EditorGUILayout.FloatField(clipInstance.speedMultiplier, GUILayout.Width(SPEED_WIDTH));
        
        // Actions
        EditorGUILayout.BeginHorizontal(GUILayout.Width(ACTIONS_WIDTH));
        
        if (clipInstance.isPlaying)
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Stop", GUILayout.Width(50)))
            {
                HapticManager.StopClipInstance(clipInstance);
            }
            GUI.backgroundColor = Color.white;
        }
        else
        {
            GUI.backgroundColor = Color.gray;
            GUILayout.Button("Stopped", GUILayout.Width(50));
            GUI.backgroundColor = Color.white;
        }
        
        // Select clip asset button
        if (clipInstance.clip != null)
        {
            if (GUILayout.Button("→", GUILayout.Width(25)))
            {
                Selection.activeObject = clipInstance.clip;
                EditorGUIUtility.PingObject(clipInstance.clip);
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndHorizontal();
        
        // Add separator line
        GUILayout.Space(2);
        EditorGUI.DrawRect(GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true)), Color.gray);
        GUILayout.Space(2);
    }

    private List<HapticClipInstance> GetAllPlayingClips()
    {
        return HapticManager.playingClips.Values.SelectMany(clips => clips).ToList();
    }

    private int GetTotalInstanceCount()
    {
        return GetAllPlayingClips().Count;
    }
}
