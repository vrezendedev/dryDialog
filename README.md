# Dry Dialog

DryDialog is a basic dialog system that base itself around "Expressions" that can be either Assertions or Questions - the dialog system works similar to a tree. Create new conversations and append expressions to it, call an event to trigger it at its manager and start talking! This is the first version of DryDialog developed for my final's paper game "Kingdom's Symphony".

## Features

1. Easily create extense Dialogues with options such as repeatable, speaker name and portrait...
2. Basic but useful Question/Answer System.
3. Few Manager customizations, like: Show Gradually, Seconds Between Expressions, Can Skip to Next Expression, Serialize Options...

## Disclaimer

It is a basic system developed for a MVP, feel free to tweak as you need it. When integrating with the answer event, an idea is to create a dictionary of <string, Func> and use the value returned as the key to execute what you need to be executed. I still need to create it as a Unity package or something like that, I'll figured it out later.

## Using

![Tutorial - Part 1](./pics/Tuto_01.png)

-   Create a new Conversation.

![Tutorial - Part 2](./pics/Tuto_02.png)

-   Customize it.

![Tutorial - Part 3](./pics/Tuto_03.png)

-   Create a new Expression, can be either an Assertion or Question. In this example, was created an Assertion... if you create a Question, you shall create the answers.

![Tutorial - Part 4](./pics/Tuto_04.png)

-   Customize it. The Feedback is where the magic happens, each Expression can have a Feedback of another Expression... imagine like the Conversation as the root of the tree and the Expressions and it's feedback's are branches.

![Tutorial - Part 5](./pics/Tuto_05.png)

-   Add the new created Expression to the Conversation's (created at Part 1) Expressions array.

![Tutorial - Part 6](./pics/Tuto_06.png)

-   Create a Dialog Manager like the one on the Example scene.

-   And then, finally, Call the Event "Talk" similar to this example:

    ```
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
```
-   Done!

## Examples of Dry Dialog

![Example  - 1](./pics/Example_One.png)

![Example  - 2](./pics/Example_Two.png)

## License

[MIT](https://choosealicense.com/licenses/mit/)
