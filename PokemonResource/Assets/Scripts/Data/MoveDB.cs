using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDB : MonoBehaviour
{

    static Dictionary<string, MoveBase> moves;

    //initialing the move data base 
    public static void Init()
    {
        //load all the scriptable objects from resources then save in dictionary

        moves = new Dictionary<string, MoveBase>();

        var movesList = Resources.LoadAll<MoveBase>("");

        foreach (var move in movesList)
        {

            //if the dictionary already has this key
            if (moves.ContainsKey(move.Name))
            {
                Debug.LogError($"There are two moves with the name {move.Name}");
                continue;
            }
            moves[move.Name] = move;
        }
    }

    public static MoveBase GetMoveByName(string name)
    {
        //if the move doesnt exist 
        if (!moves.ContainsKey(name))
        {
            Debug.LogError($"No move with {name} exists in this move dex");
            return null;
        }

        return moves[name];

    }

}
