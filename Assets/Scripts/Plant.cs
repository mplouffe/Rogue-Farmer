using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public float GerminationDuration;

    public PlantStage CurrentStage;
    public float CurrentStageDuration;

    public float SeedingDuration;
    public Sprite SeedingSprite;

    public float BuddingDuration;
    public Sprite BuddingSprite;

    public float FloweringDuration;
    public Sprite FloweringSprite;

    public float RipeningDuration;
    public Sprite RipeningSprite;

    public float DecayingDuration;
    public Sprite DecayingSprite;

    public float FruitYield;

    public SpriteRenderer SpriteRenderer;

    public int GetFruitYield()
    {
        var fruitYieldMultiplier = 1f;

        if (CurrentStage == PlantStage.Decaying)
        {
            fruitYieldMultiplier -= 0.5f;
        }

        return Mathf.FloorToInt(FruitYield * fruitYieldMultiplier);
    }
}

public enum PlantStage
{
    Germination,
    Seeding,
    Budding,
    Flowering,
    Ripening,
    Decaying
}
