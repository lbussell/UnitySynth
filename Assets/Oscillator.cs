using System.Collections;
using System.Collections.Generic;
using MidiJack;
using UnityEngine;

public class Oscillator : MonoBehaviour
{

    private Envelope envelope;

    private static int SAMPLING_FREQUENCY = 44100;
    private double phaseIncrement;
    private double phase;
    private double amplitude;

    private double dspTimeStep;
    private double chunkTime;
    private double currentDspTime;

    private bool noteIsOn = false;
    private int numberOfNotesOn;
    private double triggerOnTime;
    private double triggerOffTime = 0;

    private double oscFrequency = 440.0;
    private double[] frequencies;
    private int octave = 4; // Octave 4 for A4

    private double attack = 0.5;
    private double decay = 0.2;
    private double sustain = 0;
    private double release = 0.1f;

    // Modifiable Parameters
    public string waveShape = "Sine";
    public string keyboardLayout = "Qwerty";

    public double volume = 0.1;

    // Oscillator
    void OnAudioFilterRead(float[] data, int channels)
    {
        currentDspTime = AudioSettings.dspTime;
        chunkTime = data.Length / channels / SAMPLING_FREQUENCY;   // the time that each chunk of data lasts
        dspTimeStep = chunkTime / data.Length;
        double preciseDspTime;
        // Oscillator 1
        for (int i = 0; i < data.Length; i += channels)
        {
            preciseDspTime = currentDspTime + i * dspTimeStep;

            phaseIncrement = oscFrequency * 2.0 * Mathf.PI / SAMPLING_FREQUENCY;
            phase += phaseIncrement;

            amplitude = ComputeAmplitude(preciseDspTime) * volume;

            if (phase > (Mathf.PI * 2)) phase = phase % (Mathf.PI * 2);

            switch (waveShape)
            {
                case "Sine":
                    data[i] += (float)(amplitude * Mathf.Sin((float)phase));
                    break;
                case "Sawtooth":
                    double saw = (amplitude * phase) - (amplitude / 2);
                    data[i] += (float)saw;
                    break;
                case "Square":
                    if (phase > Mathf.PI) data[i] = 
                        (float) (amplitude - (amplitude / 2));
                    else data[i] += (float) -(amplitude / 2);
                    break;
            }

            // stereo audio
            if (channels == 2) data[i + 1] = data[i];
        }

    }

    // Start is called at the beginning
    void Start()
    {
        frequencies = GenerateFrequencies();
    }

    // Update is called once per frame
    void Update()
    {
        //if (keyboardLayout.Equals("Colemak")) checkColemakInput();
        //else if (keyboardLayout.Equals("Qwerty")) checkQwertyInput();
        // amplitude = ComputeAmplitude(Time.time) * volume;
        attack = MidiMaster.GetKnob(21, 0);
        decay = MidiMaster.GetKnob(22, 0);
        sustain = MidiMaster.GetKnob(23, 0);
        release = MidiMaster.GetKnob(24, 0);
    }

    double ComputeAmplitude(double preciseDspTime)
    {
        // return 1;
        double dTime = preciseDspTime - triggerOnTime;
        // double dTimeOff = preciseDspTime - triggerOffTime;

        double amplitude = 0;

        if (numberOfNotesOn > 0)
        {
            if (dTime <= attack && attack > 0)
            {
                amplitude =  dTime / attack;
            }
            if (dTime > attack && dTime <= attack + decay) 
            {
                amplitude =  ((dTime - attack) / decay) * (sustain - 1) + 1;
            }
            if (dTime > attack + decay)
            {
                amplitude = sustain;
            }
        }
        else
        {
            amplitude = ((preciseDspTime - triggerOffTime) / release) * (0.0 - sustain) + sustain;
        }

        if (amplitude <= 0.0001)
            amplitude = 0;

        return amplitude;

        //else
        //{
        //    // Note has been released, so in release phase
        //    dAmplitude = ((dTime - dTriggerOffTime) / dReleaseTime) * (0.0 - dSustainAmplitude) + dSustainAmplitude;
        //}

        //// Amplitude should not be negative
        //if (dAmplitude <= 0.0001)
            //dAmplitude = 0.0;
    }

    void NoteOn(MidiChannel channel, int note, float velocity)
    {
        Debug.Log("NoteOn: " + channel + "," + note + "," + velocity);
        oscFrequency = frequencies[note];
        numberOfNotesOn++;
        triggerOnTime = AudioSettings.dspTime;
    }

    void NoteOff(MidiChannel channel, int note)
    {
        Debug.Log("NoteOff: " + channel + "," + note);
        numberOfNotesOn--;
        triggerOffTime = AudioSettings.dspTime;
    }

    void Knob(MidiChannel channel, int knobNumber, float knobValue)
    {
        Debug.Log("Knob: " + knobNumber + "," + knobValue);
    }

    public void ChangeVolume(float value) {
        this.volume = value;
    }

    public void ChangeWaveform(string newWaveshape) {
        this.waveShape = newWaveshape;
    }

    public void ChangeKeyboardLayout(string newLayout) {
        this.keyboardLayout = newLayout;
    }

    //void checkColemakInput() {

    //    if (!Input.anyKey)
    //    {
    //        amplitude = 0;
    //    }

    //    if (Input.GetKeyDown(KeyCode.LeftArrow))
    //    {
    //        if (octave > 0) octave--;
    //    }
    //    else if (Input.GetKeyDown(KeyCode.RightArrow))
    //    {
    //        if (octave < 6) octave++;
    //    }

    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 0];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 1];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.X))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 2];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.C))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 3];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.T))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 4];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.V))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 5];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.D))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 6];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.B))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 7];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 8];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.N))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 9];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.M))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 10];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 11];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Comma))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 12];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 13];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Period))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 14];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Slash))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 15];
    //    }
    //}

    //void checkQwertyInput()
    //{

    //    if (!Input.anyKey)
    //    {
    //        amplitude = 0;
    //    }

    //    if (Input.GetKeyDown(KeyCode.LeftArrow))
    //    {
    //        if (octave > 0) octave--;
    //    }
    //    else if (Input.GetKeyDown(KeyCode.RightArrow))
    //    {
    //        if (octave < 6) octave++;
    //    }

    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 0];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 1];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.X))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 2];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.C))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 3];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 4];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.V))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 5];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.G))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 6];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.B))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 7];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.N))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 8];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.J))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 9];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.M))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 10];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 11];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Comma))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 12];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 13];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Period))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 14];
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Slash))
    //    {
    //        amplitude = volume;
    //        oscFrequency = frequencies[octave * 12 + 15];
    //    }
    //}

    // Generates frequencies of notes. Only called once, in Start().

    double[] GenerateFrequencies()
    {

        double[] freqs = new double[127];
        double fA4 = 432.0;

        for (int i = 0; i < freqs.Length; i++)
        {

            // (-69 + i) is the distance away from A4.
            // Starting at midi note 0 means we are -69 notes away from A4
            double ratio = Mathf.Pow(2f, (-69f + i) / 12f);
            freqs[i] = fA4 * ratio;
        }
        return freqs;
    }

    void OnEnable()
    {
        MidiMaster.noteOnDelegate += NoteOn;
        MidiMaster.noteOffDelegate += NoteOff;
        MidiMaster.knobDelegate += Knob;
    }

    void OnDisable()
    {
        MidiMaster.noteOnDelegate -= NoteOn;
        MidiMaster.noteOffDelegate -= NoteOff;
        MidiMaster.knobDelegate -= Knob;
    }

}