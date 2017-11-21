public class CharacterStats {

    /*****DESCRIPTION*****/

    /* 
     * This script holds all the information in regards
     * to every character created. The character is first
     * created using the its Constructor class. From there,
     * throughout the game, getting and setting values will
     * also be available.
     */

    //setting up initial values
    int healthPoints;      //character's health
    int attackPoints;     //character's attack power
    int defensePoints;    //character's defense
    int speedPoints;      //character's ability speed
    int abilityPoints;     //character's ability points
    int capPoints;        //character's maximum attacks
    int meterPoints;    //character's ability meter
    int positionID;     //character's position

    string charID;        //character's ID
    string charSide;      //which side the character is on

    //default constructor for all characters

    public CharacterStats(int hp, int atk, int def, int spd, int ap, 
        int cap, int meter, int pos, string cid, string side)
    {
        //setting up initial stat values
        healthPoints = hp;
        attackPoints = atk;
        defensePoints = def;
        speedPoints = spd;
        abilityPoints = ap;
        capPoints = cap;
        meterPoints = meter;

        //setting up position, id, and side of character
        positionID = pos;
        charID = cid;
        charSide = side;
    }

    //get-set properties for each stat
    public int HP{
        get { return healthPoints; }
        set { healthPoints = value; }
    }

    public int ATK
    {
        get { return attackPoints; }
        set { attackPoints = value; }
    }

    public int DEF
    {
        get { return defensePoints; }
        set { defensePoints = value; }
    }

    public int SPD
    {
        get { return speedPoints; }
        set { speedPoints = value; }
    }

    public int AP
    {
        get { return abilityPoints; }
        set { abilityPoints = value; }
    }

    public int CAP
    {
        get { return capPoints; }
        set { capPoints = value; }
    }

    public int METER
    {
        get { return meterPoints; }
        set { meterPoints = value; }
    }

    public int POSITION
    {
        get { return positionID; }
        set { positionID = value; }
    }

    public string IDENTITY
    {
        get { return charID; }
    }

    public string TAG
    {
        get { return charSide; }
        set { charSide = value; }
    }
}
