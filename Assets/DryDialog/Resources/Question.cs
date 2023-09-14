using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestion", menuName = "ScriptableObjects/DryDialog/Question", order = 2)]
public class Question : Expression
{
    public int ID;
    [JsonIgnore] public Answer[] Options;
    [JsonIgnore] public Expression[] Feedback;
}
