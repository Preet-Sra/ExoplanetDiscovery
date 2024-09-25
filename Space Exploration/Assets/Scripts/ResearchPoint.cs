
using UnityEngine;

public class ResearchPoint : MonoBehaviour
{
    string informationPoint;
   public void SetResearch(string info)
    {
        informationPoint = info;
    }


    public string GetInformation()
    {
        return informationPoint;
    }
}
