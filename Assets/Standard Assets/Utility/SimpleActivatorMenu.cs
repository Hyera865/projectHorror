using System;
using UnityEngine;
using UnityEngine.UI;  // Impor namespace UI untuk menggunakan Text

#pragma warning disable 618
namespace UnityStandardAssets.Utility
{
    public class SimpleActivatorMenu : MonoBehaviour
    {
        // Ubah GUIText menjadi Text untuk menggunakan UI yang baru
        public Text camSwitchButton;  // Menggunakan UI.Text
        public GameObject[] objects;

        private int m_CurrentActiveObject;

        private void OnEnable()
        {
            // Object aktif dimulai dari yang pertama di array
            m_CurrentActiveObject = 0;
            camSwitchButton.text = objects[m_CurrentActiveObject].name;
        }

        public void NextCamera()
        {
            int nextActiveObject = m_CurrentActiveObject + 1 >= objects.Length ? 0 : m_CurrentActiveObject + 1;

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(i == nextActiveObject);
            }

            m_CurrentActiveObject = nextActiveObject;
            camSwitchButton.text = objects[m_CurrentActiveObject].name;
        }
    }
}
