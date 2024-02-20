using FishNet.Object.Prediction;
using UnityEngine;

// Extracted from fish-net official documentation and adapted to fit the game 
// https://fish-networking.gitbook.io/docs/manual/guides/prediction/version-2-experimental/getting-started
public struct MoveData : IReplicateData
{
    public bool jump;
    public bool hitstun;
    public float horizontal;
    public float vertical;
    public MoveData(bool jump, bool beingHit, float horizontal, float vertical)
    {
        this.jump = jump;
        this.hitstun = beingHit;
        this.horizontal = horizontal;
        this.vertical = vertical;
        _tick = 0;
    }

    private uint _tick;
    public void Dispose() { }
    public uint GetTick() => _tick;
    public void SetTick(uint value) => _tick = value;
}

public struct ReconcileData : IReconcileData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Velocity;
    public Vector3 AngularVelocity;
    public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
    {
        Position = position;
        Rotation = rotation;
        Velocity = velocity;
        AngularVelocity = angularVelocity;
        _tick = 0;
    }

    private uint _tick;
    public void Dispose() { }
    public uint GetTick() => _tick;
    public void SetTick(uint value) => _tick = value;
}