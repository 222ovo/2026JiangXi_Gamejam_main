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
        if (currentCustomer != null)
        {
            currentCustomer.GetComponent<Customer>().StartOrdering();
        }
    }
}
