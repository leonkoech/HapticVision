// Copyright (C) 2019-2020 by HaptX Incorporated - All Rights Reserved.
// Unauthorized copying of this file via any medium is strictly prohibited.
// The contents of this file are proprietary and confidential.

using UnityEngine;

//! @brief An implementation of HxDirectEffect that defines the Haptic Effect using a simple sine
//! wave generator. The waveform starts at 0 then increases by default.
//!
//! @ingroup group_unity_plugin
public class HxWaveDirectEffect : HxDirectEffect {

  //! @brief The frequency that the signal repeats itself at [Hz].
  //!
  //! Changing this while the effect is playing can cause discontinuities.
  [Tooltip("The frequency that the signal repeats itself at [Hz]. Changing this while the effect is playing can cause discontinuities."),
      SerializeField, Range(0.0f, float.MaxValue)]
  public float frequencyHz = 10.0f;

  //! The amplitude of the wave signal [M].
  [Tooltip("The amplitude of the wave signal [M]."), SerializeField, Range(0.0f, float.MaxValue)]
  public float amplitudeM = 0.005f;

  //! Whether to invert (negate) the oscillation from the wave signal BEFORE adding it to
  //! #outputDisplacementOffsetM.
  [Tooltip("Whether to invert (negate) the oscillation from the wave signal BEFORE adding it to the output displacement offset."),
      SerializeField]
  public bool invertOscillation = false;

  //! @brief The phase that the wave starts at [deg], then continues from there.
  //!
  //! For example: an offset of 90 degrees turns a sine wave into a cosine wave.
  [Tooltip("The phase that the wave starts at [deg], then continues from there."), SerializeField,
      Range(0.0f, float.MaxValue)]
  public float inputPhaseOffsetDeg = 0.0f;

  //! A displacement amount [M] to always add to the output (essentialy a DC offset for the
  //! signal).
  [Tooltip("A displacement amount [M] to always add to the output (essentialy a DC offset for the signal)."),
      SerializeField]
  public float outputDisplacementOffsetM = 0.0f;

  //! The duration [s] with which to play the sine wave if #_isLooping is false.
  [Tooltip("The duration [s] with which to play the sine wave if \"Is Looping\" is false."),
      SerializeField, Range(0.0f, float.MaxValue)]
  protected float durationS = 1.0f;

  //! Default constructor.
  public HxWaveDirectEffect() : base() {
    _isLooping = true;
  }

  //! Awake is called when the script instance is being loaded.
  new protected void Awake() {
    base.Awake();
    UpdateDuration();
  }

  //! Differs from parent by outputting the output from our sine wave generator.
  protected override float GetDisplacementM(HaptxApi.DirectEffect.DirectInfo directInfo) {
    float waveOutputM = HxShared.EvaluateSineWave(directInfo.time_s, frequencyHz,
        inputPhaseOffsetDeg);
    waveOutputM *= amplitudeM;
    if (invertOscillation) {
      waveOutputM *= -1.0f;
    }
    waveOutputM += outputDisplacementOffsetM;

    return waveOutputM;
  }

  //! Updates duration internally.
  private void UpdateDuration() {
    // If we can actually set the duration
    if (EffectInternal != null) {
      float duration = 0.0f;
      // If we can actually play the effect
      if (frequencyHz > 0.0f) {
        // If we intend for the effect to loop cleanly
        if (_isLooping) {
          duration = 1 / frequencyHz;
        }
        // If we intend to use duration_s_ and only play the effect once
        else if (durationS > 0.0f) {
          duration = durationS;
        }
      }
      EffectInternal.setDurationS(duration);
    }
  }
};