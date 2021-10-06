using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Monster/Create new move")]
public class MoveBase : ScriptableObject
{
    [Tooltip("The name of the move")]
    [SerializeField]private string monsterName;


    [Tooltip("Create a short description of the move")]
    [TextArea]
    [SerializeField] string description;
    [Tooltip("The type of the move")]
    [SerializeField] MonsterType type;

    [Tooltip("The power the move has ")]
    [SerializeField] int power;
    [Tooltip("The accuracy of the move")]
    [SerializeField] int accuracy;
    [Tooltip("Does this attack always work? ")]
    [SerializeField] bool alwaysHits;
    [Tooltip("The amount of time the move can use the attack ")]
    [SerializeField] int pp;
    [Tooltip("The priority of the attack in the battle ")]
    [SerializeField] int priority;


    [Tooltip("The type of the move")]
    [SerializeField] MoveCategory category;
    [Tooltip("Does this move have any effects? ")]
    [SerializeField] MoveEffects effects;
    [Tooltip("A list of any secondary effects")]
    [SerializeField] List<SecondaryEffects> secondaries;
    [Tooltip("Does this move affect the monster or the opponent? ")]
    [SerializeField] MoveTarget target;

    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }

    public MonsterType Type {
        get { return type; }
    }

    public int Power {
        get { return power; }
    }

    public int Accuracy {
        get { return accuracy; }
    }

    public bool AlwaysHits {
        get { return alwaysHits; }
    }

    public int PP {
        get { return pp; }
    }

    public int Priority {
        get { return priority; }
    }

    public MoveCategory Category {
        get { return category; }
    }

    public MoveEffects Effects {
        get { return effects; }
    }

    public List<SecondaryEffects> Secondaries {
        get { return secondaries; }
    }

    public MoveTarget Target {
        get { return target; }
    }
}

[System.Serializable]
public class MoveEffects
{
    [Tooltip("Any status boosts to the player")]
    [SerializeField] List<StatBoost> boosts;
    [Tooltip("The type of condition the move causes for the foe")]
    [SerializeField] ConditionID status;
    [Tooltip("The type of condition the attack causes for the player")]
    [SerializeField] ConditionID volatileStatus;

    
    public List<StatBoost> Boosts {
        get { return boosts; }
    }

    public ConditionID Status {
        get { return status; }
    }

    public ConditionID VolatileStatus {
        get { return volatileStatus; }
    }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance {
        get { return chance;  }
    }

    public MoveTarget Target {
        get { return target; }
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Physical, Special, Status
}

public enum MoveTarget
{
    Foe, Self
}
