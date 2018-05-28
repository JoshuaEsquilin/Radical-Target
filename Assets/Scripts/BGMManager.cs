using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour {

    public AudioSource bgm;
    public AudioClip[] normalStageTrackList; // Stage Type = 0
    public AudioClip[] bossStageTrackList; // Stage Type = 1
    public bool loopMusic = false;
    public int stageType = 0;
    public int currentTrack = 0;
    public int volume = 25;

    private AudioClip[][] stageTrackLists;
    
    void Start() {
        stageTrackLists = new AudioClip[][] { normalStageTrackList, bossStageTrackList };
        bgm = GetComponent<AudioSource>();
        bgm.loop = loopMusic;
        bgm.volume = volume / 100f;

        bgm.clip = stageTrackLists[stageType][currentTrack];
    }

    void FixedUpdate() {
        if (!bgm.isPlaying) {
            PlayTrack(currentTrack++);
        }
    }

    void PlayTrack(int trackNo) {
        bgm.Stop();
        bgm.clip = stageTrackLists[stageType][trackNo];
        bgm.Play();
    }

    void SetStageType(int stageType) {
        this.stageType = stageType;
        currentTrack = 0;
        bgm.Stop();
        bgm.clip = stageTrackLists[stageType][currentTrack];
        bgm.Play();
    }

    void SetStageType(int stageType, int trackNo) {
        this.stageType = stageType;
        currentTrack = trackNo;
        bgm.Stop();
        bgm.clip = stageTrackLists[stageType][currentTrack];
        bgm.Play();
    }
}
