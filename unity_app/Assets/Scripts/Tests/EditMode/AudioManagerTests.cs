using System;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Unit tests for AudioManagerImpl.
    /// Validates: Requirements R26.1-R26.6
    /// </summary>
    [TestFixture]
    public class AudioManagerTests
    {
        private AudioManagerImpl _audioManager;

        [SetUp]
        public void Setup()
        {
            _audioManager = new AudioManagerImpl();
        }

        #region Initialization Tests

        /// <summary>
        /// Test that AudioManager initializes with default values.
        /// </summary>
        [Test]
        public void Constructor_InitializesWithDefaultValues()
        {
            Assert.AreEqual(1.0f, _audioManager.MusicVolume, "Default music volume should be 1.0");
            Assert.AreEqual(1.0f, _audioManager.SFXVolume, "Default SFX volume should be 1.0");
            Assert.IsFalse(_audioManager.IsMuted, "Should not be muted by default");
            Assert.IsFalse(_audioManager.IsPaused, "Should not be paused by default");
            Assert.IsNull(_audioManager.CurrentTrack, "No track should be playing initially");
            Assert.IsFalse(_audioManager.IsMusicPlaying, "Music should not be playing initially");
        }

        /// <summary>
        /// Test that AudioManager can be initialized with custom values.
        /// </summary>
        [Test]
        public void Constructor_WithCustomValues_SetsCorrectly()
        {
            var manager = new AudioManagerImpl(0.5f, 0.7f, true);

            Assert.AreEqual(0.5f, manager.MusicVolume, 0.001f);
            Assert.AreEqual(0.7f, manager.SFXVolume, 0.001f);
            Assert.IsTrue(manager.IsMuted);
        }

        #endregion

        #region PlayMusic Tests (R26.1)

        /// <summary>
        /// Test that PlayMusic sets the current track.
        /// **Validates: Requirements R26.1**
        /// </summary>
        [Test]
        public void PlayMusic_SetsCurrentTrack()
        {
            _audioManager.PlayMusic(MusicTrack.Planning);

            Assert.AreEqual(MusicTrack.Planning, _audioManager.CurrentTrack);
            Assert.IsTrue(_audioManager.IsMusicPlaying);
        }

        /// <summary>
        /// Test that PlayMusic fires OnMusicChanged event.
        /// **Validates: Requirements R26.1**
        /// </summary>
        [Test]
        public void PlayMusic_FiresOnMusicChangedEvent()
        {
            MusicTrack? changedTrack = null;
            _audioManager.OnMusicChanged += track => changedTrack = track;

            _audioManager.PlayMusic(MusicTrack.Execution);

            Assert.AreEqual(MusicTrack.Execution, changedTrack);
        }

        /// <summary>
        /// Test that PlayMusic does not fire event if same track is already playing.
        /// **Validates: Requirements R26.1**
        /// </summary>
        [Test]
        public void PlayMusic_SameTrack_DoesNotFireEvent()
        {
            _audioManager.PlayMusic(MusicTrack.MainMenu);

            int eventCount = 0;
            _audioManager.OnMusicChanged += _ => eventCount++;

            _audioManager.PlayMusic(MusicTrack.MainMenu);

            Assert.AreEqual(0, eventCount, "Should not fire event for same track");
        }

        /// <summary>
        /// Test that PlayMusic changes track when different track requested.
        /// **Validates: Requirements R26.1**
        /// </summary>
        [Test]
        public void PlayMusic_DifferentTrack_ChangesTrack()
        {
            _audioManager.PlayMusic(MusicTrack.MainMenu);
            _audioManager.PlayMusic(MusicTrack.Planning);

            Assert.AreEqual(MusicTrack.Planning, _audioManager.CurrentTrack);
        }

        #endregion

        #region StopMusic Tests

        /// <summary>
        /// Test that StopMusic stops the current track.
        /// </summary>
        [Test]
        public void StopMusic_StopsCurrentTrack()
        {
            _audioManager.PlayMusic(MusicTrack.Planning);
            _audioManager.StopMusic();

            Assert.IsNull(_audioManager.CurrentTrack);
            Assert.IsFalse(_audioManager.IsMusicPlaying);
        }

        /// <summary>
        /// Test that StopMusic fires OnMusicChanged with null.
        /// </summary>
        [Test]
        public void StopMusic_FiresOnMusicChangedWithNull()
        {
            _audioManager.PlayMusic(MusicTrack.Planning);

            MusicTrack? changedTrack = MusicTrack.MainMenu; // Set to non-null
            _audioManager.OnMusicChanged += track => changedTrack = track;

            _audioManager.StopMusic();

            Assert.IsNull(changedTrack);
        }

        /// <summary>
        /// Test that StopMusic does nothing if no music is playing.
        /// </summary>
        [Test]
        public void StopMusic_NoMusicPlaying_DoesNothing()
        {
            int eventCount = 0;
            _audioManager.OnMusicChanged += _ => eventCount++;

            _audioManager.StopMusic();

            Assert.AreEqual(0, eventCount);
        }

        #endregion

        #region PlaySFX Tests (R26.2-R26.4)

        /// <summary>
        /// Test that PlaySFX fires OnSFXPlayed event.
        /// **Validates: Requirements R26.2**
        /// </summary>
        [Test]
        public void PlaySFX_FiresOnSFXPlayedEvent()
        {
            SoundEffect? playedSFX = null;
            _audioManager.OnSFXPlayed += sfx => playedSFX = sfx;

            _audioManager.PlaySFX(SoundEffect.ButtonClick);

            Assert.AreEqual(SoundEffect.ButtonClick, playedSFX);
        }

        /// <summary>
        /// Test that PlaySFX does not fire when muted.
        /// **Validates: Requirements R26.5**
        /// </summary>
        [Test]
        public void PlaySFX_WhenMuted_DoesNotFireEvent()
        {
            _audioManager.SetMuteAll(true);

            int eventCount = 0;
            _audioManager.OnSFXPlayed += _ => eventCount++;

            _audioManager.PlaySFX(SoundEffect.Success);

            Assert.AreEqual(0, eventCount);
        }

        /// <summary>
        /// Test that PlaySFX does not fire when paused.
        /// **Validates: Requirements R26.6**
        /// </summary>
        [Test]
        public void PlaySFX_WhenPaused_DoesNotFireEvent()
        {
            _audioManager.PauseAudio();

            int eventCount = 0;
            _audioManager.OnSFXPlayed += _ => eventCount++;

            _audioManager.PlaySFX(SoundEffect.Warning);

            Assert.AreEqual(0, eventCount);
        }

        /// <summary>
        /// Test that Success SFX can be played for event success.
        /// **Validates: Requirements R26.3**
        /// </summary>
        [Test]
        public void PlaySFX_Success_ForEventSuccess()
        {
            SoundEffect? playedSFX = null;
            _audioManager.OnSFXPlayed += sfx => playedSFX = sfx;

            _audioManager.PlaySFX(SoundEffect.Success);

            Assert.AreEqual(SoundEffect.Success, playedSFX);
        }

        /// <summary>
        /// Test that Failure SFX can be played for event failure.
        /// **Validates: Requirements R26.4**
        /// </summary>
        [Test]
        public void PlaySFX_Failure_ForEventFailure()
        {
            SoundEffect? playedSFX = null;
            _audioManager.OnSFXPlayed += sfx => playedSFX = sfx;

            _audioManager.PlaySFX(SoundEffect.Failure);

            Assert.AreEqual(SoundEffect.Failure, playedSFX);
        }

        #endregion

        #region Volume Control Tests (R26.5)

        /// <summary>
        /// Test that SetMusicVolume sets volume correctly.
        /// **Validates: Requirements R26.5**
        /// </summary>
        [Test]
        public void SetMusicVolume_SetsVolumeCorrectly()
        {
            _audioManager.SetMusicVolume(0.5f);

            Assert.AreEqual(0.5f, _audioManager.MusicVolume, 0.001f);
        }

        /// <summary>
        /// Test that SetMusicVolume clamps to valid range.
        /// **Validates: Requirements R26.5**
        /// </summary>
        [Test]
        public void SetMusicVolume_ClampsToValidRange()
        {
            _audioManager.SetMusicVolume(-0.5f);
            Assert.AreEqual(0f, _audioManager.MusicVolume, 0.001f);

            _audioManager.SetMusicVolume(1.5f);
            Assert.AreEqual(1f, _audioManager.MusicVolume, 0.001f);
        }

        /// <summary>
        /// Test that SetSFXVolume sets volume correctly.
        /// **Validates: Requirements R26.5**
        /// </summary>
        [Test]
        public void SetSFXVolume_SetsVolumeCorrectly()
        {
            _audioManager.SetSFXVolume(0.3f);

            Assert.AreEqual(0.3f, _audioManager.SFXVolume, 0.001f);
        }

        /// <summary>
        /// Test that SetSFXVolume clamps to valid range.
        /// **Validates: Requirements R26.5**
        /// </summary>
        [Test]
        public void SetSFXVolume_ClampsToValidRange()
        {
            _audioManager.SetSFXVolume(-1f);
            Assert.AreEqual(0f, _audioManager.SFXVolume, 0.001f);

            _audioManager.SetSFXVolume(2f);
            Assert.AreEqual(1f, _audioManager.SFXVolume, 0.001f);
        }

        /// <summary>
        /// Test that volume changes fire OnVolumeChanged event.
        /// **Validates: Requirements R26.5**
        /// </summary>
        [Test]
        public void SetVolume_FiresOnVolumeChangedEvent()
        {
            float? musicVol = null;
            float? sfxVol = null;
            _audioManager.OnVolumeChanged += (m, s) => { musicVol = m; sfxVol = s; };

            _audioManager.SetMusicVolume(0.6f);

            Assert.AreEqual(0.6f, musicVol, 0.001f);
            Assert.AreEqual(1.0f, sfxVol, 0.001f);
        }

        #endregion

        #region Mute Tests (R26.5)

        /// <summary>
        /// Test that SetMuteAll mutes audio.
        /// **Validates: Requirements R26.5**
        /// </summary>
        [Test]
        public void SetMuteAll_True_MutesAudio()
        {
            _audioManager.SetMuteAll(true);

            Assert.IsTrue(_audioManager.IsMuted);
            Assert.AreEqual(0f, _audioManager.MusicVolume, 0.001f);
            Assert.AreEqual(0f, _audioManager.SFXVolume, 0.001f);
        }

        /// <summary>
        /// Test that SetMuteAll restores previous volumes when unmuting.
        /// **Validates: Requirements R26.5**
        /// </summary>
        [Test]
        public void SetMuteAll_False_RestoresPreviousVolumes()
        {
            _audioManager.SetMusicVolume(0.7f);
            _audioManager.SetSFXVolume(0.8f);

            _audioManager.SetMuteAll(true);
            _audioManager.SetMuteAll(false);

            Assert.IsFalse(_audioManager.IsMuted);
            Assert.AreEqual(0.7f, _audioManager.MusicVolume, 0.001f);
            Assert.AreEqual(0.8f, _audioManager.SFXVolume, 0.001f);
        }

        /// <summary>
        /// Test that SetMuteAll fires OnMuteChanged event.
        /// **Validates: Requirements R26.5**
        /// </summary>
        [Test]
        public void SetMuteAll_FiresOnMuteChangedEvent()
        {
            bool? muteState = null;
            _audioManager.OnMuteChanged += muted => muteState = muted;

            _audioManager.SetMuteAll(true);

            Assert.IsTrue(muteState);
        }

        /// <summary>
        /// Test that SetMuteAll does nothing if already in requested state.
        /// **Validates: Requirements R26.5**
        /// </summary>
        [Test]
        public void SetMuteAll_SameState_DoesNotFireEvent()
        {
            int eventCount = 0;
            _audioManager.OnMuteChanged += _ => eventCount++;

            _audioManager.SetMuteAll(false); // Already false

            Assert.AreEqual(0, eventCount);
        }

        #endregion

        #region Pause/Resume Tests (R26.6)

        /// <summary>
        /// Test that PauseAudio pauses audio playback.
        /// **Validates: Requirements R26.6**
        /// </summary>
        [Test]
        public void PauseAudio_PausesPlayback()
        {
            _audioManager.PlayMusic(MusicTrack.Planning);

            _audioManager.PauseAudio();

            Assert.IsTrue(_audioManager.IsPaused);
            Assert.IsFalse(_audioManager.IsMusicPlaying, "IsMusicPlaying should be false when paused");
        }

        /// <summary>
        /// Test that ResumeAudio resumes audio playback.
        /// **Validates: Requirements R26.6**
        /// </summary>
        [Test]
        public void ResumeAudio_ResumesPlayback()
        {
            _audioManager.PlayMusic(MusicTrack.Planning);
            _audioManager.PauseAudio();

            _audioManager.ResumeAudio();

            Assert.IsFalse(_audioManager.IsPaused);
            Assert.IsTrue(_audioManager.IsMusicPlaying);
        }

        /// <summary>
        /// Test that PauseAudio fires OnPauseChanged event.
        /// **Validates: Requirements R26.6**
        /// </summary>
        [Test]
        public void PauseAudio_FiresOnPauseChangedEvent()
        {
            bool? pauseState = null;
            _audioManager.OnPauseChanged += paused => pauseState = paused;

            _audioManager.PauseAudio();

            Assert.IsTrue(pauseState);
        }

        /// <summary>
        /// Test that ResumeAudio fires OnPauseChanged event.
        /// **Validates: Requirements R26.6**
        /// </summary>
        [Test]
        public void ResumeAudio_FiresOnPauseChangedEvent()
        {
            _audioManager.PauseAudio();

            bool? pauseState = null;
            _audioManager.OnPauseChanged += paused => pauseState = paused;

            _audioManager.ResumeAudio();

            Assert.IsFalse(pauseState);
        }

        /// <summary>
        /// Test that PauseAudio does nothing if already paused.
        /// **Validates: Requirements R26.6**
        /// </summary>
        [Test]
        public void PauseAudio_AlreadyPaused_DoesNotFireEvent()
        {
            _audioManager.PauseAudio();

            int eventCount = 0;
            _audioManager.OnPauseChanged += _ => eventCount++;

            _audioManager.PauseAudio();

            Assert.AreEqual(0, eventCount);
        }

        /// <summary>
        /// Test that ResumeAudio does nothing if not paused.
        /// **Validates: Requirements R26.6**
        /// </summary>
        [Test]
        public void ResumeAudio_NotPaused_DoesNotFireEvent()
        {
            int eventCount = 0;
            _audioManager.OnPauseChanged += _ => eventCount++;

            _audioManager.ResumeAudio();

            Assert.AreEqual(0, eventCount);
        }

        /// <summary>
        /// Test that track is preserved during pause/resume.
        /// **Validates: Requirements R26.6**
        /// </summary>
        [Test]
        public void PauseResume_PreservesCurrentTrack()
        {
            _audioManager.PlayMusic(MusicTrack.Execution);

            _audioManager.PauseAudio();
            Assert.AreEqual(MusicTrack.Execution, _audioManager.CurrentTrack);

            _audioManager.ResumeAudio();
            Assert.AreEqual(MusicTrack.Execution, _audioManager.CurrentTrack);
        }

        #endregion

        #region Convenience Method Tests

        /// <summary>
        /// Test that PlayEventSuccess plays success audio.
        /// **Validates: Requirements R26.3**
        /// </summary>
        [Test]
        public void PlayEventSuccess_PlaysSuccessAudio()
        {
            SoundEffect? playedSFX = null;
            MusicTrack? playedMusic = null;
            _audioManager.OnSFXPlayed += sfx => playedSFX = sfx;
            _audioManager.OnMusicChanged += track => playedMusic = track;

            _audioManager.PlayEventSuccess();

            Assert.AreEqual(SoundEffect.Success, playedSFX);
            Assert.AreEqual(MusicTrack.Celebration, playedMusic);
        }

        /// <summary>
        /// Test that PlayEventFailure plays failure audio.
        /// **Validates: Requirements R26.4**
        /// </summary>
        [Test]
        public void PlayEventFailure_PlaysFailureAudio()
        {
            SoundEffect? playedSFX = null;
            _audioManager.OnSFXPlayed += sfx => playedSFX = sfx;

            _audioManager.PlayEventFailure();

            Assert.AreEqual(SoundEffect.Failure, playedSFX);
        }

        /// <summary>
        /// Test that PlayMusicForContext maps contexts correctly.
        /// **Validates: Requirements R26.1**
        /// </summary>
        [Test]
        public void PlayMusicForContext_MapsContextsCorrectly()
        {
            _audioManager.PlayMusicForContext("mainmenu");
            Assert.AreEqual(MusicTrack.MainMenu, _audioManager.CurrentTrack);

            _audioManager.PlayMusicForContext("planning");
            Assert.AreEqual(MusicTrack.Planning, _audioManager.CurrentTrack);

            _audioManager.PlayMusicForContext("execution");
            Assert.AreEqual(MusicTrack.Execution, _audioManager.CurrentTrack);

            _audioManager.PlayMusicForContext("results");
            Assert.AreEqual(MusicTrack.Results, _audioManager.CurrentTrack);

            _audioManager.PlayMusicForContext("celebration");
            Assert.AreEqual(MusicTrack.Celebration, _audioManager.CurrentTrack);
        }

        /// <summary>
        /// Test that GetEffectiveMusicVolume returns 0 when muted.
        /// </summary>
        [Test]
        public void GetEffectiveMusicVolume_WhenMuted_ReturnsZero()
        {
            _audioManager.SetMusicVolume(0.8f);
            _audioManager.SetMuteAll(true);

            Assert.AreEqual(0f, _audioManager.GetEffectiveMusicVolume(), 0.001f);
        }

        /// <summary>
        /// Test that GetEffectiveSFXVolume returns 0 when muted.
        /// </summary>
        [Test]
        public void GetEffectiveSFXVolume_WhenMuted_ReturnsZero()
        {
            _audioManager.SetSFXVolume(0.9f);
            _audioManager.SetMuteAll(true);

            Assert.AreEqual(0f, _audioManager.GetEffectiveSFXVolume(), 0.001f);
        }

        #endregion
    }
}
