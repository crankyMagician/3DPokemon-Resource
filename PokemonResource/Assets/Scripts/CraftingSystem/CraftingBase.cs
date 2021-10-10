using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Crafting/Create new Crafting recipe")]
public class CraftingBase : ScriptableObject
{
    [SerializeField] string recipeName;


    [SerializeField] string description;

    [SerializeField] Sprite icon;


    [SerializeField] List<ItemBase> recipeList;

    public string Name => recipeName;

    public string Description => description;

    public Sprite Icon => icon;




}
