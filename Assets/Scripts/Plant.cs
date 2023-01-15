using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public float GerminationDuration;

    [SerializeField]
    private List<PlantGrowthStage> m_growthStages;
    private Dictionary<PlantStage, (float Duration, Sprite Sprite)> m_growthStageInformation;

    public PlantStage CurrentStage;
    public float CurrentStageDuration;

    public float SeedingDuration => GetDurationByStage(PlantStage.Seeding);
    public Sprite SeedingSprite => GetSpriteByStage(PlantStage.Seeding);

    public float BuddingDuration => GetDurationByStage(PlantStage.Budding);
    public Sprite BuddingSprite => GetSpriteByStage(PlantStage.Budding);

    public float FloweringDuration => GetDurationByStage(PlantStage.Flowering);
    public Sprite FloweringSprite => GetSpriteByStage(PlantStage.Flowering);

    public float RipeningDuration => GetDurationByStage(PlantStage.Ripening);
    public Sprite RipeningSprite => GetSpriteByStage(PlantStage.Ripening);

    public float DecayingDuration => GetDurationByStage(PlantStage.Decaying);
    public Sprite DecayingSprite => GetSpriteByStage(PlantStage.Decaying);

    public float FruitYield;

    public float WaterAbsorptionRate;
    public float NutrientAbsorptionRate;

    private float m_totalWaterAborbedCurrentStage;
    private float m_totalNutrientsAbsorbedCurrentStage;

    public SpriteRenderer SpriteRenderer;

    private void Awake()
    {
        m_growthStageInformation = new Dictionary<PlantStage, (float Duration, Sprite Sprite)>(m_growthStages.Count);
        foreach(var growthStage in m_growthStages)
        {
            m_growthStageInformation.Add(growthStage.Stage, (growthStage.StageDuration, growthStage.StageSprite));
        }
    }

    public int GetFruitYield()
    {
        var fruitYieldMultiplier = 1f;

        if (CurrentStage == PlantStage.Decaying)
        {
            fruitYieldMultiplier -= 0.5f;
        }

        return Mathf.FloorToInt(FruitYield * fruitYieldMultiplier);
    }

    private float GetDurationByStage(PlantStage stage)
    {
        return m_growthStageInformation[stage].Duration;
    }

    private Sprite GetSpriteByStage(PlantStage stage)
    {
        return m_growthStageInformation[stage].Sprite;
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

public struct PlantGrowthStage
{
    public PlantStage Stage;
    public float StageDuration;
    public Sprite StageSprite;
}
