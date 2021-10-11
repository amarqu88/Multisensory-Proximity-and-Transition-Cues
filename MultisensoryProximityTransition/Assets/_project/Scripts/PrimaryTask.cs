using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using Sirenix.OdinInspector;
using System.IO;
using System;

public class PrimaryTask : MonoBehaviour
{
    public TextMeshProUGUI pTaskText;
    public int luckyNumber;
    public float RandomNumberSpeed;
    public Vector2 timeIntervalForLuckyNumber;
    public bool luckyNumberIsVisible;
    List<int> crazyNumberList = new List<int>();

    private float appearanceTime;
    float timeLeftToLuckyNumber = 0f;
    float timeLeftToCurrentNumber = 0f;
    public bool taskStarted = false;
    public bool taskRunning = false;
    float pausedTime = 0f;
    string currentFileName;
    int currentNumber = -1;
    bool userPressed;
    int logLuckyNumberCount = 0;
    float logAppearanceTime;
    float logVanishTime;
    bool logIsLuckyNumber;
    bool lastWasLucky = false;
    int logActualNumber;
    bool logUserPressed;
    bool logHit;
    bool isTraining = true;
    float logTimeOnSubmit=-1;
    public AudioClip soundSuccess;
    public AudioClip soundError;
    public AudioSource audioSource;
    Coroutine c;

    void initLogFile()
    {
        string m_Path = Application.dataPath;
        int i = 0;
        string fileName = m_Path + "/Study-Primary" + i.ToString() + ".csv";

        while (File.Exists(fileName))
        {
            fileName = m_Path + "/Study-Primary" + i++.ToString() + ".csv";
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
        string logString = string.Concat(
            logLuckyNumberCount, ";", logAppearanceTime, ";", logVanishTime, ";", logActualNumber, ";",
            logIsLuckyNumber, ";", logUserPressed, ";", logTimeOnSubmit, ";",
            logHit, Environment.NewLine);

        WriteLogString(logString);
        
    }

    private void Start()
    {
        initLogFile();
        WriteLogString("LuckyNumberCount;AppearanceTime;VanishTime;ActualNumber;IsLuckyNumber;UserPressed;SubmitTime;Hit" + Environment.NewLine);
    }

    public List<int> CreateRandomNumberList(int length)
    {
        List<int> numbersToChooseFrom = new List<int>(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9});
        numbersToChooseFrom.Remove(luckyNumber);

        List<int> tempList = new List<int>();

        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        int lastNumber = -1;
        for (int i = 0; i < length; i++)
        {
            int chosenNumberIndex = UnityEngine.Random.Range(0, numbersToChooseFrom.Count);
            do
            {
                chosenNumberIndex = UnityEngine.Random.Range(0, numbersToChooseFrom.Count);
            } while (lastNumber == numbersToChooseFrom[chosenNumberIndex]);
            tempList.Add(numbersToChooseFrom[chosenNumberIndex]);
            lastNumber = numbersToChooseFrom[chosenNumberIndex];


        }
        return tempList;
    }

    public void printList(List<int> list)
    {
        string listText = "";
        foreach (int no in list)
        {
            listText += no.ToString() + " ";
        }
 //       print(listText);
    }

    public string numberToShow()
    {
        int n;
        if (!luckyNumberIsVisible)
        {
            n = crazyNumberList[0];
            crazyNumberList.RemoveAt(0);
        }
        else
        {
            logLuckyNumberCount++;
            n = luckyNumber;
        }
        currentNumber = n;
        return n.ToString();
    }
    IEnumerator playSoundAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        audioSource.clip = soundError;
        audioSource.Play();

    }

    void Update()
    {
        if(taskRunning)
        {
            timeLeftToLuckyNumber -= Time.deltaTime;
            timeLeftToCurrentNumber -= Time.deltaTime;
            if(timeLeftToCurrentNumber <= 0f)
            {
                logVanishTime = Time.realtimeSinceStartup;
                logActualNumber = currentNumber;
                logIsLuckyNumber = currentNumber == luckyNumber;
                logUserPressed = userPressed;
                if(logIsLuckyNumber)
                {
                    lastWasLucky = true;
                    if (logUserPressed)
                        logHit = true;
                    else
                        logHit = false;
                }else
                {
                    lastWasLucky = false;
                    if (logUserPressed)
                        logHit = false;
                    else
                        logHit = true;
                }
                // Log here
                if (isTraining && lastWasLucky && !userPressed)
                {
                    audioSource.clip = soundError;
                    audioSource.Play();
                }
                logTrial();

                
                userPressed = false;
                logTimeOnSubmit = -1f;

                luckyNumberIsVisible = false;

                if (timeLeftToLuckyNumber <= 0f)
                {
                    // set Lucky Number
                    luckyNumberIsVisible = true;
                    // set time to show next lucky number
                    timeLeftToLuckyNumber = UnityEngine.Random.Range(timeIntervalForLuckyNumber.x, timeIntervalForLuckyNumber.y);
                    appearanceTime = Time.realtimeSinceStartup;
                    
                }
                logAppearanceTime = Time.realtimeSinceStartup;
                // Get next number
                // Show next Number
                pTaskText.text = numberToShow();

                // set time to show next number
                timeLeftToCurrentNumber = RandomNumberSpeed;
            }
        }
    }

    public void userSubmit()
    {
        if (taskRunning)
        {
            if (!userPressed)
            {
                userPressed = true;
                logTimeOnSubmit = Time.realtimeSinceStartup;
                if (luckyNumberIsVisible)
                {
                    if (isTraining)
                    {
                        if (c != null)
                            StopCoroutine(c);
                        audioSource.clip = soundSuccess;
                        audioSource.Play();
                    }
                }
                else
                {
                    if (isTraining)
                    {
                        if (logVanishTime > 0 && logTimeOnSubmit - logTimeOnSubmit <= .5f && lastWasLucky)
                        {
                            if (c != null)
                                StopCoroutine(c);
                            audioSource.clip = soundSuccess;
                            audioSource.Play();
                        }
                        else
                        {

                            if (c != null)
                                StopCoroutine(c);
                            audioSource.clip = soundError;
                            audioSource.Play();
                        }
                    }

                }
            }

        }else
        {
            //print("Primary Task not running");
        }
    }

 //   [Button]
    public void startNumberTask(bool training)
    {
        if (!taskStarted)
        {
            this.isTraining = training;
            logLuckyNumberCount = -1;
            logAppearanceTime = -1;
            logVanishTime =-1;
            logIsLuckyNumber = false;
            logActualNumber = -1;
            logUserPressed = false;
            logHit = false;
            if(crazyNumberList == null || crazyNumberList.Count < 1)
                crazyNumberList = CreateRandomNumberList(9999);
            luckyNumberIsVisible = false;
            pTaskText.text = numberToShow();
            numbersVisible(true);
            timeLeftToCurrentNumber = RandomNumberSpeed;
            timeLeftToLuckyNumber = UnityEngine.Random.Range(1f, 2f);
            taskStarted = true;
            taskRunning = true;
        }
    }

    

//    [Button]
    public void stopNumberTask()
    {
        // Dont forget to log last Number if it was running
        if (taskStarted)
        {
            // Log current number
            if (luckyNumberIsVisible)
            {
                logVanishTime = -1f;
                // Current number is luckynumber. Is this a miss?
            }
        }
        if(c != null)
            StopCoroutine(c);
        userPressed = false;
        logTimeOnSubmit = -1f;
        taskRunning = false;
        taskStarted = false;
        appearanceTime = 0;
        timeLeftToLuckyNumber = 0f;
        timeLeftToCurrentNumber = 0f;
        taskStarted = false;
        taskRunning = false;
        pausedTime = 0f;
        luckyNumberIsVisible = false;
        numbersVisible(false);
    }

    void numbersVisible(bool visible)
    {
        pTaskText.enabled = visible;
    }
}
