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
}
