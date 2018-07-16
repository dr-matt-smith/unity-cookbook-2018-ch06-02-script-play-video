using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/**
 * adadpted from example post by 'Programmer' at Stackoverflow November 2017
 * https://stackoverflow.com/questions/47401759/play-different-videos-back-to-back-seamlessly-using-unity-video-player
 */

public class SceneTextureVideoSequence : MonoBehaviour
{
    //Raw Image to Show Video Images [Assign from the Editor]
    public RawImage image;

    public RenderTexture renderTexture;

    //Set from the Editor
    public List<VideoClip> videoClips;

    private List<VideoPlayer> videoPlayers;
    private int currentVideoIndex;

    void Start()
    {
        FirstRunActions();

        //Prepare first video
        currentVideoIndex = 0;
        
        PrepareAndPlay(videoPlayers[currentVideoIndex]);
    }
    
    private void PrepareAndPlay(VideoPlayer theVideoPlayer)
    {
        // register method to invoke AFTER preparation is completed
        theVideoPlayer.prepareCompleted += PlayNextVideo;
        
        // prepare video clip
        theVideoPlayer.Prepare();
        Debug.Log("A - PREPARING video: " + currentVideoIndex);        
    }
    
        
    private void PlayNextVideo(VideoPlayer theVideoPlayer)
    {
        VideoPlayer currentVideoPlayer = videoPlayers[currentVideoIndex];

        //Assign the Texture from Video to RawImage to be displayed
//        image.texture = currentVideoPlayer.texture;
//        image.texture = renderTexture;

        // Play video
        Debug.Log("B - PLAYING Index: " + currentVideoIndex);
        currentVideoPlayer.Play();
        
        // IF more clips remaining THEN prepare then and play when current clip finished
        currentVideoIndex++;
        bool someVideosLeft = currentVideoIndex < videoPlayers.Count;

        if (someVideosLeft) {
            // start Preparing next clip
            currentVideoPlayer.Prepare();
            Debug.Log("A - PREPARING video: " + currentVideoIndex);        
        
            // register to play next video once current one has finished
            currentVideoPlayer.loopPointReached += PlayNextVideo;            
        }
        else {
            Debug.Log("(no videos left)");
        }

    }
    
    private void FirstRunActions()
    {
        videoPlayers = new List<VideoPlayer>();
        for (int i = 0; i < videoClips.Count; i++)
        {
            string newGameObjectName = "videoPlayer_" + i;
            SetupVideoAudioPlayers(newGameObjectName, videoClips[i]);
        }
    }

    private void SetupVideoAudioPlayers(string goName, VideoClip videoClip)
    {
        //Create new Object to hold the Video and the sound then make it a child of this object
        GameObject containterGo = new GameObject(goName);
        containterGo.transform.SetParent(transform);

        // add video player and audio source components
        VideoPlayer videoPlayer = gameObject.AddComponent<VideoPlayer>();
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        // add reference to new video player to array
        videoPlayers.Add(videoPlayer);

        // disable Play on Awake for both vide and audio
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;

        // assign video clip
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoClip;

        // setup AudioSource 
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);
        
        // output video to RenderTexture
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;

    }
}
