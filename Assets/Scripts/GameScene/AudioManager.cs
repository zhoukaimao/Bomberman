using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
    public AudioClip placeBombAudio;
    public AudioClip pickupBonusAudio;
    public AudioClip bombBlastAudio;
    public AudioClip hurtAudio;
    public AudioClip pickupTrapAudio;
    private AudioSource _audioSource;
    private bool SFXon;
    private float soundVolume;
	// Use this for initialization
	void Start () {
        _audioSource = GetComponent<AudioSource>();
        SFXon = PlayerPrefs.GetInt("SFX")==1;
        soundVolume = PlayerPrefs.GetFloat("SoundVolume");
        _audioSource.volume = soundVolume;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void PlayPlaceBombAudio()
    {
        if(SFXon)
        _audioSource.PlayOneShot(placeBombAudio);
    }
    public void PlayPickupBonusAudio()
    {
        if(SFXon)
        _audioSource.PlayOneShot(pickupBonusAudio);
    }
    public void PlayBombBlastAudio()
    {
        if(SFXon)
        _audioSource.PlayOneShot(bombBlastAudio);
    }
    public void PlayHurtAudio()
    {
        if(SFXon)
        _audioSource.PlayOneShot(hurtAudio);
    }
    public void PlayPickupTrapAudio()
    {
        if(SFXon)
        {
            _audioSource.PlayOneShot(pickupTrapAudio);
        }
    }
}
