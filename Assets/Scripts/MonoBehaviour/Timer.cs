using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private float maxRoundTime;

    #endregion

    private float _timeLeft;
    private TMP_Text _timerText;

    public bool TimeUp { get; private set; }

    #region Event Functions

    private void Awake()
    {
        _timerText = GetComponentInChildren<TMP_Text>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _timeLeft = maxRoundTime;
        TimeUp = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (TimeUp) return;

        _timeLeft -= Time.deltaTime;
        _timerText.text = _timeLeft.ToString("00.00");

        if (_timeLeft > 5) return;
        _timerText.color = new Color(1, .5f, .5f);

        if (_timeLeft > 0) return;
        TimeUp = true;
        EndRun();
    }

    #endregion

    private void EndRun()
    {
        _timerText.text = 0.ToString("00.00");
        Deck.Hand.BossFight();
    }
}