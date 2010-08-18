package net.alphatab.midi;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.io.*;

import javax.swing.JOptionPane;

import javax.sound.midi.ControllerEventListener;
import javax.sound.midi.InvalidMidiDataException;
import javax.sound.midi.MidiSystem;
import javax.sound.midi.MidiUnavailableException;
import javax.sound.midi.Sequence;
import javax.sound.midi.Sequencer;
import javax.sound.midi.ShortMessage;
import javax.sound.midi.Transmitter;
import javax.swing.BorderFactory;
import javax.swing.JApplet;
import javax.swing.JLabel;

import netscape.javascript.JSObject;

public class MidiPlayer extends JApplet
{
    private Sequence          _sequence;
    private Sequencer         _sequencer;
    private long              _lastTick;
    private String            _updateFunction;
	private int 			  _metronomeTrack;

    @Override
    public void init()
    {
        super.init();
        _updateFunction = getParameter("onTickChanged");
        try
        {
            _sequencer = MidiSystem.getSequencer();
            _sequencer.open();

            Transmitter tickTransmitter = _sequencer.getTransmitter();
            TickNotifierReceiver tickReceiver = new TickNotifierReceiver(
                    tickTransmitter.getReceiver());
            tickTransmitter.setReceiver(tickReceiver);

            tickReceiver
                    .addControllerEventListener(new ControllerEventListener()
                    {
                        @Override
                        public void controlChange(ShortMessage event)
                        {
                            if (_sequencer.isRunning())
                            {
                                switch (event.getCommand())
                                {
                                    case 0x80:// Noteon
                                    case 0x90:// noteof
                                        notifyPosition(_sequencer
                                                .getTickPosition());
                                    break;
                                }
                            }
                        }
                    });
        }
        catch (MidiUnavailableException e)
        {
            e.printStackTrace();
        }
    }

    private void notifyPosition(long tickPosition)
    {
        if (_lastTick == tickPosition || _updateFunction == null) return;
        JSObject.getWindow(this).call(_updateFunction,
                new String[] { new Long(tickPosition).toString() });
    }

    public void updateSongData(String commands)
    {
        try
        {
            _sequence = MidiSequenceParser.parse(commands);
            _sequencer.setSequence(_sequence);
			_metronomeTrack = MidiSequenceParser.getMetronomeTrack();
        }
        catch (Throwable e)
        {
			e.printStackTrace();
        }
    }
	
	public void setMetronomeEnabled(boolean enabled) 
	{
		_sequencer.setTrackMute(_metronomeTrack, !enabled);
	}

	public void isMetronomeEnabled() 
	{
		_sequencer.getTrackMute(_metronomeTrack);
	}

    public void play()
    {
        _sequencer.start();
    }
 
    public void pause()
    {
        _sequencer.stop();
    }
    
    public void stop()
    {
        _sequencer.stop();
        _sequencer.setTickPosition(0);
    }

}
