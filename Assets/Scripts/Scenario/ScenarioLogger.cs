using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class ScenarioLogEntry
{
    public int step;
    public string timestamp;
    public string eventType;
    public string nodeId;
    public string target;
    public string details;
    public int score;
}

[Serializable]
public class ScenarioLogFile
{
    public List<ScenarioLogEntry> events =
        new List<ScenarioLogEntry>();
}

public class ScenarioLogger : MonoBehaviour
{
    [Header("Log Data")]
    public ScenarioLogFile logFile =
        new ScenarioLogFile();

    private int stepCounter = 0;

    public void LogEvent(
        string eventType,
        string nodeId,
        string target,
        string details,
        int score)
    {
        stepCounter++;

        ScenarioLogEntry entry =
            new ScenarioLogEntry();

        entry.step = stepCounter;

        entry.timestamp =
            DateTime.Now.ToString(
                "dd/MM/yyyy HH:mm:ss"
            );

        entry.eventType = eventType;
        entry.nodeId = nodeId;
        entry.target = target;
        entry.details = details;
        entry.score = score;

        logFile.events.Add(entry);

        Debug.Log(
            "[SCENARIO LOG] " +
            entry.step + " | " +
            entry.timestamp + " | " +
            entry.eventType + " | " +
            entry.nodeId + " | " +
            entry.target + " | " +
            entry.details + " | Score: " +
            entry.score
        );
    }

    public void ExportJSON()
    {
        string json =
            JsonUtility.ToJson(
                logFile,
                true
            );

        string fileName =
            "ScenarioLog_" +
            DateTime.Now.ToString(
                "yyyyMMdd_HHmmss"
            ) +
            ".json";

        string exportFolder;

#if UNITY_EDITOR

        
        exportFolder =
            Application.persistentDataPath;

#else

        
        exportFolder =
            Path.Combine(
                Application.dataPath,
                "ScenarioLogs"
            );

#endif

        
        if (!Directory.Exists(exportFolder))
        {
            Directory.CreateDirectory(exportFolder);
        }

        string path =
            Path.Combine(
                exportFolder,
                fileName
            );

        try
        {
            File.WriteAllText(
                path,
                json
            );

            Debug.Log(
                "Scenario log exported to: " +
                path
            );
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "Failed to export scenario log.\n" +
                ex.Message
            );
        }
    }

    public void ClearLog()
    {
        logFile.events.Clear();
        stepCounter = 0;
    }
}