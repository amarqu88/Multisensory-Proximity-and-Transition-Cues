using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEditor;

public class RotationStudy : MonoBehaviour
{
    public bool startWithShowcase;
    public GameObject myButtons;
    public RotateAroundCam cameraRotator;
    public UIManager uiManager;
    public GameObject modeText;
    
    public OuterFeedback[] availableOuterFeedbacks;
    public TransitionFeedback[] availableTransitionFeedbacks;

    public List<Tuple<OuterFeedback, TransitionFeedback>> combinations;
    bool nextIsRival = false;
    private string decisionString = "";
    private bool madeDecision = false;
    private string firstMode, secondMode;
    private string currentFileName;
    int[,] rivals = new int[30, 2]
    {
        {0 , 1},{0 , 2},{0 , 3},{0 , 4},{0 , 5},
        {1 , 0},{1 , 2},{1 , 3},{1 , 4},{1 , 5},
        {2 , 0},{2 , 1},{2 , 3},{2 , 4},{2 , 5},
        {3 , 0},{3 , 1},{3 , 2},{3 , 4},{3 , 5},
        {4 , 0},{4 , 1},{4 , 2},{4 , 3},{4 , 5},
        {5 , 0},{5 , 1},{5 , 2},{5 , 3},{5 , 4}
    };

    List<int> randomOrder;
    int currentCombination = -6;

    bool showCaseRunning = false;
    bool studyRunning = false;
    bool rivalsRunning = false;
    bool started = false;
    bool allRotationsDone = false;

    void trialDone()
    {
        cameraRotator.fm.disableFeedbacks();
        if (currentCombination < randomOrder.Count)
        {
            if (showCaseRunning && currentCombination < 0)
            {
                // Next showcase
                StartCoroutine(startShowCaseAfterSeconds(1.5f));
            }
            else if (showCaseRunning && currentCombination == 0)
            {
                showCaseRunning = false;
            }
            if (studyRunning)
            {
                if (nextIsRival)
                {
                    // next is Rival
                    StartCoroutine(startAfterSeconds(0.5f));
                }
                else
                {
                    rivalsRunning = false;

                    ShowDecisionButton(true);

                    if (modeText != null)
                    {
                        modeText.GetComponent<TextMeshProUGUI>().text = "Please select preferred mode.";
                    }
                }
            }
        }else
        {
            print("ESLE");
            // Done with rotation part. Disable Listener and go to UI
            cameraRotator.TrialCompleteEvent -= trialDone;
            allRotationsDone = true;
            ShowDecisionButton(true);
            //uiManager.enableUI();
        }
    }

    private void Start()
    {
        ShowDecisionButton(false);
        calcRandomOrder();
    }

    void startShowCase()
    {
        cameraRotator.TrialCompleteEvent += trialDone;
        showCaseRunning = true;
        StartCoroutine(startShowCaseAfterSeconds(1));
    }

    void continueStudy()
    {
        if (startWithShowcase)
        {
            if (!started)
            {

                showCaseRunning = true;
                started = true;
                startShowCase();
            }
            else
            {
                if (!showCaseRunning)
                {
                    if (!rivalsRunning && currentCombination < randomOrder.Count)
                    {
                        rivalsRunning = true;
                        studyRunning = true;
                        StartCoroutine(startAfterSeconds(0.5f));
                    }
                }
            }
        }
        else
        {
            startWithShowcase = true;
            cameraRotator.TrialCompleteEvent += trialDone;
            showCaseRunning = false;
            started = true;
            currentCombination = 0;
            rivalsRunning = true;
            studyRunning = true;
            StartCoroutine(startAfterSeconds(0.5f));
        }

    }

    private void Update()
    {

        if (!allRotationsDone)
        {

        }
        else
        {
        }

        if (madeDecision)
            {
            ShowDecisionButton(false);
            madeDecision = false;
            continueStudy();

            }


        if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("SPACE");
            if (!madeDecision)
            {
                decisionString = firstMode + ";";
                madeDecision = true;
            }
            else
            {
                continueStudy();
            }
            }

    }

    void calcRandomOrder()
    {
        List<int> randomOrder0 = new List<int>();
        for (int i = 0; i < 30; i++)
        {
            randomOrder0.Add(i);
        }

        Shuffle(randomOrder0);

        randomOrder = new List<int>();
        randomOrder.AddRange(randomOrder0);

        combinations = new List<Tuple<OuterFeedback, TransitionFeedback>>()
        {
            new Tuple<OuterFeedback, TransitionFeedback>(availableOuterFeedbacks[0], availableTransitionFeedbacks[0]),
            new Tuple<OuterFeedback, TransitionFeedback>(availableOuterFeedbacks[0], availableTransitionFeedbacks[1]),
            new Tuple<OuterFeedback, TransitionFeedback>(availableOuterFeedbacks[0], availableTransitionFeedbacks[2]),
            new Tuple<OuterFeedback, TransitionFeedback>(availableOuterFeedbacks[1], availableTransitionFeedbacks[0]),
            new Tuple<OuterFeedback, TransitionFeedback>(availableOuterFeedbacks[1], availableTransitionFeedbacks[1]),
            new Tuple<OuterFeedback, TransitionFeedback>(availableOuterFeedbacks[1], availableTransitionFeedbacks[2])
        };

        string s = "";//"\"Combination\";";
        for (int i = 0; i < randomOrder.Count; i++)
        {
            Tuple<OuterFeedback, TransitionFeedback> r0 = combinations[rivals[randomOrder[i], 0]];
            Tuple<OuterFeedback, TransitionFeedback> r1 = combinations[rivals[randomOrder[i], 1]];
            s += "\"" + r0.Item1.getShortcut() + r0.Item2.getShortcut() + "-" + r1.Item1.getShortcut() + r1.Item2.getShortcut() + "\";";
        }
        s = s.Remove(s.Length - 1, 1);
        Debug.Log(s);
        File.WriteAllText(CreateFileName(), s);
        File.AppendAllText(currentFileName, Environment.NewLine);
    }

    IEnumerator startShowCaseAfterSeconds(float f)
    {
        yield return new WaitForSeconds(f);
        if (currentCombination < 0)
        {
            cameraRotator.fm.outerFeedback = combinations[Mathf.Abs(currentCombination + 1)].Item1;
            cameraRotator.fm.transitionFeedback = combinations[Mathf.Abs(currentCombination + 1)].Item2;
            currentCombination++;
        }
        cameraRotator.setUpTrial();
        cameraRotator.startTrial();
    }

    IEnumerator startAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (nextIsRival)
        {
            cameraRotator.fm.outerFeedback = combinations[rivals[randomOrder[currentCombination], 1]].Item1;
            cameraRotator.fm.transitionFeedback = combinations[rivals[randomOrder[currentCombination], 1]].Item2;
            Debug.Log("Second of two: " + cameraRotator.fm.outerFeedback.getShortcut() + cameraRotator.fm.transitionFeedback.getShortcut());

            if (modeText != null)
            {
                modeText.GetComponent<TextMeshProUGUI>().text = "2"; // "cameraRotator.fm.transitionFeedback.getShortcut()";
            }
            secondMode = cameraRotator.fm.outerFeedback.getShortcut() + cameraRotator.fm.transitionFeedback.getShortcut();
        }
        else
        {
            cameraRotator.fm.outerFeedback = combinations[rivals[randomOrder[currentCombination], 0]].Item1;
            cameraRotator.fm.transitionFeedback = combinations[rivals[randomOrder[currentCombination], 0]].Item2;
            Debug.Log("First of two: " + cameraRotator.fm.outerFeedback.getShortcut() + cameraRotator.fm.transitionFeedback.getShortcut());
            if (modeText != null)
            {
                modeText.GetComponent<TextMeshProUGUI>().text = "1";// 
            }
            firstMode = cameraRotator.fm.outerFeedback.getShortcut() + cameraRotator.fm.transitionFeedback.getShortcut();
        }

        nextIsRival = !nextIsRival;

        if (!nextIsRival)
            currentCombination++;

        cameraRotator.setUpTrial();
        cameraRotator.startTrial();
    }

    public static void Shuffle<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i);
            Swap(ref array[i], ref array[r]);
        }
    }


    //-------------------------------------------------
    public static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i);
            T temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }
    }

    public static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    public void DecisionWasMade(int i)
    {
        madeDecision = true;
        if(i == 1)
        {
            decisionString = firstMode + ";";

        }
        else
        {
            decisionString = secondMode + ";";

        }

        WriteString(decisionString);
        print("Answer: " + decisionString);
    }

    void WriteString(string s)
    {
        using (StreamWriter sw = File.AppendText(currentFileName))
        {
            sw.Write(s);
        }
    }

    private string CreateFileName()
    {
        string m_Path = Application.dataPath;
        int i = 0;
        string fileName = m_Path + "/Study1-P" + i.ToString() + ".csv";

        while (File.Exists(fileName))
        {
            fileName = m_Path + "/Study1-P" + i++.ToString() + ".csv";
        }

        Debug.Log(fileName);
        currentFileName = fileName;
        return fileName;
    }

    void ShowDecisionButton(bool b)
    {
        if (myButtons != null)
        {
            myButtons.SetActive(b);
        }
    }

}
