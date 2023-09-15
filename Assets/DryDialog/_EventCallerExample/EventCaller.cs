using UnityEngine;

public class EventCaller : MonoBehaviour
{
    public Conversation conversation;

    void Start() => DialogManager.Talk(conversation);
}
