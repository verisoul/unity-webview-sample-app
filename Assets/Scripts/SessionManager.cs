using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Verisoul.Models;


namespace Verisoul
{
     public class SessionManager : MonoBehaviour
    {
        private UniWebView webView;
        public enum Environment { sandbox, prod }

        public bool debugMode = true;
        [Header("Verisoul API Settings")]
        public Environment env = Environment.sandbox;
        public string projectId;
        public string apiKey;
        public bool accountDetails = true;
        public bool sessionDetails = true;
        public int timeout = 10;
        [Header("UI Elements")]
        public TextMeshProUGUI projectIdText;
        public Button startSessionButton;
        public TextMeshProUGUI sessionIdText;
        public TMP_InputField accountIdText;
        public Button authenticateButton;
        public TextMeshProUGUI responseText;

        private static readonly HttpClient client = new HttpClient();

        void Start()
        {
            projectIdText.text = projectId;

            OnProjectIdChange();
            OnAccountIdChange();

            if (debugMode)
            {
                Debug.Log("SessionManager started");
                Debug.Log("apiKey: " + apiKey);
                Debug.Log("env: " + env);
            }
        }

        /*
         * load the webview and pass the configured project_id and uniwebview flag as true
         */
        void LoadSessionIdFromWebView()
        {
            GameObject webViewGameObject = new("uniWebView");
            webView = webViewGameObject.AddComponent<UniWebView>();

            string url = $"https://js.verisoul.ai/{env}/webview.html?project_id={projectIdText.text}&uniwebview=true";

            if (debugMode)
            {
                Debug.Log("url: " + url);
            }

            webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
            webView.Load(url);

            Coroutine webViewTimeout = StartCoroutine(WebViewTimeout(webViewGameObject, timeout));

            if (debugMode) {
                webView.Show();
            }

            // the webview will publish a message containing the session_id via uniwebview communication protocol 
            webView.OnMessageReceived += (view, message) =>
            {
                StopCoroutine(webViewTimeout);

                string RawMessage = message.RawMessage;
                string queryParams = RawMessage.Split('?')[1];

                string decodedUrlParams = HttpUtility.UrlDecode(queryParams);
                string sessionId = JsonUtility.FromJson<SessionData>(decodedUrlParams).session_id;

                if (debugMode)
                {
                    Debug.Log("sessionId: " + sessionId);
                }

                sessionIdText.color = new Color(0, 0.6415094f, 0.04044826f, 1);
                sessionIdText.text = sessionId;

                if (debugMode)
                {
                    webView.Hide();
                }

                Destroy(webViewGameObject);
            };
        }

        /*
         * it is possible the webview does not respond in which case you should close the webview and retry
         */
        IEnumerator WebViewTimeout(GameObject webViewGameObject, float timeout)
        {
            yield return new WaitForSeconds(timeout);

            if (debugMode) {
                Debug.LogError("WebView timeout");
                webView.Hide();
            }

            Destroy(webViewGameObject);
        }

        /*
         * call Verisoul's API from a secure backend server the configured API Key
         * optionally, pass query params account_detail and session_detail to see the full payload and fraud detection
         */
        async Task Authenticate() {
            AuthenticatePayload values = new()
            {
                session_id = sessionIdText.text,
                account = new Account { id = accountIdText.text }
            };

            string jsonContent = JsonUtility.ToJson(values);

            if (debugMode)
            {
                Debug.Log("jsonContent: " + jsonContent);
            }

            StringContent content = new (jsonContent, Encoding.UTF8, "application/json");

            if (debugMode)
            {
                Debug.Log("content: " + content.ToString());
            }
            
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);

            string apiUrl = $"https://api.{env}.verisoul.ai/session/authenticate";

            List<string> queryParams = new List<string>();
            if (accountDetails)
            {
                queryParams.Add("account_detail=1");
            }

            if (sessionDetails)
            {
                queryParams.Add("session_detail=1");
            }

            if (queryParams.Count > 0)
            {
                apiUrl += "?" + string.Join("&", queryParams);
            }

            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            string responseString = await response.Content.ReadAsStringAsync();

            if (debugMode)
            {
                Debug.Log("responseString: " + responseString);
            }

            responseText.text = responseString;
        }

        public void OnStartSessionClick()
        {
            LoadSessionIdFromWebView();    
        }

        public async void OnAuthenticateClick()
        {
            try
            {
                await Authenticate();
            }
            catch (HttpRequestException e)
            {
                responseText.text = e.Message;
                Debug.LogError(e.Message);
            }
             
        }

        public void OnProjectIdChange()
        {
            startSessionButton.interactable = !string.IsNullOrEmpty(projectIdText.text);
        }

        public void OnAccountIdChange()
        {
            authenticateButton.interactable = !string.IsNullOrEmpty(accountIdText.text);
        }
    }
}
