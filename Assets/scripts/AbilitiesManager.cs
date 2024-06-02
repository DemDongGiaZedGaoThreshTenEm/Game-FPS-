using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesManager : MonoBehaviour
{
    public static AbilitiesManager Instance { get; private set; }
    public Abilities Q_Ability;
    public Abilities E_Ability;
    private Dictionary<Abilities, Ability> abilityDictionary = new Dictionary<Abilities, Ability>();

    /*private void Awake()
    {
        // Singleton implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize abilities
        InitializeAbilities();
    }

    private void InitializeAbilities()
    {
        // Example: Adding abilities to the dictionary
        abilityDictionary.Add(Q_Ability); // Assign any other ability class
        abilityDictionary.Add(E_Ability); // Assign any other ability class
        // Add more abilities as needed
    }

    // Method to activate an ability
    public void ActivateAbility(Abilities ability)
    {
        if (abilityDictionary.ContainsKey(ability))
        {
            abilityDictionary[ability].Activate();
        }
        else
        {
            Debug.LogWarning($"Ability '{ability}' not found!");
        }
    }*/
}


