using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectPool : MonoBehaviour
{
    // 0 - Skill Slot
    public static ObjectPool s_sharedInstance;
    public List<List<GameObject>> PooledObjects = new List<List<GameObject>>();
    [SerializeField]
    private List<string> _objectCode = new List<string>();
    [SerializeField]
    private List<GameObject> _objectToPool = new List<GameObject>();
    [SerializeField]
    private List<int> _amountToPool = new List<int>();

    void Awake() => s_sharedInstance = this;

    void Start()
    {
        GameObject tmp;
        for (int i = 0; i < _amountToPool.Count; i++)
        {
            List<GameObject> lst = new List<GameObject>();
            for (int j = 0; j < _amountToPool[i]; j++)
            {
                tmp = Instantiate(_objectToPool[i]);
                tmp.SetActive(false);
                lst.Add(tmp);
            }
            PooledObjects.Add(lst);
        }
    }

    public GameObject GetPooledObject(string item, Transform parent)
    {
        int index = _objectCode.IndexOf(item);
        for (int i = PooledObjects[index].Count - 1; i >= 0; i--)
        {
            if(PooledObjects[index][i] == null)
                PooledObjects[index].RemoveAt(i);
            else if(!PooledObjects[index][i].activeInHierarchy)
            {
                PooledObjects[index][i].SetActive(true);
                PooledObjects[index][i].transform.SetParent(parent);
                if (PooledObjects[index][i].GetComponent<Button>() != null)
                    PooledObjects[index][i].GetComponent<Button>().onClick.RemoveAllListeners();
                if (PooledObjects[index][i].GetComponent<RectTransform>() != null)
                    PooledObjects[index][i].GetComponent<RectTransform>().localScale = new Vector2(1, 1);

                foreach (Transform child in PooledObjects[index][i].transform)
                {
                    child.gameObject.SetActive(true);
                    if(child.GetComponent<Button>() != null)
                        child.GetComponent<Button>().onClick.RemoveAllListeners();
                }
                return PooledObjects[index][i];
            }
        }
        // If all objects are active, it means none are left. So create new one.
        GameObject tmp = Instantiate(_objectToPool[index]);
        PooledObjects[index].Add(tmp);
        tmp.transform.SetParent(parent);
        if (tmp.GetComponent<RectTransform>() != null)
            tmp.GetComponent<RectTransform>().localScale = new Vector2(1, 1);

        return tmp;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ObjectPool))]
    public class ObjectPoolEditor : Editor
    {
        ObjectPool comp;

        public void OnEnable() =>   comp = (ObjectPool)target;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name");
            EditorGUILayout.LabelField("Object");
            EditorGUILayout.LabelField("Amount");
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("+"))
            {
                comp._objectCode.Add("");
                comp._objectToPool.Add(null);
                comp._amountToPool.Add(0);
            }

            for (int i = 0; i < comp._objectToPool.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                comp._objectCode[i] = EditorGUILayout.TextField(comp._objectCode[i]);
                comp._objectToPool[i] = (GameObject)EditorGUILayout.ObjectField(comp._objectToPool[i], typeof(GameObject), false);
                comp._amountToPool[i] = EditorGUILayout.IntField(comp._amountToPool[i]);
                if (GUILayout.Button("-"))
                {
                    comp._objectCode.RemoveAt(i);
                    comp._objectToPool.RemoveAt(i);
                    comp._amountToPool.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorUtility.SetDirty(comp);
        }
    }
#endif
}
