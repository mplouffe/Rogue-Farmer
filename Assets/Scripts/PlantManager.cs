using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    private static PlantManager m_instance;

    [SerializeField]
    private Plant m_plant;

    private Dictionary<Vector3Int, Plant> m_plants = new Dictionary<Vector3Int, Plant>(40);

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogError("Found multiple plant managers. Destroying now.");
            Destroy(m_instance);
        }
        m_instance = this;
    }

    public static void PlantSeed(Vector3Int position)
    {
        if (m_instance == null)
        {
            Debug.LogError("Trying to plant seed to a null instance.");
            return;
        }
        m_instance.m_plants.Add(position, Instantiate(m_instance.m_plant, position, Quaternion.identity));
    }

    public static bool TryHarvestCrop(Vector3Int position, out int yield)
    {
        yield = 0;
        if (m_instance == null)
        {
            Debug.LogError("Trying to harvest a crop from a null Instance.");
            return false;
        }

        if (m_instance.m_plants.TryGetValue(position, out Plant harvestedPlant))
        {
            if (harvestedPlant.CurrentStage == PlantStage.Ripening || harvestedPlant.CurrentStage == PlantStage.Decaying)
            {
                yield = harvestedPlant.GetFruitYield();
                m_instance.RemovePlant(position);
                return true;
            }
        }
        return false;
    }

    public static bool TryToFindPlant(Vector3Int position)
    {
        if (m_instance == null)
        {
            Debug.LogError("Trying to find plant on a null Instance.");
            return false;
        }

        if (m_instance.m_plants.TryGetValue(position, out Plant plant))
        {
            return plant.CurrentStage != PlantStage.Germination && plant.CurrentStage != PlantStage.Decaying;
        }
        return false;
    }

    public static void AttackPlant(Vector3Int position)
    {
        if (m_instance == null)
        {
            Debug.LogError("Trying to attack a plant on a null Instance.");
            return;
        }

        if (m_instance.m_plants.TryGetValue(position, out Plant _))
        {
            m_instance.RemovePlant(position);
            TileManager.PlantDiedOnTile(position);
        }
    }

    private void FixedUpdate()
    {
        GrowPlants();
    }

    private void RemovePlant(Vector3Int positionOfPlantToRemove)
    {
        var plant = m_plants[positionOfPlantToRemove];
        m_plants.Remove(positionOfPlantToRemove);
        Destroy(plant.gameObject);
    }

    private List<Vector3Int> m_plantsToRemove = new List<Vector3Int>(20);
    private void GrowPlants()
    {
        m_plantsToRemove.Clear();

        foreach(var plantLocation in m_plants.Keys)
        {
            var plant = m_plants[plantLocation];
            plant.CurrentStageDuration += Time.fixedDeltaTime;
            switch(plant.CurrentStage)
            {
                case PlantStage.Germination:
                    if (plant.CurrentStageDuration > plant.GerminationDuration)
                    {
                        plant.CurrentStage = PlantStage.Seeding;
                        plant.SpriteRenderer.sprite = plant.SeedingSprite;
                        plant.CurrentStageDuration = 0;
                    }
                    break;
                case PlantStage.Seeding:
                    if (plant.CurrentStageDuration > plant.SeedingDuration)
                    {
                        plant.CurrentStage = PlantStage.Budding;
                        plant.SpriteRenderer.sprite = plant.BuddingSprite;
                        plant.CurrentStageDuration = 0;
                    }
                    break;
                case PlantStage.Budding:
                    if (plant.CurrentStageDuration > plant.BuddingDuration)
                    {
                        plant.CurrentStage = PlantStage.Flowering;
                        plant.SpriteRenderer.sprite = plant.FloweringSprite;
                        plant.CurrentStageDuration = 0;
                    }
                    break;
                case PlantStage.Flowering:
                    if (plant.CurrentStageDuration > plant.FloweringDuration)
                    {
                        plant.CurrentStage = PlantStage.Ripening;
                        plant.SpriteRenderer.sprite = plant.RipeningSprite;
                        plant.CurrentStageDuration = 0;
                    }
                    break;
                case PlantStage.Ripening:
                    if (plant.CurrentStageDuration > plant.RipeningDuration)
                    {
                        plant.CurrentStage = PlantStage.Decaying;
                        plant.SpriteRenderer.sprite = plant.DecayingSprite;
                        plant.CurrentStageDuration = 0;
                    }
                    break;
                case PlantStage.Decaying:
                    if (plant.CurrentStageDuration > plant.DecayingDuration)
                    {
                        m_plantsToRemove.Add(plantLocation);
                    }
                    break;

            }
        }

        if (m_plantsToRemove.Count > 0)
        {
            foreach(var plantLocation in m_plantsToRemove)
            {
                RemovePlant(plantLocation);
                TileManager.PlantDiedOnTile(plantLocation);
            }
        }
    }
}
