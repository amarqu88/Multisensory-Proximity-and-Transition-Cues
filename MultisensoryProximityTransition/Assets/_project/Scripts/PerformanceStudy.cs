using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Animations;
using TMPro;
using static PerformanceStudy.PerformanceStudyState;

/*
    okay. wir rudern ein wenig zurück.
    Wir nehmen jetzt das identische setup so wie wir das jetzt haben (nur horizontale bewegung des targets)
    und erweitern das mit dem primary task (reaktion bei bestimmter Zahl).
    Um die umsetzung des primary task kümmere ich mich grade. Wir brauchen dann eigentlich nur ein paar änderungen wie es jetzt ist:
    So wollen wir u.a. auch die Probleme bei den diagonalen Transitions umgehen bis wir vielleicht gemeinsam eine geeignetere Lösung gefunden haben (nach Japan).
    Ist quasi eine vereinfachte Version von dem was ich dir vorher alles geschickt habe.
    1. - x sieger modes aus der vorgegangenen Studie (OK - Im Inspector Pärchenweise draufziehen)
    2. - targets nur horizontal (wie gehabt) (OK)
    3. - target kann immer nur in eine Richtung gehen (nicht hin und zurück) (OK)
    4. - Targets sind unsichtbar (OK)

    5. - Nutzer muss taste drücken wenn er denkt das target hat das FOV betreten (OK)
    6. - dann soll prinzipiell die richtung noch angegeben werden (in diesem Fall links oder Rechts) (OK)
    7. - danach wird Target sichtbar eingeblendet für x sekunden (OK)
    8. - dann nächster trial (OK)

    Logging:
        runCount - Sofort verfügbar
        startPositionDegree - Kurz vor start Verfügbar
        rotationDirection - Kurz vor start Verfügbar
        startTime - Bei Start verfügbar
        stopTime - Bei Stop verfügbar
        stopPositionDegree - bei stop verfügbar
        userStopped - bei stop verfügbar
        hit - bei stop verfügbar
        missType - bei stop verfügbar
        OuterFeedback - Sofort vcerfügbar
        TransitionFeedback - sofort verfügbar
*/

public class PerformanceStudy : MonoBehaviour
{
    string currentFileName;
    int logRunCount;
    float logStartPosDegree;
    float logStopPosDegree;
    TargetRotator.RotationDirection logRotationDirection;
    float logStartTime;
    float logStopTime;
    float logDegreeLeft;
    bool logUserStopped;
    bool logHit;
    MissType logMissType;
    OuterFeedback logOuterFeedback = null;
    TransitionFeedback logTransitionFeedback = null;

    public enum MissType
    {
        ROTATION_END,
        NONE,
        TOO_EARLY
    }

    public enum PerformanceStudyState
    {
        BEFORE_ROTATE = 0,
        ROTATING,
        USER_STOPPED,
        USER_MISSED,
        DIRECTION_INPUT_WAIT,
        TARGET_VISIBLE,
        END
    }
    
    // Script, welches das Target rotiert und bei 0 Grad aufhört zu rotieren.
    public TargetRotator targetRotator;
    public TargetFeedback targetFeedback;
    public PrimaryTask primaryTask;

    // selectedOuterFeedbacks[i] und selectedTransitionFeedbacks[i] ergeben jeweils ein Feedback-Pärchen. 
    // Dementsprechend können Feedbacks auch mehrfach in den Arrays vorhanden sein.
    public OuterFeedback[] selectedOuterFeedbacks;
    public TransitionFeedback[] selectedTransitionFeedbacks;
    
    // Gilt für Feedbackpärchen. Steht hier eine 1, dann wird jedes Feedback einmal durchlaufen. 
    public int runCountPerFeedback;

    public int trainingRunCountPerFeedback;
    public int secondTrainingRunCountPerTraining;
     
    bool startWithTraining = true;
    public bool isTraining
    {
        get; protected set;
    }
    public bool isSecondTraining
    {
        get; protected set;
    }
    // Folgende Werte habe ich dem "alten" "RotateAroundCam" aus dem Inspector entnommen.
    public float targetDistance = 3f;
    public float targetVelocityDegree = 37.5f;
    public float startingPositionDegree = -80.625f;
    public float startingOffsetDegree = 10f;
    public float logEntryTime;
    public float missDegree = 0f;
    // Zeit in Sekunden, in der die Richtungsanzeige Sichtbar ist.
    public float timeArrowsVisible = 2;
    public float timeBetweenPerformanceRuns = 0f;
    // Zeigt die Pfeile an
    public Animator arrowAnimator;
    public GameObject lineObject;
    public GameObject fakeSphere;
    public GameObject lineObjectAR;
    // Enthält die Runs. Jedes Tupel beschreibt hierbei das Feedback und die Rotationsrichtung.
    List<Tuple<OuterFeedback, TransitionFeedback, TargetRotator.RotationDirection>> runs;
    float startingOffset;
    int currentRun = -1;
    float currentStartingPosition;

    public PerformanceStudyState performanceStudyState = BEFORE_ROTATE;
    bool notLogged = true;
    bool userCorrectDirection = false;
    public TextMeshProUGUI modeText;


    void initLogFile()
    {
        string m_Path = Application.dataPath;
        int i = 0;
        string fileName = m_Path + "/Study-Performance" + i.ToString() + ".csv";

        while (File.Exists(fileName))
        {
            fileName = m_Path + "/Study-Performance" + i++.ToString() + ".csv";
        }

        currentFileName = fileName;
    }

    void WriteLogString(string s)
    {
        using (StreamWriter sw = File.AppendText(currentFileName))
        {
            sw.Write(s);
        }
    }

    void logTrial()
    {
        notLogged = false;

            // RunCount;StartPositionDegree;StopPositionDegree;RotationDirection;
            // StartTime;StopTime;UserStopped;Hit,MissType;OuterFeedback;TransitionFeedback
            WriteLogString(string.Concat(
                logRunCount, ";", logStartPosDegree, ";", logStopPosDegree, ";",
                logDegreeLeft, ";", logRotationDirection.ToString(), ";",
                logStartTime, ";", logEntryTime,";",logStopTime, ";", logUserStopped, ";",
                logHit, ";", logMissType.ToString(), ";", logOuterFeedback.ToString(), ";",
                logTransitionFeedback.ToString(), Environment.NewLine
                ));
    }


    private void Start()
    {
        initLogFile();
        WriteLogString("RunCount;StartPositionDegree;StopPositionDegree;DegreeLeft;RotationDirection;StartTime;EntryTime;StopTime;UserStopped;Hit;MissType;OuterFeedback;TransitionFeedback"+ Environment.NewLine);
        // Calc. runs. Wenn diese Art der Erzeugung nicht passt, kann eine andere Methode angelegt werden. 
        if (startWithTraining && trainingRunCountPerFeedback > 0)
        {
            // Get training runs
            calcRunsRandomBlocked(trainingRunCountPerFeedback);
            isTraining = true;
            isSecondTraining = false;

        }
        else if(startWithTraining && secondTrainingRunCountPerTraining > 0)
        {
            calcRunsRandomBlocked(secondTrainingRunCountPerTraining);
            isSecondTraining = true;
            isTraining = false;
        }
        else
        {
            // Get normal runs. If training is selected. Calculation of actual runs is in startNextRun
            calcRunsRandomBlocked(runCountPerFeedback);
            isSecondTraining = false;
            isTraining = false;
        }
        modeText.text = "*NEXT* Trial:\n" + SetModeTextGUI(runs[currentRun+1].Item1.getShortcut(), runs[currentRun+1].Item2.getShortcut());//runs[currentRun].Item1.getShortcut() + " " + runs[currentRun].Item2.getShortcut();


        // Hide Target.
        targetVisible(false);

        // get notified if rotation is done
        targetRotator.RotationDoneEvent += rotationDoneCallback;

        // Startet die Runs
        // continueStudy();
    }

    public void startStudy()
    {
        if(currentRun < 0)
        {
        continueStudy();
        }
    }
    
    public void continueStudy()
    {
        switch (performanceStudyState)
        {
            case (BEFORE_ROTATE):
                primaryTask.startNumberTask(isTraining);
                startNextRun();
                break;
            default:
                // Alle anderen Fälle sind hierfür nicht relevant.
                break;
        }
    }

    

    public void selectDirection(Vector2 direction)
    {
        if (performanceStudyState == ROTATING)
        {
            performanceStudyState = USER_STOPPED;
            targetFeedback.stopFeedback();
            targetRotator.stopRotation();

            // Diese Logik gilt, wenn der Nutzer in die Richtung drück aus der das Ziel kommt. Gilt aber auch nur für Bewegungen auf x-Achse, Also 1-Dimensional
            // Für 2D Muss andere Logik her.
            if (
                (direction.x == -1f && runs[currentRun].Item3 == TargetRotator.RotationDirection.CLOCKWISE) ||
                (direction.x == 1f && runs[currentRun].Item3 == TargetRotator.RotationDirection.COUNTER_CLOCKWISE)
                )
            { 
                userCorrectDirection = true;
            }else
            {
                userCorrectDirection = false;
            }

            // Zeige Richtung des Targets an
            // Aber nur, wenn training ist.
            
            if (isTraining)
            {
                showDirection();
                targetVisible(true);
                performanceStudyState = TARGET_VISIBLE;
                //StartCoroutine(startNextRunAfterSeconds(timeArrowsVisible));
            }else
            {
                //StartCoroutine(startNextRunAfterSeconds(timeBetweenPerformanceRuns));
            }
            // Starte nächsten Run
        }
    }

    // Starte nächsten Run nach gegebener anzahl sekunden
    public IEnumerator startNextRunAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        performanceStudyState = BEFORE_ROTATE;
        if (!isTraining && !isSecondTraining)
        {
            startNextRun();
        }
        else
        {
            if (isTraining) { 
                targetVisible(false);
                hideDirection();
                if (currentRun < runs.Count - 1)
                {
                    modeText.text = "*NEXT* Trial:\n" + SetModeTextGUI(runs[currentRun + 1].Item1.getShortcut(), runs[currentRun + 1].Item2.getShortcut());//runs[currentRun].Item1.getShortcut() + " " + runs[currentRun].Item2.getShortcut();
                }
                else
                {
                    modeText.color = Color.red;
                    modeText.text = "Continious TRAINING will start now.\n Please confirm *TWO* times to continue.";
                    //startNextRun();
                }
            } else
            {
                // Second training
                if (currentRun < runs.Count - 1)
                {
                    startNextRun();
                }
                else
                {
                    modeText.color = Color.red;
                    primaryTask.stopNumberTask();
                    modeText.text = "PERFORMANCE TRIALS will start now.\n Please confirm *TWO* times to continue.";
                    //startNextRun();
                }
            }
        }
    }

    public void startNextRun()
    {
        notLogged = true;

        targetVisible(false);
        hideDirection();
        if (currentRun + 1 >= runs.Count && performanceStudyState != END)
        {
            // Ende der Runs erreicht
            primaryTask.stopNumberTask();

            if (isTraining)
            {
                // Ende des Trainings. Berechne neue Runs
                isTraining = false;
                isSecondTraining = true;
                currentRun = -1;
                calcRunsRandomBlocked(secondTrainingRunCountPerTraining);
                performanceStudyState = BEFORE_ROTATE;
            }
            else if(isSecondTraining)
            {
                isSecondTraining = false;
                currentRun = -1;
                calcRunsRandomBlocked(runCountPerFeedback);
                performanceStudyState = BEFORE_ROTATE;
            }
            else
            {
                logTrial();
                performanceStudyState = END;
            }
        }
        else if (performanceStudyState == BEFORE_ROTATE)
        {
            currentRun++;

            if (isTraining)
            {
                modeText.text = "";//"*CURRENT* Trial:\n" + SetModeTextGUI(runs[currentRun].Item1.getShortcut(), runs[currentRun].Item2.getShortcut());//runs[currentRun].Item1.getShortcut() + " " + runs[currentRun].Item2.getShortcut();
            }
            else if(isSecondTraining)
            {
                modeText.text = "";
            }
            else
            {
                modeText.text = "";
            }
                // Hide target & direction

                // Set Rotation direction
                targetRotator.rotationDirection = runs[currentRun].Item3;
                startingOffset = UnityEngine.Random.Range(-startingOffsetDegree, startingOffsetDegree);
                currentStartingPosition = startingPositionDegree + startingOffset;
                // Target to startposition
                targetRotator.setInitialPositionByDegree(targetDistance, currentStartingPosition);
                // Set maximum rotation distance in degree
                targetRotator.setFiniteRotationByDegree(-currentStartingPosition + missDegree);

                targetFeedback.setFeedback(runs[currentRun].Item1, runs[currentRun].Item2, runs[currentRun].Item3);

                //Logging
                logRunCount = currentRun;
                logRotationDirection = runs[currentRun].Item3;
                logStartPosDegree = -currentStartingPosition;
                logStopPosDegree = missDegree;
                logStartTime = Time.realtimeSinceStartup;
                logOuterFeedback = runs[currentRun].Item1;
                logTransitionFeedback = runs[currentRun].Item2;



            targetFeedback.startFeedback();
                targetRotator.startRotation();
                performanceStudyState = ROTATING;

                primaryTask.startNumberTask(isTraining);
        }
    }

    void rotationDoneCallback()
    {
        //Debug.LogError("RotDoneCallback");
        // Wird aufgerufen, wenn eine Rotation beendet ist
        if (isTraining)
            primaryTask.stopNumberTask();

        targetFeedback.stopFeedback();
        logStopTime = Time.realtimeSinceStartup;
        logDegreeLeft = targetRotator.degreeLeft;

        if (performanceStudyState == USER_STOPPED)
        {
            
            logUserStopped = true;
            // TODO: Was ist die Bedingung dafür, dass Nutzer falsch gedrückt hat? Implementierung in Funktion checkUserPress
            // Prüfe ob Nutzer korrekt gedrückt hat. Wenn ja, dann warte auf Richtungsangabe durch Nutzer.
            // Wenn Nein, dann Hat user einen miss. Zeige Richtung des targets an
            if (!checkUserPress())
            {
                performanceStudyState = USER_MISSED;
                logHit = false;
                logMissType = MissType.TOO_EARLY;
            }
            else
            {
                logHit = true;
                logMissType = MissType.NONE;
            }

        } else if(performanceStudyState == ROTATING)
        {
            logHit = false;
            logUserStopped = false;
            logMissType = MissType.ROTATION_END;
            // Nutzer hat nicht gedrückt. Target ist in der Mitte angekommen.
            performanceStudyState = USER_MISSED;
        }

        if (isTraining)
        {
            // Zeige Target und Richtung an. Warte nach der Anzeige bis zum nächsten Run.
            //Debug.LogError("TRANSITIONSTARTED: " + targetFeedback.inTransitionStarted);
            showDirection();
            targetVisible(true);
            performanceStudyState = TARGET_VISIBLE;

            StartCoroutine(startNextRunAfterSeconds(timeArrowsVisible));
        }
        else if (isSecondTraining)
        {
            StartCoroutine(startNextRunAfterSeconds(timeBetweenPerformanceRuns));
        }
        else
        {
            if(currentRun > -1 && notLogged)
            {
                logTrial();
                
            }
                
            StartCoroutine(startNextRunAfterSeconds(timeBetweenPerformanceRuns));
        }
        
    }

    bool checkUserPress()
    {
        // Eine Möglichkeit zu Prüfen ob der Nutzer zu früh gedrückt hat ist zu schauen ob die verbleibende Rotation kleiner als FOV/2 ist.
        // return targetRotator.degreeLeft < targetFeedback.targetMeshChecker.simulatedFrustumDegree.x / 2f;

        // Eine andere Möglichkeit ist zu schauen ob die intransition schon gestartet wurde.
        return targetFeedback.inTransitionStarted;
    }

    void showDirection()
    {
        lineObject.SetActive(true);
        lineObjectAR.SetActive(true);

        //arrowAnimator.gameObject.SetActive(true);
        //arrowAnimator.transform.forward = targetRotator.getMovingDirection();
        //arrowAnimator.transform.localRotation = Quaternion.LookRotation(transform.InverseTransformDirection(targetRotator.getMovingDirection()), transform.InverseTransformDirection(Vector3.up));
        lineObject.transform.parent.rotation = Quaternion.LookRotation(transform.forward, transform.up);
        lineObjectAR.transform.parent.rotation = Quaternion.LookRotation(transform.forward, transform.up);

        float circleUpRotation = 0;
        if(runs[currentRun].Item3 == TargetRotator.RotationDirection.CLOCKWISE)
        {
            circleUpRotation = -90f + (currentStartingPosition + targetRotator.degreeToRotate - targetRotator.degreeLeft);
        }
        else
        {
            circleUpRotation = -missDegree + targetRotator.degreeLeft;
        }
        
        lineObject.transform.localEulerAngles = new Vector3(0, circleUpRotation, 0);
        lineObjectAR.transform.localEulerAngles = new Vector3(0, circleUpRotation, 0);

        DrawCircle dc = lineObject.GetComponent<DrawCircle>();
        DrawCircle dcAR = lineObjectAR.GetComponent<DrawCircle>();

        dc._degreeToDraw = 90f;// - targetRotator.degreeLeft;
        dcAR._degreeToDraw = 90f;// - targetRotator.degreeLeft;

        LineRenderer lr = lineObject.GetComponent<LineRenderer>();
        LineRenderer lrAR = lineObjectAR.GetComponent<LineRenderer>();
        if (performanceStudyState == USER_MISSED || performanceStudyState == ROTATING || !checkUserPress())
        {
            // Nutzer hat miss. Zeige rote Pfeile
            //arrowAnimator.SetTrigger("Red");
            lr.material.color = Color.red; 
            lrAR.material.color = Color.red; 
        }else
        {
            // Nutzer korrekt. Ist die Richtung korrekt?
            if (userCorrectDirection)
            {
                lr.material.color = Color.green;
                lrAR.material.color = Color.green;
                //arrowAnimator.SetTrigger("Green");
            }
            else
            {
                lr.material.color = new Color(1f, 0.67f, 0f);
                lrAR.material.color = new Color(1f, 0.67f, 0f);
                //arrowAnimator.SetTrigger("Orange");
            }
                
        }
        dc.CreatePoints();
        dcAR.CreatePoints();
    }    

    void hideDirection()
    {
        // Richtung ausblenden
        lineObject.SetActive(false);
        lineObjectAR.SetActive(false);
        //if (arrowAnimator.gameObject.activeSelf)
          //  arrowAnimator.SetTrigger("Stop");
        //arrowAnimator.gameObject.SetActive(false);
    }

    void targetVisible(bool visible)
    {
        fakeSphere.GetComponent<MeshRenderer>().enabled= visible;
        fakeSphere.transform.position = targetRotator.targetToRotate.transform.position;
        targetRotator.targetToRotate.GetComponent<MeshRenderer>().enabled = visible;

    }

    void calcRunsRandomBlocked(int count)
    {
        // Berechnet die Runs. Feedbacks werden in "count" angelegt und innerhalb der Blöcke gemischt.

        // Init List with correct length
        runs = new List<Tuple<OuterFeedback, TransitionFeedback, TargetRotator.RotationDirection>>(count * Mathf.Min(selectedOuterFeedbacks.Length, selectedTransitionFeedbacks.Length));
        
        for (int i = 0; i < count; i++)
        {
            List<Tuple<OuterFeedback, TransitionFeedback, TargetRotator.RotationDirection>>  tmpList = new List<Tuple<OuterFeedback, TransitionFeedback, TargetRotator.RotationDirection>>( Mathf.Min(selectedOuterFeedbacks.Length, selectedTransitionFeedbacks.Length));
            for (int j = 0; j < Mathf.Min(selectedOuterFeedbacks.Length, selectedTransitionFeedbacks.Length); j++)
            {
                tmpList.Add(new Tuple<OuterFeedback, TransitionFeedback, TargetRotator.RotationDirection>(
                    selectedOuterFeedbacks[j],
                    selectedTransitionFeedbacks[j],
                    (TargetRotator.RotationDirection) UnityEngine.Random.Range(0,2)
                ));
            }
            ShuffleList(tmpList);
            runs.AddRange(tmpList);
        }

        if (runs.Count < 1)
            Debug.LogWarning("No runs available. Can't start study");

    }

    public static void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i);
            T temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }
    }

    public static void ShuffleArray<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i);
            T temp = array[i];
            array[i] = array[r];
            array[r] = temp;
        }
    }

    public string SetModeTextGUI(string outer, string inner)
    {
        string outerPrefix = "Outer Feedback: ";
        string outerSuffix;
        string innerPrefix = "Inner Feedback: ";
        string innerSuffix;

        outerSuffix = EncodeMode(outer);
        innerSuffix = EncodeMode(inner);

        return outerPrefix + outerSuffix + "\n" + innerPrefix + innerSuffix;
    }

    public string EncodeMode(string input)
    {
        string code = "";
        switch (input)
        {
            case ("A"):
                code = "Audio";
                break;
            case ("V"):
                code = "Visual";
                break;
            case ("T"):
                code = "Tactile";
                break;
        }
        return code;
    }

}
