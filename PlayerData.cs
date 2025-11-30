using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public float money;
    public float cargoValue;
    public int dayNumber;
    public int dayRecordNumber;
    public int mainThrustUpgrade;
    public int sideThrustUpgrade;
    public int laserDmgUpgrade;
    public bool canPalUpgrade;
    public bool canNavUpgrade;
    public bool canMissileUpgrade;
    public bool canCamoUpgrade;
    public bool canDashUpgrade;
    public bool canDampUpgrade;
    public bool canRadar3Upgrade;
    public bool equipPal;
    public bool equipMissile;
    public bool equipCamo;
    public bool equipDamp;
    public bool equipDash;
    public bool equipRadar3Upgrade;
    public int playerHealth;
    public int maxPlayerHealth;
    public int missileTurn;
    public int missileThrust;
    public int missileTime;
    public int missileDamage;
    public int missileRearmTime;
    public int fusionCells;
    public bool mouseAimMode;

    public List<float> savedWaypointsX;
    public List<float> savedWaypointsY;
    public List<float> savedWaypointsZ;

    public PlayerData (ConsoleController console)
    {
        savedWaypointsX = new List<float>();
        savedWaypointsY = new List<float>();
        savedWaypointsZ = new List<float>();
        savedWaypointsX.Clear();
        savedWaypointsY.Clear();
        savedWaypointsZ.Clear();
        money = console.Balance;
        cargoValue = console.cargoValue;
        fusionCells = console.fusionCells;
        dayNumber = console.day;
        mainThrustUpgrade = console.mainThrustLvl;
        sideThrustUpgrade = console.sideThrustLvl;
        laserDmgUpgrade = console.laserDmgLvl;
        canPalUpgrade = console.palUpgrade;
        canNavUpgrade = console.navUpgrade;
        playerHealth = console.playerHealth;
        maxPlayerHealth = console.maxPlayerHealth;
        dayRecordNumber = console.highestDay;
        mouseAimMode = console.usesMouseMode;
        canMissileUpgrade = console.missileUpgrade;
        missileDamage = console.missileDamageLvl;
        missileThrust = console.missileThrustLvl;
        missileTime = console.missileTimeLvl;
        missileTurn = console.missileTurnLvl;
        missileRearmTime = console.missileRearmLvl;
        canCamoUpgrade = console.camoUpgrade;
        canDampUpgrade = console.dampDisabler;
        canDashUpgrade = console.dashUpgrade;
        canRadar3Upgrade = console.radar3Upgrade;

        equipCamo = console.usingCamoUpgrade;
        equipDamp = console.usingdampDisablerUpgrade;
        equipDash = console.usingDashUpgrade;
        equipMissile = console.usingMissileUpgrade;
        equipPal = console.usingPalUpgrade;
        equipRadar3Upgrade = console.usingRadar3Upgrade;


        for(int i = 0; i < console.waypointsToSave.Count; i++)
        {
            savedWaypointsX.Add(console.waypointsToSave[i].x);
            savedWaypointsY.Add(console.waypointsToSave[i].y);
            savedWaypointsZ.Add(console.waypointsToSave[i].z);
        }
    }

}
