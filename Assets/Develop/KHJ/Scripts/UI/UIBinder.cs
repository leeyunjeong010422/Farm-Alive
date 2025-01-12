using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIBinder : MonoBehaviour
{
    [SerializeField] bool _includeComponents = true;
    [SerializeField] bool _includeInactive = true;

    private Dictionary<string, GameObject> gameObjectDic;
    private Dictionary<(string, System.Type), Component> componentDic;

    protected virtual void Awake()
    {
        if (_includeComponents)
            BindIncludingComponents();
        else
            Bind();
    }

    // 빠른 시간에 게임오브젝트만 바인딩
    protected void Bind()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(_includeInactive);
        // 2 * 2 크기를 4배로 일단.
        gameObjectDic = new Dictionary<string, GameObject>(transforms.Length << 2);
        foreach (Transform child in transforms)
        {
            gameObjectDic.TryAdd(child.gameObject.name, child.gameObject);
        }

        componentDic = new Dictionary<(string, System.Type), Component>();
    }

    // 비교적 오랜 시간에 게임오브젝트와 모든 컴포넌트 바인딩
    protected void BindIncludingComponents()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(_includeInactive);
        gameObjectDic = new Dictionary<string, GameObject>(transforms.Length << 2);

        foreach (Transform child in transforms)
        {
            gameObjectDic.TryAdd(child.gameObject.name, child.gameObject);
        }

        Component[] components = GetComponentsInChildren<Component>(_includeInactive);
        componentDic = new Dictionary<(string, System.Type), Component>(components.Length << 4);
        foreach (Component child in components)
        {
            componentDic.TryAdd((child.gameObject.name, components.GetType()), child);
        }
    }

    // 이름이 name인 UI 게임오브젝트 가져오기
    // GetUI("Key") : Key 이름의 게임오브젝트 가져오기
    public GameObject GetUI(in string name)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        return gameObject;
    }

    // 이름이 name인 UI에서 컴포넌트 T 가져오기
    // GetUI<Image>("Key") : Key 이름의 게임오브젝트에서 Image 컴포넌트 가져오기
    public T GetUI<T>(in string name) where T : Component
    {
        (string, System.Type) key = (name, typeof(T));

        //컴포넌트 딕셔너리에 이미 있을때(찾아본 적 있는 경우): 이미 찾은걸 줌.
        componentDic.TryGetValue(key, out Component component);
        if (component != null)
            return component as T;
        // 컴포넌트 딕셔너리에 아직 없다면 찾은 후 딕셔너리에 추가하고 줌.
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        if (gameObject == null)
            return null;

        component = gameObject.GetComponent<T>();
        if (component == null)
            return null;

        componentDic.TryAdd(key, component);
        return component as T;
    }

    public enum EventType { Click, Enter, Exit, Up, Down, Move, BeginDrag, EndDrag, Drag, Drop }
    public void AddEvent(in string name, EventType eventType, UnityAction<PointerEventData> callback)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        if (gameObject == null)
            return;

        EventReceiver receiver = gameObject.GetOrAddComponent<EventReceiver>();
        switch (eventType)
        {
            case EventType.Click:
                receiver.OnClicked += callback;
                break;
            case EventType.Enter:
                receiver.OnEntered += callback;
                break;
            case EventType.Exit:
                receiver.OnExited += callback;
                break;
            case EventType.Up:
                receiver.OnUped += callback;
                break;
            case EventType.Down:
                receiver.OnDowned += callback;
                break;
            case EventType.Move:
                receiver.OnMoved += callback;
                break;
            case EventType.BeginDrag:
                receiver.OnBeginDraged += callback;
                break;
            case EventType.EndDrag:
                receiver.OnEndDraged += callback;
                break;
            case EventType.Drag:
                receiver.OnDraged += callback;
                break;
            case EventType.Drop:
                receiver.OnDroped += callback;
                break;
        }
    }

    public void RemoveEvent(in string name, EventType eventType, UnityAction<PointerEventData> callback)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        if (gameObject == null)
            return;

        EventReceiver receiver = gameObject.GetOrAddComponent<EventReceiver>();
        switch (eventType)
        {
            case EventType.Click:
                receiver.OnClicked -= callback;
                break;
            case EventType.Enter:
                receiver.OnEntered -= callback;
                break;
            case EventType.Exit:
                receiver.OnExited -= callback;
                break;
            case EventType.Up:
                receiver.OnUped -= callback;
                break;
            case EventType.Down:
                receiver.OnDowned -= callback;
                break;
            case EventType.Move:
                receiver.OnMoved -= callback;
                break;
            case EventType.BeginDrag:
                receiver.OnBeginDraged -= callback;
                break;
            case EventType.EndDrag:
                receiver.OnEndDraged -= callback;
                break;
            case EventType.Drag:
                receiver.OnDraged -= callback;
                break;
            case EventType.Drop:
                receiver.OnDroped -= callback;
                break;
        }
    }
}