﻿// Copyright (c) 2009, Tom Lokovic
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Midi
{
    /// <summary>
    /// C# wrappers for the Win32 MIDI API.
    /// </summary>
    /// Because .NET does not provide MIDI support itself, in C# we must use P/Invoke to wrap the Win32
    /// API.  That API consists of the MMSystem.h C header and the winmm.dll library.  The API is described
    /// in detail here: http://msdn.microsoft.com/en-us/library/ms712733(VS.85).aspx.  The P/Invoke
    /// interop mechanism is described here: http://msdn.microsoft.com/en-us/library/aa288468(VS.71).aspx.
    ///
    /// This file covers the subset of the MIDI protocol needed to manage input and output devices and send
    /// and receive Note On/Off, Control Change, Pitch Bend and Program Change messages.  Other portions of
    /// the MIDI protocol (such as sysex events) are supported in the Win32 API but are not wrapped here.
    ///
    /// Some of the C functions are not typesafe when wrapped, so those wrappers are made private and typesafe
    /// variants are provided.
    class MidiWin32Wrapper
    {
        #region Constants

        // The following constants come from MMSystem.h.

        // General constants.
        public const UInt32 MAXPNAMELEN = 32; // Max length of a manufacturer name.

        // General return codes.
        public const UInt32 MMSYSERR_BASE         = 0;
        public const UInt32 MMSYSERR_NOERROR      = MMSYSERR_BASE + 0;
        public const UInt32 MMSYSERR_ERROR        = MMSYSERR_BASE + 1;
        public const UInt32 MMSYSERR_BADDEVICEID  = MMSYSERR_BASE + 2;
        public const UInt32 MMSYSERR_NOTENABLED   = MMSYSERR_BASE + 3;
        public const UInt32 MMSYSERR_ALLOCATED    = MMSYSERR_BASE + 4;
        public const UInt32 MMSYSERR_INVALHANDLE  = MMSYSERR_BASE + 5;
        public const UInt32 MMSYSERR_NODRIVER     = MMSYSERR_BASE + 6;
        public const UInt32 MMSYSERR_NOMEM        = MMSYSERR_BASE + 7;
        public const UInt32 MMSYSERR_NOTSUPPORTED = MMSYSERR_BASE + 8;
        public const UInt32 MMSYSERR_BADERRNUM    = MMSYSERR_BASE + 9;
        public const UInt32 MMSYSERR_INVALFLAG    = MMSYSERR_BASE + 10;
        public const UInt32 MMSYSERR_INVALPARAM   = MMSYSERR_BASE + 11;
        public const UInt32 MMSYSERR_HANDLEBUSY   = MMSYSERR_BASE + 12;
        public const UInt32 MMSYSERR_INVALIDALIAS = MMSYSERR_BASE + 13;
        public const UInt32 MMSYSERR_BADDB        = MMSYSERR_BASE + 14;
        public const UInt32 MMSYSERR_KEYNOTFOUND  = MMSYSERR_BASE + 15;
        public const UInt32 MMSYSERR_READERROR    = MMSYSERR_BASE + 16;
        public const UInt32 MMSYSERR_WRITEERROR   = MMSYSERR_BASE + 17;
        public const UInt32 MMSYSERR_DELETEERROR  = MMSYSERR_BASE + 18;
        public const UInt32 MMSYSERR_VALNOTFOUND  = MMSYSERR_BASE + 19;
        public const UInt32 MMSYSERR_NODRIVERCB   = MMSYSERR_BASE + 20;
        public const UInt32 MMSYSERR_MOREDATA     = MMSYSERR_BASE + 21;
        public const UInt32 MMSYSERR_LASTERROR    = MMSYSERR_BASE + 21;

        // MIDI-specific return codes.
        public const UInt32 MIDIERR_BASE          = 64;
        public const UInt32 MIDIERR_UNPREPARED    = MIDIERR_BASE + 0;
        public const UInt32 MIDIERR_STILLPLAYING  = MIDIERR_BASE + 1;
        public const UInt32 MIDIERR_NOMAP         = MIDIERR_BASE + 2;
        public const UInt32 MIDIERR_NOTREADY      = MIDIERR_BASE + 3;
        public const UInt32 MIDIERR_NODEVICE      = MIDIERR_BASE + 4;
        public const UInt32 MIDIERR_INVALIDSETUP  = MIDIERR_BASE + 5;
        public const UInt32 MIDIERR_BADOPENMODE   = MIDIERR_BASE + 6;
        public const UInt32 MIDIERR_DONT_CONTINUE = MIDIERR_BASE + 7;
        public const UInt32 MIDIERR_LASTERROR     = MIDIERR_BASE + 7;

        // Constants passed to midiInOpen() and midiOutOpen() to specify the kind of callback to use.
        public const UInt32 CALLBACK_TYPEMASK = 0x70000;
        public const UInt32 CALLBACK_NULL     = 0x00000;
        public const UInt32 CALLBACK_WINDOW   = 0x10000;
        public const UInt32 CALLBACK_TASK     = 0x20000;
        public const UInt32 CALLBACK_FUNCTION = 0x30000;
        public const UInt32 CALLBACK_THREAD   = CALLBACK_TASK;
        public const UInt32 CALLBACK_EVENT    = 0x50000;
        public const UInt32 MIDI_IO_STATUS    = 0x00020;

        // Flags for wTechnology field of MIDIOUTCAPS structure.
        public const UInt32 MOD_MIDIPORT  = 1;
        public const UInt32 MOD_SYNTH     = 2;
        public const UInt32 MOD_SQSYNTH   = 3;
        public const UInt32 MOD_FMSYNTH   = 4;
        public const UInt32 MOD_MAPPER    = 5;
        public const UInt32 MOD_WAVETABLE = 6;
        public const UInt32 MOD_SWSYNTH   = 7;

        // Flags for dwSupport field of MIDIOUTCAPS structure.
        public const UInt32 MIDICAPS_VOLUME   = 0x0001;
        public const UInt32 MIDICAPS_LRVOLUME = 0x0002;
        public const UInt32 MIDICAPS_CACHE    = 0x0004;
        public const UInt32 MIDICAPS_STREAM   = 0x0008;

        // "Midi Out Messages", passed to wMsg param of MidiOutProc.
        public const UInt32 MOM_OPEN  = 0x3C7;
        public const UInt32 MOM_CLOSE = 0x3C8;
        public const UInt32 MOM_DONE  = 0x3C9;

        // "Midi In Messages", passed to wMsg param of MidiInProc.
        public const UInt32 MIM_OPEN      = 0x3C1;
        public const UInt32 MIM_CLOSE     = 0x3C2;
        public const UInt32 MIM_DATA      = 0x3C3;
        public const UInt32 MIM_LONGDATA  = 0x3C4;
        public const UInt32 MIM_ERROR     = 0x3C5;
        public const UInt32 MIM_LONGERROR = 0x3C6;
        public const UInt32 MIM_MOREDATA  = 0x3CC;

        #endregion

        #region Functions and structs for MIDI Output Devices

        /// <summary>
        /// Returns the number of MIDI output devices on this system.
        /// </summary>
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711627(VS.85).aspx
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern UInt32 midiOutGetNumDevs();

        /// <summary>
        /// Struct representing the capabilities of an output device. 
        /// </summary>
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711619(VS.85).aspx
        [StructLayout(LayoutKind.Sequential)]
        public struct MIDIOUTCAPS
        {
            public UInt16 wMid;
            public UInt16 wPid;
            public UInt32 vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)MAXPNAMELEN)]
            public string szPname;
            public UInt16 wTechnology;
            public UInt16 wVoices;
            public UInt16 wNotes;
            public UInt16 wChannelMask;
            public UInt32 dwSupport;
        }

        /// <summary>
        /// Fills in the capabilities struct for a specific output device.
        /// </summary>
        /// NOTE: This is adapted from the original Win32 function in order to make it typesafe.
        ///
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711621(VS.85).aspx
        public static UInt32 midiOutGetDevCaps(UIntPtr uDeviceID, out MIDIOUTCAPS caps)
        {
            return midiOutGetDevCaps(uDeviceID, out caps, (UInt32)Marshal.SizeOf(typeof(MIDIOUTCAPS)));
        }

        /// <summary>
        /// Win32 handle for a MIDI output device.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct HMIDIOUT
        {
            public Int32 handle;
        }

        /// <summary>
        /// Callback invoked when a MIDI output device is opened, closed, or finished with a buffer.
        /// </summary>
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711637(VS.85).aspx
        public delegate void MidiOutProc(HMIDIOUT hmo, UInt32 wMsg, UIntPtr dwInstance, UIntPtr dwParam1, UIntPtr dwParam2);

        /// <summary>
        /// Opens a MIDI output device.
        /// </summary>
        /// NOTE: This is adapted from the original Win32 function in order to make it typesafe.
        ///
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711632(VS.85).aspx
        public static UInt32 midiOutOpen(out HMIDIOUT lphmo, UIntPtr uDeviceID,
                                         MidiOutProc dwCallback, UIntPtr dwCallbackInstance)
        {
            return midiOutOpen(out lphmo, uDeviceID, dwCallback, dwCallbackInstance,
                dwCallback == null ? CALLBACK_NULL : CALLBACK_FUNCTION);
        }

        /// <summary>
        /// Turns off all notes and sustains on a MIDI output device.
        /// </summary>
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/dd798479(VS.85).aspx
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern UInt32 midiOutReset(HMIDIOUT hmo);

        /// <summary>
        /// Closes a MIDI output device.
        /// </summary>
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711620(VS.85).aspx
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern UInt32 midiOutClose(HMIDIOUT hmo);

        /// <summary>
        /// Sends a short MIDI message (anything but sysex or stream).
        /// </summary>
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711640(VS.85).aspx
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern UInt32 midiOutShortMsg(HMIDIOUT hmo, UInt32 dwMsg);

        /// <summary>
        /// Gets the error text for a return code related to an output device.
        /// </summary>
        /// NOTE: This is adapted from the original Win32 function in order to make it typesafe.
        ///
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711622(VS.85).aspx
        public static UInt32 midiOutGetErrorText(UInt32 mmrError, StringBuilder lpText)
        {
            return midiOutGetErrorText(mmrError, lpText, (UInt32)lpText.Capacity);
        }

        #endregion

        #region Functions and structs for MIDI Input Devices

        /// <summary>
        /// Returns the number of MIDI input devices on this system.
        /// </summary>
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711608(VS.85).aspx
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern UInt32 midiInGetNumDevs();

        /// <summary>
        /// Struct representing the capabilities of an input device. 
        /// </summary>
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711596(VS.85).aspx
        [StructLayout(LayoutKind.Sequential)]
        public struct MIDIINCAPS
        {
            public UInt16 wMid;
            public UInt16 wPid;
            public UInt32 vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)MAXPNAMELEN)]
            public string szPname;
            public UInt32 dwSupport;
        }

        /// <summary>
        /// Fills in the capabilities struct for a specific input device.
        /// </summary>
        /// NOTE: This is adapted from the original Win32 function in order to make it typesafe.
        ///
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711604(VS.85).aspx
        public static UInt32 midiInGetDevCaps(UIntPtr uDeviceID, out MIDIINCAPS caps)
        {
            return midiInGetDevCaps(uDeviceID, out caps, (UInt32)Marshal.SizeOf(typeof(MIDIINCAPS)));
        }

        /// <summary>
        /// Win32 handle for a MIDI input device.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct HMIDIIN
        {
            public Int32 handle;
        }

        /// <summary>
        /// Callback invoked when a MIDI event is received from an input device.
        /// </summary>
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711612(VS.85).aspx
        public delegate void MidiInProc(HMIDIIN hMidiIn, UInt32 wMsg, UIntPtr dwInstance, UIntPtr dwParam1, UIntPtr dwParam2);

        /// <summary>
        /// Opens a MIDI input device.
        /// </summary>
        /// NOTE: This is adapted from the original Win32 function in order to make it typesafe.
        ///
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711610(VS.85).aspx
        public static UInt32 midiInOpen(out HMIDIIN lphMidiIn, UIntPtr uDeviceID,
                                         MidiInProc dwCallback, UIntPtr dwCallbackInstance)
        {
            return midiInOpen(out lphMidiIn, uDeviceID, dwCallback, dwCallbackInstance,
                dwCallback == null ? CALLBACK_NULL : CALLBACK_FUNCTION);
        }

        /// <summary>
        /// Starts input on a MIDI input device.
        /// </summary>
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711614(VS.85).aspx
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern UInt32 midiInStart(HMIDIIN hMidiIn);

        /// <summary>
        /// Stops input on a MIDI input device.
        /// </summary>
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711615(VS.85).aspx
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern UInt32 midiInStop(HMIDIIN hMidiIn);

        /// <summary>
        /// Resets input on a MIDI input device.
        /// </summary>
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711613(VS.85).aspx
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern UInt32 midiInReset(HMIDIIN hMidiIn);

        /// <summary>
        /// Closes a MIDI input device.
        /// </summary>
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711602(VS.85).aspx
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern UInt32 midiInClose(HMIDIIN hMidiIn);

        /// <summary>
        /// Gets the error text for a return code related to an input device.
        /// </summary>
        /// NOTE: This is adapted from the original Win32 function in order to make it typesafe.
        ///
        /// Win32 docs: http://msdn.microsoft.com/en-us/library/ms711605(VS.85).aspx
        public static UInt32 midiInGetErrorText(UInt32 mmrError, StringBuilder lpText)
        {
            return midiInGetErrorText(mmrError, lpText, (UInt32)lpText.Capacity);
        }

        #endregion

        #region Non-Typesafe Bindings

        // The bindings in this section are not typesafe, so we make them private and privide typesafe variants above.

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern UInt32 midiOutGetDevCaps(UIntPtr uDeviceID, out MIDIOUTCAPS caps, UInt32 cbMidiOutCaps);

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern UInt32 midiOutOpen(out HMIDIOUT lphmo, UIntPtr uDeviceID,
            MidiOutProc dwCallback, UIntPtr dwCallbackInstance, UInt32 dwFlags);

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern UInt32 midiOutGetErrorText(UInt32 mmrError, StringBuilder lpText, UInt32 cchText);

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern UInt32 midiInGetDevCaps(UIntPtr uDeviceID, out MIDIINCAPS caps, UInt32 cbMidiInCaps);

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern UInt32 midiInOpen(out HMIDIIN lphMidiIn, UIntPtr uDeviceID,
            MidiInProc dwCallback, UIntPtr dwCallbackInstance, UInt32 dwFlags);

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern UInt32 midiInGetErrorText(UInt32 mmrError, StringBuilder lpText, UInt32 cchText);

        #endregion    
    }
}