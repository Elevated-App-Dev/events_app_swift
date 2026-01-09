using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the Audio Manager for background music and sound effects.
    /// This is a pure C# implementation that can be tested without Unity dependencies.
    /// The actual audio playback would be handled by a Unity MonoBehaviour wrapper.
    /// Requirements: R26.1-R26.6
    /// </summary>
    public class AudioManagerImpl : IAudioManager
    {
        private float _musicVolume;
        private float _sfxVolume;
        private bool _isMuted;
        private bool _isPaused;
        private MusicTrack? _currentTrack;
        private bool _isMusicPlaying;

        // Volume before mute was applied (for restoration)
        private float _preMuteMusicVolume;
        private float _preMuteSFXVolume;

        // Events for UI/Unity integration
        public event Action<MusicTrack?> OnMusicChanged;
        public event Action<SoundEffect> OnSFXPlayed;
        public event Action<float, float> OnVolumeChanged;
        public event Action<bool> OnMuteChanged;
        public event Action<bool> OnPauseChanged;

        // Properties
        public float MusicVolume => _musicVolume;
        public float SFXVolume => _sfxVolume;
        public bool IsMuted => _isMuted;
        public bool IsPaused => _isPaused;
        public MusicTrack? CurrentTrack => _currentTrack;
        public bool IsMusicPlaying => _isMusicPlaying && !_isPaused;

        /// <summary>
        /// Creates a new AudioManagerImpl with default settings.
        /// Default volumes are 1.0 (full volume), not muted, not paused.
        /// </summary>
        public AudioManagerImpl()
        {
            _musicVolume = 1.0f;
            _sfxVolume = 1.0f;
            _isMuted = false;
            _isPaused = false;
            _currentTrack = null;
            _isMusicPlaying = false;
            _preMuteMusicVolume = 1.0f;
            _preMuteSFXVolume = 1.0f;
        }

        /// <summary>
        /// Creates a new AudioManagerImpl with specified initial settings.
        /// </summary>
        public AudioManagerImpl(float musicVolume, float sfxVolume, bool muted = false)
        {
            _musicVolume = ClampVolume(musicVolume);
            _sfxVolume = ClampVolume(sfxVolume);
            _isMuted = muted;
            _isPaused = false;
            _currentTrack = null;
            _isMusicPlaying = false;
            _preMuteMusicVolume = _musicVolume;
            _preMuteSFXVolume = _sfxVolume;
        }


        /// <summary>
        /// Play background music for a screen/context.
        /// If the same track is already playing, this is a no-op.
        /// Requirements: R26.1
        /// </summary>
        public void PlayMusic(MusicTrack track)
        {
            // If already playing this track, do nothing
            if (_currentTrack == track && _isMusicPlaying)
                return;

            _currentTrack = track;
            _isMusicPlaying = true;

            OnMusicChanged?.Invoke(track);
        }

        /// <summary>
        /// Stop current background music.
        /// </summary>
        public void StopMusic()
        {
            if (!_isMusicPlaying && _currentTrack == null)
                return;

            _isMusicPlaying = false;
            var previousTrack = _currentTrack;
            _currentTrack = null;

            if (previousTrack.HasValue)
            {
                OnMusicChanged?.Invoke(null);
            }
        }

        /// <summary>
        /// Play a sound effect.
        /// Sound effects are fire-and-forget and can overlap.
        /// Requirements: R26.2-R26.4
        /// </summary>
        public void PlaySFX(SoundEffect sfx)
        {
            // Don't play if muted or paused
            if (_isMuted || _isPaused)
                return;

            // Fire event for Unity to handle actual playback
            OnSFXPlayed?.Invoke(sfx);
        }

        /// <summary>
        /// Set music volume (0-1).
        /// Requirements: R26.5
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            float clampedVolume = ClampVolume(volume);
            
            if (Math.Abs(_musicVolume - clampedVolume) < 0.0001f)
                return;

            _musicVolume = clampedVolume;
            
            // If not muted, also update the pre-mute volume
            if (!_isMuted)
            {
                _preMuteMusicVolume = clampedVolume;
            }

            OnVolumeChanged?.Invoke(_musicVolume, _sfxVolume);
        }

        /// <summary>
        /// Set sound effects volume (0-1).
        /// Requirements: R26.5
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            float clampedVolume = ClampVolume(volume);
            
            if (Math.Abs(_sfxVolume - clampedVolume) < 0.0001f)
                return;

            _sfxVolume = clampedVolume;
            
            // If not muted, also update the pre-mute volume
            if (!_isMuted)
            {
                _preMuteSFXVolume = clampedVolume;
            }

            OnVolumeChanged?.Invoke(_musicVolume, _sfxVolume);
        }

        /// <summary>
        /// Mute/unmute all audio.
        /// When muting, volumes are set to 0 but previous values are remembered.
        /// When unmuting, previous volumes are restored.
        /// Requirements: R26.5
        /// </summary>
        public void SetMuteAll(bool muted)
        {
            if (_isMuted == muted)
                return;

            if (muted)
            {
                // Store current volumes before muting
                _preMuteMusicVolume = _musicVolume;
                _preMuteSFXVolume = _sfxVolume;
                _musicVolume = 0f;
                _sfxVolume = 0f;
            }
            else
            {
                // Restore volumes when unmuting
                _musicVolume = _preMuteMusicVolume;
                _sfxVolume = _preMuteSFXVolume;
            }

            _isMuted = muted;
            OnMuteChanged?.Invoke(muted);
            OnVolumeChanged?.Invoke(_musicVolume, _sfxVolume);
        }

        /// <summary>
        /// Pause audio when app loses focus.
        /// Music playback is suspended but track position is remembered.
        /// Requirements: R26.6
        /// </summary>
        public void PauseAudio()
        {
            if (_isPaused)
                return;

            _isPaused = true;
            OnPauseChanged?.Invoke(true);
        }

        /// <summary>
        /// Resume audio when app regains focus.
        /// Music playback continues from where it was paused.
        /// Requirements: R26.6
        /// </summary>
        public void ResumeAudio()
        {
            if (!_isPaused)
                return;

            _isPaused = false;
            OnPauseChanged?.Invoke(false);
        }

        /// <summary>
        /// Helper method to clamp volume to valid range [0, 1].
        /// </summary>
        private static float ClampVolume(float volume)
        {
            return Math.Clamp(volume, 0f, 1f);
        }

        /// <summary>
        /// Get the effective music volume considering mute state.
        /// Returns 0 if muted, otherwise returns the set volume.
        /// </summary>
        public float GetEffectiveMusicVolume()
        {
            return _isMuted ? 0f : _musicVolume;
        }

        /// <summary>
        /// Get the effective SFX volume considering mute state.
        /// Returns 0 if muted, otherwise returns the set volume.
        /// </summary>
        public float GetEffectiveSFXVolume()
        {
            return _isMuted ? 0f : _sfxVolume;
        }

        /// <summary>
        /// Play appropriate audio feedback for event success.
        /// Convenience method combining music and SFX.
        /// Requirements: R26.3
        /// </summary>
        public void PlayEventSuccess()
        {
            PlaySFX(SoundEffect.Success);
            PlayMusic(MusicTrack.Celebration);
        }

        /// <summary>
        /// Play appropriate audio feedback for event failure.
        /// Convenience method for failure SFX.
        /// Requirements: R26.4
        /// </summary>
        public void PlayEventFailure()
        {
            PlaySFX(SoundEffect.Failure);
        }

        /// <summary>
        /// Play appropriate music for the given game context.
        /// Maps game screens to appropriate music tracks.
        /// Requirements: R26.1
        /// </summary>
        public void PlayMusicForContext(string context)
        {
            MusicTrack track = context?.ToLowerInvariant() switch
            {
                "mainmenu" or "main_menu" or "menu" => MusicTrack.MainMenu,
                "planning" or "plan" or "budget" or "vendor" or "venue" => MusicTrack.Planning,
                "execution" or "execute" or "event" or "live" => MusicTrack.Execution,
                "results" or "result" or "summary" => MusicTrack.Results,
                "celebration" or "success" or "win" => MusicTrack.Celebration,
                _ => MusicTrack.MainMenu
            };

            PlayMusic(track);
        }
    }
}
