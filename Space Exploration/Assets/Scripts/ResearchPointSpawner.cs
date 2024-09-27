
using System.Collections.Generic;
using UnityEngine;

public class ResearchPointSpawner : MonoBehaviour
{
    public PlanetInfo selectedPlanet; // Reference to the selected planet
    public PlanetInfo[] AllPlanets;
    public GameObject samplePrefab; // Prefab for the samples
    public Transform[] spawnPositions; // Array of empty transforms for spawn positions
    public Terrain terrain;
    public int samplesToSpawn;

    private void Awake()
    {
        GetCurrentPlanet();
        ChangeTerrainDiffuseTexture();
        SpawnSamples();
    }

    void GetCurrentPlanet()
    {
        foreach(PlanetInfo planet in AllPlanets)
        {
            if (planet.planetName == PlayerPrefs.GetString("CurrentPlanet"))
            {
                selectedPlanet = planet;
                break;
            }
        }
    }

    private void SpawnSamples()
    {
        // Shuffle spawn positions
        List<Transform> positions = new List<Transform>(spawnPositions);
        for (int i = 0; i < positions.Count; i++)
        {
            Transform temp = positions[i];
            int randomIndex = Random.Range(i, positions.Count);
            positions[i] = positions[randomIndex];
            positions[randomIndex] = temp;
        }

        // Spawn samples at random positions
        samplesToSpawn = Mathf.Min(selectedPlanet.information.Length, positions.Count);
        for (int i = 0; i < samplesToSpawn; i++)
        {
            GameObject sample = Instantiate(samplePrefab, positions[i].position, Quaternion.identity);
            ResearchPoint sampleScript = sample.GetComponent<ResearchPoint>();
            //sampleScript.info = selectedPlanet.information[i]; // Assign planet info to the sample
            sampleScript.SetResearch(selectedPlanet.information[i]);
        }
    }

    void ChangeTerrainDiffuseTexture()
    {
        if (terrain == null )
        {
            Debug.LogError("Terrain or new texture is not assigned.");
            return;
        }

        // Get current terrain layers
        TerrainLayer[] terrainLayers = terrain.terrainData.terrainLayers;

        if (terrainLayers.Length > 0)
        {
            // Modify the first layer's diffuse texture (you can modify other layers as well)
            terrainLayers[0].diffuseTexture =selectedPlanet.groundTexture;

            // Re-assign the modified terrain layers back to the terrain
            terrain.terrainData.terrainLayers = terrainLayers;

            Debug.Log("Terrain diffuse texture changed!");
        }
        else
        {
            Debug.LogError("No terrain layers found on the terrain.");
        }
    }
}
