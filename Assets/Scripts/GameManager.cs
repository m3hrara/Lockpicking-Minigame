using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject lockPick;
    public Transform innerLock;
    public Transform pickTransform;
    public Camera camera;
    public Slider timeSlider;
    public TMP_Text messageBox;
    public TMP_Text skillText;
    public TMP_Text timerText;
    public GameObject CheckOne;
    public GameObject CheckTwo;
    public GameObject CheckThree;
    public GameObject RestartButton;

    private float pickAngle;
    private float openAngle;
    private Vector2 unlockRange;
    private float keyPressedTime = 0;
    private bool canMovePick = true;
    private bool inUnlockZone = false;
    private int stagesComplete = 0;
    private float timer = 60;
    private bool isFinished = false;
    private bool isTimeUp = false;
    private float lockRange=8;
    public float maxAngle = 90;
    //public float lockSpeed = 5;
    public int skillLevel = 1;
    // Start is called before the first frame update
    void Start()
    {
        ResetLock();
        messageBox.text = ("Find The Sweet Spot 3 Times To Open The Lock!");
        RestartButton.SetActive(false);
        canMovePick = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isFinished)
        timer -= Time.deltaTime;
        timerText.text = ("Time: " + timer.ToString("F0"));

        lockPick.transform.localPosition = pickTransform.position;
        skillText.text = skillLevel.ToString();
        if(timer <=0)
        {
            timer = 0;
            GameOver();
        }

        // pick movement
        if (canMovePick)
        {
            Vector3 dir = Input.mousePosition - camera.WorldToScreenPoint(lockPick.transform.position);

            pickAngle = Vector3.Angle(dir, Vector3.up);

            Vector3 cross = Vector3.Cross(Vector3.up, dir);
            if (cross.z < 0)
            {
                pickAngle = -pickAngle;
            }

            pickAngle = Mathf.Clamp(pickAngle, -maxAngle, maxAngle);

            Quaternion pinRotation = Quaternion.AngleAxis(pickAngle, Vector3.forward);
            lockPick.transform.rotation = pinRotation;
        }

        if(inUnlockZone == true)
        {
            keyPressedTime += Time.deltaTime;
        }

        if (stagesComplete == 0)
        {
            CheckOne.SetActive(false);
            CheckTwo.SetActive(false);
            CheckThree.SetActive(false);
        }
        else if (stagesComplete == 1)
        {
            CheckOne.SetActive(true);
            CheckTwo.SetActive(false);
            CheckThree.SetActive(false);
        }
        else if (stagesComplete == 2)
        {
            CheckOne.SetActive(true);
            CheckTwo.SetActive(true);
            CheckThree.SetActive(false);
        }
        else if (stagesComplete == 3)
        {
            CheckOne.SetActive(true);
            CheckTwo.SetActive(true);
            CheckThree.SetActive(true);
            switch (skillLevel)
            {
                case 1:
                    messageBox.text = ("You Have Skilled UP!");
                    skillLevel++;
                    timer = 50;
                    lockRange -= 3;
                    stagesComplete = 0;
                    break;
                case 2:
                    messageBox.text = ("You Have Skilled UP!");
                    skillLevel++;
                    timer = 40;
                    lockRange -= 3;
                    stagesComplete = 0;
                    break;
                case 3:
                    messageBox.text = ("You Have Opened The Lock!");
                    RestartButton.SetActive(true);
                    isFinished = true;
                    isTimeUp = true;
                    canMovePick = false;
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isTimeUp)
        {
            canMovePick = false;
            print("Current angle: " + pickAngle + "Needed angle: " + openAngle);

            if (pickAngle < unlockRange.y && pickAngle > unlockRange.x)
            {
                inUnlockZone = true;
                messageBox.color = Color.green;
                messageBox.text = ("You Are In The Sweet Spot, Keep Holding!");
            }
            else
            {
                messageBox.color = Color.white;
                messageBox.text = ("You Are Not In The Sweet Spot");
            }
        }
        if (Input.GetKeyUp(KeyCode.Space) && !isTimeUp)
        {
            if (keyPressedTime >= 5)
            {
                if (inUnlockZone == true)
                {  
                    CompleteStage();
                }
            }
            else
            {
                messageBox.color = Color.white;
                messageBox.text = ("Keep Tring to Find The Sweet Spot!");
            }
            inUnlockZone = false;
            keyPressedTime = 0;
            canMovePick = true;
        }

        timeSlider.value = keyPressedTime;
    }

    public void CompleteStage()
    {
        //apply skill effect
        int rand = Random.Range(0, 10);
        if (skillLevel ==1 && rand <6)
        {
            messageBox.text = ("You Completed This Stage!");
            stagesComplete++;
        }
        else if (skillLevel == 2 && rand < 7)
        {
            messageBox.text = ("You Completed This Stage!");
            stagesComplete++;
        }
        else if (skillLevel == 3)
        {
            messageBox.text = ("You Completed This Stage!");
            stagesComplete++;
        }
        else
        {
            messageBox.color = Color.red;
            messageBox.text = ("Your Level Was Not High Enough!");
        }
      
        ResetLock();
    }
    void ResetLock()
    {
        openAngle = Random.Range(-maxAngle + lockRange, maxAngle - lockRange);
        unlockRange = new Vector2(openAngle - lockRange, openAngle + lockRange);
    }

    public void SetLockRange(int range)
    {
        lockRange = range;
        ResetLock();
    }

    public void GameOver()
    {
        messageBox.text = ("You Ran Out Of Time!");
        isTimeUp = true;
        RestartButton.SetActive(true);
        canMovePick = false;
    }
}
