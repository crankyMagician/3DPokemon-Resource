using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MonsterParty : MonoBehaviour
{
    [SerializeField] List<Monster> pokemons;

    public List<Monster> Monsters
    {
        get
        {
            return pokemons;
        }
    }

    private void Start()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    public Monster GetHealthyPokemon()
    {
        return pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }
}
