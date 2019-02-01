//*****************************************************************************
//Created By ZJ on 2018��6��7��.
//
//@Description ֡�ʲ鿴
//*****************************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvgFps : MonoBehaviour
{
    private List<float> _fpsList;

    private float _lastUpdateTime; //�ϴθ���ʱ��

    private const float _updateDelta = 1; //����ʱ������
    private const int recordCount = 50; //ƽ��ֵͳ������

    private int _frame = 0;//ͳ���������ۼ�֡��
    private float _currFps = 0;//��ǰ֡��

    // Use this for initialization
    void Start () {
	    _fpsList = new List<float>();
	    _lastUpdateTime = Time.realtimeSinceStartup;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    _frame++;
	    if (Time.realtimeSinceStartup - _lastUpdateTime >= _updateDelta)
	    {
	        _currFps = _frame / (Time.realtimeSinceStartup - _lastUpdateTime);

            if (_fpsList.Count < recordCount)
	        {
                _fpsList.Add(_currFps);
	        }
	        else
	        {
                _fpsList.RemoveAt(0);
	            _fpsList.Add(_currFps);
            }

	        _frame = 0;
	        _lastUpdateTime = Time.realtimeSinceStartup;

	    }

	}
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        style.normal.textColor = Color.white;
        GUI.Box(new Rect(0, 0, 280, 100), "");
        if (_fpsList.Count <= 0)
        {
            GUI.Label(new Rect(0, 0, 100, 100), "FPS: " + "0", style);

        }
        else
        {
            GUI.Label(new Rect(0, 0, 100, 100), "CurFPS: " + _currFps.ToString("#0.00"), style);
            GUI.Label(new Rect(0, 50, 100, 100), "AvgFPS: " + Mathf.Round(_fpsList.Sum() / _fpsList.Count), style);
        }
       
    }
}
