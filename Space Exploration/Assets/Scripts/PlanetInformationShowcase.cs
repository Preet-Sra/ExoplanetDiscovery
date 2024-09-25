
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlanetInformationShowcase : MonoBehaviour
{
    [SerializeField] Image PlanetIcon;
    [SerializeField] TMP_Text PlanetName, PlanetInfo;
    public void SetInfo(string planetName,Sprite planetIcon,string planteInfo)
    {
        PlanetName.text = planetName;
        PlanetIcon.sprite = planetIcon;
        PlanetInfo.text = planteInfo;
    }
}
