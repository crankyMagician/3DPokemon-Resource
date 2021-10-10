using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class PartyScreen : SelectionUI<PartyMemberUI>
{
    [SerializeField] TMP_Text messageText;

    PartyMemberUI[] memberSlots;

    //List<Monster> Monsters { get; set; }

    public List<Monster> monsters;


  //  MonsterParty party;

    int selection = 0;

   // public Monster SelectedMember => monsters[selection];
    public Monster SelectedMember => monsters[selection];

    public BattleState? CalledFrom { get; set; }

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
    }
    
    public void SetPartyData(List<Monster> monsters)
    {
        this.monsters = monsters;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < monsters.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(monsters[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        UpdateMemberSelection(selection);

        messageText.text = "Choose a Pokemon";
    }
    
    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selection;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selection;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            selection += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selection -= 2;

        selection = Mathf.Clamp(selection, 0, monsters.Count - 1);

        if (selection != prevSelection)
            UpdateMemberSelection(selection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }

  
}

/*
public void Init()
{
    memberSlots = GetComponentsInChildren<PartyMemberUI>(true);

    party = MonsterParty.GetPlayerParty();
    party.OnUpdated += SetPartyData;

    SetSelectionSettings(SelectionUIType.Grid);

    SetPartyData();
}

//  int selection = 0;
public void HandleUpdate(Action onSelected, Action onBack)
{
    var prevSelection = selection;

    if (Input.GetKeyDown(KeyCode.RightArrow))
        ++selection;
    else if (Input.GetKeyDown(KeyCode.LeftArrow))
        --selection;
    else if (Input.GetKeyDown(KeyCode.DownArrow))
        selection += 2;
    else if (Input.GetKeyDown(KeyCode.UpArrow))
        selection -= 2;

    selection = Mathf.Clamp(selection, 0, monsters.Count - 1);

    if (selection != prevSelection)
        UpdateMemberSelection(selection);

    if (Input.GetKeyDown(KeyCode.Z))
    {
        onSelected?.Invoke();
    }
    else if (Input.GetKeyDown(KeyCode.X))
    {
        onBack?.Invoke();
    }
}


public void SetPartyData()
{
    monsters = party.Monsters;

    for (int i = 0; i < memberSlots.Length; i++)
    {
        if (i < monsters.Count)
        {
            memberSlots[i].gameObject.SetActive(true);
            memberSlots[i].Init(monsters[i]);
        }
        else
            memberSlots[i].gameObject.SetActive(false);
    }


    SetItems(memberSlots.Take(monsters.Count).ToList());


    messageText.text = "Choose a Monster";
}
public void UpdateSlotData(int slotIndex)
{
    memberSlots[slotIndex].Init(monsters[slotIndex]);
}

public void SetMessageText(string message)
{
    messageText.text = message;
}
public void UpdateMemberSelection(int selectedMember)
{
    for (int i = 0; i < monsters.Count; i++)
    {
        if (i == selectedMember)
            memberSlots[i].SetSelected(true);
        else
            memberSlots[i].SetSelected(false);
    }
}

}



/*


public void UpdateSlotData(int slotIndex)
{
    memberSlots[slotIndex].Init(monsters[slotIndex]);
}

public void SetMessageText(string message)
{
    messageText.text = message;
}

}
*/
/*

/// <summary>
/// Party screen can be called from different states like ActionSelection, RunningTurn, AboutToUse
/// </summary>
public BattleState? CalledFrom { get; set; }

public void Init()
{
    memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
}

public void SetPartyData(List<Monster> monsters)
{
    this.monsters = monsters;

    for (int i = 0; i < memberSlots.Length; i++)
    {
        if (i < monsters.Count)
        {
            memberSlots[i].gameObject.SetActive(true);
            memberSlots[i].SetData(monsters[i]);
        }
        else
            memberSlots[i].gameObject.SetActive(false);
    }

    UpdateMemberSelection(selection);

    messageText.text = "Choose a Monster";
}

public void HandleUpdate(Action onSelected, Action onBack)
{
    var prevSelection = selection;

    if (Input.GetKeyDown(KeyCode.RightArrow))
        ++selection;
    else if (Input.GetKeyDown(KeyCode.LeftArrow))
        --selection;
    else if (Input.GetKeyDown(KeyCode.DownArrow))
        selection += 2;
    else if (Input.GetKeyDown(KeyCode.UpArrow))
        selection -= 2;

    selection = Mathf.Clamp(selection, 0, monsters.Count - 1);

    if (selection != prevSelection)
        UpdateMemberSelection(selection);

    if (Input.GetKeyDown(KeyCode.Z))
    {
        onSelected?.Invoke();
    }
    else if (Input.GetKeyDown(KeyCode.X))
    {
        onBack?.Invoke();
    }
}

public void UpdateMemberSelection(int selectedMember)
{
    for (int i = 0; i < monsters.Count; i++)
    {
        if (i == selectedMember)
            memberSlots[i].SetSelected(true);
        else
            memberSlots[i].SetSelected(false);
    }
}

public void SetMessageText(string message)
{
    messageText.text = message;
}
}
*/
/*
public Monster SelectedMember => Monsters[selection];

public BattleState? CalledFrom { get; set; }
public void Init()
{
    memberSlots = GetComponentsInChildren<PartyMemberUI>(true);

    party = MonsterParty.GetPlayerParty();
    party.OnUpdated += SetPartyData;

    SetSelectionSettings(SelectionUIType.Grid);

    SetPartyData();
}

int selection = 0;
public void HandleUpdate(Action onSelected, Action onBack)
{
    var prevSelection = selection;

    if (Input.GetKeyDown(KeyCode.RightArrow))
        ++selection;
    else if (Input.GetKeyDown(KeyCode.LeftArrow))
        --selection;
    else if (Input.GetKeyDown(KeyCode.DownArrow))
        selection += 2;
    else if (Input.GetKeyDown(KeyCode.UpArrow))
        selection -= 2;

    selection = Mathf.Clamp(selection, 0, Monsters.Count - 1);

    if (selection != prevSelection)
        UpdateMemberSelection(selection);

    if (Input.GetKeyDown(KeyCode.Z))
    {
        onSelected?.Invoke();
    }
    else if (Input.GetKeyDown(KeyCode.X))
    {
        onBack?.Invoke();
    }
}


public void SetPartyData()
{
    Monsters = party.Monsters;

    for (int i = 0; i < memberSlots.Length; i++)
    {
        if (i < Monsters.Count)
        {
            memberSlots[i].gameObject.SetActive(true);
            memberSlots[i].Init(Monsters[i]);
        }
        else
            memberSlots[i].gameObject.SetActive(false);
    }


    SetItems(memberSlots.Take(Monsters.Count).ToList());


    messageText.text = "Choose a Monster";
}
public void UpdateSlotData(int slotIndex)
{
    memberSlots[slotIndex].Init(Monsters[slotIndex]);
}

public void SetMessageText(string message)
{
    messageText.text = message;
}
public void UpdateMemberSelection(int selectedMember)
{
    for (int i = 0; i < Monsters.Count; i++)
    {
        if (i == selectedMember)
            memberSlots[i].SetSelected(true);
        else
            memberSlots[i].SetSelected(false);
    }
}




/*


public void UpdateSlotData(int slotIndex)
{
    memberSlots[slotIndex].Init(monsters[slotIndex]);
}

public void SetMessageText(string message)
{
    messageText.text = message;
}

}
*/