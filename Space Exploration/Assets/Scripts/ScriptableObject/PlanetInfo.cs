using UnityEngine;

[CreateAssetMenu(fileName = "NewPlanet", menuName = "Planet Information")]
public class PlanetInfo : ScriptableObject
{
    public Texture2D groundTexture;
    public string planetName;
    public Sprite planetIcon;
    [TextArea] public string[] information; // Array to hold planet information
}