
using UnityEngine;


public enum AudioType { Error,DataFound,Rotate,UiDroppedSucces,PointerDown ,Close,Confirm,Explosion,CameraChange,Won,ButtonPress,Asteroid,AsteroidDestroyed}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    AudioSource SfxAudio;
    [SerializeField] AudioClip errorSound, dataFoundSound,rotateSound,uiDroopedSuccessSound,PointerDownSound,CloseSound,ConfirmSound,explosionSound,cameraChangeSound,wonSound,buttonPressSound,asteroidDestroyedSound;
    [SerializeField] AudioClip[] AsteroidsSounds;

    private void Start()
    {
        SfxAudio = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    

    public void PlayAudio(AudioType type)
    {
        SfxAudio.PlayOneShot(GetSound(type));
    }

    AudioClip GetSound(AudioType type)
    {
        switch (type)
        {
            case AudioType.Error:
                return errorSound;
            case AudioType.DataFound:
                return dataFoundSound;
            case AudioType.Rotate:
                return rotateSound;
            case AudioType.UiDroppedSucces:
                return uiDroopedSuccessSound;

            case AudioType.PointerDown:
                return PointerDownSound;
            case AudioType.Close:
                return CloseSound;
            case AudioType.Confirm:
                return ConfirmSound;
            case AudioType.Explosion:
                return explosionSound;
            case AudioType.CameraChange:
                return cameraChangeSound;
            case AudioType.Won:
                return wonSound;
            case AudioType.ButtonPress:
                return buttonPressSound;

            case AudioType.Asteroid:
                return AsteroidsSounds[Random.Range(0, AsteroidsSounds.Length)];

            case AudioType.AsteroidDestroyed:
                return asteroidDestroyedSound;
            default:
                return null;
        }
    }
}
