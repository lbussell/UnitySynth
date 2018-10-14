using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour {

    private static double SAMPLING_FREQUENCY = 48000;
    private double increment;
    private double phase;
    private float gain;

    private double oscFreqency = 440.0;
    private double[] frequencies;
    private int octave = 4; // Octave 4 for A4

    // Modifiable Parameters
    public string waveShape = "Sine";
    public string keyboardLayout = "Qwerty";

    public float volume = 0.1f;
    public float attack;
    public float decay;
    public float sustain;
    public float resonance;

    // Oscillator
    void OnAudioFilterRead(float[] data, int channels) {

        // Oscillator 1
        for (int i = 0; i < data.Length; i += channels) {

            increment = oscFreqency * 2.0 * Mathf.PI / SAMPLING_FREQUENCY;
            phase += increment;

            if (phase > (Mathf.PI * 2)) phase = phase % (Mathf.PI * 2);

            if (waveShape.Equals("Sine")) 
            {
                data[i] = (float)(gain * Mathf.Sin((float)phase));
            } 
            else if (waveShape.Equals("Sawtooth")) 
            {
                double saw = (gain * phase) - (gain / 2);
                data[i] = (float) saw;
            } 
            else if (waveShape.Equals("Square"))
            {
                if (phase > Mathf.PI) data[i] = (gain) - (gain / 2);
                else data[i] = -(gain / 2);
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
        if (keyboardLayout.Equals("Colemak")) checkColemakInput();
        else if (keyboardLayout.Equals("Qwerty")) checkQwertyInput();
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

    // Checks for Colemak keyboard input
    void checkColemakInput() {

        if (!Input.anyKey)
        {
            gain = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (octave > 0) octave--;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (octave < 6) octave++;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 0];
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 1];
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 2];
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 3];
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 4];
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 5];
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 6];
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 7];
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 8];
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 9];
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 10];
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 11];
        }
        else if (Input.GetKeyDown(KeyCode.Comma))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 12];
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 13];
        }
        else if (Input.GetKeyDown(KeyCode.Period))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 14];
        }
        else if (Input.GetKeyDown(KeyCode.Slash))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 15];
        }
    }

    void checkQwertyInput()
    {

        if (!Input.anyKey)
        {
            gain = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (octave > 0) octave--;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (octave < 6) octave++;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 0];
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 1];
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 2];
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 3];
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 4];
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 5];
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 6];
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 7];
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 8];
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 9];
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 10];
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 11];
        }
        else if (Input.GetKeyDown(KeyCode.Comma))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 12];
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 13];
        }
        else if (Input.GetKeyDown(KeyCode.Period))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 14];
        }
        else if (Input.GetKeyDown(KeyCode.Slash))
        {
            gain = volume;
            oscFreqency = frequencies[octave * 12 + 15];
        }
    }

    // Generates frequencies of notes. Only called once, in Start().
    double[] GenerateFrequencies()
    {

        double[] freqs = new double[88];
        double fA4 = 440.0;

        for (int i = 0; i < freqs.Length; i++)
        {

            // (-48 + i) is the distance away from A4.
            // Starting at A0 means we are -48 notes away from A4 (0)
            double ratio = Mathf.Pow(2f, (-48f + i) / 12f);
            freqs[i] = fA4 * ratio;
        }

        return freqs;
    }

}
