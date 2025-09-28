using UnityEngine;

public class ButtonSound : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip clickSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlaySound()
    {
        audioSource.PlayOneShot(clickSound);
    }
}
