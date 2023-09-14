using UnityEngine;

[CreateAssetMenu(fileName = "NewAssertion", menuName = "ScriptableObjects/DryDialog/Assertion", order = 1)]
public class Assertion : Expression
{
    public Expression Feedback;
}
