using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Manages background music and sound effects.
    /// Requirements: R26.1-R26.6
    /// </summary>
    public interface IAudioManager
    {
        /// <summary>
        /// Play background music for a screen/context.
        /// Requirements: R26.1
        /// </summary>
        void PlayMusic(MusicTrack track);

        /// <summary>
        /// Stop current background music.
        /// </summary>
        void StopMusic();

        /// <summary>
        /// Play a sound effect.
        /// Requirements: R26.2-R26.4
        /// </summary>
        void PlaySFX(SoundEffect sfx);

        /// <summary>
        /// Set music volume (0-1).
        /// Requirements: R26.5
        /// </summary>
        void SetMusicVolume(float volume);

        /// <summary>
        /// Set sound effects volume (0-1).
        /// Requirements: R26.5
        /// </summary>
        void SetSFXVolume(float volume);

        /// <summary>
        /// Mute/unmute all audio.
        /// Requirements: R26.5
        /// </summary>
        void SetMuteAll(bool muted);

        /// <summary>
        /// Pause audio when app loses focus.
        /// Requirements: R26.6
        /// </summary>
        void PauseAudio();

        /// <summary>
        /// Resume audio when app regains focus.
        /// Requirements: R26.6
        /// </summary>
        void ResumeAudio();

        /// <summary>
        /// Get the current music volume (0-1).
        /// </summary>
        float MusicVolume { get; }

        /// <summary>
        /// Get the current SFX volume (0-1).
        /// </summary>
        float SFXVolume { get; }

        /// <summary>
        /// Check if all audio is muted.
        /// </summary>
        bool IsMuted { get; }

        /// <summary>
        /// Check if audio is currently paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Get the currently playing music track, or null if none.
        /// </summary>
        MusicTrack? CurrentTrack { get; }

        /// <summary>
        /// Check if music is currently playing.
        /// </summary>
        bool IsMusicPlaying { get; }

        /// <summary>
        /// Event fired when music track changes.
        /// </summary>
        event Action<MusicTrack?> OnMusicChanged;

        /// <summary>
        /// Event fired when a sound effect is played.
        /// </summary>
        event Action<SoundEffect> OnSFXPlayed;

        /// <summary>
        /// Event fired when volume settings change.
        /// </summary>
        event Action<float, float> OnVolumeChanged;

        /// <summary>
        /// Event fired when mute state changes.
        /// </summary>
        event Action<bool> OnMuteChanged;

        /// <summary>
        /// Event fired when pause state changes.
        /// </summary>
        event Action<bool> OnPauseChanged;
    }
}
