using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public AudioSource MainTrack;
    public AudioSource WinningTrack;
    public AudioSource SadTrack;
    public AudioSource AngryTrack;
    public AudioSource BoredomTrack;
    public string State;

    private float smoothTrans = 2.5f;
    private float abnormalStateTimer = 0.0f;
    private const float MAX_ABNORMAL_STATE_TIME = 4.0f;

    public void ChangeState(string newState)
    {
        State = newState;
        abnormalStateTimer = 0f;
    }
    // Use this for initialization
    void Start()
    {
        MainTrack.volume = 1f;
        SadTrack.volume = 0f;
        WinningTrack.volume = 0f;
        AngryTrack.volume = 0f;
        BoredomTrack.volume = 0f;

        MainTrack.Play();
        WinningTrack.Play();
        SadTrack.Play();
        AngryTrack.Play();
        BoredomTrack.Play();
}

    // Update is called once per frame
    void Update()
    {
        if(State.ToLower() == "normal")
        {
            MainTrack.volume = Mathf.Lerp(MainTrack.volume, 1.0f, smoothTrans * Time.deltaTime);
            AngryTrack.volume = Mathf.Lerp(AngryTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            SadTrack.volume = Mathf.Lerp(SadTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            WinningTrack.volume = Mathf.Lerp(WinningTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            BoredomTrack.volume = Mathf.Lerp(BoredomTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
        }
        if (State.ToLower() == "boredom")
        {
            MainTrack.volume = Mathf.Lerp(MainTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            AngryTrack.volume = Mathf.Lerp(AngryTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            SadTrack.volume = Mathf.Lerp(SadTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            WinningTrack.volume = Mathf.Lerp(WinningTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            BoredomTrack.volume = Mathf.Lerp(BoredomTrack.volume, 1.0f, smoothTrans * Time.deltaTime);
        }
        if (State.ToLower() == "sad")
        {
            MainTrack.volume = Mathf.Lerp(MainTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            BoredomTrack.volume = Mathf.Lerp(BoredomTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            AngryTrack.volume = Mathf.Lerp(AngryTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            WinningTrack.volume = Mathf.Lerp(WinningTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            SadTrack.volume = Mathf.Lerp(SadTrack.volume, 1.0f, smoothTrans * Time.deltaTime);
        }

        if (State.ToLower() == "angry")
        {
            MainTrack.volume = Mathf.Lerp(MainTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            SadTrack.volume = Mathf.Lerp(SadTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            BoredomTrack.volume = Mathf.Lerp(BoredomTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            WinningTrack.volume = Mathf.Lerp(WinningTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            AngryTrack.volume = Mathf.Lerp(AngryTrack.volume, 1.0f, smoothTrans * Time.deltaTime);
        }

        if (State.ToLower() == "winning")
        {
            MainTrack.volume = Mathf.Lerp(MainTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            SadTrack.volume = Mathf.Lerp(SadTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            BoredomTrack.volume = Mathf.Lerp(BoredomTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            AngryTrack.volume = Mathf.Lerp(AngryTrack.volume, 0.0f, smoothTrans * Time.deltaTime);
            WinningTrack.volume = Mathf.Lerp(WinningTrack.volume, 1.0f, smoothTrans * Time.deltaTime);
        }

        if(State != "normal")
        {
            abnormalStateTimer += Time.deltaTime;
            if(abnormalStateTimer > MAX_ABNORMAL_STATE_TIME)
            {
                abnormalStateTimer = 0.0f;
                State = "normal";
            }
        }

    }

}
