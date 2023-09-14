using Newtonsoft.Json;
using UnityEngine;

public class Expression : ScriptableObject
{
    public string Speaker;
    [JsonIgnore] public Sprite SpeakerPortrait;
    [TextArea(5, 0)] public string Sentence;
}
