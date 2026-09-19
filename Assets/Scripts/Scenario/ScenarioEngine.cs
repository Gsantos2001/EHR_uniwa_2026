using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System.IO;

public class ScenarioEngine : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("Patient Vitals.")]
    public PatientVitals patientVitals;

    [Tooltip("EHRManager.")]
    public EHRManager ehrManager;

    [Header("Scenario Data")]
    [Tooltip("Όνομα του JSON αρχείου μέσα στο StreamingAssets.")]
    public string scenarioFileName = "icu_scenario.json";

    [Header("Logging")]
    public ScenarioLogger scenarioLogger;

    [Header("Debrief")]
    public DebriefManager debriefManager;

    private ScenarioData currentScenario;
    private Dictionary<string, ScenarioNode> nodeDictionary;
    private ScenarioNode currentNode;

    public int CurrentScore
    {
        get
        {
            return currentScore;
        }
    }

    public string CurrentNodeId
    {
        get
        {
            return currentNode != null ? currentNode.id : "";
        }
    }

    public string CurrentNodeType
    {
        get
        {
            return currentNode != null ? currentNode.type : "";
        }
    }

    // Game State
    private int currentScore;
    private Dictionary<string, bool> stateFlags;

    // Coroutines
    private Coroutine timeoutCoroutine;

    // Events για να ακούει το UI & τα Alarms
    public delegate void NodeChangedHandler(ScenarioNode newNode);
    public event NodeChangedHandler OnNodeChanged;

    public delegate void ToastRequestedHandler(string message, string style);
    public event ToastRequestedHandler OnToastRequested;

    public delegate void ScoreChangedHandler(int newScore);
    public event ScoreChangedHandler OnScoreChanged;

    // Event για το alarm system
    public delegate void AlarmStateHandler(bool isActive);
    public event AlarmStateHandler OnAlarmStateChanged;

    private bool wasAlarmActive = false;

    void Start()
    {
        LoadAndStartScenario();
    }

    public void LoadAndStartScenario()
    {
        string filePath = Path.Combine(
            Application.streamingAssetsPath,
            scenarioFileName
        );

        Debug.Log("Loading scenario JSON from: " + filePath);

        if (!File.Exists(filePath))
        {
            Debug.LogError(
                "Δεν βρέθηκε το JSON αρχείο του scenario.\nPath: " +
                filePath
            );

            return;
        }

        string json;

        try
        {
            json = File.ReadAllText(filePath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(
                "Αποτυχία ανάγνωσης JSON αρχείου:\n" +
                ex.Message
            );

            return;
        }

        try
        {
            currentScenario =
                JsonConvert.DeserializeObject<ScenarioData>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(
                "Αποτυχία parsing του JSON scenario:\n" +
                ex.Message
            );

            return;
        }

        if (currentScenario == null)
        {
            Debug.LogError(
                "Το scenario JSON φορτώθηκε αλλά δεν δημιουργήθηκε ScenarioData."
            );

            return;
        }

        if (currentScenario.initial_state == null)
        {
            Debug.LogError(
                "Το scenario JSON δεν περιέχει initial_state."
            );

            return;
        }

        if (currentScenario.nodes == null ||
            currentScenario.nodes.Count == 0)
        {
            Debug.LogError(
                "Το scenario JSON δεν περιέχει nodes."
            );

            return;
        }

        currentScore =
            currentScenario.initial_state.current_score;

        stateFlags =
            currentScenario.initial_state.flags
            ?? new Dictionary<string, bool>();

        InitPatientInfo(
            currentScenario.initial_state.patient_info
        );

        InitVitals(
            currentScenario.initial_state.vitals
        );

        nodeDictionary =
            currentScenario.nodes.ToDictionary(
                n => n.id,
                n => n
            );

        OnScoreChanged?.Invoke(currentScore);

        // Ελέγχουμε τους global rules αμέσως
        EvaluateGlobalRules();

        // Ξεκινάμε από τον πρώτο κόμβο του JSON
        GoToNode(currentScenario.nodes[0].id);
    }

    private void GoToNode(string nodeId)
    {
        if (timeoutCoroutine != null)
        {
            StopCoroutine(timeoutCoroutine);
            timeoutCoroutine = null;
        }

        if (string.IsNullOrEmpty(nodeId) ||
            !nodeDictionary.ContainsKey(nodeId))
        {
            Debug.LogWarning(
                $"Ο κόμβος {nodeId} δεν βρέθηκε. Τερματισμός σεναρίου."
            );

            return;
        }

        currentNode = nodeDictionary[nodeId];

        if (scenarioLogger != null)
        {
            scenarioLogger.LogEvent(
                "NODE_ENTER",
                currentNode.id,
                currentNode.type,
                currentNode.text,
                currentScore
            );
        }

        Debug.Log(
            $"--- Είσοδος στον κόμβο: {currentNode.id} ---"
        );

        if (!string.IsNullOrEmpty(currentNode.text))
        {
            Debug.Log(
                $"[ΣΕΝΑΡΙΟ]: {currentNode.text}"
            );
        }

        OnNodeChanged?.Invoke(currentNode);

        switch (currentNode.type)
        {
            case "message":

                if (!string.IsNullOrEmpty(
                    currentNode.next_node_id))
                {
                    GoToNode(
                        currentNode.next_node_id
                    );
                }

                break;

            case "decision":

                if (currentNode.timeout != null &&
                    currentNode.timeout.seconds > 0)
                {
                    timeoutCoroutine =
                        StartCoroutine(
                            HandleTimeout(
                                currentNode.timeout
                            )
                        );
                }

                break;

            case "gate":

                Debug.Log(
                    "Βρέθηκε Gate Node."
                );

                break;

            case "end":

                Debug.Log(
                    $"Τέλος σεναρίου! Τελικό Σκορ: {currentScore}"
                );

                if (scenarioLogger != null)
                {
                    scenarioLogger.ExportJSON();
                }

                if (debriefManager != null)
                {
                    debriefManager.PrintDebriefToConsole();
                    debriefManager.ShowDebrief();
                }

                break;
        }
    }

    public void SelectOption(string optionId)
    {
        if (currentNode == null ||
            currentNode.type != "decision")
        {
            return;
        }

        var option =
            currentNode.options.FirstOrDefault(
                o => o.id == optionId
            );

        if (option != null)
        {
            if (scenarioLogger != null)
            {
                scenarioLogger.LogEvent(
                    "OPTION_SELECTED",
                    currentNode.id,
                    option.id,
                    option.label,
                    currentScore
                );
            }

            ApplyEffects(option.effects);

            GoToNode(
                option.next_node_id
            );
        }
    }

    public void TrySelectOptionByHotspot(
        string hotspotId)
    {
        if (currentNode == null)
        {
            return;
        }

        // Decision Node
        if (currentNode.type == "decision")
        {
            var option =
                currentNode.options.FirstOrDefault(
                    o =>
                        o.target_hotspot ==
                        hotspotId
                );

            if (option != null)
            {
                Debug.Log(
                    $"Επιλέχθηκε η ενέργεια '{option.label}' μέσω του hotspot {hotspotId}"
                );

                SelectOption(option.id);
            }
            else
            {
                Debug.Log(
                    $"Το hotspot {hotspotId} δεν αποτελεί επιλογή σε αυτή τη φάση του σεναρίου."
                );
            }
        }

        // Gate Node
        else if (currentNode.type == "gate")
        {
            if (currentNode.target_hotspot ==
                hotspotId)
            {
                Debug.Log(
                    $"Ενεργοποιήθηκε η πύλη (gate) μέσω του hotspot {hotspotId}."
                );
            }
            else
            {
                Debug.Log(
                    $"Πρέπει πρώτα να ολοκληρώσεις την τεκμηρίωση στο {currentNode.target_hotspot}."
                );

                if (!string.IsNullOrEmpty(
                    currentNode.feedback_blocked))
                {
                    OnToastRequested?.Invoke(
                        currentNode.feedback_blocked,
                        "danger"
                    );
                }
            }
        }
    }

    private void ApplyEffects(NodeEffects effects)
    {
        if (effects == null)
        {
            return;
        }

        if (effects.score_delta != 0)
        {
            currentScore +=
                effects.score_delta;

            OnScoreChanged?.Invoke(
                currentScore
            );
        }

        if (effects.state_update != null)
        {
            foreach (var kvp in
                     effects.state_update)
            {
                string flagKey =
                    kvp.Key;

                if (flagKey.StartsWith(
                    "flags."))
                {
                    flagKey =
                        flagKey.Substring(
                            "flags.".Length
                        );
                }

                stateFlags[flagKey] =
                    kvp.Value;

                Debug.Log(
                    "STATE FLAG: " +
                    flagKey +
                    " = " +
                    kvp.Value
                );
            }
        }

        if (effects.vitals_update != null &&
            patientVitals != null)
        {
            UpdatePatientVitals(
                effects.vitals_update
            );
        }

        if (!string.IsNullOrEmpty(
            effects.toast))
        {
            OnToastRequested?.Invoke(
                effects.toast,
                "info"
            );
        }

        EvaluateGlobalRules();
    }

    private void UpdatePatientVitals(
        Dictionary<string, float> newVitals)
    {
        if (newVitals.ContainsKey("hr"))
        {
            patientVitals.SetHeartRate(
                newVitals["hr"]
            );
        }

        if (newVitals.ContainsKey("spo2"))
        {
            patientVitals.SetOxygenSaturation(
                newVitals["spo2"]
            );
        }

        if (newVitals.ContainsKey("rr"))
        {
            patientVitals.SetRespiratoryRate(
                newVitals["rr"]
            );
        }

        if (newVitals.ContainsKey("temp"))
        {
            patientVitals.SetTemperature(
                newVitals["temp"]
            );
        }
    }

    private void InitPatientInfo(
        PatientInfoData info)
    {
        if (info == null ||
            ehrManager == null)
        {
            return;
        }

        ehrManager.patientInfo.fullName =
            info.full_name;

        ehrManager.patientInfo.age =
            info.age;

        ehrManager.patientInfo.location =
            info.location;

        ehrManager.patientInfo.admissionDiagnosis =
            info.admission_diagnosis;

        Debug.Log(
            "Patient loaded from JSON: " +
            ehrManager.patientInfo.fullName +
            " | Age: " +
            ehrManager.patientInfo.age +
            " | Location: " +
            ehrManager.patientInfo.location +
            " | Diagnosis: " +
            ehrManager.patientInfo.admissionDiagnosis
        );
    }

    private void InitVitals(
        Dictionary<string, object> initialVitals)
    {
        if (initialVitals == null ||
            patientVitals == null)
        {
            return;
        }

        float hr =
            patientVitals.HeartRate;

        float spo2 =
            patientVitals.OxygenSaturation;

        float rr =
            patientVitals.RespiratoryRate;

        float temp =
            patientVitals.Temperature;

        string bp =
            patientVitals.BloodPressure;

        if (initialVitals.TryGetValue(
            "hr",
            out object hrObj))
        {
            hr =
                System.Convert.ToSingle(
                    hrObj
                );
        }

        if (initialVitals.TryGetValue(
            "spo2",
            out object spo2Obj))
        {
            spo2 =
                System.Convert.ToSingle(
                    spo2Obj
                );
        }

        if (initialVitals.TryGetValue(
            "rr",
            out object rrObj))
        {
            rr =
                System.Convert.ToSingle(
                    rrObj
                );
        }

        if (initialVitals.TryGetValue(
            "temp",
            out object tempObj))
        {
            temp =
                System.Convert.ToSingle(
                    tempObj
                );
        }

        if (initialVitals.TryGetValue(
            "bp",
            out object bpObj))
        {
            bp =
                bpObj.ToString();
        }

        patientVitals.SetVitals(
            hr,
            spo2,
            bp,
            rr,
            temp
        );
    }

    private IEnumerator HandleTimeout(
        TimeoutData timeoutData)
    {
        yield return new WaitForSeconds(
            timeoutData.seconds
        );

        Debug.Log(
            "Χρόνος επιλογής έληξε!"
        );

        ApplyEffects(
            timeoutData.on_timeout_effects
        );

        GoToNode(
            timeoutData.next_node_id
        );
    }

    private void EvaluateGlobalRules()
    {
        if (currentScenario.rules?.global_rules == null ||
            patientVitals == null)
        {
            OnAlarmStateChanged?.Invoke(false);

            wasAlarmActive = false;

            return;
        }

        bool isAlarmActive = false;

        foreach (var rule in
                 currentScenario.rules.global_rules)
        {
            if (rule.condition != null &&
                rule.condition.spo2_condition != null)
            {
                var cond =
                    rule.condition.spo2_condition;

                bool conditionMet = false;

                if (cond.lt.HasValue &&
                    patientVitals.OxygenSaturation <
                    cond.lt.Value)
                {
                    conditionMet = true;
                }

                if (cond.gt.HasValue &&
                    patientVitals.OxygenSaturation >
                    cond.gt.Value)
                {
                    conditionMet = true;
                }

                if (conditionMet)
                {
                    isAlarmActive = true;

                    // Toast μόνο όταν ο συναγερμός
                    // ενεργοποιείται πρώτη φορά
                    if (!wasAlarmActive)
                    {
                        foreach (var effect in
                                 rule.effects)
                        {
                            if (effect.type ==
                                "ui_toast")
                            {
                                OnToastRequested?.Invoke(
                                    effect.message,
                                    effect.style
                                );
                            }
                        }
                    }
                }
            }
        }

        if (isAlarmActive !=
            wasAlarmActive)
        {
            OnAlarmStateChanged?.Invoke(
                isAlarmActive
            );

            wasAlarmActive =
                isAlarmActive;
        }
    }

    public bool TryCompleteCurrentEHRGate(
        EHRManager ehrManager,
        out string feedback)
    {
        feedback = "";

        if (currentNode == null)
        {
            feedback =
                "No active scenario node.";

            return false;
        }

        if (currentNode.type != "gate")
        {
            feedback =
                "Documentation saved.\n" +
                "No documentation gate is currently active.";

            return false;
        }

        if (ehrManager == null)
        {
            feedback =
                "EHR system is not available.";

            return false;
        }

        bool gateComplete = false;

        // Gate 1
        if (currentNode.id ==
            "n4_gate_documentation_1")
        {
            gateComplete =
                ehrManager
                    .DocumentationGate1Complete();

            if (!gateComplete)
            {
                feedback =
                    "Documentation incomplete.\n" +
                    "Required: Observation and FiO2 Setting.";

                if (!string.IsNullOrEmpty(
                    currentNode.feedback_blocked))
                {
                    OnToastRequested?.Invoke(
                        currentNode.feedback_blocked,
                        "danger"
                    );
                }

                return false;
            }
        }

        // Gate 2
        else if (currentNode.id ==
                 "n7_gate_documentation_2")
        {
            gateComplete =
                ehrManager
                    .DocumentationGate2Complete();

            if (!gateComplete)
            {
                feedback =
                    "Documentation incomplete.\n" +
                    "Required: Recipient and Outcome.";

                if (!string.IsNullOrEmpty(
                    currentNode.feedback_blocked))
                {
                    OnToastRequested?.Invoke(
                        currentNode.feedback_blocked,
                        "danger"
                    );
                }

                return false;
            }
        }
        else
        {
            feedback =
                "Unknown documentation gate.";

            return false;
        }

        // Gate passed
        ApplyEffects(
            currentNode.effects_on_pass
        );

        feedback =
            !string.IsNullOrEmpty(
                currentNode.feedback_success)
            ? currentNode.feedback_success
            : "Documentation complete.";

        OnToastRequested?.Invoke(
            feedback,
            "info"
        );

        string nextNodeId =
            currentNode.next_node_id;

        Debug.Log(
            "EHR Gate completed: " +
            currentNode.id
        );

        GoToNode(
            nextNodeId
        );

        return true;
    }
}