using System.Collections;
using System.Collections.Generic;
using DarkNaku.Stat;
using UnityEditor;
using UnityEngine;

public class StatMonitoringWindow : EditorWindow
{
    private Vector2 _scrollPosition;
    private float _lastTime;
    private Dictionary<ICharacterStats, bool> _foldoutCharacters = new();
    private Dictionary<IStat, bool> _foldoutStats = new();

    [MenuItem("Tools/Stats Monitoring")]
    public static void ShowWindow()
    {
        var window = GetWindow<StatMonitoringWindow>("Stat Monitoring Window");
        window.minSize = new Vector2(960, 640);
        window.maxSize = window.minSize;
    }
    
    private void OnEnable()
    {
        _lastTime = Time.realtimeSinceStartup;
        EditorApplication.update += UpdateWindow;
    }

    private void OnDisable()
    {
        EditorApplication.update -= UpdateWindow;
    }
    
    private void UpdateWindow()
    {
        float currentTime = Time.realtimeSinceStartup;
        
        if (currentTime - _lastTime >= 1f) // 1초마다 실행
        {
            Repaint(); // 윈도우 갱신
            
            _lastTime = currentTime;
        }
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(960), GUILayout.Height(640));

        foreach (var character in CharacterStatsManager.All)
        {
            EditorGUILayout.LabelField($"Character - {character.Name}");

            bool foldout;

            if (_foldoutCharacters.TryGetValue(character, out foldout) == false)
            {
                foldout = false;
                _foldoutCharacters.Add(character, foldout);
            }
            
            _foldoutCharacters[character] = EditorGUILayout.Foldout(foldout, "Stats");

            if (foldout)
            {
                EditorGUI.indentLevel++;

                foreach (var stat in character.Stats)
                {
                    EditorGUILayout.LabelField($"{stat.Name}: {stat.Value}");

                    if (_foldoutStats.TryGetValue(stat, out foldout) == false)
                    {
                        foldout = false;
                        _foldoutStats.Add(stat, foldout);
                    }

                    _foldoutStats[stat] = EditorGUILayout.Foldout(foldout, "Modifier");

                    if (foldout)
                    {

                        EditorGUI.indentLevel++;

                        var sumModifiers = stat.GetModifiers(ModifierType.Add);

                        foreach (var modifier in sumModifiers)
                        {
                            EditorGUILayout.LabelField(
                                $"ID : {modifier.ID}, Type : {modifier.Type}, Value : {modifier.Value}");
                        }

                        var percentAddModifiers = stat.GetModifiers(ModifierType.Percent);

                        foreach (var modifier in percentAddModifiers)
                        {
                            EditorGUILayout.LabelField(
                                $"ID : {modifier.ID}, Type : {modifier.Type}, Value : {modifier.Value}");
                        }

                        var percentMultiplyModifiers = stat.GetModifiers(ModifierType.Multiply);

                        foreach (var modifier in percentMultiplyModifiers)
                        {
                            EditorGUILayout.LabelField(
                                $"ID : {modifier.ID}, Type : {modifier.Type}, Value : {modifier.Value}");
                        }

                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.EndScrollView();
    }
}