using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
    public AudioClip placeBombAudio;
    public AudioClip pickupBonusAudio;
    public AudioClip bombBlastAudio;
    public AudioClip hurtAudio;
    private AudioSource _audioSource;
	// Use this for initialization
	void Start () {
        _audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void PlayPlaceBombAudio()
    {
        _audioSource.PlayOneShot(placeBombAudio);
    }
    public void PlayPickupBonusAudio()
    {
        _audioSource.PlayOneShot(pickupBonusAudio);
    }
    public void PlayBombBlastAudio()
    {
        _audioSource.PlayOneShot(bombBlastAudio);
    }
    public void PlayHurtAudio()
    {
        _audioSource.PlayOneShot(hurtAudio);
    }
}
