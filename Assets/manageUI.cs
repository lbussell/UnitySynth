using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class manageUI : MonoBehaviour {

    public Dropdown synthDropdown;
    public Dropdown keyboardLayoutDropdown;
    public Slider volumeSlider;
    public Oscillator osc1;

    private List<string> waveforms = new List<string>() {
        "Sine",
        "Square",
        "Sawtooth"
    };

    private List<string> keyboardLayouts = new List<string>() {
        "Qwerty",
        "Colemak"
    };

    // Use this for initialization
    void Start () {

        PopulateLists();

        volumeSlider.onValueChanged.AddListener(delegate {
            VolumeSliderValueChanged(volumeSlider);
        });

        synthDropdown.onValueChanged.AddListener(delegate {
            SynthDropdownValueChanged(synthDropdown);
        });

        keyboardLayoutDropdown.onValueChanged.AddListener(delegate {
            KeyboardLayoutDropdownValueChanged(keyboardLayoutDropdown);
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void VolumeSliderValueChanged(Slider change) {
        osc1.ChangeVolume(change.value);
    }

    void SynthDropdownValueChanged(Dropdown change) {
        osc1.ChangeWaveform(waveforms[change.value]);
    }

    void KeyboardLayoutDropdownValueChanged(Dropdown change) {
        osc1.ChangeKeyboardLayout(keyboardLayouts[change.value]);
    }

    void PopulateLists() {
        synthDropdown.AddOptions(waveforms);
        keyboardLayoutDropdown.AddOptions(keyboardLayouts);
    }
}
