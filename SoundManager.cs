using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;
using System.Linq;

public class SoundManager : MonoBehaviour
{
    [SerializeField] ConsoleController console;

    public Stack<int> requests = new Stack<int>();
    [SerializeField] List<Song> songs;
    public Song currentSong;

    bool isTransitioning = false;

    //Priority:
    //5: Plot Event
    //4: Final Hour
    //3: Redactor
    //2: Dogfight
    //1: Ambience

    //IDs:
    //0 - Final Hour
    //1 - Ambience1
    //2 - Ambience2
    //3 - Dogfight
    //4 - Redactor

    // Update is called once per frame
    void Update()
    {
        CheckPause();
        keepMusicPlaying();
        ambienceRequest();
        handleRequests();
    }

    void CheckPause()
    {
        if (Time.timeScale != 1 && currentSong.clip != null)
        {
            currentSong.clip.Pause();
            return;
        }
        else if(Time.timeScale == 1 && currentSong.clip != null)
        {
            currentSong.clip.UnPause();
        }
    }

    void keepMusicPlaying()
    {
        if(isTransitioning) return;
        if (currentSong.clip != null)
        {
            currentSong.clip.volume = 1;
            if (!currentSong.clip.isPlaying)
            {
                currentSong.clip.Play();
            }
        }
    }

    public void Request(int id) //I'll just keep this little song request bin.
    {
        requests.Push(id);
    }

    public void EndRequest(int id) //If an exit condition happens for a high priority song, we'll go through with it.
    {
        Song requestedSongEnd = songs[id];
        if (currentSong.clip != null && requestedSongEnd.clip == currentSong.clip)
        {
            StartCoroutine(fadeOut());
        }
    }

    void handleRequests() //Our fans will tell us to play their favorite songs because we are their favorite DJ. But not all songs are created equal. I have to choose which one to play, and make sure it doesn't loop every frame.
    {
        print("REQUESTS: " + requests);
        Song highestPrioritySong = new Song();
        while(requests.Count > 0)
        {
            int id = requests.Pop();
            Song requestedSong = songs[id];
            if(currentSong.clip == null)
            {
                currentSong.clip = requestedSong.clip; //Normally this would cause problems (current song plays no matter what and we're just playing a random song here). But we switch everything around in the same frame anyway, the player probably wont notice.
                currentSong.priority = requestedSong.priority;
            }
            if(requestedSong.clip == currentSong.clip)
            {
                continue;
            }
            if(requestedSong.priority >= currentSong.priority && requestedSong.priority >= highestPrioritySong.priority) //If our currently selected track has higher priority than the current music and the last music we've pulled, select it.
            {
                highestPrioritySong = requestedSong;
            }
        }
        if (highestPrioritySong.clip != null)
        {
            print("HIGHEST PRIORITY: " +  highestPrioritySong.clip.name);
            StartCoroutine(transition(highestPrioritySong));
        }
    }

    void ambienceRequest()
    {
        if (console.canWarp && !console.navMode) //Make sure we're not changing waypoints.
        {
            //Waypoints: 0 - Initial Point, 1 - Cluster Field, 2 - Sancturary, 3 - Greatness, 4 - Dead Man's Zone
            switch (console.currentWaypoint)
            {
                case 1:
                    Request(1);
                    break;
                case 2:
                    Request(2);
                    break;
                case 3:
                    Request(1);
                    break;
                case 4:
                    Request(2);
                    break;
            }
        }
    }

    IEnumerator transition(Song newSong) //This little trick is why I'm everyone's favorite DJ.
    {
        while(isTransitioning)
        {
            print("TRANSITION IS AWAITING OTHER TRANSITION");
            yield return null;
        }
        isTransitioning = true;
        if (currentSong.clip != null && currentSong.clip.volume != 0) //If we have a song already playing, fade it out.
        {
            while (currentSong.clip.volume > 0)
            {
                currentSong.clip.volume -= Time.deltaTime * 3f;
                yield return new WaitForEndOfFrame();
            }
            print("TRANSITIONER IS FADING OUT");
        }

        currentSong.clip = newSong.clip;
        currentSong.priority = newSong.priority;
        currentSong.clip.Play();

        while (currentSong.clip.volume < 1) //Bring in the new music.
        {
            currentSong.clip.volume += Time.deltaTime*3f;
            yield return new WaitForEndOfFrame();
        }
        print("TRANSITIONER HAS COME BACK!");
        isTransitioning = false;
    }

    IEnumerator fadeOut()
    {
        while(isTransitioning)
        {
            print("FADING OUT, AWAITING TRANSITION.");
            yield return null;
        }
        print("FADE OUT COMMENCED");
        isTransitioning = true;
        if (currentSong.clip != null && currentSong.clip.volume != 0) //If we got music, fade it out.
        {
            while (currentSong.clip.volume > 0)
            {
                currentSong.clip.volume -= Time.deltaTime * 3f;
                yield return new WaitForEndOfFrame();
            }
        }
        print("SONG NO LONGER PLAYING");
        currentSong.clip = null;
        currentSong.priority = 0;
        isTransitioning = false;
    }

    [System.Serializable]
    public class Song
    {
        public AudioSource clip = null;
        public int priority = 0;
    }
}
