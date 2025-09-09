using BSOAP.Events;
using BSOAP.Variables;
using UnityEngine;

[CreateAssetMenu (menuName = "BSOAP/Examples/StatsExample")]
public class StatsExampleSO : ScriptableObject
{
    [SubAsset]
    public BoolVariable BoolVariable;
    [SubAsset]
    public DoubleVariable DoubleVariable;
    [SubAsset]
    public FloatVariable FloatVariable;
    [SubAsset]
    public IntVariable IntVariable;
    [SubAsset]
    public ObjectVariable ObjectVariable;
    [SubAsset]
    public Vector2Variable Vector2Variable;
    [SubAsset]
    public Vector3Variable Vector3Variable;
    [SubAsset]
    public Vector4Variable Vector4Variable;
    [SubAsset]
    public BoolEventSo BoolEventSo;
    [SubAsset]
    public StringEventSo StringEventSo;
    [SubAsset]
    public FloatEventSo FloatEventSo;
    [SubAsset]
    public IntEventSo IntEventSo;
    [SubAsset]
    public ObjectEventSo ObjectEventSo;
    [SubAsset]
    public Vector2EventSo Vector2EventSo;
    [SubAsset]
    public Vector3EventSo Vector3EventSo;
    [SubAsset]
    public Vector4EventSo Vector4EventSo;
    [SubAsset] 
    public CommandEventSo CommandEventSo;
}
