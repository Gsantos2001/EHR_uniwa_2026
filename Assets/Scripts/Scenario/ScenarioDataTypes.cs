using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class ScenarioData
{
    public string schema_version;
    public ScenarioMeta scenario_meta;
    public InitialState initial_state;
    public List<HotspotData> hotspots;
    public RuleData rules;
    public List<ScenarioNode> nodes;
}

[System.Serializable]
public class ScenarioMeta
{
    public string id;
    public string title;
    public string description;
    public int estimated_duration_minutes;
    public string difficulty;
    public List<string> learning_goals;
}

[System.Serializable]
public class InitialState
{
    public int time_elapsed;
    public int current_score;
    public PatientInfoData patient_info;
    public Dictionary<string, bool> flags;
    public Dictionary<string, object> vitals; // Αποθήκευση HR, SpO2, κλπ.
}

[System.Serializable]
public class PatientInfoData
{
    public string full_name;
    public int age;
    public string location;
    public string admission_diagnosis;
}

[System.Serializable]
public class HotspotData
{
    public string id;
    public string label;
}

[System.Serializable]
public class RuleData
{
    public List<GlobalRule> global_rules;
}

[System.Serializable]
public class GlobalRule
{
    public string id;
    public Condition condition;
    public List<Effect> effects;
}

[System.Serializable]
public class Condition
{
    [JsonProperty("vitals.spo2")]
    public CompareCondition spo2_condition;
}

[System.Serializable]
public class CompareCondition
{
    public int? lt; // less than
    public int? gt; // greater than
}

[System.Serializable]
public class ScenarioNode
{
    public string id;
    public string type; // "message", "decision", "gate", "end"
    public string text;
    public string next_node_id;
    public List<DecisionOption> options;
    public TimeoutData timeout;
    
    // Πεδία για Gate (EHR) - Προετοιμασία
    public string description;
    public string target_hotspot;
    public string feedback_blocked;
    public string feedback_success;
    public NodeEffects effects_on_pass;
}

[System.Serializable]
public class DecisionOption
{
    public string id;
    public string label;
    public string target_hotspot;
    public NodeEffects effects;
    public string next_node_id;
}

[System.Serializable]
public class NodeEffects
{
    public int score_delta;
    public Dictionary<string, bool> state_update;
    public Dictionary<string, float> vitals_update;
    public string toast;
}

[System.Serializable]
public class TimeoutData
{
    public int seconds;
    public NodeEffects on_timeout_effects;
    public string next_node_id;
}

[System.Serializable]
public class Effect
{
    public string type;
    public string target;
    public string state;
    public string style;
    public string message;
}