using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WebSocketSharp;

public class Touchpad : MonoBehaviour {
  /// ////////////////////////////////////
  /// /// //////////// API ///////////////  
  /// ////////////////////////////////////
  public string uri;

  public delegate void Touch(TouchPoint t);

  public static event Touch OnTouch;

  public delegate void Click(TouchPoint t);

  public static event Click OnClick;

  public delegate void DblClick(TouchPoint t);

  public static event DblClick OnDblClick;

  public delegate void Scale(float delta);

  public static event Scale OnScale;

  public delegate void Translate(Vector2 delta);

  public static event Translate OnTranslate;

  public delegate void Rotate(Vector2 direction, float delta);

  public static event Rotate OnRotate;

  /// ////////////////////////////////////
  /// //////////// END API ///////////////
  /// ////////////////////////////////////
  private readonly Queue<SimultaneousTouchPoints> _touchPoints = new();

  private WebSocket _ws;

  private void Start() {
    _ws = new WebSocket($"ws://{uri}");
    _ws.OnOpen += (sender, args) => { Debug.Log("Connected."); };
    _ws.OnClose += (sender, args) => { Debug.Log("Disconnected."); };
    _ws.OnMessage += (sender, e) => {
      var rawData = e.Data.Trim('[', ']').Split(',');
      var data = new int[rawData.Length];
      for (int i = 0; i < rawData.Length; i++) {
        data[i] = int.Parse(rawData[i]);
      }

      _touchPoints.Enqueue(new SimultaneousTouchPoints(data));
    };

    _ws.Connect();
    StartCoroutine(SemanticHandler());
  }


  private IEnumerator SemanticHandler() {
    bool firstTouch = true;

    float initialDistanceFingers = 0;
    Vector2 initialCenter = Vector2.zero;

    int y = 0;
    while (true) {
      // FIXME - refactor once poc is hover
      // this allows to handle all new inputs on the main thread. 
      if (y == 5) {
        y = 0;
        yield return null;
      }
      else y++;

      if (_touchPoints.Count == 0) continue;
      var points = _touchPoints.Dequeue();

      for (var i = 0; i < 10; i++) {
        var t = points.Data[i];
        if (t == null) break;
        OnTouch?.Invoke((TouchPoint)t);
      }

      if (points.Data[0] != null && points.Data[1] != null) {
        var t1 = (TouchPoint)points.Data[0];
        var t2 = (TouchPoint)points.Data[1];
        if (firstTouch) {
          firstTouch = false;
          initialDistanceFingers = Vector2.Distance(t1.coordinates, t2.coordinates);
          initialCenter = (t1.coordinates + t2.coordinates) / 2f;
        }

        // Scale
        var distanceFingers = Vector2.Distance(t1.coordinates, t2.coordinates);
        var diffDistance = distanceFingers - initialDistanceFingers;
        if (diffDistance != 0) OnScale?.Invoke(diffDistance);
        initialDistanceFingers = distanceFingers;

        // Rotate
        var currentCenter = (t1.coordinates + t2.coordinates) / 2f;
        var directionCenter = (initialCenter - currentCenter).normalized;
        if (directionCenter != Vector2.zero) {
          var delta = Vector2.Distance(currentCenter, initialCenter);
          OnRotate?.Invoke(directionCenter, delta);
        }

        initialCenter = currentCenter;

        // Translate
        // TODO
        if (t1.isLast || t2.isLast) firstTouch = true;
      }
    }
  }
}

public class SimultaneousTouchPoints {
  public readonly TouchPoint?[] Data = new TouchPoint?[10];

  public SimultaneousTouchPoints(int[] dataFromHid) {
    for (int index = 1, count = 0; index < dataFromHid.Length; index += 6, count++) {
      var touchPointData = new int[6];
      Array.Copy(dataFromHid, index, touchPointData, 0, 6);
      if (touchPointData.Length < 6) break;
      if (touchPointData.Sum() == 0) break;
      Data[count] = new TouchPoint(touchPointData);
    }
  }
}

public readonly struct TouchPoint {
  public int id { get; }

  public bool isLast { get; }

  public Vector2 coordinates { get; }

  public TouchPoint(IReadOnlyList<int> raw) {
    id = raw[1];
    isLast = raw[0] == 4;
    var x = ((raw[3] + raw[2] / 256.0f) / 16f);
    var y = ((raw[5] + raw[4] / 256.0f) / 16f);
    coordinates = new Vector2(x, y);
  }

  public override string ToString() {
    return $"Touch {id} on ({coordinates.x}, {coordinates.y})";
  }
}