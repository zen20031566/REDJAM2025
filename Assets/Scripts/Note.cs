public enum NoteType { Tap, SwipeUp }

public class Note
{
    public float time;     // When this note should be hit
    public bool hit;       // Whether it's already been hit
    public NoteType type;
}