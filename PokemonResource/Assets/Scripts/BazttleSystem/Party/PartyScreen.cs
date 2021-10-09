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
    public List<Monster> Monsters { get; set; }

    MonsterParty party;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);

        party = MonsterParty.GetPlayerParty();
        party.OnUpdated += SetPartyData;

        SetSelectionSettings(SelectionUIType.Grid);

        SetPartyData();
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
    */
}
