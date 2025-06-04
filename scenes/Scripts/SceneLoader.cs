using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad;
    public AudioClip clickSound;
    public float delayBeforeLoad = 1f;

    private AudioSource audioSource;
    private bool isClicked = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void LoadScene()
    {
        if (isClicked) return;  // prevent multiple clicks
        isClicked = true;

        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);

        StartCoroutine(DelayedLoad());
    }

    private IEnumerator DelayedLoad()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}
