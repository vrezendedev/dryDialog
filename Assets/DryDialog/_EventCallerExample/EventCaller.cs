using System;
using UnityEngine;

public class EventCaller : MonoBehaviour
{
    public Conversation conversation;

    void OnEnable()
    {
        DialogManager.AnswerChoosen += HandleAnswerChoosen;
    }

    void OnDisable()
    {
        DialogManager.AnswerChoosen -= HandleAnswerChoosen;
    }

    private void HandleAnswerChoosen(string value)
    {
        Debug.Log(value);
    }

    void Start() => DialogManager.Talk(conversation);
}
