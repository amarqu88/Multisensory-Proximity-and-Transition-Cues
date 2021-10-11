using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public TargetMeshChecker tmc;

    public Canvas canvas;
    public GameObject ARDisplay;
    public RotationStudy rotationStudy;
    public RotateAroundCam rotAroundCam;
    int currentCombination = 0;
    public Button submitBtn;

    public List<Tuple<OuterFeedback, TransitionFeedback>> combinations;
    public GameObject[] enabledOnUI;

    public Slider transitionStart;
    public Slider transitionStop;
    public Slider peripheralStart;
//    public Slider peripheralStop;

    public GameObject transitionStartText;
    public GameObject transitionStopText;
    public GameObject peripheralStartText;
//    public GameObject peripheralStopText;

    public float transitionStartValue;
    public float transitionStopValue;
    public float peripheralStartValue;
    public float peripheralStopValue;


    private Vector2 standardTransitionInStart;
    private Vector2 standardTransitionInStop;
    private Vector2 standardTransitionOutStart;
    private Vector2 standardTransitionOutStop;
    private Vector2 standardperipheralStart;
    private Vector2 standardperipheralStop;

    // Start is called before the first frame update
    void Start()
    {
        transitionStart.onValueChanged.AddListener(delegate { ChangeFrustumValue(frustum.tStart); });
        transitionStop.onValueChanged.AddListener(delegate { ChangeFrustumValue(frustum.tStop); });
        peripheralStart.onValueChanged.AddListener(delegate { ChangeFrustumValue(frustum.pStart); });
//        peripheralStop.onValueChanged.AddListener(delegate { ChangeFrustumValue(frustum.pStop); });

        standardTransitionInStart = tmc.transitionInStartFrustumDegree;
        standardTransitionOutStart = tmc.transitionOutStartFrustumDegree;
        standardTransitionInStop = tmc.transitionInStopFrustumDegree;
        standardTransitionOutStop = tmc.transitionOutStopFrustumDegree;
        standardperipheralStart = tmc.outerFeedbackStartFrustumDegree;
        standardperipheralStop = tmc.outerFeedbackStopFrustumDegree;
    }

//    [Sirenix.OdinInspector.Button]
    public void enableUI()
    {
        canvas.gameObject.SetActive(true);
        ARDisplay.SetActive(false);
        if (enabledOnUI != null)
        {
            foreach (GameObject go in enabledOnUI)
            {
                go.SetActive(true);
            }
        }
        combinations = new List<Tuple<OuterFeedback, TransitionFeedback>>()
        {
            new Tuple<OuterFeedback, TransitionFeedback>(rotationStudy.availableOuterFeedbacks[0], rotationStudy.availableTransitionFeedbacks[0]),
            new Tuple<OuterFeedback, TransitionFeedback>(rotationStudy.availableOuterFeedbacks[0], rotationStudy.availableTransitionFeedbacks[1]),
            new Tuple<OuterFeedback, TransitionFeedback>(rotationStudy.availableOuterFeedbacks[0], rotationStudy.availableTransitionFeedbacks[2]),
            new Tuple<OuterFeedback, TransitionFeedback>(rotationStudy.availableOuterFeedbacks[1], rotationStudy.availableTransitionFeedbacks[0]),
            new Tuple<OuterFeedback, TransitionFeedback>(rotationStudy.availableOuterFeedbacks[1], rotationStudy.availableTransitionFeedbacks[1]),
            new Tuple<OuterFeedback, TransitionFeedback>(rotationStudy.availableOuterFeedbacks[1], rotationStudy.availableTransitionFeedbacks[2])
        };

        rotAroundCam.stopTrial();
        rotAroundCam.TrialCompleteEvent += trialDone;
    }

    void prepareNextFeedback()
    {
        if (currentCombination < combinations.Count)
        {
            rotAroundCam.fm.outerFeedback = combinations[currentCombination].Item1;
            rotAroundCam.fm.transitionFeedback = combinations[currentCombination].Item2;
            currentCombination++;
        }
    }


    public void runTrial()
    {
        canvas.gameObject.SetActive(false);
        ARDisplay.SetActive(true);
        tmc.GenerateMeshes();
        rotAroundCam.setUpTrial();
        rotAroundCam.startTrial();
        if (enabledOnUI != null)
        {
            foreach (GameObject go in enabledOnUI)
            {
                go.SetActive(false);
            }
        }
    }

    public void submit()
    {
        // TODO: Save values
        canvas.gameObject.SetActive(true);
        ARDisplay.SetActive(false);
        tmc.transitionInStartFrustumDegree = standardTransitionInStart;
        tmc.transitionOutStartFrustumDegree = standardTransitionOutStart;
        tmc.transitionInStopFrustumDegree = standardTransitionInStop;
        tmc.transitionOutStopFrustumDegree = standardTransitionOutStop;
        tmc.outerFeedbackStartFrustumDegree = standardperipheralStart;
        tmc.outerFeedbackStopFrustumDegree = standardperipheralStop;

        transitionStart.value = 0;
        transitionStop.value = 0;
        transitionStart.value = 0;
        tmc.GenerateMeshes();
        if(currentCombination < combinations.Count)
        {
            submitBtn.interactable = false;
            if (enabledOnUI != null)
            {
                foreach (GameObject go in enabledOnUI)
                {
                    go.SetActive(true);
                }
            }
            prepareNextFeedback();
        }else
        {
            canvas.gameObject.SetActive(false);
        }
    }

    void trialDone()
    {
        canvas.gameObject.SetActive(true);
        ARDisplay.SetActive(false);
        submitBtn.interactable = true;
        if (enabledOnUI != null)
        {
            foreach (GameObject go in enabledOnUI)
            {
                go.SetActive(true);
            }
        }
    }

    public void ChangeFrustumValue(frustum f)
    {
        transitionStartValue = transitionStart.value;
        transitionStopValue = transitionStop.value;
        peripheralStartValue = peripheralStart.value;
 //       peripheralStopValue = peripheralStop.value;
        

        transitionStartText.GetComponent<TextMeshProUGUI>().SetText((transitionStartValue / 10).ToString());
        transitionStopText.GetComponent<TextMeshProUGUI>().SetText((transitionStopValue / 10).ToString());
        peripheralStartText.GetComponent<TextMeshProUGUI>().SetText((peripheralStartValue / 10).ToString());
 //       peripheralStopText.GetComponent<TextMeshProUGUI>().SetText((peripheralStopValue / 10).ToString());

        switch (f)
        {
            case frustum.tStart:

                tmc.transitionInStartFrustumDegree.x = standardTransitionInStart.x + transitionStartValue;
                tmc.transitionInStartFrustumDegree.y = tmc.transitionInStartFrustumDegree.x / (tmc.simulatedFrustumDegree.x / tmc.simulatedFrustumDegree.y);
                tmc.transitionOutStartFrustumDegree.x = standardTransitionOutStart.x + transitionStartValue;
                tmc.transitionOutStartFrustumDegree.y = tmc.transitionOutStartFrustumDegree.x / (tmc.simulatedFrustumDegree.x / tmc.simulatedFrustumDegree.y);
                break;
            case frustum.tStop:
                tmc.transitionInStopFrustumDegree.x = standardTransitionInStop.x + transitionStopValue;
                tmc.transitionInStopFrustumDegree.y = tmc.transitionInStopFrustumDegree.x / (tmc.simulatedFrustumDegree.x / tmc.simulatedFrustumDegree.y);
                tmc.transitionOutStopFrustumDegree.x = standardTransitionOutStop.x + transitionStopValue;
                tmc.transitionOutStopFrustumDegree.y = tmc.transitionOutStopFrustumDegree.x / (tmc.simulatedFrustumDegree.x / tmc.simulatedFrustumDegree.y);
                break;

            case frustum.pStart:
                tmc.outerFeedbackStartFrustumDegree.x = standardperipheralStart.x + peripheralStartValue;
                tmc.outerFeedbackStartFrustumDegree.y = tmc.outerFeedbackStartFrustumDegree.x / (tmc.simulatedFrustumDegree.x / tmc.simulatedFrustumDegree.y);
                tmc.outerFeedbackStopFrustumDegree.x = standardperipheralStop.x + peripheralStartValue;
                tmc.outerFeedbackStopFrustumDegree.y = tmc.outerFeedbackStopFrustumDegree.x / (tmc.simulatedFrustumDegree.x / tmc.simulatedFrustumDegree.y);
                break;
            case frustum.pStop:
                break;
        }
    }

    public enum frustum
    {
        tStart,
        tStop,
        pStart,
        pStop
    }

}


