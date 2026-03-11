import Foundation
import Combine

/// Audio system for background music and sound effects.
/// Pure Swift logic — actual AVFoundation playback to be added in Xcode.
class AudioManager: AudioManagerProtocol {

    private(set) var musicVolume: Double = 1.0
    private(set) var sfxVolume: Double = 1.0
    private(set) var isMuted: Bool = false
    private(set) var isPaused: Bool = false
    private(set) var currentTrack: MusicTrack?
    private(set) var isMusicPlaying: Bool = false

    private var preMuteMusicVolume: Double = 1.0
    private var preMuteSFXVolume: Double = 1.0

    private let _onMusicChanged = PassthroughSubject<MusicTrack?, Never>()
    private let _onSFXPlayed = PassthroughSubject<SoundEffect, Never>()
    private let _onVolumeChanged = PassthroughSubject<(Double, Double), Never>()

    var onMusicChanged: AnyPublisher<MusicTrack?, Never> { _onMusicChanged.eraseToAnyPublisher() }
    var onSFXPlayed: AnyPublisher<SoundEffect, Never> { _onSFXPlayed.eraseToAnyPublisher() }
    var onVolumeChanged: AnyPublisher<(Double, Double), Never> { _onVolumeChanged.eraseToAnyPublisher() }

    // MARK: - Music

    func playMusic(_ track: MusicTrack) {
        currentTrack = track
        isMusicPlaying = true
        isPaused = false
        _onMusicChanged.send(track)
    }

    func stopMusic() {
        currentTrack = nil
        isMusicPlaying = false
        _onMusicChanged.send(nil)
    }

    func pauseMusic() {
        guard isMusicPlaying else { return }
        isPaused = true
    }

    func resumeMusic() {
        guard isPaused else { return }
        isPaused = false
    }

    // MARK: - Sound Effects

    func playSFX(_ effect: SoundEffect) {
        guard !isMuted, sfxVolume > 0 else { return }
        _onSFXPlayed.send(effect)
    }

    // MARK: - Volume

    func setMusicVolume(_ volume: Double) {
        musicVolume = max(0, min(1, volume))
        _onVolumeChanged.send((musicVolume, sfxVolume))
    }

    func setSFXVolume(_ volume: Double) {
        sfxVolume = max(0, min(1, volume))
        _onVolumeChanged.send((musicVolume, sfxVolume))
    }

    func mute() {
        guard !isMuted else { return }
        preMuteMusicVolume = musicVolume
        preMuteSFXVolume = sfxVolume
        isMuted = true
    }

    func unmute() {
        guard isMuted else { return }
        isMuted = false
        musicVolume = preMuteMusicVolume
        sfxVolume = preMuteSFXVolume
        _onVolumeChanged.send((musicVolume, sfxVolume))
    }

    func toggleMute() {
        isMuted ? unmute() : mute()
    }

    // MARK: - Music for Game State

    func playMusicForStage(_ stage: Int) {
        // Stage-specific themes not yet defined; use planning track for gameplay
        playMusic(.planning)
    }
}
