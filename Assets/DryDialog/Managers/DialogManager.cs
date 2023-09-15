using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


[RequireComponent(typeof(PlayerInput))]
public class DialogManager : MonoBehaviour
{
    [Header("Required:")]
    public GameObject textPanel;
    public TextMeshProUGUI textHolder;
    [Tooltip("Must have a Button with TMP...")] public GameObject answerTemplate;


    [Header("Recommended:")]
    public RawImage speakerPortrait;
    public TextMeshProUGUI speakerName;
    public RawImage respondantPortrait;
    public TextMeshProUGUI respondantName;


    [Header("Customizations:")]
    public bool showGradually = false;
    public float secondsBetweenChars = 0.2f;
    public float secondsBetweenExpressions = 5f;
    public float spaceBetweenAnswers = 2.2f;
    public bool canSkipToNextExpression = false;

    [Header("Options")]
    [Tooltip("Highly recommend in order to use the Repeatable feature during different runtimes")] public bool serializeConversation = false;
    public bool serializeQuestionsAndAnswers = false;
    public string serializeConversationFileName = "cvsHstrc";
    public string serializeQuestionsAndAnswersFileName = "qaHstrc";

    [HideInInspector] public bool _isTalking = false;
    private Coroutine _talkingCoroutine = null;
    private Expression _currentExpression = null;
    private Conversation _currentConversation = null;
    private int _currentConversationExpressionIndex = 0;

    private List<Conversation> _conversationHistoric = new List<Conversation>();
    private List<Tuple<Question, Answer>> _questionsAndAnswerHistoric = new List<Tuple<Question, Answer>>();

    public static UnityAction<Conversation> Talk;

    void OnEnable() => Talk += HandleTalk;
    void OnDisable() => Talk -= HandleTalk;

    void Awake()
    {
        try
        {
            _conversationHistoric = Deserialize<List<Conversation>>(serializeConversationFileName);
            _questionsAndAnswerHistoric = Deserialize<List<Tuple<Question, Answer>>>(serializeQuestionsAndAnswersFileName);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void HandleTalk(Conversation cvs)
    {
        if (cvs == null) return;

        if (cvs.Repeatable)
        {
            StartTalking(cvs);
        }
        else
        {
            if (!_conversationHistoric.Exists(obj => obj.ID == cvs.ID))
            {
                StartTalking(cvs);
            }
        }

    }

    private void OnSkipToNextExpression()
    {
        if (_isTalking && canSkipToNextExpression && _talkingCoroutine != null)
        {
            StopAllCoroutines();
            _talkingCoroutine = null;
            switch (_currentExpression)
            {
                case Assertion a:
                    HandleExpression(a.Feedback);
                    break;
                case Question q:

                    textHolder.text = q.Sentence;

                    GenerateAnswers();
                    break;
                default:
                    break;
            }
        }
    }

    private void HandleExpression(Expression exp)
    {
        StopAllCoroutines();
        _talkingCoroutine = null;
        textHolder.text = "";

        if (exp == null)
        {
            _currentConversationExpressionIndex++;

            if (_currentConversationExpressionIndex >= _currentConversation.Expressions.Length)
            {
                _isTalking = false;

                if (speakerPortrait != null)
                    speakerPortrait.gameObject.SetActive(false);

                if (speakerName != null)
                    speakerName.gameObject.SetActive(false);

                return;
            }
            else
            {
                exp = _currentConversation.Expressions[_currentConversationExpressionIndex];
            }
        }

        _currentExpression = exp;

        if (speakerPortrait != null)
            speakerPortrait.texture = exp.SpeakerPortrait.texture;

        if (speakerName != null)
            speakerName.text = exp.Speaker;

        switch (exp)
        {
            case Assertion a:
                ExpressAssertion(exp);
                break;
            case Question q:
                ExpressQuestion(exp);
                break;
            default:
                break;
        }
    }

    private void StartTalking(Conversation cvs)
    {
        bool alreadyAddedOnHistoric = _conversationHistoric.Exists(obj => obj.ID == cvs.ID);

        _isTalking = true;

        if (speakerPortrait != null)
            speakerPortrait.gameObject.SetActive(true);

        if (cvs.Expressions.Length > 0)
        {
            _currentConversation = cvs;
            HandleExpression(cvs.Expressions[0]);
        }
        else
        {
            _isTalking = false;
        }

        if (!alreadyAddedOnHistoric)
        {
            _conversationHistoric.Add(_currentConversation);
            if (serializeConversation)
                Serialize(serializeConversationFileName, _conversationHistoric);
        }
    }

    private void ExpressAssertion(Expression exp) => _talkingCoroutine = StartCoroutine(Express(exp.Sentence, () => HandleExpression((exp as Assertion).Feedback)));

    private void ExpressQuestion(Expression exp) => _talkingCoroutine = StartCoroutine(Express(exp.Sentence, GenerateAnswers));

    private void GenerateAnswers()
    {
        var q = _currentExpression as Question;

        for (int i = 0; i < q.Options.Length; i++)
        {
            int index = i;
            var obj = Instantiate(answerTemplate, textPanel.transform);
            var tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = q.Options[i].Sentence;
            var tPos = obj.GetComponent<RectTransform>();
            var inpT = textHolder.gameObject.GetComponent<RectTransform>();
            var position = new Vector3(inpT.localPosition.x, inpT.localPosition.y + (tmp.fontSize * (i + 1) * spaceBetweenAnswers * -1), tPos.position.z);
            tPos.localPosition = position;
            var orgBtn = obj.GetComponent<Button>();
            orgBtn.onClick.AddListener(delegate { HandleChosenAnswer(index); });
            var btn = obj.AddComponent<ButtonUI>();
            btn.SetData(orgBtn, q.Options[i].RespondantPortrait, q.Options[i].Respondant, respondantPortrait, respondantName);
        }
    }

    private void HandleChosenAnswer(int index)
    {
        foreach (Button child in textPanel.GetComponentsInChildren<Button>())
        {
            child.onClick.RemoveAllListeners();
            Destroy(child.gameObject);
        }

        if (serializeQuestionsAndAnswers)
        {
            _questionsAndAnswerHistoric.Add(new Tuple<Question, Answer>(_currentExpression as Question, (_currentExpression as Question).Options[index]));
            Serialize(serializeQuestionsAndAnswersFileName, _questionsAndAnswerHistoric);
        }

        if (respondantPortrait != null)
            respondantPortrait.gameObject.SetActive(false);

        if (respondantName != null)
            respondantName.gameObject.SetActive(false);

        if (respondantName != null)

            HandleExpression((_currentExpression as Question).Feedback[index]);
    }

    private IEnumerator Express(string sentence, Action callback = null)
    {

        if (showGradually)
        {
            foreach (var c in sentence)
            {
                textHolder.text += c;
                yield return new WaitForSeconds(secondsBetweenChars);
            }
        }
        else
            textHolder.text = sentence;

        yield return new WaitForSeconds(secondsBetweenExpressions);

        if (callback != null)
        {
            callback.Invoke();
        }

        yield return null;
    }

    private bool Serialize<T>(string extraPath, T serializable)
    {
        string path = Application.persistentDataPath + "/" + extraPath + ".json";

        try
        {
            if (File.Exists(path))
                File.Delete(path);

            FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(serializable));
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return false;
        }
    }

    public static T Deserialize<T>(string extraPath)
    {
        string path = Application.persistentDataPath + "/" + extraPath + ".json";

        if (!File.Exists(path)) throw new FileNotFoundException($"File with path [{path}] not found.");

        try
        {
            T data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            throw ex;
        }
    }

}
class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public Sprite respondantPortrait;
    public string respondantName;
    public RawImage uiRespondantPortrait;
    public TextMeshProUGUI uiRespondantName;

    public void SetData(Button b, Sprite rP, string rN, RawImage uiRP, TextMeshProUGUI uiRN)
    {
        button = b;
        respondantPortrait = rP;
        respondantName = rN;
        uiRespondantPortrait = uiRP;
        uiRespondantName = uiRN;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (uiRespondantPortrait != null)
        {
            uiRespondantPortrait.gameObject.SetActive(true);
            uiRespondantPortrait.texture = respondantPortrait.texture;
        }

        if (uiRespondantName != null)
        {
            uiRespondantName.gameObject.SetActive(true);
            uiRespondantName.text = respondantName;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (uiRespondantPortrait != null)
            uiRespondantPortrait.gameObject.SetActive(false);

        if (uiRespondantName != null)
            uiRespondantName.gameObject.SetActive(false);
    }
}