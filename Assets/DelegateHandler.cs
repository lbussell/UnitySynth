using System.Collections;
using System.Collections.Generic;
using MidiJack;
using UnityEngine;

public class DelegateHandler : MonoBehaviour {

    //public Oscillator oscillator;
    //public Envelope envelope;

    //private int numberOfNotesOn;
    //private double increment;
    //private bool noteIsOn;

    //private void Update() // runs every frame
    //{
    //    noteIsOn = numberOfNotesOn > 0;
    //    if (noteIsOn) increment += 0.001f;
    //    oscillator.ChangeGain(envelope.ComputeGain(increment, noteIsOn));
    //}

    //void NoteOn(MidiChannel channel, int note, float velocity)
    //{
    //    Debug.Log("NoteOn: " + channel + "," + note + "," + velocity);
    //    oscillator.ChangeNote(note);
    //    increment = 0;
    //    numberOfNotesOn++;
    //}

    //void NoteOff(MidiChannel channel, int note)
    //{
    //    Debug.Log("NoteOff: " + channel + "," + note);
    //    numberOfNotesOn--;
    //}

    //void Knob(MidiChannel channel, int knobNumber, float knobValue)
    //{
    //    Debug.Log("Knob: " + knobNumber + "," + knobValue);
    //}
}
