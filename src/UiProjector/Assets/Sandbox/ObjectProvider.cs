#if UNITY_EDITOR
using UnityEngine;

namespace UiProjector.Sandbox
{
    /// <summary>
    /// 追従対象の3Dオブジェクトを生成・提供する
    /// </summary>
    public class ObjectProvider : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField, Min(0)] private int _createCount = 20;
        [SerializeField, Min(0)] private float _radius = 10f;

        private GameObject[] _objects;

        private void Awake()
        {
            // 半径内のランダムな位置にオブジェクトを生成
            _objects = new GameObject[_createCount];
            for (var i = 0; i < _createCount; i++)
            {
                var obj = Instantiate(_prefab, Vector3.zero, Quaternion.identity, transform);
                obj.transform.localPosition = Random.insideUnitSphere * _radius;
                obj.transform.SetParent(transform);
                _objects[i] = obj;
            }
        }

        /// <summary>
        /// ランダムなオブジェクトを取得
        /// </summary>
        public GameObject GetRandom()
        {
            return _objects[Random.Range(0, _objects.Length)];
        }
    }
}
#endif