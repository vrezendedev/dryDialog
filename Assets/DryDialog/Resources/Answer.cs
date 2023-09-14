using Newtonsoft.Json;
using UnityEngine;


[CreateAssetMenu(fileName = "NewAnswer", menuName = "ScriptableObjects/DryDialog/Answer", order = 3)]
public class Answer : ScriptableObject
{
    public string Respondant;
    [JsonIgnore] public Sprite RespondantPortrait;
    public int ID;
    public string Value;
    [TextArea(5, 0)] public string Sentence;
}
