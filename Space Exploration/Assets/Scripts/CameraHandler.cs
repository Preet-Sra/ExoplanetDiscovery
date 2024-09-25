using UnityEngine.UI;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public GameObject[] AllCameras;
    int Index;
    public Button cameraswitchButton;

    // Start is called before the first frame update
    void Start()
    {
        Index = 0;
        cameraswitchButton.onClick.AddListener(SwitchCamera);
    }

    public void SwitchCamera()
    {
        Index++;
        if (Index >= AllCameras.Length)
        {
            Index = 0;
        }

       for(int i = 0; i < AllCameras.Length; i++)
        {
            if (i == Index)
                AllCameras[i].SetActive(true);
            else
                AllCameras[i].SetActive(false);
        }
    }
}
