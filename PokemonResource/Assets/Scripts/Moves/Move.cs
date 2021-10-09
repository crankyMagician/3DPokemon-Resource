using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    //setting the move 
    public MoveBase Base { get; set; }

    //getting the pp of the move 
    public int PP { get; set; }

    //setting the base of base
    public Move(MoveBase pBase)
    {
        Base = pBase;
        PP = pBase.PP;
    }

    public Move(MoveSaveData saveData)
    {

        Base = MoveDB.GetMoveByName(saveData.moveName);
        PP = saveData.pp;


        

    }
    public MoveSaveData GetSaveData()
    {
        var saveData = new MoveSaveData()
        {
            moveName = Base.Name,

            pp = PP


        };

        return saveData;
    }
}


[System.Serializable]

public class MoveSaveData
{

    public string moveName;

    public int pp;


}