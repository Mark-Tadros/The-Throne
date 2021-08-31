// Initialises the title scene after LoadMenu.cs and GameManager.cs run.
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public GameManager gameManagerScript;
    public bool firstLoading = false; public bool secondLoading = false; bool thirdLoading = false;
    // Plays an initial cutscene.
    IEnumerator Start()
    {
        // Initialises the game and saving settings.
        int randomQuote = Random.Range(1, 6);
        yield return new WaitUntil(() => firstLoading && secondLoading);
        // Initialises the quote after the settings.
        thirdLoading = true;
        LeanTween.alphaCanvas(transform.GetChild(randomQuote).GetChild(0).GetComponent<CanvasGroup>(), 1, 1.5f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(2f);
        LeanTween.alphaCanvas(transform.GetChild(randomQuote).GetChild(1).GetComponent<CanvasGroup>(), 1, 1.25f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(1.75f);
        LeanTween.alphaCanvas(transform.GetChild(randomQuote).GetChild(2).GetComponent<CanvasGroup>(), 1, 1f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(2f);
        thirdLoading = false;
        LeanTween.alphaCanvas(transform.GetChild(randomQuote).GetComponent<CanvasGroup>(), 0, 1f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    // Allows the user to skip the initial quote.
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && firstLoading && secondLoading && thirdLoading)
        {
            thirdLoading = false;
            StopAllCoroutines();
            StartCoroutine(SkipStart());
        }
    }
    IEnumerator SkipStart()
    {
        for (int i = 1; i < 6; i++) LeanTween.alphaCanvas(transform.GetChild(i).GetComponent<CanvasGroup>(), 0, 0.25f).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}