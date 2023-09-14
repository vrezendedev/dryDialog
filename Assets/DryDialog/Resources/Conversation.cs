using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "NewConversation", menuName = "ScriptableObjects/DryDialog/Conversation", order = 4)]
public class Conversation : ScriptableObject
{
    public int ID;
    public bool Repeatable = false;
    [JsonIgnore] public Expression[] Expressions;
}
