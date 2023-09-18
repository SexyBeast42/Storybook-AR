using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonColor : MonoBehaviour
{
    private Button button;
    private Color originalColor;
    public Color pressedColor = Color.red; // Set the desired pressed color in the Inspector

    private void Start()
    {
        button = GetComponent<Button>();

        // Store the original color of the button
        originalColor = button.colors.normalColor;

        // Attach a listener to the button's click event
        // button.onClick.AddListener(ChangeColor);
    }

    public void ChangeColor()
    {
        // Change the button's color to the pressed color when clicked
        ColorBlock colors = button.colors;
        colors.normalColor = pressedColor;
        button.colors = colors;
    }

    public void ResetButtonColor()
    {
        // Reset the button's color to the original color
        ColorBlock colors = button.colors;
        colors.normalColor = originalColor;
        button.colors = colors;
    }
}