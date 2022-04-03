using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum CardScreenState
{
    FADE_IN,
    WAITING,
    FADE_OUT
};
public class TitleScreenController : MonoBehaviour
{
    public CardScreenState State;

    public Image FadeCover;
    public float fadeInOutTime;
    public GameObject MusicPlayerPrefab;

    public string NextScene;

    private float coverFadeEndTime;
    private float coverFadeStartTime;
    private bool doneFirstUpdate = false;

    // Start is called before the first frame update
    void Start()
    {
        // instantiate the music player if needed
        GameObject[] musicPlayers = GameObject.FindGameObjectsWithTag("MusicPlayer");
        if (musicPlayers.Length == 0)
        {
            GameObject musicPlayer = Instantiate(MusicPlayerPrefab, Vector3.zero, Quaternion.identity);
            DontDestroyOnLoad(musicPlayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
            
        switch (State)
        {
            case CardScreenState.FADE_IN:
                if (!doneFirstUpdate)
                {
                    doneFirstUpdate = true;
                    coverFadeStartTime = Time.time;
                    coverFadeEndTime = Time.time + fadeInOutTime;
                }
                else
                {
                    if (Time.time > coverFadeEndTime)
                    {
                        State = CardScreenState.WAITING;
                    }
                    else
                    {
                        FadeCover.color = Color.Lerp(Color.black, Color.clear, (Time.time - coverFadeStartTime) / fadeInOutTime);
                    }
                }
                break;
            case CardScreenState.FADE_OUT:
                if (Time.time > coverFadeEndTime)
                {
                    // to be replaced with GameOver when that's ready
                    SceneManager.LoadSceneAsync(NextScene);
                }
                else
                {
                    FadeCover.color = Color.Lerp(Color.clear, Color.black, (Time.time - coverFadeStartTime) / fadeInOutTime);
                }
                break;
            case CardScreenState.WAITING:
                if (Input.anyKeyDown)
                {
                    State = CardScreenState.FADE_OUT;
                    coverFadeStartTime = Time.time;
                    coverFadeEndTime = Time.time + fadeInOutTime;
                }
                break;
        }
    }
}
