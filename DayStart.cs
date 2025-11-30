using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class DayStart : MonoBehaviour
{
    [SerializeField] GameObject IntroPanel;
    [SerializeField] Texture2D cursorTexture;
    [SerializeField] PlayerController playerController;
    [SerializeField] ConsoleController console;
    [SerializeField] TextMeshProUGUI DayCounter;
    [SerializeField] TextMeshProUGUI ShiftTime;
    [SerializeField] TextMeshProUGUI RequiredAmount;
    [SerializeField] TextMeshProUGUI lifeSupportTime;
    [SerializeField] TextMeshProUGUI TOSText;
    bool gameStarted = false;

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width/2, cursorTexture.height/2), CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted)
        {
            Time.timeScale = 0f;
        }

        DayCounter.text = "DAY " + console.day;
        ShiftTime.text = "REMINDER: YOU MAY NOT DOCK UNTIL YOU MEET YOUR QUOTA!";
        lifeSupportTime.text = "LIFE SUPPORT: " + console.lifeSupportDuration + "H";
        RequiredAmount.text = "HOLD [TAB] TO VIEW ORE QUOTAS OF TODAY";
        if (console.day == 1)
        {
            TOSText.text = "CONGRATULATIONS ON BEING ACCEPTED INTO THE HAZARDOUS ASTEROID AND RESOURCE VOLATILE EXTRACTION SYSTEMS THROUGH ENGINEERED ROUTINES (H.A.R.V.E.S.T.E.R) PROGRAM! VERY FEW MANAGE TO GET INTO SUCH AN HONORED AND EXCELLENT JOB WITHOUT TREMENDOUS EFFORT. AS THIS IS YOUR ORIENTATION FLIGHT, YOU DO NOT HAVE ANY QUOTA TO MEET TODAY. SPEND THIS TIME TO GET FAMILIARIZED WITH THE CONTROLS AND THE ENVIRONMENT. OH, AND HELPING EXTRACT A FEW EXTRA RESOURCES FOR THE COMPANY WON'T GO UNAPPRECIATED. MAKE SURE NOT TO GET TOO CLOSE TO ANYTHING THAT GLOWS. OR MOVES... LOTS OF STRANGE THINGS OUT THERE. ANYWAY, IF YOU HAVE ANY QUESTIONS, PLEASE REFER TO YOUR SPACECRAFT'S MANUAL (PRESS ESCAPE AND OPEN THE HELP MENU). AGAIN, CONGRATULATIONS ON YOUR NEW JOB. WE ARE SO GLAD TO HAVE YOU AS PART OF THE SHERGEO FAMILY!";
        }
        else if (console.day == 2)
        {
            TOSText.text = "GOOD MORNING SHERGEO FAMILY. WE HAVE RECEIVED NUMEROUS CONCERNS ABOUT A BLINDING LIGHT FLOODING SEVERAL LOCAL SYSTEMS. DO NOT WORRY, FOR ALL ISSUES HAVE NOW BEEN RESOLVED. ANY DETAILS REGARDING A “SITE DISASTER” ARE MERELY SPECULATION. KEEP IN MIND THAT SHREGEO POLICY ENFORCES THE HIGHEST STANDARDS OF MINING SAFETY IN THE GALAXY. THE DEPARTMENT OF SELF-ACCOUNTABILITY IS LOOKING INTO THE EVENT.";
        }
        else if(console.day == 3)
        {
            TOSText.text = "GOOD MORNING SHERGEO FAMILY! WE HAVE RECEIVED NUMEROUS CONCERNS REGARDING LARGE AMOUNTS OF ASTEROID DEBRIS MAKING MINING CONDITIONS UNSAFE. DO NOT BELIEVE OR REPEAT ANY INFORMATION THAT HAS NOT BEEN VERIFIED THROUGH ANY OF THE SHERGEO COMPANY’S SUBSIDIARIES, AND WAIT AS THE DEPARTMENT OF SELF-ACCOUNTABILITY INVESTIGATES THE SUDDEN STORM THAT ARE DUE TO NATURAL CAUSES.";
        }
        else if(console.day == 4)
        {
            TOSText.text = "ATTENTION SHERGEO FAMILY! YESTERDAY, OVER 20 UNION RADICALS HAVE BEEN DISCOVERED AND DEALT WITH ACCORDINGLY. IF YOU OR A SUSPECTED LOVED ONE IS SUSPECTED TO BE A SELF-PROCLAIMED UNION MEMBER, OR PARROTS UNION TALKING-POINTS, PLEASE REFER THEM TO THE NEAREST DEPARTMENT OF MUTUAL ACCOUNTABILITY SO THAT YOU OR THEY MAY BE PERMANENTLY SPIRITUALLY RELOCATED. THANK YOU FOR YOUR ATTENTION! \r\n";
        }
        else if(console.day == 5)
        {
            TOSText.text = "ATTENTION SHERGEO FAMILY! OUR MISSILEBOTS HAVE BEEN HARD AT WORK RESTORING SAFETY TO SEVERAL SYSTEMS. ALL SECTORS WITH A SECURITY LEVEL OF 4 OR HIGHER ARE NOW OPEN ONCE AGAIN.";
        }
        else if (console.day == 6)
        {
            TOSText.text = "ATTENTION SHERGEO FAMILY! THERE IS NO NEWS TODAY! THANK YOU FOR YOUR ATTENTION!\r\n";
        }
        else if (console.day == 7)
        {
            TOSText.text = "ATTENTION SHERGEO FAMILY! RATIONS WILL NOT EXIST TODAY. THANK YOU FOR YOUR UNDERSTANDING!\r\n";
        }
        else if (console.day == 8)
        {
            TOSText.text = "ATTENTION SHERGEO FAMILY! STARTING FROM TODAY, EACH SHIP WILL BEGIN SEEING NEW MANDATORY MODIFICATIONS (FULLY PAID BY EACH PILOT/COMMANDER/CREW MEMBER). THANK YOU FOR YOUR UNDERSTANDING!\r\n";
        }
        else
        {
            TOSText.text = "ATTENTION SHERGEO FAMILY! THERE IS NO NEWS TODAY. THANK YOU FOR YOUR ATTENTION!";
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        playerController.enabled = true;
        Time.timeScale = 1f;
        StartCoroutine("fadeOut");
    }

    IEnumerator fadeOut()
    {
        IntroPanel.GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(1f);
        IntroPanel.SetActive(false);
    }
}
