using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class kneebookController : MonoBehaviour
{
    [SerializeField] GameObject kneebook;
    
    private int pageNumber = 0;

    [SerializeField] List<string> entries;
    [SerializeField] TextMeshProUGUI entryText;
    [SerializeField] TextMeshProUGUI pageCounter;
    [SerializeField] AudioSource pageFlip;

    [SerializeField] monitorManager monitor;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        entries.Add("THE H.A.R.V.E.S.T.E.R GUIDE! \n\nPROPERTY OF THE SHERGEO COMPANY"); // Page 0

        entries.Add("TABLE OF CONTENTS\n\n2-3: MOVEMENT\n4-7: SYSTEM INFO\n8: SYMBOLOGY GUIDE\n9-12: RADAR & NAVIGATION INFO\n13-19: MINING & LASERS\n20: ECONOMIC GUIDE\n21-33: ORE GUIDE"); // Page 1

        entries.Add("MOVEMENT CONTROLS:\n\nW/S - PITCH\nA/D - ROLL\nLEFT SHIFT/CTRL - THROTTLE"); // Page 2

        entries.Add("IMPORTANT TID BITS:\n\n YOU CAN ENABLE MOUSE MODE IN THE SETTINGS.\n\nHOLD LEFT ALT TO SHOW CURSOR WHILE IN MOUSE MODE.\n\nHOLD DOWN RIGHT CLICK TO LOOK AROUND ."); // Page 3

        entries.Add("SYSTEM CONTROLS:\n\nE - PILOT AUTO LOCK (RADAR V2 REQUIRED)\nSPACEBAR - SECONDARY SYSTEM\nX - UNLOCK TARGET\nF - TOGGLE MONITOR"); // Page 4

        entries.Add("APP INFO:\n\nSCN: SCAN APP. USE FOR ANALYZING ASTEROIDS\nNAV: USE FOR NAVIGATION\nDTA: USE TO DOWNLOAD DATA\nBOOK: THIS IS BOOK\nSTN: USE TO DOCK AND CHECK QUOTAS"); // Page 5

        entries.Add("SYSTEM INFO:\n\nCLICK ON TRACKING SYMBOLS (BLUE CIRCLES) WITH CURSOR TO LOCK.\n\nRADAR V2 HAS NO SPACEBAR FUNCTION.\n\nYOU CANNOT LOCK TARGETS WHILE IN NAV MODE."); // Page 6

        entries.Add("WARNING:\n\nPTI SENSORS ARE NOT ACCURATE.\nLOW PTI READINGS MAY RESULT IN CATASTROPHIC FAILURE."); // Page 7

        entries.Add("SYMBOLOGY GUIDE:\n\nPTI: PREDICTED TARGET INTEGRITY\nHULL: SELF EXPLANATORY\nLF: LIFE SUPPORT REMAINING\nNAV: NAV MODE ACTIVE\nPAL: PILOT AUTO LOCK\nWP: WAYPOINT ACTIVE\nSCAN: RADAR IS SCANNING"); // Page 8

        entries.Add("RADAR INFO:\n\nBLUE CIRCLES = TRACKABLE\nBLUE DIAMOND = LOCKED TARGET\n\nRADAR LOCKS THE FIRST OBJECT IT DETECTS."); // Page 9

        entries.Add("RADAR UPGRADE INFO:\n\nDEFAULT RADAR SHOWS ALL OBJECTS.\nRADAR V2 FILTERS OUT SMALL ASTEROIDS.\n\nIF RADAR APPEARS TO BE FAILING, PRESS 'UNLOCK' TO TURN IT OFF AND ON AGAIN."); // Page 10

        entries.Add("NAV CONTROLS:\n\nM - TOGGLE NAV MODE\n[ / ] - CYCLE WAYPOINT\nZ - WARP TO SELECTED WAYPOINT"); // Page 11

        entries.Add("NAVIGATION INFO:\n\nWARPING REQUIRES AT LEAST ONE FULL FUSION CELL.\n\nNAVIGATION CAN BE HANDLED THROUGH THE HUD OR THE MONTIOR (NAV APP)"); // Page 12

        entries.Add("MINING GUIDE:\n\nMINERALS HAVE UNIQUE VALUES, DENSITY, & MELTING POINTS.\nLARGER ASTEROIDS TEND TO HOLD LARGER ORE DEPOSITS."); // Page 13

        entries.Add("STEP-BY-STEP MINING PROCEDURE:\n\n1. LOCK AN ASTEROID\n2. ANALYZE ASTEROID\n3. NOTE ORE COMPOSITION AND ADJUST LASER AS NEEDED\nHOLD LEFT CLICK TO FIRE LASER AND TRACTOR BEAM."); // Page 14

        entries.Add("LASER INTENSITY:\n\nSCROLL TO ADJUST.\n HIGH INTENSITY = HIGHER YIELD.\nLOW INTENSITY = HIGHER STABILITY"); // Page 15

        entries.Add("INTENSITY WARNING:\n\nTOO MUCH POWER MAY DESTROY LOW-INTEGRITY ORE BEFORE EXTRACTION."); // Page 16

        entries.Add("ANALYSIS TIP:\n\nSEE 'ORE GUIDE' (PG. 21) FOR INTENSITY RECOMMENDATIONS PER ORE TYPE."); // Page 17

        entries.Add("WEAPON CONTROLS:\n\n(HOLD) LEFT CLICK - FIRE LASER\nSCROLL WHEEL - ADJUST INTENSITY"); // Page 18

        entries.Add("TIP:\n\nLASER DAMAGE UPGRADES ARE ONLY FOR SELF DEFENSE.\nLASER DAMAGE UPGRADES HAVE NO EFFECT ON MINING PROCEDURES"); // Page 19

        entries.Add("ECONOMIC GUIDE:\n\nTHE SHERGEO COMPANY RETAINS THE RIGHT TO DEDUCT:\n- UP TO 95% OF CARGO VALUE\n\nAND\n\n- UP TO AN ADDITIONAL $50,000 IN DAILY FEES"); // Page 20

        entries.Add("ORE GUIDE\n\nWRITTEN BY THE SHERGEO COMPANY."); // Page 21

        entries.Add("IRON\n\nINTENSITY: 20–55%\nVALUE: MID\nDENSITY: HIGH"); // Page 22

        entries.Add("CLAY\n\nINTENSITY: 0–25%\nVALUE: LOW\nDENSITY: LOW"); // Page 23

        entries.Add("MAGNESIUM\n\nINTENSITY: 20–55%\nVALUE: HIGH\nDENSITY: MID"); // Page 24

        entries.Add("ALUMINUM\n\nINTENSITY: 20–55%\nVALUE: HIGH\nDENSITY: MID"); // Page 25

        entries.Add("NICKEL\n\nINTENSITY: 50–80%\nVALUE: EXT HIGH\nDENSITY: HIGH"); // Page 26

        entries.Add("COBALT\n\nINTENSITY: 50–80%\nVALUE: EXT HIGH+\nDENSITY: HIGH"); // Page 27

        entries.Add("CARBON\n\nINTENSITY: 20–55%\nVALUE: MID\nDENSITY: MID"); // Page 28

        entries.Add("ICE\n\nINTENSITY: 0–25%\nVALUE: MID\nDENSITY: LOW"); // Page 29

        entries.Add("HYDROGEN\n\nINTENSITY: 0–25%\nVALUE: HIGH\nDENSITY: EXT LOW"); // Page 30

        entries.Add("HELIUM-3\n\nINTENSITY: 75–100%\nVALUE: EXT HIGH++\nDENSITY: EXT LOW"); // Page 31

        entries.Add("DIAMOND\n\nINTENSITY: 75–100%\nVALUE: EXT HIGH+++\nDENSITY: MID"); // Page 32

        entries.Add("PLATINIUM\n\nINTENSITY: 75–100%\nVALUE: EXT HIGH++++\nDENSITY: EXT HIGH"); // Page 33

        entries.Add("PLUTONIUM\n\nINTENSITY: 75–100%\nVALUE: MINE AT ALL COSTS\nDENSITY: EXT HIGH"); // Page 34
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            monitor.openMonitor();
            monitor.loadApp(kneebook);
        }

        entryText.text = entries[pageNumber];
        pageCounter.text = pageNumber.ToString();
    }

    public void rightPage()
    {
        pageNumber++;
        pageFlip.Play();
        if(pageNumber > entries.Count-1)
        {
            pageNumber = 0;
        }
    }
    public void leftPage()
    {
        pageNumber--;
        pageFlip.Play();
        if (pageNumber < 0)
        {
            pageNumber = entries.Count-1;
        }
    }
}
