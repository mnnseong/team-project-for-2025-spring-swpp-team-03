using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}
