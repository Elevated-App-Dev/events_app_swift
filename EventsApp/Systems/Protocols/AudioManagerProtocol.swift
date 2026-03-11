import Foundation

protocol AudioManagerProtocol {
    var musicVolume: Double { get set }
    var sfxVolume: Double { get set }
    var isMuted: Bool { get }
    var isPaused: Bool { get }
    var currentTrack: MusicTrack? { get }
    var isMusicPlaying: Bool { get }
    func playMusic(_ track: MusicTrack)
    func stopMusic()
    func playSFX(_ effect: SoundEffect)
    func setMusicVolume(_ volume: Double)
    func setSFXVolume(_ volume: Double)
    func setMuteAll(_ muted: Bool)
    func pauseAudio()
    func resumeAudio()
}
