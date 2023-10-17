using UnityEngine;
using UnityEngine.SceneManagement;

public class PictureToARSceneController : MonoBehaviour
{
    public PhotoLogicManager PhotoLogicManager;
    public GameObject textureList;

    public string aRTrack;
    private bool hasTakenPhotos;

    void Update()
    {
        if (PhotoLogicManager != null && PhotoLogicManager.GetPhotoFinished() && !hasTakenPhotos)
        {
            hasTakenPhotos = true;

            DontDestroyOnLoad(textureList);
            SceneManager.LoadScene(aRTrack);
        }
    }
}
