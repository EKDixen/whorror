using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BoxButton : MonoBehaviour
{
    public List<string> allowedTags = new() { "Player", "Box" };
    public bool revertOnRelease = true;
    [Min(0f)] public float pressDelay = 0f;

    public List<Transform> targetsToMove = new();
    public List<Vector3> positionsA = new();
    public List<Vector3> positionsB = new();
    public bool useLocalPositions = false;
    [Min(0.01f)] public float moveDuration = 0.5f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public List<MonoBehaviour> pickableBehaviours = new();
    public bool pickableStateWhenPressed = false;

    private readonly HashSet<Collider2D> _currentPressers = new();
    private float _pressTimer = 0f;
    private bool _isActive = false;
    private Coroutine _moveRoutine;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void Awake()
    {
        CacheDefaultPositionsIfNeeded();
    }

    private void OnValidate()
    {
        if (targetsToMove == null) return;
        while (positionsA.Count < targetsToMove.Count) positionsA.Add(Vector3.zero);
        while (positionsB.Count < targetsToMove.Count) positionsB.Add(Vector3.zero);
    }

    private void CacheDefaultPositionsIfNeeded()
    {
        if (targetsToMove == null) return;
        for (int i = 0; i < targetsToMove.Count; i++)
        {
            var t = targetsToMove[i];
            if (!t) continue;

            if (i >= positionsA.Count) positionsA.Add(Vector3.zero);
            if (i >= positionsB.Count) positionsB.Add(Vector3.zero);

            if (positionsA[i] == Vector3.zero)
                positionsA[i] = useLocalPositions ? t.localPosition : t.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsAllowed(other)) return;
        _currentPressers.Add(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_currentPressers.Remove(other) && _currentPressers.Count == 0)
        {
            _pressTimer = 0f;
            SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!IsAllowed(other)) return;
        if (_currentPressers.Count == 0) return;

        if (!_isActive)
        {
            if (pressDelay <= 0f)
            {
                SetActive(true);
            }
            else
            {
                _pressTimer += Time.deltaTime;
                if (_pressTimer >= pressDelay)
                {
                    SetActive(true);
                }
            }
        }
    }

    private bool IsAllowed(Collider2D col)
    {
        if (!col) return false;
        if (allowedTags == null || allowedTags.Count == 0) return true;
        foreach (var tag in allowedTags)
        {
            if (!string.IsNullOrEmpty(tag) && col.CompareTag(tag)) return true;
        }
        return false;
    }

    private void SetActive(bool active)
    {
        if (_isActive == active) return;
        _isActive = active;

        foreach (var mb in pickableBehaviours)
        {
            if (!mb) continue;
            if (mb is IPickableToggle toggle)
            {
                toggle.SetPickable(active ? pickableStateWhenPressed : !pickableStateWhenPressed);
            }
            else
            {
                var behaviour = mb as Behaviour;
                if (behaviour)
                {
                    behaviour.enabled = active ? pickableStateWhenPressed : !pickableStateWhenPressed;
                }
            }
        }

        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(MoveTargets(active));
    }

    private IEnumerator MoveTargets(bool toB)
    {
        if (targetsToMove == null || targetsToMove.Count == 0) yield break;

        var fromList = toB ? positionsA : (revertOnRelease ? positionsB : positionsA);
        var toList = toB ? positionsB : (revertOnRelease ? positionsA : positionsA);

        float tElapsed = 0f;
        var startPositions = new Vector3[targetsToMove.Count];
        var endPositions = new Vector3[targetsToMove.Count];

        for (int i = 0; i < targetsToMove.Count; i++)
        {
            var t = targetsToMove[i];
            if (!t) continue;
            var from = i < fromList.Count ? fromList[i] : (useLocalPositions ? t.localPosition : t.position);
            var to = i < toList.Count ? toList[i] : (useLocalPositions ? t.localPosition : t.position);

            startPositions[i] = from;
            endPositions[i] = to;
        }

        while (tElapsed < moveDuration)
        {
            tElapsed += Time.deltaTime;
            float p = Mathf.Clamp01(tElapsed / moveDuration);
            float curved = moveCurve != null ? moveCurve.Evaluate(p) : p;

            for (int i = 0; i < targetsToMove.Count; i++)
            {
                var tr = targetsToMove[i];
                if (!tr) continue;
                var pos = Vector3.LerpUnclamped(startPositions[i], endPositions[i], curved);
                if (useLocalPositions) tr.localPosition = pos; else tr.position = pos;
            }
            yield return null;
        }

        for (int i = 0; i < targetsToMove.Count; i++)
        {
            var tr = targetsToMove[i];
            if (!tr) continue;
            var final = endPositions[i];
            if (useLocalPositions) tr.localPosition = final; else tr.position = final;
        }
    }
}

public interface IPickableToggle
{
    void SetPickable(bool canPick);
}
