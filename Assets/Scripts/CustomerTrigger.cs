using UnityEngine;

public class CustomerTrigger : MonoBehaviour
{
    [SerializeField] private GameObject currentCustomer;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Customer")
        {
            currentCustomer = other.gameObject;
        }
        UpdateCustomerState();
    }
    
    private void UpdateCustomerState()
    {
        Animator animator;
        Customer customer;
        if (currentCustomer != null)
        {
            animator = currentCustomer.GetComponent<Animator>();
            customer = currentCustomer.GetComponent<Customer>();
            animator.avatar = customer.idleAvatar;
            animator.SetBool("Idle", true);
        }
        currentCustomer.transform.Rotate(Vector3.up, 90f, Space.World);
    }
}
