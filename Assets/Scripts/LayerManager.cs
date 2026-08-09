using UnityEngine;
using UnityEngine.Playables;

public class LayerManager : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject startLayer;
    public GameObject hiddenPanel;
    public GameObject manifestPanel;
    public PlayableDirector hiddenTimeline;
    public PlayableDirector manifestTimeline;

    
    public void OnClickStartButton()
    {
        Debug.Log("Start Game");
        startLayer.SetActive(false);
        hiddenTimeline.Play();
        hiddenTimeline.stopped += OnHiddenTimelineEnd;
    }

    void OnHiddenTimelineEnd(PlayableDirector pd)
    {
        hiddenPanel.SetActive(false);
        GameManager.Instance.OnGameStart();
        manifestTimeline.Play();
        manifestTimeline.stopped += OnManifestTimelineEnd;
    }

    void OnManifestTimelineEnd(PlayableDirector pd)
    {
        manifestPanel.SetActive(false);
    }
    
    public void OnClickExitButton()
    {
        Debug.Log("Quit Game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
