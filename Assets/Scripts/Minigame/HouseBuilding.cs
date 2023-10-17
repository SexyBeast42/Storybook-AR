using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public class MiniGameRules
{
    public string materialName;
    public int amountOfTriggers;
    public GameObject housePrefab;
}

public class HouseBuilding : MonoBehaviour
{
    [SerializeField] private List<MiniGameRules> miniGameRulesList = new List<MiniGameRules>();
    private MiniGameRules currentMiniGameRules;
    
    public GameObject triggerPrefab;
    private List<GameObject> triggers = new List<GameObject>();

    public GameObject playArea;

    private string currentMaterial;

    private bool hasFinished;

    private enum State
    {
        Check,
        Spawn,
        Wait,
        Finished
    }

    private State state;

    void Start()
    {
        state = State.Finished;
    }

    void Update()
    {
        switch (state)
        {
            case State.Check:
                SetMiniGameRules();
                
                break;
            
            case State.Spawn:
                SpawnTriggersWithMiniGameRules();
                
                break;
            
            case State.Wait:
                CheckPlayerProgress();
                
                break;
            
            case State.Finished:
                
                break;
        }
    }

    public void StartMiniGameWithMaterialRule(string material)
    {
        currentMaterial = material;

        state = State.Check;
    }

    private void SetMiniGameRules()
    {
        foreach (var miniGameRules in miniGameRulesList)
        {
            string currentMaterialSet = miniGameRules.materialName;

            if (currentMaterialSet != currentMaterial) continue;

            currentMiniGameRules = miniGameRules;

            state = State.Spawn;
        }
    }

    private void SpawnTriggersWithMiniGameRules()
    {
        for (int i = 0; i < currentMiniGameRules.amountOfTriggers; i++)
        {
            Vector3 randomPosition = GetRandomPositionInArea(playArea);

            triggers[i] = Instantiate(triggerPrefab, randomPosition, Quaternion.identity);
        }

        if (currentMiniGameRules.amountOfTriggers == triggers.Count)
        {
            state = State.Wait;
        }
    }

    private Vector3 GetRandomPositionInArea(GameObject area)
    {
        if (area == null) return Vector3.zero;
        
        Vector3 areaSize = area.transform.localScale;

        float randomX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float randomZ = Random.Range(-areaSize.z / 2f, areaSize.z / 2f);
        
        return new Vector3(randomX, 0f, randomZ);
    }

    private void CheckPlayerProgress()
    {
        foreach (var trigger in triggers)
        {
            Trigger currentTrigger = trigger.GetComponent<Trigger>();

            if (currentTrigger.IsTriggered(currentMaterial))
            {
                triggers.Remove(trigger);
            }
        }

        if (triggers.Count == 0)
        {
            state = State.Finished;
        }
    }
}
