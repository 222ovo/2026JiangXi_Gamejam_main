using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Avatar walkingAvatar;
    [SerializeField] private Avatar idleAvatar;

    [SerializeField] private GameObject requestPlank;
    [SerializeField] private TextAsset tasteText;
    
    [SerializeField] private Transform requestPlankTransform;

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        
    }
    
    public void StartOrdering()
    {
        animator.avatar = idleAvatar;
        animator.SetBool("Idle",true);
        transform.Rotate(Vector3.up, 90f, Space.World);
        GameObject obj = Instantiate(requestPlank, requestPlankTransform.position, requestPlankTransform.rotation);

        TextMeshPro tmp = obj.GetComponentInChildren<TextMeshPro>(true);
        tmp.text = tasteText.text;
    }
}
