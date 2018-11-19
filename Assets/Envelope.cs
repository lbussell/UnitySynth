using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Envelope : MonoBehaviour {

    public Oscillator oscillator;

    private float attack = 0.2f;
    private float decay;
    private float sustain;
    private float resonance;

    public double ComputeGain(double time, bool noteOn) {

        if (time <= attack && noteOn) {
            return time / attack;
        } else if (noteOn) {
            return 1;
        } else {
            return 0;
        }

        //    if (time > attack && time <= attack + decay) {
        //    return (decay - time) / (decay);
        //}
    }

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
        
    }
}
