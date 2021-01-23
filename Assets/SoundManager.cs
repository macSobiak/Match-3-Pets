using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> CatAudioClips;
    public List<AudioClip> SpecialAudioClips;
    public List<AudioClip> DogAudioClips;
    public AudioClip DestroySound;
    public SetGroup CatAudioSet;
    public SetGroup DogAudioSet;
    public SetGroup SpecialAudioSet;
    public AudioSource AudioSource;

    public void PlaySound(GameObject objectClicked)
    {
        if(DogAudioSet.GroupOfSets.Contains(objectClicked.GetComponent<BlockElement>().Block.BlockSet))
        {
            AudioSource.PlayOneShot(DogAudioClips[Random.Range(0, DogAudioClips.Count)]);
        }
        else if(CatAudioSet.GroupOfSets.Contains(objectClicked.GetComponent<BlockElement>().Block.BlockSet))
        {
            AudioSource.PlayOneShot(CatAudioClips[Random.Range(0, CatAudioClips.Count)]);
        }
        else if (SpecialAudioSet.GroupOfSets.Contains(objectClicked.GetComponent<BlockElement>().Block.BlockSet))
            AudioSource.PlayOneShot(SpecialAudioClips[Random.Range(0, SpecialAudioClips.Count)]);
    }
    public void PlayDestroySound()
    {
        AudioSource.PlayOneShot(DestroySound);
    }
}
