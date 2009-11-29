﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Midi
{
    /// <summary>
    /// General MIDI percussion notes.
    /// </summary>
    /// In General MIDI, notes played on channel 10 (channel 9 in code) make the following percussion sounds,
    /// regardless of any ProgramChange messages on that channel.
    public enum Percussion
    {
        /// <summary>General MIDI percussion 35 ("Bass Drum 2").</summary>
        BassDrum2 = 35,
        /// <summary>General MIDI percussion 36 ("Bass Drum 1").</summary>
        BassDrum1 = 36,
        /// <summary>General MIDI percussion 37 ("Side Stick").</summary>
        SideStick = 37,
        /// <summary>General MIDI percussion 38 ("Snare Drum 1").</summary>
        SnareDrum1 = 38,
        /// <summary>General MIDI percussion 39 ("Hand Clap").</summary>
        HandClap = 39,
        /// <summary>General MIDI percussion 40 ("Snare Drum 2").</summary>
        SnareDrum2 = 40,
        /// <summary>General MIDI percussion 41 ("Low Tom 2").</summary>
        LowTom2 = 41,
        /// <summary>General MIDI percussion 42 ("Closed Hi-hat").</summary>
        ClosedHiHat = 42,
        /// <summary>General MIDI percussion 43 ("Low Tom 1").</summary>
        LowTom1 = 43,
        /// <summary>General MIDI percussion 44 ("Pedal Hi-hat").</summary>
        PedalHiHat = 44,
        /// <summary>General MIDI percussion 45 ("Mid Tom 2").</summary>
        MidTom2 = 45,
        /// <summary>General MIDI percussion 46 ("Open Hi-hat").</summary>
        OpenHiHat = 46,
        /// <summary>General MIDI percussion 47 ("Mid Tom 1").</summary>
        MidTom1 = 47,
        /// <summary>General MIDI percussion 48 ("High Tom 2").</summary>
        HighTom2 = 48,
        /// <summary>General MIDI percussion 49 ("Crash Cymbal 1").</summary>
        CrashCymbal1 = 49,
        /// <summary>General MIDI percussion 50 ("High Tom 1").</summary>
        HighTom1 = 50,
        /// <summary>General MIDI percussion 51 ("Ride Cymbal 1").</summary>
        RideCymbal1 = 51,
        /// <summary>General MIDI percussion 52 ("Chinese Cymbal").</summary>
        ChineseCymbal = 52,
        /// <summary>General MIDI percussion 53 ("Ride Bell").</summary>
        RideBell = 53,
        /// <summary>General MIDI percussion 54 ("Tambourine").</summary>
        Tambourine = 54,
        /// <summary>General MIDI percussion 55 ("Splash Cymbal").</summary>
        SplashCymbal = 55,
        /// <summary>General MIDI percussion 56 ("Cowbell").</summary>
        Cowbell = 56,
        /// <summary>General MIDI percussion 57 ("Crash Cymbal 2").</summary>
        CrashCymbal2 = 57,
        /// <summary>General MIDI percussion 58 ("Vibra Slap").</summary>
        VibraSlap = 58,
        /// <summary>General MIDI percussion 59 ("Ride Cymbal 2").</summary>
        RideCymbal2 = 59,
        /// <summary>General MIDI percussion 60 ("High Bongo").</summary>
        HighBongo = 60,
        /// <summary>General MIDI percussion 61 ("Low Bongo").</summary>
        LowBongo = 61,
        /// <summary>General MIDI percussion 62 ("Mute High Conga").</summary>
        MuteHighConga = 62,
        /// <summary>General MIDI percussion 63 ("Open High Conga").</summary>
        OpenHighConga = 63,
        /// <summary>General MIDI percussion 64 ("Low Conga").</summary>
        LowConga = 64,
        /// <summary>General MIDI percussion 65 ("High Timbale").</summary>
        HighTimbale = 65,
        /// <summary>General MIDI percussion 66 ("Low Timbale").</summary>
        LowTimbale = 66,
        /// <summary>General MIDI percussion 67 ("High Agogo").</summary>
        HighAgogo = 67,
        /// <summary>General MIDI percussion 68 ("Low Agogo").</summary>
        LowAgogo = 68,
        /// <summary>General MIDI percussion 69 ("Cabasa").</summary>
        Cabasa = 69,
        /// <summary>General MIDI percussion 70 ("Maracas").</summary>
        Maracas = 70,
        /// <summary>General MIDI percussion 71 ("Short Whistle").</summary>
        ShortWhistle = 71,
        /// <summary>General MIDI percussion 72 ("Long Whistle").</summary>
        LongWhistle = 72,
        /// <summary>General MIDI percussion 73 ("Short Guiro").</summary>
        ShortGuiro = 74,
        /// <summary>General MIDI percussion 74 ("Long Guiro").</summary>
        LongGuiro = 74,
        /// <summary>General MIDI percussion 75 ("Claves").</summary>
        Claves = 75,
        /// <summary>General MIDI percussion 76 ("High Wood Block").</summary>
        HighWoodBlock = 76,
        /// <summary>General MIDI percussion 77 ("Low Wood Block").</summary>
        LowWoodBlock = 77,
        /// <summary>General MIDI percussion 78 ("Mute Cuica").</summary>
        MuteCuica = 78,
        /// <summary>General MIDI percussion 79 ("Open Cuica").</summary>
        OpenCuica = 79,
        /// <summary>General MIDI percussion 80 ("Mute Triangle").</summary>
        MuteTriangle = 80,
        /// <summary>General MIDI percussion 81 ("Open Triangle").</summary>
        OpenTriangle = 81
    };

    /// <summary>
    /// Extension methods for the Percussion enum.
    /// </summary>
    public static class PercussionExtensionMethods
    {
        /// <summary>
        /// Returns true if the specified percussion is valid.
        /// </summary>
        /// <param name="percussion">The percussion to test.</param>
        public static bool IsValid(this Percussion percussion)
        {
            return (int)percussion >= 35 && (int)percussion < 81;
        }

        /// <summary>
        /// Throws an exception if percussion is not valid.
        /// </summary>
        /// <param name="percussion">The percussion to validate.</param>
        public static void Validate(this Percussion percussion)
        {
            if (!percussion.IsValid())
            {
                throw new Exception("invalid Percussion");
            }
        }

        private static string[] PercussionNames = new string[]
        {
            "Bass Drum 2",
            "Bass Drum 1",
            "Side Stick",
            "Snare Drum 1",
            "Hand Clap",
            "Snare Drum 2",
            "Low Tom 2",
            "Closed Hi-hat",
            "Low Tom 1",
            "Pedal Hi-hat",
            "Mid Tom 2",
            "Open Hi-hat",
            "Mid Tom 1",
            "High Tom 2",
            "Crash Cymbal 1",
            "High Tom 1",
            "Ride Cymbal 1",
            "Chinese Cymbal",
            "Ride Bell",
            "Tambourine",
            "Splash Cymbal",
            "Cowbell",
            "Crash Cymbal 2",
            "Vibra Slap",
            "Ride Cymbal 2",
            "High Bongo",
            "Low Bongo",
            "Mute High Conga",
            "Open High Conga",
            "Low Conga",
            "High Timbale",
            "Low Timbale",
            "High Agogo",
            "Low Agogo",
            "Cabasa",
            "Maracas",
            "Short Whistle",
            "Long Whistle",
            "Short Guiro",
            "Long Guiro",
            "Claves",
            "High Wood Block",
            "Low Wood Block",
            "Mute Cuica",
            "Open Cuica",
            "Mute Triangle",
            "Open Triangle"
        };

        /// <summary>
        /// Returns the human-readable name of a MIDI percussion.
        /// </summary>
        /// <param name="percussion">The percussion.</param>
        public static string Name(this Percussion percussion)
        {
            if (!percussion.IsValid())
            {
                throw new InvalidOperationException("invalid Percussion");
            }
            return PercussionNames[(int)percussion];
        }
    }
}