import Foundation

protocol AudioManagerProtocol {
    var musicVolume: Double { get }
    var sfxVolume: Double { get }
    var isMuted: Bool { get }
    var isPaused: Bool { get }
    var currentTrack: MusicTrack? { get }
    var isMusicPlaying: Bool { get }
    func playMusic(_ track: MusicTrack)
    func stopMusic()
    func pauseMusic()
    func resumeMusic()
    func playSFX(_ effect: SoundEffect)
    func setMusicVolume(_ volume: Double)
    func setSFXVolume(_ volume: Double)
    func mute()
    func unmute()
    func toggleMute()
    func playMusicForStage(_ stage: Int)
}
