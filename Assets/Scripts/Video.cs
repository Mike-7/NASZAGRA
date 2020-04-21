using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class Video : MonoBehaviour
{
    VideoPlayer videoPlayer;

    void Start()
    {
#if UNITY_EDITOR
        Destroy(gameObject);
#endif

        videoPlayer = GetComponent<VideoPlayer>();
    }

    void Update()
    {
        if(!videoPlayer.isPlaying && videoPlayer.isPrepared)
        {
            Destroy(gameObject);
        }
    }
}
