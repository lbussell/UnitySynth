using System.Collections;
using System.Collections.Generic;
using MidiJack;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    // For GUI purposes
    public KnobController knobController;

    // Samplerate information
    private static int _FS;
    private static double _sampleDuration;

    // For oscillation
    private double phaseIncrement;
    private double phase;

    // For envelope control
    private double envelope;
    private double currentDspTime;
    private int numberOfNotesOn;
    private double triggerOnTime;
    private double triggerOffTime;

    // For single-pole low pass filter
    private float previousOutput;
    private float nextOutput;

    private double oscFrequency = 440.0;
    private double[] frequencies;

    // Envelope controls
    // The [Range] parameter is only for control in the Unity editor.
    [Range(0f,1f)]
    public double attack;
    [Range(0f, 1f)]
    public double decay;
    [Range(0f, 1f)]
    public double sustain = 1;
    [Range(0f, 1f)]
    public double release = 0.1f;
    [Range(0f, 1f)]
    public double lowPass;

    // Waveshape weights
    [Range(0f, 1f)]
    public double sinWeight = 0;
    [Range(0f, 1f)]
    public double sqrWeight = 0;
    [Range(0f, 1f)]
    public double sawWeight = 1;

    public double volume = 0.1;

    // Oscillator
    void OnAudioFilterRead(float[] data, int channels)
    {
        /* 
         * This is "the current time of the audio system", as given
         * by Unity. It is updated every time the OnAudioFilterRead() function
         * is called. It's usually every 1024 samples.
         * 
         * A note on the sample rate:
         * We don't actually see real numbers for the sample rate, we instead
         * read it from the system in the Start() function.
         */
        currentDspTime = AudioSettings.dspTime;

        for (int i = 0; i < data.Length; i += channels)
        {
            /*
             * Sample duration is just 1/fs. Because dspTime isn't updated every
             * sample, we "update" it ourselves so our envelope sounds smooth.
             */
            currentDspTime += _sampleDuration;
            envelope = ComputeAmplitude(currentDspTime) * volume;

            /*
             * The phase variable below increments by the calculated amount for
             * every audio sample. We can then use this value to calculate what
             * each waveform's value should be.
             * 
             *             2pi * f
             *     phase = -------
             *               fs
             * 
             * When phase is greater than 2pi, we just reset
             * it so we don't have an ever-increasing variable that will cause
             * an overflow error.
             */
            phaseIncrement = oscFrequency * 2.0 * Mathf.PI / _FS;
            phase += phaseIncrement;
            if (phase > (Mathf.PI * 2)){
                phase = phase % (Mathf.PI * 2);
            }

            nextOutput = 0;

            // Adds sinusoidal wave to the next output sample
            nextOutput += (float)(sinWeight * envelope * Mathf.Sin((float)phase));

            // Adds sawtooth wave to the next output sample
            nextOutput += (float)(sawWeight * ((envelope * phase) / (2 * Mathf.PI)) - (envelope / 2));

            // Adds square wave to the next output sample
            if (phase > Mathf.PI) {
                nextOutput += (float)(sqrWeight*(envelope - (envelope / 2)));
            } else {
                nextOutput += (float)((sqrWeight)*-(envelope / 2));
            }

            /*
             * Here we apply a single-pole low-pass filter. Even if the filter
             * is completely open (lowPass = 1) we still compute this step so we
             * don't have to have any conditional logic.
             */
            nextOutput = (float)((nextOutput * lowPass) + (previousOutput * (1 - lowPass)));

            // Write the output to the audio filter
            data[i] += nextOutput;

            // This is for the next low-pass calculation
            previousOutput = nextOutput;

            // Copy the sound from one channel into the next channel for stereo
            if (channels == 2) data[i + 1] = data[i];
        }
    }

    /*
     * Called at the beginning of the program
     */
    void Start()
    {
        // Grab the sample rate from the system
        _FS = AudioSettings.outputSampleRate;
        // Calculate how long each sample lasts
        _sampleDuration = 1.0 / _FS;
        // Generate frequencies corresponding to all MIDI values
        frequencies = GenerateFrequencies();
    }

    /*
     * Update is called once per frame (usually 60 fps)
     */
    void Update()
    {
        // For testing purposes
        CheckSingleKeyboardInput();
    }

    /*
     * Computes the amplitude, or the envelope, given the current time of the
     * audio system (the precise one that we calculated).
     */
    double ComputeAmplitude(double preciseDspTime)
    {
        /*
         * Compute the time since we last pressed a note
         */
        double dTime = preciseDspTime - triggerOnTime;

        double amplitude = 0;

        if (numberOfNotesOn > 0)
        {
            if (dTime <= attack && attack > 0)
            { // Time since note press is less than attack
                amplitude = dTime / attack;
            }

            if (dTime > attack && dTime <= attack + decay) 
            { // Done with attack phase, so calculate decay
                amplitude = ((dTime - attack) / decay) * (sustain - 1) + 1;
            }

            if (dTime > attack + decay)
            { // Done with attack and decay, so sustain
                amplitude = sustain;
            }
        }
        else if (release == 0) 
        { // Prevent dividing by 0 in the event that release is 0s
            amplitude = 0;
        }
        else
        { // Calculate release. This only happens when no notes are on.
            amplitude = ((preciseDspTime - triggerOffTime) / release) * (0.0 - sustain) + sustain;
        }

        // Make sure amplitude actually reaches 0
        if (amplitude <= 0.0001)
            amplitude = 0;

        return amplitude;
    }

    /*
     * This method is called whenever a note is pressed, thanks to a delegate
     * provided by the MIDI support frameworks that we are using
     */
    void NoteOn(MidiChannel channel, int note, float velocity)
    {
        // This is only for debugging purposes
        Debug.Log("NoteOn: " + channel + "," + note + "," + velocity);

        /*
         * Some MIDI keyboards report note on signals and note down signals.
         * Other MIDI devices report note on signals and then another note on
         * signal with a velocity of 0 as a "note off" signal.
         * 
         * This check prevents this from registering as an additional note press
         * since we don't care about velocity sensitivity.
         */
        if (velocity > 0) {
            // Read the proper frequency from the frequency table we generated
            oscFrequency = frequencies[note];
            // Record that a note was pressed
            numberOfNotesOn++;
            // Set the trigger on time for amplitude calculation
            triggerOnTime = AudioSettings.dspTime;
        }
    }

    /*
     * This method is called whenever a note is released. It's basically the
     * opposite of NoteOn.
     */
    void NoteOff(MidiChannel channel, int note)
    {
        Debug.Log("NoteOff: " + channel + "," + note);
        numberOfNotesOn--;
        triggerOffTime = AudioSettings.dspTime;
    }

    /*
     * This note is called whenever any MIDI knob is changed.
     * Unfortunately we had to hard code support for specific MIDI controllers.
     * The numbers on the left are for the Novation Launchkey Mini MKII
     * The numbers on the right are for the M-Audio Oxygen 61 (the old one)
     */
    void Knob(MidiChannel channel, int knobNumber, float knobValue)
    {
        Debug.Log("Knob: " + knobNumber + "," + knobValue);

        // Envelope Controls
        if (knobNumber == 21 || knobNumber == 74) 
        {
            attack = knobValue;
            // This line is for updating the GUI
            knobController.ChangeSliderPosition("a", knobValue);
        } 
        else if (knobNumber == 22 || knobNumber == 71) 
        {
            decay = knobValue;
            knobController.ChangeSliderPosition("d", knobValue);
        } 
        else if (knobNumber == 23 || knobNumber == 91)
        {
            sustain = knobValue;
            knobController.ChangeSliderPosition("s", knobValue);
        }
        else if (knobNumber == 24 || knobNumber == 93)
        {
            release = knobValue;
            knobController.ChangeSliderPosition("r", knobValue);
        }

        // Filter controls
        else if (knobNumber == 25 || knobNumber == 75) 
        {
            lowPass = knobValue;
            knobController.ChangeKnobRotation("lpf", knobValue);
        }

        // Waveshape weightings
        else if (knobNumber == 26 || knobNumber == 73) 
        {
            sinWeight = knobValue;
            knobController.ChangeKnobRotation("sin", knobValue);
        } 
        else if (knobNumber == 27 || knobNumber == 72)
        {
            sawWeight = knobValue;
            knobController.ChangeKnobRotation("saw", knobValue);
        } 
        else if (knobNumber == 28 || knobNumber == 5) 
        {
            sqrWeight = knobValue;
            knobController.ChangeKnobRotation("sqr", knobValue);
        }
    }

    /*
     * This method simulates NoteOn and NoteOff, but only looks at the 'A' key
     * on the computer keyboard. This is for testing sound without MIDI input.
     */
    void CheckSingleKeyboardInput() {
        if (Input.GetKeyDown(KeyCode.A)) {
            oscFrequency = frequencies[45];
            numberOfNotesOn++;
            triggerOnTime = AudioSettings.dspTime;
        } else if (Input.GetKeyUp(KeyCode.A)) {
            numberOfNotesOn--;
            triggerOffTime = AudioSettings.dspTime;
        }
    }

    /*
     * Generates frequencies corresponding to MIDI values 0 to 127.
     */
    double[] GenerateFrequencies()
    {

        double[] freqs = new double[127];
        double fA4 = 440.0;

        for (int i = 0; i < freqs.Length; i++)
        {
            /*
             * (-69 + i) is the distance away from A4.
             * Starting at midi note 0 means we are -69 notes away from A4
             */
            double ratio = Mathf.Pow(2f, (-69f + i) / 12f);
            freqs[i] = fA4 * ratio;
        }

        return freqs;
    }

    /*
     * Called when the object is created
     */
    void OnEnable()
    {
        /*
         * These delegates are how MIDI input is handled. We assign a method
         * (NoteOn, NoteOff, etc) to a specific delegate provided to us by the
         * MIDI support library that we use.
         */
        MidiMaster.noteOnDelegate += NoteOn;
        MidiMaster.noteOffDelegate += NoteOff;
        MidiMaster.knobDelegate += Knob;
    }

    /*
     * Called when the object is deleted (this will never happen, but it's
     * good practice)
     */
    void OnDisable()
    {
        MidiMaster.noteOnDelegate -= NoteOn;
        MidiMaster.noteOffDelegate -= NoteOff;
        MidiMaster.knobDelegate -= Knob;
    }
}