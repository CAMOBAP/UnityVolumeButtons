package org.camobap.unity3d;

import java.util.List;
import java.util.ArrayList;

import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.content.Context;
import android.media.AudioManager;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class VolumeButtonsActivity extends UnityPlayerActivity {
  /**
   * Keep in sync with VolumeButtonsEvent in C#
   */
  private enum Event {
    VOLUME_DOWN(-1),
    VOLUME_MUTE(0),
    VOLUME_UP(+1);

    private final String mValue;
    private Event(int type) {
      mValue = Integer.toString(type);
    }

    public String getValue() {
      return mValue;
    }
  }

  private static final String TAG = "VolumeButtonsActivity";
  private static final String MESSAGE_NAME = "_OnVolumeButtonEvent";
  private List<String> gameObjectNames = new ArrayList<>();

  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    Log.d(TAG, "onCreate called!");
  }

  public void addGameObjectListener(String gameObjectName) {
    synchronized (gameObjectNames) {
      gameObjectNames.add(gameObjectName);
      Log.d(TAG, "addGameObjectListener " + gameObjectName);
    }
  }

  public void removeGameObjectListener(String gameObjectName) {
    synchronized (gameObjectNames) {
      gameObjectNames.remove(gameObjectName);
      Log.d(TAG, "removeGameObjectListener " + gameObjectName);
    }
  }
  
  @Override
  public boolean dispatchKeyEvent(KeyEvent event) {
    int action = event.getAction();
    int keyCode = event.getKeyCode();
    switch (keyCode) {
    case KeyEvent.KEYCODE_VOLUME_UP:
      if (action == KeyEvent.ACTION_DOWN) {
        for (String gameObjectName : gameObjectNames) {
          UnityPlayer.UnitySendMessage(gameObjectName, MESSAGE_NAME, Event.VOLUME_UP.getValue());
        }
      }
      break;
    case KeyEvent.KEYCODE_VOLUME_DOWN:
      if (action == KeyEvent.ACTION_DOWN) {
        for (String gameObjectName : gameObjectNames) {
          UnityPlayer.UnitySendMessage(gameObjectName, MESSAGE_NAME, Event.VOLUME_DOWN.getValue());
        }
      }
      break;
    }

    return super.dispatchKeyEvent(event);
  }

  public float getSystemVolumeLevel() {
    AudioManager audioManager = (AudioManager) getSystemService(Context.AUDIO_SERVICE);
    if (audioManager == null) {
        return -1.f;
    }

    Log.d(TAG, "getSystemVolumeLevel called!");

    // https://stackoverflow.com/questions/43886901/how-to-convert-android-stream-volume-to-a-value-between-0-and-1
    int currentVolume = audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
    int maxVolume = audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);

    return currentVolume / (float) maxVolume;
  }
}
