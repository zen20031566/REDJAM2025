using UnityEngine;

public enum NoteType
{
    Tap, Hold, SwipeUp, SwipeDown, Shout
}

public class NoteData
{
    public float hitTiming;
    public float releaseTiming; //only used for hold
    public NoteType type;
}
