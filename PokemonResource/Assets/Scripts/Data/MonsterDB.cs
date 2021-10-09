using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*This will eventually become the monster dex
 * 
 * 
 * 
 */
public class MonsterDB : MonoBehaviour
{
   static Dictionary<string, MonsterBase> monsters;

    //initialing the monster data base 
    public static void Init()
    {
        //load all the scriptable objects from resources then save in dictionary

        monsters = new Dictionary<string, MonsterBase>();

        var monsterArray = Resources.LoadAll<MonsterBase>("");

        foreach (var monster in monsterArray)
        {

            //if the dictionary already has this key
            if (monsters.ContainsKey(monster.Name))
            {
                Debug.LogError($"There are two monsters with the name {monster.Name}");
                continue;
            }
            monsters[monster.Name] = monster;
        }
    }

    public static MonsterBase GetMonsterByName(string name)
    {
        //if the monster doesnt exist 
        if (!monsters.ContainsKey(name))
        {
            Debug.LogError($"No monster with {name} exists in this monster dex");
            return null;
        }
      
        return monsters[name];
        
    }


}
