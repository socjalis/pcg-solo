using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public GameObject parameters;
    public GameObject mainMenu;
    public GameObject options;


    private void Start()
    {
        GetComponentInChildren<Dropdown>().value = 1;
    }

    public void onAlgorithmDropdownChange(Dropdown dropdown)
    {
        string option = dropdown.captionText.text;

       
        GameObject param = GameObject.Find("Param");

        foreach (Transform child in parameters.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        if (option == "Random")
        {
            Algorithms.current = new RandomAlgorithm();
        }
        else if (option == "BSP")
        {
            Algorithms.current = new BSPAlgorithm();
        }
        else
        {
            Algorithms.current = null;
        }

        if (Algorithms.current != null)
        {
            foreach (string key in Algorithms.current.getParameters().Keys)
            {
                GameObject obj = Instantiate(param, transform.position, transform.rotation);
                obj.transform.SetParent(parameters.transform);

                obj.GetComponentInChildren<Text>().text = key;
            }
        }
    }

    public void onOkClicked()
    {
        SortedDictionary<string, string> paramMap = new SortedDictionary<string, string>();
        foreach(Transform child in parameters.transform)
        {
            string key = child.gameObject.GetComponentInChildren<Text>().text;
            string value = child.gameObject.GetComponentInChildren<InputField>().text;
            paramMap.Add(key, value);
        }
        Algorithms.current.setParameters(paramMap);
        options.SetActive(false);
        mainMenu.SetActive(true);
    }
}
