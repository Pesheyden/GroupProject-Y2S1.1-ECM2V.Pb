using System;
using System.Collections;
using System.Collections.Generic;
using BSOAP.Variables;
using UnityEngine;

public class BSOAPExample : MonoBehaviour
{
    [Header("Variables")]
    public BoolVariable BoolVariable;
    public DoubleVariable DoubleVariable;
    public FloatVariable FloatVariable;
    public IntVariable IntVariable;
    public ObjectVariable ObjectVariable;
    public StringVariable StringVariable;
    public Vector2Variable Vector2Variable;
    public Vector3Variable Vector3Variable;
    public Vector4Variable Vector4Variable;

    [Header("Stats")] 
    public StatsExampleSO StatsExampleSo;

    [Header("Events")] 
    public BoolEventSo BoolEventSo;
    public FloatEventSo FloatEventSo;
    public IntEventSo IntEventSo;
    public ObjectEventSo ObjectEventSo;
    public StringEventSo StringEventSo;
    public Vector2EventSo Vector2EventSo;
    public Vector3EventSo Vector3EventSo;
    public Vector4EventSo Vector4EventSo;
}
