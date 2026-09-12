using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebriefManager : MonoBehaviour
{   
    [Header("Player")]
    public PlayerMovement playerMovement;
    public PlayerCam playerCam;
    public Interactor interactor;
    [Header("References")]
    public ScenarioLogger scenarioLogger;

    [Header("Debrief UI")]
    public GameObject debriefPanel;
    public TMP_Text debriefText;
    public ScrollRect debriefScrollRect;

    [Header("Other UI")]
    public EHRUI ehrUI;



    // START

    private void Start()
    {
        if (debriefPanel != null)
        {
            debriefPanel.SetActive(false);
        }
    }


    // KEEP CURSOR ACTIVE WHILE DEBRIEF IS OPEN

    private void LateUpdate()
    {
        if (debriefPanel != null &&
            debriefPanel.activeSelf)
        {
            // Ο παίκτης δεν παίρνει control όσο είναι ανοιχτό το Debrief
            if (playerMovement != null)
                playerMovement.inputEnabled = false;

            if (playerCam != null)
                playerCam.inputEnabled = false;

            // Το mouse παραμένει διαθέσιμο μόνο για το UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    // BUILD COMPLETE DEBRIEF

    public string BuildDebriefText()
    {
        if (scenarioLogger == null ||
            scenarioLogger.logFile == null ||
            scenarioLogger.logFile.events == null)
        {
            return "No scenario log available.";
        }

        List<ScenarioLogEntry> events =
            scenarioLogger.logFile.events;

        StringBuilder builder =
            new StringBuilder();

        int finalScore =
            GetFinalScore(events);

        builder.AppendLine(
            "SCENARIO DEBRIEF"
        );

        builder.AppendLine();

        builder.AppendLine(
            "Final Score: " +
            finalScore
        );

        builder.AppendLine();


        // CHECKLIST

        builder.AppendLine(
            "CHECKLIST"
        );

        BuildChecklist(
            builder,
            events
        );

        builder.AppendLine();


        // DECISION PATH

        builder.AppendLine(
            "DECISION PATH"
        );

        BuildDecisionPath(
            builder,
            events
        );

        builder.AppendLine();


        // DOCUMENTATION

        builder.AppendLine(
            "DOCUMENTATION"
        );

        BuildDocumentationReview(
            builder,
            events
        );

        return builder.ToString();
    }


    // PRINT TO CONSOLE

    public void PrintDebriefToConsole()
    {
        string debrief =
            BuildDebriefText();

        Debug.Log(
            "\nDEBRIEF\n" +
            debrief +
            "\n"
        );
    }


    // FINAL SCORE

    private int GetFinalScore(
        List<ScenarioLogEntry> events)
    {
        if (events == null ||
            events.Count == 0)
        {
            return 0;
        }

        return events[
            events.Count - 1
        ].score;
    }


    // CHECKLIST

    private void BuildChecklist(
        StringBuilder builder,
        List<ScenarioLogEntry> events)
    {
        bool initialActionCompleted =
            HasOption(
                events,
                "opt_assess"
            )
            ||
            HasOption(
                events,
                "opt_check_monitor"
            );

        bool oxygenIntervention =
            HasOption(
                events,
                "opt_increase_o2"
            );

        bool reachedDocumentation1 =
            HasNode(
                events,
                "n4_gate_documentation_1"
            );

        bool completedDocumentation1 =
            HasNode(
                events,
                "n5_reassessment"
            );

        bool escalationDecisionCompleted =
            HasOption(
                events,
                "opt_call_doc"
            )
            ||
            HasOption(
                events,
                "opt_wait"
            );


        builder.AppendLine(
            (initialActionCompleted
                ? "[Completed] "
                : "[Not completed] ")
            +
            "Initial assessment/action completed"
        );

        builder.AppendLine(
            (oxygenIntervention
                ? "[Completed] "
                : "[Not completed] ")
            +
            "Oxygen intervention completed"
        );

        builder.AppendLine(
            (
                reachedDocumentation1 &&
                completedDocumentation1
                ? "[Completed] "
                : "[Not completed] "
            )
            +
            "Required clinical documentation completed"
        );

        builder.AppendLine(
            (escalationDecisionCompleted
                ? "[Completed] "
                : "[Not completed] ")
            +
            "Escalation decision completed"
        );
    }


    // DECISION PATH

    private void BuildDecisionPath(
        StringBuilder builder,
        List<ScenarioLogEntry> events)
    {
        int decisionNumber = 1;

        foreach (
            ScenarioLogEntry entry
            in events)
        {
            if (entry.eventType !=
                "OPTION_SELECTED")
            {
                continue;
            }

            string readableDecision =
                SanitizeDecision(entry);

            builder.AppendLine(
                decisionNumber +
                ". " +
                readableDecision
            );

            decisionNumber++;
        }

        if (decisionNumber == 1)
        {
            builder.AppendLine(
                "No decision data available."
            );
        }
    }


    // SANITIZE DECISIONS

    private string SanitizeDecision(
        ScenarioLogEntry entry)
    {
        switch (entry.target)
        {
            case "opt_assess":

                return
                    "Assessed the patient.";


            case "opt_check_monitor":

                return
                    "Checked the patient monitor.";


            case "opt_increase_o2":

                return
                    "Increased FiO2 to 100%.";


            case "opt_do_nothing":

                return
                    "Waited without changing the oxygen intervention.";


            case "opt_call_doc":

                return
                    "Called the physician for escalation.";


            case "opt_wait":

                return
                    "Waited without escalating to the physician.";


            default:

                if (!string.IsNullOrEmpty(
                    entry.details))
                {
                    return entry.details;
                }

                return entry.target;
        }
    }


    // DOCUMENTATION REVIEW

    private void BuildDocumentationReview(
        StringBuilder builder,
        List<ScenarioLogEntry> events)
    {
        int gate1Attempts = 0;
        int gate2Attempts = 0;

        bool gate1Completed =
            HasNode(
                events,
                "n5_reassessment"
            );

        bool communicationRequired =
            HasOption(
                events,
                "opt_call_doc"
            );

        bool scenarioCompleted =
            HasNode(
                events,
                "n8_end_scenario"
            );


        foreach (
            ScenarioLogEntry entry
            in events)
        {
            if (entry.eventType !=
                "EHR_SUBMIT")
            {
                continue;
            }

            if (entry.nodeId ==
                "n4_gate_documentation_1")
            {
                gate1Attempts++;
            }

            if (entry.nodeId ==
                "n7_gate_documentation_2")
            {
                gate2Attempts++;
            }
        }


        // GATE 1

        if (gate1Completed)
        {
            builder.AppendLine(
                "[Completed] Assessment / Intervention documentation."
            );

            if (gate1Attempts > 1)
            {
                builder.AppendLine(
                    "Required correction before completion."
                );
            }
        }
        else
        {
            builder.AppendLine(
                "[Not completed] Assessment / Intervention documentation."
            );
        }


        // GATE 2

        if (!communicationRequired)
        {
            builder.AppendLine(
                "Communication documentation was not required on this path."
            );

            return;
        }

        if (scenarioCompleted)
        {
            builder.AppendLine(
                "[Completed] Communication documentation."
            );

            if (gate2Attempts > 1)
            {
                builder.AppendLine(
                    "Required correction before completion."
                );
            }
        }
        else
        {
            builder.AppendLine(
                "[Not completed] Communication documentation."
            );
        }
    }


    // OPTION HELPER

    private bool HasOption(
        List<ScenarioLogEntry> events,
        string optionId)
    {
        foreach (
            ScenarioLogEntry entry
            in events)
        {
            if (
                entry.eventType ==
                "OPTION_SELECTED"
                &&
                entry.target ==
                optionId
            )
            {
                return true;
            }
        }

        return false;
    }


    // NODE HELPER

    private bool HasNode(
        List<ScenarioLogEntry> events,
        string nodeId)
    {
        foreach (
            ScenarioLogEntry entry
            in events)
        {
            if (
                entry.eventType ==
                "NODE_ENTER"
                &&
                entry.nodeId ==
                nodeId
            )
            {
                return true;
            }
        }

        return false;
    }


    // SHOW DEBRIEF

    public void ShowDebrief()
    {
        StartCoroutine(
            ShowDebriefRoutine()
        );
    }


    private IEnumerator ShowDebriefRoutine()
    {
        if (debriefPanel == null ||
            debriefText == null)
        {
            yield break;
        }



        // CLOSE EHR FIRST

        if (ehrUI != null &&
            ehrUI.ehrPanel != null &&
            ehrUI.ehrPanel.activeSelf)
        {
            ehrUI.CloseEHR();
        }


        // WAIT UNTIL CAMERA EXITS EHR FOCUS

        if (ehrUI != null &&
            ehrUI.cameraController != null)
        {
            while (
                ehrUI.cameraController.IsFocused
            )
            {
                yield return null;
            }
        }


        // One extra frame so any cursor/focus code
        // from the camera system finishes first.
        yield return null;


        // OPEN DEBRIEF

        debriefPanel.SetActive(true);
        if (interactor != null)
        {
            interactor.enabled = false;
        }

        

        // Disable player movement
        if (playerMovement != null)
        {
            playerMovement.inputEnabled =
                false;
        }


        // Disable camera movement
        if (playerCam != null)
        {
            playerCam.inputEnabled =
                false;
        }


        // Give control of mouse to the UI
        Cursor.lockState = CursorLockMode.None;

        Cursor.visible = true;


        // Build player-readable debrief
        debriefText.text = BuildDebriefText();


        // Refresh scroll layout
        StartCoroutine(
            RefreshDebriefLayout()
        );
    }


    // REFRESH SCROLL VIEW

    private IEnumerator RefreshDebriefLayout()
    {
        // Wait for TMP and layout groups
        // to calculate preferred sizes.
        yield return null;

        Canvas.ForceUpdateCanvases();


        if (debriefText != null)
        {
            LayoutRebuilder
                .ForceRebuildLayoutImmediate(
                    debriefText.rectTransform
                );
        }


        if (debriefScrollRect != null &&
            debriefScrollRect.content != null)
        {
            LayoutRebuilder
                .ForceRebuildLayoutImmediate(
                    debriefScrollRect.content
                );

            Canvas.ForceUpdateCanvases();


            // Start at top of debrief
            debriefScrollRect
                .verticalNormalizedPosition =
                1f;


            debriefScrollRect
                .StopMovement();
        }
    }


    // CLOSE DEBRIEF

    public void CloseDebrief()
    {
        if (debriefPanel != null)
        {
            debriefPanel.SetActive(false);
        }

        // Μόνο εδώ ξαναενεργοποιούμε interactions
        if (interactor != null)
        {
            interactor.enabled = true;
        }

        if (playerMovement != null)
        {
            playerMovement.inputEnabled = true;
        }

        if (playerCam != null)
        {
            playerCam.inputEnabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}