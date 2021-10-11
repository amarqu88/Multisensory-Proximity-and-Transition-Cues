using UnityEngine;
using static PerformanceStudy;

// UserInput am Computer. Für VR-Controller ein neues Script mit ähnlicher Logik anlegen oder den INPUt hier entsprechend mappen
// TODO: Aktuell drückt Nutzer einmal für das hereinkommen des targets. Danach drückt er für die Richtung. Reicht es hier nicht aus nur die Richtung zu drücken?
// TODO: Dafür muss Verhalten in PerformanceStudy geändert werden.
public class PCStudyInput : MonoBehaviour
{
    public KeyCode rotationKey = KeyCode.Space;
    public KeyCode selectLeftKey = KeyCode.LeftArrow;
    public KeyCode selectRightKey = KeyCode.RightArrow;
    public KeyCode primarySelectKey = KeyCode.A;

    public PerformanceStudy performanceStudy;
    public PrimaryTask primaryTask;

    private void Update()
    {
        if(Input.GetKeyDown(rotationKey))
        {
            performanceStudy.startStudy();
        }

        if (performanceStudy.performanceStudyState == PerformanceStudyState.BEFORE_ROTATE && Input.GetKeyDown(rotationKey) && performanceStudy.isTraining)
        {
            performanceStudy.startNextRun();
        }

        // Kann auch mit axis-parametern gemacht werden
        // Input.GetAxis("Horizontal")
        if (Input.GetKeyDown(selectLeftKey))
        {
            performanceStudy.selectDirection(new Vector2(-1, 0));
        } else if(Input.GetKeyDown(selectRightKey))
        {
            performanceStudy.selectDirection(new Vector2(1, 0));
        }

        if(Input.GetKeyDown(primarySelectKey))
        {
            primaryTask.userSubmit();
        }
    }
}
