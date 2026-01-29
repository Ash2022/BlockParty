using System;
using UnityEngine;


public class SoundsController : MonoBehaviour
{


    [SerializeField] AudioClip _menuLoop = null;
    [SerializeField] AudioClip _gameLoop = null;

    [SerializeField] AudioClip _personPop;
    [SerializeField] AudioClip _lineComplete;
    [SerializeField] AudioClip _piecePlaced;
    [SerializeField] AudioClip _levelComplete;
    [SerializeField] AudioClip _levelFailed;
    [SerializeField] AudioClip _confettiPop;
    [SerializeField] AudioClip _personStartWalking;
    [SerializeField] AudioClip _queueOptemizedWalking;
    [SerializeField] AudioClip _levelBuilding;

    [SerializeField] AudioClip _resultProgressBar;
    [SerializeField] AudioClip _lobbyUnlockFeature;


    [SerializeField] AudioSource _SFX_Source1 = null;
    [SerializeField] AudioSource _SFX_Source2 = null;
    [SerializeField] AudioSource _SFX_Source3 = null;
    [SerializeField] AudioSource _SFX_Source4 = null;
    [SerializeField] AudioSource _SFX_Source5 = null;

    [SerializeField] AudioSource _SFX_Source6 = null;
    [SerializeField] AudioSource _SFX_Source7 = null;
    [SerializeField] AudioSource _SFX_Source8 = null;
    [SerializeField] AudioSource _SFX_Source9 = null;
    [SerializeField] AudioSource _SFX_Source10 = null;
    
    [SerializeField] AudioSource _ambianceLoop = null;
    [SerializeField] AudioSource _menuSourceLoop = null;
    private AudioSource _chawSource=null;
    static SoundsController _instance;

    public static SoundsController Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    public void DisableEnableMixer(bool disable)
    {
        if (disable)
            AudioListener.volume = 0;
        else
            AudioListener.volume = 1f;

    }

    public void MuteAll(bool mute)
    {
        _SFX_Source1.mute = mute;
        _SFX_Source2.mute = mute;
        _SFX_Source3.mute = mute;
        _SFX_Source4.mute = mute;
        _SFX_Source5.mute = mute;
        _SFX_Source6.mute = mute;
        _SFX_Source7.mute = mute;
        _SFX_Source8.mute = mute;
        _SFX_Source9.mute = mute;
        _SFX_Source10.mute = mute;
        _ambianceLoop.mute = mute;
        _menuSourceLoop.mute = mute;
        
    }

   

   
    public void PlayLevelCompelte(bool success)
    {
        PlayClip(success?_levelComplete:_levelFailed);
    }

    public void PlayGridSquarePop()
    {
        PlayClip(_personPop,0.75f,2.5f);
    }
   
    public void PlayLineComplete()
    {
        PlayClip(_lineComplete);
    }

    public void PlayPiecePlaced()
    {
        PlayClip(_piecePlaced,0.75f,2f);
    }

    public void PlayConfettiPop()
    {
        PlayClip(_confettiPop, 0.4f, 3.75f);
    }

    public void PlayStartWalking()
    {
        PlayClip(_personStartWalking);
    }

    public void PlayBuildLevel()
    {
        PlayClip(_levelBuilding,0.65f,2f);
    }

    public void PlayMoveInQueueOptemize()
    {
        PlayClip(_queueOptemizedWalking);
    }

    public void PlayMenuAmbiance(bool start)
    {
        if (start)
        {
            _menuSourceLoop.clip = _menuLoop;
            _menuSourceLoop.loop = true;
            _menuSourceLoop.Play();
        }
        else
            _menuSourceLoop.Stop();
    }

    public void PlayGameAmbiance(bool start)
    {
        if (start)
        {
            _ambianceLoop.clip = _gameLoop;
            _ambianceLoop.loop = false;
            _ambianceLoop.Play();
        }
        else
            _ambianceLoop.Stop();
    }

    public AudioSource PlayClip(AudioClip clip, float volume = 1, float pitch = 1)
    {
        AudioSource audio_source = GetFreeAudioSource();

        if (audio_source != null && audio_source.enabled == true)
        {
            audio_source.clip = clip;
            audio_source.Play();
            audio_source.pitch = pitch;
            audio_source.volume = volume;
        }

        return audio_source;
    }



    private AudioSource GetFreeAudioSource()
    {
        if (!_SFX_Source1.isPlaying)
            return _SFX_Source1;


        if (!_SFX_Source2.isPlaying)
            return _SFX_Source2;

        if (!_SFX_Source3.isPlaying)
            return _SFX_Source3;

        if (!_SFX_Source4.isPlaying)
            return _SFX_Source4;

        if (!_SFX_Source5.isPlaying)
            return _SFX_Source5;

        if (!_SFX_Source6.isPlaying)
            return _SFX_Source6;
        if (!_SFX_Source7.isPlaying)
            return _SFX_Source7;
        if (!_SFX_Source8.isPlaying)
            return _SFX_Source8;
        if (!_SFX_Source9.isPlaying)
            return _SFX_Source9;
        if (!_SFX_Source10.isPlaying)
            return _SFX_Source10;
        
        return null;

    }

    public void PlayLobbyUnlockFeature()
    {
        PlayClip(_lobbyUnlockFeature);
    }

    public void PlayProgressBar()
    {
        PlayClip(_resultProgressBar);
    }
}
