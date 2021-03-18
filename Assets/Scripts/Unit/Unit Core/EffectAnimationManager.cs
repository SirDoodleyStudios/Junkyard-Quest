using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Playables;
using UnityEngine.Animations;


public class EffectAnimationManager : MonoBehaviour
{
    public AnimationClip clip;
    PlayableGraph playableGraph;

    private void Awake()
    { 

    }
    private void Start()
    {
        playableGraph = PlayableGraph.Create();
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());

        // Wrap the clip in a playable

        AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);

        // Connect the Playable to an output

        playableOutput.SetSourcePlayable(clipPlayable);

        // Plays the Graph.

        playableGraph.Play();

        playableGraph.Stop();
        //if (playableGraph.IsDone())
        //{

        //    playableGraph.Stop();
        //}

    }
}
