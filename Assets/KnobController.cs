using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnobController : MonoBehaviour {

    public GameObject sinKnob;
    public GameObject sawKnob;
    public GameObject sqrKnob;
    public GameObject lpfKnob;
    public GameObject aSlider;
    public GameObject dSlider;
    public GameObject sSlider;
    public GameObject rSlider;
    private Quaternion quaternion;

	// Use this for initialization
	void Start () 
    {
        quaternion = new Quaternion();
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    public void ChangeKnobRotation(string knob, float value) 
    {
        if (knob.Equals("sin")) 
        {
            sinKnob.transform.localEulerAngles = new Vector3(0, CalculateRotation(value), 0);
        } 
        else if (knob.Equals("saw")) 
        {
            sawKnob.transform.localEulerAngles = new Vector3(0, CalculateRotation(value), 0);
        } 
        else if (knob.Equals("sqr")) 
        {
            sqrKnob.transform.localEulerAngles = new Vector3(0, CalculateRotation(value), 0);
        } 
        else if (knob.Equals("lpf")) 
        {
            lpfKnob.transform.localEulerAngles = new Vector3(0, CalculateRotation(value), 0);
        }
    }

    public void ChangeSliderPosition(string param, float value) {
        if (param.Equals("a"))
        {
            aSlider.transform.localPosition = new Vector3(0, 0, CalculatePosition(value));
        }
        else if (param.Equals("d"))
        {
            dSlider.transform.localPosition = new Vector3(0, 0, CalculatePosition(value));
        }
        else if (param.Equals("s"))
        {
            sSlider.transform.localPosition = new Vector3(0, 0, CalculatePosition(value));
        }
        else if (param.Equals("r"))
        {
            rSlider.transform.localPosition = new Vector3(0, 0, CalculatePosition(value));
        }
    }

    float CalculateRotation(float value) 
    {
        return value * 300 + 30;
    }

    float CalculatePosition(float value)
    {
        return value * 40;
    }
}