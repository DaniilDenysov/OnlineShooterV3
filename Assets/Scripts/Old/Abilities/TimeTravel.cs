using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class TimeTravel : Ability
{
    [Range(0, 10000)]
    [SerializeField] private float _travelSpeed, _recordRate = 1f,_travelTime=10f;
    [Range(0,10000)]
    [SerializeField] private int _history;
    [SerializeField] private UnityEvent OnTravel;
    [SyncVar]
    [SerializeField]private List<Position> _positionHistory = new List<Position>();
    private float _time,_travelStarted;
    [SyncVar]
    private bool _isTravelling;
    private int _positionIndex;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody rb;
    private float _transferCD = 0.1f,_transferTime;

    [System.Serializable]
    struct Position
    {
        [SerializeField] private Vector3 _position;
        [SerializeField] private Quaternion _rotation;
        [SerializeField] private Vector2 _velocity;
        public Vector2 GetVelocity() => _velocity;
        public Vector3 GetPosition() => _position;
        public Quaternion GetRotation() => _rotation;
        public void SetPosition(Vector3 _position) => this._position = _position; 
        public void SetVelocity(Vector2 _velocity) => this._velocity = _velocity; 
        public void SetRotation(Quaternion _rotation) => this._rotation = _rotation; 
    }


    public override void FixedUpdate()
    {
        if (_isTravelling) return;
        Debug.Log("Not travelling!");
        base.FixedUpdate();
    }


    public void Update()
    {
        if (_isTravelling)
        {
            Debug.Log(_positionIndex);
            transform.position = Vector3.Lerp(transform.position, _positionHistory[_positionIndex].GetPosition(), _travelSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _positionHistory[_positionIndex].GetRotation(), _travelSpeed * Time.deltaTime);
            _animator.SetFloat("X", _positionHistory[_positionIndex].GetVelocity().x);
            _animator.SetFloat("Y", _positionHistory[_positionIndex].GetVelocity().y);
            if (!(Time.time - _transferTime  >= _transferCD)) return;
            OnTravel.Invoke();
            _transferTime = Time.time;
            _positionIndex--;
        
            if (_travelStarted-Time.time>=_travelTime || _positionIndex <= 0)
            {
                StopTravel();
            }
            return;
        }
        if (Time.time - _time < _recordRate) return;
        Position _position = new Position();
        _position.SetPosition(transform.position);
        _position.SetRotation(transform.rotation);
        _position.SetVelocity(new Vector2(rb.velocity.x, rb.velocity.y));
        _positionHistory.Add(_position);
        if (_positionHistory.Count > _history)
        {
            _positionHistory.RemoveAt(0);
        }
        _time = Time.time;
    }

 
    public void StartTravel ()
    {
        Debug.Log("TravelStarted");
        _positionIndex = _positionHistory.Count - 1;
        _travelStarted = Time.time;
        _transferTime = _travelStarted;
        _isTravelling = true;
    }


    public void StopTravel()
    {
        _isTravelling = false;
        _positionHistory.Clear();
    }

}
